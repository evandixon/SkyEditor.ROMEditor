Imports PPMDU
Imports SkyEditor.ROMEditor.Projects
Imports SkyEditor.ROMEditor.Windows

Namespace MysteryDungeon.Explorers.Projects
    Public Class PpmduStatsUtilProject
        Inherits GenericModProject

        Public Overrides Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {GameStrings.SkyCode}
        End Function

        Public Overrides Async Function Initialize() As Task
            Await MyBase.Initialize()

            Dim outputDir = IO.Path.Combine(Me.GetRootDirectory)

            Me.IsIndeterminate = True
            Me.Message = My.Resources.Language.Loading

            Using external As New UtilityManager
                Dim options As New StatsUtilOptions
                options.IsImport = False
                options.EnablePokemon = True
                options.EnableMoves = True
                options.EnableItems = True
                options.EnableScripts = True
                Await external.RunStatsUtil(GetRawFilesDir, outputDir, options)
            End Using

            'Add files to project
            For Each item In IO.Directory.GetFiles(outputDir, "*.xml", IO.SearchOption.AllDirectories)
                Me.AddExistingFile(IO.Path.GetDirectoryName(item).Replace(outputDir, ""), item, CurrentPluginManager.CurrentIOProvider)
            Next

            Me.Message = My.Resources.Language.Complete
            Me.Progress = 1
            Me.IsIndeterminate = False
        End Function

        Protected Overrides Async Function DoBuild() As Task
            Dim outputDir = IO.Path.Combine(Me.GetRootDirectory)

            Using external As New UtilityManager
                Dim options As New StatsUtilOptions
                options.IsImport = True
                options.EnablePokemon = True
                options.EnableMoves = True
                options.EnableItems = True
                options.EnableScripts = True
                Await external.RunStatsUtil(GetRawFilesDir, outputDir, options)
            End Using

            Await MyBase.DoBuild
        End Function
    End Class
End Namespace

