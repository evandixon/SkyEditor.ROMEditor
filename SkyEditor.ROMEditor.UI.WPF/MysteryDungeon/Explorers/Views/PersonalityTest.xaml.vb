Imports SkyEditor.SaveEditor

Namespace MysteryDungeon.Explorers.Views
    Public Class PersonalityTest

        Public Sub New()

            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.

        End Sub

        Private Sub ComboBox_KeyUp(sender As Object, e As KeyEventArgs)
            'Typing "Mew" autocompletes to "MewTwo"
            'Typing backspace to get just Mew does nothing
            'So let's help the combobox out
            If e.Key = Key.Back Then
                Dim combobox = DirectCast(sender, ComboBox)
                combobox.SelectedItem = Lists.ExplorersPokemon.Where(Function(kv) kv.Value = combobox.Text).FirstOrDefault()
            End If
        End Sub
    End Class
End Namespace
