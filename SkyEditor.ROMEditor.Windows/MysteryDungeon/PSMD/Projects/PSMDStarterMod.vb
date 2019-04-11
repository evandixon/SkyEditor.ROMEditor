Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports SkyEditor.Core.Projects
Imports SkyEditor.Core.Utilities
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD.Dungeon
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD.Pokemon
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD.Extensions
Imports SkyEditor.Core.IO
Imports SkyEditor.ROMEditor.MysteryDungeon.GTI

Namespace MysteryDungeon.PSMD.Projects
    ''' <summary>
    ''' Mod for PSMD that allows editing playable starter Pokémon.
    ''' </summary>
    Public Class PsmdStarterMod
        Inherits PsmdLuaProject

        Public Sub New(ioProvider As IIOProvider)
            If ioProvider Is Nothing Then
                Throw New ArgumentNullException(NameOf(ioProvider))
            End If

            CurrentIOProvider = ioProvider

            Me.AddScriptsToProject = False
        End Sub

        Protected Property CurrentIOProvider As IIOProvider

#Region "Settings"
        Public Property EnableModelPatching As Boolean
            Get
                Return Settings(NameOf(EnableModelPatching))
            End Get
            Set(value As Boolean)
                Settings(NameOf(EnableModelPatching)) = value
            End Set
        End Property

        Public Property EnablePortraitPatching As Boolean
            Get
                Return Settings(NameOf(EnablePortraitPatching))
            End Get
            Set(value As Boolean)
                Settings(NameOf(EnablePortraitPatching)) = value
            End Set
        End Property
#End Region

        Public Overrides Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {GameStrings.PSMDCode, GameStrings.GTICode}
        End Function

        Public Overrides Function GetFilesToCopy(solution As Solution, baseRomProjectName As String) As IEnumerable(Of String)
            Return {Path.Combine("romfs", "script"), 'Both
                    Path.Combine("romfs", "message_fr"), 'GTI
                    Path.Combine("romfs", "message_ge"), 'GTI
                    Path.Combine("romfs", "message_it"), 'GTI
                    Path.Combine("romfs", "message_sp"), 'GTI
                    Path.Combine("romfs", "message"), 'GTI
                    Path.Combine("romfs", "message_en.bin"), 'PSMD
                    Path.Combine("romfs", "message_fr.bin"), 'PSMD
                    Path.Combine("romfs", "message_ge.bin"), 'PSMD
                    Path.Combine("romfs", "message_it.bin"), 'PSMD
                    Path.Combine("romfs", "message_sp.bin"), 'PSMD
                    Path.Combine("romfs", "message_us.bin"), 'PSMD
                    Path.Combine("romfs", "message.bin"), 'PSMD
                    Path.Combine("romfs", "message_en.lst"), 'PSMD
                    Path.Combine("romfs", "message_fr.lst"), 'PSMD
                    Path.Combine("romfs", "message_ge.lst"), 'PSMD
                    Path.Combine("romfs", "message_it.lst"), 'PSMD
                    Path.Combine("romfs", "message_sp.lst"), 'PSMD
                    Path.Combine("romfs", "message_us.lst"), 'PSMD
                    Path.Combine("romfs", "message.lst"), 'PSMD
                    Path.Combine("romfs", "dungeon", "fixed_pokemon.bin"), 'Both
                    Path.Combine("romfs", "pokemon", "pokemon_actor_data_info.bin"), 'PSMD
                    Path.Combine("romfs", "script", "pokemon_actor_data.bin"), 'GTI
                    Path.Combine("romfs", "face_graphic.bin"), 'Both
                    Path.Combine("romfs", "message_debug.bin"), 'PSMD
                    Path.Combine("romfs", "message_debug.lst"), 'PSMD
                    Path.Combine("romfs", "pokemon_graphic.bin"), 'Both
                    Path.Combine("romfs", "pokemon_graphics_database.bin"), 'Both
                    Path.Combine("exefs", "code.bin"), 'GTI
                    Path.Combine("ExHeader.bin")  'Both
            }
        End Function

#Region "Shared Patching Functions"

        ''' <summary>
        ''' Fixes Pokemon with a dummy model reference
        ''' </summary>
        ''' <remarks>This will call Save on <paramref name="pgdb"/>. This is not thread safe with regards to <paramref name="pgdb"/>.</remarks>
        Private Async Function FixPokemonWithDummyModel(pgdb As PGDB) As Task
            For Each item In pgdb.Entries.Where(Function(x) x.PrimaryBgrsFilename = "dummypokemon_00.bgrs").ToArray()
                item.PrimaryBgrsFilename = item.ActorName & "_00.bgrs"
            Next
            Await pgdb.Save(CurrentIOProvider)
        End Function

        ''' <summary>
        ''' Fixes hard-coded Pokemon IDs in the LUA scripts.
        ''' </summary>
        Private Async Function FixPokemonIDsInScripts(starters As StarterDefinitions) As Task
            Dim replacementDictionary As Dictionary(Of Integer, Integer) = starters.GetReplacementDictionary
            Dim evoDictionary As Dictionary(Of Integer, Integer) = starters.GetEvoDictionary

            Dim patchExpression As New Regex("if ((pokemonIndexHero)|(pokemonIndexPartner)) \=\= ([0-9]{1,3}) then", RegexOptions.Compiled)
            Me.Progress = 0
            Me.IsIndeterminate = False
            Me.Message = My.Resources.Language.LoadingPatchingScripts
            Dim f As New AsyncFor
            AddHandler f.ProgressChanged, Sub(sender As Object, e As ProgressReportedEventArgs)
                                              Me.Progress = e.Progress
                                          End Sub
            Await f.RunForEach(Directory.GetFiles(IO.Path.Combine(Me.GetRootDirectory, "script"), "*.lua", SearchOption.AllDirectories),
                               Sub(filename As String)
                                   Dim script = CurrentPluginManager.CurrentIOProvider.ReadAllText(filename & ".original")

                                   Dim edited As Boolean = False
                                   For Each item As Match In patchExpression.Matches(script)
                                       Dim originalID = CInt(item.Groups(4).Value)
                                       If replacementDictionary.ContainsKey(originalID) Then
                                           Dim newId = replacementDictionary(originalID)
                                           script = script.Replace(item.Value, item.Value.Replace(item.Groups(4).Value, newId.ToString))
                                           edited = True
                                       ElseIf evoDictionary.ContainsKey(originalID) Then
                                           Dim newId = evoDictionary(originalID)
                                           script = script.Replace(item.Value, item.Value.Replace(item.Groups(4).Value, newId.ToString))
                                           edited = True
                                       Else
                                           Console.WriteLine("Unknown ID in script: " & filename)
                                       End If
                                   Next

                                   If edited Then
                                       'Preserves filesystem timestamp if we don't overwrite all the files
                                       CurrentPluginManager.CurrentIOProvider.WriteAllText(filename, script)
                                       Console.WriteLine("Edited script: " & filename)
                                   End If

                               End Sub)
            Me.Progress = 1
        End Function

