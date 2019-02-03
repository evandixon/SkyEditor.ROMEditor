Imports System.Text
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

            ''' <summary>
            ''' Gets or sets the filename, updating the filename hash on set
            ''' </summary>
            Public Property Filename As String
                Get
                    Return _filename
                End Get
                Set(value As String)
                    _filename = value
                    FilenameHash = PmdFunctions.Crc32Hash(value)
                End Set
            End Property
            Dim _filename As String

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

        Public Property Sir0Fat5Type As Integer

        Public ReadOnly Property UsesFilenames As Boolean
            Get
                Return Sir0Fat5Type = 0
            End Get
        End Property

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

            Dim entryLength As Integer
            If Sir0Fat5Type = 0 Then
                entryLength = 16
            ElseIf Sir0Fat5Type = 1 Then
                entryLength = 12
            Else
                Throw New NotSupportedException("FAT type not supported: " & Sir0Fat5Type.ToString())
            End If

            For count = 0 To fileCount - 1
                Dim info As New Entry
                info.Index = count

                Dim filenameOffset = Await sir0.ReadUInt32Async(dataOffset + count * entryLength + 0)
                info.DataOffset = Await sir0.ReadInt32Async(dataOffset + count * entryLength + 4)
                info.DataLength = Await sir0.ReadInt32Async(dataOffset + count * entryLength + 8)

                If Sir0Fat5Type = 0 Then
                    info.Filename = Await sir0.ReadNullTerminatedUnicodeStringAsync(filenameOffset)
                    info.IsFilenameSet = True
                ElseIf Sir0Fat5Type = 1 Then
                    info.Filename = Conversion.Hex(filenameOffset).PadLeft(8, "0"c)
                    info.FilenameHash = filenameOffset
                    info.IsFilenameSet = False
                Else
                    Throw New NotSupportedException("FAT type not supported: " & Sir0Fat5Type.ToString())
                End If

                Entries.Add(info)
            Next
        End Function

        Public Async Function GetRawData() As Task(Of Byte())
            Select Case Sir0Fat5Type
                Case 0
                    Using f As New Sir0
                        f.AutoAddSir0HeaderRelativePointers = True
                        f.AutoAddSir0SubHeaderRelativePointers = True
                        f.CreateFile()

                        'Generate data
                        Dim data As New List(Of Byte)
                        Dim stringData As New List(Of Byte)
                        For Each item In Entries.OrderBy(Function(x) x.Filename, StringComparer.Ordinal) 'Sorting by filename is critical to correct lookups.
                            data.AddRange(BitConverter.GetBytes(&H10 + stringData.Count))
                            data.AddRange(BitConverter.GetBytes(item.DataOffset))
                            data.AddRange(BitConverter.GetBytes(item.DataLength))
                            data.AddRange(BitConverter.GetBytes(0))

                            stringData.AddRange(Text.Encoding.Unicode.GetBytes(item.Filename))
                            stringData.AddRange(Enumerable.Repeat(Of Byte)(0, 2)) 'Null terminator

                            'Add padding to make sure strings start at an index divisible by 4
                            While stringData.Count Mod 4 <> 0
                                stringData.Add(0)
                            End While
                        Next

                        'Add padding to make sure data section's start index is divisible by 0x10
                        While stringData.Count Mod &H10 <> 0
                            stringData.Add(0)
                        End While

                        Dim combinedData As New List(Of Byte)(stringData.Count + data.Count)
                        combinedData.AddRange(stringData)
                        combinedData.AddRange(data)
                        Await f.SetContent(combinedData.ToArray())

                        'Generate content header
                        Dim contentHeader As New List(Of Byte)
                        contentHeader.AddRange(BitConverter.GetBytes(&H10 + stringData.Count)) 'Index to content section
                        contentHeader.AddRange(BitConverter.GetBytes(Entries.Count))
                        contentHeader.AddRange(BitConverter.GetBytes(Sir0Fat5Type))

                        'SIR0 pointer offsets
                        f.SubHeaderRelativePointers.Add(0)

                        f.RelativePointers.Add(stringData.Count)
                        f.RelativePointers.AddRange(Enumerable.Repeat(&H10, Entries.Count - 1))

                        f.ContentHeader = contentHeader.ToArray
                        Return Await f.GetRawData()
                    End Using
                Case 1
                    Using f As New Sir0
                        f.AutoAddSir0HeaderRelativePointers = True
                        f.AutoAddSir0SubHeaderRelativePointers = True
                        f.CreateFile()

                        'Generate data
                        Dim data As New List(Of Byte)
                        For Each item In Entries.OrderBy(Function(x) x.FilenameHash) 'Sorting by filename hash is critical to correct lookups. This is a hash table after all.
                            data.AddRange(BitConverter.GetBytes(item.FilenameHash))
                            data.AddRange(BitConverter.GetBytes(item.DataOffset))
                            data.AddRange(BitConverter.GetBytes(item.DataLength))
                        Next

                        Await f.SetContent(data.ToArray())

                        'Generate content header
                        Dim contentHeader As New List(Of Byte)
                        contentHeader.AddRange(BitConverter.GetBytes(&H10)) 'Index to content section
                        contentHeader.AddRange(BitConverter.GetBytes(Entries.Count))
                        contentHeader.AddRange(BitConverter.GetBytes(Sir0Fat5Type))
                        f.SubHeaderRelativePointers.Add(0)

                        f.ContentHeader = contentHeader.ToArray
                        Return Await f.GetRawData()
                    End Using
                Case Else
                    Throw New NotSupportedException("FAT type not supported: " & Sir0Fat5Type.ToString())
            End Select
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

