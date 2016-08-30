Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.ServiceModel
Imports System.Timers
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.ROMEditor.Windows.MysteryDungeon.PSMD

Namespace PSMD.ViewModels
    Public Class MessageBinViewModel
        Inherits GenericViewModel(Of MessageBin)
        Implements INotifyPropertyChanged
        Implements INotifyModified

        Public Class MessageBinEntryViewModel
            Inherits GenericViewModel(Of MessageBinStringEntry)
            Implements INotifyPropertyChanged

            Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

            Public Property Hash As Integer
                Get
                    Return Model.Hash
                End Get
                Set(value As Integer)
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
        End Class

        Public Sub New()
            searchTimer = New Timers.Timer(500)
            cancelSearch = False
        End Sub


        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Public Event Modified As INotifyModified.ModifiedEventHandler Implements INotifyModified.Modified

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
                End If
            End Set
        End Property
        Dim _searchText As String

        Public Sub Save(provider As IOProvider)
            Model.Save(provider)
        End Sub

        Public Sub AddBlankEntry(id As UInteger)
            Model.AddBlankEntry(id)
        End Sub

#Region "Search Functions"
        Public Sub Sort(Keys As List(Of Integer))
            Task.Run(New Action(Sub()
                                    DoSort(Keys)
                                End Sub))
        End Sub

        Private Sub DoSort(Keys As List(Of Integer))
            Dim results As New ObservableCollection(Of MessageBinEntryViewModel)
            'Todo: use dispatcher
            CurrentEntryList = results
            For Each item In Keys
                Dim entry = (From s In RawEntries Where s.HashSigned = item).FirstOrDefault
                If entry IsNot Nothing Then
                    'Todo: use dispatcher
                    results.Add(entry)
                End If
            Next
        End Sub

        Private Async Sub searchTimer_Elapsed(sender As Object, e As ElapsedEventArgs) Handles searchTimer.Elapsed
            searchTimer.Stop()
            Dim searchText As String = ""
            'Todo: use dispatcher
            'Wait for the current task to stop itself
            If searchTask IsNot Nothing Then
                Await searchTask
            End If
            'Start a new async task
            searchTask = Task.Run(Sub() RunSearch(searchText))
        End Sub


        Private Sub RunSearch(SearchText As String)
            cancelSearch = False
            If String.IsNullOrEmpty(SearchText) Then
                'Todo: use dispatcher
                CurrentEntryList = RawEntries
            Else
                Dim results As New ObservableCollection(Of MessageBinEntryViewModel)
                'Todo: use dispatcher
                CurrentEntryList = results

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
                        'Todo: use dispatcher
                        results.Add(item)
                    End If
                Next
            End If
        End Sub
#End Region

    End Class
End Namespace

