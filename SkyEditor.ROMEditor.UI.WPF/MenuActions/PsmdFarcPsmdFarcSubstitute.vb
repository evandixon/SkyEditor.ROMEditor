Imports System.Reflection
Imports System.Windows.Forms
Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Utilities
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD.Extensions

Namespace MenuActions
    Public Class PsmdFarcSubstitute
        Inherits MenuAction

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuFarc, My.Resources.Language.MenuFarcSubstitute})
            Dialog = New FolderBrowserDialog
            SortOrder = 5
        End Sub

        Private WithEvents Dialog As FolderBrowserDialog

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of TypeInfo)
            Return {GetType(Farc).GetTypeInfo}
        End Function

        Public Overrides Async Function SupportsObject(obj As Object) As Task(Of Boolean)
            Return Await MyBase.SupportsObject(obj) AndAlso TypeOf obj Is Farc AndAlso IO.File.Exists(IO.Path.Combine(IO.Path.GetDirectoryName(DirectCast(obj, Farc).Filename), "pokemon_graphics_database.bin"))
        End Function

        Public Overrides Async Sub DoAction(Targets As IEnumerable(Of Object))
            Dim loadingTasks As New List(Of Task)
            For Each item As Farc In Targets

                Dim pgdbPath = IO.Path.Combine(IO.Path.GetDirectoryName(item.Filename), "pokemon_graphics_database.bin")

                If Not IO.File.Exists(pgdbPath) Then
                    'We shouldn't get here since SupportsObject checks for it, but just in case
                    Throw New IO.FileNotFoundException("Pokemon Graphics Database not found.", pgdbPath)
                End If

                Dim pgdb As New PGDB
                Await pgdb.OpenFile(pgdbPath, CurrentApplicationViewModel.CurrentIOProvider)

                Dim token As New ProgressReportToken
                CurrentApplicationViewModel.ShowLoading(token)

                loadingTasks.Add(Task.Run(Function() item.SubstituteMissingAnimations(pgdb, token)))
            Next

            'While the application view model is handling the extraction display, we need to wait here in case there's exceptions.
            'Otherwise, they'll be swallowed
            Await Task.WhenAll(loadingTasks)
        End Sub
    End Class
End Namespace

