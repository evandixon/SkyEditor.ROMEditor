Imports SkyEditor.Core
Imports System.IO
Imports SkyEditor.Core.Windows
Imports SkyEditor.ROMEditor.Projects

Namespace Windows
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

        Public Overrides Sub Load(manager As PluginManager)
            manager.LoadRequiredPlugin(New ROMEditor.PluginDefinition, Me)
            manager.CurrentIOUIManager.RegisterIOFilter("*.img", My.Resources.Language.CTEImageFiles)

            manager.RegisterTypeRegister(GetType(GenericModProject))

            'Manager.RegisterConsoleCommand("import-language", New ConsoleCommands.ImportLanguage)
            'Manager.RegisterConsoleCommand("cteconvert", New ConsoleCommands.BatchCteConvert)
            'Manager.RegisterConsoleCommand("gzip", New ConsoleCommands.Gzip)

            GameCodeRegistry.RegisterGameCode(My.Resources.Language.Game_OR, GameStrings.ORCode)
            GameCodeRegistry.RegisterGameCode(My.Resources.Language.Game_AS, GameStrings.ASCode)
            GameCodeRegistry.RegisterGameCode(My.Resources.Language.Game_X, GameStrings.PokemonXCode)
            GameCodeRegistry.RegisterGameCode(My.Resources.Language.Game_Y, GameStrings.PokemonYCode)
            GameCodeRegistry.RegisterGameCode(My.Resources.Language.Game_GTI, GameStrings.GTICode)
            GameCodeRegistry.RegisterGameCode(My.Resources.Language.Game_PSMD, GameStrings.PSMDCode)
        End Sub

        Public Overrides Sub UnLoad(manager As PluginManager)
            Dim dir As String = EnvironmentPaths.GetResourceName("Temp")
            If Directory.Exists(dir) Then
                On Error Resume Next
                Directory.Delete(dir, True)
                Directory.CreateDirectory(dir)
            End If
        End Sub

        Private Sub EnsureDirDeleted(Dir As String)
            If Directory.Exists(Dir) Then
                Directory.Delete(Dir, True)
            End If
        End Sub
        Private Sub EnsureFileDeleted(Dir As String)
            If File.Exists(Dir) Then
                File.Delete(Dir)
            End If
        End Sub

    End Class
End Namespace
