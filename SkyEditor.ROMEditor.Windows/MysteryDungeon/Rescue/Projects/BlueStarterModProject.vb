Imports System.IO
Imports DS_ROM_Patcher
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Projects
Imports SkyEditor.Core.Utilities
Imports SkyEditor.ROMEditor.MysteryDungeon.Explorers.ViewModels
Imports SkyEditor.ROMEditor.Projects

Namespace MysteryDungeon.Rescue.Projects
    Public Class BlueStarterModProject
        Inherits GenericModProject

        Public Overrides Function GetFilesToCopy(solution As Solution, baseRomProjectName As String) As IEnumerable(Of String)
            Return {"arm9.bin"}
        End Function
        Public Overrides Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {GameStrings.BRTUSCode}
        End Function

        Public Overrides Async Function Initialize() As Task
            Await MyBase.Initialize

            Dim rawDir = GetRawFilesDir()
            Dim projDir = GetRootDirectory()


            Me.Progress = 0
            Me.IsIndeterminate = True
            Me.Message = My.Resources.Language.LoadingConvertingLanguages

            'Add Personality Test
            Me.AddExistingFileToPath("/Starter Pokemon", Path.Combine(rawDir, "arm9.bin"), GetType(BlueNorthAmericanArm9Bin), CurrentPluginManager.CurrentIOProvider)

            Me.Progress = 1
            Me.IsIndeterminate = False
            Me.Message = My.Resources.Language.Complete
        End Function

        Public Overrides Async Function Build() As Task
            'No need for anything special here right now
            Await MyBase.Build
        End Function

    End Class

End Namespace
