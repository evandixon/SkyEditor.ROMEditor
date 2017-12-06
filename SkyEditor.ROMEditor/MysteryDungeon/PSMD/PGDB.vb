Imports SkyEditor.Core.IO

Namespace MysteryDungeon.PSMD
    Public Class PGDB
        Implements IOpenableFile
        Implements IDetectableFileType

        Public Class Entry
            Public Property String1 As String 'Filename
            Public Property String2 As String 'Skeleton/animation
            Public Property String3 As String 'Romanized japanese name
            Public Property Data As Byte()

            Public Overrides Function ToString() As String
                Return String1 & "|" & String2 & "|" & String3
            End Function
        End Class

        Public Sub New()
            MyBase.New
            Entries = New List(Of Entry)
        End Sub

        Public Property Entries As List(Of Entry)

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

                    'Structure of entry:
                    '3 pointers
                    '0x4C bytes of unknown data

                    Dim entry As New Entry

                    entry.String1 = Await ReadString(f, Await f.ReadInt32Async(entryOffset + 0))
                    entry.String2 = Await ReadString(f, Await f.ReadInt32Async(entryOffset + 4))
                    entry.String3 = Await ReadString(f, Await f.ReadInt32Async(entryOffset + 8))
                    entry.Data = Await f.ReadAsync(entryOffset + 12, &H4C)

                    Entries.Add(entry)

                Next
            End Using
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
            Return Await file.ReadInt32Async(subHeader) = &H42444750
        End Function
    End Class

End Namespace
