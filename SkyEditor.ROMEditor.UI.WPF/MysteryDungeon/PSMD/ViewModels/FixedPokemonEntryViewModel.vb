Imports System.ComponentModel
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD.Dungeon
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD.Projects

Namespace MysteryDungeon.PSMD.ViewModels
    Public Class FixedPokemonEntryViewModel
        Inherits GenericViewModel(Of FixedPokemon.PokemonEntry)
        Implements INotifyPropertyChanged
        Implements INotifyModified

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Public Event Modified As INotifyModified.ModifiedEventHandler Implements INotifyModified.Modified

        Private Sub FixedPokemonEntryViewModel_PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles Me.PropertyChanged
            RaiseEvent Modified(Me, New EventArgs)
        End Sub

        Public ReadOnly Property Name As String
            Get
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

        Public Property Move1 As Integer
            Get
                Return Model.Move1
            End Get
            Set(value As Integer)
                If Not Model.Move1 = value Then
                    Model.Move1 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Move1)))
                End If
            End Set
        End Property

        Public Property Move2 As Integer
            Get
                Return Model.Move2
            End Get
            Set(value As Integer)
                If Not Model.Move2 = value Then
                    Model.Move2 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Move2)))
                End If
            End Set
        End Property

        Public Property Move3 As Integer
            Get
                Return Model.Move3
            End Get
            Set(value As Integer)
                If Not Model.Move3 = value Then
                    Model.Move3 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Move3)))
                End If
            End Set
        End Property

        Public Property Move4 As Integer
            Get
                Return Model.Move4
            End Get
            Set(value As Integer)
                If Not Model.Move4 = value Then
                    Model.Move4 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Move4)))
                End If
            End Set
        End Property

        Public Property HPBoost As Integer
            Get
                Return Model.HPBoost
            End Get
            Set(value As Integer)
                If Not Model.HPBoost = value Then
                    Model.HPBoost = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HPBoost)))
                End If
            End Set
        End Property

        Public Property AttackBoost As Integer
            Get
                Return Model.AttackBoost
            End Get
            Set(value As Integer)
                If Not Model.AttackBoost = value Then
                    Model.AttackBoost = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(AttackBoost)))
                End If
            End Set
        End Property

        Public Property SpAttackBoost As Integer
            Get
                Return Model.SpAttackBoost
            End Get
            Set(value As Integer)
                If Not Model.SpAttackBoost = value Then
                    Model.SpAttackBoost = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SpAttackBoost)))
                End If
            End Set
        End Property

        Public Property DefenseBoost As Integer
            Get
                Return Model.DefenseBoost
            End Get
            Set(value As Integer)
                If Not Model.DefenseBoost = value Then
                    Model.DefenseBoost = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(DefenseBoost)))
                End If
            End Set
        End Property

        Public Property SpDefenseBoost As Integer
            Get
                Return Model.SpDefenseBoost
            End Get
            Set(value As Integer)
                If Not Model.SpDefenseBoost = value Then
                    Model.SpDefenseBoost = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SpDefenseBoost)))
                End If
            End Set
        End Property

        Public Property SpeedBoost As Integer
            Get
                Return Model.SpeedBoost
            End Get
            Set(value As Integer)
                If Not Model.SpeedBoost = value Then
                    Model.SpeedBoost = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SpeedBoost)))
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

        Public Property MoveNames As Dictionary(Of Integer, String)
            Get
                Return _moveNames
            End Get
            Private Set(value As Dictionary(Of Integer, String))
                _moveNames = value
            End Set
        End Property
        Dim _moveNames As Dictionary(Of Integer, String)

        Public Async Function SetLanguageProject(project As IPsmdMessageBinProject) As Task
            If project IsNot Nothing Then
                PokemonNames = Await project.GetPokemonNames
                MoveNames = Await project.GetMoveNames
            End If
        End Function

    End Class
End Namespace
