Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Text.RegularExpressions
Imports SkyEditor.Core.Projects
Imports SkyEditor.Core.Utilities
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD.Pokemon
Imports SkyEditor.ROMEditor.Projects

Namespace MysteryDungeon.PSMD.Projects
    Public Class PsmdPortraitProject
        Inherits GenericModProject

        Public Overrides Async Function Initialize() As Task
            Await MyBase.Initialize

            Me.Message = My.Resources.Language.LoadingExtractingPortraits
            Me.Progress = 0
            Me.IsIndeterminate = False

            Dim provider = CurrentPluginManager.CurrentIOProvider
            Dim faceFarc As New Farc
            Await faceFarc.OpenFile(Path.Combine(Me.GetRawFilesDir, "romfs", "face_graphic.bin"), provider)

            Dim unmatched = faceFarc.GetFiles("/", "*", True).Where(Function(x) x.ToLower <> x).ToList()

            Dim onProgressed = Sub(sender As Object, e As ProgressReportedEventArgs)
                                   Me.Message = My.Resources.Language.LoadingExtractingPortraits
                                   Me.Progress = e.Progress
                                   Me.IsIndeterminate = False
                               End Sub

            Dim filenameRegex As New Regex("(([a-z0-9]|_)+)(_f)?(_hanten)?(_r)?_([0-9]{2})", RegexOptions.Compiled)
            Dim directoryCreateLock As New Object
            Dim a = New AsyncFor
            AddHandler a.ProgressChanged, onProgressed
            Await a.RunForEach(faceFarc.GetFiles("/", "*", True),
                               Sub(portrait As String)
                                   Dim match = filenameRegex.Match(Path.GetFileNameWithoutExtension(portrait))
                                   Dim outputPath As String

                                   If match.Success Then
                                       outputPath = Path.Combine(Me.GetRootDirectory, match.Groups(1).Value, Path.GetFileNameWithoutExtension(portrait) & ".png")
                                   Else
                                       outputPath = Path.Combine(Me.GetRootDirectory, "_Unknown", Path.GetFileNameWithoutExtension(portrait) & ".png")
                                   End If

                                   'Create directory if it doesn't exist
                                   If Not provider.DirectoryExists(Path.GetDirectoryName(outputPath)) Then
                                       SyncLock directoryCreateLock
                                           If Not provider.DirectoryExists(Path.GetDirectoryName(outputPath)) Then 'Check again in case of race condition
                                               provider.CreateDirectory(Path.GetDirectoryName(outputPath))
                                           End If
                                       End SyncLock
                                   End If

                                   Dim rawData = faceFarc.ReadAllBytes(portrait)
                                   Using bitmap = PmdGraphics.ReadPortrait(rawData)
                                       bitmap.Save(outputPath, ImageFormat.Png)
                                   End Using
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
            Await a.RunForEach(Directory.GetFiles(Me.GetRootDirectory, "*.png", SearchOption.AllDirectories),
                               Sub(portrait As String)
                                   Using img As New Bitmap(portrait)
                                       f.WriteAllBytes(Path.GetFileNameWithoutExtension(portrait) & ".bin", PmdGraphics.SavePortrait(img))
                                   End Using
                               End Sub)
            RemoveHandler a.ProgressChanged, onProgressed

            Await f.Save(Path.Combine(Me.GetRawFilesDir, "romfs", "face_graphic.bin"), CurrentPluginManager.CurrentIOProvider)

            Me.IsCompleted = True

            Await MyBase.Build
        End Function

        Public Overrides Function GetFilesToCopy(Solution As Solution, BaseRomProjectName As String) As IEnumerable(Of String)
            Return {Path.Combine("romfs", "face_graphic.bin"),
                Path.Combine("romfs", "message_debug.bin"),
                Path.Combine("romfs", "message_debug.lst"),
                Path.Combine("romfs", "pokemon_graphics_database.bin"),
                Path.Combine("romfs", "pokemon", "pokemon_actor_data_info.bin")}
        End Function
    End Class

End Namespace
