Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization

Module Module1

    Sub Main()
        'Dim path = "C:\Tests\EoS-EU-ChallengeModeDev\LevelUp\Pokemon\pokemon_data\0289_Poochyena.xml"
        'Dim path2 = "C:\Tests\EoS-EU-ChallengeModeDev\LevelUp\Pokemon\pokemon_data\0489_Riolu_edit_saved.xml"
        'Dim deserializer As New XmlSerializer(GetType(SkyEditor.ROMEditor.MysteryDungeon.Explorers.PPMDU.Pokemon))
        'Dim reader As New XmlTextReader(File.OpenRead(path))
        'Dim pokemon As SkyEditor.ROMEditor.MysteryDungeon.Explorers.PPMDU.Pokemon = deserializer.Deserialize(reader)

        'Dim csv As New StringBuilder
        'Dim level As Integer = 1
        'For Each item In pokemon.StatsGrowth
        '    csv.AppendLine($"{level},{item.RequiredExp}")
        '    level += 1
        'Next

        'File.WriteAllText(IO.Path.ChangeExtension(path, "csv"), csv.ToString)

        Dim dungeon As New SkyEditor.ROMEditor.MysteryDungeon.Explorers.mappa
        Dim dungeonCurve As New StringBuilder
        dungeon.OpenFile("C:\Tests\EoS-EU-ChallengeModeDev\BaseRom\Raw Files\data\BALANCE\mappa_s.bin", New SkyEditor.Core.Windows.Providers.WindowsIOProvider)
        Dim maxPokemonCount As Integer = 0

        For count = 1 To 28
            For Each floor In dungeon.Dungeons(count).Floors
                maxPokemonCount = Math.Max(floor.PokemonSpawns.Count, maxPokemonCount)
            Next
        Next

        Console.WriteLine("Max Pokemon in floor: " & maxPokemonCount)

        Dim floorCount As Integer = 0
        For dungeonCount = 1 To 26
            For Each floor In dungeon.Dungeons(dungeonCount).Floors
                floorCount += 1
                dungeonCurve.Append(dungeonCount)
                dungeonCurve.Append(",")
                dungeonCurve.Append(floorCount)

                Dim skipped As Integer = 0
                For i = 0 To maxPokemonCount - 1

                    If floor.PokemonSpawns.Count > i Then
                        If Not {553, 383}.Contains(floor.PokemonSpawns(i).PokemonID) Then
                            dungeonCurve.Append(",")
                            dungeonCurve.Append(floor.PokemonSpawns(i).LevelX2 / 2)
                            dungeonCurve.Append("")
                        Else
                            skipped += 1
                        End If
                    Else
                        dungeonCurve.Append(",")
                    End If
                Next
                For count = 1 To skipped
                    dungeonCurve.Append(",")
                Next

                'Average spawn levels
                'Dim avgEndChar = ChrW(Text.Encoding.Unicode.GetBytes(avgLetter)(0) + maxPokemonCount)
                dungeonCurve.Append($"""=AVERAGE(C{floorCount}:{ExcelColumnFromNumber(floor.PokemonSpawns.Count)}{floorCount})""")

                dungeonCurve.AppendLine()

            Next
        Next

        File.WriteAllText("DungeonCurve.csv", dungeonCurve.ToString)


        'deserializer.Serialize(File.OpenWrite(path2), pokemon)
    End Sub

    Public Function ExcelColumnFromNumber(column As Integer) As String
        Dim columnString As String = ""
        Dim columnNumber As Decimal = column
        While columnNumber > 0
            Dim currentLetterNumber As Integer = (columnNumber - 1) Mod 26
            Dim currentLetter As Char = ChrW(currentLetterNumber + 65)
            columnString = currentLetter & columnString
            columnNumber = (columnNumber - (currentLetterNumber + 1)) / 26
        End While
        Return columnString
    End Function

End Module
