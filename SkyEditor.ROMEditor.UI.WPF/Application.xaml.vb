Class Application

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.

    Private Async Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
        Try
            Await SkyEditor.UI.WPF.StartupHelpers.RunWPFStartupSequence(New SkyEditor.UI.WPF.WPFCoreSkyEditorPlugin(New PluginDefinition))
        Catch ex As Exception
            Debugger.Break()
            Throw
        End Try
    End Sub

End Class
