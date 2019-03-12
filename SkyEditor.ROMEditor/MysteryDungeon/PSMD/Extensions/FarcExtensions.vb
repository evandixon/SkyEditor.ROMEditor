Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports SkyEditor.Core.Utilities
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD

Namespace MysteryDungeon.PSMD.Extensions
    Public Module FarcExtensions
        ''' <summary>
        ''' Adds missing animations using comparable existing animations. Build progress is reported too.
        ''' </summary>
        ''' <param name="pokemonGraphic">The pokemon_graphic.bin to modify</param>
        ''' <param name="pkmDb">The pokemon_graphics_database.bin corresponding to <paramref name="pokemonGraphic"/></param>
        ''' <param name="progressReportToken">An optional token used to relay the progress of the operation</param>
        Private Async Function SubstituteMissingAnimations(pokemonGraphic As Farc, pkmDb As PGDB, substitutes As Dictionary(Of String, IEnumerable(Of String)), Optional progressReportToken As ProgressReportToken = Nothing) As Task
            If progressReportToken IsNot Nothing Then
                progressReportToken.Progress = 0
                progressReportToken.IsIndeterminate = False
            End If

            Dim entries = pkmDb.Entries.Select(Function(x) x.SecondaryBgrsName & ".bgrs").Distinct().Concat(pkmDb.Entries.Select(Function(y) y.PrimaryBgrsFilename)).ToList()

            Dim af As New AsyncFor

            If progressReportToken IsNot Nothing Then
                AddHandler af.ProgressChanged, Sub(sender As Object, e As ProgressReportedEventArgs)
                                                   progressReportToken.Progress = e.Progress
                                               End Sub
            End If

            Dim numComplete = 0

            Await af.RunForEach(entries,
                                Async Function(entry As String) As Task
                                    If Not String.IsNullOrEmpty(entry) Then
                                        If pokemonGraphic.FileExists("/" & entry) Then
                                            Dim currentBgrs As New BGRS
                                            Await currentBgrs.OpenFile("/" & entry, pokemonGraphic)

                                            'Dark matter causes a crash in a cutscene. Let's skip it.
                                            'Using a "contains" because there's 9 variants
                                            If currentBgrs.Filename.Contains("darkmatter") Then
                                                Exit Function
                                            End If

                                            For Each substituteData In substitutes
                                                Dim substitutePending = True
                                                For Each substitute In substituteData.Value
                                                    Dim newAnimation = currentBgrs.Animations.FirstOrDefault(Function(a) a.AnimationName = substituteData.Key)

                                                    If newAnimation IsNot Nothing Then
                                                        'Then we have an animation and there's no need to substitute
                                                        substitutePending = False
                                                        Continue For
                                                    End If

                                                    Dim oldAnimation = currentBgrs.Animations.FirstOrDefault(Function(a) a.AnimationName = substitute)
                                                    If oldAnimation Is Nothing Then
                                                        'One of our substitute candidates doesn't exist.
                                                        Continue For
                                                    End If

                                                    'We now have an animation to use and an empty slot to put it in
                                                    Dim copiedAnimation = oldAnimation.Clone
                                                    copiedAnimation.Name = oldAnimation.Name.Replace(substitute, substituteData.Key)
                                                    copiedAnimation.DevName = oldAnimation.DevName

                                                    If copiedAnimation.AnimationType And BGRS.AnimationType.SkeletalAnimation > 0 Then
                                                        Await pokemonGraphic.CopyFileAsync("/" & oldAnimation.Name & ".bcskla", "/" & copiedAnimation.Name & ".bcskla") 'GTI
                                                        Await pokemonGraphic.CopyFileAsync("/" & oldAnimation.Name & ".bchskla", "/" & copiedAnimation.Name & ".bchskla") 'PSMD
                                                    End If

                                                    If copiedAnimation.AnimationType And BGRS.AnimationType.MaterialAnimation > 0 Then
                                                        Await pokemonGraphic.CopyFileAsync("/" & oldAnimation.Name & ".bcmata", "/" & copiedAnimation.Name & ".bcmata") 'GTI
                                                        Await pokemonGraphic.CopyFileAsync("/" & oldAnimation.Name & ".bchmata", "/" & copiedAnimation.Name & ".bchmata") 'PSMD
                                                    End If

                                                    currentBgrs.Animations.Add(copiedAnimation)
                                                    substitutePending = False
                                                    Exit For
                                                Next

                                                If substitutePending Then
                                                    'Could not find an animation to substitute. Not necessarily a bad thing, but this shows we need more explicit substitute data
                                                    'Note to future devs: it is safe to ignore this breakpoint if you are not actively working on this feature
                                                    Debugger.Break()
                                                End If
                                            Next

                                            Await currentBgrs.Save(pokemonGraphic)
                                        End If
                                    End If
                                End Function)

            If progressReportToken IsNot Nothing Then
                progressReportToken.IsCompleted = True
            End If
        End Function

        ''' <summary>
        ''' Adds missing animations using comparable existing animations. Build progress is reported too.
        ''' </summary>
        ''' <param name="pokemonGraphic">The pokemon_graphic.bin to modify</param>
        ''' <param name="pkmDb">The pokemon_graphics_database.bin corresponding to <paramref name="pokemonGraphic"/></param>
        ''' <param name="progressReportToken">An optional token used to relay the progress of the operation</param>
        <Extension>
        Public Async Function SubstituteMissingAnimationsPsmd(pokemonGraphic As Farc, pkmDb As PGDB, Optional progressReportToken As ProgressReportToken = Nothing) As Task
            Dim substitutes As New Dictionary(Of String, IEnumerable(Of String))

            'Key: animation to be supplied
            'Value: animations that can be substituted. The ones that come first will be used first if they exist.
            'Comments are a description of what Fennekin looks like, and should be mostly true of other starters
            'Values are the best guess of animations that Zorua has that would work
            substitutes.Add("bd_ev000_cswait", {"bd_wait00"}) 'Variant of wait
            substitutes.Add("bd_ev001_down00", {"bd_sleep"}) 'Falling down (just after being defeated)
            substitutes.Add("bd_ev001_down01", {"bd_sleeploop"}) 'Lying down (defeated)
            substitutes.Add("bd_ev001_down02", {"bd_jump"}) 'Getting back up after being defeated
            substitutes.Add("bd_ev001_gasagasa", {"bd_walk"}) 'Digging
            substitutes.Add("bd_ev001_look01", {"bd_wait00"}) 'Looking down
            substitutes.Add("bd_ev001_sleep00", {"bd_sleep"}) 'Falling asleep
            substitutes.Add("bd_ev001_sleep01", {"bd_sleeploop"}) 'Asleep
            substitutes.Add("bd_ev001_sleep02", {"bd_jump"}) 'Waking up
            substitutes.Add("bd_ev003_relax00", {"bd_sleep"}) 'Starting to sit down
            substitutes.Add("bd_ev003_relax01", {"bd_sleeploop"}) 'Sitting, looking up
            substitutes.Add("bd_ev003_relax02", {"bd_jump"}) 'Getting up
            substitutes.Add("bd_ev003_relax03", {"bd_sleeploop"}) 'Sitting, slowly moving head in a "yes"
            substitutes.Add("bd_ev003_relax04", {"bd_sleeploop"}) 'Sitting, slowly moving head in a "no"
            substitutes.Add("bd_ev003_relax05", {"bd_sleeploop"}) 'Variant of bd_ev001_sleep05
            substitutes.Add("bd_ev003_relax06", {"bd_sleeploop"}) 'Variant of bd_ev001_sleep04
            substitutes.Add("bd_ev013_avoid00", {"bd_damage", "bd_ddamage", "bd_ddamege"}) 'Jump up
            substitutes.Add("bd_ev013_avoid01", {"bd_damage", "bd_ddamage", "bd_ddamege"}) 'Jump up & forward
            substitutes.Add("bd_ev013_avoid02", {"bd_surprise", "bd_damage", "bd_ddamage", "bd_ddamege"}) 'Jump up & back
            substitutes.Add("bd_ev013_avoid03", {"bd_damage", "bd_ddamage", "bd_ddamege"}) 'Jump back & lie down defeated
            substitutes.Add("bd_ev013_avoid04", {"bd_sleeploop"}) 'Lying down defeated
            substitutes.Add("bd_ev013_start", {"bd_wait00"}) 'Swaying forward. Prepping avoid00 or avoid01?
            substitutes.Add("bd_ev013_tired", {"bd_wait02"}) 'Out of breath

            'Harmony scarf evolution animations
            'I have no idea what to put here.
            'IIRC a T-pose is used here in the absense of animations, which is fine I guess?
            'substitutes.Add("bd_ev015_evolve_00", "")
            'substitutes.Add("bd_ev015_evolve_01", "")
            'substitutes.Add("bd_ev015_evolve_02", "")
            'substitutes.Add("bd_ev015_evolve_03", "")
            'substitutes.Add("bd_ev015_evolve_04", "")

            substitutes.Add("bd_ev015_wakeup00", {"bd_jump"}) 'Wake up from being defeated
            substitutes.Add("bd_ev018_attack", {"bd_attack"}) 'Standard attack?
            substitutes.Add("bd_ev018_attack00", {"bd_attack"}) 'Slower version of attack, without pullback afterward
            substitutes.Add("bd_ev018_cry00", {"bd_cry", "bd_endure"}) 'Lowering head to cry
            substitutes.Add("bd_ev018_cry01", {"bd_cry", "bd_endure"}) 'Crying
            substitutes.Add("bd_ev018_kneeattache00", {"bd_sleeploop"}) 'Lying down, defeated
            substitutes.Add("bd_ev018_lies00", {"bd_backwalk", "bd_damage", "bd_ddamage", "bd_ddamege"}) 'Knocked backward, still standing
            substitutes.Add("bd_ev018_lies00loop", {"bd_pain", "bd_endure"}) 'Panting?
            substitutes.Add("bd_ev018_lies01", {"bd_walk"}) 'Walking forward low to ground, probably lacking the energy to fully stand
            substitutes.Add("bd_ev021_depress00", {"bd_pain", "bd_endure"}) 'Transition to depress01
            substitutes.Add("bd_ev021_depress01", {"bd_pain", "bd_endure"}) 'Trying to stand, but unable?
            substitutes.Add("bd_ev024_jump00", {"bd_jump"}) 'Jumping
            substitutes.Add("bd_ev024_jump01", {"bd_jumploop", "bd_endure"}) 'Mid-air after jump
            substitutes.Add("bd_ev024_jump02", {"bd_landing"}) 'Landing
            substitutes.Add("bd_ev026_doya00", {"bd_wait00"}) 'Transition to bd_ev026_doya01
            substitutes.Add("bd_ev026_doya01", {"bd_wait00"}) '"I hate it when you make that face, Fennekin" -Espur
            substitutes.Add("bd_ev026_doya02", {"bd_wait00"}) 'Transition from bd_ev026_doya01 to normal
            substitutes.Add("bd_ev026_finish00", {"bd_jump"}) 'Some sort of jump?
            substitutes.Add("bd_ev026_finish01", {"bd_wait00"}) 'Variant of tired?
            substitutes.Add("bd_ev026_finish02", {"bd_wait00"}) 'Transition to normal

            'On the hill next to the big tree
            'When the parter... (starts crying at the thought)
            substitutes.Add("bd_ev026_relax00", {"bd_wait00"}) 'Sitting, then turning left to the player
            substitutes.Add("bd_ev026_relax01", {"bd_wait00"}) 'Looking left at the player
            substitutes.Add("bd_ev026_relax02", {"bd_wait00"}) 'Turning back ahead

            Await SubstituteMissingAnimations(pokemonGraphic, pkmDb, substitutes, progressReportToken)
        End Function

        ''' <summary>
        ''' Adds missing animations using comparable existing animations. Build progress is reported too.
        ''' </summary>
        ''' <param name="pokemonGraphic">The pokemon_graphic.bin to modify</param>
        ''' <param name="pkmDb">The pokemon_graphics_database.bin corresponding to <paramref name="pokemonGraphic"/></param>
        ''' <param name="progressReportToken">An optional token used to relay the progress of the operation</param>
        <Extension>
        Public Async Function SubstituteMissingAnimationsGti(pokemonGraphic As Farc, pkmDb As PGDB, Optional progressReportToken As ProgressReportToken = Nothing) As Task
            Dim substitutes As New Dictionary(Of String, IEnumerable(Of String))

            'Key: animation to be supplied
            'Value: animations that can be substituted. The ones that come first will be used first if they exist.
            'Comments are a description of what Tepig looks like, and should be mostly true of other starters
            'Values are the best guess of animations that Zorua has that would work
            substitutes.Add("bd_ev000_cswait", {"bd_wait02", "bd_wait00", "bd_wait"}) 'Variant of wait
            substitutes.Add("bd_ev000_cswalk", {"bd_ev011_tired05", "bd_walk", "bd_wait02", "bd_wait"}) 'Walking, but sneakier
            substitutes.Add("bd_ev000_surprise", {"bd_surprise", "bd_wait02", "bd_wait"}) 'Jump back, slower than bd_surprise
            substitutes.Add("bd_ev000_banzai", {"bd_letsgo", "bd_wait00", "bd_wait"}) 'Looking up (two-legged Pokemon throw hands up too). Perhaps the team cheer?
            substitutes.Add("bd_ev001_confirms", {"bd_wait00", "bd_wait"}) 'Always look both ways before crossing the street. bd_disagree may work, but it's too fast.
            substitutes.Add("bd_ev001_down00", {"bd_ev011_tired02", "bd_fall", "bd_sleep", "bd_wait02", "bd_wait"}) 'To down01
            substitutes.Add("bd_ev001_down01", {"bd_ev011_tired02_loop", "bd_sleeploop", "bd_dsleeploop", "bd_wait02", "bd_wait"}) 'Lying on ground defeated
            substitutes.Add("bd_ev001_down02", {"bd_ev011_tired04", "bd_dwait02", "bd_wait02", "bd_wait"}) 'From down01
            substitutes.Add("bd_ev001_gasagasa", {"bd_greet", "bd_wait02", "bd_wait"}) 'Digging, searching for something. bd_greet is probably a too different, but kinda similar between Tepig and Zorua
            substitutes.Add("bd_ev001_look00", {"bd_wait02", "bd_wait"}) 'To look01
            substitutes.Add("bd_ev000_look01", {"bd_wait02", "bd_wait"}) 'Looking off a cliff
            substitutes.Add("bd_ev001_look02", {"bd_wait02", "bd_wait"}) 'To look 03
            substitutes.Add("bd_ev001_look03", {"bd_landing", "bd_wait02", "bd_wait"}) 'Jumping off a cliff
            substitutes.Add("bd_ev001_shy", {"bd_wait02", "bd_wait"}) 'Tepig: lower and tilt head. Pikachu: tilt head and put paw to cheek. No clue how to substitute.
            substitutes.Add("bd_ev001_skydv00", {"bd_jumploop", "bd_wait02", "bd_wait"}) 'Probably falling into the Pokemon world for the first time.
            substitutes.Add("bd_ev001_dkydv01", {"bd_endure", "bd_wait02", "bd_wait"}) 'skydv00, except flailing
            substitutes.Add("bd_ev001_sleep00", {"bd_sleep", "bd_wait02", "bd_wait"}) 'Falling asleep
            substitutes.Add("bd_ev001_sleep01", {"bd_sleeploop", "bd_dsleeploop", "bd_wait02", "bd_wait"}) 'Sleeping
            substitutes.Add("bd_ev001_sleep02", {"bd_wait02", "bd_wait"}) 'Getting up
            substitutes.Add("bd_ev011_kneeattache00", {"bd_ev011_tired00", "bd_damage", "bd_ddamage", "bd_wait02", "bd_wait"}) 'Hit in leg
            substitutes.Add("bd_ev011_kneeattache01", {"bd_ev011_tired01", "bd_wait00", "bd_wait"}) 'Trying to stand, in pain
            substitutes.Add("bd_ev011_kneeattache02", {"bd_ev011_tired02", "bd_wait02", "bd_wait"}) 'Falling down
            substitutes.Add("bd_ev011_kneeattache03", {"bd_ev011_tired04", "bd_wait02", "bd_wait"}) 'Getting up
            substitutes.Add("bd_ev013_avoid00", {"bd_jump", "bd_ddamage", "bd_wait02", "bd_wait"}) 'Jump right to avoid
            substitutes.Add("bd_ev013_avoid01", {"bd_jump", "bd_ddamage", "bd_wait02", "bd_wait"}) 'Jump left to avoid (direction varies by Pokemon)
            substitutes.Add("bd_ev013_avoid02", {"bd_jump", "bd_ddamage", "bd_wait02", "bd_wait"}) 'Jump center to avoid (direction varies by Pokemon)
            substitutes.Add("bd_ev013_avoid03", {"bd_ddamage", "bd_wait02", "bd_wait"}) 'Backflip, too avoid04
            substitutes.Add("bd_ev013_avoid04", {"bd_ev011_tired04", "bd_sleeploop", "bd_dsleeploop", "bd_wait02", "bd_wait"}) 'Failed to avoid; on ground.
            substitutes.Add("bd_ev013_start", {"bd_push", "bd_walk", "bd_wait02", "bd_wait"}) 'Pushing something
            substitutes.Add("bd_ev013_tired", {"bd_ev011_tired01", "bd_wait02", "bd_wait"}) 'Panting
            substitutes.Add("bd_ev013_wakeup", {"bd_jump", "bd_wait02", "bd_wait"}) 'Jump up
            substitutes.Add("bd_ev014_wait", {"bd_jump", "bd_wait02", "bd_wait"}) 'Some kind of jump??
            substitutes.Add("bd_ev015_protect00", {"bd_wait02", "bd_wait"}) 'Standing in from of someone to protect
            substitutes.Add("bd_ev015_protect01", {"bd_ev011_tired00", "bd_sleep", "bd_wait02", "bd_wait"}) 'Falling strait down
            substitutes.Add("bd_ev015_protect02", {"bd_ev011_tired01", "bd_sleeploop", "bd_dsleeploop", "bd_wait02", "bd_wait"}) 'Being down
            substitutes.Add("bd_ev015_wakeup00", {"bd_ev011_tired04", "bd_wait02", "bd_wait"}) 'Getting up from being on side (unrelated to protect02)
            substitutes.Add("bd_ev018_cry00", {"bd_wait02", "bd_wait"}) 'To cry01
            substitutes.Add("bd_ev018_cry01", {"bd_cry", "bd_wait02", "bd_wait"}) 'Variant of wait
            substitutes.Add("bd_op000_jump00", {"bd_jump", "bd_wait02", "bd_wait"}) 'Initial jump
            substitutes.Add("bd_op000_jump00loop", {"bd_jumploop", "bd_wait02", "bd_wait"}) 'In air after jump
            substitutes.Add("bd_op000_jump01", {"bd_jumploop", "bd_wait02", "bd_wait"}) 'Starting to fall
            substitutes.Add("bd_op000_jump01loop", {"bd_jumploop", "bd_wait02", "bd_wait"}) 'Falling
            substitutes.Add("bd_op000_sleep00", {"bd_wait02", "bd_wait"}) 'Getting up and starting stretching
            substitutes.Add("bd_op000_sleep00loop", {"bd_wait02", "bd_wait"}) 'Holding the stretching
            substitutes.Add("bd_op000_sleep01", {"bd_wait02", "bd_wait"}) 'Getting up and rubbing eyes
            substitutes.Add("bd_op000_sleep01loop", {"bd_wait02", "bd_wait"}) 'Rubbing eyes

            Await SubstituteMissingAnimations(pokemonGraphic, pkmDb, substitutes, progressReportToken)
        End Function

        ''' <summary>
        ''' Adds missing portraits in GTI's face_graphic.bin
        ''' </summary>
        ''' <param name="faceGraphic"></param>
        ''' <param name="progressReportToken"></param>
        ''' <returns></returns>
        <Extension>
        Public Async Function SubstituteMissingPortraitsGti(faceGraphic As Farc, Optional progressReportToken As ProgressReportToken = Nothing) As Task
            Dim filenameRegex As New Regex("((([a-z0-9]|_)+)(_f)?(_hanten)?(_r)?)_([0-9]{2})", RegexOptions.Compiled)
            Dim files = faceGraphic.GetFiles("/", "*", True)

            Dim pokemonNames = files.Select(Function(x) filenameRegex.Match(x).Groups(1).Value).Distinct().ToList()

            Dim a As New AsyncFor
            AddHandler a.ProgressChanged, Sub(sender As Object, e As ProgressReportedEventArgs)
                                              If progressReportToken IsNot Nothing Then
                                                  progressReportToken.Progress = e.Progress
                                                  progressReportToken.Message = e.Message
                                                  progressReportToken.IsIndeterminate = e.IsIndeterminate
                                              End If
                                          End Sub
            a.RunSynchronously = True
            Await a.RunForEach(pokemonNames,
                               Async Function(pokemonName As String) As Task
                                   Dim sourceFile = $"/{pokemonName}_00.bin"
                                   For i = 1 To 21
                                       Dim destFile = $"/{pokemonName}_{i.ToString().PadLeft(2, "0")}.bin"
                                       If Not faceGraphic.FileExists(destFile) Then
                                           Await faceGraphic.CopyFileAsync(sourceFile, destFile)
                                       End If
                                   Next
                               End Function)

            If progressReportToken IsNot Nothing Then
                progressReportToken.IsCompleted = True
            End If
        End Function
    End Module
End Namespace