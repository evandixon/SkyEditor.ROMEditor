Imports SkyEditor.Core.IO
Imports SkyEditor.ROMEditor.MysteryDungeon

Namespace MysteryDungeon.Explorers
    Public Class mappa
        Inherits Sir0

        'File map:
        'SIR0 header
        'Floor Index data
        'Floor Index pointers
        '(other)
        'PokemonSpawn Data
        'PokemonSpawn pointers
        'Content Header
        'SIR0 footer

        Public Enum FloorStructure As Byte
            ''' <summary>
            ''' Medium-Large (Biggest 6 x 4)
            ''' </summary>
            MediumLarge = 0

            ''' <summary>
            ''' Small (Biggest 2 x 3)
            ''' </summary>
            Small = 1

            ''' <summary>
            ''' One-Room Monster House
            ''' </summary>
            OneRoomMonsterHouse = 2

            ''' <summary>
            ''' Outer Square (Long hallway around edges of map with 8 rooms inside)
            ''' </summary>
            OuterSquare = 3

            ''' <summary>
            ''' CrossRoads (3 rooms at top and bottom and 2 rooms at left and right side with a string of hallways in the middle of the map connecting the rooms)
            ''' </summary>
            CrossRoads = 4

            ''' <summary>
            ''' Two-Room (One of them has a Monster House)
            ''' </summary>
            TwoRoom = 5

            ''' <summary>
            ''' Line (1 horizontal straight line of 5 rooms)
            ''' </summary>
            Line = 6

            ''' <summary>
            ''' Cross (5 Rooms in form of Cross; 3 x 3 with Top Left, Top Right, Bottom Left, and Bottom Right Room missing)
            ''' </summary>
            Cross = 7

            ''' <summary>
            ''' Small-Medium (Biggest 4 x 2)
            ''' </summary>
            SmallMedium4x2 = 8

            ''' <summary>
            ''' "Beetle" (1 Giant Room in middle of map with 3 vertical rooms to the left of it and to the right of it)
            ''' </summary>
            Beetle = 9

            ''' <summary>
            ''' Outer Rooms (All Rooms at edge of map; Biggest 6 x 4 with no rooms in the middle)
            ''' </summary>
            OuterRooms = &HA

            ''' <summary>
            ''' Small-Medium (Biggest 3 x 3)
            ''' </summary>
            SmallMedium3x3 = &HB

            ''' <summary>
            ''' Medium-Large (Biggest 6 x 4)
            ''' </summary>
            MediumLarge2 = &HC

            ''' <summary>
            ''' Medium-Large (Biggest 6 x 4)
            ''' </summary>
            MediumLarge3 = &HD

            ''' <summary>
            ''' Medium-Large (Biggest 6 x 4)
            ''' </summary>
            MediumLarge4 = &HE

            ''' <summary>
            ''' Medium-Large (Biggest 6 x 4)
            ''' </summary>
            MediumLarge5 = &HF
        End Enum

