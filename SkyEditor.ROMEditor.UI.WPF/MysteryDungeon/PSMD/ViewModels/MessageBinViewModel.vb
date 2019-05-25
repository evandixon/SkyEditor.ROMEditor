Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Timers
Imports System.Windows.Forms
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.UI.WPF
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD
Imports System.Text
Imports System.IO
Imports SkyEditor.IO.FileSystem

Namespace MysteryDungeon.PSMD.ViewModels
    Public Class MessageBinViewModel
        Inherits GenericViewModel(Of MessageBin)
        Implements INotifyPropertyChanged
        Implements INotifyModified

        Public Sub New(FileSystem As IFileSystem)
            If FileSystem Is Nothing Then
                Throw New ArgumentNullException(NameOf(FileSystem))
            End If

            searchTimer = New Timers.Timer(500)
            cancelSearch = False
            RawEntries = New ObservableCollection(Of MessageBinEntryViewModel)
            ResetSearchCommand = New RelayCommand(AddressOf ResetSearch)
            CurrentFileSystem = FileSystem
        End Sub

#Region "Events"
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Public Event Modified As EventHandler Implements INotifyModified.Modified
#End Region

#Region "Event Handlers"
        Private Sub OnModified(sender As Object, e As EventArgs)
            RaiseEvent Modified(Me, New EventArgs)
        End Sub
