Imports SkyEditor.Core.IO
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD.Dungeon
Namespace Windows
    <Obsolete> Public Class PsmdDir
        Implements IOpenableFile

        Public Property PokemonInfo As PokemonDataInfo
        Public Property PokemonExpTable As Experience
        Public Property LanguageFiles As Dictionary(Of String, FarcF5)
        Public Property DungeonFixedPokemon As FixedPokemon
        Public Property WazaData As WazaDataInfo
        Public Property ActData As ActDataInfo
        Public Property ActXlData As ActXlWaza
        Public Property ActHitCountData As ActHitCountTableDataInfo
        Public Property ItemData As ItemDataInfo
        Public Property PokemonNames As List(Of String)
        Protected Property CommonLanguages As Dictionary(Of String, MessageBin)

        Public Function GetEnglishCommon() As MessageBin
            If CommonLanguages.ContainsKey("message_us.bin") Then
                Return CommonLanguages("message_us.bin")
            ElseIf CommonLanguages.ContainsKey("message_en.bin") Then
                Return CommonLanguages("message_en.bin")
            Else
                Return Nothing
            End If
        End Function

        Public Sub New()
            MyBase.New
        End Sub

        Public Async Function OpenFile(RootDirectory As String, Provider As IIOProvider) As Task Implements IOpenableFile.OpenFile
            PokemonInfo = New PokemonDataInfo
            PokemonInfo.EnableInMemoryLoad = True
            Await PokemonInfo.OpenFile(IO.Path.Combine(RootDirectory, "romfs", "pokemon", "pokemon_data_info.bin"), Provider)

            PokemonExpTable = New Experience
            Await PokemonExpTable.OpenFile(IO.Path.Combine(RootDirectory, "romfs", "pokemon", "experience.bin"), Provider)

            LanguageFiles = New Dictionary(Of String, FarcF5)
            CommonLanguages = New Dictionary(Of String, MessageBin)
            For Each item In {"message_en.bin", "message_fr.bin", "message_ge.bin", "message_it.bin", "message_sp.bin", "message_us.bin", "message.bin"}
                Dim filename = IO.Path.Combine(RootDirectory, "romfs", item)
                If IO.File.Exists(filename) Then
                    Dim f As New FarcF5
                    f.EnableInMemoryLoad = True
                    Await f.OpenFile(filename, Provider)
                    LanguageFiles.Add(item, f)
                    Dim m As New MessageBin
                    m.EnableInMemoryLoad = True
                    m.CreateFile(item, f.GetFileData("common"))
                    CommonLanguages.Add(item, m)
                End If
            Next

            DungeonFixedPokemon = New FixedPokemon
            DungeonFixedPokemon.EnableInMemoryLoad = True
            Await DungeonFixedPokemon.OpenFile(IO.Path.Combine(RootDirectory, "romfs", "dungeon", "fixed_pokemon.bin"), Provider)

            Dim en = GetEnglishCommon()
            PokemonNames = en.GetCommonPokemonNames.Values.ToList

            WazaData = New WazaDataInfo
            Await WazaData.OpenFile(IO.Path.Combine(RootDirectory, "romfs", "pokemon", "waza_data_info.bin"), Provider)

            ActData = New ActDataInfo
            Await ActData.OpenFile(IO.Path.Combine(RootDirectory, "romfs", "dungeon", "act_data_info.bin"), Provider)

            ActXlData = New ActXlWaza
            Await ActXlData.OpenFile(IO.Path.Combine(RootDirectory, "romfs", "dungeon", "act_xlwaza.bin"), Provider)

            ActHitCountData = New ActHitCountTableDataInfo
            Await ActHitCountData.OpenFile(IO.Path.Combine(RootDirectory, "romfs", "dungeon", "act_hit_count_table_data_info.bin"), Provider)

            ItemData = New ItemDataInfo
            Await ItemData.OpenFile(IO.Path.Combine(RootDirectory, "romfs", "item_data_info.bin"), Provider)
        End Function
    End Class

End Namespace
