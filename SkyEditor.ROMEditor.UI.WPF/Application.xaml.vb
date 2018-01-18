Class Application

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.

    Private Async Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
#If DEBUG Then
        Try
#End If

        SkyEditor.UI.WPF.StartupHelpers.EnableErrorDialog()
        Await SkyEditor.UI.WPF.StartupHelpers.ShowMainWindow(New SkyEditor.UI.WPF.WPFCoreSkyEditorPlugin(New PluginDefinition))

#If DEBUG Then
Catch ex As Exception
            Debugger.Break()
            Throw
        End Try
#End If

    End Sub

End Class
