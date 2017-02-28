Imports SkyEditor.Core.Projects
Imports SkyEditor.Core.Utilities
Imports SkyEditor.ROMEditor.Projects

Namespace MysteryDungeon.Explorers.Projects
    Public Class SkyBackModProject
        Inherits GenericModProject

        Public Overrides Function GetFilesToCopy(Solution As Solution, BaseRomProjectName As String) As IEnumerable(Of String)
            Return {IO.Path.Combine("data", "BACK")}
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

            Dim BACKdir As String = IO.Path.Combine(projectDir, "Backgrounds")
            Dim backFiles = IO.Directory.GetFiles(IO.Path.Combine(sourceDir, "Data", "BACK"), "*.bgp")
            Dim f As New AsyncFor
            AddHandler f.ProgressChanged, Sub(sender As Object, e As ProgressReportedEventArgs)
                                              Me.Progress = e.Progress
                                          End Sub
            Await f.RunForEach(backFiles,
                               Async Function(Item As String) As Task
                                   Using b As New BGP
                                       Await b.OpenFile(Item, CurrentPluginManager.CurrentIOProvider)

                                       Dim newFilename = IO.Path.Combine(BACKdir, IO.Path.GetFileNameWithoutExtension(Item) & ".bmp")
                                       If Not IO.Directory.Exists(IO.Path.GetDirectoryName(newFilename)) Then
                                           IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(newFilename))
                                       End If

                                       b.GetImage.Save(newFilename, Drawing.Imaging.ImageFormat.Bmp)
                                       IO.File.Copy(newFilename, newFilename & ".original")

                                       Me.AddExistingFileToPath("/" & IO.Path.GetFileName(newFilename), newFilename, Nothing, CurrentPluginManager.CurrentIOProvider)
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
            If IO.Directory.Exists(IO.Path.Combine(projectDir, "Backgrounds")) Then
                For Each background In IO.Directory.GetFiles(IO.Path.Combine(projectDir, "Backgrounds"), "*.bmp")
                    Dim includeInPack As Boolean

                    If IO.File.Exists(background & ".original") Then
                        Using bmp As New IO.FileStream(background, IO.FileMode.Open)
                            Using orig As New IO.FileStream(background & ".original", IO.FileMode.Open)
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
                        Using img = BGP.ConvertFromBitmap(Drawing.Bitmap.FromFile(background))
                            Await img.Save(IO.Path.Combine(rawDir, "Data", "BACK", IO.Path.GetFileNameWithoutExtension(background) & ".bgp"), CurrentPluginManager.CurrentIOProvider)
                        End Using
                    End If

                Next
            End If

            Await MyBase.Build
        End Function
    End Class
End Namespace