#Region "Structures"
        Public Structure FloorIndex
            '00-01	Attribute Index
            '02-03	Pokemon spawn Index
            '04-05	Trap spawn Index?
            '06-07	Item spawn Index
            '08-09	Kecleon shop Index (same as item)
            '0A-0B	Monster house item Index (same as item)
            '0C-0D	Buried item Index (same as item)
            '0E-0F	Unknown
            '10-11	Unknown

            Public Sub New(rawData As Byte())
                AttributeIndex = BitConverter.ToUInt16(rawData, 0)
                PokemonSpawnIndex = BitConverter.ToUInt16(rawData, 2)
                TrapSpawnIndex = BitConverter.ToUInt16(rawData, 4)
                ItemSpawnIndex = BitConverter.ToUInt16(rawData, 6)
                KeckleonShopIndex = BitConverter.ToUInt16(rawData, 8)
                MonsterHouseItemIndex = BitConverter.ToUInt16(rawData, 10)
                BuriedItemIndex = BitConverter.ToUInt16(rawData, 12)
                Unknown1 = BitConverter.ToUInt16(rawData, 14)
                Unknown2 = BitConverter.ToUInt16(rawData, 16)
            End Sub

            Public Function GetBytes() As Byte()
                Dim out As New List(Of Byte)(18)
                out.AddRange(BitConverter.GetBytes(AttributeIndex))
                out.AddRange(BitConverter.GetBytes(PokemonSpawnIndex))
                out.AddRange(BitConverter.GetBytes(TrapSpawnIndex))
                out.AddRange(BitConverter.GetBytes(ItemSpawnIndex))
                out.AddRange(BitConverter.GetBytes(KeckleonShopIndex))
                out.AddRange(BitConverter.GetBytes(MonsterHouseItemIndex))
                out.AddRange(BitConverter.GetBytes(BuriedItemIndex))
                out.AddRange(BitConverter.GetBytes(Unknown1))
                out.AddRange(BitConverter.GetBytes(Unknown2))
                Return out.ToArray
            End Function

            Public Property AttributeIndex As UInt16
            Public Property PokemonSpawnIndex As UInt16
            Public Property TrapSpawnIndex As UInt16
            Public Property ItemSpawnIndex As UInt16
            Public Property KeckleonShopIndex As UInt16
            Public Property MonsterHouseItemIndex As UInt16
            Public Property BuriedItemIndex As UInt16
            Public Property Unknown1 As UInt16
            Public Property Unknown2 As UInt16

            Public Function IsDefault() As Boolean
                Return AttributeIndex = 0 AndAlso PokemonSpawnIndex = 0 AndAlso TrapSpawnIndex = 0 AndAlso ItemSpawnIndex = 0 AndAlso KeckleonShopIndex = 0 AndAlso MonsterHouseItemIndex = 0 AndAlso BuriedItemIndex = 0 AndAlso Unknown1 = 0 AndAlso Unknown2 = 0
            End Function
        End Structure

        Public Structure FloorAttribute
            Public Sub New(rawData As Byte())
                Layout = rawData(0)
                Unknown1 = rawData(1)
                TerrainAppearance = rawData(2)
                MusicIndex = rawData(3)
                Weather = rawData(4)
                Unknown5 = rawData(5)
                InitialPokemonDensity = rawData(6)
                KeckleonShopPercentage = rawData(7)
                MonsterHousePercentage = rawData(8)
                Flag9 = rawData(9)
                UnknownA = rawData(&HA)
                FlagB = rawData(&HB)
                RoomsWithWaterIndex = rawData(&HC)
                FlagD = rawData(&HD)
                FlagE = rawData(&HE)
                ItemDensity = rawData(&HF)
                TrapDensity = rawData(&H10)
                FloorCounter = rawData(&H11)
                EventIndex = rawData(&H12)
                Unknown13 = rawData(&H13)
                BuriedItemDensity = rawData(&H14)
                WaterDensity = rawData(&H15)
                DarknessLevel = rawData(&H16)
                CoinMax = rawData(&H17)
                Unknown18 = rawData(&H18)
                Unknown19 = rawData(&H19)
                Unknown1A = rawData(&H1A)
                Flag1B = rawData(&H1B)
                EnemyIQ = BitConverter.ToUInt16(rawData, &H1C)
            End Sub

            Public Function GetBytes() As Byte()
                Dim out As New List(Of Byte)(32)
                out.Add(Layout)
                out.Add(Unknown1)
                out.Add(TerrainAppearance)
                out.Add(MusicIndex)
                out.Add(Weather)
                out.Add(Unknown5)
                out.Add(InitialPokemonDensity)
                out.Add(KeckleonShopPercentage)
                out.Add(MonsterHousePercentage)
                out.Add(Flag9)
                out.Add(UnknownA)
                out.Add(FlagB)
                out.Add(RoomsWithWaterIndex)
                out.Add(FlagD)
                out.Add(FlagE)
                out.Add(ItemDensity)
                out.Add(TrapDensity)
                out.Add(FloorCounter)
                out.Add(EventIndex)
                out.Add(Unknown13)
                out.Add(BuriedItemDensity)
                out.Add(WaterDensity)
                out.Add(DarknessLevel)
                out.Add(CoinMax)
                out.Add(Unknown18)
                out.Add(Unknown19)
                out.Add(Unknown1A)
                out.Add(Flag1B)
                out.AddRange(BitConverter.GetBytes(EnemyIQ))
                Return out.ToArray
            End Function

            Public Property Layout As FloorStructure
            Public Property Unknown1 As Byte
            Public Property TerrainAppearance As Byte
            Public Property MusicIndex As Byte
            Public Property Weather As Byte
            Public Property Unknown5 As Byte
            Public Property InitialPokemonDensity As Byte
            Public Property KeckleonShopPercentage As Byte
            Public Property MonsterHousePercentage As Byte
            Public Property Flag9 As Byte
            Public Property UnknownA As Byte
            Public Property FlagB As Byte
            Public Property RoomsWithWaterIndex As Byte
            Public Property FlagD As Byte
            Public Property FlagE As Byte
            Public Property ItemDensity As Byte
            Public Property TrapDensity As Byte
            Public Property FloorCounter As Byte
            Public Property EventIndex As Byte
            Public Property Unknown13 As Byte
            Public Property BuriedItemDensity As Byte
            Public Property WaterDensity As Byte
            Public Property DarknessLevel As Byte
            Public Property CoinMax As Byte 'Steps of 40
            Public Property Unknown18 As Byte
            Public Property Unknown19 As Byte
            Public Property Unknown1A As Byte
            Public Property Flag1B As Byte
            Public Property EnemyIQ As UInt16
        End Structure

        Public Structure PokemonSpawn
            '00  Unknown
            '01  Levelx2
            '02-03	Probability of appearing?
            '04-05	Probability of appearing?
            '06-07	Pokemon ID

            Public Sub New(rawData As Byte())
                Unknown = rawData(0)
                LevelX2 = rawData(1)
                Probability1 = BitConverter.ToUInt16(rawData, 2)
                Probability2 = BitConverter.ToUInt16(rawData, 4)
                PokemonID = BitConverter.ToUInt16(rawData, 6)
            End Sub

            Public Function GetBytes() As Byte()
                Dim out As New List(Of Byte)(8)
                out.Add(Unknown)
                out.Add(LevelX2)
                out.AddRange(BitConverter.GetBytes(Probability1))
                out.AddRange(BitConverter.GetBytes(Probability2))
                out.AddRange(BitConverter.GetBytes(PokemonID))
                Return out.ToArray
            End Function

            Public Property Unknown As Byte
            Public Property LevelX2 As Byte
            Public Property Probability1 As UInt16
            Public Property Probability2 As UInt16
            Public Property PokemonID As UInt16

            Public Function IsDefault() As Boolean
                Return Unknown = 0 AndAlso LevelX2 = 0 AndAlso Probability1 = 0 AndAlso Probability2 = 0 AndAlso PokemonID = 0
            End Function
        End Structure

