Imports System.IO
Imports SkyEditor.Core.Projects
Imports SkyEditor.Core.Utilities
Imports SkyEditor.ROMEditor.Projects
Imports SkyEditor.Utilities.AsyncFor

Namespace MysteryDungeon.PSMD.Projects
    Public Class GtiImage2dProject
        Inherits GenericModProject

        Public Overrides Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {GameStrings.GTICode}
        End Function

        Public Overrides Function GetFilesToCopy(Solution As Solution, BaseRomProjectName As String) As IEnumerable(Of String)
            Return {Path.Combine("romfs", "bg"), Path.Combine("romfs", "font"), Path.Combine("romfs", "image_2d")}
        End Function

        Public Overrides Async Function Initialize() As Task
            Await MyBase.Initialize
            Dim rawFilesDir = GetRawFilesDir()
            Dim backDir = GetRootDirectory()

            Me.Message = My.Resources.Language.LoadingConvertingBackgrounds
            Me.IsIndeterminate = False
            Me.Progress = 0

            Dim backFiles = Directory.GetFiles(Path.Combine(rawFilesDir, "romfs"), "*.img", SearchOption.AllDirectories)
            Dim f As New AsyncFor
            AddHandler f.ProgressChanged, Sub(sender As Object, e As ProgressReportedEventArgs)
                                              Me.Progress = e.Progress
                                          End Sub
            Await f.RunForEach(backFiles,
                               Async Function(Item As String) As Task
                                   Using b As New CteImage
                                       Await b.OpenFile(Item, CurrentPluginManager.CurrentFileSystem)
                                       Dim image = b.ContainedImage
                                       Dim newFilename = Path.Combine(backDir, Path.GetDirectoryName(Item).Replace(rawFilesDir, "").Replace("\romfs", "").Trim("\"), Path.GetFileNameWithoutExtension(Item) & ".png")
                                       If Not Directory.Exists(Path.GetDirectoryName(newFilename)) Then
                                           Directory.CreateDirectory(Path.GetDirectoryName(newFilename))
                                       End If
                                       image.Save(newFilename, Drawing.Imaging.ImageFormat.Png)
                                       File.Copy(newFilename, newFilename & ".original")

                                       Dim internalDir = Path.GetDirectoryName(Item).Replace(rawFilesDir, "").Replace("\romfs", "")
                                       Me.AddExistingFile(internalDir, newFilename, CurrentPluginManager.CurrentFileSystem)
                                   End Using
                               End Function)

            Me.Progress = 1
            Me.Message = My.Resources.Language.Complete
        End Function

        Public Overrides Async Function Build() As Task
            'Convert BACK
            Dim sourceDir = GetRootDirectory()
            Dim rawFilesDir = GetRawFilesDir()

            For Each background In Directory.GetFiles(GetRootDirectory, "*.png", SearchOption.AllDirectories)
                Dim includeInPack As Boolean

                If File.Exists(background & ".original") Then
                    Using bmp As New FileStream(background, FileMode.Open)
                        Using orig As New FileStream(background & ".original", FileMode.Open)
                            Dim equal As Boolean = (bmp.Length = orig.Length)
                            While equal
                                Dim b = bmp.ReadByte
                                Dim o = orig.ReadByte
                                equal = (b = o)
                                If b = -1 OrElse o = -1 Then
                                    Exit While
                                End If
                            End While
                            includeInPack = Not equal
                        End Using
                    End Using
                Else
                    includeInPack = True
                End If

                If includeInPack Then
                    Dim img As New CteImage
                    Await img.OpenFile(Path.Combine(rawFilesDir, "romfs", Path.GetDirectoryName(background).Replace(sourceDir, ""), Path.GetFileNameWithoutExtension(background) & ".img"), CurrentPluginManager.CurrentFileSystem)
                    img.ContainedImage = Drawing.Image.FromFile(background)
                    Await img.Save(CurrentPluginManager.CurrentFileSystem)
                    img.Dispose()
                End If

            Next
            Await MyBase.Build
        End Function
    End Class

End Namespace