#End Region

#Region "GTI Patching Functions"
        Private Async Function AddMissingModelAnimationsGti(pgdb As PGDB) As Task
            Using pokemonGraphic As New Farc
                Await pokemonGraphic.OpenFile(Path.Combine(Me.GetRawFilesDir, "romfs", "pokemon_graphic.bin"), CurrentIOProvider)

                Dim token As New ProgressReportToken
                WatchProgressReportToken(token, False)
                Await pokemonGraphic.SubstituteMissingAnimationsGti(pgdb, token)
                UnwatchProgressReportToken(token)

                Await pokemonGraphic.Save(CurrentIOProvider)
            End Using
        End Function

        Private Async Function SubstituteMissingPortraitsGti() As Task
            Dim provider = CurrentPluginManager.CurrentIOProvider

            'Extract face_graphic.bin
            Me.Message = My.Resources.Language.LoadingExtractingPortraits
            Me.Progress = 0
            Me.IsIndeterminate = False

            Using faceFarc As New Farc
                Await faceFarc.OpenFile(Path.Combine(Me.GetRawFilesDir, "romfs", "face_graphic.bin"), provider)
                Await faceFarc.SubstituteMissingPortraitsGti()
                Await faceFarc.Save(provider)
            End Using
        End Function

        Private Async Function FixCodeBinGti(starters As StarterDefinitionsGti) As Task
            If IsGtiUS Then
                Using codeBin As New CodeBinGtiUS
                    Await codeBin.OpenFile(Path.Combine(Me.GetRawFilesDir, "exefs", "code.bin"), CurrentPluginManager.CurrentIOProvider)
                    'Await codeBin.SetStarter1(starters.Starter1)
                    Await codeBin.SetStarter39(starters.Starter39)
                    'Await codeBin.SetStarter42(starters.Starter42)
                    Await codeBin.SetStarter45(starters.Starter45)
                    Await codeBin.SetStarter122(starters.Starter122)
                    Await codeBin.Save(CurrentPluginManager.CurrentIOProvider)
                End Using
            ElseIf IsGtiEU Then
                Throw New NotImplementedException("EU regions of GTI are not currently implemented.")
            ElseIf IsGtiJP Then
                Throw New NotImplementedException("JP regions of GTI are not currently implemented.")
            Else
                Throw New NotSupportedException("Only the US, EU, and JP regions of GTI are supported.")
            End If
        End Function

        Private Async Function FixHighResModelsGti(starters As StarterDefinitionsGti) As Task
            Dim actorInfo As New ActorDataInfoGti
            Await actorInfo.OpenFile(IO.Path.Combine(Me.GetRawFilesDir, "romfs", "script", "pokemon_actor_data.bin"), CurrentPluginManager.CurrentIOProvider)
            'actorInfo.GetEntryByName("PIKACHUU_H").PokemonID = starters.Starter1
            actorInfo.GetEntryByName("TSUTAAJA_H").PokemonID = starters.Starter39
            'actorInfo.GetEntryByName("POKABU_H").PokemonID = starters.Starter42
            actorInfo.GetEntryByName("MIJUMARU_H").PokemonID = starters.Starter45
            actorInfo.GetEntryByName("KIBAGO_H").PokemonID = starters.Starter122
            Await actorInfo.Save(CurrentPluginManager.CurrentIOProvider)
        End Function

        Private Sub FixPersonalityTestGti(starters As StarterDefinitionsGti)
            Dim sourceScript = File.ReadAllText(Path.Combine(Me.GetRootDirectory, "script", "menu", "menu_ground_chara_select.lua.original"))
            Dim toReplaceCharaSet =
"  local charaSet = {
    45,
    122,
    39,
    1,
    42
  }"

            Dim toReplaceTable =
"  local pokeTbl = {
      [45] = -391927830,
      [122] = -757706441,
      [39] = 446223748,
      [1] = 494718355,
      [42] = 989755712
    }
"

            Dim toReplaceOffsets =
"  local faceImgOffsTbl = {
    [1] = {u = 0, v = 0},
    [39] = {u = 80, v = 0},
    [42] = {u = 160, v = 0},
    [45] = {u = 0, v = 80},
    [122] = {u = 80, v = 80}
  }"

            Dim repalceWithCharaSet =
"  local charaSet = {
    " & starters.Starter45 & ",
    " & starters.Starter122 & ",
    " & starters.Starter39 & ",
    1,
    42
  }"

            Dim repalceWithTable =
"  local pokeTbl = {
      [" & starters.Starter45 & "] = -391927830,
      [" & starters.Starter122 & "] = -757706441,
      [" & starters.Starter39 & "] = 446223748,
      [1] = 494718355,
      [42] = 989755712
    }
"

            Dim repalceWithOffsets =
"  local faceImgOffsTbl = {
    [1] = {u = 0, v = 0},
    [" & starters.Starter39 & "] = {u = 80, v = 0},
    [42] = {u = 160, v = 0},
    [" & starters.Starter45 & "] = {u = 0, v = 80},
    [" & starters.Starter122 & "] = {u = 80, v = 80}
  }"

            sourceScript = sourceScript.Replace(toReplaceCharaSet, repalceWithCharaSet)
            sourceScript = sourceScript.Replace(toReplaceTable, repalceWithTable)
            sourceScript = sourceScript.Replace(toReplaceOffsets, repalceWithOffsets)

            File.WriteAllText(Path.Combine(Me.GetRootDirectory, "script", "menu", "menu_ground_chara_select.lua"), sourceScript)
        End Sub

#End Region

