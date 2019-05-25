Imports System.IO
Imports SkyEditor.Core.Projects
Imports SkyEditor.Core.Utilities
Imports SkyEditor.ROMEditor.Projects
Imports SkyEditor.Utilities.AsyncFor

Namespace MysteryDungeon.Explorers.Projects
    Public Class SkyBackModProject
        Inherits GenericModProject

        Public Overrides Function GetFilesToCopy(Solution As Solution, BaseRomProjectName As String) As IEnumerable(Of String)
            Return {Path.Combine("data", "BACK")}
        End Function

        Public Overrides Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {GameStrings.SkyCode}
        End Function

        Public Overrides Async Function Initialize() As Task
            Await MyBase.Initialize

            'Stop loading
            Me.Progress = 0
            Me.IsIndeterminate = False
            Me.Message = My.Resources.Language.LoadingConvertingBackgrounds

            Dim projectDir = GetRootDirectory()
            Dim sourceDir = GetRawFilesDir()

            Dim BACKdir As String = Path.Combine(projectDir, "Backgrounds")
            Dim backFiles = Directory.GetFiles(Path.Combine(sourceDir, "Data", "BACK"), "*.bgp")
            Dim f As New AsyncFor
            AddHandler f.ProgressChanged, Sub(sender As Object, e As ProgressReportedEventArgs)
                                              Me.Progress = e.Progress
                                          End Sub
            Await f.RunForEach(backFiles,
                               Async Function(Item As String) As Task
                                   Using b As New BGP
                                       Await b.OpenFile(Item, CurrentPluginManager.CurrentFileSystem)

                                       Dim newFilename = Path.Combine(BACKdir, Path.GetFileNameWithoutExtension(Item) & ".png")
                                       If Not Directory.Exists(Path.GetDirectoryName(newFilename)) Then
                                           Directory.CreateDirectory(Path.GetDirectoryName(newFilename))
                                       End If

                                       b.GetImage.Save(newFilename, Drawing.Imaging.ImageFormat.Png)
                                       File.Copy(newFilename, newFilename & ".original")

                                       Me.AddExistingFileToPath("/" & Path.GetFileName(newFilename), newFilename, Nothing, CurrentPluginManager.CurrentFileSystem)
                                   End Using
                               End Function)

            'Stop loading
            Me.Progress = 1
            Me.IsIndeterminate = False
            Me.Message = My.Resources.Language.Complete
        End Function

        Public Overrides Async Function Build() As Task
            'Convert BACK
            Dim projectDir = GetRootDirectory()
            Dim rawDir = GetRawFilesDir()
            If Directory.Exists(Path.Combine(projectDir, "Backgrounds")) Then
                For Each background In Directory.GetFiles(Path.Combine(projectDir, "Backgrounds"), "*.png")
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
                        Using bitmap = Drawing.Bitmap.FromFile(background)
                            Using img = BGP.ConvertFromBitmap(bitmap)
                                Await img.Save(Path.Combine(rawDir, "Data", "BACK", Path.GetFileNameWithoutExtension(background) & ".bgp"), CurrentPluginManager.CurrentFileSystem)
                            End Using
                        End Using
                    End If

                Next
            End If

            Await MyBase.Build
        End Function
    End Class
End Namespace

