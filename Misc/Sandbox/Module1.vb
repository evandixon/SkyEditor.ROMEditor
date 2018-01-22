Imports SkyEditor.Core.IO
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD

Module Module1

    Sub Main()
        MainAsync.Wait()
    End Sub

    Async Function MainAsync() As Task
        'Test pokemon_graphic.bin modification
        Dim provider = New PhysicalIOProvider
        Dim pokemonGraphic As New Farc
        Await pokemonGraphic.OpenFile("C:\Users\evanl\Desktop\BaseRom\Raw Files\RomFS\pokemon_graphic.bin", provider)

        Dim zoroaBgrs As New BGRS
        Await zoroaBgrs.OpenFile("/zoroa_00.bgrs", pokemonGraphic)

        Dim sleepAnimation = zoroaBgrs.Animations.First(Function(a) a.AnimationName = "bd_sleep")
        Dim sleepLoopAnimation = zoroaBgrs.Animations.First(Function(a) a.AnimationName = "bd_sleep")

        Dim newSleepAnimation = sleepAnimation.Clone
        Dim newSleepLoopAnimation = sleepLoopAnimation.Clone

        newSleepAnimation.Name = sleepAnimation.Name.Replace("bd_sleep", "bd_ev001_sleep00")
        newSleepAnimation.AnimationType = BGRS.AnimationType.RemoteSkeletalAnimation
        newSleepLoopAnimation.Name = sleepLoopAnimation.Name.Replace("bd_sleeploop", "bd_ev001_sleep01")
        newSleepLoopAnimation.AnimationType = BGRS.AnimationType.RemoteSkeletalAnimation

        pokemonGraphic.CopyFile("/" & sleepAnimation.Name & ".bchskla", "/" & newSleepAnimation.Name & ".bchskla")
        pokemonGraphic.CopyFile("/" & sleepLoopAnimation.Name & ".bchskla", "/" & newSleepLoopAnimation.Name & ".bchskla")

        zoroaBgrs.Animations.Add(newSleepAnimation)
        zoroaBgrs.Animations.Add(newSleepLoopAnimation)

        Await zoroaBgrs.Save(pokemonGraphic)

        ''Just for testing. Results should be obvious.
        'Dim zoroa2Bgrs As New BGRS
        'Await zoroa2Bgrs.OpenFile("/zoroa_00.bgrs", pokemonGraphic)
        'zoroa2Bgrs.Animations.First(Function(a) a.Name = "4leg_beast_00__bd_walk").Name = "2leg_rioru_00__bd_walk"
        'Await zoroa2Bgrs.Save(pokemonGraphic)

        Console.WriteLine("Saving farc")
        Await pokemonGraphic.Save("C:\Users\evanl\Desktop\updated_pokemon_graphic.bin", provider)
    End Function

End Module
