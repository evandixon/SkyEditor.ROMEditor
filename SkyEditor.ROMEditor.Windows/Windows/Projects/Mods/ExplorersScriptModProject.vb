Imports SkyEditor.Core.Projects

Namespace Windows.Projects.Mods
    Public Class ExplorersScriptModProject
        Inherits GenericModProject
        Public Overrides Function GetRawFilesDir() As String
            Return GetRootDirectory()
        End Function
        Public Overrides Function GetFilesToCopy(Solution As Solution, BaseRomProjectName As String) As IEnumerable(Of String)
            Return {IO.Path.Combine("data", "script")}
        End Function
        Public Overrides Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {GameStrings.SkyCode}
        End Function

        Protected Overrides Async Function Initialize() As Task
            Await MyBase.Initialize

            Dim projectDir = GetRootDirectory()
            Dim sourceDir = GetRawFilesDir()

            Dim scriptFiles = IO.Directory.GetFiles(IO.Path.Combine(sourceDir, "Data", "SCRIPT"), "*", IO.SearchOption.AllDirectories)
            For Each item In scriptFiles
                Me.AddExistingFile(IO.Path.GetDirectoryName(item).Replace(projectDir, ""), item, CurrentPluginManager.CurrentIOProvider)
            Next

            Me.BuildProgress = 1
            Me.IsBuildProgressIndeterminate = False
            Me.BuildStatusMessage = My.Resources.Language.Complete
        End Function

    End Class

End Namespace
