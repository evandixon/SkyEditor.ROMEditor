Imports System.IO
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

            Dim outputDir = Path.Combine(Me.GetRootDirectory)

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
            For Each item In Directory.GetFiles(outputDir, "*.xml", SearchOption.AllDirectories)
                Me.AddExistingFile(Path.GetDirectoryName(item).Replace(outputDir, ""), item, CurrentPluginManager.CurrentFileSystem)
            Next

            Me.Message = My.Resources.Language.Complete
            Me.Progress = 1
            Me.IsIndeterminate = False
        End Function

        Public Overrides Async Function Build() As Task
            Dim outputDir = Path.Combine(Me.GetRootDirectory)

            Using external As New UtilityManager
                Dim options As New StatsUtilOptions
                options.IsImport = True
                options.EnablePokemon = True
                options.EnableMoves = True
                options.EnableItems = True
                options.EnableScripts = True
                Await external.RunStatsUtil(GetRawFilesDir, outputDir, options)
            End Using

            Await MyBase.Build
        End Function
    End Class
End Namespace

