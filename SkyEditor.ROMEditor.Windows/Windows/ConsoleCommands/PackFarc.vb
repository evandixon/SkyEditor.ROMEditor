Imports System.IO
Imports SkyEditor.Core.ConsoleCommands
Imports SkyEditor.Core.IO
Imports SkyEditor.IO.FileSystem
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD

Namespace Windows.ConsoleCommands
    Public Class PackFarc
        Inherits ConsoleCommand

        Public Sub New(FileSystem As IFileSystem)
            If FileSystem Is Nothing Then
                Throw New ArgumentNullException(NameOf(FileSystem))
            End If

            CurrentFileSystem = FileSystem
        End Sub

        Protected Property CurrentFileSystem As IFileSystem

        Public Overrides Async Function MainAsync(Arguments() As String) As Task
            If Arguments.Count > 1 Then
                If Directory.Exists(Arguments(0)) Then
                    Await Farc.Pack(Arguments(0), Arguments(1), Integer.Parse(Arguments(2)), Boolean.Parse(Arguments(3)), CurrentFileSystem)
                Else
                    Console.WriteLine("Directory does not exist: " & Arguments(0))
                End If
            Else
                Console.WriteLine("Usage: PackFarc <Input Directory> <Output Filename> <FARC Type (4 or 5)> <Use Filenames?>")
            End If
        End Function
    End Class

End Namespace
