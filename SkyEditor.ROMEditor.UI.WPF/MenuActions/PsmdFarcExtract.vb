Imports System.Reflection
Imports System.Windows.Forms
Imports SkyEditor.Core.UI
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD

Namespace MenuActions
    Public Class PsmdFarcExtract
        Inherits MenuAction

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuFarc, My.Resources.Language.MenuFarcExtract})
            Dialog = New FolderBrowserDialog
            SortOrder = 5
        End Sub

        Private WithEvents Dialog As FolderBrowserDialog

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of TypeInfo)
            Return {GetType(Farc).GetTypeInfo}
        End Function

        Public Overrides Async Sub DoAction(Targets As IEnumerable(Of Object))
            Dim loadingTasks As New List(Of Task)
            For Each item As Farc In Targets
                If Dialog.ShowDialog = DialogResult.OK Then
                    loadingTasks.Add(item.Extract(Dialog.SelectedPath, CurrentApplicationViewModel.CurrentIOProvider))
                    CurrentApplicationViewModel.ShowLoading(item)
                End If
            Next

            'While the application view model is handling the extraction display, we need to wait here in case there's exceptions.
            'Otherwise, they'll be swallowed
            Await Task.WhenAll(loadingTasks)
        End Sub
    End Class
End Namespace

