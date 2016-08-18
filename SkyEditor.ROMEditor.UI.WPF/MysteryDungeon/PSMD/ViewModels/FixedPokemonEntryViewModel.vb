Imports System.ComponentModel
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.ROMEditor.Windows.FileFormats.PSMD.Dungeon
Imports SkyEditor.ROMEditor.Windows.MysteryDungeon.PSMD.Projects

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

        Public Property PokemonID As Int16
            Get
                Return Model.PokemonID
            End Get
            Set(value As Int16)
                If Not value = Model.PokemonID Then
                    value = Model.PokemonID
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(PokemonID)))
                End If
            End Set
        End Property

        Public Property Move1 As UInt16
            Get
                Return Model.Move1
            End Get
            Set(value As UInt16)
                If Not value = Model.Move1 Then
                    value = Model.Move1
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Move1)))
                End If
            End Set
        End Property

        Public Property Move2 As UInt16
            Get
                Return Model.Move2
            End Get
            Set(value As UInt16)
                If Not value = Model.Move2 Then
                    value = Model.Move2
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Move2)))
                End If
            End Set
        End Property

        Public Property Move3 As UInt16
            Get
                Return Model.Move3
            End Get
            Set(value As UInt16)
                If Not value = Model.Move3 Then
                    value = Model.Move3
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Move3)))
                End If
            End Set
        End Property

        Public Property Move4 As UInt16
            Get
                Return Model.Move4
            End Get
            Set(value As UInt16)
                If Not value = Model.Move4 Then
                    value = Model.Move4
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Move4)))
                End If
            End Set
        End Property

        Public Property HPBoost As Int16
            Get
                Return Model.HPBoost
            End Get
            Set(value As Int16)
                If Not value = Model.HPBoost Then
                    value = Model.HPBoost
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HPBoost)))
                End If
            End Set
        End Property

        Public Property AttackBoost As Byte
            Get
                Return Model.AttackBoost
            End Get
            Set(value As Byte)
                If Not value = Model.AttackBoost Then
                    value = Model.AttackBoost
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(AttackBoost)))
                End If
            End Set
        End Property

        Public Property SpAttackBoost As Byte
            Get
                Return Model.SpAttackBoost
            End Get
            Set(value As Byte)
                If Not value = Model.SpAttackBoost Then
                    value = Model.SpAttackBoost
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SpAttackBoost)))
                End If
            End Set
        End Property

        Public Property DefenseBoost As Byte
            Get
                Return Model.DefenseBoost
            End Get
            Set(value As Byte)
                If Not value = Model.DefenseBoost Then
                    value = Model.DefenseBoost
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(DefenseBoost)))
                End If
            End Set
        End Property

        Public Property SpDefenseBoost As Byte
            Get
                Return Model.SpDefenseBoost
            End Get
            Set(value As Byte)
                If Not value = Model.SpDefenseBoost Then
                    value = Model.SpDefenseBoost
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SpDefenseBoost)))
                End If
            End Set
        End Property

        Public Property SpeedBoost As Byte
            Get
                Return Model.SpeedBoost
            End Get
            Set(value As Byte)
                If Not value = Model.SpeedBoost Then
                    value = Model.SpeedBoost
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
