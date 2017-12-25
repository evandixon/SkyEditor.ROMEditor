Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Text.RegularExpressions
Imports SkyEditor.Core.Projects
Imports SkyEditor.Core.Utilities
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD.Dungeon
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD.Pokemon

Namespace MysteryDungeon.PSMD.Projects
    ''' <summary>
    ''' Mod for PSMD that allows editing playable starter Pokémon.
    ''' </summary>
    Public Class PsmdStarterMod
        Inherits PsmdLuaProject

        Public Sub New()
            MyBase.New
            Me.AddScriptsToProject = False
        End Sub

        Public Overrides Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {GameStrings.PSMDCode}
        End Function

        Public Overrides Function GetFilesToCopy(solution As Solution, baseRomProjectName As String) As IEnumerable(Of String)
            Return {Path.Combine("romfs", "script"),
                    Path.Combine("romfs", "message_en.bin"),
                    Path.Combine("romfs", "message_fr.bin"),
                    Path.Combine("romfs", "message_ge.bin"),
                    Path.Combine("romfs", "message_it.bin"),
                    Path.Combine("romfs", "message_sp.bin"),
                    Path.Combine("romfs", "message_us.bin"),
                    Path.Combine("romfs", "message.bin"),
                    Path.Combine("romfs", "message_en.lst"),
                    Path.Combine("romfs", "message_fr.lst"),
                    Path.Combine("romfs", "message_ge.lst"),
                    Path.Combine("romfs", "message_it.lst"),
                    Path.Combine("romfs", "message_sp.lst"),
                    Path.Combine("romfs", "message_us.lst"),
                    Path.Combine("romfs", "message.lst"),
                    Path.Combine("romfs", "dungeon", "fixed_pokemon.bin"),
                    Path.Combine("romfs", "pokemon", "pokemon_actor_data_info.bin"),
                    Path.Combine("romfs", "face_graphic.bin"),
                    Path.Combine("romfs", "message_debug.bin"),
                    Path.Combine("romfs", "message_debug.lst"),
                    Path.Combine("romfs", "pokemon_graphics_database.bin")}
        End Function

        Public Overrides Async Function Initialize() As Task
            Await MyBase.Initialize()

            'Add fixed_pokemon to project
            Me.AddExistingFile("", Path.Combine(Me.GetRawFilesDir, "romfs", "dungeon", "fixed_pokemon.bin"), CurrentPluginManager.CurrentIOProvider)

            'Extract face_graphic.bin
            Me.Message = My.Resources.Language.LoadingExtractingPortraits
            Me.Progress = 0
            Me.IsIndeterminate = False

            Dim provider = CurrentPluginManager.CurrentIOProvider
            Dim faceFarc As New Farc
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

            'Use default portrait for placeholders
            Const samplePokemon = "amaruruga" 'This is largly arbitrary
            Dim blankPortrait01 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, "erureido_mega", "erureido_mega" & "_01.png"))
            Dim blankPortrait02 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, "erureido_mega", "erureido_mega" & "_02.png"))
            Dim blankPortrait03 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_03.png"))
            Dim blankPortrait04 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_04.png"))
            Dim blankPortrait05 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_05.png"))
            Dim blankPortrait06 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_06.png"))
            Dim blankPortrait07 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_07.png"))
            Dim blankPortrait08 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_08.png"))
            Dim blankPortrait09 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_09.png"))
            Dim blankPortrait10 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_10.png"))
            Dim blankPortrait11 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_11.png"))
            Dim blankPortrait12 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_12.png"))
            Dim blankPortrait13 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_13.png"))
            Dim blankPortrait14 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_14.png"))
            Dim blankPortrait15 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_15.png"))
            Dim blankPortrait16 = File.ReadAllBytes(Path.Combine(Me.GetRootDirectory, samplePokemon, samplePokemon & "_16.png"))

            Dim defaultPortraitTransformRegex As New Regex("(([a-z0-9]|_)+)_[0-9]{2}\.png")
            Directory.GetDirectories(Me.GetRootDirectory, "*", SearchOption.TopDirectoryOnly).AsParallel().ForAll(
                    Sub(pkmDir)
                        For Each imagePath In Directory.GetFiles(pkmDir, "*.png", SearchOption.TopDirectoryOnly)
                            Dim sample As Byte() = Nothing
                            If imagePath.EndsWith("_01.png") Then
                                sample = blankPortrait01
                            ElseIf imagePath.EndsWith("_02.png") Then
                                sample = blankPortrait02
                            ElseIf imagePath.EndsWith("_03.png") Then
                                sample = blankPortrait03
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
                            ElseIf imagePath.EndsWith("_11.png") Then
                                sample = blankPortrait11
                            ElseIf imagePath.EndsWith("_12.png") Then
                                sample = blankPortrait12
                            ElseIf imagePath.EndsWith("_13.png") Then
                                sample = blankPortrait13
                            ElseIf imagePath.EndsWith("_14.png") Then
                                sample = blankPortrait14
                            ElseIf imagePath.EndsWith("_15.png") Then
                                sample = blankPortrait15
                            ElseIf imagePath.EndsWith("_16.png") Then
                                sample = blankPortrait16
                            End If

                            If sample IsNot Nothing AndAlso defaultPortraitTransformRegex.IsMatch(imagePath) Then
                                If File.ReadAllBytes(imagePath).SequenceEqual(sample) Then
                                    Dim defaultPortraitPath = Path.Combine(Path.GetDirectoryName(imagePath), defaultPortraitTransformRegex.Match(imagePath).Groups(1).Value & "_00.png")
                                    If File.Exists(defaultPortraitPath) Then
                                        File.Copy(defaultPortraitPath, imagePath, True)
                                    End If
                                End If
                            End If
                        Next
                    End Sub
            )

            Dim f As New Farc()
            f.CreateFile()

            Dim onProgressed2 = Sub(sender As Object, e As ProgressReportedEventArgs)
                                    Me.Message = My.Resources.Language.LoadingBuildingPortraits
                                    Me.Progress = e.Progress
                                    Me.IsIndeterminate = False
                                End Sub

            Dim a2 = New AsyncFor
            AddHandler a2.ProgressChanged, onProgressed2
            Await a2.RunForEach(Directory.GetFiles(Me.GetRootDirectory, "*.png", SearchOption.AllDirectories),
                               Sub(portrait As String)
                                   Using img As New Bitmap(portrait)
                                       f.WriteAllBytes(Path.GetFileNameWithoutExtension(portrait) & ".bin", PmdGraphics.SavePortrait(img))
                                   End Using
                               End Sub)
            RemoveHandler a2.ProgressChanged, onProgressed2

            Await f.Save(Path.Combine(Me.GetRawFilesDir, "romfs", "face_graphic.bin"), CurrentPluginManager.CurrentIOProvider)

            Me.IsCompleted = True
        End Function

        Public Overrides Async Function Build() As Task
            'Open fixed_pokemon
            Dim fpFilename = Me.GetItem("fixed_pokemon.bin").Filename
            Dim fixedPokemon As New FixedPokemon()
            Await fixedPokemon.OpenFile(fpFilename, Me.CurrentPluginManager.CurrentIOProvider)
            File.Copy(fpFilename, Path.Combine(Me.GetRawFilesDir, "romfs", "dungeon", "fixed_pokemon.bin"), True)

            'Select just what we want.  Note that the numbers in the variable names are the original, unmodified values
            'It is safe to use static indexes (at least for now) because the game uses them too.
            'If it is ever discovered where the game stores the indexes, refactoring will need to be done.
            Dim starter1 As Integer = fixedPokemon.Entries(19).PokemonID 'FUSHIGIDANE_H (Bulbasaur)
            Dim starter5 As Integer = fixedPokemon.Entries(21).PokemonID 'HITOKAGE_H (Charmander)
            Dim starter10 As Integer = fixedPokemon.Entries(23).PokemonID 'ZENIGAME_H (Squirtle)
            Dim starter30 As Integer = fixedPokemon.Entries(17).PokemonID 'PIKACHUU_H (Pikachu)
            Dim starter197 As Integer = fixedPokemon.Entries(25).PokemonID 'CHIKORIITA_H (Chikorita)
            Dim starter200 As Integer = fixedPokemon.Entries(27).PokemonID 'HINOARASHI_H (Cyndaquil)
            Dim starter203 As Integer = fixedPokemon.Entries(29).PokemonID  'WANINOKO_H (Totodile)
            Dim starter322 As Integer = fixedPokemon.Entries(31).PokemonID  'KIMORI_H (Treecko)
            Dim starter325 As Integer = fixedPokemon.Entries(33).PokemonID 'ACHAMO_H (Torchic)
            Dim starter329 As Integer = fixedPokemon.Entries(35).PokemonID 'MIZUGOROU_H (Mudkip)
            Dim starter479 As Integer = fixedPokemon.Entries(37).PokemonID 'NAETORU_H (Turtwig)
            Dim starter482 As Integer = fixedPokemon.Entries(39).PokemonID 'HIKOZARU_H (Chimchar)
            Dim starter485 As Integer = fixedPokemon.Entries(41).PokemonID  'POTCHAMA_H (Piplup)
            Dim starter537 As Integer = fixedPokemon.Entries(18).PokemonID 'RIORU_H (Riolu)
            Dim starter592 As Integer = fixedPokemon.Entries(43).PokemonID  'TSUTAAJA_H (Snivy)
            Dim starter595 As Integer = fixedPokemon.Entries(45).PokemonID  'POKABU_H (Tepig)
            Dim starter598 As Integer = fixedPokemon.Entries(47).PokemonID  'MIJUMARU_H (Oshawott)
            Dim starter766 As Integer = fixedPokemon.Entries(49).PokemonID  'HARIMARON_H (Chespin)
            Dim starter769 As Integer = fixedPokemon.Entries(51).PokemonID  'FOKKO_H (Fennekin)
            Dim starter772 As Integer = fixedPokemon.Entries(53).PokemonID  'KEROMATSU_H (Froakie)
            'These have high res actors, but are not starters.  May or may not be significant to starter patching
            'Dim starter167 As Integer 'IIBUI_H (Eevee)
            'Dim starter352 As Integer 'RARUTOSU_H (Ralts)

            Dim evo1 As Integer = fixedPokemon.Entries(57).PokemonID '(Bulbasaur)
            Dim evo5 As Integer = fixedPokemon.Entries(58).PokemonID '(Charmander)
            Dim evo10 As Integer = fixedPokemon.Entries(59).PokemonID '(Squirtle)
            Dim evo30 As Integer = fixedPokemon.Entries(55).PokemonID '(Pikachu)
            Dim evo197 As Integer = fixedPokemon.Entries(60).PokemonID '(Chikorita)
            Dim evo200 As Integer = fixedPokemon.Entries(61).PokemonID '(Cyndaquil)
            Dim evo203 As Integer = fixedPokemon.Entries(62).PokemonID '(Totodile)
            Dim evo322 As Integer = fixedPokemon.Entries(63).PokemonID '(Treecko)
            Dim evo325 As Integer = fixedPokemon.Entries(64).PokemonID '(Torchic)
            Dim evo329 As Integer = fixedPokemon.Entries(65).PokemonID '(Mudkip)
            Dim evo479 As Integer = fixedPokemon.Entries(66).PokemonID '(Turtwig)
            Dim evo482 As Integer = fixedPokemon.Entries(67).PokemonID '(Chimchar)
            Dim evo485 As Integer = fixedPokemon.Entries(68).PokemonID '(Piplup)
            Dim evo537 As Integer = fixedPokemon.Entries(56).PokemonID '(Riolu)
            Dim evo592 As Integer = fixedPokemon.Entries(69).PokemonID '(Snivy)
            Dim evo595 As Integer = fixedPokemon.Entries(70).PokemonID '(Tepig)
            Dim evo598 As Integer = fixedPokemon.Entries(71).PokemonID '(Oshawott)
            Dim evo766 As Integer = fixedPokemon.Entries(72).PokemonID '(Chespin)
            Dim evo769 As Integer = fixedPokemon.Entries(73).PokemonID '(Fennekin)
            Dim evo772 As Integer = fixedPokemon.Entries(74).PokemonID '(Froakie)

            Dim replacementDictionary As New Dictionary(Of Integer, Integer) 'Key: original ID, Value: edited ID
            replacementDictionary.Add(1, starter1)
            replacementDictionary.Add(5, starter5)
            replacementDictionary.Add(10, starter10)
            replacementDictionary.Add(30, starter30)
            replacementDictionary.Add(197, starter197)
            replacementDictionary.Add(200, starter200)
            replacementDictionary.Add(203, starter203)
            replacementDictionary.Add(322, starter322)
            replacementDictionary.Add(325, starter325)
            replacementDictionary.Add(329, starter329)
            replacementDictionary.Add(479, starter479)
            replacementDictionary.Add(482, starter482)
            replacementDictionary.Add(485, starter485)
            replacementDictionary.Add(537, starter537)
            replacementDictionary.Add(592, starter592)
            replacementDictionary.Add(595, starter595)
            replacementDictionary.Add(598, starter598)
            replacementDictionary.Add(766, starter766)
            replacementDictionary.Add(769, starter769)
            replacementDictionary.Add(772, starter772)

            Dim evoDictionary As New Dictionary(Of Integer, Integer) 'Key: original evo ID, value: new evo ID
            evoDictionary.Add(6, evo1)
            evoDictionary.Add(7, evo5)
            evoDictionary.Add(12, evo10)
            evoDictionary.Add(31, evo30)
            evoDictionary.Add(199, evo197)
            evoDictionary.Add(202, evo200)
            evoDictionary.Add(205, evo203)
            evoDictionary.Add(324, evo322)
            evoDictionary.Add(327, evo325)
            evoDictionary.Add(331, evo329)
            evoDictionary.Add(481, evo479)
            evoDictionary.Add(484, evo482)
            evoDictionary.Add(487, evo485)
            evoDictionary.Add(538, evo537)
            evoDictionary.Add(594, evo592)
            evoDictionary.Add(597, evo595)
            evoDictionary.Add(600, evo598)
            evoDictionary.Add(768, evo766)
            evoDictionary.Add(771, evo769)
            evoDictionary.Add(774, evo772)

            'Patch actor data info
            Dim actorInfo As New ActorDataInfo
            Await actorInfo.OpenFile(IO.Path.Combine(Me.GetRawFilesDir, "romfs", "pokemon", "pokemon_actor_data_info.bin"), CurrentPluginManager.CurrentIOProvider)
            actorInfo.GetEntryByName("FUSHIGIDANE_H").PokemonID = starter1
            actorInfo.GetEntryByName("HITOKAGE_H").PokemonID = starter5
            actorInfo.GetEntryByName("ZENIGAME_H").PokemonID = starter10
            actorInfo.GetEntryByName("PIKACHUU_H").PokemonID = starter30
            actorInfo.GetEntryByName("PIKACHUU_F_H").PokemonID = starter30
            actorInfo.GetEntryByName("CHIKORIITA_H").PokemonID = starter197
            actorInfo.GetEntryByName("HINOARASHI_H").PokemonID = starter200
            actorInfo.GetEntryByName("WANINOKO_H").PokemonID = starter203
            actorInfo.GetEntryByName("KIMORI_H").PokemonID = starter322
            actorInfo.GetEntryByName("ACHAMO_H").PokemonID = starter325
            actorInfo.GetEntryByName("ACHAMO_F_H").PokemonID = starter325
            actorInfo.GetEntryByName("MIZUGOROU_H").PokemonID = starter329
            actorInfo.GetEntryByName("NAETORU_H").PokemonID = starter479
            actorInfo.GetEntryByName("HIKOZARU_H").PokemonID = starter482
            actorInfo.GetEntryByName("POTCHAMA_H").PokemonID = starter485
            actorInfo.GetEntryByName("RIORU_H").PokemonID = starter537
            actorInfo.GetEntryByName("TSUTAAJA_H").PokemonID = starter592
            actorInfo.GetEntryByName("POKABU_H").PokemonID = starter595
            actorInfo.GetEntryByName("MIJUMARU_H").PokemonID = starter598
            actorInfo.GetEntryByName("HARIMARON_H").PokemonID = starter766
            actorInfo.GetEntryByName("FOKKO_H").PokemonID = starter769
            actorInfo.GetEntryByName("KEROMATSU_H").PokemonID = starter772
            Await actorInfo.Save(CurrentPluginManager.CurrentIOProvider)

            'Replace intro sequence with custom script
            ' -- TODO: Figure out how to patch this without storing a modified copy
            Dim starterscriptContent = My.Resources.PSMDStarterIntroScript
            starterscriptContent = starterscriptContent.Replace("#Starter1#", starter1.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter5#", starter5.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter10#", starter10.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter30#", starter30.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter197#", starter197.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter200#", starter200.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter203#", starter203.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter322#", starter322.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter325#", starter325.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter329#", starter329.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter479#", starter479.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter482#", starter482.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter485#", starter485.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter537#", starter537.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter592#", starter592.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter595#", starter595.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter598#", starter598.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter766#", starter766.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter769#", starter769.ToString)
            starterscriptContent = starterscriptContent.Replace("#Starter772#", starter772.ToString)

            Dim pokemonNameHashes = My.Resources.PSMD_Pokemon_Name_Hashes.Replace(vbCrLf, vbLf).Split(vbLf).Select(Function(x) Integer.Parse(x.Trim))
            starterscriptContent = starterscriptContent.Replace("#StarterHash1#", pokemonNameHashes(starter1 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash5#", pokemonNameHashes(starter5 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash10#", pokemonNameHashes(starter10 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash30#", pokemonNameHashes(starter30 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash197#", pokemonNameHashes(starter197 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash200#", pokemonNameHashes(starter200 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash203#", pokemonNameHashes(starter203 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash322#", pokemonNameHashes(starter322 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash325#", pokemonNameHashes(starter325 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash329#", pokemonNameHashes(starter329 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash479#", pokemonNameHashes(starter479 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash482#", pokemonNameHashes(starter482 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash485#", pokemonNameHashes(starter485 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash537#", pokemonNameHashes(starter537 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash592#", pokemonNameHashes(starter592 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash595#", pokemonNameHashes(starter595 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash598#", pokemonNameHashes(starter598 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash766#", pokemonNameHashes(starter766 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash769#", pokemonNameHashes(starter769 - 1))
            starterscriptContent = starterscriptContent.Replace("#StarterHash772#", pokemonNameHashes(starter772 - 1))
            CurrentPluginManager.CurrentIOProvider.WriteAllText(IO.Path.Combine(Me.GetRootDirectory, "script", "event", "other", "seikakushindan", "seikakushindan.lua"), starterscriptContent)

            'Create language resources
            Dim enLangFile = IO.Path.Combine(Me.GetRootDirectory, "Languages", "en", "seikakushindan.bin")
            Dim frLangFile = IO.Path.Combine(Me.GetRootDirectory, "Languages", "fr", "seikakushindan.bin")
            Dim geLangFile = IO.Path.Combine(Me.GetRootDirectory, "Languages", "ge", "seikakushindan.bin")
            Dim itLangFile = IO.Path.Combine(Me.GetRootDirectory, "Languages", "it", "seikakushindan.bin")
            Dim jpLangFile = IO.Path.Combine(Me.GetRootDirectory, "Languages", "jp", "seikakushindan.bin")
            Dim spLangFile = IO.Path.Combine(Me.GetRootDirectory, "Languages", "sp", "seikakushindan.bin")
            Dim usLangFile = IO.Path.Combine(Me.GetRootDirectory, "Languages", "us", "seikakushindan.bin")

            For Each langFilename In {enLangFile, frLangFile, geLangFile, itLangFile, jpLangFile, spLangFile, usLangFile}
                If IO.File.Exists(langFilename) Then
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
                    Dim pokemonNames = common.GetCommonPokemonNames

                    'Open the message bin file
                    Dim charchoiceFile As New MessageBin
                    Await charchoiceFile.OpenFile(IO.Path.Combine(Me.GetRootDirectory, "Languages", charchoiceLanguageTemplate.Key, "inc_charchoice.bin"), CurrentPluginManager.CurrentIOProvider)

                    For Each charchoice In charchoiceData
                        Dim pokemonID As Integer
                        Select Case charchoice.Key
                            Case "FUSHIGIDANE"
                                pokemonID = starter1
                            Case "HITOKAGE"
                                pokemonID = starter5
                            Case "ZENIGAME"
                                pokemonID = starter10
                            Case "PIKACHUU"
                                pokemonID = starter30
                            Case "CHIKORIITA"
                                pokemonID = starter197
                            Case "HINOARASHI"
                                pokemonID = starter200
                            Case "WANINOKO"
                                pokemonID = starter203
                            Case "KIMORI"
                                pokemonID = starter322
                            Case "ACHAMO"
                                pokemonID = starter325
                            Case "MIZUGOROU"
                                pokemonID = starter329
                            Case "NAETORU"
                                pokemonID = starter479
                            Case "HIKOZARU"
                                pokemonID = starter482
                            Case "POTCHAMA"
                                pokemonID = starter485
                            Case "RIORU"
                                pokemonID = starter537
                            Case "TSUTAAJA"
                                pokemonID = starter592
                            Case "POKABU"
                                pokemonID = starter595
                            Case "MIJUMARU"
                                pokemonID = starter598
                            Case "HARIMARON"
                                pokemonID = starter766
                            Case "FOKKO"
                                pokemonID = starter769
                            Case "KEROMATSU"
                                pokemonID = starter772
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

            'Patch script ID checks
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

            'Continue the build (script compilation, mod building, etc.)
            Await MyBase.Build()
        End Function

    End Class
End Namespace

