﻿Imports SkyEditor.Core

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
        'Load the plugin this one depends on
        'Manager.LoadRequiredPlugin(New SkyEditor.ROMEditor.PluginDefinition, Me) 'No need to include this, because it's loaded by SkyEditor.ROMEditor.Windows below
        Manager.LoadRequiredPlugin(New SkyEditor.ROMEditor.Windows.PluginDefinition, Me)
        Manager.LoadRequiredPlugin(New SkyEditor.CodeEditor.PluginDefinition, Me)
        Manager.LoadRequiredPlugin(New SkyEditor.CodeEditor.UI.WPF.PluginInfo, Me)
    End Sub

End Class