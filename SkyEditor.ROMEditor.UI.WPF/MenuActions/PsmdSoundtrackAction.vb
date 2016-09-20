Imports System.Reflection
Imports SkyEditor.Core.UI
Imports SkyEditor.ROMEditor.Windows.MysteryDungeon.PSMD
Imports SkyEditor.ROMEditor.Windows.Projects
Imports SkyEditor.UI.WPF
Imports SkyEditor.UI.WPF.ViewModels.Projects

Namespace MenuActions
    Public Class PsmdSoundtrackMenuAction
        Inherits MenuAction

        Public Overrides Function SupportedTypes() As IEnumerable(Of TypeInfo)
            Return {GetType(SolutionHeiarchyItemViewModel).GetTypeInfo}
        End Function

        Public Overrides Function SupportsObject(Obj As Object) As Boolean
            If TypeOf Obj Is SolutionHeiarchyItemViewModel Then
                Dim vm As SolutionHeiarchyItemViewModel = Obj
                Return Not vm.IsDirectory AndAlso TypeOf vm.GetNodeProject Is BaseRomProject AndAlso PSMDSoundtrackConverter.SupportsProject(vm.GetNodeProject)
            Else
                Return False
            End If
        End Function

        Public Overrides Async Sub DoAction(Targets As IEnumerable(Of Object))
            For Each project As SolutionHeiarchyItemViewModel In Targets
                Await PSMDSoundtrackConverter.Convert(project.GetNodeProject)
            Next
        End Sub

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuUtilities, My.Resources.Language.MenuUtilitiesExportSoundtrack})
            SortOrder = 4.1
            IsContextBased = True
        End Sub
    End Class

End Namespace
