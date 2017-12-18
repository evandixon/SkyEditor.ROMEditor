Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Utilities

Namespace MysteryDungeon.PSMD
    Public Class FarcFat5
        Implements IOpenableFile
        Implements IOnDisk
        Implements ISavableAs

        Public Class Entry
            Public Property Index As Integer
            Public Property DataOffset As Integer
            Public Property DataLength As Integer
            Public Property Filename As String
            Public Property FilenameHash As UInteger

            ''' <summary>
            ''' Whether or not the <see cref="Filename"/> is a proper filename. Otherwise, it's a representation of <see cref="FilenameHash"/>.
            ''' </summary>
            Public Property IsFilenameSet As Boolean

            Public Overrides Function ToString() As String
                If Filename IsNot Nothing Then
                    Return Filename
                Else
                    Return MyBase.ToString
                End If
            End Function
        End Class

        Public Sub New()
            Entries = New List(Of Entry)
        End Sub

        Public Event FileSaved As EventHandler Implements ISavable.FileSaved

        Public Property Entries As List(Of Entry)

        Public Property Filename As String Implements IOnDisk.Filename

        Protected Property sir0Fat5Type As Integer

        Public Async Function OpenFile(filename As String, provider As IIOProvider) As Task Implements IOpenableFile.OpenFile
            Entries = New List(Of Entry)

            Using f As New Sir0
                Await f.OpenFile(filename, provider)
                Await ProcessSir0(f)
            End Using

            Me.Filename = filename
        End Function

        Public Async Function OpenFile(rawData As Byte()) As Task
            Entries = New List(Of Entry)

            Using f As New Sir0
                f.CreateFile(rawData)
                Await ProcessSir0(f)
            End Using
        End Function

        Protected Async Function ProcessSir0(sir0 As Sir0) As Task
            Dim dataOffset = BitConverter.ToInt32(sir0.ContentHeader, 0)
            Dim fileCount = BitConverter.ToInt32(sir0.ContentHeader, 4)
            Sir0Fat5Type = BitConverter.ToInt32(sir0.ContentHeader, 8)

            For count = 0 To fileCount - 1
                Dim info As New Entry
                info.Index = count

                Dim filenameOffset = Await sir0.ReadUInt32Async(dataOffset + count * 12 + 0)
                info.DataOffset = Await sir0.ReadInt32Async(dataOffset + count * 12 + 4)
                info.DataLength = Await sir0.ReadInt32Async(dataOffset + count * 12 + 8)

                If Sir0Fat5Type = 0 Then
                    'We're inferring the length based on the offset of the next filename
                    Dim filenameLength = Await sir0.ReadInt32Async(dataOffset + (count + 1) * 12 + 0) - filenameOffset
                    info.Filename = Await sir0.ReadUnicodeStringAsync(filenameOffset, filenameLength / 2)
                    info.IsFilenameSet = True
                ElseIf sir0Fat5Type = 1 Then
                    info.Filename = Conversion.Hex(filenameOffset).PadLeft(8, "0"c)
                    info.FilenameHash = filenameOffset
                    info.IsFilenameSet = False
                Else
                    Throw New NotSupportedException("FAT type not supported: " & sir0Fat5Type.ToString())
                End If

                Entries.Add(info)
            Next
        End Function

        Public Async Function GetRawData() As Task(Of Byte())
            If sir0Fat5Type = 0 Then
                Throw New NotImplementedException("Saving FAT type 0 not implemented.")

                'Type 1 is supported

            ElseIf sir0Fat5Type >= 2 Then
                Throw New NotSupportedException("FAT type not supported: " & sir0Fat5Type.ToString())
            End If

            Using f As New Sir0
                f.AutoAddSir0HeaderRelativePointers = True
                f.AutoAddSir0SubHeaderRelativePointers = True
                f.CreateFile()

                'Generate data
                Dim data As New List(Of Byte)
                For Each item In Entries
                    data.AddRange(BitConverter.GetBytes(item.FilenameHash))
                    data.AddRange(BitConverter.GetBytes(item.DataOffset))
                    data.AddRange(BitConverter.GetBytes(item.DataLength))
                Next

                Await f.SetContent(data.ToArray())

                'Generate content header
                Dim contentHeader As New List(Of Byte)
                contentHeader.AddRange(BitConverter.GetBytes(&H10)) 'Index to content section
                contentHeader.AddRange(BitConverter.GetBytes(Entries.Count))
                contentHeader.AddRange(BitConverter.GetBytes(sir0Fat5Type))
                f.SubHeaderRelativePointers.Add(0)

                f.ContentHeader = contentHeader.ToArray
                Return Await f.GetRawData()
            End Using
        End Function

        Public Async Function Save(filename As String, provider As IIOProvider) As Task Implements ISavableAs.Save
            provider.WriteAllBytes(filename, Await GetRawData())
        End Function

        Public Async Function Save(provider As IIOProvider) As Task Implements ISavable.Save
            Await Save(Filename, provider)
        End Function

        Public Function GetDefaultExtension() As String Implements ISavableAs.GetDefaultExtension
            Return "*.bin"
        End Function

        Public Function GetSupportedExtensions() As IEnumerable(Of String) Implements ISavableAs.GetSupportedExtensions
            Return {"*.bin"}
        End Function

    End Class
End Namespace

