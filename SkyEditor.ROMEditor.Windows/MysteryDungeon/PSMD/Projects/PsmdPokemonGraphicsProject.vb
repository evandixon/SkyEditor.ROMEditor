Imports System.IO
Imports System.Text.RegularExpressions
Imports SkyEditor.Core.Projects
Imports SkyEditor.Core.Utilities
Imports SkyEditor.ROMEditor.Projects

Namespace MysteryDungeon.PSMD.Projects
    Public Class PsmdPokemonGraphicsProject
        Inherits GenericModProject

        Public Overrides Async Function Initialize() As Task
            Await MyBase.Initialize

            Me.Message = My.Resources.Language.FarcLoadingExtract
            Me.Progress = 0
            Me.IsIndeterminate = False

            Dim provider = CurrentPluginManager.CurrentIOProvider
            Dim graphicFarc As New Farc
            Await graphicFarc.OpenFile(Path.Combine(Me.GetRawFilesDir, "romfs", "pokemon_graphic.bin"), provider)

            Dim onProgressed = Sub(sender As Object, e As ProgressReportedEventArgs)
                                   Me.Message = My.Resources.Language.FarcLoadingExtract
                                   Me.Progress = e.Progress
                                   Me.IsIndeterminate = False
                               End Sub

            Dim filenameRegex As New Regex("(([a-z0-9]|_)+?)(__.*)?\..+", RegexOptions.Compiled)
            Dim directoryCreateLock As New Object
            Dim a = New AsyncFor
            AddHandler a.ProgressChanged, onProgressed
            Await a.RunForEach(graphicFarc.GetFiles("/", "*", True),
                               Sub(portrait As String)
                                   Dim match = filenameRegex.Match(Path.GetFileName(portrait))
                                   Dim outputPath As String

                                   If match.Success Then
                                       outputPath = Path.Combine(Me.GetRootDirectory, "Models", match.Groups(1).Value, Path.GetFileName(portrait))
                                   Else
                                       outputPath = Path.Combine(Me.GetRootDirectory, "Models", "_Unknown", Path.GetFileName(portrait))
                                   End If

                                   'Create directory if it doesn't exist
                                   If Not provider.DirectoryExists(Path.GetDirectoryName(outputPath)) Then
                                       SyncLock directoryCreateLock
                                           If Not provider.DirectoryExists(Path.GetDirectoryName(outputPath)) Then 'Check again in case of race condition
                                               provider.CreateDirectory(Path.GetDirectoryName(outputPath))
                                           End If
                                       End SyncLock
                                   End If

                                   Dim rawData = graphicFarc.ReadAllBytes(portrait)
                                   File.WriteAllBytes(outputPath, rawData)
                               End Sub)
            RemoveHandler a.ProgressChanged, onProgressed

            Me.IsCompleted = True
        End Function

        Public Overrides Async Function Build() As Task
            Dim onProgressed = Sub(sender As Object, e As ProgressReportedEventArgs)
                                   Me.Message = My.Resources.Language.LoadingBuildingPortraits
                                   Me.Progress = e.Progress
                                   Me.IsIndeterminate = False
                               End Sub

            Dim f As New Farc()
            f.CreateFile()

            Dim a = New AsyncFor
            AddHandler a.ProgressChanged, onProgressed
            Await a.RunForEach(Directory.GetFiles(Path.Combine(Me.GetRootDirectory, "Models"), "*", SearchOption.AllDirectories),
                               Sub(model As String)
                                   f.WriteAllBytes("/" & Path.GetFileName(model), File.ReadAllBytes(model))
                               End Sub)
            RemoveHandler a.ProgressChanged, onProgressed

            Await f.Save(Path.Combine(Me.GetRawFilesDir, "romfs", "pokemon_graphic.bin"), CurrentPluginManager.CurrentIOProvider)

            Me.IsCompleted = True

            Await MyBase.Build
        End Function

        Public Overrides Function GetFilesToCopy(Solution As Solution, BaseRomProjectName As String) As IEnumerable(Of String)
            Return {Path.Combine("romfs", "pokemon_graphic.bin"),
                Path.Combine("romfs", "pokemon_graphics_database.bin")}
        End Function
    End Class
End Namespace
