Imports PPMDU
Imports SkyEditor.Core.Projects
Imports SkyEditor.Core.Utilities
Imports SkyEditor.ROMEditor.Projects
Imports SkyEditor.ROMEditor.Windows.FileFormats.Explorers

Namespace MysteryDungeon.Explorers.Projects
    Public Class KaomadoProject
        Inherits GenericModProject

        Public Overrides Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {GameStrings.SkyCode}
        End Function

        Public Overrides Function GetFilesToCopy(Solution As Solution, BaseRomProjectName As String) As IEnumerable(Of String)
            Return {IO.Path.Combine("Data", "FONT", "kaomado.kao")}
        End Function

        Public Overrides Async Function Initialize() As Task
            Await MyBase.Initialize

            'Start loading
            Me.Progress = 0
            Me.IsIndeterminate = True
            Me.Message = My.Resources.Language.LoadingUnpacking

            'Unpack
            Dim rootDir = GetRootDirectory()
            Dim portraitDir = IO.Path.Combine(rootDir, "Pokemon", "Portraits")
            If Not IO.Directory.Exists(portraitDir) Then
                IO.Directory.CreateDirectory(portraitDir)
            End If

            Using manager As New UtilityManager
                Await manager.RunKaoUtil(IO.Path.Combine(GetRawFilesDir, "data", "FONT", "kaomado.kao"), portraitDir, False)
            End Using

            Await ApplyMissingPortraitFix(portraitDir)

            ''Add files to project
            ''Disabled because it takes too long
            'For Each item In IO.Directory.GetFiles(portraitDir, "*", IO.SearchOption.AllDirectories)
            '    Me.AddExistingFile(IO.Path.GetDirectoryName(item).Replace(portraitDir, IO.Path.Combine("Pokemon", "Portraits")), item, CurrentPluginManager.CurrentIOProvider)
            'Next

            'Stop loading
            Me.Progress = 1
            Me.IsIndeterminate = False
            Me.Message = My.Resources.Language.Complete

        End Function

        Public Overrides Async Function Build() As Task
            'Start loading
            Me.Progress = 0
            Me.IsIndeterminate = True
            Me.Message = My.Resources.Language.LoadingPacking

            'Pack
            Using manager As New UtilityManager
                Await manager.RunKaoUtil(IO.Path.Combine(GetRootDirectory, "Pokemon", "Portraits"), IO.Path.Combine(GetRawFilesDir, "data", "FONT", "kaomado.kao"), False)
            End Using

            'Stop loading
            Me.Progress = 1
            Me.IsIndeterminate = False
            Me.Message = My.Resources.Language.Complete

            'Build the mod
            Await MyBase.Build
        End Function

        Public Async Function ApplyMissingPortraitFix(unpackDirectory As String) As Task
            Dim faces = {"0000_STANDARD.png",
                         "0002_GRIN.png",
                         "0004_PAINED.png",
                         "0006_ANGRY.png",
                         "0008_WORRIED.png",
                         "0010_SAD.png",
                         "0012_CRYING.png",
                         "0014_SHOUTING.png",
                         "0016_TEARY_EYED.png",
                         "0018_DETERMINED.png",
                         "0020_JOYOUS.png",
                         "0022_INSPIRED.png",
                         "0024_SURPRISED.png",
                         "0026_DIZZY.png",
                         "0032_SIGH.png",
                         "0034_STUNNED.png"}

            Dim runner As New AsyncFor
            Dim directories = IO.Directory.GetDirectories(unpackDirectory)
            Await runner.RunForEach(directories,
                                    Sub(directory As String)
                                        For j As Integer = 1 To faces.Length - 1
                                            If Not IO.File.Exists(IO.Path.Combine(directory, faces(j))) Then
                                                IO.File.Copy(IO.Path.Combine(directory, faces(0)), IO.Path.Combine(directory, faces(j)))
                                            End If
                                        Next
                                    End Sub)
        End Function
    End Class
End Namespace

