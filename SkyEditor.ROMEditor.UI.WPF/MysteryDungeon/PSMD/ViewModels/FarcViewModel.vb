Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports SkyEditor.Core
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD

Namespace MysteryDungeon.PSMD.ViewModels

    Public Class FarcViewModel
        Inherits GenericViewModel(Of Farc)
        Implements INotifyPropertyChanged

        Public Sub New(appViewModel As ApplicationViewModel)
            If appViewModel Is Nothing Then
                Throw New ArgumentNullException(NameOf(appViewModel))
            End If

            Entries = New ObservableCollection(Of FarcEntryViewModel)
            CurrentApplicationViewModel = appViewModel

            OpenSelectedCommand = New RelayCommand(Sub() OpenSelectedFile())
            DeleteCommand = New RelayCommand(Sub() DeleteSelectedItem())
        End Sub

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Protected Property CurrentApplicationViewModel As ApplicationViewModel

        Public Property Entries As ObservableCollection(Of FarcEntryViewModel)
            Get
                Return _entries
            End Get
            Set(value As ObservableCollection(Of FarcEntryViewModel))
                _entries = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Entries)))
            End Set
        End Property
        Private WithEvents _entries As ObservableCollection(Of FarcEntryViewModel)

        Public Property SelectedEntry As FarcEntryViewModel
            Get
                Return _farcEntryViewModel
            End Get
            Set(value As FarcEntryViewModel)
                _farcEntryViewModel = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SelectedEntry)))
            End Set
        End Property
        Private _farcEntryViewModel As FarcEntryViewModel

        Public ReadOnly Property OpenSelectedCommand As ICommand

        Public ReadOnly Property DeleteCommand As ICommand

        Public Overrides Sub SetModel(model As Object)
            MyBase.SetModel(model)

            Entries.Clear()

            Dim farc As Farc = model
            For Each item In farc.GetEntries.OrderBy(Function(entry) entry.Filename)
                Entries.Add(New FarcEntryViewModel(item))
            Next
        End Sub

        Public Overrides Sub UpdateModel(model As Object)
            MyBase.UpdateModel(model)

            Dim farc As Farc = model

        End Sub

        Public Async Sub OpenSelectedFile()
            If SelectedEntry IsNot Nothing Then
                Dim file = New GenericFile(Model.GetFileData(SelectedEntry.Filename))
                file.Name = SelectedEntry.Filename
                Await CurrentApplicationViewModel.OpenFile(file, True)
            End If
        End Sub

        Private Sub DeleteSelectedItem()
            If SelectedEntry IsNot Nothing Then
                Model.DeleteFile(SelectedEntry.Model.Filename)
                Entries.Remove(SelectedEntry)
                SelectedEntry = Nothing
            End If
        End Sub

        Public Class FarcEntryViewModel
            Implements INotifyPropertyChanged

            Public Sub New(model As Farc.FarcEntry)
                Me.Model = model
            End Sub

            Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

            Public Property Model As Farc.FarcEntry

            Public ReadOnly Property Filename As String
                Get
                    Return Model.Filename
                End Get
            End Property

            Public ReadOnly Property FileSize As String
                Get
                    'Todo: display as 5 KB, 2 MB, 300 Bytes, etc.
                    Return Model.DataLength.ToString()
                End Get
            End Property
        End Class
    End Class

End Namespace
