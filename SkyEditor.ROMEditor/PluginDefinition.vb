Imports SkyEditor.Core

Public Class PluginDefinition
    Inherits SkyEditorPlugin

    Public Overrides ReadOnly Property Credits As String
        Get
            Return My.Resources.Language.PluginCredits
        End Get
    End Property

    Public Overrides ReadOnly Property PluginAuthor As String
        Get
            Return My.Resources.Language.PluginAuthor
        End Get
    End Property

    Public Overrides ReadOnly Property PluginName As String
        Get
            Return My.Resources.Language.PluginName
        End Get
    End Property

    Public Overrides Sub Load(Manager As PluginManager)
        'Register extensions
        Manager.RegisterIOFilter("*.bin", My.Resources.Language.FileType_Bin)
        Manager.RegisterIOFilter("*.bgrs", My.Resources.Language.FileType_Bgrs)
    End Sub
End Class
