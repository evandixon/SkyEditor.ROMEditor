Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Timers
Imports System.Windows.Controls
Imports System.Windows.Forms
Imports SkyEditor.ROMEditor.Windows.MysteryDungeon.PSMD
Imports SkyEditor.UI.WPF

Namespace PSMD.Views
    Public Class MessageBinEditor
        Inherits DataBoundObjectControl

        Private Sub OnMsgItemAdded(sender As Object, e As MessageBin.EntryAddedEventArgs)
            Dim addedEntry = (From i As MessageBinStringEntry In lstEntries.ItemsSource Where i.Hash = e.NewID).FirstOrDefault
            If addedEntry IsNot Nothing Then
                lstEntries.SelectedIndex = lstEntries.Items.IndexOf(addedEntry)
                lstEntries.ScrollIntoView(addedEntry)
            End If
        End Sub

    End Class
End Namespace