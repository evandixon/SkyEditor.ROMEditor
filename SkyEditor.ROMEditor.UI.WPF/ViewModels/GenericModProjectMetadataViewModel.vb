Imports System.ComponentModel
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.ROMEditor.Windows.Projects

Namespace ViewModels
    Public Class GenericModProjectMetadataViewModel
        Inherits GenericViewModel(Of GenericModProject)

        Implements INotifyModified
        Implements INotifyPropertyChanged

        Public Event Modified As INotifyModified.ModifiedEventHandler Implements INotifyModified.Modified
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Public Property ModName As String
            Get
                Return Model.ModName
            End Get
            Set(value As String)
                If Not value = Model.ModName Then
                    value = Model.ModName
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ModName)))
                End If
            End Set
        End Property

        Public Property ModDescription As String
            Get
                Return Model.ModDescription
            End Get
            Set(value As String)
                If Not value = Model.ModDescription Then
                    value = Model.ModDescription
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ModDescription)))
                End If
            End Set
        End Property

        Public Property ModAuthor As String
            Get
                Return Model.ModAuthor
            End Get
            Set(value As String)
                If Not value = Model.ModAuthor Then
                    value = Model.ModAuthor
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ModAuthor)))
                End If
            End Set
        End Property

        Public Property ModVersion As String
            Get
                Return Model.ModVersion
            End Get
            Set(value As String)
                If Not value = Model.ModVersion Then
                    value = Model.ModVersion
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ModVersion)))
                End If
            End Set
        End Property

        Private Sub ModpackInfoViewModel_PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles Me.PropertyChanged
            RaiseEvent Modified(Me, New EventArgs)
        End Sub
    End Class
End Namespace
