Imports System.ComponentModel
Imports DS_ROM_Patcher
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
                If Not Model.Name = value Then
                    Model.Name = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Name)))
                End If
            End Set
        End Property

        Public Property ShortName As String
            Get
                Return Model.ShortName
            End Get
            Set(value As String)
                If Not Model.ShortName = value Then
                    Model.ShortName = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ShortName)))
                End If
            End Set
        End Property

        Public Property Author As String
            Get
                Return Model.Author
            End Get
            Set(value As String)
                If Not Model.Author = value Then
                    Model.Author = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Author)))
                End If
            End Set
        End Property

        Public Property Version As String
            Get
                Return Model.Version
            End Get
            Set(value As String)
                If Not Model.Version = value Then
                    Model.Version = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Version)))
                End If
            End Set
        End Property

        Public Property System As String
            Get
                Return Model.System
            End Get
            Set(value As String)
                If Not Model.System = value Then
                    Model.System = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(System)))
                End If
            End Set
        End Property

        Public Property GameCode As String
            Get
                Return Model.GameCode
            End Get
            Set(value As String)
                If Not Model.GameCode = value Then
                    Model.GameCode = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(GameCode)))
                End If
            End Set
        End Property

        Private Sub ModpackInfoViewModel_PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles Me.PropertyChanged
            RaiseEvent Modified(Me, New EventArgs)
        End Sub
    End Class
End Namespace
