Imports SkyEditor.Core.IO

Namespace MysteryDungeon.Rescue
    Public Class BlueNorthAmericanArm9Bin
        Inherits GenericFile
        Implements IRescueTeamStarters

#Region "Starter Block"
        Protected Property StarterBlockOffset As Integer = &HBE5F0

        Public Property HardyMale As UInt16 Implements IRescueTeamStarters.HardyMale
            Get
                Return ReadUInt16(StarterBlockOffset + 0)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 0, value)
            End Set
        End Property

        Public Property HardyFemale As UInt16 Implements IRescueTeamStarters.HardyFemale
            Get
                Return ReadUInt16(StarterBlockOffset + 2)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 2, value)
            End Set
        End Property

        Public Property DocileMale As UInt16 Implements IRescueTeamStarters.DocileMale
            Get
                Return ReadUInt16(StarterBlockOffset + 4)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 4, value)
            End Set
        End Property

        Public Property DocileFemale As UInt16 Implements IRescueTeamStarters.DocileFemale
            Get
                Return ReadUInt16(StarterBlockOffset + 6)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 6, value)
            End Set
        End Property

        Public Property BraveMale As UInt16 Implements IRescueTeamStarters.BraveMale
            Get
                Return ReadUInt16(StarterBlockOffset + 8)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 8, value)
            End Set
        End Property

        Public Property BraveFemale As UInt16 Implements IRescueTeamStarters.BraveFemale
            Get
                Return ReadUInt16(StarterBlockOffset + 10)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 10, value)
            End Set
        End Property

        Public Property JollyMale As UInt16 Implements IRescueTeamStarters.JollyMale
            Get
                Return ReadUInt16(StarterBlockOffset + 12)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 12, value)
            End Set
        End Property

        Public Property JollyFemale As UInt16 Implements IRescueTeamStarters.JollyFemale
            Get
                Return ReadUInt16(StarterBlockOffset + 14)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 14, value)
            End Set
        End Property

        Public Property ImpishMale As UInt16 Implements IRescueTeamStarters.ImpishMale
            Get
                Return ReadUInt16(StarterBlockOffset + 16)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 16, value)
            End Set
        End Property

        Public Property ImpishFemale As UInt16 Implements IRescueTeamStarters.ImpishFemale
            Get
                Return ReadUInt16(StarterBlockOffset + 18)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 18, value)
            End Set
        End Property

        Public Property NaiveMale As UInt16 Implements IRescueTeamStarters.NaiveMale
            Get
                Return ReadUInt16(StarterBlockOffset + 20)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 20, value)
            End Set
        End Property

        Public Property NaiveFemale As UInt16 Implements IRescueTeamStarters.NaiveFemale
            Get
                Return ReadUInt16(StarterBlockOffset + 22)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 22, value)
            End Set
        End Property

        Public Property TimidMale As UInt16 Implements IRescueTeamStarters.TimidMale
            Get
                Return ReadUInt16(StarterBlockOffset + 24)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 24, value)
            End Set
        End Property

        Public Property TimidFemale As UInt16 Implements IRescueTeamStarters.TimidFemale
            Get
                Return ReadUInt16(StarterBlockOffset + 26)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 26, value)
            End Set
        End Property

        Public Property HastyMale As UInt16 Implements IRescueTeamStarters.HastyMale
            Get
                Return ReadUInt16(StarterBlockOffset + 28)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 28, value)
            End Set
        End Property

        Public Property HastyFemale As UInt16 Implements IRescueTeamStarters.HastyFemale
            Get
                Return ReadUInt16(StarterBlockOffset + 30)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 30, value)
            End Set
        End Property

        Public Property SassyMale As UInt16 Implements IRescueTeamStarters.SassyMale
            Get
                Return ReadUInt16(StarterBlockOffset + 32)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 32, value)
            End Set
        End Property

        Public Property SassyFemale As UInt16 Implements IRescueTeamStarters.SassyFemale
            Get
                Return ReadUInt16(StarterBlockOffset + 34)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 34, value)
            End Set
        End Property

        Public Property CalmMale As UInt16 Implements IRescueTeamStarters.CalmMale
            Get
                Return ReadUInt16(StarterBlockOffset + 36)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 36, value)
            End Set
        End Property

        Public Property CalmFemale As UInt16 Implements IRescueTeamStarters.CalmFemale
            Get
                Return ReadUInt16(StarterBlockOffset + 38)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 38, value)
            End Set
        End Property

        Public Property RelaxedMale As UInt16 Implements IRescueTeamStarters.RelaxedMale
            Get
                Return ReadUInt16(StarterBlockOffset + 40)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 40, value)
            End Set
        End Property

        Public Property RelaxedFemale As UInt16 Implements IRescueTeamStarters.RelaxedFemale
            Get
                Return ReadUInt16(StarterBlockOffset + 42)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 42, value)
            End Set
        End Property

        Public Property LonelyMale As UInt16 Implements IRescueTeamStarters.LonelyMale
            Get
                Return ReadUInt16(StarterBlockOffset + 44)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 44, value)
            End Set
        End Property

        Public Property LonelyFemale As UInt16 Implements IRescueTeamStarters.LonelyFemale
            Get
                Return ReadUInt16(StarterBlockOffset + 46)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 46, value)
            End Set
        End Property

        Public Property QuirkyMale As UInt16 Implements IRescueTeamStarters.QuirkyMale
            Get
                Return ReadUInt16(StarterBlockOffset + 48)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 48, value)
            End Set
        End Property

        Public Property QuirkyFemale As UInt16 Implements IRescueTeamStarters.QuirkyFemale
            Get
                Return ReadUInt16(StarterBlockOffset + 50)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 50, value)
            End Set
        End Property
