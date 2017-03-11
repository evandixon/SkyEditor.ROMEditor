Imports System.ComponentModel
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.ROMEditor.MysteryDungeon.Rescue

Namespace MysteryDungeon.Rescue.ViewModels
    Public Class RescueTeamStartersViewModel
        Inherits GenericViewModel(Of IRescueTeamStarters)
        Implements INotifyPropertyChanged
        Implements INotifyModified

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Public Event Modified As EventHandler Implements INotifyModified.Modified

        Public Property HardyMale As Integer
            Get
                Return Model.HardyMale
            End Get
            Set(value As Integer)
                If Model.HardyMale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.HardyMale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HardyMale)))
                End If
            End Set
        End Property

        Public Property HardyFemale As Integer
            Get
                Return Model.HardyFemale
            End Get
            Set(value As Integer)
                If Model.HardyFemale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.HardyFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HardyFemale)))
                End If
            End Set
        End Property

        Public Property DocileMale As Integer
            Get
                Return Model.DocileMale
            End Get
            Set(value As Integer)
                If Model.DocileMale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.DocileMale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(DocileMale)))
                End If
            End Set
        End Property

        Public Property DocileFemale As Integer
            Get
                Return Model.DocileFemale
            End Get
            Set(value As Integer)
                If Model.DocileFemale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.DocileFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(DocileFemale)))
                End If
            End Set
        End Property

        Public Property BraveMale As Integer
            Get
                Return Model.BraveMale
            End Get
            Set(value As Integer)
                If Model.BraveMale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.BraveMale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(BraveMale)))
                End If
            End Set
        End Property

        Public Property BraveFemale As Integer
            Get
                Return Model.BraveFemale
            End Get
            Set(value As Integer)
                If Model.BraveFemale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.BraveFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(BraveFemale)))
                End If
            End Set
        End Property

        Public Property JollyMale As Integer
            Get
                Return Model.JollyMale
            End Get
            Set(value As Integer)
                If Model.JollyMale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.JollyMale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(JollyMale)))
                End If
            End Set
        End Property

        Public Property JollyFemale As Integer
            Get
                Return Model.JollyFemale
            End Get
            Set(value As Integer)
                If Model.JollyFemale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.JollyFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(JollyFemale)))
                End If
            End Set
        End Property

        Public Property ImpishMale As Integer
            Get
                Return Model.ImpishMale
            End Get
            Set(value As Integer)
                If Model.ImpishMale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.ImpishMale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ImpishMale)))
                End If
            End Set
        End Property

        Public Property ImpishFemale As Integer
            Get
                Return Model.ImpishFemale
            End Get
            Set(value As Integer)
                If Model.ImpishFemale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.ImpishFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ImpishFemale)))
                End If
            End Set
        End Property

        Public Property NaiveMale As Integer
            Get
                Return Model.NaiveMale
            End Get
            Set(value As Integer)
                If Model.NaiveMale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.NaiveMale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(NaiveMale)))
                End If
            End Set
        End Property

        Public Property NaiveFemale As Integer
            Get
                Return Model.NaiveFemale
            End Get
            Set(value As Integer)
                If Model.NaiveFemale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.NaiveFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(NaiveFemale)))
                End If
            End Set
        End Property

        Public Property TimidMale As Integer
            Get
                Return Model.TimidMale
            End Get
            Set(value As Integer)
                If Model.TimidMale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.TimidMale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TimidMale)))
                End If
            End Set
        End Property

        Public Property TimidFemale As Integer
            Get
                Return Model.TimidFemale
            End Get
            Set(value As Integer)
                If Model.TimidFemale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.TimidFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TimidFemale)))
                End If
            End Set
        End Property

        Public Property HastyMale As Integer
            Get
                Return Model.HastyMale
            End Get
            Set(value As Integer)
                If Model.HastyMale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.HastyMale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HastyMale)))
                End If
            End Set
        End Property

        Public Property HastyFemale As Integer
            Get
                Return Model.HastyFemale
            End Get
            Set(value As Integer)
                If Model.HastyFemale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.HastyFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HastyFemale)))
                End If
            End Set
        End Property

        Public Property SassyMale As Integer
            Get
                Return Model.SassyMale
            End Get
            Set(value As Integer)
                If Model.SassyMale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.SassyMale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SassyMale)))
                End If
            End Set
        End Property

        Public Property SassyFemale As Integer
            Get
                Return Model.SassyFemale
            End Get
            Set(value As Integer)
                If Model.SassyFemale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.SassyFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SassyFemale)))
                End If
            End Set
        End Property

        Public Property CalmMale As Integer
            Get
                Return Model.CalmMale
            End Get
            Set(value As Integer)
                If Model.CalmMale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.CalmMale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(CalmMale)))
                End If
            End Set
        End Property

        Public Property CalmFemale As Integer
            Get
                Return Model.CalmFemale
            End Get
            Set(value As Integer)
                If Model.CalmFemale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.CalmFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(CalmFemale)))
                End If
            End Set
        End Property

        Public Property RelaxedMale As Integer
            Get
                Return Model.RelaxedMale
            End Get
            Set(value As Integer)
                If Model.RelaxedMale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.RelaxedMale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RelaxedMale)))
                End If
            End Set
        End Property

        Public Property RelaxedFemale As Integer
            Get
                Return Model.RelaxedFemale
            End Get
            Set(value As Integer)
                If Model.RelaxedFemale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.RelaxedFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RelaxedFemale)))
                End If
            End Set
        End Property

        Public Property LonelyMale As Integer
            Get
                Return Model.LonelyMale
            End Get
            Set(value As Integer)
                If Model.LonelyMale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.LonelyMale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LonelyMale)))
                End If
            End Set
        End Property

        Public Property LonelyFemale As Integer
            Get
                Return Model.LonelyFemale
            End Get
            Set(value As Integer)
                If Model.LonelyFemale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.LonelyFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LonelyFemale)))
                End If
            End Set
        End Property

        Public Property QuirkyMale As Integer
            Get
                Return Model.QuirkyMale
            End Get
            Set(value As Integer)
                If Model.QuirkyMale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.QuirkyMale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(QuirkyMale)))
                End If
            End Set
        End Property
        Public Property QuirkyFemale As Integer
            Get
                Return Model.QuirkyFemale
            End Get
            Set(value As Integer)
                If Model.QuirkyFemale <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.QuirkyFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(QuirkyFemale)))
                End If
            End Set
        End Property

        Public Property Partner01 As Integer
            Get
                Return Model.Partner01
            End Get
            Set(value As Integer)
                If Model.Partner01 <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.Partner01 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner01)))
                End If
            End Set
        End Property

        Public Property Partner02 As Integer
            Get
                Return Model.Partner02
            End Get
            Set(value As Integer)
                If Model.Partner02 <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.Partner02 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner02)))
                End If
            End Set
        End Property

        Public Property Partner03 As Integer
            Get
                Return Model.Partner03
            End Get
            Set(value As Integer)
                If Model.Partner03 <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.Partner03 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner03)))
                End If
            End Set
        End Property

        Public Property Partner04 As Integer
            Get
                Return Model.Partner04
            End Get
            Set(value As Integer)
                If Model.Partner04 <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.Partner04 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner04)))
                End If
            End Set
        End Property

        Public Property Partner05 As Integer
            Get
                Return Model.Partner05
            End Get
            Set(value As Integer)
                If Model.Partner05 <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.Partner05 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner05)))
                End If
            End Set
        End Property

        Public Property Partner06 As Integer
            Get
                Return Model.Partner06
            End Get
            Set(value As Integer)
                If Model.Partner06 <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.Partner06 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner06)))
                End If
            End Set
        End Property

        Public Property Partner07 As Integer
            Get
                Return Model.Partner07
            End Get
            Set(value As Integer)
                If Model.Partner07 <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.Partner07 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner07)))
                End If
            End Set
        End Property

        Public Property Partner08 As Integer
            Get
                Return Model.Partner08
            End Get
            Set(value As Integer)
                If Model.Partner08 <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.Partner08 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner08)))
                End If
            End Set
        End Property

        Public Property Partner09 As Integer
            Get
                Return Model.Partner09
            End Get
            Set(value As Integer)
                If Model.Partner09 <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.Partner09 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner09)))
                End If
            End Set
        End Property

        Public Property Partner10 As Integer
            Get
                Return Model.Partner10
            End Get
            Set(value As Integer)
                If Model.Partner10 <> value AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    Model.Partner10 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner10)))
                End If
            End Set
        End Property
    End Class
End Namespace

