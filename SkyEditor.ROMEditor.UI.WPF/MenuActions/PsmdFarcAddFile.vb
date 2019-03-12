Imports System.IO
Imports System.Reflection
Imports System.Windows.Forms
Imports SkyEditor.Core
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD

Namespace MenuActions
    Public Class PsmdFarcAddFile
        Inherits MenuAction

        Public Sub New(ioProvider As IIOProvider, appViewModel As ApplicationViewModel)
            MyBase.New({My.Resources.Language.MenuFarc, My.Resources.Language.MenuFarcAddFile})

            If ioProvider Is Nothing Then
                Throw New ArgumentNullException(NameOf(ioProvider))
            End If

            If appViewModel Is Nothing Then
                Throw New ArgumentNullException(NameOf(appViewModel))
            End If

            Dialog = New OpenFileDialog
            SortOrder = 6
            CurrentIOProvider = ioProvider
            CurrentApplicationViewModel = appViewModel
        End Sub

        Protected Property CurrentIOProvider As IIOProvider
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

