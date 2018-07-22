Imports System.IO
Imports System.Reflection
Imports System.Windows.Forms
Imports SkyEditor.Core
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Utilities
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD

Namespace MenuActions
    Public Class PsmdFarcExtractPortraits
        Inherits MenuAction

        Public Sub New(ioProvider As IIOProvider, appViewModel As ApplicationViewModel)
            MyBase.New({My.Resources.Language.MenuFarc, My.Resources.Language.MenuFarcExtractPortraits})

            If ioProvider Is Nothing Then
                Throw New ArgumentNullException(NameOf(ioProvider))
            End If

            If appViewModel Is Nothing Then
                Throw New ArgumentNullException(NameOf(appViewModel))
            End If

            Dialog = New FolderBrowserDialog
            SortOrder = 5
            CurrentIOProvider = ioProvider
            CurrentApplicationViewModel = appViewModel
        End Sub

        Protected Property CurrentIOProvider As IIOProvider
        Protected Property CurrentApplicationViewModel As ApplicationViewModel

        Private WithEvents Dialog As FolderBrowserDialog

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of TypeInfo)
            Return {GetType(Farc).GetTypeInfo}
        End Function

        Public Overrides Async Function SupportsObject(obj As Object) As Task(Of Boolean)
            Return Await MyBase.SupportsObject(obj) AndAlso TypeOf obj Is Farc
        End Function

        Public Overrides Async Sub DoAction(Targets As IEnumerable(Of Object))
            Dim loadingTasks As New List(Of Task)
            For Each item As Farc In Targets

                Dim token As New ProgressReportToken
                CurrentApplicationViewModel.ShowLoading(token)

                If Dialog.ShowDialog = DialogResult.OK Then
                    loadingTasks.Add(Task.Run(Function() FarcExtensions.ExtractPortraits(item, Dialog.SelectedPath, CurrentIOProvider, token)))
                End If
            Next

            'While the application view model is handling the extraction display, we need to wait here in case there's exceptions.
            'Otherwise, they'll be swallowed
            Await Task.WhenAll(loadingTasks)
        End Sub
    End Class
End Namespace

