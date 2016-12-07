Imports System.IO
Imports SkyEditor.Core
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Projects
Imports SkyEditor.Core.Utilities
Imports SkyEditor.Core.Windows
Imports SkyEditor.ROMEditor

Namespace Windows.Projects
    Public Class BaseRomProject
        Inherits Project

        Public Const System3DS = "3DS"
        Public Const SystemNDS = "NDS"

        Public Property RomSystem As String
            Get
                Return Setting("System")
            End Get
            Set(value As String)
                Setting("System") = value
            End Set
        End Property

        Public Property GameCode As String
            Get
                Return Setting("GameCode")
            End Get
            Set(value As String)
                Setting("GameCode") = value
            End Set
        End Property

        Public Overrides Function CanBuild() As Boolean
            Return (Me.GetItem("/BaseRom") IsNot Nothing)
        End Function

        Public Overrides Function CanCreateDirectory(Path As String) As Boolean
            Return False
        End Function

        Public Overrides Function CanCreateFile(Path As String) As Boolean
            Return False
        End Function

        Public Overrides Function CanDeleteDirectory(Path As String) As Boolean
            Return False
        End Function

        Public Overrides Function CanAddExistingFile(Path As String) As Boolean
            'Only if it's the root, and there isn't already a file named BaseRom.
            Return (Path.Replace("\", "/").TrimStart("/") = "") AndAlso (Me.GetItem("/BaseRom") Is Nothing)
        End Function

        Public Overrides Function CanDeleteFile(FilePath As String) As Boolean
            Return (FilePath.Replace("\", "/").TrimStart("/").ToLower = "baserom")
        End Function

        Public Overrides Function GetImportIOFilter(ParentProjectPath As String, manager As PluginManager) As String
            Select Case Me.Setting("System")
                Case "NDS"
                    Return $"{My.Resources.Language.NDSRomFile} (*.nds)|*.nds|{My.Resources.Language.AllFiles} (*.*)|*.*"
                Case "3DS"
                    Return $"{My.Resources.Language.ThreeDSRomFile} (*.3ds;*.3dz)|*.3ds;*.3dz|{My.Resources.Language.AllFiles} (*.*)|*.*"
                Case Else
                    Return $"{My.Resources.Language.AllFiles} (*.*)|*.*"
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
            Dim fullPath = Me.GetItem("/BaseRom").GetFilename

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

            Me.BuildProgress = 0
            Me.BuildStatusMessage = My.Resources.Language.LoadingUnpacking

            Using unpacker As New DotNet3dsToolkit.Converter
                Dim unpackProgressEventHandler = Sub(sender As Object, e As ProgressReportedEventArgs)
                                                     Me.BuildProgress = e.Progress
                                                     Me.BuildStatusMessage = e.Message
                                                     Me.IsBuildProgressIndeterminate = e.IsIndeterminate
                                                 End Sub

                AddHandler unpacker.UnpackProgressed, unpackProgressEventHandler

                Await unpacker.ExtractAuto(fullPath, GetRawFilesDir)

                RemoveHandler unpacker.UnpackProgressed, unpackProgressEventHandler
            End Using

            GameCode = Await DotNet3dsToolkit.MetadataReader.GetGameID(GetRawFilesDir)

            Dim baseromFilename = Me.GetFilename("/BaseRom")
            DeleteFile("/BaseRom")
            File.Delete(baseromFilename)

            Me.IsBuildProgressIndeterminate = False
            Me.BuildProgress = 1
            Me.BuildStatusMessage = My.Resources.Language.Complete
        End Function

        Public Overridable Function GetRawFilesDir() As String
            Return Path.Combine(Path.GetDirectoryName(Me.Filename), "Raw Files")
        End Function

        Public Overridable Function GetIconPath() As String
            Return Path.Combine(GetRawFilesDir, "exefs", "icon.bin")
        End Function
    End Class

End Namespace