#End Region

#Region "Parter Block"
        Protected Property PartnerBlockOffset As Integer = &HC0B64

        Public Property Partner01 As UInt16 Implements IRescueTeamStarters.Partner01
            Get
                Return ReadUInt16(PartnerBlockOffset + 0)
            End Get
            Set(value As UInt16)
                WriteUInt16(PartnerBlockOffset + 0, value)
            End Set
        End Property

        Public Property Partner02 As UInt16 Implements IRescueTeamStarters.Partner02
            Get
                Return ReadUInt16(PartnerBlockOffset + 2)
            End Get
            Set(value As UInt16)
                WriteUInt16(PartnerBlockOffset + 2, value)
            End Set
        End Property

        Public Property Partner03 As UInt16 Implements IRescueTeamStarters.Partner03
            Get
                Return ReadUInt16(PartnerBlockOffset + 4)
            End Get
            Set(value As UInt16)
                WriteUInt16(PartnerBlockOffset + 4, value)
            End Set
        End Property

        Public Property Partner04 As UInt16 Implements IRescueTeamStarters.Partner04
            Get
                Return ReadUInt16(PartnerBlockOffset + 6)
            End Get
            Set(value As UInt16)
                WriteUInt16(PartnerBlockOffset + 6, value)
            End Set
        End Property

        Public Property Partner05 As UInt16 Implements IRescueTeamStarters.Partner05
            Get
                Return ReadUInt16(PartnerBlockOffset + 8)
            End Get
            Set(value As UInt16)
                WriteUInt16(PartnerBlockOffset + 8, value)
            End Set
        End Property

        Public Property Partner06 As UInt16 Implements IRescueTeamStarters.Partner06
            Get
                Return ReadUInt16(PartnerBlockOffset + 10)
            End Get
            Set(value As UInt16)
                WriteUInt16(PartnerBlockOffset + 10, value)
            End Set
        End Property

        Public Property Partner07 As UInt16 Implements IRescueTeamStarters.Partner07
            Get
                Return ReadUInt16(PartnerBlockOffset + 12)
            End Get
            Set(value As UInt16)
                WriteUInt16(PartnerBlockOffset + 12, value)
            End Set
        End Property

        Public Property Partner08 As UInt16 Implements IRescueTeamStarters.Partner08
            Get
                Return ReadUInt16(PartnerBlockOffset + 14)
            End Get
            Set(value As UInt16)
                WriteUInt16(PartnerBlockOffset + 14, value)
            End Set
        End Property

        Public Property Partner09 As UInt16 Implements IRescueTeamStarters.Partner09
            Get
                Return ReadUInt16(PartnerBlockOffset + 16)
            End Get
            Set(value As UInt16)
                WriteUInt16(PartnerBlockOffset + 16, value)
            End Set
        End Property

        Public Property Partner10 As UInt16 Implements IRescueTeamStarters.Partner10
            Get
                Return ReadUInt16(PartnerBlockOffset + 18)
            End Get
            Set(value As UInt16)
                WriteUInt16(PartnerBlockOffset + 18, value)
            End Set
        End Property
#End Region

    End Class
End Namespace

