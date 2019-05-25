Imports SkyEditor.Core.IO
Imports SkyEditor.IO.FileSystem

Namespace MysteryDungeon.Explorers
    Public Class TableDatItemList
        Inherits GenericFile

        Public Class TableDatItem

            Public Sub New(itemID As UInt16, obtainPercentage As Single)
                Me.ItemID = itemID
                Me.ObtainPercentage = obtainPercentage
            End Sub

            Public Property ObtainPercentage As Single

            Public Property ItemID As UInt16

            Public Overrides Function ToString() As String
                Return $"Item {ItemID} ({SaveEditor.Lists.SkyItems(ItemID)}), {ObtainPercentage * 100}%"
            End Function

        End Class

        Public Property Items As List(Of TableDatItem)

        Private Async Function InitItems() As Task
            Items = New List(Of TableDatItem)
            If Length >= 2 Then
                Dim itemCount = BitConverter.ToUInt16(Await ReadAsync(0, 2), 0)

                For count As Integer = 2 To (itemCount - 1) * 4 Step 4
                    Dim itemId = BitConverter.ToUInt16(Await ReadAsync(count + 2, 2), 0)

                    Dim percentage As Single
                    If count = 2 Then
                        percentage = BitConverter.ToUInt16(Await ReadAsync(count, 2), 0) / 1024
                    Else
                        percentage = (BitConverter.ToUInt16(Await ReadAsync(count, 2), 0) - BitConverter.ToUInt16(Await ReadAsync(count - 4, 2), 0)) / 1024
                    End If

                    Items.Add(New TableDatItem(itemId, percentage))
                Next
            End If
        End Function

        Public Overrides Async Function OpenFile(filename As String, provider As IFileSystem) As Task
            Await MyBase.OpenFile(filename, provider)
            Await InitItems()
        End Function

        Public Overrides Async Function Save(Destination As String, provider As IFileSystem) As Task
            SetLength(2 + (Items.Count * 4))
            Await WriteAsync(0, 2, BitConverter.GetBytes(Items.Count))
            For count As Integer = 0 To Items.Count - 1
                If count = 0 Then
                    Await WriteAsync(count * 4, 2, BitConverter.GetBytes(CInt(Items(count).ObtainPercentage * 1024)))
                Else
                    Await WriteAsync(count * 4, 2, BitConverter.GetBytes(CInt((Items(count).ObtainPercentage + Items(count + 1).ObtainPercentage) * 1024)))
                End If
                Await WriteAsync(count * 4 + 2, 2, BitConverter.GetBytes(Items(count).ItemID))
            Next
            Await MyBase.Save(Destination, provider)
        End Function
    End Class

End Namespace
