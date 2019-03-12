Imports SkyEditor.Core.ConsoleCommands
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD

Namespace ConsoleCommands
    Public Class PmdCrc32
        Inherits ConsoleCommand

        Protected Overrides Sub Main(arguments() As String)
            MyBase.Main(arguments)

            If arguments.Length < 2 Then
                Console.WriteLine("Usage: PmdCrc32 <stringToHash>")
            End If

            Dim hash = PmdFunctions.Crc32Hash(arguments(1))
            Console.WriteLine(hash.ToString("X").PadLeft(8, "0"c))
        End Sub
    End Class

End Namespace
