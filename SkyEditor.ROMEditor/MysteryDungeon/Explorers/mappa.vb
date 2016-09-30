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
        End Class

        Public Overrides Async Function OpenFile(Filename As String, Provider As IOProvider) As Task
            Await MyBase.OpenFile(Filename, Provider)

            'Floor Indexes
            Dim floorIndexPointerBlockStart As Integer = BitConverter.ToInt32(Header, 0)
            LoadFloorIndex(floorIndexPointerBlockStart)

            Dim ptr2 As Integer = BitConverter.ToInt32(Header, 4) 'Block of data, Floor Attribute Data, 32 byte entries
            Dim ptr3 As Integer = BitConverter.ToInt32(Header, 8) 'Block of pointers

            'Pokemon Spawns
            Dim spawnPointerBlockStart As Integer = BitConverter.ToInt32(Header, &HC) 'Block of pointers
            LoadPokemonSpawns(spawnPointerBlockStart)

            'Item Spawns
            Dim ptr5 As Integer = BitConverter.ToInt32(Header, &H10)
            'Each pointer points to a 50 byte (0x32) entry

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

        Private Sub ProcessBlocks()
            Dungeons = New List(Of DungeonBalance)

            For Each dungeon In RawFloorIndexes
                Dim dungeonEntry As New DungeonBalance

                For Each floor In dungeon
                    Dim floorEntry As New FloorBalance

                    'Pokemon spawns
                    For Each spawn In RawPokemonSpawns(floor.PokemonSpawnIndex)
                        floorEntry.PokemonSpawns.Add(spawn)
                    Next

                    dungeonEntry.Floors.Add(floorEntry)
                Next

                Dungeons.Add(dungeonEntry)
            Next
        End Sub

        Public Property Dungeons As List(Of DungeonBalance)

        Private Property RawFloorIndexes As List(Of List(Of FloorIndex))
        Private Property RawPokemonSpawns As List(Of List(Of PokemonSpawn))

    End Class
End Namespace

