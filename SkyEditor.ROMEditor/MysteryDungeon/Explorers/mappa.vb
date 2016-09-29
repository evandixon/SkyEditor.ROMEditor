Imports SkyEditor.Core.IO
Imports SkyEditor.ROMEditor.MysteryDungeon

Namespace MysteryDungeon.Explorers
    Public Class mappa
        Inherits Sir0

        Public Overrides Async Function OpenFile(Filename As String, Provider As IOProvider) As Task
            Await MyBase.OpenFile(Filename, Provider)

            Dim ptr1 As Integer = BitConverter.ToInt32(Header, 0) 'Block of pointers, ends at pointer 2, appears to point to Floor Index
            Dim ptr2 As Integer = BitConverter.ToInt32(Header, 4) 'Block of data, Floor Attribute Data, 32 byte entries
            Dim ptr3 As Integer = BitConverter.ToInt32(Header, 8) 'Block of pointers

            'Pokemon Spawns
            Dim ptr4 As Integer = BitConverter.ToInt32(Header, &HC) 'Block of pointers
            'Each pointer points to a dungeon's Pokemon entries, 8 byte entries, last entry in each group is empty

            'Item Spawns
            Dim ptr5 As Integer = BitConverter.ToInt32(Header, &H10)
            'Each pointer points to a 50 byte (0x32) entry
        End Function
    End Class
End Namespace