#Region "PSMD Patching Functions"

        Private Async Function AddMissingModelAnimationsPsmd(pgdb As PGDB) As Task
            Using pokemonGraphic As New Farc
                Await pokemonGraphic.OpenFile(Path.Combine(Me.GetRawFilesDir, "romfs", "pokemon_graphic.bin"), CurrentIOProvider)

                Dim token As New ProgressReportToken
                WatchProgressReportToken(token, False)
                Await pokemonGraphic.SubstituteMissingAnimationsPsmd(pgdb, token)
                UnwatchProgressReportToken(token)

                Await pokemonGraphic.Save(CurrentIOProvider)
            End Using
        End Function

        ''' <summary>
        ''' Replaces placeholder portraits with the default emotion. Build progress is reported too.
        ''' </summary>
        Private Async Function SubstituteMissingPortraitsPsmd() As Task
            Dim provider = CurrentPluginManager.CurrentIOProvider

            'Extract face_graphic.bin
            Me.Message = My.Resources.Language.LoadingExtractingPortraits
            Me.Progress = 0
            Me.IsIndeterminate = False

            Using faceFarc As New Farc
                Await faceFarc.OpenFile(Path.Combine(Me.GetRawFilesDir, "romfs", "face_graphic.bin"), provider)

                Dim unmatched = faceFarc.GetFiles("/", "*", True).Where(Function(x) x.ToLower <> x).ToList()

                Dim onProgressed = Sub(sender As Object, e As ProgressReportedEventArgs)
                                       Me.Message = My.Resources.Language.LoadingExtractingPortraits
                                       Me.Progress = e.Progress
                                       Me.IsIndeterminate = False
                                   End Sub

                Dim filenameRegex As New Regex("(([a-z0-9]|_)+)(_f)?(_hanten)?(_r)?_([0-9]{2})", RegexOptions.Compiled)
                Dim directoryCreateLock As New Object
                Dim a = New AsyncFor
                AddHandler a.ProgressChanged, onProgressed
                Await a.RunForEach(faceFarc.GetFiles("/", "*", True),
                                   Sub(portrait As String)
                                       Dim match = filenameRegex.Match(Path.GetFileNameWithoutExtension(portrait))
                                       Dim outputPath As String

                                       If match.Success Then
                                           outputPath = Path.Combine(Me.GetRootDirectory, match.Groups(1).Value, Path.GetFileNameWithoutExtension(portrait) & ".png")
                                       Else
                                           outputPath = Path.Combine(Me.GetRootDirectory, "_Unknown", Path.GetFileNameWithoutExtension(portrait) & ".png")
                                       End If

                                       'Create directory if it doesn't exist
                                       If Not provider.DirectoryExists(Path.GetDirectoryName(outputPath)) Then
                                           SyncLock directoryCreateLock
                                               If Not provider.DirectoryExists(Path.GetDirectoryName(outputPath)) Then 'Check again in case of race condition
                                                   provider.CreateDirectory(Path.GetDirectoryName(outputPath))
                                               End If
                                           End SyncLock
                                       End If

                                       Dim rawData = faceFarc.ReadAllBytes(portrait)
                                       Using bitmap = PmdGraphics.ReadPortrait(rawData)
                                           bitmap.Save(outputPath, ImageFormat.Png)
                                       End Using
                                   End Sub)
                RemoveHandler a.ProgressChanged, onProgressed
            End Using

            'Analyze the portraits to find which ones need replacing
            Me.Message = My.Resources.Language.LoadingAnalyzingPortraits
            Me.Progress = 0
            Me.IsIndeterminate = False

            Const samplePokemon = "amaruruga" 'This is largly arbitrary. With a few exceptions such as iibui, most Pokemon will do
            Const samplePokemon2 = "iibui" 'Some Pokemon have slightly different backgrounds, so there needs to be a different sample.
            Dim blankPortrait01 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, "erureido_mega", "erureido_mega" & "_01.png"))
            Dim blankPortrait02 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, "erureido_mega", "erureido_mega" & "_02.png"))
            Dim blankPortrait03 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_03.png"))
            Dim blankPortrait03Version2 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon2, samplePokemon2 & "_03.png"))
            Dim blankPortrait04 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_04.png"))
            Dim blankPortrait05 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_05.png"))
            Dim blankPortrait06 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_06.png"))
            Dim blankPortrait07 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_07.png"))
            Dim blankPortrait08 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_08.png"))
            Dim blankPortrait09 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_09.png"))
            Dim blankPortrait10 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_10.png"))
            Dim blankPortrait10Version2 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, "nyasupaa", "nyasupaa" & "_10.png"))
            Dim blankPortrait11 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_11.png"))
            Dim blankPortrait12 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_12.png"))
            Dim blankPortrait12Version2 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon2, samplePokemon2 & "_12.png"))
            Dim blankPortrait13 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_13.png"))
            Dim blankPortrait14 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_14.png"))
            Dim blankPortrait15 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_15.png"))
            Dim blankPortrait16 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_16.png"))
            Dim blankPortrait17 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, "nyasupaa", "nyasupaa" & "_17.png"))

            Dim defaultPortraitTransformRegex As New Regex("(([a-z0-9]|_)+)_[0-9]{2}\.png")
            Dim onProgressed2 = Sub(sender As Object, e As ProgressReportedEventArgs)
                                    Me.Message = My.Resources.Language.LoadingAnalyzingPortraits
                                    Me.Progress = e.Progress
                                    Me.IsIndeterminate = False
                                End Sub

            Dim a2 = New AsyncFor
            AddHandler a2.ProgressChanged, onProgressed2
            Await a2.RunForEach(Directory.GetDirectories(Me.GetRootDirectory, "*", SearchOption.TopDirectoryOnly),
                               Sub(pkmDir As String)
                                   For Each imagePath In Directory.GetFiles(pkmDir, "*.png", SearchOption.TopDirectoryOnly)
                                       Dim sample As Byte() = Nothing
                                       Dim sample2 As Byte() = Nothing
                                       If imagePath.EndsWith("_01.png") Then
                                           sample = blankPortrait01
                                       ElseIf imagePath.EndsWith("_02.png") Then
                                           sample = blankPortrait02
                                       ElseIf imagePath.EndsWith("_03.png") Then
                                           sample = blankPortrait03
                                           sample2 = blankPortrait03Version2
                                       ElseIf imagePath.EndsWith("_04.png") Then
                                           sample = blankPortrait04
                                       ElseIf imagePath.EndsWith("_05.png") Then
                                           sample = blankPortrait05
                                       ElseIf imagePath.EndsWith("_06.png") Then
                                           sample = blankPortrait06
                                       ElseIf imagePath.EndsWith("_07.png") Then
                                           sample = blankPortrait07
                                       ElseIf imagePath.EndsWith("_08.png") Then
                                           sample = blankPortrait08
                                       ElseIf imagePath.EndsWith("_09.png") Then
                                           sample = blankPortrait09
                                       ElseIf imagePath.EndsWith("_10.png") Then
                                           sample = blankPortrait10
                                           sample2 = blankPortrait10Version2
                                       ElseIf imagePath.EndsWith("_11.png") Then
                                           sample = blankPortrait11
                                       ElseIf imagePath.EndsWith("_12.png") Then
                                           sample = blankPortrait12
                                           sample2 = blankPortrait12Version2
                                       ElseIf imagePath.EndsWith("_13.png") Then
                                           sample = blankPortrait13
                                       ElseIf imagePath.EndsWith("_14.png") Then
                                           sample = blankPortrait14
                                       ElseIf imagePath.EndsWith("_15.png") Then
                                           sample = blankPortrait15
                                       ElseIf imagePath.EndsWith("_16.png") Then
                                           sample = blankPortrait16
                                       ElseIf imagePath.EndsWith("_17.png") Then
                                           sample = blankPortrait17
                                       End If

                                       If sample IsNot Nothing AndAlso defaultPortraitTransformRegex.IsMatch(imagePath) Then
                                           Dim doCopy = False
                                           Dim data = File.ReadAllBytes(imagePath)
                                           doCopy = data.SequenceEqual(sample)

                                           If Not doCopy AndAlso sample2 IsNot Nothing Then
                                               doCopy = data.SequenceEqual(sample2)
                                           End If

                                           If doCopy Then
                                               Dim defaultPortraitPath = Path.Combine(Path.GetDirectoryName(imagePath), defaultPortraitTransformRegex.Match(imagePath).Groups(1).Value & "_00.png")
                                               If File.Exists(defaultPortraitPath) Then
                                                   File.Copy(defaultPortraitPath, imagePath, True)
                                               End If
                                           End If
                                       End If
                                   Next
                               End Sub)
            RemoveHandler a2.ProgressChanged, onProgressed2

            'Rebuild the portraits
            Me.Message = My.Resources.Language.LoadingRepackingPortraits
            Me.Progress = 0
            Me.IsIndeterminate = False

            Dim f As New Farc(5)

            Dim onProgressed3 = Sub(sender As Object, e As ProgressReportedEventArgs)
                                    Me.Message = My.Resources.Language.LoadingRepackingPortraits
                                    Me.Progress = e.Progress
                                    Me.IsIndeterminate = False
                                End Sub

            Dim a3 = New AsyncFor
            AddHandler a3.ProgressChanged, onProgressed3
            Await a3.RunForEach(Directory.GetFiles(Me.GetRootDirectory, "*.png", SearchOption.AllDirectories),
                               Sub(portrait As String)
                                   Using img As New Bitmap(portrait)
                                       f.WriteAllBytes(Path.GetFileNameWithoutExtension(portrait) & ".bin", PmdGraphics.SavePortrait(img))
                                   End Using
                               End Sub)
            RemoveHandler a3.ProgressChanged, onProgressed3

            Await f.Save(Path.Combine(Me.GetRawFilesDir, "romfs", "face_graphic.bin"), CurrentPluginManager.CurrentIOProvider)
        End Function

        ''' <summary>
        ''' Replaces the Pokemon IDs of pokemon_actor_data_info.bin to allow displaying models in high-res mode
        ''' </summary>
        ''' <param name="starters"></param>
        ''' <returns></returns>
        Private Async Function FixHighResModelsPsmd(starters As StarterDefinitionsPsmd) As Task
            Dim actorInfo As New ActorDataInfo
            Await actorInfo.OpenFile(IO.Path.Combine(Me.GetRawFilesDir, "romfs", "pokemon", "pokemon_actor_data_info.bin"), CurrentPluginManager.CurrentIOProvider)
            actorInfo.GetEntryByName("FUSHIGIDANE_H").PokemonID = starters.Starter1
            actorInfo.GetEntryByName("HITOKAGE_H").PokemonID = starters.Starter5
            actorInfo.GetEntryByName("ZENIGAME_H").PokemonID = starters.Starter10
            actorInfo.GetEntryByName("PIKACHUU_H").PokemonID = starters.Starter30
            actorInfo.GetEntryByName("PIKACHUU_F_H").PokemonID = starters.Starter30
            actorInfo.GetEntryByName("CHIKORIITA_H").PokemonID = starters.Starter197
            actorInfo.GetEntryByName("HINOARASHI_H").PokemonID = starters.Starter200
            actorInfo.GetEntryByName("WANINOKO_H").PokemonID = starters.Starter203
            actorInfo.GetEntryByName("KIMORI_H").PokemonID = starters.Starter322
            actorInfo.GetEntryByName("ACHAMO_H").PokemonID = starters.Starter325
            actorInfo.GetEntryByName("ACHAMO_F_H").PokemonID = starters.Starter325
            actorInfo.GetEntryByName("MIZUGOROU_H").PokemonID = starters.Starter329
            actorInfo.GetEntryByName("NAETORU_H").PokemonID = starters.Starter479
            actorInfo.GetEntryByName("HIKOZARU_H").PokemonID = starters.Starter482
            actorInfo.GetEntryByName("POTCHAMA_H").PokemonID = starters.Starter485
            actorInfo.GetEntryByName("RIORU_H").PokemonID = starters.Starter537
            actorInfo.GetEntryByName("TSUTAAJA_H").PokemonID = starters.Starter592
            actorInfo.GetEntryByName("POKABU_H").PokemonID = starters.Starter595
            actorInfo.GetEntryByName("MIJUMARU_H").PokemonID = starters.Starter598
            actorInfo.GetEntryByName("HARIMARON_H").PokemonID = starters.Starter766
            actorInfo.GetEntryByName("FOKKO_H").PokemonID = starters.Starter769
            actorInfo.GetEntryByName("KEROMATSU_H").PokemonID = starters.Starter772
            Await actorInfo.Save(CurrentPluginManager.CurrentIOProvider)
        End Function


        Private Function GetCustomPersonalityTestScriptPsmd(starters As StarterDefinitionsPsmd) As String
            Dim starterscriptContent = My.Resources.PSMDStarterIntroScript
            starterscriptContent = starterscriptContent.Replace("#Starter1#", starters.Starter1.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter5#", starters.Starter5.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter10#", starters.Starter10.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter30#", starters.Starter30.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter197#", starters.Starter197.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter200#", starters.Starter200.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter203#", starters.Starter203.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter322#", starters.Starter322.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter325#", starters.Starter325.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter329#", starters.Starter329.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter479#", starters.Starter479.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter482#", starters.Starter482.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter485#", starters.Starter485.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter537#", starters.Starter537.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter592#", starters.Starter592.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter595#", starters.Starter595.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter598#", starters.Starter598.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter766#", starters.Starter766.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter769#", starters.Starter769.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter772#", starters.Starter772.ToString)

            Dim pokemonNameHashes = MessageBin.PsmdPokemonNameHashes.Replace(vbCrLf, vbLf).Split(vbLf).Select(Function(x) Integer.Parse(x.Trim))
            starterscriptContent = starterscriptContent.Replace("#StarterHash1#", pokemonNameHashes(starters.Starter1 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash5#", pokemonNameHashes(starters.Starter5 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash10#", pokemonNameHashes(starters.Starter10 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash30#", pokemonNameHashes(starters.Starter30 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash197#", pokemonNameHashes(starters.Starter197 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash200#", pokemonNameHashes(starters.Starter200 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash203#", pokemonNameHashes(starters.Starter203 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash322#", pokemonNameHashes(starters.Starter322 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash325#", pokemonNameHashes(starters.Starter325 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash329#", pokemonNameHashes(starters.Starter329 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash479#", pokemonNameHashes(starters.Starter479 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash482#", pokemonNameHashes(starters.Starter482 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash485#", pokemonNameHashes(starters.Starter485 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash537#", pokemonNameHashes(starters.Starter537 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash592#", pokemonNameHashes(starters.Starter592 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash595#", pokemonNameHashes(starters.Starter595 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash598#", pokemonNameHashes(starters.Starter598 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash766#", pokemonNameHashes(starters.Starter766 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash769#", pokemonNameHashes(starters.Starter769 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash772#", pokemonNameHashes(starters.Starter772 - 1))

            Return starterscriptContent
        End Function

        Private Sub ReplacePersonalityTestScriptPsmd(starters As StarterDefinitionsPsmd)
            CurrentIOProvider.WriteAllText(IO.Path.Combine(Me.GetRootDirectory, "script", "event", "other", "seikakushindan", "seikakushindan.lua"), GetCustomPersonalityTestScriptPsmd(starters))
        End Sub

        Private Async Function UpdateLanguageFilesForCustomPersonalityTestScriptPsmd() As Task
            Dim enLangFile = IO.Path.Combine(Me.GetRootDirectory, "Languages", "en", "seikakushindan.bin")
            Dim frLangFile = IO.Path.Combine(Me.GetRootDirectory, "Languages", "fr", "seikakushindan.bin")
            Dim geLangFile = IO.Path.Combine(Me.GetRootDirectory, "Languages", "ge", "seikakushindan.bin")
            Dim itLangFile = IO.Path.Combine(Me.GetRootDirectory, "Languages", "it", "seikakushindan.bin")
            Dim jpLangFile = IO.Path.Combine(Me.GetRootDirectory, "Languages", "jp", "seikakushindan.bin")
            Dim spLangFile = IO.Path.Combine(Me.GetRootDirectory, "Languages", "sp", "seikakushindan.bin")
            Dim usLangFile = IO.Path.Combine(Me.GetRootDirectory, "Languages", "us", "seikakushindan.bin")

            For Each langFilename In {enLangFile, geLangFile, itLangFile, jpLangFile, spLangFile, usLangFile}
                If File.Exists(langFilename) Then
                    Using langFile As New MessageBin()
                        Await langFile.OpenFile(langFilename, CurrentPluginManager.CurrentIOProvider)
                        If Not langFile.Strings.Any(Function(x) x.Hash = 200000) Then
                            langFile.Strings.Add(New MessageBinStringEntry() With {.Hash = 200000, .Entry = "Hero: \D301" & vbLf & "Partner: \D302"})
                            langFile.Strings.Add(New MessageBinStringEntry() With {.Hash = 200001, .Entry = "Done"})
                            langFile.Strings.Add(New MessageBinStringEntry() With {.Hash = 200002, .Entry = "Set Hero"})
                            langFile.Strings.Add(New MessageBinStringEntry() With {.Hash = 200003, .Entry = "Set Partner"})
                            langFile.Strings.Add(New MessageBinStringEntry() With {.Hash = 200004, .Entry = "Setting Hero..."})
                            langFile.Strings.Add(New MessageBinStringEntry() With {.Hash = 200005, .Entry = "Setting Partner..."})
                        End If
                        Await langFile.Save(CurrentPluginManager.CurrentIOProvider)
                    End Using
                End If
            Next

            If File.Exists(frLangFile) Then
                Using langFile As New MessageBin()
                    Await langFile.OpenFile(frLangFile, CurrentPluginManager.CurrentIOProvider)
                    If Not langFile.Strings.Any(Function(x) x.Hash = 200000) Then
                        langFile.Strings.Add(New MessageBinStringEntry() With {.Hash = 200000, .Entry = "L'héro: \D301" & vbLf & "Le partenaire: \D302"})
                        langFile.Strings.Add(New MessageBinStringEntry() With {.Hash = 200001, .Entry = "Fini"})
                        langFile.Strings.Add(New MessageBinStringEntry() With {.Hash = 200002, .Entry = "Définir l'héros"})
                        langFile.Strings.Add(New MessageBinStringEntry() With {.Hash = 200003, .Entry = "Définir le partenaire"})
                        langFile.Strings.Add(New MessageBinStringEntry() With {.Hash = 200004, .Entry = "Définir le héros..."})
                        langFile.Strings.Add(New MessageBinStringEntry() With {.Hash = 200005, .Entry = "Définir le partenaire..."})
                    End If
                    Await langFile.Save(CurrentPluginManager.CurrentIOProvider)
                End Using
            End If

        End Function

        ''' <summary>
        ''' Patches inc_charchoice.lua for use with reporting the results of the custom personality test result screen
        ''' </summary>
        Private Async Function FixIncCharChoiceScriptPsmd(starters As StarterDefinitionsPsmd) As Task
            'Patch inc_charchoice script
            '-Get the hashes of the scripts to change
            Dim charchoiceRegex = New Regex("function char([A-Z]+)\(\)\s*WINDOW\:SysMsg\((-?[0-9]+)\)", RegexOptions.IgnoreCase)
            Dim sourceScript = File.ReadAllText(Path.Combine(Me.GetRootDirectory, "script", "include", "inc_charchoice.lua.original"))
            Dim charchoiceMatches = charchoiceRegex.Matches(sourceScript)
            Dim charchoiceData As New Dictionary(Of String, Integer) 'Key: Internal name, Value: String Hash
            For Each item As Match In charchoiceMatches
                charchoiceData.Add(item.Groups(1).Value, Integer.Parse(item.Groups(2).Value))
            Next
            sourceScript = sourceScript.Replace("SysMsg", "ExplanationB")
            File.WriteAllText(Path.Combine(Me.GetRootDirectory, "script", "include", "inc_charchoice.lua"), sourceScript)

            '-Patch the script text
            Dim charchoiceLanguageTemplates As New Dictionary(Of String, String) 'Key: Language name, Value: template
            charchoiceLanguageTemplates.Add("en", "\C200{0}!")
            charchoiceLanguageTemplates.Add("fr", "\C200{0} !")
            charchoiceLanguageTemplates.Add("ge", "\C200...{0}!")
            charchoiceLanguageTemplates.Add("it", "\C200{0}!")
            charchoiceLanguageTemplates.Add("jp", "\C200{0}だ\FF01")
            charchoiceLanguageTemplates.Add("sp", "\C200¡{0}!")
            charchoiceLanguageTemplates.Add("us", "\C200{0}!")
            For Each charchoiceLanguageTemplate In charchoiceLanguageTemplates
                If IO.Directory.Exists(IO.Path.Combine(Me.GetRootDirectory, "Languages", charchoiceLanguageTemplate.Key)) Then
                    'Get Pokemon names to work with
                    Dim common As New MessageBin
                    Await common.OpenFile(IO.Path.Combine(Me.GetRootDirectory, "Languages", charchoiceLanguageTemplate.Key, "common.bin"), CurrentPluginManager.CurrentIOProvider)
                    Dim pokemonNames = common.GetPsmdCommonPokemonNames

                    'Open the message bin file
                    Dim charchoiceFile As New MessageBin
                    Await charchoiceFile.OpenFile(IO.Path.Combine(Me.GetRootDirectory, "Languages", charchoiceLanguageTemplate.Key, "inc_charchoice.bin"), CurrentPluginManager.CurrentIOProvider)

                    For Each charchoice In charchoiceData
                        Dim pokemonID As Integer
                        Select Case charchoice.Key
                            Case "FUSHIGIDANE"
                                pokemonID = starters.Starter1
                            Case "HITOKAGE"
                                pokemonID = starters.Starter5
                            Case "ZENIGAME"
                                pokemonID = starters.Starter10
                            Case "PIKACHUU"
                                pokemonID = starters.Starter30
                            Case "CHIKORIITA"
                                pokemonID = starters.Starter197
                            Case "HINOARASHI"
                                pokemonID = starters.Starter200
                            Case "WANINOKO"
                                pokemonID = starters.Starter203
                            Case "KIMORI"
                                pokemonID = starters.Starter322
                            Case "ACHAMO"
                                pokemonID = starters.Starter325
                            Case "MIZUGOROU"
                                pokemonID = starters.Starter329
                            Case "NAETORU"
                                pokemonID = starters.Starter479
                            Case "HIKOZARU"
                                pokemonID = starters.Starter482
                            Case "POTCHAMA"
                                pokemonID = starters.Starter485
                            Case "RIORU"
                                pokemonID = starters.Starter537
                            Case "TSUTAAJA"
                                pokemonID = starters.Starter592
                            Case "POKABU"
                                pokemonID = starters.Starter595
                            Case "MIJUMARU"
                                pokemonID = starters.Starter598
                            Case "HARIMARON"
                                pokemonID = starters.Starter766
                            Case "FOKKO"
                                pokemonID = starters.Starter769
                            Case "KEROMATSU"
                                pokemonID = starters.Starter772
                            Case Else
                                'Unknown Pokemon name.  Make no changes.
                                Exit For
                        End Select

                        'Patch the message bin
                        Dim charchoiceEntry = charchoiceFile.Strings.Where(Function(x) x.HashSigned = charchoice.Value).Single
                        charchoiceEntry.Entry = String.Format(charchoiceLanguageTemplate.Value, pokemonNames(pokemonID))
                    Next

                    'Save the file
                    Await charchoiceFile.Save(CurrentPluginManager.CurrentIOProvider)
                End If
            Next
        End Function

        Private Sub FixHarmonyScarfGlowPsmd()
            Dim scarfGlowRegex = New Regex("(if(.|\n)*?)(else\s(.|\n)*?)(end)", RegexOptions.Compiled Or RegexOptions.IgnoreCase)
            Dim scarfGlowScripts = {"script/event/main15/120_enteishujinkoushinkaboss1st/enteishujinkoushinkaboss1st.lua"}
            For Each item In scarfGlowScripts
                Dim scriptContent = CurrentIOProvider.ReadAllText(Path.Combine(Me.GetRootDirectory, item))

                For Each targetSection As Match In scarfGlowRegex.Matches(scriptContent)
                    Dim newContent As New StringBuilder

                    'Keep the first if statements as-is
                    newContent.Append(targetSection.Groups(1).Value)

                    'Copy/paste the else statement for each normal starter type
                    'Some of these IDs are already in one of the above If statements. They can probably be removed. Leaving them here in case other scripts are different.
                    For Each starterId In {1, 5, 10, 30, 197, 200, 203, 322, 325, 329, 479, 482, 485, 537, 592, 595, 598, 766, 769, 722}
                        newContent.Append(targetSection.Groups(3).Value.Replace("else", "elseif pokemonIndexHero == " & starterId.ToString & " then"))
                    Next

                    'Change the default value for the else statement
                    newContent.Append(targetSection.Groups(3).Value.Replace("BODY_POINT.SCARF", "BODY_POINT.CENTER"))

                    'The end statement
                    newContent.Append(targetSection.Groups(5).Value)

                    scriptContent = scriptContent.Replace(targetSection.Value, newContent.ToString())
                Next

                CurrentIOProvider.WriteAllText(Path.Combine(Me.GetRootDirectory, item), scriptContent)
            Next
        End Sub

#End Region

        Public Overrides Async Function Initialize() As Task
            Await MyBase.Initialize()

            Me.EnableModelPatching = True
            Me.EnablePortraitPatching = True

            'Add fixed_pokemon to project, so we can edit it with our UI
            Me.AddExistingFile("", Path.Combine(Me.GetRawFilesDir, "romfs", "dungeon", "fixed_pokemon.bin"), CurrentIOProvider)

            Me.IsCompleted = True
        End Function

        Public Overrides Async Function Build() As Task
            'Copy the project's fixed_pokemon.bin to the RawFiles directory, so the parent class can generate a patch
            Dim fpFilename = Me.GetItem("fixed_pokemon.bin").Filename
            File.Copy(fpFilename, Path.Combine(Me.GetRawFilesDir, "romfs", "dungeon", "fixed_pokemon.bin"), True)

            'Non Pokemon-dependent patches
            Dim pgdb As New PGDB
            Await pgdb.OpenFile(Path.Combine(Me.GetRawFilesDir, "romfs", "pokemon_graphics_database.bin"), CurrentIOProvider)

            Await FixPokemonWithDummyModel(pgdb)

            If IsPsmd Then
                If EnableModelPatching Then
                    Await AddMissingModelAnimationsPsmd(pgdb)
                End If

                If EnablePortraitPatching Then
                    Await SubstituteMissingPortraitsPsmd()
                End If

                'Pokemon-dependent patches
                Dim fixedPokemon As New FixedPokemon()
                Await fixedPokemon.OpenFile(fpFilename, Me.CurrentIOProvider)
                Dim starters = New StarterDefinitionsPsmd(fixedPokemon)

                ReplacePersonalityTestScriptPsmd(starters)
                Await FixHighResModelsPsmd(starters)
                Await UpdateLanguageFilesForCustomPersonalityTestScriptPsmd()
                Await FixIncCharChoiceScriptPsmd(starters)
                Await FixPokemonIDsInScripts(starters)
                FixHarmonyScarfGlowPsmd()

            ElseIf IsGti Then
                If EnableModelPatching Then
                    Await AddMissingModelAnimationsGti(pgdb)
                End If

                If EnablePortraitPatching Then
                    Await SubstituteMissingPortraitsGti()
                End If

                'Pokemon-dependent patches
                Dim fixedPokemon As New FixedPokemon()
                Await fixedPokemon.OpenFile(fpFilename, Me.CurrentIOProvider)
                Dim starters = New StarterDefinitionsGti(fixedPokemon)

                FixPersonalityTestGti(starters)
                Await FixCodeBinGti(starters)
                Await FixHighResModelsGti(starters)
                Await FixPokemonIDsInScripts(starters)
            Else
                Throw New Exception("Unexpected ROM title ID: " & GetTitleId())
            End If

            'Continue the build (script compilation, mod building, etc.)
            Await MyBase.Build()
        End Function

        Private MustInherit Class StarterDefinitions
            Public MustOverride Function GetReplacementDictionary() As Dictionary(Of Integer, Integer)
            Public MustOverride Function GetEvoDictionary() As Dictionary(Of Integer, Integer)
        End Class

        Private Class StarterDefinitionsPsmd
            Inherits StarterDefinitions
            Public Sub New(fixedPokemon As FixedPokemon)
                'Select just what we want.  Note that the numbers in the variable names are the original, unmodified values
                'It is safe to use static indexes (at least for now) because the game uses them too.
                'If it is ever discovered where the game stores the indexes, refactoring will need to be done.
                Starter1 = fixedPokemon.Entries(19).PokemonID 'FUSHIGIDANE_H (Bulbasaur)
                Starter5 = fixedPokemon.Entries(21).PokemonID 'HITOKAGE_H (Charmander)
                Starter10 = fixedPokemon.Entries(23).PokemonID 'ZENIGAME_H (Squirtle)
                Starter30 = fixedPokemon.Entries(17).PokemonID 'PIKACHUU_H (Pikachu)
                Starter197 = fixedPokemon.Entries(25).PokemonID 'CHIKORIITA_H (Chikorita)
                Starter200 = fixedPokemon.Entries(27).PokemonID 'HINOARASHI_H (Cyndaquil)
                Starter203 = fixedPokemon.Entries(29).PokemonID  'WANINOKO_H (Totodile)
                Starter322 = fixedPokemon.Entries(31).PokemonID  'KIMORI_H (Treecko)
                Starter325 = fixedPokemon.Entries(33).PokemonID 'ACHAMO_H (Torchic)
                Starter329 = fixedPokemon.Entries(35).PokemonID 'MIZUGOROU_H (Mudkip)
                Starter479 = fixedPokemon.Entries(37).PokemonID 'NAETORU_H (Turtwig)
                Starter482 = fixedPokemon.Entries(39).PokemonID 'HIKOZARU_H (Chimchar)
                Starter485 = fixedPokemon.Entries(41).PokemonID  'POTCHAMA_H (Piplup)
                Starter537 = fixedPokemon.Entries(18).PokemonID 'RIORU_H (Riolu)
                Starter592 = fixedPokemon.Entries(43).PokemonID  'TSUTAAJA_H (Snivy)
                Starter595 = fixedPokemon.Entries(45).PokemonID  'POKABU_H (Tepig)
                Starter598 = fixedPokemon.Entries(47).PokemonID  'MIJUMARU_H (Oshawott)
                Starter766 = fixedPokemon.Entries(49).PokemonID  'HARIMARON_H (Chespin)
                Starter769 = fixedPokemon.Entries(51).PokemonID  'FOKKO_H (Fennekin)
                Starter772 = fixedPokemon.Entries(53).PokemonID  'KEROMATSU_H (Froakie)

                Evo1 = fixedPokemon.Entries(57).PokemonID '(Bulbasaur)
                Evo5 = fixedPokemon.Entries(58).PokemonID '(Charmander)
                Evo10 = fixedPokemon.Entries(59).PokemonID '(Squirtle)
                Evo30 = fixedPokemon.Entries(55).PokemonID '(Pikachu)
                Evo197 = fixedPokemon.Entries(60).PokemonID '(Chikorita)
                Evo200 = fixedPokemon.Entries(61).PokemonID '(Cyndaquil)
                Evo203 = fixedPokemon.Entries(62).PokemonID '(Totodile)
                Evo322 = fixedPokemon.Entries(63).PokemonID '(Treecko)
                Evo325 = fixedPokemon.Entries(64).PokemonID '(Torchic)
                Evo329 = fixedPokemon.Entries(65).PokemonID '(Mudkip)
                Evo479 = fixedPokemon.Entries(66).PokemonID '(Turtwig)
                Evo482 = fixedPokemon.Entries(67).PokemonID '(Chimchar)
                Evo485 = fixedPokemon.Entries(68).PokemonID '(Piplup)
                Evo537 = fixedPokemon.Entries(56).PokemonID '(Riolu)
                Evo592 = fixedPokemon.Entries(69).PokemonID '(Snivy)
                Evo595 = fixedPokemon.Entries(70).PokemonID '(Tepig)
                Evo598 = fixedPokemon.Entries(71).PokemonID '(Oshawott)
                Evo766 = fixedPokemon.Entries(72).PokemonID '(Chespin)
                Evo769 = fixedPokemon.Entries(73).PokemonID '(Fennekin)
                Evo772 = fixedPokemon.Entries(74).PokemonID '(Froakie)
            End Sub

            Public Starter1 As Integer
            Public Starter5 As Integer
            Public Starter10 As Integer
            Public Starter30 As Integer
            Public Starter197 As Integer
            Public Starter200 As Integer
            Public Starter203 As Integer
            Public Starter322 As Integer
            Public Starter325 As Integer
            Public Starter329 As Integer
            Public Starter479 As Integer
            Public Starter482 As Integer
            Public Starter485 As Integer
            Public Starter537 As Integer
            Public Starter592 As Integer
            Public Starter595 As Integer
            Public Starter598 As Integer
            Public Starter766 As Integer
            Public Starter769 As Integer
            Public Starter772 As Integer

            Public Evo1 As Integer
            Public Evo5 As Integer
            Public Evo10 As Integer
            Public Evo30 As Integer
            Public Evo197 As Integer
            Public Evo200 As Integer
            Public Evo203 As Integer
            Public Evo322 As Integer
            Public Evo325 As Integer
            Public Evo329 As Integer
            Public Evo479 As Integer
            Public Evo482 As Integer
            Public Evo485 As Integer
            Public Evo537 As Integer
            Public Evo592 As Integer
            Public Evo595 As Integer
            Public Evo598 As Integer
            Public Evo766 As Integer
            Public Evo769 As Integer
            Public Evo772 As Integer

            ''' <summary>
            ''' Gets a dictionary representing matching new starter Pokemon IDs to the old.
            ''' Key: old ID, value: new ID
            ''' </summary>
            Public Overrides Function GetReplacementDictionary() As Dictionary(Of Integer, Integer)
                Dim replacementDictionary As New Dictionary(Of Integer, Integer) 'Key: original ID, Value: edited ID
                replacementDictionary.Add(1, Starter1)
                replacementDictionary.Add(5, Starter5)
                replacementDictionary.Add(10, Starter10)
                replacementDictionary.Add(30, Starter30)
                replacementDictionary.Add(197, Starter197)
                replacementDictionary.Add(200, Starter200)
                replacementDictionary.Add(203, Starter203)
                replacementDictionary.Add(322, Starter322)
                replacementDictionary.Add(325, Starter325)
                replacementDictionary.Add(329, Starter329)
                replacementDictionary.Add(479, Starter479)
                replacementDictionary.Add(482, Starter482)
                replacementDictionary.Add(485, Starter485)
                replacementDictionary.Add(537, Starter537)
                replacementDictionary.Add(592, Starter592)
                replacementDictionary.Add(595, Starter595)
                replacementDictionary.Add(598, Starter598)
                replacementDictionary.Add(766, Starter766)
                replacementDictionary.Add(769, Starter769)
                replacementDictionary.Add(772, Starter772)
                Return replacementDictionary
            End Function

            ''' <summary>
            ''' Gets a dictionary representing matching new Harmony Scarf Evolution Pokemon IDs to the old.
            ''' Key: old ID, value: new ID
            ''' </summary>
            Public Overrides Function GetEvoDictionary() As Dictionary(Of Integer, Integer)
                Dim evoDictionary As New Dictionary(Of Integer, Integer) 'Key: original evo ID, value: new evo ID
                evoDictionary.Add(6, Evo1)
                evoDictionary.Add(7, Evo5)
                evoDictionary.Add(12, Evo10)
                evoDictionary.Add(31, Evo30)
                evoDictionary.Add(199, Evo197)
                evoDictionary.Add(202, Evo200)
                evoDictionary.Add(205, Evo203)
                evoDictionary.Add(324, Evo322)
                evoDictionary.Add(327, Evo325)
                evoDictionary.Add(331, Evo329)
                evoDictionary.Add(481, Evo479)
                evoDictionary.Add(484, Evo482)
                evoDictionary.Add(487, Evo485)
                evoDictionary.Add(538, Evo537)
                evoDictionary.Add(594, Evo592)
                evoDictionary.Add(597, Evo595)
                evoDictionary.Add(600, Evo598)
                evoDictionary.Add(768, Evo766)
                evoDictionary.Add(771, Evo769)
                evoDictionary.Add(774, Evo772)
                Return evoDictionary
            End Function
        End Class

        Private Class StarterDefinitionsGti
            Inherits StarterDefinitions
            Public Sub New(fixedPokemon As FixedPokemon)
                'Starter1 = fixedPokemon.Entries(71).PokemonID
                Starter39 = fixedPokemon.Entries(72).PokemonID
                'Starter42 = fixedPokemon.Entries(73).PokemonID
                Starter45 = fixedPokemon.Entries(74).PokemonID
                Starter122 = fixedPokemon.Entries(75).PokemonID
            End Sub

            'Public Starter1 As Integer
            Public Starter39 As Integer
            'Public Starter42 As Integer
            Public Starter45 As Integer
            Public Starter122 As Integer

            ''' <summary>
            ''' Gets a dictionary representing matching new starter Pokemon IDs to the old.
            ''' Key: old ID, value: new ID
            ''' </summary>
            Public Overrides Function GetReplacementDictionary() As Dictionary(Of Integer, Integer)
                Dim replacementDictionary As New Dictionary(Of Integer, Integer) 'Key: original ID, Value: edited ID
                'replacementDictionary.Add(1, Starter1)
                replacementDictionary.Add(39, Starter39)
                'replacementDictionary.Add(42, Starter42)
                replacementDictionary.Add(45, Starter45)
                replacementDictionary.Add(122, Starter122)
                Return replacementDictionary
            End Function

            Public Overrides Function GetEvoDictionary() As Dictionary(Of Integer, Integer)
                Return New Dictionary(Of Integer, Integer)
            End Function
        End Class

    End Class
End Namespace

