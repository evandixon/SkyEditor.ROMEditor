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
            'Only if it's the root, and there isn't already a file named BaseRom.
            Return (path.Replace("\", "/").TrimStart("/") = "") AndAlso (Me.GetItem("/BaseRom") Is Nothing)
        End Function

        Public Overrides Function CanDeleteFile(FilePath As String) As Boolean
            Return (FilePath.Replace("\", "/").TrimStart("/").ToLower = "baserom")
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

        Protected Overrides Function GetImportedFilePath(ParentProjectPath As String, FullFilename As String) As String
            Return "/BaseRom"
        End Function

        Private Async Sub BaseRomProject_FileAdded(sender As Object, e As ProjectFileAddedEventArgs) Handles Me.FileAdded
            'Calling DoBuild directly would result in the solution not raising the build event, which is needed for SolutionBuildProgress
            Await ParentSolution.Build({Me})
        End Sub

        Public Overrides Async Function Build() As Task
            Await MyBase.Build
            If CanBuild() Then
                Await DoBuild()
            End If
        End Function

        Private Async Function DoBuild() As Task
            Dim mode As String = Nothing
            Dim fullPath = Me.GetItem("/BaseRom").Filename

            If Not String.IsNullOrEmpty(Me.RomSystem) Then
                If Me.RomSystem = SystemNDS Then
                    mode = "nds"
                ElseIf Me.RomSystem = System3DS Then
                    mode = "3ds"
                End If
            End If

            If mode Is Nothing Then
                Select Case Await DotNet3dsToolkit.MetadataReader.GetSystem(fullPath)
                    Case DotNet3dsToolkit.SystemType.NDS
                        RomSystem = SystemNDS
                    Case DotNet3dsToolkit.SystemType.ThreeDS
                        RomSystem = System3DS
                    Case Else
                        'Todo: Replace with better exception
                        Throw New NotSupportedException("File format not supported.")
                End Select
            End If

            Me.Progress = 0
            Me.Message = My.Resources.Language.LoadingUnpacking

            Using unpacker As New DotNet3dsToolkit.Converter
                Dim unpackProgressEventHandler = Sub(sender As Object, e As ProgressReportedEventArgs)
                                                     Me.Progress = e.Progress
                                                     Me.Message = e.Message
                                                     Me.IsIndeterminate = e.IsIndeterminate
                                                 End Sub

                AddHandler unpacker.UnpackProgressed, unpackProgressEventHandler

                Await unpacker.ExtractAuto(fullPath, GetRawFilesDir)

                RemoveHandler unpacker.UnpackProgressed, unpackProgressEventHandler
            End Using

            GameCode = Await DotNet3dsToolkit.MetadataReader.GetGameID(GetRawFilesDir)

            Dim baseromFilename = Me.GetFilename("/BaseRom")
            DeleteFile("/BaseRom")
            File.Delete(baseromFilename)

            Me.IsIndeterminate = False
            Me.Progress = 1
            Me.Message = My.Resources.Language.Complete
            Me.IsCompleted = True
        End Function

        Public Overridable Function GetRawFilesDir() As String
            Return Path.Combine(Path.GetDirectoryName(Me.Filename), "Raw Files")
        End Function

        Public Overridable Function GetIconPath() As String
            Return Path.Combine(GetRawFilesDir, "exefs", "icon.bin")
        End Function
    End Class

End Namespace
