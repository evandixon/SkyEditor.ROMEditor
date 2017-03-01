Imports System.ComponentModel
Imports System.Drawing
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.ROMEditor.MysteryDungeon.Explorers

Namespace MysteryDungeon.Explorers.ViewModels
    Public Class BGPViewModel
        Inherits GenericViewModel(Of BGP)
        Implements INotifyModified
        Implements INotifyPropertyChanged

        Public Event Modified As EventHandler Implements INotifyModified.Modified
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Public Property ContainedImage As Bitmap
            Get
                Return Model.GetImage
            End Get
            Set(value As Bitmap)
                Me.Model = BGP.ConvertFromBitmap(value)
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ContainedImage)))
            End Set
        End Property
    End Class
End Namespace

