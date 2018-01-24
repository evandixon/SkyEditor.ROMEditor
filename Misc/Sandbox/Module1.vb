Imports SkyEditor.Core.IO
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD

Module Module1

    Sub Main()
        MainAsync.Wait()
    End Sub

    Async Function MainAsync() As Task
        'Test pokemon_graphic.bin modification
        Dim provider = New PhysicalIOProvider

        Dim pkmDb As New PGDB
        Await pkmDb.OpenFile("C:\Users\evanl\Desktop\BaseRom\Raw Files\RomFS\pokemon_graphics_database.bin", provider)

        Dim pokemonGraphic As New Farc
        Await pokemonGraphic.OpenFile("C:\Users\evanl\Desktop\BaseRom\Raw Files\RomFS\pokemon_graphic.bin", provider)

        For Each entry In pkmDb.Entries.Select(Function(x) x.SecondaryBgrsName & ".bgrs").Distinct().Concat(pkmDb.Entries.Select(Function(y) y.PrimaryBgrsFilename))
            If Not String.IsNullOrEmpty(entry) Then
                If pokemonGraphic.FileExists("/" & entry) Then
                    Try
                        Console.WriteLine("Processing " & entry)
                        Dim zoroaBgrs As New BGRS
                        Await zoroaBgrs.OpenFile("/" & entry, pokemonGraphic)

                        Dim sleepAnimation = zoroaBgrs.Animations.FirstOrDefault(Function(a) a.AnimationName = "bd_sleep")
                        Dim sleepLoopAnimation = zoroaBgrs.Animations.FirstOrDefault(Function(a) a.AnimationName = "bd_sleeploop")
                        Dim oldHrSleepAnimation = zoroaBgrs.Animations.FirstOrDefault(Function(a) a.AnimationName = "bd_ev001_sleep00")
                        Dim oldHrSleepLoopAnimation = zoroaBgrs.Animations.FirstOrDefault(Function(a) a.AnimationName = "bd_ev001_sleep01")

                        If sleepAnimation IsNot Nothing AndAlso sleepLoopAnimation IsNot Nothing AndAlso
                                oldHrSleepAnimation Is Nothing AndAlso oldHrSleepLoopAnimation Is Nothing Then

                            Dim newSleepAnimation = sleepAnimation.Clone
                            Dim newSleepLoopAnimation = sleepLoopAnimation.Clone

                            newSleepAnimation.Name = sleepAnimation.Name.Replace("bd_sleep", "bd_ev001_sleep00")
                            newSleepLoopAnimation.Name = sleepLoopAnimation.Name.Replace("bd_sleeploop", "bd_ev001_sleep01")

                            If newSleepAnimation.AnimationType And BGRS.AnimationType.SkeletalAnimation > 0 Then
                                pokemonGraphic.CopyFile("/" & sleepAnimation.Name & ".bchskla", "/" & newSleepAnimation.Name & ".bchskla")
                                pokemonGraphic.CopyFile("/" & sleepLoopAnimation.Name & ".bchskla", "/" & newSleepLoopAnimation.Name & ".bchskla")
                            End If

                            If newSleepAnimation.AnimationType And BGRS.AnimationType.MaterialAnimation > 0 Then
                                pokemonGraphic.CopyFile("/" & sleepAnimation.Name & ".bchmata", "/" & newSleepAnimation.Name & ".bchmata")
                                pokemonGraphic.CopyFile("/" & sleepLoopAnimation.Name & ".bchmata", "/" & newSleepLoopAnimation.Name & ".bchmata")
                            End If

                            zoroaBgrs.Animations.Add(newSleepAnimation)
                            zoroaBgrs.Animations.Add(newSleepLoopAnimation)

                            Await zoroaBgrs.Save(pokemonGraphic)
                            Console.WriteLine("Did the thing")
                        End If
                    Catch ex As Exception
                        Debugger.Break()
                    End Try
                End If
            End If
        Next




        ''Just for testing. Results should be obvious.
        'Dim zoroa2Bgrs As New BGRS
        'Await zoroa2Bgrs.OpenFile("/zoroa_00.bgrs", pokemonGraphic)
        'zoroa2Bgrs.Animations.First(Function(a) a.Name = "4leg_beast_00__bd_walk").Name = "2leg_rioru_00__bd_walk"
        'Await zoroa2Bgrs.Save(pokemonGraphic)

        Console.WriteLine("Saving farc")
        Await pokemonGraphic.Save("C:\Users\evanl\Desktop\updated3_pokemon_graphic.bin", provider)
    End Function

End Module
