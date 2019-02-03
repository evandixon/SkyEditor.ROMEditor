Imports System.Collections.Concurrent
Imports System.Drawing
Imports System.IO
Imports PPMDU
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Utilities
Imports SkyEditor.ROMEditor.MysteryDungeon.Rescue
Imports SkyEditor.ROMEditor.Utilities
Imports SkyEditor.SaveEditor.MysteryDungeon

Namespace MysteryDungeon.Explorers
    Public Class Kaomado
        Implements IOpenableFile
        Implements ISavableAs
        Implements IOnDisk
        Implements IDisposable

        Public Event FileSaved As EventHandler Implements ISavable.FileSaved

        Public Sub New()
            Portraits = New List(Of Bitmap())(1154)
        End Sub

        Public Property Portraits As List(Of Bitmap())

        Public Property Filename As String Implements IOnDisk.Filename

        Public Sub CreateNew()
            Portraits.Clear()
            For count = 0 To 1154 - 1
                Portraits.Add(Enumerable.Repeat(Of Bitmap)(Nothing, 40).ToArray)
            Next
        End Sub

        Public Async Function Initialize(data As Byte()) As Task
            Portraits = New List(Of Bitmap())

            '====================
            'Part 1: Reading the table of contents
            '====================

            'This is the first pointer in the first non-null entry.
            'It will be used to determine the length of the ToC
            Dim firstPointer As Integer = BitConverter.ToInt32(data, &HA0)

            'Read the TOC
            Dim toc As New List(Of List(Of Integer))
            For blockIndex = &HA0 To firstPointer - 1 Step &HA0
                Dim block As New List(Of Integer)
                For pointerIndex = 0 To &HA0 Step 4
                    block.Add(BitConverter.ToInt32(data, blockIndex + pointerIndex))
                Next
                toc.Add(block)
            Next

            '====================
            'Part 2: Allocate space in Portraits
            '====================
            'This makes it so that each pokemon/portrait can be processed asynchronously with no threading issues
            For count = 0 To toc.Count - 1
                Dim array(39) As Bitmap
                Portraits.Add(array)
            Next

            '====================
            'Part 3: Start converting data into bitmaps
            '====================
            'This might take a while, so each portrait will be processed asynchronously
            Using manager As New UtilityManager
                Dim f As New AsyncFor
                f.BatchSize = Environment.ProcessorCount * 2
                Await f.RunFor(Async Function(pokemon As Integer) As Task
                                   For portrait = 0 To 39
                                       Await ProcessBitmap(data, toc, pokemon, portrait, manager)
                                   Next
                               End Function, 0, toc.Count - 1)
            End Using

        End Function

        Private Async Function ProcessBitmap(data As Byte(), toc As List(Of List(Of Integer)), pokemonID As Integer, portrait As Integer, manager As UtilityManager) As Task
            '====================
            'Part 4: Select the correct data
            '====================

            'Read the pointers
            Dim firstPointer = toc(pokemonID)(portrait)
            If firstPointer < 0 Then
                'This is a null pointer; nothing to convert
                Exit Function
            End If

            Dim secondPointer As Integer
            If portrait < toc(pokemonID).Count Then
                secondPointer = toc(pokemonID)(portrait + 1)
            Else
                secondPointer = toc(pokemonID + 1)(portrait - toc(pokemonID).Count)
            End If

            If secondPointer < 0 Then
                'The next pointer is a null pointer, but still marks the end of the current portrait
                secondPointer *= -1
            End If

            Dim length As Integer
            length = secondPointer - firstPointer

            'Read the palette
            Dim palette As New List(Of Color)
            For count = 0 To 47 Step 3
                Dim index = firstPointer + count
                palette.Add(Color.FromArgb(data(index + 0), data(index + 1), data(index + 2)))
            Next

            'Read the compressed data
            Dim compressedData = data.Skip(firstPointer + 48).Take(length - 48).ToArray

            '====================
            'Part 5: Decompress the tile data
            '====================
            Dim decompressedData As Byte()
            'Create a temporary file
            Dim tempCompressed = Path.GetTempFileName
            File.WriteAllBytes(tempCompressed, compressedData)

            'Decompress the file
            Dim tempDecompressed = Path.GetTempFileName
            Await manager.RunUnPX(tempCompressed, tempDecompressed)

            'Read the decompressed file
            decompressedData = File.ReadAllBytes(tempDecompressed)

            'Cleanup
            File.Delete(tempCompressed)
            File.Delete(tempDecompressed)

            '====================
            'Part 6: Generate the bitmap and put it in Portraits
            '====================
            If decompressedData.Length > 0 Then
                Portraits(pokemonID)(portrait) = GraphicsHelpers.BuildPokemonPortraitBitmap(palette, decompressedData)
            End If
        End Function

        Public Async Function OpenFile(Filename As String, Provider As IIOProvider) As Task Implements IOpenableFile.OpenFile
            Await Initialize(Provider.ReadAllBytes(Filename))
            Me.Filename = Filename
        End Function

        Public Async Function GetBytes() As Task(Of Byte())
            Dim palettes As New List(Of List(Of List(Of Color))) ' Top level: Pokemon; Second level: Portraits; Third level: Colors in the pallete
            Dim compressedPortraits As New List(Of List(Of Byte())) 'Top level: Pokemon; Second level: Portraits
            'Allocate space
            palettes.AddRange(Enumerable.Repeat(Of List(Of List(Of Color)))(Nothing, Portraits.Count))
            compressedPortraits.AddRange(Enumerable.Repeat(Of List(Of Byte()))(Nothing, Portraits.Count))

            'Fill the allocated space
            Using manager As New UtilityManager
                Dim f As New AsyncFor
                f.BatchSize = Environment.ProcessorCount * 2
                Await f.RunFor(Async Function(pokemonIndex) As Task
                                   Dim pokemon = Portraits(pokemonIndex)
                                   Dim pokemonPalettes As New List(Of List(Of Color))
                                   Dim pokemonPortraitsCompressed As New List(Of Byte())
                                   For Each portrait In pokemon
                                       If portrait IsNot Nothing Then
                                           Dim uncompressed As Byte()
                                           Dim palette As List(Of Color)

                                           palette = GraphicsHelpers.GetKaoPalette(portrait)
                                           pokemonPalettes.Add(palette)

                                           uncompressed = GraphicsHelpers.Get4bppPortraitData(portrait, palette)

                                           Dim compressed = Await manager.RunDoPX(uncompressed, PXFormat.AT4PX)
                                           pokemonPortraitsCompressed.Add(compressed)
                                       Else
                                           'If a portrait is missing, add null to each of the lists so the indexes are maintained later on
                                           pokemonPalettes.Add(Nothing)
                                           pokemonPortraitsCompressed.Add(Nothing)
                                       End If
                                   Next
                                   palettes(pokemonIndex) = pokemonPalettes
                                   compressedPortraits(pokemonIndex) = pokemonPortraitsCompressed
                               End Function, 0, Portraits.Count - 1)
            End Using

            'Generate the data to write to the file
            Dim nextEntryStart = (compressedPortraits.Count + 1) * 160
            Dim tocSection As New List(Of Byte)(nextEntryStart)
            Dim dataSection As New List(Of Byte)

            tocSection.AddRange(Enumerable.Repeat(CByte(0), 160)) 'The null entry

            For pokemon = 0 To compressedPortraits.Count - 1
                For portrait = 0 To compressedPortraits(pokemon).Count - 1
                    If compressedPortraits(pokemon)(portrait) Is Nothing Then
                        'Write a null toc entry
                        tocSection.AddRange(BitConverter.GetBytes(nextEntryStart * -1))
                    Else
                        'Write the toc entry
                        tocSection.AddRange(BitConverter.GetBytes(nextEntryStart))

                        'Write the data
                        For Each item In palettes(pokemon)(portrait)
                            dataSection.Add(item.R)
                            dataSection.Add(item.G)
                            dataSection.Add(item.B)
                            nextEntryStart += 3
                        Next

                        dataSection.AddRange(compressedPortraits(pokemon)(portrait))
                        nextEntryStart += compressedPortraits(pokemon)(portrait).Length
                    End If
                Next
            Next

            Return tocSection.Concat(dataSection).ToArray
        End Function

        Public Async Function Save(Filename As String, provider As IIOProvider) As Task Implements ISavableAs.Save
            provider.WriteAllBytes(Filename, Await GetBytes())
            RaiseEvent FileSaved(Me, New EventArgs)
        End Function

        Public Async Function Save(provider As IIOProvider) As Task Implements ISavable.Save
            Await Save(Filename, provider)
        End Function

        Public Function GetDefaultExtension() As String Implements ISavableAs.GetDefaultExtension
            Return ".kao"
        End Function

        Public Function GetSupportedExtensions() As IEnumerable(Of String) Implements ISavableAs.GetSupportedExtensions
            Return {".kao"}
        End Function

        ''' <summary>
        ''' Copies the appropriate portraits to a Rescue Team monster.sbin file.
        ''' </summary>
        ''' <param name="monsterBin">The file to which to copy portraits.</param>
        ''' <param name="copyAllEmotions">Whether or not to substitute the default portrait for missing emotions.</param>
        Public Async Function CopyToRescueTeam(monsterBin As SBin, Optional copyAllEmotions As Boolean = False) As Task
            Dim data As New ConcurrentDictionary(Of String, Byte())(monsterBin.Files)
            Dim a As New AsyncFor
            a.BatchSize = Environment.ProcessorCount * 2
            Await a.RunFor(Async Function(count As Integer) As Task
                               Dim eosPokemonID = count + 1
                               Dim rescuePokemonID = IDConversion.ConvertEoSPokemonToRB(eosPokemonID, False)
                               If rescuePokemonID > -1 Then
                                   Using kao As New KaoFile()
                                       'Initialize
                                       Dim monsterBinFilename = "kao" & rescuePokemonID.ToString.PadLeft(3, "0"c)
                                       If data.ContainsKey(monsterBinFilename) Then
                                           Await kao.Initialize(data(monsterBinFilename))
                                       Else
                                           kao.CreateFile()
                                       End If

                                       While kao.Portraits.Count <= 13
                                           kao.Portraits.Add(Nothing)
                                       End While

                                       'Copy Portraits
                                       For eosPortrait = 0 To Portraits(count).Length - 1
                                           Try
                                               If Not eosPortrait = 18 AndAlso 'Rescue Team uses another emotion instead of Determined
                                                                      eosPortrait Mod 2 = 0 AndAlso 'Rescue Team does not have asymmetric portraits
                                                                      eosPortrait <= 24 Then 'Rescue Team's max portrait index is 12

                                                   Dim rescuePortrait = (eosPortrait / 2)
                                                   If Portraits(count)(eosPortrait) IsNot Nothing Then
                                                       'Copy portrait if there isn't already one

                                                       If kao.Portraits(rescuePortrait) Is Nothing Then
                                                           kao.Portraits(rescuePortrait) = Portraits(count)(eosPortrait).Clone
                                                       End If
                                                   End If

                                                   If eosPortrait = 2 AndAlso Portraits(count)(2) IsNot Nothing AndAlso kao.Portraits(9) Is Nothing Then
                                                       'Additionally copy to index 9, since 9 is a closed-mouth variant of Grin 
                                                       kao.Portraits(9) = Portraits(count)(2).Clone
                                                   End If
                                               End If
                                           Catch ex As Exception
                                               Throw
                                           End Try


                                       Next

                                       'Copy default to other emotions if applicable
                                       If copyAllEmotions AndAlso kao.Portraits(0) IsNot Nothing Then
                                           For rescuePortrait = 1 To kao.Portraits.Count - 1
                                               If kao.Portraits(rescuePortrait) Is Nothing Then
                                                   kao.Portraits(rescuePortrait) = kao.Portraits(0).Clone
                                               End If
                                           Next
                                       End If

                                       'Save
                                       Try
                                           Dim rawData = Await kao.GetRawData
                                           data(monsterBinFilename) = rawData
                                       Catch ex As Exception
                                           Throw
                                       End Try

                                   End Using
                               End If
                           End Function, 0, Portraits.Count)
            For Each item In data
                If monsterBin.Files.ContainsKey(item.Key) Then
                    monsterBin.Files(item.Key) = item.Value
                Else
                    monsterBin.Files.Add(item.Key, item.Value)
                End If
            Next
        End Function

        Public Async Function Extract(outputDirectory As String, provider As IIOProvider) As Task
            If (Not provider.DirectoryExists(outputDirectory)) Then
                provider.CreateDirectory(outputDirectory)
            End If

            Await AsyncFor.For(0, Portraits.Count - 1,
                Async Function(pokemonIndex As Integer) As Task
                    Dim pokemonId = pokemonIndex + 1
                    Dim pokemonName As String = Nothing
                    If PokemonNames.Length > pokemonId Then
                        pokemonName = PokemonNames(pokemonId)
                    End If

                    Dim pokemonDirectory As String
                    If Not String.IsNullOrEmpty(pokemonName) Then
                        pokemonDirectory = Path.Combine(outputDirectory, $"{pokemonId.ToString().PadLeft(4, "0")}_{pokemonName}")
                    Else
                        pokemonDirectory = Path.Combine(outputDirectory, $"{pokemonId.ToString().PadLeft(4, "0")}")
                    End If

                    If (Not provider.DirectoryExists(pokemonDirectory)) Then
                        provider.CreateDirectory(pokemonDirectory)
                    End If

                    If Not Portraits(pokemonIndex).Any(Function(p) p IsNot Nothing) Then
                        Exit Function
                    End If

                    Await AsyncFor.For(0, Portraits(pokemonIndex).Length - 1,
                        Sub(portraitIndex As Integer)
                            Dim portraitName = PortraitNames(portraitIndex)
                            Dim outputFile As String
                            If Not String.IsNullOrEmpty(portraitName) Then
                                outputFile = Path.Combine(pokemonDirectory, $"{portraitIndex.ToString().PadLeft(4, "0")}_{portraitName}.png")
                            Else
                                outputFile = Path.Combine(pokemonDirectory, $"{portraitIndex.ToString().PadLeft(4, "0")}.png")
                            End If

                            Dim bitmap = Portraits(pokemonIndex)(portraitIndex)
                            If bitmap Is Nothing Then
                                Exit Sub
                            End If

                            Using outputStream = provider.OpenFileWriteOnly(outputFile)
                                bitmap.Save(outputStream, Imaging.ImageFormat.Png)
                            End Using
                        End Sub)
                End Function)
        End Function

        Public Async Function Import(inputDirectory As String, provider As IIOProvider) As Task
            Dim pokemonDirectories = Directory.GetDirectories(inputDirectory)
            Dim maxPokemonId As Integer = pokemonDirectories.
                Select(Function(d)
                           Dim id As Integer
                           Dim valid = Integer.TryParse(Path.GetFileName(d).Split("_")(0), id)
                           Return New With {id, valid}
                       End Function).
                Where(Function(o) o.valid).
                Select(Function(o) o.id).
                Max()

            While maxPokemonId > Portraits.Count
                Portraits.Add(Array.CreateInstance(GetType(Bitmap), 40))
            End While

            Await AsyncFor.ForEach(pokemonDirectories,
                Async Function(pokemonDirectory As String) As Task
                    Dim pokemonId As Integer
                    If Not Integer.TryParse(Path.GetFileName(pokemonDirectory).Split("_")(0), pokemonId) Then
                        Exit Function
                    End If

                    Dim pokemonIndex = pokemonId - 1
                    Await AsyncFor.ForEach(Directory.GetFiles(pokemonDirectory, "*.png"),
                        Sub(portraitFilename As String)
                            Dim portraitIndex As Integer
                            If Not Integer.TryParse(Path.GetFileName(portraitFilename).Split("_")(0), portraitIndex) Then
                                Exit Sub
                            End If

                            Using inputStream = provider.OpenFileReadOnly(portraitFilename)
                                Portraits(pokemonIndex)(portraitIndex) = Bitmap.FromStream(inputStream)
                            End Using
                        End Sub)
                End Function)
        End Function

#Region "Strings"
            'These strings have been copy/pasted from ppmd_kaoutil for backwards compatibility

            Private Shared PortraitNames As String() = {
            "STANDARD",
            "",
            "GRIN",
            "",
            "PAINED",
            "",
            "ANGRY",
            "",
            "WORRIED",
            "",
            "SAD",
            "",
            "CRYING",
            "",
            "SHOUTING",
            "",
            "TEARY_EYED",
            "",
            "DETERMINED",
            "",
            "JOYOUS",
            "",
            "INSPIRED",
            "",
            "SURPRISED",
            "",
            "DIZZY",
            "",
            "",
            "",
            "",
            "",
            "SIGH",
            "",
            "STUNNED",
            "",
            "",
            "",
            "",
            ""}

        Private Shared PokemonNames As String() = {"",
            "bulbasaur",
            "ivysaur",
            "venusaur",
            "charmander",
            "charmeleon",
            "charizard",
            "squirtle",
            "wartortle",
            "blastoise",
            "caterpie",
            "metapod",
            "butterfree",
            "weedle",
            "kakuna",
            "beedrill",
            "pidgey",
            "pidgeotto",
            "pidgeot",
            "rattata",
            "raticate",
            "spearow",
            "fearow",
            "ekans",
            "arbok",
            "pikachu",
            "raichu",
            "sandshrew",
            "sandslash",
            "nidoran-f",
            "nidorina",
            "nidoqueen",
            "nidoran-m",
            "nidorino",
            "nidoking",
            "clefairy",
            "clefable",
            "vulpix",
            "ninetales",
            "jigglypuff",
            "wigglytuff",
            "zubat",
            "golbat",
            "oddish",
            "gloom",
            "vileplume",
            "paras",
            "parasect",
            "venonat",
            "venomoth",
            "diglett",
            "dugtrio",
            "meowth",
            "persian",
            "psyduck",
            "golduck",
            "mankey",
            "primeape",
            "growlithe",
            "arcanine",
            "poliwag",
            "poliwhirl",
            "poliwrath",
            "abra",
            "kadabra",
            "alakazam",
            "machop",
            "machoke",
            "machamp",
            "bellsprout",
            "weepinbell",
            "victreebel",
            "tentacool",
            "tentacruel",
            "geodude",
            "graveler",
            "golem",
            "ponyta",
            "rapidash",
            "slowpoke",
            "slowbro",
            "magnemite",
            "magneton",
            "farfetchd",
            "doduo",
            "dodrio",
            "seel",
            "dewgong",
            "grimer",
            "muk",
            "shellder",
            "cloyster",
            "gastly",
            "haunter",
            "gengar",
            "onix",
            "drowzee",
            "hypno",
            "krabby",
            "kingler",
            "voltorb",
            "electrode",
            "exeggcute",
            "exeggutor",
            "cubone",
            "marowak",
            "hitmonlee",
            "hitmonchan",
            "lickitung",
            "koffing",
            "weezing",
            "rhyhorn",
            "rhydon",
            "chansey",
            "tangela",
            "kangaskhan",
            "horsea",
            "seadra",
            "goldeen",
            "seaking",
            "staryu",
            "starmie",
            "mr-mime",
            "scyther",
            "jynx",
            "electabuzz",
            "magmar",
            "pinsir",
            "tauros",
            "magikarp",
            "gyarados",
            "lapras",
            "ditto",
            "eevee",
            "vaporeon",
            "jolteon",
            "flareon",
            "porygon",
            "omanyte",
            "omastar",
            "kabuto",
            "kabutops",
            "aerodactyl",
            "snorlax",
            "articuno",
            "zapdos",
            "moltres",
            "dratini",
            "dragonair",
            "dragonite",
            "mewtwo",
            "mew",
            "chikorita",
            "bayleef",
            "meganium",
            "cyndaquil",
            "quilava",
            "typhlosion",
            "totodile",
            "croconaw",
            "feraligatr",
            "sentret",
            "furret",
            "hoothoot",
            "noctowl",
            "ledyba",
            "ledian",
            "spinarak",
            "ariados",
            "crobat",
            "chinchou",
            "lanturn",
            "pichu",
            "cleffa",
            "igglybuff",
            "togepi",
            "togetic",
            "natu",
            "xatu",
            "mareep",
            "flaaffy",
            "ampharos",
            "bellossom",
            "marill",
            "azumarill",
            "sudowoodo",
            "politoed",
            "hoppip",
            "skiploom",
            "jumpluff",
            "aipom",
            "sunkern",
            "sunflora",
            "yanma",
            "wooper",
            "quagsire",
            "espeon",
            "umbreon",
            "murkrow",
            "slowking",
            "misdreavus",
            "unown",
            "unown",
            "unown",
            "unown",
            "unown",
            "unown",
            "unown",
            "unown",
            "unown",
            "unown",
            "unown",
            "unown",
            "unown",
            "unown",
            "unown",
            "unown",
            "unown",
            "unown",
            "unown",
            "unown",
            "unown",
            "unown",
            "unown",
            "unown",
            "unown",
            "unown",
            "unown",
            "unown",
            "wobbuffet",
            "girafarig",
            "pineco",
            "forretress",
            "dunsparce",
            "gligar",
            "steelix",
            "snubbull",
            "granbull",
            "qwilfish",
            "scizor",
            "shuckle",
            "heracross",
            "sneasel",
            "teddiursa",
            "ursaring",
            "slugma",
            "magcargo",
            "swinub",
            "piloswine",
            "corsola",
            "remoraid",
            "octillery",
            "delibird",
            "mantine",
            "skarmory",
            "houndour",
            "houndoom",
            "kingdra",
            "phanpy",
            "donphan",
            "porygon2",
            "stantler",
            "smeargle",
            "tyrogue",
            "hitmontop",
            "smoochum",
            "elekid",
            "magby",
            "miltank",
            "blissey",
            "raikou",
            "entei",
            "suicune",
            "larvitar",
            "pupitar",
            "tyranitar",
            "lugia",
            "ho-oh",
            "celebi",
            "celebi-shiny",
            "treecko",
            "grovyle",
            "sceptile",
            "torchic",
            "combusken",
            "blaziken",
            "mudkip",
            "marshtomp",
            "swampert",
            "poochyena",
            "mightyena",
            "zigzagoon",
            "linoone",
            "wurmple",
            "silcoon",
            "beautifly",
            "cascoon",
            "dustox",
            "lotad",
            "lombre",
            "ludicolo",
            "seedot",
            "nuzleaf",
            "shiftry",
            "taillow",
            "swellow",
            "wingull",
            "pelipper",
            "ralts",
            "kirlia",
            "gardevoir",
            "surskit",
            "masquerain",
            "shroomish",
            "breloom",
            "slakoth",
            "vigoroth",
            "slaking",
            "nincada",
            "ninjask",
            "shedinja",
            "whismur",
            "loudred",
            "exploud",
            "makuhita",
            "hariyama",
            "azurill",
            "nosepass",
            "skitty",
            "delcatty",
            "sableye",
            "mawile",
            "aron",
            "lairon",
            "aggron",
            "meditite",
            "medicham",
            "electrike",
            "manectric",
            "plusle",
            "minun",
            "volbeat",
            "illumise",
            "roselia",
            "gulpin",
            "swalot",
            "carvanha",
            "sharpedo",
            "wailmer",
            "wailord",
            "numel",
            "camerupt",
            "torkoal",
            "spoink",
            "grumpig",
            "spinda",
            "trapinch",
            "vibrava",
            "flygon",
            "cacnea",
            "cacturne",
            "swablu",
            "altaria",
            "zangoose",
            "seviper",
            "lunatone",
            "solrock",
            "barboach",
            "whiscash",
            "corphish",
            "crawdaunt",
            "baltoy",
            "claydol",
            "lileep",
            "cradily",
            "anorith",
            "armaldo",
            "feebas",
            "milotic",
            "castform",
            "castform",
            "castform",
            "castform",
            "kecleon",
            "kecleon-purple",
            "shuppet",
            "banette",
            "duskull",
            "dusclops",
            "tropius",
            "chimecho",
            "absol",
            "wynaut",
            "snorunt",
            "glalie",
            "spheal",
            "sealeo",
            "walrein",
            "clamperl",
            "huntail",
            "gorebyss",
            "relicanth",
            "luvdisc",
            "bagon",
            "shelgon",
            "salamence",
            "beldum",
            "metang",
            "metagross",
            "regirock",
            "regice",
            "registeel",
            "latias",
            "latios",
            "kyogre",
            "groudon",
            "rayquaza",
            "jirachi",
            "deoxys",
            "deoxys",
            "deoxys",
            "deoxys",
            "turtwig",
            "grotle",
            "torterra",
            "chimchar",
            "monferno",
            "infernape",
            "piplup",
            "prinplup",
            "empoleon",
            "starly",
            "staravia",
            "staraptor",
            "bidoof",
            "bibarel",
            "kricketot",
            "kricketune",
            "shinx",
            "luxio",
            "luxray",
            "budew",
            "roserade",
            "cranidos",
            "rampardos",
            "shieldon",
            "bastiodon",
            "burmy",
            "burmy",
            "burmy",
            "wormadam",
            "wormadam",
            "wormadam",
            "mothim",
            "combee",
            "vespiquen",
            "pachirisu",
            "buizel",
            "floatzel",
            "cherubi",
            "cherrim",
            "cherrim",
            "shellos",
            "shellos",
            "gastrodon",
            "gastrodon",
            "ambipom",
            "drifloon",
            "drifblim",
            "buneary",
            "lopunny",
            "mismagius",
            "honchkrow",
            "glameow",
            "purugly",
            "chingling",
            "stunky",
            "skuntank",
            "bronzor",
            "bronzong",
            "bonsly",
            "mime-jr",
            "happiny",
            "chatot",
            "spiritomb",
            "gible",
            "gabite",
            "garchomp",
            "munchlax",
            "riolu",
            "lucario",
            "hippopotas",
            "hippowdon",
            "skorupi",
            "drapion",
            "croagunk",
            "toxicroak",
            "carnivine",
            "finneon",
            "lumineon",
            "mantyke",
            "snover",
            "abomasnow",
            "weavile",
            "magnezone",
            "lickilicky",
            "rhyperior",
            "tangrowth",
            "electivire",
            "magmortar",
            "togekiss",
            "yanmega",
            "leafeon",
            "glaceon",
            "gliscor",
            "mamoswine",
            "porygon-z",
            "gallade",
            "probopass",
            "dusknoir",
            "froslass",
            "rotom",
            "uxie",
            "mesprit",
            "azelf",
            "dialga",
            "palkia",
            "heatran",
            "regigigas",
            "giratina",
            "cresselia",
            "phione",
            "manaphy",
            "darkrai",
            "shaymin",
            "shaymin",
            "giratina",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "primal-dialga",
            "DUMMY-munchlax",
            "DUMMY-munchlax",
            "DUMMY-wigglytuff",
            "DUMMY-regigigas",
            "DUMMY-bronzong",
            "DUMMY-hitmonlee",
            "DUMMY-chimecheo",
            "DUMMY-wigglytuff",
            "DUMMY",
            "DUMMY",
            "DUMMY",
            "DUMMY",
            "DUMMY",
            "DUMMY",
            "DUMMY",
            "DUMMY",
            "DUMMY",
            "DUMMY",
            "DUMMY",
            "DUMMY",
            "DUMMY",
            "DUMMY",
            "wigglytuff",
            "DUMMY",
            "DUMMY",
            "DUMMY",
            "ditto-sentret",
            "ditto-bellossom",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "DUMMY-bulbasaur",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "rattata-female",
            "raticate-female",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "zubat-female",
            "golbat-female",
            "",
            "gloom-female",
            "vileplume-female",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "doduo-female",
            "dodrio-female",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "hypno-female",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "rhyhorn-female",
            "rhydon-female",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "magikarp-female",
            "gyarados-female",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "meganium-female",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "ledyba-female",
            "ledian-female",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "politoed-female",
            "",
            "",
            "",
            "aipom-female",
            "",
            "",
            "",
            "wooper-female",
            "",
            "",
            "",
            "murkrow-female",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "wobbuffet-female",
            "",
            "",
            "",
            "",
            "",
            "steelix-female",
            "",
            "",
            "",
            "",
            "",
            "heracross-female",
            "sneasel-female",
            "",
            "",
            "",
            "",
            "",
            "piloswine-female",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "donphan-female",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "combusken-female",
            "blaziken-female",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "beautifly-female",
            "",
            "",
            "",
            "",
            "ludicolo-female",
            "",
            "",
            "shiftry-female",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "meditite-female",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "swalot-female",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "relicanth-female",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "starly-female",
            "staravia-female",
            "staraptor-female",
            "",
            "bibarel-female",
            "kricketot-female",
            "kricketune-female",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "combee-female",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "ambipom-female",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "gible-female",
            "",
            "",
            "",
            "",
            "",
            "hippopotas-female",
            "hippowdon-female",
            "",
            "",
            "croagunk-female",
            "toxicroak-female",
            "",
            "finneon-female",
            "lumineon-female",
            "",
            "snover-female",
            "abomasnow-female",
            "",
            "",
            "",
            "rhyperior-female",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "mamoswine-female"}
#End Region

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).

                    If Portraits IsNot Nothing Then
                        For Each pokemon In Portraits
                            If pokemon IsNot Nothing Then
                                For Each portrait In pokemon
                                    If portrait IsNot Nothing Then
                                        portrait.Dispose()
                                    End If
                                Next
                            End If
                        Next
                    End If
                    Portraits = Nothing
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
