Imports SkyEditor.Core.IO

Namespace MysteryDungeon.Rescue
    Public Class BlueNorthAmericanArm9Bin
        Inherits GenericFile

#Region "Starter Block"
        Protected Property StarterBlockOffset As Integer = &HBE5F0

        Public Property Starter01 As UInt16
            Get
                Return ReadUInt16(StarterBlockOffset + 0)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 0, value)
            End Set
        End Property

        Public Property Starter02 As UInt16
            Get
                Return ReadUInt16(StarterBlockOffset + 2)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 2, value)
            End Set
        End Property

        Public Property Starter03 As UInt16
            Get
                Return ReadUInt16(StarterBlockOffset + 4)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 4, value)
            End Set
        End Property

        Public Property Starter04 As UInt16
            Get
                Return ReadUInt16(StarterBlockOffset + 6)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 6, value)
            End Set
        End Property

        Public Property Starter05 As UInt16
            Get
                Return ReadUInt16(StarterBlockOffset + 8)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 8, value)
            End Set
        End Property

        Public Property Starter06 As UInt16
            Get
                Return ReadUInt16(StarterBlockOffset + 10)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 10, value)
            End Set
        End Property

        Public Property Starter07 As UInt16
            Get
                Return ReadUInt16(StarterBlockOffset + 12)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 12, value)
            End Set
        End Property

        Public Property Starter08 As UInt16
            Get
                Return ReadUInt16(StarterBlockOffset + 14)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 14, value)
            End Set
        End Property

        Public Property Starter09 As UInt16
            Get
                Return ReadUInt16(StarterBlockOffset + 16)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 16, value)
            End Set
        End Property

        Public Property Starter10 As UInt16
            Get
                Return ReadUInt16(StarterBlockOffset + 18)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 18, value)
            End Set
        End Property

        Public Property Starter11 As UInt16
            Get
                Return ReadUInt16(StarterBlockOffset + 20)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 20, value)
            End Set
        End Property

        Public Property Starter12 As UInt16
            Get
                Return ReadUInt16(StarterBlockOffset + 22)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 22, value)
            End Set
        End Property

        Public Property Starter13 As UInt16
            Get
                Return ReadUInt16(StarterBlockOffset + 24)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 24, value)
            End Set
        End Property

        Public Property Starter14 As UInt16
            Get
                Return ReadUInt16(StarterBlockOffset + 26)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 26, value)
            End Set
        End Property

        Public Property Starter15 As UInt16
            Get
                Return ReadUInt16(StarterBlockOffset + 28)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 28, value)
            End Set
        End Property

        Public Property Starter16 As UInt16
            Get
                Return ReadUInt16(StarterBlockOffset + 30)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 30, value)
            End Set
        End Property

        Public Property Starter17 As UInt16
            Get
                Return ReadUInt16(StarterBlockOffset + 32)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 32, value)
            End Set
        End Property

        Public Property Starter18 As UInt16
            Get
                Return ReadUInt16(StarterBlockOffset + 34)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 34, value)
            End Set
        End Property

        Public Property Starter19 As UInt16
            Get
                Return ReadUInt16(StarterBlockOffset + 36)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 36, value)
            End Set
        End Property

        Public Property Starter20 As UInt16
            Get
                Return ReadUInt16(StarterBlockOffset + 38)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 38, value)
            End Set
        End Property

        Public Property Starter21 As UInt16
            Get
                Return ReadUInt16(StarterBlockOffset + 40)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 40, value)
            End Set
        End Property

        Public Property Starter22 As UInt16
            Get
                Return ReadUInt16(StarterBlockOffset + 42)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 42, value)
            End Set
        End Property

        Public Property Starter23 As UInt16
            Get
                Return ReadUInt16(StarterBlockOffset + 44)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 44, value)
            End Set
        End Property

        Public Property Starter24 As UInt16
            Get
                Return ReadUInt16(StarterBlockOffset + 46)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 46, value)
            End Set
        End Property

        Public Property Starter25 As UInt16
            Get
                Return ReadUInt16(StarterBlockOffset + 48)
            End Get
            Set(value As UInt16)
                WriteUInt16(StarterBlockOffset + 48, value)
            End Set
        End Property

        Public Property Starter26 As UInt16
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

        Public Property Partner01 As UInt16
            Get
                Return ReadUInt16(PartnerBlockOffset + 0)
            End Get
            Set(value As UInt16)
                WriteUInt16(PartnerBlockOffset + 0, value)
            End Set
        End Property

        Public Property Partner02 As UInt16
            Get
                Return ReadUInt16(PartnerBlockOffset + 2)
            End Get
            Set(value As UInt16)
                WriteUInt16(PartnerBlockOffset + 2, value)
            End Set
        End Property

        Public Property Partner03 As UInt16
            Get
                Return ReadUInt16(PartnerBlockOffset + 4)
            End Get
            Set(value As UInt16)
                WriteUInt16(PartnerBlockOffset + 4, value)
            End Set
        End Property

        Public Property Partner04 As UInt16
            Get
                Return ReadUInt16(PartnerBlockOffset + 6)
            End Get
            Set(value As UInt16)
                WriteUInt16(PartnerBlockOffset + 6, value)
            End Set
        End Property

        Public Property Partner05 As UInt16
            Get
                Return ReadUInt16(PartnerBlockOffset + 8)
            End Get
            Set(value As UInt16)
                WriteUInt16(PartnerBlockOffset + 8, value)
            End Set
        End Property

        Public Property Partner06 As UInt16
            Get
                Return ReadUInt16(PartnerBlockOffset + 10)
            End Get
            Set(value As UInt16)
                WriteUInt16(PartnerBlockOffset + 10, value)
            End Set
        End Property

        Public Property Partner07 As UInt16
            Get
                Return ReadUInt16(PartnerBlockOffset + 12)
            End Get
            Set(value As UInt16)
                WriteUInt16(PartnerBlockOffset + 12, value)
            End Set
        End Property

        Public Property Partner08 As UInt16
            Get
                Return ReadUInt16(PartnerBlockOffset + 14)
            End Get
            Set(value As UInt16)
                WriteUInt16(PartnerBlockOffset + 14, value)
            End Set
        End Property

        Public Property Partner09 As UInt16
            Get
                Return ReadUInt16(PartnerBlockOffset + 16)
            End Get
            Set(value As UInt16)
                WriteUInt16(PartnerBlockOffset + 16, value)
            End Set
        End Property

        Public Property Partner10 As UInt16
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

