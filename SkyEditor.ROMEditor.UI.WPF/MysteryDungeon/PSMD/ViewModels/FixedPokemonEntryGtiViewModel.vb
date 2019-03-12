Imports System.ComponentModel
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD.Dungeon
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD.Projects

Namespace MysteryDungeon.PSMD.ViewModels
    Public Class FixedPokemonEntryGtiViewModel
        Inherits GenericViewModel(Of FixedPokemon.PokemonEntryGti)
        Implements INotifyPropertyChanged
        Implements INotifyModified

        Public Sub New(fixedPokemon As FixedPokemon, ioProvider As IIOProvider)
            If fixedPokemon Is Nothing Then
                Throw New ArgumentNullException(NameOf(fixedPokemon))
            End If
            If ioProvider Is Nothing Then
                Throw New ArgumentNullException(NameOf(ioProvider))
            End If

            CurrentIOProvider = ioProvider
        End Sub

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Public Event Modified As EventHandler Implements INotifyModified.Modified

        Private Sub FixedPokemonEntryViewModel_PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles Me.PropertyChanged
            RaiseEvent Modified(Me, New EventArgs)
        End Sub

        Protected Property CurrentIOProvider As IIOProvider

        Protected Property FixedPokemon As FixedPokemon

        Public ReadOnly Property Name As String
            Get
                If PokemonNames Is Nothing Then
                    Throw New NotImplementedException("Unable to display a fixed_pokemon.bin file from outside a project")
                End If
                Return PokemonNames(PokemonID)
            End Get
        End Property

        Public Property PokemonID As Integer
            Get
                Return Model.PokemonID
            End Get
            Set(value As Integer)
                If Not Model.PokemonID = value Then
                    Model.PokemonID = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(PokemonID)))
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Name)))
                End If
            End Set
        End Property

        Public Property PokemonNames As Dictionary(Of Integer, String)
            Get
                Return _pokemonNames
            End Get
            Private Set(value As Dictionary(Of Integer, String))
                _pokemonNames = value
            End Set
        End Property
        Dim _pokemonNames As Dictionary(Of Integer, String)

        Public Async Function SetLanguageProject(project As IPsmdMessageBinProject) As Task
            If project IsNot Nothing Then
                PokemonNames = Await project.GetPokemonNames
            End If
        End Function

    End Class
End Namespace
