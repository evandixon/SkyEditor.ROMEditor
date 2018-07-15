Imports System.Reflection
Imports SkyEditor.Core
Imports SkyEditor.Core.UI
Imports SkyEditor.ROMEditor.Projects
Imports SkyEditor.UI.WPF.ViewModels.Projects

Namespace MenuActions
    Public Class PsmdSoundtrackMenuAction
        Inherits MenuAction

        Public Sub New(appViewModel As ApplicationViewModel)
            MyBase.New({My.Resources.Language.MenuUtilities, My.Resources.Language.MenuUtilitiesExportSoundtrack})
            SortOrder = 4.1
            IsContextBased = True

            If appViewModel Is Nothing Then
                Throw New ArgumentNullException(NameOf(appViewModel))
            End If

            CurrentApplicationViewModel = appViewModel
        End Sub

        Protected Property CurrentApplicationViewModel As ApplicationViewModel

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of TypeInfo)
            Return {GetType(SolutionHeiarchyItemViewModel).GetTypeInfo}
        End Function

        Public Overrides Async Function SupportsObject(obj As Object) As Task(Of Boolean)
            If TypeOf obj Is SolutionHeiarchyItemViewModel Then
                Dim converter As New DotNet3dsSoundtrackConverter.SoundtrackConverter
                Dim vm As SolutionHeiarchyItemViewModel = obj
                Return Not vm.IsDirectory AndAlso TypeOf vm.GetNodeProject Is BaseRomProject AndAlso Await converter.CanConvert(DirectCast(vm.GetNodeProject, BaseRomProject).GetRawFilesDir())
            Else
                Return False
            End If
        End Function

        Public Overrides Sub DoAction(targets As IEnumerable(Of Object))
            For Each project As SolutionHeiarchyItemViewModel In targets
                Dim c As New DotNet3dsSoundtrackConverter.SoundtrackConverter
                Dim sourceDir = DirectCast(project.GetNodeProject, BaseRomProject).GetRawFilesDir
                Dim outputDir = System.IO.Path.Combine(project.GetNodeProject.GetRootDirectory, "Soundtrack")
                c.Convert(sourceDir, outputDir)
                CurrentApplicationViewModel.ShowLoading(c)
            Next
        End Sub
    End Class

End Namespace
