Imports SkyEditor.Core.IO
Imports SkyEditor.ROMEditor.MysteryDungeon

Namespace MysteryDungeon.Explorers
    Public Class mappa
        Inherits Sir0

        Public Overrides Async Function OpenFile(Filename As String, Provider As IOProvider) As Task
            Await MyBase.OpenFile(Filename, Provider)

            Dim ptr1 As Integer = BitConverter.ToInt32(Header, 0) 'Block of pointers, ends at pointer 2
            Dim ptr2 As Integer = BitConverter.ToInt32(Header, 4) 'Block of data
            Dim ptr3 As Integer = BitConverter.ToInt32(Header, 8) 'Block of pointers
            Dim ptr4 As Integer = BitConverter.ToInt32(Header, &HC) 'Block of pointers
            Dim ptr5 As Integer = BitConverter.ToInt32(Header, &H10)
        End Function
    End Class
End Namespace

