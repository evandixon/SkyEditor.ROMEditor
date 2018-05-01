Imports System.IO
Imports SkyEditor.Core
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Projects
Imports SkyEditor.Core.Utilities
Imports SkyEditor.ROMEditor

Namespace Projects
    Public Class BaseRomProject
        Inherits Project

        Public Const System3DS = "3DS"
        Public Const SystemNDS = "NDS"

#Region "Settings"
        Public Property RomSystem As String
            Get
                Return Settings("System")
            End Get
            Set(value As String)
                Settings("System") = value
            End Set
        End Property

        Public Property GameCode As String
            Get
                Return Settings("GameCode")
            End Get
            Set(value As String)
                Settings("GameCode") = value
            End Set
        End Property

        Public Property HasRom As Boolean
            Get
                Dim value = Settings("HasRom")
                If value IsNot Nothing AndAlso TypeOf value Is Boolean Then
                    Return value
                Else
                    Return False
                End If
            End Get
            Set(value As Boolean)
                Settings("HasRom") = value
            End Set
        End Property

#End Region

#Region "Solution Stuff"
        Public Overrides ReadOnly Property CanBuild As Boolean
            Get
                Return (Me.GetItem("/BaseRom") IsNot Nothing)
            End Get
        End Property

        Public Overrides Function CanCreateDirectory(Path As String) As Boolean
            Return False
        End Function

        Public Overrides Function CanCreateFile(Path As String) As Boolean
            Return False
        End Function

        Public Overrides Function CanDeleteDirectory(Path As String) As Boolean
            Return False
        End Function

        Public Overrides Function CanImportFile(path As String) As Boolean
            Return False
        End Function

        Public Overrides Function CanDeleteFile(FilePath As String) As Boolean
            Return False
        End Function

        Public Overrides Function GetSupportedImportFileExtensions(parentProjectPath As String) As IEnumerable(Of String)
            Select Case Me.Settings("System")
                Case "NDS"
                    Return {"*.nds"}
                Case "3DS"
                    Return {"*.3ds", "*.3dz", "*.cia", "*.cci", "*.cxi"}
                Case Else
                    Return {"*"}
            End Select
        End Function

#End Region

        Public Async Function ImportRom(romPath As String) As Task
            Me.CurrentBuildStatus = BuildStatus.Building
            Me.IsCompleted = False

            Select Case Await DotNet3dsToolkit.MetadataReader.GetSystem(romPath)
                Case DotNet3dsToolkit.SystemType.NDS
                    RomSystem = SystemNDS
                Case DotNet3dsToolkit.SystemType.ThreeDS
                    RomSystem = System3DS
                Case Else
                    'Todo: Replace with better exception
                    Throw New NotSupportedException("File format not supported.")
            End Select

            Me.Progress = 0
            Me.Message = My.Resources.Language.LoadingUnpacking

            Using unpacker As New DotNet3dsToolkit.Converter
                Dim unpackProgressEventHandler = Sub(sender As Object, e As ProgressReportedEventArgs)
                                                     Me.Progress = e.Progress
                                                     Me.Message = e.Message
                                                     Me.IsIndeterminate = e.IsIndeterminate
                                                 End Sub

                AddHandler unpacker.UnpackProgressed, unpackProgressEventHandler

                Await unpacker.ExtractAuto(romPath, GetRawFilesDir)

                RemoveHandler unpacker.UnpackProgressed, unpackProgressEventHandler
            End Using

            GameCode = Await DotNet3dsToolkit.MetadataReader.GetGameID(GetRawFilesDir)

            Me.HasRom = True

            Me.CurrentBuildStatus = BuildStatus.Done

            Me.IsIndeterminate = False
            Me.Progress = 1
            Me.Message = My.Resources.Language.Complete
            Me.IsCompleted = True
        End Function

        ''' <summary>
        ''' The directory in which the extracted ROM files are stored
        ''' </summary>
        Public Overridable Function GetRawFilesDir() As String
            Return Path.Combine(Path.GetDirectoryName(Me.Filename), "Raw Files")
        End Function

        Public Overridable Function GetIconPath() As String
            Return Path.Combine(GetRawFilesDir, "exefs", "icon.bin")
        End Function
    End Class

End Namespace
