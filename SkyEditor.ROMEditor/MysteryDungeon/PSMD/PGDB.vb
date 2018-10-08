Imports System.Text
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.IO.PluginInfrastructure

Namespace MysteryDungeon.PSMD
    Public Class PGDB
        Implements IOpenableFile
        Implements IOnDisk
        Implements ISavableAs
        Implements IDetectableFileType

        Public Class Entry

            ''' <summary>
            ''' Filename of the primary BGRS file associated with a BCH file. This should include the *.bgrs extension. Ex. "dummy_pokemon_00.bgrs".
            ''' </summary>
            Public Property PrimaryBgrsFilename As String

            ''' <summary>
            ''' Name of the secondary BGRS file defining extended animations. This should NOT include the *.bgrs extension. Ex. "4leg_beast_00"
            ''' This can be null if the primary BGRS file defines all of the extended animations.
            ''' </summary>
            Public Property SecondaryBgrsName As String

            ''' <summary>
            ''' Romanized Japanese name of the Pokémon or Actor
            ''' </summary>
            Public Property ActorName As String

            Public Property Data As Byte()

            Public Overrides Function ToString() As String
                Return PrimaryBgrsFilename & " | " & SecondaryBgrsName & " | " & ActorName
            End Function
        End Class

        Public Sub New()
            MyBase.New
            Entries = New List(Of Entry)
        End Sub

        Public Event FileSaved As EventHandler Implements ISavable.FileSaved

        Public Property Entries As List(Of Entry)
        Public Property Filename As String Implements IOnDisk.Filename

        Public Async Function OpenFile(Filename As String, Provider As IIOProvider) As Task Implements IOpenableFile.OpenFile
            Dim total As New Text.StringBuilder
            Using f As New GenericFile
                f.IsReadOnly = True
                Await f.OpenFile(Filename, Provider)
                Dim subHeaderPointer = Await f.ReadInt32Async(4)
                Dim dataOffset = Await f.ReadInt32Async(subHeaderPointer + 4)
                Dim numEntries = Await f.ReadInt32Async(subHeaderPointer + 8)

                For count = 0 To numEntries - 1
                    Dim entryOffset = dataOffset + count * &H54

                    Dim entry As New Entry

                    entry.PrimaryBgrsFilename = Await ReadString(f, Await f.ReadInt32Async(entryOffset + 0))
                    entry.SecondaryBgrsName = Await ReadString(f, Await f.ReadInt32Async(entryOffset + 4))
                    entry.ActorName = Await ReadString(f, Await f.ReadInt32Async(entryOffset + 8))
                    entry.Data = Await f.ReadAsync(entryOffset + 12, &H48)

                    Entries.Add(entry)

                Next
            End Using

            Me.Filename = Filename
        End Function

        Private Async Function ReadString(f As GenericFile, offset As Integer) As Task(Of String)
            If offset > 0 Then
                Dim s As New Text.StringBuilder()
                Dim e = Text.UnicodeEncoding.Unicode

                'Parse the null-terminated UTF-16 string
                Dim j As Integer = 0
                Dim cRaw As Byte()
                Dim c As String
                Do
                    cRaw = Await f.ReadAsync(offset + j * 2, 2)
                    c = e.GetString(cRaw, 0, cRaw.Length)

                    If Not c = vbNullChar Then
                        s.Append(c)
                    End If

                    j += 1
                Loop Until c = vbNullChar

                Return s.ToString
            Else
                Return Nothing
            End If
        End Function

        Public Async Function IsOfType(file As GenericFile) As Task(Of Boolean) Implements IDetectableFileType.IsOfType
            If file.Length <= 8 Then
                Return False
            End If

            Dim subHeader = Await file.ReadInt32Async(4)

            If file.Length <= subHeader + 4 Then
                Return False
            End If

            'look for PGDB magic
            Return Await file.ReadInt32Async(subHeader) = &H42444750
        End Function

        Public Async Function Save(filename As String, provider As IIOProvider) As Task Implements ISavableAs.Save
            Using f As New Sir0
                f.AutoAddSir0HeaderRelativePointers = True
                f.CreateFile()

                Dim currentOffset = &H10
                Dim stringSection As New List(Of Byte)
                Dim dataSection As New List(Of Byte)

                For Each item In Entries
                    Dim str1Bytes = Encoding.Unicode.GetBytes(item.PrimaryBgrsFilename)
                    stringSection.AddRange(str1Bytes)
                    stringSection.Add(0)
                    stringSection.Add(0)
                    dataSection.AddRange(BitConverter.GetBytes(currentOffset))
                    currentOffset += str1Bytes.Length + 2

                    'Offset of the pointer since the last pointer
                    'The first of these is based on the length of the string section, and we'll update it later
                    f.RelativePointers.Add(&H4C)

                    While stringSection.Count Mod 4 <> 0
                        stringSection.Add(0)
                        currentOffset += 1
                    End While

                    Dim str2Bytes = Encoding.Unicode.GetBytes(item.SecondaryBgrsName)
                    stringSection.AddRange(str2Bytes)
                    stringSection.Add(0)
                    stringSection.Add(0)
                    dataSection.AddRange(BitConverter.GetBytes(currentOffset))
                    currentOffset += str2Bytes.Length + 2
                    f.RelativePointers.Add(4)

                    While stringSection.Count Mod 4 <> 0
                        stringSection.Add(0)
                        currentOffset += 1
                    End While

                    Dim str3Bytes = Encoding.Unicode.GetBytes(item.ActorName)
                    stringSection.AddRange(str3Bytes)
                    stringSection.Add(0)
                    stringSection.Add(0)
                    dataSection.AddRange(BitConverter.GetBytes(currentOffset))
                    currentOffset += str3Bytes.Length + 2
                    f.RelativePointers.Add(4)

                    While stringSection.Count Mod 4 <> 0
                        stringSection.Add(0)
                        currentOffset += 1
                    End While

                    dataSection.AddRange(item.Data)
                Next

                'Fill in first relative pointer from earlier
                f.RelativePointers(0) = stringSection.Count

                Dim magicString() As Byte = {&H50, &H47, &H44, &H42}
                Dim contentHeader(12) As Byte
                magicString.CopyTo(contentHeader, 0)
                BitConverter.GetBytes(currentOffset).CopyTo(contentHeader, 4)
                BitConverter.GetBytes(Entries.Count).CopyTo(contentHeader, 8)
                f.ContentHeader = contentHeader

                'Add relative pointer in content header
                f.RelativePointers.Add(&H50)

                Await f.SetContent(stringSection.Concat(dataSection).ToArray())

                Await f.Save(filename, provider)
            End Using
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
