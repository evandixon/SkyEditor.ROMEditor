Imports System.Drawing.Imaging
Imports System.IO
Imports SkyEditor.Core.Projects
Imports SkyEditor.Core.Utilities
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD.Pokemon
Imports SkyEditor.ROMEditor.Projects

Namespace MysteryDungeon.PSMD.Projects
    Public Class PsmdPortraitProject
        Inherits GenericModProject

        Public Overrides Async Function Initialize() As Task
            Await MyBase.Initialize

            Me.Message = My.Resources.Language.LoadingExtractingPortraits
            Me.Progress = 0
            Me.IsIndeterminate = False

            Dim provider = CurrentPluginManager.CurrentIOProvider
            Dim faceFarc As New Farc
            Await faceFarc.OpenFile(Path.Combine(Me.GetRawFilesDir, "romfs", "face_graphic.bin"), provider)

            Dim debugMsg As New Farc
            Await debugMsg.OpenFile(Path.Combine(Me.GetRawFilesDir, "romfs", "message_debug.bin"), provider)
            Dim commonDebugMsg As New MessageBinDebug
            Await commonDebugMsg.OpenFile("common.dbin", debugMsg)

            Dim graphicsDb As New PGDB
            Await graphicsDb.OpenFile(Path.Combine(Me.GetRawFilesDir, "romfs", "pokemon_graphics_database.bin"), provider)

            Dim actorInfo As New ActorDataInfo
            Await actorInfo.OpenFile(IO.Path.Combine(Me.GetRawFilesDir, "romfs", "pokemon", "pokemon_actor_data_info.bin"), CurrentPluginManager.CurrentIOProvider)



            Dim pokemonNames = graphicsDb.Entries.Select(Function(x) x.ActorName).ToList()
            pokemonNames.AddRange(actorInfo.Entries.Select(Function(x) x.Name.ToLower()))
            pokemonNames.AddRange(commonDebugMsg.GetCommonPokemonNames().Select(Function(p) p.Value.ToLower().Replace("pokemon_", "")))
            pokemonNames.Add("dummy_pokemon")
            pokemonNames.Add("houou_rarecolor")
            pokemonNames.Add("meroetta_step")
            pokemonNames.Add("meroetta_voice")

            Dim potentialFilenames As New List(Of String)
            For Each pokemonName In pokemonNames
                For emotionNumber = 0 To 30
                    potentialFilenames.Add($"{pokemonName}_{emotionNumber.ToString().PadLeft(2, "0")}.bin")
                    potentialFilenames.Add($"{pokemonName}_hanten_{emotionNumber.ToString().PadLeft(2, "0")}.bin")
                    potentialFilenames.Add($"{pokemonName}_f_{emotionNumber.ToString().PadLeft(2, "0")}.bin")
                    potentialFilenames.Add($"{pokemonName}_f{emotionNumber.ToString().PadLeft(2, "0")}.bin")
                    potentialFilenames.Add($"{pokemonName}_f_hanten_{emotionNumber.ToString().PadLeft(2, "0")}.bin")
                    potentialFilenames.Add($"{pokemonName}_r_{emotionNumber.ToString().PadLeft(2, "0")}.bin") 'Speculation, Not seen in code
                Next
            Next
            faceFarc.SetFilenames(potentialFilenames)

            Dim unmatched = faceFarc.GetFiles("/", "*", True).Where(Function(x) x.ToLower <> x).ToList()

            Dim onProgressed = Sub(sender As Object, e As ProgressReportedEventArgs)
                                   Me.Message = My.Resources.Language.LoadingExtractingPortraits
                                   Me.Progress = e.Progress
                                   Me.IsIndeterminate = False
                               End Sub

            Dim a = New AsyncFor
            AddHandler a.ProgressChanged, onProgressed
            a.RunSynchronously = False
            Await a.RunForEach(faceFarc.GetFiles("/", "*", True),
                               Sub(portrait As String)
                                   Dim outputPath = Path.Combine(Me.GetRootDirectory, Path.GetFileNameWithoutExtension(portrait) & ".png")
                                   Dim rawData = faceFarc.ReadAllBytes(portrait)
                                   Using bitmap = PmdGraphics.ReadPortrait(rawData)
                                       bitmap.Save(outputPath, ImageFormat.Png)
                                   End Using
                               End Sub)
            RemoveHandler a.ProgressChanged, onProgressed

            Me.IsCompleted = True
        End Function

        Public Overrides Async Function Build() As Task
            '...
            Throw New NotImplementedException

            Await MyBase.Build
        End Function

        Public Overrides Function GetFilesToCopy(Solution As Solution, BaseRomProjectName As String) As IEnumerable(Of String)
            Return {Path.Combine("romfs", "face_graphic.bin"),
                Path.Combine("romfs", "message_debug.bin"),
                Path.Combine("romfs", "message_debug.lst"),
                Path.Combine("romfs", "pokemon_graphics_database.bin"),
                Path.Combine("romfs", "pokemon", "pokemon_actor_data_info.bin")}
        End Function
    End Class

End Namespace
