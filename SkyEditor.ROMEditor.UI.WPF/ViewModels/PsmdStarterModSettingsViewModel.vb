Imports System.ComponentModel
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD.Projects

Namespace ViewModels
    Public Class PsmdStarterModSettingsViewModel
        Inherits GenericViewModel(Of PsmdStarterMod)

        Implements INotifyModified
        Implements INotifyPropertyChanged

        Public Event Modified As EventHandler Implements INotifyModified.Modified
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Public Property EnableModelPatching As Boolean
            Get
                Return Model.EnableModelPatching
            End Get
            Set(value As Boolean)
                If Not Model.EnableModelPatching = value Then
                    Model.EnableModelPatching = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(EnableModelPatching)))
                End If
            End Set
        End Property

        Public Property EnablePortraitPatching As Boolean
            Get
                Return Model.EnablePortraitPatching
            End Get
            Set(value As Boolean)
                If Not Model.EnablePortraitPatching = value Then
                    Model.EnablePortraitPatching = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(EnablePortraitPatching)))
                End If
            End Set
        End Property

        Private Sub ModpackInfoViewModel_PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles Me.PropertyChanged
            RaiseEvent Modified(Me, New EventArgs)
        End Sub
    End Class
End Namespace
