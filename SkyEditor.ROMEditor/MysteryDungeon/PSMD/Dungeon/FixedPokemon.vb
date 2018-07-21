Imports System.IO
Imports SkyEditor.Core.IO

Namespace MysteryDungeon.PSMD.Dungeon
    Public Class FixedPokemon
        Inherits Sir0
        Implements IDetectableFileType

        Public Shared Shadows Async Function IsFileOfType(file As GenericFile) As Task(Of Boolean)
            Return Await Sir0.IsFileOfType(file) AndAlso Path.GetFileName(file.Filename) = "fixed_pokemon.bin"
        End Function

        Public Class PokemonEntry
            Public Sub New(RawData As Byte())
                Data = RawData

                PokemonID = BitConverter.ToInt16(RawData, 0)
                HPBoost = BitConverter.ToInt16(RawData, 2)
                Move1 = BitConverter.ToUInt16(RawData, 8)
                Move2 = BitConverter.ToUInt16(RawData, &HA)
                Move3 = BitConverter.ToUInt16(RawData, &HC)
                Move4 = BitConverter.ToUInt16(RawData, &HE)
                Level = RawData(&H16)
                AttackBoost = Data(&H17)
                SpAttackBoost = Data(&H18)
                DefenseBoost = Data(&H19)
                SpDefenseBoost = Data(&H1A)
                SpeedBoost = Data(&H1B)
            End Sub

            Public Sub New()
                Dim tmp(&H30 - 1) As Byte
                PokemonID = 0
                Move1 = 0
                Move2 = 0
                Move3 = 0
                Move4 = 0
                Data = tmp
            End Sub

            Private Property Data As Byte()
            Public Property PokemonID As Int16
            Public Property Move1 As UInt16
            Public Property Move2 As UInt16
            Public Property Move3 As UInt16
            Public Property Move4 As UInt16
            Public Property HPBoost As Int16
            Public Property AttackBoost As Byte
            Public Property SpAttackBoost As Byte
            Public Property DefenseBoost As Byte
            Public Property SpDefenseBoost As Byte
            Public Property SpeedBoost As Byte
            Public Property Level As Byte

            Public Function GetBytes() As Byte()
                Dim pid = BitConverter.GetBytes(PokemonID)
                Dim m1 = BitConverter.GetBytes(Move1)
                Dim m2 = BitConverter.GetBytes(Move2)
                Dim m3 = BitConverter.GetBytes(Move3)
                Dim m4 = BitConverter.GetBytes(Move4)

                For count = 0 To 1
                    Data(0 + count) = pid(count)
                    Data(8 + count) = m1(count)
                    Data(&HA + count) = m2(count)
                    Data(&HC + count) = m3(count)
                    Data(&HE + count) = m4(count)
                Next

                Data(&H16) = Level
                Data(&H17) = AttackBoost
                Data(&H18) = SpAttackBoost
                Data(&H19) = DefenseBoost
                Data(&H1A) = SpDefenseBoost
                Data(&H1B) = SpeedBoost

                Return Data
            End Function

            Public Overrides Function ToString() As String
                Return PokemonID.ToString
            End Function

        End Class

        Public Sub New()
            MyBase.New
            Entries = New List(Of PokemonEntry)
        End Sub

        Public Property Entries As List(Of PokemonEntry)

        ''' <summary>
        ''' Gets the entries that define starter Pokemon (and sometimes their 2nd non-final form).
        ''' </summary>
        ''' <returns></returns>
        Protected ReadOnly Property StarterEntries As List(Of PokemonEntry)
            Get
                Dim out As New List(Of PokemonEntry)

                For count = 17 To 54
                    out.Add(Entries(count))
                Next

                Return out
            End Get
        End Property

        '''' <summary>
        '''' Gets the entries that define the forms of Pokemon when evolved through harmony scarves.
        '''' </summary>
        '''' <returns></returns>
        'Protected ReadOnly Property EvolvedEntries As List(Of PokemonEntry)
        '    Get

        '    End Get
        'End Property

        Public Overrides Async Function OpenFile(Filename As String, Provider As IIOProvider) As Task
            Await MyBase.OpenFile(Filename, Provider)

            Dim numEntries = BitConverter.ToUInt32(Me.ContentHeader, 0)
            'Unknown integer at 0x4

            For count = 0 To numEntries - 2 'Subtract 1 because we're talking indexes, subtract another it seems like it's 1 too high afterward, and I don't know why
                Dim dataPointer = BitConverter.ToUInt32(Me.ContentHeader, 8 + count * 4)
                Entries.Add(New PokemonEntry(Await Me.ReadAsync(dataPointer, &H30)))
            Next
        End Function

        Public Overrides Async Function Save(Destination As String, provider As IIOProvider) As Task
            Me.RelativePointers.Clear()
            'Sir0 header pointers
            Me.RelativePointers.Add(4)
            Me.RelativePointers.Add(4)

            'Generate sections
            Dim dataSection As New List(Of Byte)
            For Each item In Entries
                dataSection.AddRange(item.GetBytes)
            Next

            'Add pointers
            Me.RelativePointers.Add(dataSection.Count + 12)
            For count = 0 To Entries.Count - 2
                Me.RelativePointers.Add(&H4)
            Next

            'Write sections to file
            Me.Length = 16 + dataSection.Count
            Await Me.WriteAsync(16, dataSection.Count, dataSection.ToArray)

            'Update header
            Dim headerBytes As New List(Of Byte)
            headerBytes.AddRange(BitConverter.GetBytes(Entries.Count))
            headerBytes.AddRange({0, 0, 0, 0})
            For count = 0 To Entries.Count - 1
                headerBytes.AddRange(BitConverter.GetBytes(&H10 + &H30 * count))
            Next
            Me.ContentHeader = headerBytes.ToArray

            'Let the general SIR0 stuff happen
            Await MyBase.Save(Destination, provider)
        End Function

        Private Async Function IDetectableFileType_IsOfType(file As GenericFile) As Task(Of Boolean) Implements IDetectableFileType.IsOfType
            Return Await FixedPokemon.IsFileOfType(file)
        End Function
    End Class

End Namespace
