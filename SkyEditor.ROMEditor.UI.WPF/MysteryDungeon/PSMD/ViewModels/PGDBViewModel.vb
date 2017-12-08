Imports System.Collections.ObjectModel
Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Timers
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD

Namespace MysteryDungeon.PSMD.ViewModels
    Public Class PGDBViewModel
        Inherits GenericViewModel(Of PGDB)
        Implements INotifyPropertyChanged
        Implements INotifyModified

        Public Class EntryViewModel
            Implements INotifyPropertyChanged

            Public Sub New(model As PGDB.Entry)
                Me.Model = model
            End Sub

            Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

            Public Property Model As PGDB.Entry

            Public Property String1 As String
                Get
                    Return Model.String1
                End Get
                Set(value As String)
                    Model.String1 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(String1)))
                End Set
            End Property

            Public Property String2 As String
                Get
                    Return Model.String2
                End Get
                Set(value As String)
                    Model.String2 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(String2)))
                End Set
            End Property

            Public Property String3 As String
                Get
                    Return Model.String3
                End Get
                Set(value As String)
                    Model.String3 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(String3)))
                End Set
            End Property

            Public Function MatchesSearchCriteria(searchTerm As String) As Boolean
                Return Model.ToString().ToLower().Contains(searchTerm.ToLower())
            End Function

            Public Overrides Function ToString() As String
                Return Model.ToString()
            End Function

        End Class

        Public Sub New()
            SearchTimer = New Timer(500)
        End Sub

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Public Event Modified As EventHandler Implements INotifyModified.Modified

        Private WithEvents SearchTimer As Timer

        ''' <summary>
        ''' The currently-selected entry
        ''' </summary>
        Public Property SelectedEntry As EntryViewModel
            Get
                Return _selectedEntry
            End Get
            Set(value As EntryViewModel)
                _selectedEntry = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SelectedEntry)))
            End Set
        End Property
        Dim _selectedEntry As EntryViewModel

        ''' <summary>
        ''' The entries in the file
        ''' </summary>
        Public Property Entries As ObservableCollection(Of EntryViewModel)
            Get
                Return _entries
            End Get
            Set(value As ObservableCollection(Of EntryViewModel))
                _entries = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Entries)))
            End Set
        End Property
        Private WithEvents _entries As ObservableCollection(Of EntryViewModel)

        ''' <summary>
        ''' The current search term, responsible for filtering <see cref="CurrentResultSet"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property SearchTerm As String
            Get
                Return _searchTerm
            End Get
            Set(value As String)
                _searchTerm = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SearchTerm)))
                SearchTimer.Stop()
                SearchTimer.Start()
            End Set
        End Property
        Dim _searchTerm As String

        ''' <summary>
        ''' The current set of entries to be viewed, after search filters if applicable
        ''' </summary>
        Public Property CurrentResultSet As ObservableCollection(Of EntryViewModel)
            Get
                If _currentResultSet Is Nothing Then
                    Return Entries
                Else
                    Return _currentResultSet
                End If
            End Get
            Set(value As ObservableCollection(Of EntryViewModel))
                _currentResultSet = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(CurrentResultSet)))
            End Set
        End Property
        Dim _currentResultSet As ObservableCollection(Of EntryViewModel)

        Public ReadOnly Property ResetSearchCommand As ICommand = New RelayCommand(AddressOf ResetSearch)

        Public ReadOnly Property RemoveSelectedEntryCommand As ICommand = New RelayCommand(AddressOf RemoveSelectedEntry)

        Public Sub ResetSearch()
            SearchTerm = ""
            CurrentResultSet = Nothing
        End Sub

        Public Sub RemoveSelectedEntry()
            Entries.Remove(SelectedEntry)
            CurrentResultSet.Remove(SelectedEntry)
        End Sub

        Private Sub SearchTimer_Tick(sender As Object, e As EventArgs) Handles SearchTimer.Elapsed
            SearchTimer.Stop()

            Dim term = SearchTerm 'Avoid race conditions
            CurrentResultSet = New ObservableCollection(Of EntryViewModel)(
                Entries.Where(Function(x) x.MatchesSearchCriteria(term))
            )
        End Sub

        Private Sub _entries_CollectionChanged(sender As Object, e As NotifyCollectionChangedEventArgs) Handles _entries.CollectionChanged
            If e.Action = NotifyCollectionChangedAction.Remove Then
                For Each item As EntryViewModel In e.OldItems
                    RemoveHandler item.PropertyChanged, AddressOf OnEntryPropertyChanged
                Next
            End If
            RaiseEvent Modified(Me, New EventArgs())
        End Sub

        Private Sub OnEntryPropertyChanged(sender As Object, e As EventArgs)
            RaiseEvent Modified(Me, New EventArgs())
        End Sub

        Public Overrides Sub SetModel(model As Object)
            MyBase.SetModel(model)

            Dim m As PGDB = model

            Entries = New ObservableCollection(Of EntryViewModel)
            For Each item In m.Entries
                Dim vm = New EntryViewModel(item)
                AddHandler vm.PropertyChanged, AddressOf OnEntryPropertyChanged
                Entries.Add(vm)
            Next
        End Sub

        Public Overrides Sub UpdateModel(model As Object)
            Dim m As PGDB = model
            m.Entries = Entries.Select(Function(x) x.Model).ToList()

            MyBase.UpdateModel(model)
        End Sub

    End Class
End Namespace
