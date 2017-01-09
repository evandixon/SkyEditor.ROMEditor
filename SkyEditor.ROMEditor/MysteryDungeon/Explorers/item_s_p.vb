Imports SkyEditor.Core.IO

Namespace MysteryDungeon.Explorers
    Public Class item_s_p
        Inherits ExplorersSir0

        Public Structure Item
            ''' <summary>
            ''' Rarity of the item (ex. *, **, ***).  Also controls whether the item affects a Pokemon or a Type.
            ''' </summary>
            ''' <returns></returns>
            Public Property Rarity As ItemRarity
            ''' <summary>
            ''' If the rarity is 1-4, the ID of the Type the item affects.
            ''' If the rarity is 5-10, the ID of the Pokemon the item affects.
            ''' </summary>
            ''' <returns></returns>
            Public Property TargetID As UInt16
            Public Enum ItemRarity As UInt16
                None = 0
                OneStar1_Type = 1 'Dungeon with a key
                OneStar2_Type = 2 'Job payment
                TwoStar_Type = 3 'Trade the 1 star items for the same type
                ThreeStar_Type = 4 'Trade the 1 star items and 2 star item for the same type

                OneStar1_Pokémon = 5 'Box appraisal
                OneStar2_Pokémon = 6 'Box appraisal
                TwoStar_Pokémon = 7 'Trade the 1 star items for the same Pokemon.
                ThreeStar_Pokémon = 8 'Trade the 1 star items and the two star item for the same Pokemon.
                ThreeStar_HatchItem = 9 'The Pokemon may hatch holding the item
                ThreeStar_Evolution_Pokemon = 10 'Trade the 1 star items and the two star item for the previous Pokemon.
            End Enum
            Public Sub New(Rarity As ItemRarity, TargetID As UInt16)
                Me.Rarity = Rarity
                Me.TargetID = TargetID
            End Sub
        End Structure

        Public Property Items As List(Of Item)

        Public Overrides Async Function OpenFile(Filename As String, Provider As IOProvider) As Task
            Await MyBase.OpenFile(Filename, Provider)

            Items = New List(Of Item)

            For count = 0 To ContentHeader.Length - 1 Step 4
                Dim item As New Item
                item.Rarity = BitConverter.ToUInt16(ContentHeader, count + 0)
                item.TargetID = BitConverter.ToUInt16(ContentHeader, count + 2)
                Items.Add(item)
            Next
        End Function

        Public Overrides Async Function Save(Destination As String, provider As IOProvider) As Task
            Dim out As New List(Of Byte)

            For Each item In Items
                out.AddRange(BitConverter.GetBytes(item.Rarity))
                out.AddRange(BitConverter.GetBytes(item.TargetID))
            Next

            Me.ContentHeader = out.ToArray

            Await MyBase.Save(Destination, provider)
        End Function
    End Class
End Namespace
