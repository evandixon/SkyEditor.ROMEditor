Imports System.Reflection
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI

Namespace MysteryDungeon.Explorers.ViewModels
    ''' <summary>
    ''' Stores the starter data from Overlay13.
    ''' </summary>
    Public Class PersonalityTestContainer
        Inherits GenericViewModel(Of Overlay13)
        Implements INotifyPropertyChanged
        Implements INotifyModified

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Public Event Modified As EventHandler Implements INotifyModified.Modified

        Public Sub New()
        End Sub

        Public Sub New(overlay As Overlay13)
            SetModelInternal(overlay)
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of TypeInfo)
            Return {GetType(Overlay13).GetTypeInfo}
        End Function

        Public Overrides Sub SetModel(model As Object)
            MyBase.SetModel(model)
            SetModelInternal(model)
        End Sub

        Private Sub SetModelInternal(overlay As Overlay13)
            If overlay.Partner01 > 600 Then
                Me.Partner01Pokemon = overlay.Partner01 - 600
                Me.Partner01IsFemale = True
            Else
                Me.Partner01Pokemon = overlay.Partner01
                Me.Partner01IsFemale = False
            End If

            If overlay.Partner02 > 600 Then
                Me.Partner02Pokemon = overlay.Partner02 - 600
                Me.Partner02IsFemale = True
            Else
                Me.Partner02Pokemon = overlay.Partner02
                Me.Partner02IsFemale = False
            End If

            If overlay.Partner03 > 600 Then
                Me.Partner03Pokemon = overlay.Partner03 - 600
                Me.Partner03IsFemale = True
            Else
                Me.Partner03Pokemon = overlay.Partner03
                Me.Partner03IsFemale = False
            End If

            If overlay.Partner04 > 600 Then
                Me.Partner04Pokemon = overlay.Partner04 - 600
                Me.Partner04IsFemale = True
            Else
                Me.Partner04Pokemon = overlay.Partner04
                Me.Partner04IsFemale = False
            End If

            If overlay.Partner05 > 600 Then
                Me.Partner05Pokemon = overlay.Partner05 - 600
                Me.Partner05IsFemale = True
            Else
                Me.Partner05Pokemon = overlay.Partner05
                Me.Partner05IsFemale = False
            End If

            If overlay.Partner06 > 600 Then
                Me.Partner06Pokemon = overlay.Partner06 - 600
                Me.Partner06IsFemale = True
            Else
                Me.Partner06Pokemon = overlay.Partner06
                Me.Partner06IsFemale = False
            End If

            If overlay.Partner07 > 600 Then
                Me.Partner07Pokemon = overlay.Partner07 - 600
                Me.Partner07IsFemale = True
            Else
                Me.Partner07Pokemon = overlay.Partner07
                Me.Partner07IsFemale = False
            End If

            If overlay.Partner08 > 600 Then
                Me.Partner08Pokemon = overlay.Partner08 - 600
                Me.Partner08IsFemale = True
            Else
                Me.Partner08Pokemon = overlay.Partner08
                Me.Partner08IsFemale = False
            End If

            If overlay.Partner09 > 600 Then
                Me.Partner09Pokemon = overlay.Partner09 - 600
                Me.Partner09IsFemale = True
            Else
                Me.Partner09Pokemon = overlay.Partner09
                Me.Partner09IsFemale = False
            End If

            If overlay.Partner10 > 600 Then
                Me.Partner10Pokemon = overlay.Partner10 - 600
                Me.Partner10IsFemale = True
            Else
                Me.Partner10Pokemon = overlay.Partner10
                Me.Partner10IsFemale = False
            End If

            If overlay.Partner11 > 600 Then
                Me.Partner11Pokemon = overlay.Partner11 - 600
                Me.Partner11IsFemale = True
            Else
                Me.Partner11Pokemon = overlay.Partner11
                Me.Partner11IsFemale = False
            End If

            If overlay.Partner12 > 600 Then
                Me.Partner12Pokemon = overlay.Partner12 - 600
                Me.Partner12IsFemale = True
            Else
                Me.Partner12Pokemon = overlay.Partner12
                Me.Partner12IsFemale = False
            End If

            If overlay.Partner13 > 600 Then
                Me.Partner13Pokemon = overlay.Partner13 - 600
                Me.Partner13IsFemale = True
            Else
                Me.Partner13Pokemon = overlay.Partner13
                Me.Partner13IsFemale = False
            End If

            If overlay.Partner14 > 600 Then
                Me.Partner14Pokemon = overlay.Partner14 - 600
                Me.Partner14IsFemale = True
            Else
                Me.Partner14Pokemon = overlay.Partner14
                Me.Partner14IsFemale = False
            End If

            If overlay.Partner15 > 600 Then
                Me.Partner15Pokemon = overlay.Partner15 - 600
                Me.Partner15IsFemale = True
            Else
                Me.Partner15Pokemon = overlay.Partner15
                Me.Partner15IsFemale = False
            End If

            If overlay.Partner16 > 600 Then
                Me.Partner16Pokemon = overlay.Partner16 - 600
                Me.Partner16IsFemale = True
            Else
                Me.Partner16Pokemon = overlay.Partner16
                Me.Partner16IsFemale = False
            End If

            If overlay.Partner17 > 600 Then
                Me.Partner17Pokemon = overlay.Partner17 - 600
                Me.Partner17IsFemale = True
            Else
                Me.Partner17Pokemon = overlay.Partner17
                Me.Partner17IsFemale = False
            End If

            If overlay.Partner18 > 600 Then
                Me.Partner18Pokemon = overlay.Partner18 - 600
                Me.Partner18IsFemale = True
            Else
                Me.Partner18Pokemon = overlay.Partner18
                Me.Partner18IsFemale = False
            End If

            If overlay.Partner19 > 600 Then
                Me.Partner19Pokemon = overlay.Partner19 - 600
                Me.Partner19IsFemale = True
            Else
                Me.Partner19Pokemon = overlay.Partner19
                Me.Partner19IsFemale = False
            End If

            If overlay.Partner20 > 600 Then
                Me.Partner20Pokemon = overlay.Partner20 - 600
                Me.Partner20IsFemale = True
            Else
                Me.Partner20Pokemon = overlay.Partner20
                Me.Partner20IsFemale = False
            End If

            If overlay.Partner21 > 600 Then
                Me.Partner21Pokemon = overlay.Partner21 - 600
                Me.Partner21IsFemale = True
            Else
                Me.Partner21Pokemon = overlay.Partner21
                Me.Partner21IsFemale = False
            End If

            If overlay.HardyMale > 600 Then
                Me.HardyMalePokemon = overlay.HardyMale - 600
                Me.HardyMaleIsFemale = True
            Else
                Me.HardyMalePokemon = overlay.HardyMale
                Me.HardyMaleIsFemale = False
            End If

            If overlay.HardyFemale > 600 Then
                Me.HardyFemalePokemon = overlay.HardyFemale - 600
                Me.HardyFemaleIsFemale = True
            Else
                Me.HardyFemalePokemon = overlay.HardyFemale
                Me.HardyFemaleIsFemale = False
            End If

            If overlay.DocileMale > 600 Then
                Me.DocileMalePokemon = overlay.DocileMale - 600
                Me.DocileMaleIsFemale = True
            Else
                Me.DocileMalePokemon = overlay.DocileMale
                Me.DocileMaleIsFemale = False
            End If

            If overlay.DocileFemale > 600 Then
                Me.DocileFemalePokemon = overlay.DocileFemale - 600
                Me.DocileFemaleIsFemale = True
            Else
                Me.DocileFemalePokemon = overlay.DocileFemale
                Me.DocileFemaleIsFemale = False
            End If

            If overlay.BraveMale > 600 Then
                Me.BraveMalePokemon = overlay.BraveMale - 600
                Me.BraveMaleIsFemale = True
            Else
                Me.BraveMalePokemon = overlay.BraveMale
                Me.BraveMaleIsFemale = False
            End If

            If overlay.BraveFemale > 600 Then
                Me.BraveFemalePokemon = overlay.BraveFemale - 600
                Me.BraveFemaleIsFemale = True
            Else
                Me.BraveFemalePokemon = overlay.BraveFemale
                Me.BraveFemaleIsFemale = False
            End If

            If overlay.JollyMale > 600 Then
                Me.JollyMalePokemon = overlay.JollyMale - 600
                Me.JollyMaleIsFemale = True
            Else
                Me.JollyMalePokemon = overlay.JollyMale
                Me.JollyMaleIsFemale = False
            End If

            If overlay.JollyFemale > 600 Then
                Me.JollyFemalePokemon = overlay.JollyFemale - 600
                Me.JollyFemaleIsFemale = True
            Else
                Me.JollyFemalePokemon = overlay.JollyFemale
                Me.JollyFemaleIsFemale = False
            End If

            If overlay.ImpishMale > 600 Then
                Me.ImpishMalePokemon = overlay.ImpishMale - 600
                Me.ImpishMaleIsFemale = True
            Else
                Me.ImpishMalePokemon = overlay.ImpishMale
                Me.ImpishMaleIsFemale = False
            End If

            If overlay.ImpishFemale > 600 Then
                Me.ImpishFemalePokemon = overlay.ImpishFemale - 600
                Me.ImpishFemaleIsFemale = True
            Else
                Me.ImpishFemalePokemon = overlay.ImpishFemale
                Me.ImpishFemaleIsFemale = False
            End If

            If overlay.NaiveMale > 600 Then
                Me.NaiveMalePokemon = overlay.NaiveMale - 600
                Me.NaiveMaleIsFemale = True
            Else
                Me.NaiveMalePokemon = overlay.NaiveMale
                Me.NaiveMaleIsFemale = False
            End If

            If overlay.NaiveFemale > 600 Then
                Me.NaiveFemalePokemon = overlay.NaiveFemale - 600
                Me.NaiveFemaleIsFemale = True
            Else
                Me.NaiveFemalePokemon = overlay.NaiveFemale
                Me.NaiveFemaleIsFemale = False
            End If

            If overlay.TimidMale > 600 Then
                Me.TimidMalePokemon = overlay.TimidMale - 600
                Me.TimidMaleIsFemale = True
            Else
                Me.TimidMalePokemon = overlay.TimidMale
                Me.TimidMaleIsFemale = False
            End If

            If overlay.TimidFemale > 600 Then
                Me.TimidFemalePokemon = overlay.TimidFemale - 600
                Me.TimidFemaleIsFemale = True
            Else
                Me.TimidFemalePokemon = overlay.TimidFemale
                Me.TimidFemaleIsFemale = False
            End If

            If overlay.HastyMale > 600 Then
                Me.HastyMalePokemon = overlay.HastyMale - 600
                Me.HastyMaleIsFemale = True
            Else
                Me.HastyMalePokemon = overlay.HastyMale
                Me.HastyMaleIsFemale = False
            End If

            If overlay.HastyFemale > 600 Then
                Me.HastyFemalePokemon = overlay.HastyFemale - 600
                Me.HastyFemaleIsFemale = True
            Else
                Me.HastyFemalePokemon = overlay.HastyFemale
                Me.HastyFemaleIsFemale = False
            End If

            If overlay.SassyMale > 600 Then
                Me.SassyMalePokemon = overlay.SassyMale - 600
                Me.SassyMaleIsFemale = True
            Else
                Me.SassyMalePokemon = overlay.SassyMale
                Me.SassyMaleIsFemale = False
            End If

            If overlay.SassyFemale > 600 Then
                Me.SassyFemalePokemon = overlay.SassyFemale - 600
                Me.SassyFemaleIsFemale = True
            Else
                Me.SassyFemalePokemon = overlay.SassyFemale
                Me.SassyFemaleIsFemale = False
            End If

            If overlay.CalmMale > 600 Then
                Me.CalmMalePokemon = overlay.CalmMale - 600
                Me.CalmMaleIsFemale = True
            Else
                Me.CalmMalePokemon = overlay.CalmMale
                Me.CalmMaleIsFemale = False
            End If

            If overlay.CalmFemale > 600 Then
                Me.CalmFemalePokemon = overlay.CalmFemale - 600
                Me.CalmFemaleIsFemale = True
            Else
                Me.CalmFemalePokemon = overlay.CalmFemale
                Me.CalmFemaleIsFemale = False
            End If

            If overlay.RelaxedMale > 600 Then
                Me.RelaxedMalePokemon = overlay.RelaxedMale - 600
                Me.RelaxedMaleIsFemale = True
            Else
                Me.RelaxedMalePokemon = overlay.RelaxedMale
                Me.RelaxedMaleIsFemale = False
            End If

            If overlay.RelaxedFemale > 600 Then
                Me.RelaxedFemalePokemon = overlay.RelaxedFemale - 600
                Me.RelaxedFemaleIsFemale = True
            Else
                Me.RelaxedFemalePokemon = overlay.RelaxedFemale
                Me.RelaxedFemaleIsFemale = False
            End If

            If overlay.LonelyMale > 600 Then
                Me.LonelyMalePokemon = overlay.LonelyMale - 600
                Me.LonelyMaleIsFemale = True
            Else
                Me.LonelyMalePokemon = overlay.LonelyMale
                Me.LonelyMaleIsFemale = False
            End If

            If overlay.LonelyFemale > 600 Then
                Me.LonelyFemalePokemon = overlay.LonelyFemale - 600
                Me.LonelyFemaleIsFemale = True
            Else
                Me.LonelyFemalePokemon = overlay.LonelyFemale
                Me.LonelyFemaleIsFemale = False
            End If

            If overlay.QuirkyMale > 600 Then
                Me.QuirkyMalePokemon = overlay.QuirkyMale - 600
                Me.QuirkyMaleIsFemale = True
            Else
                Me.QuirkyMalePokemon = overlay.QuirkyMale
                Me.QuirkyMaleIsFemale = False
            End If

            If overlay.QuirkyFemale > 600 Then
                Me.QuirkyFemalePokemon = overlay.QuirkyFemale - 600
                Me.QuirkyFemaleIsFemale = True
            Else
                Me.QuirkyFemalePokemon = overlay.QuirkyFemale
                Me.QuirkyFemaleIsFemale = False
            End If

            If overlay.QuietMale > 600 Then
                Me.QuietMalePokemon = overlay.QuietMale - 600
                Me.QuietMaleIsFemale = True
            Else
                Me.QuietMalePokemon = overlay.QuietMale
                Me.QuietMaleIsFemale = False
            End If

            If overlay.QuietFemale > 600 Then
                Me.QuietFemalePokemon = overlay.QuietFemale - 600
                Me.QuietFemaleIsFemale = True
            Else
                Me.QuietFemalePokemon = overlay.QuietFemale
                Me.QuietFemaleIsFemale = False
            End If

            If overlay.RashMale > 600 Then
                Me.RashMalePokemon = overlay.RashMale - 600
                Me.RashMaleIsFemale = True
            Else
                Me.RashMalePokemon = overlay.RashMale
                Me.RashMaleIsFemale = False
            End If

            If overlay.RashFemale > 600 Then
                Me.RashFemalePokemon = overlay.RashFemale - 600
                Me.RashFemaleIsFemale = True
            Else
                Me.RashFemalePokemon = overlay.RashFemale
                Me.RashFemaleIsFemale = False
            End If

            If overlay.BoldMale > 600 Then
                Me.BoldMalePokemon = overlay.BoldMale - 600
                Me.BoldMaleIsFemale = True
            Else
                Me.BoldMalePokemon = overlay.BoldMale
                Me.BoldMaleIsFemale = False
            End If

            If overlay.BoldFemale > 600 Then
                Me.BoldFemalePokemon = overlay.BoldFemale - 600
                Me.BoldFemaleIsFemale = True
            Else
                Me.BoldFemalePokemon = overlay.BoldFemale
                Me.BoldFemaleIsFemale = False
            End If
        End Sub

        Public Overrides Sub UpdateModel(model As Object)
            MyBase.UpdateModel(model)

            Dim overlay As Overlay13 = model
            If Me.Partner01IsFemale Then
                overlay.Partner01 = Me.Partner01Pokemon + 600
            Else
                overlay.Partner01 = Me.Partner01Pokemon
            End If
            If Me.Partner02IsFemale Then
                overlay.Partner02 = Me.Partner02Pokemon + 600
            Else
                overlay.Partner02 = Me.Partner02Pokemon
            End If
            If Me.Partner03IsFemale Then
                overlay.Partner03 = Me.Partner03Pokemon + 600
            Else
                overlay.Partner03 = Me.Partner03Pokemon
            End If
            If Me.Partner04IsFemale Then
                overlay.Partner04 = Me.Partner04Pokemon + 600
            Else
                overlay.Partner04 = Me.Partner04Pokemon
            End If
            If Me.Partner05IsFemale Then
                overlay.Partner05 = Me.Partner05Pokemon + 600
            Else
                overlay.Partner05 = Me.Partner05Pokemon
            End If
            If Me.Partner06IsFemale Then
                overlay.Partner06 = Me.Partner06Pokemon + 600
            Else
                overlay.Partner06 = Me.Partner06Pokemon
            End If
            If Me.Partner07IsFemale Then
                overlay.Partner07 = Me.Partner07Pokemon + 600
            Else
                overlay.Partner07 = Me.Partner07Pokemon
            End If
            If Me.Partner08IsFemale Then
                overlay.Partner08 = Me.Partner08Pokemon + 600
            Else
                overlay.Partner08 = Me.Partner08Pokemon
            End If
            If Me.Partner09IsFemale Then
                overlay.Partner09 = Me.Partner09Pokemon + 600
            Else
                overlay.Partner09 = Me.Partner09Pokemon
            End If
            If Me.Partner10IsFemale Then
                overlay.Partner10 = Me.Partner10Pokemon + 600
            Else
                overlay.Partner10 = Me.Partner10Pokemon
            End If
            If Me.Partner11IsFemale Then
                overlay.Partner11 = Me.Partner11Pokemon + 600
            Else
                overlay.Partner11 = Me.Partner11Pokemon
            End If
            If Me.Partner12IsFemale Then
                overlay.Partner12 = Me.Partner12Pokemon + 600
            Else
                overlay.Partner12 = Me.Partner12Pokemon
            End If
            If Me.Partner13IsFemale Then
                overlay.Partner13 = Me.Partner13Pokemon + 600
            Else
                overlay.Partner13 = Me.Partner13Pokemon
            End If
            If Me.Partner14IsFemale Then
                overlay.Partner14 = Me.Partner14Pokemon + 600
            Else
                overlay.Partner14 = Me.Partner14Pokemon
            End If
            If Me.Partner15IsFemale Then
                overlay.Partner15 = Me.Partner15Pokemon + 600
            Else
                overlay.Partner15 = Me.Partner15Pokemon
            End If
            If Me.Partner16IsFemale Then
                overlay.Partner16 = Me.Partner16Pokemon + 600
            Else
                overlay.Partner16 = Me.Partner16Pokemon
            End If
            If Me.Partner17IsFemale Then
                overlay.Partner17 = Me.Partner17Pokemon + 600
            Else
                overlay.Partner17 = Me.Partner17Pokemon
            End If
            If Me.Partner18IsFemale Then
                overlay.Partner18 = Me.Partner18Pokemon + 600
            Else
                overlay.Partner18 = Me.Partner18Pokemon
            End If
            If Me.Partner19IsFemale Then
                overlay.Partner19 = Me.Partner19Pokemon + 600
            Else
                overlay.Partner19 = Me.Partner19Pokemon
            End If
            If Me.Partner20IsFemale Then
                overlay.Partner20 = Me.Partner20Pokemon + 600
            Else
                overlay.Partner20 = Me.Partner20Pokemon
            End If
            If Me.Partner21IsFemale Then
                overlay.Partner21 = Me.Partner21Pokemon + 600
            Else
                overlay.Partner21 = Me.Partner21Pokemon
            End If
            If Me.HardyMaleIsFemale Then
                overlay.HardyMale = Me.HardyMalePokemon + 600
            Else
                overlay.HardyMale = Me.HardyMalePokemon
            End If
            If Me.HardyFemaleIsFemale Then
                overlay.HardyFemale = Me.HardyFemalePokemon + 600
            Else
                overlay.HardyFemale = Me.HardyFemalePokemon
            End If
            If Me.DocileMaleIsFemale Then
                overlay.DocileMale = Me.DocileMalePokemon + 600
            Else
                overlay.DocileMale = Me.DocileMalePokemon
            End If
            If Me.DocileFemaleIsFemale Then
                overlay.DocileFemale = Me.DocileFemalePokemon + 600
            Else
                overlay.DocileFemale = Me.DocileFemalePokemon
            End If
            If Me.BraveMaleIsFemale Then
                overlay.BraveMale = Me.BraveMalePokemon + 600
            Else
                overlay.BraveMale = Me.BraveMalePokemon
            End If
            If Me.BraveFemaleIsFemale Then
                overlay.BraveFemale = Me.BraveFemalePokemon + 600
            Else
                overlay.BraveFemale = Me.BraveFemalePokemon
            End If
            If Me.JollyMaleIsFemale Then
                overlay.JollyMale = Me.JollyMalePokemon + 600
            Else
                overlay.JollyMale = Me.JollyMalePokemon
            End If
            If Me.JollyFemaleIsFemale Then
                overlay.JollyFemale = Me.JollyFemalePokemon + 600
            Else
                overlay.JollyFemale = Me.JollyFemalePokemon
            End If
            If Me.ImpishMaleIsFemale Then
                overlay.ImpishMale = Me.ImpishMalePokemon + 600
            Else
                overlay.ImpishMale = Me.ImpishMalePokemon
            End If
            If Me.ImpishFemaleIsFemale Then
                overlay.ImpishFemale = Me.ImpishFemalePokemon + 600
            Else
                overlay.ImpishFemale = Me.ImpishFemalePokemon
            End If
            If Me.NaiveMaleIsFemale Then
                overlay.NaiveMale = Me.NaiveMalePokemon + 600
            Else
                overlay.NaiveMale = Me.NaiveMalePokemon
            End If
            If Me.NaiveFemaleIsFemale Then
                overlay.NaiveFemale = Me.NaiveFemalePokemon + 600
            Else
                overlay.NaiveFemale = Me.NaiveFemalePokemon
            End If
            If Me.TimidMaleIsFemale Then
                overlay.TimidMale = Me.TimidMalePokemon + 600
            Else
                overlay.TimidMale = Me.TimidMalePokemon
            End If
            If Me.TimidFemaleIsFemale Then
                overlay.TimidFemale = Me.TimidFemalePokemon + 600
            Else
                overlay.TimidFemale = Me.TimidFemalePokemon
            End If
            If Me.HastyMaleIsFemale Then
                overlay.HastyMale = Me.HastyMalePokemon + 600
            Else
                overlay.HastyMale = Me.HastyMalePokemon
            End If
            If Me.HastyFemaleIsFemale Then
                overlay.HastyFemale = Me.HastyFemalePokemon + 600
            Else
                overlay.HastyFemale = Me.HastyFemalePokemon
            End If
            If Me.SassyMaleIsFemale Then
                overlay.SassyMale = Me.SassyMalePokemon + 600
            Else
                overlay.SassyMale = Me.SassyMalePokemon
            End If
            If Me.SassyFemaleIsFemale Then
                overlay.SassyFemale = Me.SassyFemalePokemon + 600
            Else
                overlay.SassyFemale = Me.SassyFemalePokemon
            End If
            If Me.CalmMaleIsFemale Then
                overlay.CalmMale = Me.CalmMalePokemon + 600
            Else
                overlay.CalmMale = Me.CalmMalePokemon
            End If
            If Me.CalmFemaleIsFemale Then
                overlay.CalmFemale = Me.CalmFemalePokemon + 600
            Else
                overlay.CalmFemale = Me.CalmFemalePokemon
            End If
            If Me.RelaxedMaleIsFemale Then
                overlay.RelaxedMale = Me.RelaxedMalePokemon + 600
            Else
                overlay.RelaxedMale = Me.RelaxedMalePokemon
            End If
            If Me.RelaxedFemaleIsFemale Then
                overlay.RelaxedFemale = Me.RelaxedFemalePokemon + 600
            Else
                overlay.RelaxedFemale = Me.RelaxedFemalePokemon
            End If
            If Me.LonelyMaleIsFemale Then
                overlay.LonelyMale = Me.LonelyMalePokemon + 600
            Else
                overlay.LonelyMale = Me.LonelyMalePokemon
            End If
            If Me.LonelyFemaleIsFemale Then
                overlay.LonelyFemale = Me.LonelyFemalePokemon + 600
            Else
                overlay.LonelyFemale = Me.LonelyFemalePokemon
            End If
            If Me.QuirkyMaleIsFemale Then
                overlay.QuirkyMale = Me.QuirkyMalePokemon + 600
            Else
                overlay.QuirkyMale = Me.QuirkyMalePokemon
            End If
            If Me.QuirkyFemaleIsFemale Then
                overlay.QuirkyFemale = Me.QuirkyFemalePokemon + 600
            Else
                overlay.QuirkyFemale = Me.QuirkyFemalePokemon
            End If
            If Me.QuietMaleIsFemale Then
                overlay.QuietMale = Me.QuietMalePokemon + 600
            Else
                overlay.QuietMale = Me.QuietMalePokemon
            End If
            If Me.QuietFemaleIsFemale Then
                overlay.QuietFemale = Me.QuietFemalePokemon + 600
            Else
                overlay.QuietFemale = Me.QuietFemalePokemon
            End If
            If Me.RashMaleIsFemale Then
                overlay.RashMale = Me.RashMalePokemon + 600
            Else
                overlay.RashMale = Me.RashMalePokemon
            End If
            If Me.RashFemaleIsFemale Then
                overlay.RashFemale = Me.RashFemalePokemon + 600
            Else
                overlay.RashFemale = Me.RashFemalePokemon
            End If
            If Me.BoldMaleIsFemale Then
                overlay.BoldMale = Me.BoldMalePokemon + 600
            Else
                overlay.BoldMale = Me.BoldMalePokemon
            End If
            If Me.BoldFemaleIsFemale Then
                overlay.BoldFemale = Me.BoldFemalePokemon + 600
            Else
                overlay.BoldFemale = Me.BoldFemalePokemon
            End If
        End Sub

        Private Sub PersonalityTestContainer_PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles Me.PropertyChanged
            RaiseEvent Modified(Me, New EventArgs)
        End Sub

#Region "Properties"


        Public Property Partner01Pokemon As Integer
            Get
                Return _Partner01Pokemon
            End Get
            Set(value As Integer)
                If Not _Partner01Pokemon = value Then
                    _Partner01Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner01Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner01Pokemon As Integer

        Public Property Partner01IsFemale As Boolean
            Get
                Return _Partner01IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner01IsFemale = value Then
                    _Partner01IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner01IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner01IsFemale As Boolean

        Public Property Partner02Pokemon As Integer
            Get
                Return _Partner02Pokemon
            End Get
            Set(value As Integer)
                If Not _Partner02Pokemon = value Then
                    _Partner02Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner02Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner02Pokemon As Integer

        Public Property Partner02IsFemale As Boolean
            Get
                Return _Partner02IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner02IsFemale = value Then
                    _Partner02IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner02IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner02IsFemale As Boolean

        Public Property Partner03Pokemon As Integer
            Get
                Return _Partner03Pokemon
            End Get
            Set(value As Integer)
                If Not _Partner03Pokemon = value Then
                    _Partner03Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner03Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner03Pokemon As Integer

        Public Property Partner03IsFemale As Boolean
            Get
                Return _Partner03IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner03IsFemale = value Then
                    _Partner03IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner03IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner03IsFemale As Boolean

        Public Property Partner04Pokemon As Integer
            Get
                Return _Partner04Pokemon
            End Get
            Set(value As Integer)
                If Not _Partner04Pokemon = value Then
                    _Partner04Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner04Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner04Pokemon As Integer

        Public Property Partner04IsFemale As Boolean
            Get
                Return _Partner04IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner04IsFemale = value Then
                    _Partner04IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner04IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner04IsFemale As Boolean

        Public Property Partner05Pokemon As Integer
            Get
                Return _Partner05Pokemon
            End Get
            Set(value As Integer)
                If Not _Partner05Pokemon = value Then
                    _Partner05Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner05Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner05Pokemon As Integer

        Public Property Partner05IsFemale As Boolean
            Get
                Return _Partner05IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner05IsFemale = value Then
                    _Partner05IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner05IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner05IsFemale As Boolean

        Public Property Partner06Pokemon As Integer
            Get
                Return _Partner06Pokemon
            End Get
            Set(value As Integer)
                If Not _Partner06Pokemon = value Then
                    _Partner06Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner06Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner06Pokemon As Integer

        Public Property Partner06IsFemale As Boolean
            Get
                Return _Partner06IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner06IsFemale = value Then
                    _Partner06IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner06IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner06IsFemale As Boolean

        Public Property Partner07Pokemon As Integer
            Get
                Return _Partner07Pokemon
            End Get
            Set(value As Integer)
                If Not _Partner07Pokemon = value Then
                    _Partner07Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner07Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner07Pokemon As Integer

        Public Property Partner07IsFemale As Boolean
            Get
                Return _Partner07IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner07IsFemale = value Then
                    _Partner07IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner07IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner07IsFemale As Boolean

        Public Property Partner08Pokemon As Integer
            Get
                Return _Partner08Pokemon
            End Get
            Set(value As Integer)
                If Not _Partner08Pokemon = value Then
                    _Partner08Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner08Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner08Pokemon As Integer

        Public Property Partner08IsFemale As Boolean
            Get
                Return _Partner08IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner08IsFemale = value Then
                    _Partner08IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner08IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner08IsFemale As Boolean

        Public Property Partner09Pokemon As Integer
            Get
                Return _Partner09Pokemon
            End Get
            Set(value As Integer)
                If Not _Partner09Pokemon = value Then
                    _Partner09Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner09Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner09Pokemon As Integer

        Public Property Partner09IsFemale As Boolean
            Get
                Return _Partner09IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner09IsFemale = value Then
                    _Partner09IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner09IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner09IsFemale As Boolean

        Public Property Partner10Pokemon As Integer
            Get
                Return _Partner10Pokemon
            End Get
            Set(value As Integer)
                If Not _Partner10Pokemon = value Then
                    _Partner10Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner10Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner10Pokemon As Integer

        Public Property Partner10IsFemale As Boolean
            Get
                Return _Partner10IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner10IsFemale = value Then
                    _Partner10IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner10IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner10IsFemale As Boolean

        Public Property Partner11Pokemon As Integer
            Get
                Return _Partner11Pokemon
            End Get
            Set(value As Integer)
                If Not _Partner11Pokemon = value Then
                    _Partner11Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner11Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner11Pokemon As Integer

        Public Property Partner11IsFemale As Boolean
            Get
                Return _Partner11IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner11IsFemale = value Then
                    _Partner11IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner11IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner11IsFemale As Boolean

        Public Property Partner12Pokemon As Integer
            Get
                Return _Partner12Pokemon
            End Get
            Set(value As Integer)
                If Not _Partner12Pokemon = value Then
                    _Partner12Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner12Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner12Pokemon As Integer

        Public Property Partner12IsFemale As Boolean
            Get
                Return _Partner12IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner12IsFemale = value Then
                    _Partner12IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner12IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner12IsFemale As Boolean

        Public Property Partner13Pokemon As Integer
            Get
                Return _Partner13Pokemon
            End Get
            Set(value As Integer)
                If Not _Partner13Pokemon = value Then
                    _Partner13Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner13Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner13Pokemon As Integer

        Public Property Partner13IsFemale As Boolean
            Get
                Return _Partner13IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner13IsFemale = value Then
                    _Partner13IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner13IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner13IsFemale As Boolean

        Public Property Partner14Pokemon As Integer
            Get
                Return _Partner14Pokemon
            End Get
            Set(value As Integer)
                If Not _Partner14Pokemon = value Then
                    _Partner14Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner14Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner14Pokemon As Integer

        Public Property Partner14IsFemale As Boolean
            Get
                Return _Partner14IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner14IsFemale = value Then
                    _Partner14IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner14IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner14IsFemale As Boolean

        Public Property Partner15Pokemon As Integer
            Get
                Return _Partner15Pokemon
            End Get
            Set(value As Integer)
                If Not _Partner15Pokemon = value Then
                    _Partner15Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner15Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner15Pokemon As Integer

        Public Property Partner15IsFemale As Boolean
            Get
                Return _Partner15IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner15IsFemale = value Then
                    _Partner15IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner15IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner15IsFemale As Boolean

        Public Property Partner16Pokemon As Integer
            Get
                Return _Partner16Pokemon
            End Get
            Set(value As Integer)
                If Not _Partner16Pokemon = value Then
                    _Partner16Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner16Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner16Pokemon As Integer

        Public Property Partner16IsFemale As Boolean
            Get
                Return _Partner16IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner16IsFemale = value Then
                    _Partner16IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner16IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner16IsFemale As Boolean

        Public Property Partner17Pokemon As Integer
            Get
                Return _Partner17Pokemon
            End Get
            Set(value As Integer)
                If Not _Partner17Pokemon = value Then
                    _Partner17Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner17Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner17Pokemon As Integer

        Public Property Partner17IsFemale As Boolean
            Get
                Return _Partner17IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner17IsFemale = value Then
                    _Partner17IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner17IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner17IsFemale As Boolean

        Public Property Partner18Pokemon As Integer
            Get
                Return _Partner18Pokemon
            End Get
            Set(value As Integer)
                If Not _Partner18Pokemon = value Then
                    _Partner18Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner18Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner18Pokemon As Integer

        Public Property Partner18IsFemale As Boolean
            Get
                Return _Partner18IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner18IsFemale = value Then
                    _Partner18IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner18IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner18IsFemale As Boolean

        Public Property Partner19Pokemon As Integer
            Get
                Return _Partner19Pokemon
            End Get
            Set(value As Integer)
                If Not _Partner19Pokemon = value Then
                    _Partner19Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner19Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner19Pokemon As Integer

        Public Property Partner19IsFemale As Boolean
            Get
                Return _Partner19IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner19IsFemale = value Then
                    _Partner19IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner19IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner19IsFemale As Boolean

        Public Property Partner20Pokemon As Integer
            Get
                Return _Partner20Pokemon
            End Get
            Set(value As Integer)
                If Not _Partner20Pokemon = value Then
                    _Partner20Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner20Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner20Pokemon As Integer

        Public Property Partner20IsFemale As Boolean
            Get
                Return _Partner20IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner20IsFemale = value Then
                    _Partner20IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner20IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner20IsFemale As Boolean

        Public Property Partner21Pokemon As Integer
            Get
                Return _Partner21Pokemon
            End Get
            Set(value As Integer)
                If Not _Partner21Pokemon = value Then
                    _Partner21Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner21Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner21Pokemon As Integer

        Public Property Partner21IsFemale As Boolean
            Get
                Return _Partner21IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner21IsFemale = value Then
                    _Partner21IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner21IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner21IsFemale As Boolean

        Public Property HardyMalePokemon As Integer
            Get
                Return _HardyMalePokemon
            End Get
            Set(value As Integer)
                If Not _HardyMalePokemon = value Then
                    _HardyMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HardyMalePokemon)))
                End If
            End Set
        End Property
        Dim _HardyMalePokemon As Integer

        Public Property HardyMaleIsFemale As Boolean
            Get
                Return _HardyMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _HardyMaleIsFemale = value Then
                    _HardyMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HardyMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _HardyMaleIsFemale As Boolean

        Public Property HardyFemalePokemon As Integer
            Get
                Return _HardyFemalePokemon
            End Get
            Set(value As Integer)
                If Not _HardyFemalePokemon = value Then
                    _HardyFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HardyFemalePokemon)))
                End If
            End Set
        End Property
        Dim _HardyFemalePokemon As Integer

        Public Property HardyFemaleIsFemale As Boolean
            Get
                Return _HardyFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _HardyFemaleIsFemale = value Then
                    _HardyFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HardyFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _HardyFemaleIsFemale As Boolean

        Public Property DocileMalePokemon As Integer
            Get
                Return _DocileMalePokemon
            End Get
            Set(value As Integer)
                If Not _DocileMalePokemon = value Then
                    _DocileMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(DocileMalePokemon)))
                End If
            End Set
        End Property
        Dim _DocileMalePokemon As Integer

        Public Property DocileMaleIsFemale As Boolean
            Get
                Return _DocileMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _DocileMaleIsFemale = value Then
                    _DocileMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(DocileMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _DocileMaleIsFemale As Boolean

        Public Property DocileFemalePokemon As Integer
            Get
                Return _DocileFemalePokemon
            End Get
            Set(value As Integer)
                If Not _DocileFemalePokemon = value Then
                    _DocileFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(DocileFemalePokemon)))
                End If
            End Set
        End Property
        Dim _DocileFemalePokemon As Integer

        Public Property DocileFemaleIsFemale As Boolean
            Get
                Return _DocileFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _DocileFemaleIsFemale = value Then
                    _DocileFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(DocileFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _DocileFemaleIsFemale As Boolean

        Public Property BraveMalePokemon As Integer
            Get
                Return _BraveMalePokemon
            End Get
            Set(value As Integer)
                If Not _BraveMalePokemon = value Then
                    _BraveMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(BraveMalePokemon)))
                End If
            End Set
        End Property
        Dim _BraveMalePokemon As Integer

        Public Property BraveMaleIsFemale As Boolean
            Get
                Return _BraveMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _BraveMaleIsFemale = value Then
                    _BraveMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(BraveMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _BraveMaleIsFemale As Boolean

        Public Property BraveFemalePokemon As Integer
            Get
                Return _BraveFemalePokemon
            End Get
            Set(value As Integer)
                If Not _BraveFemalePokemon = value Then
                    _BraveFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(BraveFemalePokemon)))
                End If
            End Set
        End Property
        Dim _BraveFemalePokemon As Integer

        Public Property BraveFemaleIsFemale As Boolean
            Get
                Return _BraveFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _BraveFemaleIsFemale = value Then
                    _BraveFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(BraveFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _BraveFemaleIsFemale As Boolean

        Public Property JollyMalePokemon As Integer
            Get
                Return _JollyMalePokemon
            End Get
            Set(value As Integer)
                If Not _JollyMalePokemon = value Then
                    _JollyMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(JollyMalePokemon)))
                End If
            End Set
        End Property
        Dim _JollyMalePokemon As Integer

        Public Property JollyMaleIsFemale As Boolean
            Get
                Return _JollyMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _JollyMaleIsFemale = value Then
                    _JollyMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(JollyMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _JollyMaleIsFemale As Boolean

        Public Property JollyFemalePokemon As Integer
            Get
                Return _JollyFemalePokemon
            End Get
            Set(value As Integer)
                If Not _JollyFemalePokemon = value Then
                    _JollyFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(JollyFemalePokemon)))
                End If
            End Set
        End Property
        Dim _JollyFemalePokemon As Integer

        Public Property JollyFemaleIsFemale As Boolean
            Get
                Return _JollyFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _JollyFemaleIsFemale = value Then
                    _JollyFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(JollyFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _JollyFemaleIsFemale As Boolean

        Public Property ImpishMalePokemon As Integer
            Get
                Return _ImpishMalePokemon
            End Get
            Set(value As Integer)
                If Not _ImpishMalePokemon = value Then
                    _ImpishMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ImpishMalePokemon)))
                End If
            End Set
        End Property
        Dim _ImpishMalePokemon As Integer

        Public Property ImpishMaleIsFemale As Boolean
            Get
                Return _ImpishMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _ImpishMaleIsFemale = value Then
                    _ImpishMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ImpishMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _ImpishMaleIsFemale As Boolean

        Public Property ImpishFemalePokemon As Integer
            Get
                Return _ImpishFemalePokemon
            End Get
            Set(value As Integer)
                If Not _ImpishFemalePokemon = value Then
                    _ImpishFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ImpishFemalePokemon)))
                End If
            End Set
        End Property
        Dim _ImpishFemalePokemon As Integer

        Public Property ImpishFemaleIsFemale As Boolean
            Get
                Return _ImpishFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _ImpishFemaleIsFemale = value Then
                    _ImpishFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ImpishFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _ImpishFemaleIsFemale As Boolean

        Public Property NaiveMalePokemon As Integer
            Get
                Return _NaiveMalePokemon
            End Get
            Set(value As Integer)
                If Not _NaiveMalePokemon = value Then
                    _NaiveMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(NaiveMalePokemon)))
                End If
            End Set
        End Property
        Dim _NaiveMalePokemon As Integer

        Public Property NaiveMaleIsFemale As Boolean
            Get
                Return _NaiveMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _NaiveMaleIsFemale = value Then
                    _NaiveMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(NaiveMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _NaiveMaleIsFemale As Boolean

        Public Property NaiveFemalePokemon As Integer
            Get
                Return _NaiveFemalePokemon
            End Get
            Set(value As Integer)
                If Not _NaiveFemalePokemon = value Then
                    _NaiveFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(NaiveFemalePokemon)))
                End If
            End Set
        End Property
        Dim _NaiveFemalePokemon As Integer

        Public Property NaiveFemaleIsFemale As Boolean
            Get
                Return _NaiveFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _NaiveFemaleIsFemale = value Then
                    _NaiveFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(NaiveFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _NaiveFemaleIsFemale As Boolean

        Public Property TimidMalePokemon As Integer
            Get
                Return _TimidMalePokemon
            End Get
            Set(value As Integer)
                If Not _TimidMalePokemon = value Then
                    _TimidMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TimidMalePokemon)))
                End If
            End Set
        End Property
        Dim _TimidMalePokemon As Integer

        Public Property TimidMaleIsFemale As Boolean
            Get
                Return _TimidMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _TimidMaleIsFemale = value Then
                    _TimidMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TimidMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _TimidMaleIsFemale As Boolean

        Public Property TimidFemalePokemon As Integer
            Get
                Return _TimidFemalePokemon
            End Get
            Set(value As Integer)
                If Not _TimidFemalePokemon = value Then
                    _TimidFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TimidFemalePokemon)))
                End If
            End Set
        End Property
        Dim _TimidFemalePokemon As Integer

        Public Property TimidFemaleIsFemale As Boolean
            Get
                Return _TimidFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _TimidFemaleIsFemale = value Then
                    _TimidFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TimidFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _TimidFemaleIsFemale As Boolean

        Public Property HastyMalePokemon As Integer
            Get
                Return _HastyMalePokemon
            End Get
            Set(value As Integer)
                If Not _HastyMalePokemon = value Then
                    _HastyMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HastyMalePokemon)))
                End If
            End Set
        End Property
        Dim _HastyMalePokemon As Integer

        Public Property HastyMaleIsFemale As Boolean
            Get
                Return _HastyMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _HastyMaleIsFemale = value Then
                    _HastyMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HastyMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _HastyMaleIsFemale As Boolean

        Public Property HastyFemalePokemon As Integer
            Get
                Return _HastyFemalePokemon
            End Get
            Set(value As Integer)
                If Not _HastyFemalePokemon = value Then
                    _HastyFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HastyFemalePokemon)))
                End If
            End Set
        End Property
        Dim _HastyFemalePokemon As Integer

        Public Property HastyFemaleIsFemale As Boolean
            Get
                Return _HastyFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _HastyFemaleIsFemale = value Then
                    _HastyFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HastyFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _HastyFemaleIsFemale As Boolean

        Public Property SassyMalePokemon As Integer
            Get
                Return _SassyMalePokemon
            End Get
            Set(value As Integer)
                If Not _SassyMalePokemon = value Then
                    _SassyMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SassyMalePokemon)))
                End If
            End Set
        End Property
        Dim _SassyMalePokemon As Integer

        Public Property SassyMaleIsFemale As Boolean
            Get
                Return _SassyMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _SassyMaleIsFemale = value Then
                    _SassyMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SassyMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _SassyMaleIsFemale As Boolean

        Public Property SassyFemalePokemon As Integer
            Get
                Return _SassyFemalePokemon
            End Get
            Set(value As Integer)
                If Not _SassyFemalePokemon = value Then
                    _SassyFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SassyFemalePokemon)))
                End If
            End Set
        End Property
        Dim _SassyFemalePokemon As Integer

        Public Property SassyFemaleIsFemale As Boolean
            Get
                Return _SassyFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _SassyFemaleIsFemale = value Then
                    _SassyFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SassyFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _SassyFemaleIsFemale As Boolean

        Public Property CalmMalePokemon As Integer
            Get
                Return _CalmMalePokemon
            End Get
            Set(value As Integer)
                If Not _CalmMalePokemon = value Then
                    _CalmMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(CalmMalePokemon)))
                End If
            End Set
        End Property
        Dim _CalmMalePokemon As Integer

        Public Property CalmMaleIsFemale As Boolean
            Get
                Return _CalmMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _CalmMaleIsFemale = value Then
                    _CalmMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(CalmMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _CalmMaleIsFemale As Boolean

        Public Property CalmFemalePokemon As Integer
            Get
                Return _CalmFemalePokemon
            End Get
            Set(value As Integer)
                If Not _CalmFemalePokemon = value Then
                    _CalmFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(CalmFemalePokemon)))
                End If
            End Set
        End Property
        Dim _CalmFemalePokemon As Integer

        Public Property CalmFemaleIsFemale As Boolean
            Get
                Return _CalmFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _CalmFemaleIsFemale = value Then
                    _CalmFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(CalmFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _CalmFemaleIsFemale As Boolean

        Public Property RelaxedMalePokemon As Integer
            Get
                Return _RelaxedMalePokemon
            End Get
            Set(value As Integer)
                If Not _RelaxedMalePokemon = value Then
                    _RelaxedMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RelaxedMalePokemon)))
                End If
            End Set
        End Property
        Dim _RelaxedMalePokemon As Integer

        Public Property RelaxedMaleIsFemale As Boolean
            Get
                Return _RelaxedMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _RelaxedMaleIsFemale = value Then
                    _RelaxedMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RelaxedMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _RelaxedMaleIsFemale As Boolean

        Public Property RelaxedFemalePokemon As Integer
            Get
                Return _RelaxedFemalePokemon
            End Get
            Set(value As Integer)
                If Not _RelaxedFemalePokemon = value Then
                    _RelaxedFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RelaxedFemalePokemon)))
                End If
            End Set
        End Property
        Dim _RelaxedFemalePokemon As Integer

        Public Property RelaxedFemaleIsFemale As Boolean
            Get
                Return _RelaxedFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _RelaxedFemaleIsFemale = value Then
                    _RelaxedFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RelaxedFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _RelaxedFemaleIsFemale As Boolean

        Public Property LonelyMalePokemon As Integer
            Get
                Return _LonelyMalePokemon
            End Get
            Set(value As Integer)
                If Not _LonelyMalePokemon = value Then
                    _LonelyMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LonelyMalePokemon)))
                End If
            End Set
        End Property
        Dim _LonelyMalePokemon As Integer

        Public Property LonelyMaleIsFemale As Boolean
            Get
                Return _LonelyMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _LonelyMaleIsFemale = value Then
                    _LonelyMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LonelyMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _LonelyMaleIsFemale As Boolean

        Public Property LonelyFemalePokemon As Integer
            Get
                Return _LonelyFemalePokemon
            End Get
            Set(value As Integer)
                If Not _LonelyFemalePokemon = value Then
                    _LonelyFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LonelyFemalePokemon)))
                End If
            End Set
        End Property
        Dim _LonelyFemalePokemon As Integer

        Public Property LonelyFemaleIsFemale As Boolean
            Get
                Return _LonelyFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _LonelyFemaleIsFemale = value Then
                    _LonelyFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LonelyFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _LonelyFemaleIsFemale As Boolean

        Public Property QuirkyMalePokemon As Integer
            Get
                Return _QuirkyMalePokemon
            End Get
            Set(value As Integer)
                If Not _QuirkyMalePokemon = value Then
                    _QuirkyMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(QuirkyMalePokemon)))
                End If
            End Set
        End Property
        Dim _QuirkyMalePokemon As Integer

        Public Property QuirkyMaleIsFemale As Boolean
            Get
                Return _QuirkyMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _QuirkyMaleIsFemale = value Then
                    _QuirkyMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(QuirkyMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _QuirkyMaleIsFemale As Boolean

        Public Property QuirkyFemalePokemon As Integer
            Get
                Return _QuirkyFemalePokemon
            End Get
            Set(value As Integer)
                If Not _QuirkyFemalePokemon = value Then
                    _QuirkyFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(QuirkyFemalePokemon)))
                End If
            End Set
        End Property
        Dim _QuirkyFemalePokemon As Integer

        Public Property QuirkyFemaleIsFemale As Boolean
            Get
                Return _QuirkyFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _QuirkyFemaleIsFemale = value Then
                    _QuirkyFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(QuirkyFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _QuirkyFemaleIsFemale As Boolean

        Public Property QuietMalePokemon As Integer
            Get
                Return _QuietMalePokemon
            End Get
            Set(value As Integer)
                If Not _QuietMalePokemon = value Then
                    _QuietMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(QuietMalePokemon)))
                End If
            End Set
        End Property
        Dim _QuietMalePokemon As Integer

        Public Property QuietMaleIsFemale As Boolean
            Get
                Return _QuietMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _QuietMaleIsFemale = value Then
                    _QuietMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(QuietMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _QuietMaleIsFemale As Boolean

        Public Property QuietFemalePokemon As Integer
            Get
                Return _QuietFemalePokemon
            End Get
            Set(value As Integer)
                If Not _QuietFemalePokemon = value Then
                    _QuietFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(QuietFemalePokemon)))
                End If
            End Set
        End Property
        Dim _QuietFemalePokemon As Integer

        Public Property QuietFemaleIsFemale As Boolean
            Get
                Return _QuietFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _QuietFemaleIsFemale = value Then
                    _QuietFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(QuietFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _QuietFemaleIsFemale As Boolean

        Public Property RashMalePokemon As Integer
            Get
                Return _RashMalePokemon
            End Get
            Set(value As Integer)
                If Not _RashMalePokemon = value Then
                    _RashMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RashMalePokemon)))
                End If
            End Set
        End Property
        Dim _RashMalePokemon As Integer

        Public Property RashMaleIsFemale As Boolean
            Get
                Return _RashMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _RashMaleIsFemale = value Then
                    _RashMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RashMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _RashMaleIsFemale As Boolean

        Public Property RashFemalePokemon As Integer
            Get
                Return _RashFemalePokemon
            End Get
            Set(value As Integer)
                If Not _RashFemalePokemon = value Then
                    _RashFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RashFemalePokemon)))
                End If
            End Set
        End Property
        Dim _RashFemalePokemon As Integer

        Public Property RashFemaleIsFemale As Boolean
            Get
                Return _RashFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _RashFemaleIsFemale = value Then
                    _RashFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RashFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _RashFemaleIsFemale As Boolean

        Public Property BoldMalePokemon As Integer
            Get
                Return _BoldMalePokemon
            End Get
            Set(value As Integer)
                If Not _BoldMalePokemon = value Then
                    _BoldMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(BoldMalePokemon)))
                End If
            End Set
        End Property
        Dim _BoldMalePokemon As Integer

        Public Property BoldMaleIsFemale As Boolean
            Get
                Return _BoldMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _BoldMaleIsFemale = value Then
                    _BoldMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(BoldMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _BoldMaleIsFemale As Boolean

        Public Property BoldFemalePokemon As Integer
            Get
                Return _BoldFemalePokemon
            End Get
            Set(value As Integer)
                If Not _BoldFemalePokemon = value Then
                    _BoldFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(BoldFemalePokemon)))
                End If
            End Set
        End Property
        Dim _BoldFemalePokemon As Integer

        Public Property BoldFemaleIsFemale As Boolean
            Get
                Return _BoldFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _BoldFemaleIsFemale = value Then
                    _BoldFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(BoldFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _BoldFemaleIsFemale As Boolean

#End Region
    End Class
End Namespace