#End Region

        Public Class DungeonBalance
            Public Sub New()
                Floors = New List(Of FloorBalance)
            End Sub
            Public Property Floors As List(Of FloorBalance)
        End Class

        Public Class FloorBalance
            Public Sub New()
                PokemonSpawns = New List(Of PokemonSpawn)
            End Sub
            Public Property PokemonSpawns As List(Of PokemonSpawn)
            Public Property Attributes As FloorAttribute
        End Class

#Region "Open"

        Public Overrides Async Function OpenFile(Filename As String, Provider As IOProvider) As Task
            Await MyBase.OpenFile(Filename, Provider)

            'Load pointers
            Dim floorIndexPointerBlockStart As Integer = BitConverter.ToInt32(Header, 0)
            Dim attributeBlockPointer As Integer = BitConverter.ToInt32(Header, 4)
            Dim itemSpawnPointerBlockPointer As Integer = BitConverter.ToInt32(Header, 8)
            Dim spawnPointerBlockStart As Integer = BitConverter.ToInt32(Header, &HC) 'Block of pointers
            Dim ptr5 As Integer = BitConverter.ToInt32(Header, &H10)

            Dim pkmSpawnPtr1 As Integer = Me.Int32(spawnPointerBlockStart)
            Dim itemSpawnPtr1 As Integer = Me.Int32(itemSpawnPointerBlockPointer)

            'Load sections
            LoadFloorIndex(floorIndexPointerBlockStart)
            LoadAttributeData(attributeBlockPointer, pkmSpawnPtr1)
            LoadItemSpawns(itemSpawnPointerBlockPointer)
            LoadPokemonSpawns(spawnPointerBlockStart)
            LoadBlock5(ptr5, itemSpawnPtr1)

            'Consolidate everything that was just read
            ProcessBlocks()
        End Function

        Private Sub LoadFloorIndex(pointerBlockPointer As Integer)
            RawFloorIndexes = New List(Of List(Of FloorIndex))

            Dim currentPointerOffset = pointerBlockPointer 'Pointer to the currently processing pointer
            Dim entryPointer As Integer = Me.Int32(currentPointerOffset) 'Pointer to the currently processing entry
            Dim floorEntries As New List(Of FloorIndex) 'Buffer of entries in a single group

            Dim isFirstEntry As Boolean 'Used in processing entries
            Do
                'Read a single group of floor indexes
                Dim currentIndex As FloorIndex
                isFirstEntry = True
                Do
                    'Read a single Floor Index
                    currentIndex = New FloorIndex(Me.RawData(entryPointer, 18))

                    If (currentIndex.IsDefault AndAlso Not isFirstEntry) Then
                        'Then we've reached the end
                        Exit Do
                    Else
                        If Not isFirstEntry Then 'Skip the first entry since it's null
                            floorEntries.Add(currentIndex)
                        End If
                    End If

                    isFirstEntry = False
                    entryPointer += 18

                    If entryPointer >= pointerBlockPointer Then
                        'We've reached the end of the last entry
                        Exit Do
                    End If
                Loop

                'Add current entry buffer to list of processed entries
                RawFloorIndexes.Add(floorEntries)
                floorEntries = New List(Of FloorIndex)

                If entryPointer >= pointerBlockPointer Then
                    'Stop after the last empty spawn entry
                    Exit Do
                Else
                    'Go to the next PokemonSpawn list
                    currentPointerOffset += 4
                    entryPointer = Me.Int32(currentPointerOffset)
                End If
            Loop
        End Sub

        Private Sub LoadAttributeData(attributeBlockPointer As Integer, nextPointer As Integer)
            RawAttributeData = New List(Of FloorAttribute)
            For count = attributeBlockPointer To nextPointer - 1 Step 32
                RawAttributeData.Add(New FloorAttribute(RawData(count, 32)))
            Next
        End Sub

        Private Sub LoadItemSpawns(pointerBlockPointer As Integer)
            'Parse the pointers in the pointer block
            Dim pointers As New List(Of Integer)
            Dim pointerPtr As Integer = pointerBlockPointer
            Do
                Dim pointer = Me.Int32(pointerPtr)
                pointerPtr += 4

                If pointer = &HAAAAAAAA OrElse pointer = 0 Then
                    Exit Do 'We've reached padding.
                ElseIf pointerPtr = Me.HeaderOffset Then
                    'The next pointer is in the header, which is outside of this block
                    pointers.Add(pointer) 'The current pointer is good
                    Exit Do 'But stop looking for them
                Else
                    pointers.Add(pointer)
                End If
            Loop

            'Read the item spawn data
            SuperRawItemSpawnData = New List(Of Byte())
            For count = 0 To pointers.Count - 1
                Dim currentItemPointer = pointers(count)
                Dim nextItemPointer As Integer

                If count < pointers.Count - 1 Then
                    'Compare to next pointer to determine length
                    nextItemPointer = pointers(count + 1)
                Else
                    'Compare to start of pointer block to determine length
                    nextItemPointer = pointerBlockPointer
                End If

                Dim length = nextItemPointer - currentItemPointer

                SuperRawItemSpawnData.Add(RawData(currentItemPointer, length))
            Next
        End Sub

        Private Sub LoadPokemonSpawns(pointerBlockPointer As Integer)
            RawPokemonSpawns = New List(Of List(Of PokemonSpawn))

            Dim currentPointerOffset = pointerBlockPointer 'Pointer to the currently processing pointer
            Dim entryPointer As Integer = Me.Int32(currentPointerOffset) 'Pointer to the currently processing entry
            Dim spawnEntries As New List(Of PokemonSpawn) 'Buffer of entries in a single group

            Do
                'Read a single PokemonSpawn list
                Dim currentSpawn As PokemonSpawn
                Do
                    currentSpawn = New PokemonSpawn(Me.RawData(entryPointer, 8))
                    entryPointer += 8

                    If currentSpawn.IsDefault Then 'Keep reading until the last entry is blank
                        Exit Do
                    Else
                        spawnEntries.Add(currentSpawn)
                    End If
                Loop

                'Add current entry buffer to the list of processed entries
                RawPokemonSpawns.Add(spawnEntries)
                spawnEntries = New List(Of PokemonSpawn)

                If entryPointer >= pointerBlockPointer Then
                    'Stop after the last empty spawn entry
                    Exit Do
                Else
                    'Go to the next PokemonSpawn list
                    currentPointerOffset += 4
                    entryPointer = Me.Int32(currentPointerOffset)
                End If
            Loop
        End Sub

        Private Sub LoadBlock5(pointerBlockPointer As Integer, pointerBlockEnd As Integer)
            'Parse the pointers in the pointer block
            Dim pointers As New List(Of Integer)
            For count = pointerBlockPointer To pointerBlockEnd - 1 Step 4
                pointers.Add(Me.Int32(count))
            Next

            'Read the item spawn data
            SuperRawBlock5 = New List(Of Byte())
            For count = 0 To pointers.Count - 1
                Dim currentItemPointer = pointers(count)
                Dim nextItemPointer As Integer

                If count < pointers.Count - 1 Then
                    'Compare to next pointer to determine length
                    nextItemPointer = pointers(count + 1)
                Else
                    'Compare to start of pointer block to determine length
                    nextItemPointer = pointerBlockPointer
                End If

                Dim length = nextItemPointer - currentItemPointer

                SuperRawBlock5.Add(RawData(currentItemPointer, length))
            Next
        End Sub

        Private Sub ProcessBlocks()
            Dungeons = New List(Of DungeonBalance)

            For Each dungeon In RawFloorIndexes
                Dim dungeonEntry As New DungeonBalance

                For Each floor In dungeon
                    Dim floorEntry As New FloorBalance

                    floorEntry.Attributes = RawAttributeData(floor.AttributeIndex)

                    'Pokemon spawns
                    For Each spawn In RawPokemonSpawns(floor.PokemonSpawnIndex)
                        floorEntry.PokemonSpawns.Add(spawn)
                    Next

                    dungeonEntry.Floors.Add(floorEntry)
                Next

                Dungeons.Add(dungeonEntry)
            Next
        End Sub

#End Region

        Public Property Dungeons As List(Of DungeonBalance)

        Private Property RawFloorIndexes As List(Of List(Of FloorIndex))
        Private Property RawAttributeData As List(Of FloorAttribute)
        Private Property RawPokemonSpawns As List(Of List(Of PokemonSpawn))
        Private Property SuperRawItemSpawnData As List(Of Byte())
        Private Property SuperRawBlock5 As List(Of Byte())


    End Class
End Namespace

