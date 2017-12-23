Imports SkyEditor.Core.ConsoleCommands

Namespace MysteryDungeon.PSMD.ConsoleCommands
    Public Class Crc32Hash
        Inherits ConsoleCommand

        Protected Overrides Sub Main(arguments() As String)
            If arguments.Length <= 1 Then
                Console.WriteLine("Usage: Crc32Hash <StringToHash>")
                Exit Sub
            End If

            Console.WriteLine("0x" & PmdFunctions.Crc32Hash(arguments(1)).ToString("X").PadLeft(6, "0"c))
        End Sub
    End Class
End Namespace
