Imports System.ComponentModel
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.ROMEditor.Windows.Projects

Namespace ViewModels
    Public Class DSModPackProject3DSBuildOptionsViewModel
        Inherits GenericViewModel(Of DSModPackProject)
        Implements INotifyModified
        Implements INotifyPropertyChanged

        Public Event Modified As INotifyModified.ModifiedEventHandler Implements INotifyModified.Modified
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Public Property Output3DSFile As Boolean
            Get
                Return Model.Output3DSFile
            End Get
            Set(value As Boolean)
                If Not Model.Output3DSFile = value Then
                    Model.Output3DSFile = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Output3DSFile)))
                End If
            End Set
        End Property

        Public Property OutputCIAFile As Boolean
            Get
                Return Model.OutputCIAFile
            End Get
            Set(value As Boolean)
                If Not Model.OutputCIAFile = value Then
                    Model.OutputCIAFile = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(OutputCIAFile)))
                End If
            End Set
        End Property

        Public Property OutputHANS As Boolean
            Get
                Return Model.OutputHans
            End Get
            Set(value As Boolean)
                If Not Model.OutputHans = value Then
                    Model.OutputHans = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(OutputHANS)))
                End If
            End Set
        End Property

        Public Overrides Function SupportsObject(Obj As Object) As Boolean
            Return MyBase.SupportsObject(Obj) AndAlso DirectCast(Obj, DSModPackProject).GetBaseRomSystem(CurrentPluginManager.CurrentIOUIManager.CurrentSolution) = BaseRomProject.System3DS
        End Function

        Private Sub ModpackInfoViewModel_PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles Me.PropertyChanged
            RaiseEvent Modified(Me, New EventArgs)
        End Sub
    End Class

End Namespace
