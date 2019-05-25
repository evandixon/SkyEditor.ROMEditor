Imports SkyEditor.Core.IO
Imports SkyEditor.IO.FileSystem

Namespace MysteryDungeon.PSMD
    Public Class DatabaseBin
        Implements IOpenableFile

        ''' <summary>
        ''' Matches string hashes to the strings contained in the file.
        ''' </summary>
        ''' <returns>The games' scripts refer to the strings by this hash.</returns>
        Public Property Strings As List(Of String)

        Public Async Function OpenFile(Filename As String, Provider As IFileSystem) As Task Implements IOpenableFile.OpenFile
            Dim total As New Text.StringBuilder
            Using f As New GenericFile
                f.IsReadOnly = True
                Await f.OpenFile(Filename, Provider)
                Dim subHeaderPointer = Await f.ReadInt32Async(4)
                Dim stringPointerOffset = Await f.ReadInt32Async(subHeaderPointer + 4)
                Dim numEntries = Await f.ReadInt32Async(subHeaderPointer + 8) * 4

                For count = 0 To numEntries - 1
                    Dim entryPointer = Await f.ReadInt32Async(stringPointerOffset + count * 4)

                    If entryPointer > 0 Then
                        Dim s As New Text.StringBuilder()
                        Dim e = Text.UnicodeEncoding.Unicode

                        'Parse the null-terminated UTF-16 string
                        Dim j As Integer = 0
                        Dim cRaw As Byte()
                        Dim c As String
                        Do
                            cRaw = Await f.ReadAsync(entryPointer + j * 2, 2)
                            c = e.GetString(cRaw, 0, cRaw.Length)

                            If Not c = vbNullChar Then
                                s.Append(c)
                            End If

                            j += 1
                        Loop Until c = vbNullChar

                        Strings.Add(s.ToString)
                        total.AppendLine(s.ToString)
                    Else
                        Strings.Add("")
                    End If
                Next
            End Using
        End Function

        Public Sub New()
            MyBase.New
            Strings = New List(Of String)
        End Sub
    End Class

End Namespace
