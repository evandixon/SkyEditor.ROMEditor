Imports SkyEditor.ROMEditor.UI.WPF.MysteryDungeon.PSMD.ViewModels

Public Class FarcView
    Private Sub dgEntries_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles dgEntries.MouseDoubleClick
        DirectCast(ViewModel, FarcViewModel).OpenSelectedFile()
    End Sub
End Class
