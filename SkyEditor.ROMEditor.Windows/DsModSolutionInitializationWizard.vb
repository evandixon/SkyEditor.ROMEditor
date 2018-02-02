Imports System.ComponentModel
Imports System.IO
Imports SkyEditor.Core
Imports SkyEditor.Core.ConsoleCommands
Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Utilities
Imports SkyEditor.ROMEditor.Projects

Public Class DsModSolutionInitializationWizard
    Inherits Wizard

    Public Sub New(appViewModel As ApplicationViewModel, solution As DSModSolution)
        MyBase.New(appViewModel)
        Me.Solution = solution

        StepsInternal.Add(New IntroStep)
        StepsInternal.Add(New BaseRomStep(solution.GetBaseRomProject))
    End Sub

    Public Overrides ReadOnly Property Name As String
        Get
            Return My.Resources.Language.Solution_DsModSolution_InitializationWizard
        End Get
    End Property

    Public ReadOnly Property Solution As DSModSolution

    Public Class IntroStep
        Implements IWizardStepViewModel

        Protected Class IntroStepConsoleCommand
            Inherits ConsoleCommand

            Protected Overrides Sub Main(arguments() As String)
                Console.WriteLine(My.Resources.Language.Solution_DsModSolution_InitializationWizard_IntroStepDescription)
            End Sub

        End Class

        Public ReadOnly Property IsComplete As Boolean Implements IWizardStepViewModel.IsComplete
            Get
                Return True
            End Get
        End Property

        Public ReadOnly Property Name As String Implements INamed.Name
            Get
                Return My.Resources.Language.Solution_DsModSolution_InitializationWizard_IntroStepName
            End Get
        End Property

        Public Function GetConsoleCommand() As ConsoleCommand Implements IWizardStepViewModel.GetConsoleCommand
            Return New IntroStepConsoleCommand()
        End Function
    End Class

    Public Class BaseRomStep
        Implements IWizardStepViewModel
        Implements INotifyPropertyChanged

        Protected Class BaseRomStepConsoleCommand
            Inherits ConsoleCommand

            Public Sub New(baseRomStep As BaseRomStep)
                Me.BaseRomStep = baseRomStep
            End Sub

            Public Property BaseRomStep As BaseRomStep

            Public Overrides Async Function MainAsync(arguments() As String) As Task
                Console.WriteLine(My.Resources.Language.Solution_DsModSolution_InitializationWizard_BaseRomStep_Console_ChooseRomPath)

                Dim path = Console.ReadLine
                BaseRomStep.RomFilename = path

                'To-do: add retry if path is invalid
                'To-do: some kind of status report

                Await BaseRomStep.ExtractRom
            End Function
        End Class

        Public Sub New(baseRomProject As BaseRomProject)
            Me.BaseRomProject = baseRomProject
        End Sub

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Protected Property BaseRomProject As BaseRomProject
            Get
                Return _baseRomProject
            End Get
            Set(value As BaseRomProject)
                _baseRomProject = value
            End Set
        End Property
        Private WithEvents _baseRomProject As BaseRomProject

        Public ReadOnly Property IsComplete As Boolean Implements IWizardStepViewModel.IsComplete
            Get
                Return BaseRomProject.HasRom
            End Get
        End Property

        Public ReadOnly Property Name As String Implements INamed.Name
            Get
                Return My.Resources.Language.Solution_DsModSolution_InitializationWizard_BaseRomStepName
            End Get
        End Property

        Public Property RomFilename As String

        ''' <summary>
        ''' The percentage complete the ROM extraction is. Range is 0 to 1.
        ''' </summary>
        Public ReadOnly Property ExtractProgress As Single
            Get
                Return BaseRomProject.Progress
            End Get
        End Property

        Public ReadOnly Property IsExtractIndeterminate As Boolean
            Get
                Return BaseRomProject.IsIndeterminate
            End Get
        End Property

        ''' <summary>
        ''' Initializes the Base ROM project with the rom defined in the property <see cref="RomFilename"/>
        ''' </summary>
        Public Async Function ExtractRom() As Task
            If Not String.IsNullOrEmpty(RomFilename) AndAlso File.Exists(RomFilename) Then
                Await BaseRomProject.ImportRom(RomFilename)
            End If
        End Function

        Public Function GetConsoleCommand() As ConsoleCommand Implements IWizardStepViewModel.GetConsoleCommand
            Return New BaseRomStepConsoleCommand(Me)
        End Function

        Private Sub _baseRomProject_ProgressChanged(sender As Object, e As ProgressReportedEventArgs) Handles _baseRomProject.ProgressChanged
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ExtractProgress)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(IsExtractIndeterminate)))
        End Sub

    End Class

    Public Class ModpackDetailsStep

    End Class
End Class
