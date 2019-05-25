Imports SkyEditor.Core.IO
Imports SkyEditor.IO.FileSystem

Namespace MysteryDungeon.PSMD
    Public Class JukeboxBin
        Implements IOpenableFile

        Public Class JukeboxEntry
            Public Property Filename As String
            Public Property Unk2 As String
            Public Property Unk3 As String
            Public Property UnlockCriteria As String
            Public Property unk5 As Integer
            Public Property unk6 As Integer
            Public Property unk7 As Integer
            Public Property unk8 As Integer
            Public Property unk9 As Integer
        End Class

        Public Property Entries As List(Of JukeboxEntry)


        Public Async Function OpenFile(Filename As String, Provider As IFileSystem) As Task Implements IOpenableFile.OpenFile
            Using f As New GenericFile
                f.IsReadOnly = True
                Await f.OpenFile(Filename, Provider)

                Dim subHeaderPointer = Await f.ReadInt32Async(4)
                Dim jukeboxPointerOffset = Await f.ReadInt32Async(subHeaderPointer + 0)
                Dim numEntries = Await f.ReadInt32Async(subHeaderPointer + 4)

                For count = 0 To numEntries - 1
                    Dim filenamePointer As Integer = Await f.ReadInt32Async(jukeboxPointerOffset + count * 36 + 0)
                    Dim u2 As Integer = Await f.ReadInt32Async(jukeboxPointerOffset + count * 36 + 4)
                    Dim u3 As Integer = Await f.ReadInt32Async(jukeboxPointerOffset + count * 36 + 8)
                    Dim unlockPointer As Integer = Await f.ReadInt32Async(jukeboxPointerOffset + count * 36 + 12)
                    Dim unk5 As Integer = Await f.ReadInt32Async(jukeboxPointerOffset + count * 36 + 16)
                    Dim unk6 As Integer = Await f.ReadInt32Async(jukeboxPointerOffset + count * 36 + 20)
                    Dim unk7 As Integer = Await f.ReadInt32Async(jukeboxPointerOffset + count * 36 + 24)
                    Dim unk8 As Integer = Await f.ReadInt32Async(jukeboxPointerOffset + count * 36 + 28)
                    Dim unk9 As Integer = Await f.ReadInt32Async(jukeboxPointerOffset + count * 36 + 32)

                    Dim e As New JukeboxEntry
                    e.Filename = Await f.ReadNullTerminatedUnicodeStringAsync(filenamePointer)
                    e.Unk2 = Await f.ReadNullTerminatedUnicodeStringAsync(u2)
                    e.Unk3 = Await f.ReadNullTerminatedUnicodeStringAsync(u3)
                    e.UnlockCriteria = Await f.ReadNullTerminatedUnicodeStringAsync(unlockPointer)
                    e.unk5 = unk5
                    e.unk6 = unk6
                    e.unk7 = unk7
                    e.unk8 = unk8
                    e.unk9 = unk9

                    Entries.Add(e)
                Next
            End Using
        End Function

        Public Sub New()
            MyBase.New
            Entries = New List(Of JukeboxEntry)
        End Sub
    End Class

End Namespace
