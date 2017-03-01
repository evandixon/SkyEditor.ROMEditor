Imports System.ComponentModel
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD.Dungeon

Namespace MysteryDungeon.PSMD.ViewModels
    Public Class FixedPokemonStarterViewModel
        Inherits GenericViewModel(Of FixedPokemon)
        Implements INotifyModified
        Implements INotifyPropertyChanged

        Public Event Modified As EventHandler Implements INotifyModified.Modified
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Public Property StarterEntries As List(Of FixedPokemonEntryViewModel)
        Public Property EvolutionEntries As List(Of FixedPokemonEntryViewModel)

        Public Property SelectedStarterEntry As FixedPokemonEntryViewModel
            Get
                Return _selectedStarterEntry
            End Get
            Set(value As FixedPokemonEntryViewModel)
                'Update current property
                _selectedStarterEntry = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SelectedStarterEntry)))

                'Update sister property
                Dim currentIndex = StarterEntries.IndexOf(value)
                _selectedEvolutionEntry = EvolutionEntries(currentIndex)
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SelectedEvolutionEntry)))
            End Set
        End Property
        Dim _selectedStarterEntry As FixedPokemonEntryViewModel

        Public ReadOnly Property SelectedEvolutionEntry As FixedPokemonEntryViewModel
            Get
                Return _selectedEvolutionEntry
            End Get
        End Property
        Dim _selectedEvolutionEntry As FixedPokemonEntryViewModel

        Public Overrides Async Sub SetModel(model As Object)
            MyBase.SetModel(model)

            Dim f As FixedPokemon = model

            ClearList(StarterEntries)
            StarterEntries.Add(Await CreateFileViewModel(f.Entries(19))) 'FUSHIGIDANE_H (Bulbasaur))
            StarterEntries.Add(Await CreateFileViewModel(f.Entries(21))) 'HITOKAGE_H (Charmander)
            StarterEntries.Add(Await CreateFileViewModel(f.Entries(23))) 'ZENIGAME_H (Squirtle)
            StarterEntries.Add(Await CreateFileViewModel(f.Entries(17))) 'PIKACHUU_H (Pikachu)
            StarterEntries.Add(Await CreateFileViewModel(f.Entries(25))) 'CHIKORIITA_H (Chikorita)
            StarterEntries.Add(Await CreateFileViewModel(f.Entries(27))) 'HINOARASHI_H (Cyndaquil)
            StarterEntries.Add(Await CreateFileViewModel(f.Entries(29))) 'WANINOKO_H (Totodile)
            StarterEntries.Add(Await CreateFileViewModel(f.Entries(31))) 'KIMORI_H (Treecko)
            StarterEntries.Add(Await CreateFileViewModel(f.Entries(33))) 'ACHAMO_H (Torchic)
            StarterEntries.Add(Await CreateFileViewModel(f.Entries(35))) 'MIZUGOROU_H (Mudkip)
            StarterEntries.Add(Await CreateFileViewModel(f.Entries(37))) 'NAETORU_H (Turtwig)
            StarterEntries.Add(Await CreateFileViewModel(f.Entries(39))) 'HIKOZARU_H (Chimchar)
            StarterEntries.Add(Await CreateFileViewModel(f.Entries(41))) 'POTCHAMA_H (Piplup)
            StarterEntries.Add(Await CreateFileViewModel(f.Entries(18))) 'RIORU_H (Riolu)
            StarterEntries.Add(Await CreateFileViewModel(f.Entries(43))) 'TSUTAAJA_H (Snivy)
            StarterEntries.Add(Await CreateFileViewModel(f.Entries(45))) 'POKABU_H (Tepig)
            StarterEntries.Add(Await CreateFileViewModel(f.Entries(47))) 'MIJUMARU_H (Oshawott)
            StarterEntries.Add(Await CreateFileViewModel(f.Entries(49))) 'HARIMARON_H (Chespin)
            StarterEntries.Add(Await CreateFileViewModel(f.Entries(51))) 'FOKKO_H (Fennekin)
            StarterEntries.Add(Await CreateFileViewModel(f.Entries(53))) 'KEROMATSU_H (Froakie)

            ClearList(EvolutionEntries)
            EvolutionEntries.Add(Await CreateFileViewModel(f.Entries(57))) '(Evo for Bulbasaur)
            EvolutionEntries.Add(Await CreateFileViewModel(f.Entries(58))) '(Evo for Charmander)
            EvolutionEntries.Add(Await CreateFileViewModel(f.Entries(59))) '(Evo for Squirtle)
            EvolutionEntries.Add(Await CreateFileViewModel(f.Entries(55))) '(Evo for Pikachu)
            EvolutionEntries.Add(Await CreateFileViewModel(f.Entries(60))) '(Evo for Chikorita)
            EvolutionEntries.Add(Await CreateFileViewModel(f.Entries(61))) '(Evo for Cyndaquil)
            EvolutionEntries.Add(Await CreateFileViewModel(f.Entries(62))) '(Evo for Totodile)
            EvolutionEntries.Add(Await CreateFileViewModel(f.Entries(63))) '(Evo for Treecko)
            EvolutionEntries.Add(Await CreateFileViewModel(f.Entries(64))) '(Evo for Torchic)
            EvolutionEntries.Add(Await CreateFileViewModel(f.Entries(65))) '(Evo for Mudkip)
            EvolutionEntries.Add(Await CreateFileViewModel(f.Entries(66))) '(Evo for Turtwig)
            EvolutionEntries.Add(Await CreateFileViewModel(f.Entries(67))) '(Evo for Chimchar)
            EvolutionEntries.Add(Await CreateFileViewModel(f.Entries(68))) '(Evo for Piplup)
            EvolutionEntries.Add(Await CreateFileViewModel(f.Entries(56))) '(Evo for Riolu)
            EvolutionEntries.Add(Await CreateFileViewModel(f.Entries(69))) '(Evo for Snivy)
            EvolutionEntries.Add(Await CreateFileViewModel(f.Entries(70))) '(Evo for Tepig)
            EvolutionEntries.Add(Await CreateFileViewModel(f.Entries(71))) '(Evo for Oshawott)
            EvolutionEntries.Add(Await CreateFileViewModel(f.Entries(72))) '(Evo for Chespin)
            EvolutionEntries.Add(Await CreateFileViewModel(f.Entries(73))) '(Evo for Fennekin)
            EvolutionEntries.Add(Await CreateFileViewModel(f.Entries(74))) '(Evo for Froakie)

            SelectedStarterEntry = StarterEntries(0)
        End Sub

        Public Overrides Sub UpdateModel(model As Object)
            MyBase.UpdateModel(model)

            Dim f As FixedPokemon = model

            f.Entries(19) = StarterEntries(0).Model
            f.Entries(21) = StarterEntries(1).Model
            f.Entries(23) = StarterEntries(2).Model
            f.Entries(17) = StarterEntries(3).Model
            f.Entries(25) = StarterEntries(4).Model
            f.Entries(27) = StarterEntries(5).Model
            f.Entries(29) = StarterEntries(6).Model
            f.Entries(31) = StarterEntries(7).Model
            f.Entries(33) = StarterEntries(8).Model
            f.Entries(35) = StarterEntries(9).Model
            f.Entries(37) = StarterEntries(10).Model
            f.Entries(39) = StarterEntries(11).Model
            f.Entries(41) = StarterEntries(12).Model
            f.Entries(18) = StarterEntries(13).Model
            f.Entries(43) = StarterEntries(14).Model
            f.Entries(45) = StarterEntries(15).Model
            f.Entries(47) = StarterEntries(16).Model
            f.Entries(49) = StarterEntries(17).Model
            f.Entries(51) = StarterEntries(18).Model
            f.Entries(53) = StarterEntries(19).Model

            f.Entries(57) = EvolutionEntries(0).Model
            f.Entries(58) = EvolutionEntries(1).Model
            f.Entries(59) = EvolutionEntries(2).Model
            f.Entries(55) = EvolutionEntries(3).Model
            f.Entries(60) = EvolutionEntries(4).Model
            f.Entries(61) = EvolutionEntries(5).Model
            f.Entries(62) = EvolutionEntries(6).Model
            f.Entries(63) = EvolutionEntries(7).Model
            f.Entries(64) = EvolutionEntries(8).Model
            f.Entries(65) = EvolutionEntries(9).Model
            f.Entries(66) = EvolutionEntries(10).Model
            f.Entries(67) = EvolutionEntries(11).Model
            f.Entries(68) = EvolutionEntries(12).Model
            f.Entries(56) = EvolutionEntries(13).Model
            f.Entries(69) = EvolutionEntries(14).Model
            f.Entries(70) = EvolutionEntries(15).Model
            f.Entries(71) = EvolutionEntries(16).Model
            f.Entries(72) = EvolutionEntries(17).Model
            f.Entries(73) = EvolutionEntries(18).Model
            f.Entries(74) = EvolutionEntries(19).Model
        End Sub

        Private Sub OnEntryModified(sender As Object, e As EventArgs)
            RaiseEvent Modified(Me, New EventArgs)
        End Sub

        Private Async Function CreateFileViewModel(model As Object) As Task(Of FixedPokemonEntryViewModel)
            Dim vm As New FixedPokemonEntryViewModel
            vm.SetApplicationViewModel(CurrentApplicationViewModel)
            vm.SetModel(model)
            Await vm.SetLanguageProject(CurrentApplicationViewModel.GetFileViewModelForModel(Me.Model).ParentProject)
            AddHandler vm.Modified, AddressOf OnEntryModified

            Return vm
        End Function

        ''' <summary>
        ''' Clears the list, or initializes it if null.
        ''' </summary>
        ''' <param name="list">List to clear or initialize.</param>
        ''' <remarks>Also removes event handlers added by <see cref="CreateFileViewModel(Object)"/>.</remarks>
        Private Sub ClearList(ByRef list As List(Of FixedPokemonEntryViewModel))
            If list Is Nothing Then
                list = New List(Of FixedPokemonEntryViewModel)
            Else
                For Each item In list
                    RemoveHandler item.Modified, AddressOf OnEntryModified
                Next
                list.Clear()
            End If
        End Sub
    End Class
End Namespace