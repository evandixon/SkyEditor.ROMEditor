Imports System.Text
Imports SkyEditor.Core.IO

Namespace MysteryDungeon.PSMD
    Public Class SAJD
        Implements IOpenableFile
        Implements IOnDisk
        Implements ISavableAs
        Implements IDetectableFileType

        Public Class Entry
            ''' <summary>
            ''' The symbol of a resource, used in scripts.
            ''' This is usually all-caps.
            ''' </summary>
            Public Property Symbol As String

            ''' <summary>
            ''' The name of the file associated with the symbol.
            ''' This should not end with ".img", because it will be appended elsewhere.
            ''' 
            ''' This is usually all lowercase
            ''' </summary>
            Public Property FileName As String

            Public Property String3 As String
            Public Property String4 As String

            Public Overrides Function ToString() As String
                Return Symbol & " | " & FileName & " | " & String3 & " | " & String4
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
                    Dim entryOffset = dataOffset + count * 16

                    Dim entry As New Entry

                    entry.Symbol = Await ReadString(f, Await f.ReadInt32Async(entryOffset + 0))
                    entry.FileName = Await ReadString(f, Await f.ReadInt32Async(entryOffset + 4))
                    entry.String3 = Await ReadString(f, Await f.ReadInt32Async(entryOffset + 8))
                    entry.String4 = Await ReadString(f, Await f.ReadInt32Async(entryOffset + 12))

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

                    If Not c = VBConstants.vbNullChar Then
                        s.Append(c)
                    End If

                    j += 1
                Loop Until c = VBConstants.vbNullChar

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
            Return Await file.ReadInt32Async(subHeader) = &H444A4153
        End Function

        Public Async Function Save(filename As String, provider As IIOProvider) As Task Implements ISavableAs.Save
            Using f As New Sir0
                f.AutoAddSir0HeaderRelativePointers = True
                f.CreateFile()

                Dim currentOffset = &H10
                Dim stringSection As New List(Of Byte)
                Dim dataSection As New List(Of Byte)

                For Each item In Entries
                    If item.Symbol IsNot Nothing Then
                        Dim str1Bytes = Encoding.Unicode.GetBytes(item.Symbol)
                        stringSection.AddRange(str1Bytes)
                        stringSection.Add(0)
                        stringSection.Add(0)
                        dataSection.AddRange(BitConverter.GetBytes(currentOffset))
                        currentOffset += str1Bytes.Length + 2

                        'Offset of the pointer since the last pointer
                        'The first of these is based on the length of the string section, and we'll update it later
                        f.RelativePointers.Add(&H4)

                        While stringSection.Count Mod 4 <> 0
                            stringSection.Add(0)
                            currentOffset += 1
                        End While
                    Else
                        dataSection.AddRange(BitConverter.GetBytes(0))
                    End If

                    If item.FileName IsNot Nothing Then
                        Dim str2Bytes = Encoding.Unicode.GetBytes(item.FileName)
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
                    Else
                        dataSection.AddRange(BitConverter.GetBytes(0))
                    End If

                    If item.String3 IsNot Nothing Then
                        Dim str3Bytes = Encoding.Unicode.GetBytes(item.String3)
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
                    Else
                        dataSection.AddRange(BitConverter.GetBytes(currentOffset))
                    End If

                    If item.String4 IsNot Nothing Then
                        Dim str4Bytes = Encoding.Unicode.GetBytes(item.String4)
                        stringSection.AddRange(str4Bytes)
                        stringSection.Add(0)
                        stringSection.Add(0)
                        dataSection.AddRange(BitConverter.GetBytes(currentOffset))
                        currentOffset += str4Bytes.Length + 2
                        f.RelativePointers.Add(4)

                        While stringSection.Count Mod 4 <> 0
                            stringSection.Add(0)
                            currentOffset += 1
                        End While
                    Else
                        dataSection.AddRange(BitConverter.GetBytes(currentOffset))
                    End If
                Next

                'Fill in first relative pointer from earlier
                f.RelativePointers(0) = stringSection.Count

                Dim magicString() As Byte = {&H53, &H41, &H4A, &H44}
                Dim contentHeader(12) As Byte
                magicString.CopyTo(contentHeader, 0)
                BitConverter.GetBytes(currentOffset).CopyTo(contentHeader, 4)
                BitConverter.GetBytes(Entries.Count).CopyTo(contentHeader, 8)
                f.ContentHeader = contentHeader

                'Add relative pointer in content header
                f.RelativePointers.Add(&H8)

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
