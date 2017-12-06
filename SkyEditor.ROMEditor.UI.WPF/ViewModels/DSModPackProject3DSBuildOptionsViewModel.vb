Imports System.ComponentModel
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.ROMEditor.Projects

Namespace ViewModels
    Public Class DSModPackProject3DSBuildOptionsViewModel
        Inherits GenericViewModel(Of DSModPackProject)
        Implements INotifyModified
        Implements INotifyPropertyChanged

        Public Event Modified As EventHandler Implements INotifyModified.Modified
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Public Property OutputEnc3DSFile As Boolean
            Get
                Return Model.OutputEnc3DSFile
            End Get
            Set(value As Boolean)
                If Not Model.OutputEnc3DSFile = value Then
                    Model.OutputEnc3DSFile = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(OutputEnc3DSFile)))
                End If
            End Set
        End Property

        Public Property OutputDec3DSFile As Boolean
            Get
                Return Model.OutputDec3DSFile
            End Get
            Set(value As Boolean)
                If Not Model.OutputDec3DSFile = value Then
                    Model.OutputDec3DSFile = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(OutputDec3DSFile)))
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

        Public Property OutputLuma As Boolean
            Get
                Return Model.OutputLuma
            End Get
            Set(value As Boolean)
                If Not Model.OutputLuma = value Then
                    Model.OutputLuma = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(OutputLuma)))
                End If
            End Set
        End Property

        Public Overrides Function SupportsObject(Obj As Object) As Boolean
            Return MyBase.SupportsObject(Obj) AndAlso DirectCast(Obj, DSModPackProject).GetBaseRomSystem(CurrentApplicationViewModel.CurrentSolution) = BaseRomProject.System3DS
        End Function

        Private Sub ModpackInfoViewModel_PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles Me.PropertyChanged
            RaiseEvent Modified(Me, New EventArgs)
        End Sub
    End Class

End Namespace
