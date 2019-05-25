Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Text.RegularExpressions
Imports SkyEditor.Core
Imports SkyEditor.Core.Projects
Imports SkyEditor.Core.Utilities
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD.Pokemon
Imports SkyEditor.ROMEditor.Projects
Imports SkyEditor.Utilities.AsyncFor

Namespace MysteryDungeon.PSMD.Projects
    Public Class PsmdPortraitProject
        Inherits GenericModProject

        Public Overrides Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {GameStrings.PSMDCode}
        End Function

        Public Overrides Async Function Initialize() As Task
            Await MyBase.Initialize

            Me.Message = My.Resources.Language.LoadingExtractingPortraits
            Me.Progress = 0
            Me.IsIndeterminate = False

            Dim provider = CurrentPluginManager.CurrentFileSystem
            Dim faceFarc As New Farc
            Await faceFarc.OpenFile(Path.Combine(Me.GetRawFilesDir, "romfs", "face_graphic.bin"), provider)

            'Dim unmatched = faceFarc.GetFiles("/", "*", True).Where(Function(x) x.ToLower <> x).ToList()

            Dim progressToken As New ProgressReportToken()
            'Todo: use the progress token
            Await FarcExtensions.ExtractPortraits(faceFarc, Me.GetRootDirectory, provider, progressToken)

            'Dim onProgressed = Sub(sender As Object, e As ProgressReportedEventArgs)
            '                       Me.Message = My.Resources.Language.LoadingExtractingPortraits
            '                       Me.Progress = e.Progress
            '                       Me.IsIndeterminate = False
            '                   End Sub


            Me.IsCompleted = True
        End Function

        Public Overrides Async Function Build() As Task
            Dim onProgressed = Sub(sender As Object, e As ProgressReportedEventArgs)
                                   Me.Message = My.Resources.Language.LoadingBuildingPortraits
                                   Me.Progress = e.Progress
                                   Me.IsIndeterminate = False
                               End Sub

            Dim f As New Farc(5, False)

            Dim a = New AsyncFor
            AddHandler a.ProgressChanged, onProgressed
            Await a.RunForEach(Directory.GetFiles(Me.GetRootDirectory, "*.png", SearchOption.AllDirectories),
                               Sub(portrait As String)
                                   Using img As New Bitmap(portrait)
                                       f.WriteAllBytes(Path.GetFileNameWithoutExtension(portrait) & ".bin", PmdGraphics.SavePortrait(img))
                                   End Using
                               End Sub)
            RemoveHandler a.ProgressChanged, onProgressed

            Await f.Save(Path.Combine(Me.GetRawFilesDir, "romfs", "face_graphic.bin"), CurrentPluginManager.CurrentFileSystem)

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
