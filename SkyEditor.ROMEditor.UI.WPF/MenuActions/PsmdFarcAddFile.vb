Imports System.IO
Imports System.Reflection
Imports System.Windows.Forms
Imports SkyEditor.Core
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.IO.FileSystem
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD

Namespace MenuActions
    Public Class PsmdFarcAddFile
        Inherits MenuAction

        Public Sub New(FileSystem As IFileSystem, appViewModel As ApplicationViewModel)
            MyBase.New({My.Resources.Language.MenuFarc, My.Resources.Language.MenuFarcAddFile})

            If FileSystem Is Nothing Then
                Throw New ArgumentNullException(NameOf(FileSystem))
            End If

            If appViewModel Is Nothing Then
                Throw New ArgumentNullException(NameOf(appViewModel))
            End If

            Dialog = New OpenFileDialog
            SortOrder = 6
            CurrentFileSystem = FileSystem
            CurrentApplicationViewModel = appViewModel
        End Sub

        Protected Property CurrentFileSystem As IFileSystem
        Protected Property CurrentApplicationViewModel As ApplicationViewModel

        Private WithEvents Dialog As OpenFileDialog

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of TypeInfo)
            Return {GetType(Farc).GetTypeInfo}
        End Function

        Public Overrides Sub DoAction(Targets As IEnumerable(Of Object))
            Dim loadingTasks As New List(Of Task)
            For Each item As Farc In Targets
                If Dialog.ShowDialog = DialogResult.OK Then
                    item.WriteAllBytes(Path.GetFileName(Dialog.FileName), File.ReadAllBytes(Dialog.FileName))
                End If
            Next
        End Sub
    End Class
End Namespace

