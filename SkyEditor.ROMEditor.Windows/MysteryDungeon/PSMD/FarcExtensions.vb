Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Utilities

Namespace MysteryDungeon.PSMD
    Public Module FarcExtensions

        'Can't define this as an extension method due to conflicting definitions :(
        '<Extension>
        Public Async Function ExtractPortraits(farc As Farc, outputDirectory As String, ioProvider As IIOProvider, Optional progressToken As ProgressReportToken = Nothing) As Task
            Dim onProgressed = Sub(sender As Object, e As ProgressReportedEventArgs)
                                   If progressToken IsNot Nothing Then
                                       progressToken.Message = My.Resources.Language.LoadingExtractingPortraits
                                       progressToken.Progress = e.Progress
                                       progressToken.IsIndeterminate = False
                                   End If
                               End Sub

            Dim filenameRegex As New Regex("(([a-z0-9]|_)+)(_f)?(_hanten)?(_r)?_([0-9]{2})", RegexOptions.Compiled)
            Dim directoryCreateLock As New Object
            Dim a = New AsyncFor
            AddHandler a.ProgressChanged, onProgressed
            Await a.RunForEach(farc.GetFiles("/", "*", True),
                               Sub(portrait As String)
                                   Dim match = filenameRegex.Match(Path.GetFileNameWithoutExtension(portrait))
                                   Dim outputPath As String

                                   If match.Success Then
                                       outputPath = Path.Combine(outputDirectory, match.Groups(1).Value, Path.GetFileNameWithoutExtension(portrait) & ".png")
                                   Else
                                       outputPath = Path.Combine(outputDirectory, "_Unknown", Path.GetFileNameWithoutExtension(portrait) & ".png")
                                   End If

                                   'Create directory if it doesn't exist
                                   If Not ioProvider.DirectoryExists(Path.GetDirectoryName(outputPath)) Then
                                       SyncLock directoryCreateLock
                                           If Not ioProvider.DirectoryExists(Path.GetDirectoryName(outputPath)) Then 'Check again in case of race condition
                                               ioProvider.CreateDirectory(Path.GetDirectoryName(outputPath))
                                           End If
                                       End SyncLock
                                   End If

                                   Dim rawData = farc.ReadAllBytes(portrait)
                                   Using bitmap = PmdGraphics.ReadPortrait(rawData)
                                       bitmap.Save(outputPath, ImageFormat.Png)
                                   End Using
                               End Sub)
            RemoveHandler a.ProgressChanged, onProgressed

            If progressToken IsNot Nothing Then
                progressToken.IsCompleted = True
            End If
        End Function
    End Module

End Namespace
