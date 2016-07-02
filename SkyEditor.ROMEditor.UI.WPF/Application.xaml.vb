Class Application
    Private Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
        SkyEditor.UI.WPF.StartupHelpers.RunWPFStartupSequence(New SkyEditor.UI.WPF.WPFCoreSkyEditorPlugin(New PluginDefinition))
    End Sub

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.

End Class
