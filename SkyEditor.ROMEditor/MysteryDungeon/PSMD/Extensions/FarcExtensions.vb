Imports System.Runtime.CompilerServices
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
        <Extension>
        Public Async Function SubstituteMissingAnimations(pokemonGraphic As Farc, pkmDb As PGDB, Optional progressReportToken As ProgressReportToken = Nothing) As Task
            If progressReportToken IsNot Nothing Then
                progressReportToken.Progress = 0
                progressReportToken.IsIndeterminate = False
            End If

            Dim substitutes As New Dictionary(Of String, String)

            'Key: animation to be supplied
            'Value: animation to substitute it with
            'Comments are a description of what Fennekin looks like, and should be mostly true of other starters
            'Values are the best guess of animations that Zorua has that would work
            substitutes.Add("bd_ev000_cswait", "bd_wait") 'Variant of wait
            substitutes.Add("bd_ev001_down00", "bd_sleep") 'Falling down (just after being defeated)
            substitutes.Add("bd_ev001_down01", "bd_sleeploop") 'Lying down (defeated)
            substitutes.Add("bd_ev001_down02", "bd_jump") 'Getting back up after being defeated
            substitutes.Add("bd_ev001_gasagasa", "bd_walk") 'Digging
            substitutes.Add("bd_ev001_look01", "bd_wait") 'Looking down
            substitutes.Add("bd_ev001_sleep00", "bd_sleep") 'Falling asleep
            substitutes.Add("bd_ev001_sleep01", "bd_sleeploop") 'Asleep
            substitutes.Add("bd_ev001_sleep02", "bd_jump") 'Waking up
            substitutes.Add("bd_ev003_relax00", "bd_sleep") 'Starting to sit down
            substitutes.Add("bd_ev003_relax01", "bd_sleeploop") 'Sitting, looking up
            substitutes.Add("bd_ev003_relax02", "bd_jump") 'Getting up
            substitutes.Add("bd_ev003_relax03", "bd_sleeploop") 'Sitting, slowly moving head in a "yes"
            substitutes.Add("bd_ev003_relax04", "bd_sleeploop") 'Sitting, slowly moving head in a "no"
            substitutes.Add("bd_ev003_relax05", "bd_sleeploop") 'Variant of bd_ev001_sleep05
            substitutes.Add("bd_ev003_relax06", "bd_sleeploop") 'Variant of bd_ev001_sleep04
            substitutes.Add("bd_ev013_avoid00", "bd_damage") 'Jump up
            substitutes.Add("bd_ev013_avoid01", "bd_damage") 'Jump up & forward
            substitutes.Add("bd_ev013_avoid02", "bd_surprise") 'Jump up & back
            substitutes.Add("bd_ev013_avoid03", "bd_damage") 'Jump back & lie down defeated
            substitutes.Add("bd_ev013_avoid04", "bd_sleeploop") 'Lying down defeated
            substitutes.Add("bd_ev013_start", "bd_wait") 'Swaying forward. Prepping avoid00 or avoid01?
            substitutes.Add("bd_ev013_tired", "bd_wait02") 'Out of breath

            'Harmony scarf evolution animations
            'I have no idea what to put here.
            'IIRC a T-pose is used here in the absense of animations, which is fine I guess?
            'substitutes.Add("bd_ev015_evolve_00", "")
            'substitutes.Add("bd_ev015_evolve_01", "")
            'substitutes.Add("bd_ev015_evolve_02", "")
            'substitutes.Add("bd_ev015_evolve_03", "")
            'substitutes.Add("bd_ev015_evolve_04", "")

            substitutes.Add("bd_ev015_wakeup00", "bd_jump") 'Wake up from being defeated
            substitutes.Add("bd_ev018_attack", "bd_attack") 'Standard attack?
            substitutes.Add("bd_ev018_attack00", "bd_attack") 'Slower version of attack, without pullback afterward
            substitutes.Add("bd_ev018_cry00", "bd_cry") 'Lowering head to cry
            substitutes.Add("bd_ev018_cry01", "bd_cry") 'Crying
            substitutes.Add("bd_ev018_kneeattache00", "bd_sleeploop") 'Lying down, defeated
            substitutes.Add("bd_ev018_lies00", "bd_backwalk") 'Knocked backward, still standing
            substitutes.Add("bd_ev018_lies00loop", "bd_pain") 'Panting?
            substitutes.Add("bd_ev018_lies01", "bd_walk") 'Walking forward low to ground, probably lacking the energy to fully stand
            substitutes.Add("bd_ev021_depress00", "bd_pain") 'Transition to depress01
            substitutes.Add("bd_ev021_depress01", "bd_pain") 'Trying to stand, but unable?
            substitutes.Add("bd_ev024_jump00", "bd_jump") 'Jumping
            substitutes.Add("bd_ev024_jump01", "bd_jumploop") 'Mid-air after jump
            substitutes.Add("bd_ev024_jump02", "bd_landing") 'Landing
            substitutes.Add("bd_ev026_doya00", "bd_wait00") 'Transition to bd_ev026_doya01
            substitutes.Add("bd_ev026_doya01", "bd_wait00") '"I hate it when you make that face, Fennekin" -Espur
            substitutes.Add("bd_ev026_doya02", "bd_wait00") 'Transition from bd_ev026_doya01 to normal
            substitutes.Add("bd_ev026_finish00", "bd_jump") 'Some sort of jump?
            substitutes.Add("bd_ev026_finish01", "bd_wait") 'Variant of tired?
            substitutes.Add("bd_ev026_finish02", "bd_wait") 'Transition to normal

            'On the hill next to the big tree
            'When the parter... (starts crying at the thought)
            substitutes.Add("bd_ev026_relax00", "bd_jumploop") 'Sitting, then turning left to the player
            substitutes.Add("bd_ev026_relax01", "bd_jumploop") 'Looking left at the player
            substitutes.Add("bd_ev026_relax02", "bd_jumploop") 'Turning back ahead

            Dim entries = pkmDb.Entries.Select(Function(x) x.SecondaryBgrsName & ".bgrs").Distinct().Concat(pkmDb.Entries.Select(Function(y) y.PrimaryBgrsFilename)).ToList()

            Dim af As New AsyncFor

            If progressReportToken IsNot Nothing Then
                AddHandler af.ProgressChanged, Sub(sender As Object, e As ProgressReportedEventArgs)
                                                   progressReportToken.Progress = e.Progress
                                               End Sub
            End If

            Dim numComplete = 0

            entries.AsParallel.ForAll(Async Sub(entry As String)
                                          If Not String.IsNullOrEmpty(entry) Then
                                              If pokemonGraphic.FileExists("/" & entry) Then
                                                  Dim currentBgrs As New BGRS
                                                  Await currentBgrs.OpenFile("/" & entry, pokemonGraphic)

                                                  'Dark matter causes a crash in a cutscene. Let's skip it.
                                                  'Using a "contains" because there's 9 variants
                                                  If currentBgrs.Filename.Contains("darkmatter") Then
                                                      Exit Sub
                                                  End If

                                                  For Each substitute In substitutes
                                                      Dim oldAnimation = currentBgrs.Animations.FirstOrDefault(Function(a) a.AnimationName = substitute.Value)
                                                      Dim newAnimation = currentBgrs.Animations.FirstOrDefault(Function(a) a.AnimationName = substitute.Key)

                                                      If oldAnimation IsNot Nothing AndAlso newAnimation Is Nothing Then
                                                          Dim copiedAnimation = oldAnimation.Clone
                                                          copiedAnimation.Name = oldAnimation.Name.Replace(substitute.Value, substitute.Key)

                                                          If copiedAnimation.AnimationType And BGRS.AnimationType.SkeletalAnimation > 0 Then
                                                              pokemonGraphic.CopyFile("/" & oldAnimation.Name & ".bchskla", "/" & copiedAnimation.Name & ".bchskla")
                                                          End If

                                                          If copiedAnimation.AnimationType And BGRS.AnimationType.MaterialAnimation > 0 Then
                                                              pokemonGraphic.CopyFile("/" & oldAnimation.Name & ".bchmata", "/" & copiedAnimation.Name & ".bchmata")
                                                          End If

                                                          currentBgrs.Animations.Add(copiedAnimation)
                                                      End If
                                                  Next

                                                  Await currentBgrs.Save(pokemonGraphic)
                                              End If
                                          End If

                                          Threading.Interlocked.Increment(numComplete)
                                          progressReportToken.Progress = numComplete / entries.Count
                                      End Sub)

            If ProgressReportToken IsNot Nothing Then
                ProgressReportToken.IsCompleted = True
            End If
        End Function
    End Module
End Namespace