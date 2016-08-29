Imports System.ComponentModel
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.ROMEditor.Windows

Namespace ViewModels
    Public Class ModpackInfoViewModel
        Inherits GenericViewModel(Of ModpackInfo)
        Implements INotifyModified
        Implements INotifyPropertyChanged

        Public Event Modified As INotifyModified.ModifiedEventHandler Implements INotifyModified.Modified
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Public Property Name As String
            Get
                Return Model.Name
            End Get
            Set(value As String)
                If Not value = Model.Name Then
                    value = Model.Name
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Name)))
                End If
            End Set
        End Property

        Public Property ShortName As String
            Get
                Return Model.ShortName
            End Get
            Set(value As String)
                If Not value = Model.ShortName Then
                    value = Model.ShortName
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ShortName)))
                End If
            End Set
        End Property

        Public Property Author As String
            Get
                Return Model.Author
            End Get
            Set(value As String)
                If Not value = Model.Author Then
                    value = Model.Author
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Author)))
                End If
            End Set
        End Property

        Public Property Version As String
            Get
                Return Model.Version
            End Get
            Set(value As String)
                If Not value = Model.Version Then
                    value = Model.Version
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Version)))
                End If
            End Set
        End Property

        Public Property System As String
            Get
                Return Model.System
            End Get
            Set(value As String)
                If Not value = Model.System Then
                    value = Model.System
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(System)))
                End If
            End Set
        End Property

        Public Property GameCode As String
            Get
                Return Model.GameCode
            End Get
            Set(value As String)
                If Not value = Model.GameCode Then
                    value = Model.GameCode
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(GameCode)))
                End If
            End Set
        End Property

        Private Sub ModpackInfoViewModel_PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles Me.PropertyChanged
            RaiseEvent Modified(Me, New EventArgs)
        End Sub
    End Class
End Namespace
