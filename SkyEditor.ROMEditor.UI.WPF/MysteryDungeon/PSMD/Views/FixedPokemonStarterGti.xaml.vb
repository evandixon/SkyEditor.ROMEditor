Imports SkyEditor.ROMEditor.UI.WPF.MysteryDungeon.PSMD.ViewModels
Imports SkyEditor.UI.WPF

Namespace MysteryDungeon.PSMD.Views
    Public Class FixedPokemonStarterGti
        Inherits DataBoundViewControl

        Public Overrides Function SupportsObject(Obj As Object) As Boolean
            Return MyBase.SupportsObject(Obj) AndAlso TypeOf Obj Is FixedPokemonStarterViewModel AndAlso TypeOf DirectCast(Obj, FixedPokemonStarterViewModel).StarterEntries.First() Is FixedPokemonEntryGtiViewModel
        End Function
    End Class

End Namespace
