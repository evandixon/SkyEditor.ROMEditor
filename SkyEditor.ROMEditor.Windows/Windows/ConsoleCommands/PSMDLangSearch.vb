Imports System.IO
Imports SkyEditor.Core.ConsoleCommands
Imports SkyEditor.Core.IO
Imports SkyEditor.IO.FileSystem
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD

Namespace Windows.ConsoleCommands
    Public Class PSMDLangSearch
        Inherits ConsoleCommand

        Public Sub New(FileSystem As IFileSystem)
            If FileSystem Is Nothing Then
                Throw New ArgumentNullException(NameOf(FileSystem))
            End If

            CurrentFileSystem = FileSystem
        End Sub

        Protected Property CurrentFileSystem As IFileSystem

        Public Overrides Async Function MainAsync(Arguments() As String) As Task
            If Directory.Exists(Arguments(0)) Then
                Dim languageEntries As New Dictionary(Of String, Dictionary(Of UInteger, String))
                Dim totalList As New Dictionary(Of UInteger, String)
                For Each item In Directory.GetFiles(Arguments(0))
                    Dim msg As New MessageBin
                    Await msg.OpenFile(item, CurrentFileSystem)
                    languageEntries.Add(Path.GetFileNameWithoutExtension(item), New Dictionary(Of UInteger, String))
                    For Each s In msg.Strings
                        languageEntries(Path.GetFileNameWithoutExtension(item)).Add(s.Hash, s.Entry)
                        If Not totalList.ContainsKey(s.Hash) Then
                            totalList.Add(s.Hash, s.Entry)
                        Else
                            Console.WriteLine("Unable to add " & s.Hash & ": " & s.Entry)
                        End If
                    Next
                    msg.Dispose()
                Next

                Dim q = (From s In totalList Where s.Value.Contains("Stick")).ToList

                Console.WriteLine("Language loaded.")
                Console.WriteLine("In-console searching not implemented.  Please attach debugger.")
                Console.ReadLine()
                Debugger.Break()
            Else
                Console.WriteLine($"Directory ""{Arguments(0)}"" not found.")
            End If
        End Function
    End Class

End Namespace
