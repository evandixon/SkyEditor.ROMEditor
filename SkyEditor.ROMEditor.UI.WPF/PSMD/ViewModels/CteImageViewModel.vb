Imports System.ComponentModel
Imports System.Drawing
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.ROMEditor.Windows.FileFormats.PSMD

Namespace PSMD.ViewModels
    Public Class CteImageViewModel
        Inherits GenericViewModel(Of CteImage)
        Implements INotifyModified
        Implements INotifyPropertyChanged

        Public Event Modified As INotifyModified.ModifiedEventHandler Implements INotifyModified.Modified
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Public Property ContainedImage As Bitmap
            Get
                Return Model.ContainedImage
            End Get
            Set(value As Bitmap)
                If Model.ContainedImage IsNot value Then
                    Model.ContainedImage = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ContainedImage)))
                    RaiseEvent Modified(Me, New EventArgs)
                End If
            End Set
        End Property
    End Class
End Namespace

