Imports SkyEditor.Core.IO

Namespace MysteryDungeon.Explorers
    Public Class item_p
        Inherits ExplorersSir0
        Implements IOpenableFile
        Public Property Items As List(Of Item)

        Public Class Item
            Public Property RawData As Byte()
            Public Property Index As Integer
            Public Property BuyPrice As UInt16
                Get
                    Return BitConverter.ToUInt16(RawData, 0)
                End Get
                Set(value As UInt16)
                    Dim b = BitConverter.GetBytes(value)
                    RawData(0) = b(0)
                    RawData(1) = b(1)
                End Set
            End Property
            Public Property SellPrice As UInt16
                Get
                    Return BitConverter.ToUInt16(RawData, 2)
                End Get
                Set(value As UInt16)
                    Dim b = BitConverter.GetBytes(value)
                    RawData(2) = b(0)
                    RawData(3) = b(1)
                End Set
            End Property
            Public Property Category As ItemCategory
                Get
                    Return RawData(4)
                End Get
                Set(value As ItemCategory)
                    RawData(4) = value
                End Set
            End Property
            ''' <summary>
            ''' Reference to the item sprite to be used in-game.
            ''' </summary>
            ''' <returns></returns>
            Public Property Sprite As Byte
                Get
                    Return RawData(5)
                End Get
                Set(value As Byte)
                    RawData(5) = value
                End Set
            End Property
            ''' <summary>
            ''' ID the game refers to the item by.
            ''' </summary>
            ''' <returns></returns>
            Public Property ID As UInt16
                Get
                    Return BitConverter.ToUInt16(RawData, 6)
                End Get
                Set(value As UInt16)
                    Dim b = BitConverter.GetBytes(value)
                    RawData(6) = b(0)
                    RawData(7) = b(1)
                End Set
            End Property
            ''' <summary>
            ''' If the item is an unused TM/HM or an Orb, this is the ID of the contained Move.
            ''' </summary>
            ''' <returns></returns>
            Public Property ItemParameter1 As UInt16
                Get
                    Return BitConverter.ToUInt16(RawData, 8)
                End Get
                Set(value As UInt16)
                    Dim b = BitConverter.GetBytes(value)
                    RawData(8) = b(0)
                    RawData(9) = b(1)
                End Set
            End Property
            Public Property B10 As Byte
                Get
                    Return RawData(10)
                End Get
                Set(value As Byte)
                    RawData(10) = value
                End Set
            End Property
            Public Property B11 As Byte
                Get
                    Return RawData(11)
                End Get
                Set(value As Byte)
                    RawData(11) = value
                End Set
            End Property
            Public Property B12 As Byte
                Get
                    Return RawData(12)
                End Get
                Set(value As Byte)
                    RawData(12) = value
                End Set
            End Property
            Public Property B13 As Byte
                Get
                    Return RawData(13)
                End Get
                Set(value As Byte)
                    RawData(13) = value
                End Set
            End Property
            Public Property B14 As Byte
                Get
                    Return RawData(14)
                End Get
                Set(value As Byte)
                    RawData(14) = value
                End Set
            End Property
            ''' <summary>
            ''' Unknown, usually is set to 0.
            ''' Probably padding to make the item's length 16 bytes.
            ''' </summary>
            ''' <returns></returns>
            Public Property B15 As Byte
                Get
                    Return RawData(15)
                End Get
                Set(value As Byte)
                    RawData(15) = value
                End Set
            End Property
            Public Sub New(Data As Byte())
                Me.RawData = Data
            End Sub
            Public Sub New()
                Me.New({0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0})
            End Sub
            Public Enum ItemCategory
                Projectile = 0
                Arc = 1
                Seed_Drink = 2
                Food_Gummi = 3
                HoldItem = 4
                HM_TM = 5
                Poké_Money = 6
                NA = 7
                Evolution_Misc = 8
                Orb = 9
                LinkBox = &HA
                UsedTM = &HB
                Box1 = &HC
                Box2 = &HD
                Box3 = &HE
                SpecificItems = &HF
            End Enum
        End Class

        Public Sub New()

        End Sub

        Public Overrides Async Function OpenFile(Filename As String, Provider As IIOProvider) As Task Implements IOpenableFile.OpenFile
            Await MyBase.OpenFile(Filename, Provider)
            ProcessRawData()
        End Function

        Private Sub ProcessRawData()
            Items = New List(Of Item)

            For count As Integer = 0 To ContentHeader.Length - 1 Step 16
                Items.Add(New Item(ContentHeader.Skip(count).Take(16).ToArray))
            Next
        End Sub

        Public Overrides Async Function Save(Destination As String, provider As IIOProvider) As Task
            'Let the Sir0 class automatically handle the relative pointers in the SIR0 header
            AutoAddSir0HeaderRelativePointers = True
            AutoAddSir0SubHeaderRelativePointers = True

            'There are no pointers in this particular file type
            RelativePointers.Clear()
            SubHeaderRelativePointers.Clear()

            Dim out As New List(Of Byte)

            For Each item In Items
                For count As Integer = 0 To 15
                    out.Add(item.RawData(count))
                Next
            Next

            ContentHeader = out.ToArray

            Await MyBase.Save(Destination, provider)
        End Function
    End Class
End Namespace