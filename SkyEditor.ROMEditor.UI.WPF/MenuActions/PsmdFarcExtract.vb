Imports System.Reflection
Imports System.Windows.Forms
Imports SkyEditor.Core.UI
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD

Namespace MenuActions
    Public Class PsmdFarcExtract
        Inherits MenuAction

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuImage, My.Resources.Language.MenuImageExport})
            Dialog = New FolderBrowserDialog
            SortOrder = 5
        End Sub

        Private WithEvents Dialog As FolderBrowserDialog

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of TypeInfo)
            Return {GetType(Farc).GetTypeInfo}
        End Function

        Public Overrides Sub DoAction(Targets As IEnumerable(Of Object))
            For Each item As Farc In Targets
                If Dialog.ShowDialog = DialogResult.OK Then
                    CurrentApplicationViewModel.ShowLoading(item.Extract(Dialog.SelectedPath, CurrentApplicationViewModel.CurrentIOProvider))
                End If
            Next
        End Sub
    End Class
End Namespace

