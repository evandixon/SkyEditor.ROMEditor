Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Utilities

Namespace MysteryDungeon.Explorers
    Public Class MonsterMDEntry
        Public Enum PokemonGender As Byte
            Invalid = 0
            Male = 1
            Female = 2
            Genderless = 3
        End Enum

        Public Enum PokemonEvolutionMethod As UInt16
            None = 0
            Level = 1
            IQ = 2
            Items = 3
            Unknown = 4
            LinkCable = 5
        End Enum

        Public Enum EvolutionaryItem As Byte
            None = 0
            LinkCable = 1
            Unknown2 = 2
            Unknown3 = 3
            Unknown4 = 4
            SunRibbon = 5
            LunarRibbon = 6
            BeautyScarf = 7
        End Enum

        Public Enum PokemonMovementType As Byte
            Standard = 0
            Unknown1 = 1
            Hovering = 2
            Unknown3 = 3
            Lava = 4
            Water = 5
        End Enum

        Public Property EntityID As UInt16
        Public Property Unk_02 As UInt16
        Public Property DexNumber As UInt16 'National Pokédex number, as displayed in Chimecho's assembly.
        Public Property Unk_06 As UInt16 'Classification?
        Public Property EvolveFrom As UInt16 'Index of entity in Monster.MD.  NOT the EntityID.
        Public Property EvolveMethod As PokemonEvolutionMethod
        Public Property EvolveParam As UInt16
        Public Property EvolveItem As EvolutionaryItem
        Public Property SpriteIndex As UInt16
        Public Property Gender As PokemonGender
        Public Property BodySize As Byte
        Public Property MainType As Byte
        Public Property AltType As Byte
        Public Property MovementType As PokemonMovementType
        Public Property IqGroup As Byte
        Public Property Ability1 As Byte
        Public Property Ability2 As Byte
        Public Property Unk_1a As UInt16
        Public Property ExpYield As UInt16
        Public Property RecruitRate As UInt16
        Public Property BaseHP As UInt16
        Public Property RecruitRate2 As UInt16 'Possibly
        Public Property BaseATK As Byte
        Public Property BaseSPATK As Byte
        Public Property BaseDEF As Byte
        Public Property BaseSPDEF As Byte
        Public Property Weight As UInt16
        Public Property Size As UInt16
        Public Property Unk_29 As UInt32
        Public Property BaseFormIndex As UInt16 'Index of entity in Monster.MD
        Public Property ExclusiveItem1 As UInt16
        Public Property ExclusiveItem2 As UInt16
        Public Property ExclusiveItem3 As UInt16
        Public Property ExclusiveItem4 As UInt16
        Public Property Unk3C As UInt16
        Public Property Unk3E As UInt16
        Public Property Unk40 As UInt16
        Public Property Unk42 As UInt16
        Public Shared Function FromBytes(RawData As Byte())
            Dim e As New MonsterMDEntry
            e.EntityID = BitConverter.ToUInt16(RawData, 0)
            e.Unk_02 = BitConverter.ToUInt16(RawData, 2)
            e.DexNumber = BitConverter.ToUInt16(RawData, 4)
            e.Unk_06 = BitConverter.ToUInt16(RawData, 6)
            e.EvolveFrom = BitConverter.ToUInt16(RawData, 8)
            e.EvolveMethod = BitConverter.ToUInt16(RawData, &HA)
            e.EvolveParam = BitConverter.ToUInt16(RawData, &HC)
            e.EvolveItem = BitConverter.ToUInt16(RawData, &HE)
            e.SpriteIndex = BitConverter.ToUInt16(RawData, &H10)
            e.Gender = RawData(&H12)
            e.BodySize = RawData(&H13)
            e.MainType = RawData(&H14)
            e.AltType = RawData(&H15)
            e.MovementType = RawData(&H16)
            e.IqGroup = RawData(&H17)
            e.Ability1 = RawData(&H18)
            e.Ability2 = RawData(&H19)
            e.Unk_1a = BitConverter.ToUInt16(RawData, &H1A)
            e.ExpYield = BitConverter.ToUInt16(RawData, &H1C)
            e.RecruitRate = BitConverter.ToUInt16(RawData, &H1E)
            e.BaseHP = BitConverter.ToUInt16(RawData, &H20)
            e.RecruitRate2 = BitConverter.ToUInt16(RawData, &H22)
            e.BaseATK = RawData(&H24)
            e.BaseSPATK = RawData(&H25)
            e.BaseDEF = RawData(&H26)
            e.BaseSPDEF = RawData(&H27)
            e.Weight = BitConverter.ToUInt16(RawData, &H28)
            e.Size = BitConverter.ToUInt16(RawData, &H2A)
            e.Unk_29 = BitConverter.ToUInt16(RawData, &H30)
            e.BaseFormIndex = BitConverter.ToUInt16(RawData, &H32)
            e.ExclusiveItem1 = BitConverter.ToUInt16(RawData, &H34)
            e.ExclusiveItem2 = BitConverter.ToUInt16(RawData, &H36)
            e.ExclusiveItem3 = BitConverter.ToUInt16(RawData, &H38)
            e.ExclusiveItem4 = BitConverter.ToUInt16(RawData, &H3A)
            e.Unk3C = BitConverter.ToUInt16(RawData, &H3C)
            e.Unk3E = BitConverter.ToUInt16(RawData, &H3E)
            e.Unk40 = BitConverter.ToUInt16(RawData, &H40)
            e.Unk42 = BitConverter.ToUInt16(RawData, &H42)
            Return e
        End Function
    End Class
    ''' <summary>
    ''' Not 100% correct
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MonsterMDFile
        Implements IOpenableFile

        Public Shared Function FromBytes(RawData As Byte())
            Dim out As New MonsterMDFile
            out.ProcessData(RawData)
            Return out
        End Function

        Private Property Magic As Int32
        Private Property NumberOfEntries As UInt32
        Public Property Entries As Generic.List(Of MonsterMDEntry)

        Public Function OpenFile(filename As String, provider As IIOProvider) As Task Implements IOpenableFile.OpenFile
            Dim rawData = provider.ReadAllBytes(filename)
            ProcessData(rawData)
            Return Task.CompletedTask
        End Function

        Protected Sub ProcessData(rawData As Byte())
            Entries = New List(Of MonsterMDEntry)
            Magic = BitConverter.ToInt32(rawData, 0) 'MD\0\0
            NumberOfEntries = BitConverter.ToUInt32(rawData, 4)
            For count As UInteger = 0 To NumberOfEntries - 1
                Entries.Add(MonsterMDEntry.FromBytes(rawData.Skip(8 + (count * &H44)).Take(&H44).ToArray))
            Next
        End Sub
    End Class
End Namespace
