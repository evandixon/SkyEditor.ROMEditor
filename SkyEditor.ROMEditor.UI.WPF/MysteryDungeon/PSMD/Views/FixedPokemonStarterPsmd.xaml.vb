Imports SkyEditor.Core.UI
Imports SkyEditor.ROMEditor.UI.WPF.MysteryDungeon.PSMD.ViewModels
Imports SkyEditor.UI.WPF

Namespace MysteryDungeon.PSMD.Views
    Public Class FixedPokemonStarterPsmd
        Inherits DataBoundViewControl
        Implements IViewControl

        Public Overrides Function SupportsObject(Obj As Object) As Boolean Implements IViewControl.SupportsObject
            Return MyBase.SupportsObject(Obj) AndAlso
                TypeOf Obj Is FixedPokemonStarterViewModel AndAlso
                TypeOf DirectCast(Obj, FixedPokemonStarterViewModel).StarterEntries.First() Is FixedPokemonEntryPsmdViewModel
        End Function
    End Class

End Namespace

