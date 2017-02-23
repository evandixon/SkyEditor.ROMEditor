Imports SkyEditor.Core.ConsoleCommands
Imports SkyEditor.Core.IO
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD

Namespace Windows.ConsoleCommands
    Public Class NameListResearcher
        Inherits ConsoleCommand

        Private Function GetHash(Search As String, File As MessageBin) As MessageBinStringEntry
            Return (From s In File.Strings Where s.Entry = Search).First
        End Function

        Public Overrides Async Function MainAsync(Arguments() As String) As Task
            If Arguments.Length > 0 Then
                If IO.File.Exists(Arguments(0)) Then
                    Dim output As New Text.StringBuilder
                    Dim nameOutput As New Text.StringBuilder
                    Dim msg As New GenericFile()
                    Dim msg2 As New MessageBin
                    Await msg.OpenFile(Arguments(0), CurrentApplicationViewModel.CurrentIOProvider)
                    Await msg2.OpenFile(Arguments(0), CurrentApplicationViewModel.CurrentIOProvider)
                    'Dim position = &HD0BA 'For Items
                    Dim position = &H14F56 'For Moves
                    For count = 0 To 2000
                        Dim s = msg.ReadNullTerminatedUnicodeString(position)
                        Console.WriteLine(s)
                        Dim entry = (From ent In msg2.Strings Where ent.Pointer = position).First
                        output.AppendLine(entry.HashSigned)
                        nameOutput.AppendLine(s)
                        position += s.Length * 2 + 2
                    Next
                    'IO.File.WriteAllText("PSMD Dungeon Name Hashes.txt", output.ToString)
                    'IO.File.WriteAllText("PSMD Dungeon BGM Names.txt", nameOutput.ToString)
                    'IO.File.WriteAllText("PSMD Item Name Hashes.txt", output.ToString)
                    'IO.File.WriteAllText("PSMD Item Names.txt", nameOutput.ToString)
                    IO.File.WriteAllText("PSMD Move Name Hashes.txt", output.ToString)
                    IO.File.WriteAllText("PSMD Move Names.txt", nameOutput.ToString)
                    Console.Write("Done.")
                Else
                    Console.WriteLine("File doesn't exist")
                End If
            Else
                Console.WriteLine("Usage: NameListResearcher <filename>")
            End If
        End Function
    End Class
End Namespace

