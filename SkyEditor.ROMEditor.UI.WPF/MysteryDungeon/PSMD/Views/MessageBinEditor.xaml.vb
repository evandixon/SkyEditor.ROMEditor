Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD
Imports SkyEditor.ROMEditor.UI.WPF.MysteryDungeon.PSMD.ViewModels
Imports SkyEditor.UI.WPF

Namespace MysteryDungeon.PSMD.Views
    Public Class MessageBinEditor
        Inherits DataBoundViewControl

        Private Sub OnMsgItemAdded(sender As Object, e As MessageBin.EntryAddedEventArgs)
            Dim addedEntry = (From i As MessageBinStringEntry In lstEntries.ItemsSource Where i.Hash = e.NewID).FirstOrDefault
            If addedEntry IsNot Nothing Then
                lstEntries.SelectedIndex = lstEntries.Items.IndexOf(addedEntry)
                lstEntries.ScrollIntoView(addedEntry)
            End If
        End Sub

        Private Sub menuExport_Click(sender As Object, e As RoutedEventArgs) Handles menuExport.Click
            DirectCast(ViewModel, MessageBinViewModel).Export(lstEntries.SelectedItems)
        End Sub
    End Class
End Namespace