Imports System.IO
Imports SkyEditor.Core
Imports SkyEditor.Core.ConsoleCommands
Imports SkyEditor.Core.Windows
Imports SkyEditor.ROMEditor.MysteryDungeon.Explorers
Imports SkyEditor.ROMEditor.Windows.FileFormats.Explorers

Namespace Windows.ConsoleCommands
    Public Class ImportLanguage
        Inherits ConsoleCommand

        Public Overrides Async Function MainAsync(Arguments() As String) As Task
            Dim LanguageStringPath = Arguments(0)
            Dim formatRegex As New Text.RegularExpressions.Regex("\[.+\]")
            Dim ls As New LanguageString
            Await ls.OpenFile(LanguageStringPath, CurrentApplicationViewModel.CurrentIOProvider)
            Dim languagechar As String = Path.GetFileNameWithoutExtension(LanguageStringPath).Replace("text_", "")
            Dim language As String
            Select Case languagechar
                Case "e"
                    language = "English"
                Case "f"
                    language = "Français"
                Case "s"
                    language = "Español"
                Case "g"
                    language = "Deutsche"
                Case "i"
                    language = "Italiano"
                Case "j"
                    language = "日本語"
                Case Else
                    Console.WriteLine("Unrecognized language character :" & languagechar)
                    Console.WriteLine("Please type the name of the language this file corresponds to:")
                    language = Console.ReadLine
            End Select

            'Import Pokemon
            Dim PokemonLines As New List(Of String)
            For count = 0 To LanguageString.PokemonNameLength - 1
                PokemonLines.Add(count.ToString & "=" & formatRegex.Replace(ls.GetPokemonName(count), ""))
            Next
            Dim pkmFile = EnvironmentPaths.GetResourceName(language & "/SkyPokemon.txt", "SkyEditor")
            If Not Directory.Exists(Path.GetDirectoryName(pkmFile)) Then
                Directory.CreateDirectory(Path.GetDirectoryName(pkmFile))
            End If
            File.WriteAllLines(pkmFile, PokemonLines.ToList)
            Console.WriteLine("Saved Pokemon.")

            'Import Items
            Dim ItemLines As New List(Of String)
            For count = 0 To LanguageString.ItemLength - 1
                ItemLines.Add(count.ToString & "=" & formatRegex.Replace(ls.GetItemName(count), ""))
            Next
            Dim itemFile = EnvironmentPaths.GetResourceName(language & "/SkyItems.txt", "SkyEditor")
            If Not Directory.Exists(Path.GetDirectoryName(itemFile)) Then
                Directory.CreateDirectory(Path.GetDirectoryName(itemFile))
            End If
            File.WriteAllLines(itemFile, ItemLines.ToList)
            Console.WriteLine("Saved Items.")

            'Import Moves
            Dim MoveLines As New List(Of String)
            For count = 0 To LanguageString.MoveLength - 1
                MoveLines.Add(count.ToString & "=" & formatRegex.Replace(ls.GetMoveName(count), ""))
            Next
            Dim moveFile = EnvironmentPaths.GetResourceName(language & "/SkyMoves.txt", "SkyEditor")
            If Not Directory.Exists(Path.GetDirectoryName(moveFile)) Then
                Directory.CreateDirectory(Path.GetDirectoryName(moveFile))
            End If
            File.WriteAllLines(moveFile, MoveLines.ToList)
            Console.WriteLine("Saved Moves.")

            'Import Locations
            Dim LocationLines As New List(Of String)
            For count = 0 To LanguageString.LocationLength - 1
                LocationLines.Add(count.ToString & "=" & formatRegex.Replace(ls.GetLocationName(count), ""))
            Next
            Dim locFile = EnvironmentPaths.GetResourceName(language & "/SkyLocations.txt", "SkyEditor")
            If Not Directory.Exists(Path.GetDirectoryName(locFile)) Then
                Directory.CreateDirectory(Path.GetDirectoryName(locFile))
            End If
            File.WriteAllLines(locFile, LocationLines.ToList)
            Console.WriteLine("Saved Locations.")

            Console.WriteLine("Done!")
        End Function
    End Class

End Namespace