#End Region

        Private WithEvents searchTimer As Timers.Timer
        Private cancelSearch As Boolean
        Private searchTask As Task

        Public Property RawEntries As ObservableCollection(Of MessageBinEntryViewModel)

        Public Property CurrentEntryList As ObservableCollection(Of MessageBinEntryViewModel)
            Get
                Return _currentEntryList
            End Get
            Set(value As ObservableCollection(Of MessageBinEntryViewModel))
                If _currentEntryList IsNot value Then
                    _currentEntryList = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(CurrentEntryList)))
                End If
            End Set
        End Property
        Dim _currentEntryList As ObservableCollection(Of MessageBinEntryViewModel)

        Public Property SearchText As String
            Get
                Return _searchText
            End Get
            Set(value As String)
                If Not _searchText = value Then
                    _searchText = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SearchText)))

                    cancelSearch = True
                    searchTimer.Stop()
                    searchTimer.Start()

                    ResetSearchCommand.IsEnabled = Not String.IsNullOrEmpty(_searchText)
                End If
            End Set
        End Property
        Dim _searchText As String

        Public ReadOnly Property ResetSearchCommand As RelayCommand

        Public ReadOnly Property ExportCommand As RelayCommand

        Protected Property CurrentFileSystem As IFileSystem

        Public Async Function Save(provider As IFileSystem) As Task
            UpdateModel(Model)
            Await Model.Save(provider)

            'Update debug file for project
            Dim debugFilename = Path.ChangeExtension(Model.Filename.Replace("\", "/").Replace("Languages/", "Languages/debug_"), ".dbin")
            If provider.FileExists(debugFilename) Then
                Dim d As New MessageBinDebug
                Await d.OpenFile(debugFilename, provider)
                UpdateDebugSymbols(d)
                Await d.Save(provider)
            End If
        End Function

        Public Sub AddBlankEntry(id As UInteger)
            RawEntries.Add(CreateViewModel(New MessageBinStringEntry With {.Hash = id, .OriginalIndex = RawEntries.Count}))
        End Sub

        Private Sub ResetSearch()
            cancelSearch = True
            _searchText = ""
            CurrentEntryList = RawEntries
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SearchText)))
        End Sub

        ''' <summary>
        ''' Saves the current entries as a CSV file
        ''' </summary>
        ''' <param name="filename">The path of the file to save the resulting CSV</param>
        ''' <param name="entries">The entries to export, or an empty list to export everything</param>
        Public Sub Export(filename As String, entries As IList)
            Dim hashes As New HashSet(Of UInteger)
            For Each item In entries
                If TypeOf item Is MessageBinEntryViewModel Then
                    hashes.Add(DirectCast(item, MessageBinEntryViewModel).Hash)
                End If
            Next

            Dim output As New StringBuilder
            output.AppendLine("Index,Hash,Entry")
            For Each item In CurrentEntryList.
                Where(Function(currentEntry) Not hashes.Any OrElse hashes.Contains(currentEntry.Hash)).
                OrderBy(Function(currentEntry) currentEntry.OriginalIndex)

                output.Append(item.OriginalIndex)
                output.Append(",")
                output.Append(item.HashSigned)
                output.Append(",")
                output.Append(item.Entry)
                output.AppendLine()
            Next
            CurrentFileSystem.WriteAllText(filename, output.ToString)
        End Sub

        ''' <summary>
        ''' Saves the current entries as a CSV file, the path of which determined by an SaveFileDialog
        ''' </summary>
        ''' <param name="entries">The entries to export, or an empty list to export everything</param>
        Public Sub Export(entries As IList)
            Dim s As New SaveFileDialog
            s.Filter = CurrentApplicationViewModel.GetIOFilter({"*.csv"}, False, True, False)
            If s.ShowDialog = DialogResult.OK Then
                Export(s.FileName, entries)
            End If
        End Sub

        Public Sub LoadDebugSymbols(debug As MessageBinDebug)
            For Each item In debug.Strings
                Dim target = RawEntries.FirstOrDefault(Function(e) e.Hash = item.Hash)
                If target IsNot Nothing Then
                    target.DebugSymbol = item.Entry
                End If
            Next
        End Sub

        Public Sub UpdateDebugSymbols(debug As MessageBinDebug)
            For Each item In RawEntries
                Dim target = debug.Strings.FirstOrDefault(Function(x) x.Hash = item.Hash)
                If target IsNot Nothing Then
                    target.Entry = item.DebugSymbol
                Else
                    debug.Strings.Add(New MessageBinStringEntry With {.Hash = item.Hash, .Entry = item.DebugSymbol})
                End If
            Next
        End Sub

#Region "Set/Load ViewModel"
        Private Function CreateViewModel(model As MessageBinStringEntry) As MessageBinEntryViewModel
            Dim vm As New MessageBinEntryViewModel
            vm.SetApplicationViewModel(Me.CurrentApplicationViewModel)
            vm.Model = model
            AddHandler vm.PropertyChanged, AddressOf OnModified
            Return vm
        End Function

        Private Sub ClearEntries()
            For Each item In RawEntries
                RemoveHandler item.PropertyChanged, AddressOf OnModified
            Next
            RawEntries.Clear()
        End Sub

        Public Overrides Sub SetModel(model As Object)
            MyBase.SetModel(model)

            RawEntries.Clear()
            For Each item In Me.Model.Strings
                RawEntries.Add(CreateViewModel(item))
            Next

            CurrentEntryList = RawEntries
        End Sub

        Public Overrides Sub UpdateModel(model As Object)
            MyBase.UpdateModel(model)

            Me.Model.Strings.Clear()
            For Each item In Me.RawEntries
                Me.Model.Strings.Add(item.Model)
            Next
        End Sub
#End Region

#Region "Search Functions"
        Public Class ItemIndexComparer
            Implements IComparer(Of MessageBinEntryViewModel)

            Public Sub New(sourceList As List(Of Integer))
                Me.SourceList = sourceList
            End Sub

            Private Property SourceList As List(Of Integer)

            Public Function Compare(x As MessageBinEntryViewModel, y As MessageBinEntryViewModel) As Integer Implements IComparer(Of MessageBinEntryViewModel).Compare
                Return SourceList.IndexOf(x.HashSigned).CompareTo(SourceList.IndexOf(y.HashSigned))
            End Function
        End Class

        Public Sub Sort(Keys As List(Of Integer))
            Task.Run(New Action(Sub()
                                    DoSort(Keys)
                                End Sub))
        End Sub

        Private Sub DoSort(Keys As List(Of Integer))
            Dim comparer = New ItemIndexComparer(Keys)
            Dim items = CurrentEntryList.ToList()
            items.Sort(comparer)
            Application.Current.Dispatcher.Invoke(Sub() CurrentEntryList = New ObservableCollection(Of MessageBinEntryViewModel)(items))
        End Sub

        Private Async Sub searchTimer_Elapsed(sender As Object, e As ElapsedEventArgs) Handles searchTimer.Elapsed
            searchTimer.Stop()
            'Wait for the current task to stop itself
            If searchTask IsNot Nothing Then
                Await searchTask
            End If
            'Start a new async task
            searchTask = Task.Run(Sub() RunSearch(SearchText))
        End Sub


        Private Sub RunSearch(SearchText As String)
            cancelSearch = False
            If String.IsNullOrEmpty(SearchText) Then
                Application.Current.Dispatcher.Invoke(Sub() CurrentEntryList = RawEntries)
            Else
                Dim results As New ObservableCollection(Of MessageBinEntryViewModel)
                Application.Current.Dispatcher.Invoke(Sub() CurrentEntryList = results)

                Dim searchTerms = SearchText.Split(" ")

                For Each item In RawEntries
                    If cancelSearch = True Then
                        'If we get here, the search textbox has been changed, so we'll stop searching
                        Exit For
                    End If

                    Dim isMatch As Boolean
                    For Each term In searchTerms
                        isMatch = False 'For every term, we'll set isMatch to false

                        'The entry must match every term
                        If item.Hash.ToString.Contains(term) Then
                            isMatch = True
                        ElseIf item.HashSigned.ToString.Contains(term) Then
                            isMatch = True
                        ElseIf item.Entry.ToString.ToLower.Contains(term.ToLower) Then
                            isMatch = True
                        End If

                        'If any terms aren't a match, then we don't use this entry
                        If Not isMatch Then
                            Exit For
                        End If
                    Next

                    If isMatch Then
                        Application.Current.Dispatcher.Invoke(Sub() results.Add(item))
                    End If
                Next
            End If
        End Sub
#End Region

        Public Class MessageBinEntryViewModel
            Inherits GenericViewModel(Of MessageBinStringEntry)
            Implements INotifyPropertyChanged

            Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

            Public Property Hash As UInteger
                Get
                    Return Model.Hash
                End Get
                Set(value As UInteger)
                    If Not Model.Hash = value Then
                        Model.Hash = value
                        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Hash)))
                    End If
                End Set
            End Property

            Public Property HashSigned As Integer
                Get
                    Return Model.HashSigned
                End Get
                Set(value As Integer)
                    If Not Model.HashSigned = value Then
                        Model.HashSigned = value
                        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HashSigned)))
                    End If
                End Set
            End Property

            Public Property Entry As String
                Get
                    Return Model.Entry
                End Get
                Set(value As String)
                    If Not Model.Entry = value Then
                        Model.Entry = value
                        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Entry)))
                    End If
                End Set
            End Property

            Public Property DebugSymbol As String
                Get
                    Return _debugSymbol
                End Get
                Set(value As String)
                    If Not _debugSymbol = value Then
                        _debugSymbol = value
                        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(DebugSymbol)))
                    End If
                End Set
            End Property
            Dim _debugSymbol As String

            Public ReadOnly Property OriginalIndex As Integer
                Get
                    Return Model.OriginalIndex
                End Get
            End Property
        End Class

    End Class
End Namespace

