Imports SkyEditor.Core.IO

Namespace Projects
    Public Class PpmduStatsUtilProject
        Inherits GenericModProject

        Public Overrides Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {GameStrings.SkyCode}
        End Function

        Protected Overrides Async Function Initialize() As Task
            Await MyBase.Initialize()

            Dim outputDir = IO.Path.Combine(Me.GetRootDirectory)
            Using external As New ExternalProgramManager
                Await external.RunPPMDStatsUtil($"-e -scripts -romroot ""{Me.GetRawFilesDir}"" ""{outputDir}""")
            End Using

            'Add files to project
            Dim toAdd As New List(Of AddExistingFileBatchOperation)
            For Each item In IO.Directory.GetFiles(outputDir, "*.xml", IO.SearchOption.AllDirectories)
                toAdd.Add(New AddExistingFileBatchOperation With {.ActualFilename = item, .ParentPath = IO.Path.GetDirectoryName(item).Replace(outputDir, "")})
            Next
            Await Me.RecreateRootWithExistingFiles(toAdd, CurrentPluginManager.CurrentIOProvider)
        End Function

        Protected Overrides Async Function DoBuild() As Task
            Dim outputDir = IO.Path.Combine(Me.GetRootDirectory)
            Using external As New ExternalProgramManager
                Await external.RunPPMDStatsUtil($"-i -scripts -romroot ""{Me.GetRawFilesDir}"" ""{outputDir}""")
            End Using
            Await MyBase.DoBuild
        End Function
    End Class
End Namespace

