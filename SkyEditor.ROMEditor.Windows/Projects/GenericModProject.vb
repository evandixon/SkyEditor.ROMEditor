Imports System.IO
Imports System.Security.Cryptography
Imports System.Text.RegularExpressions
Imports DS_ROM_Patcher
Imports SkyEditor.Core
Imports SkyEditor.Core.Processes
Imports SkyEditor.Core.Projects
Imports SkyEditor.Core.Utilities

Namespace Projects
    Public Class GenericModProject
        Inherits Project

        Public Sub New()
            MyBase.New
            Me.ModDependenciesBefore = New List(Of String)
            Me.ModDependenciesAfter = New List(Of String)
        End Sub

#Region "Project Settings"
        Public Property ModName As String
            Get
                Return Settings("ModName")
            End Get
            Set(value As String)
                Settings("ModName") = value
            End Set
        End Property

        Public Property ModVersion As String
            Get
                Return Settings("ModVersion")
            End Get
            Set(value As String)
                Settings("ModVersion") = value
            End Set
        End Property

        Public Property ModAuthor As String
            Get
                Return Settings("ModAuthor")
            End Get
            Set(value As String)
                Settings("ModAuthor") = value
            End Set
        End Property

        Public Property ModDescription As String
            Get
                Return Settings("ModDescription")
            End Get
            Set(value As String)
                Settings("ModDescription") = value
            End Set
        End Property

        Public Property Homepage As String
            Get
                Return Settings("Homepage")
            End Get
            Set(value As String)
                Settings("Homepage") = value
            End Set
        End Property

        Public Property ModDependenciesBefore As List(Of String)
            Get
                Return Settings("Dependencies-Before")
            End Get
            Set(value As List(Of String))
                Settings("Dependencies-Before") = value
            End Set
        End Property

        Public Property ModDependenciesAfter As List(Of String)
            Get
                Return Settings("Dependencies-After")
            End Get
            Set(value As List(Of String))
                Settings("Dependencies-After") = value
            End Set
        End Property
#End Region

#Region "Project Type Properties"

        Public Overrides ReadOnly Property CanBuild As Boolean
            Get
                Return Not IsBuilding
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

        Public Overridable Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {".*"}
        End Function

#End Region

        ''' <summary>
        ''' Gets the paths of the files or directories to copy on initialization, relative to the the root RawFiles directory.
        ''' If empty, will copy everything.
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Function GetFilesToCopy(solution As Solution, baseRomProjectName As String) As IEnumerable(Of String)
            Return {}
        End Function

        Public Overridable Function SupportsAdd() As Boolean
            Return True
        End Function

        Public Overridable Function SupportsDelete() As Boolean
            Return False
        End Function

        Public Overridable Function GetCustomFilePatchers() As IEnumerable(Of FilePatcher)
            Return {}
        End Function

#Region "File Paths"
        Public Overridable Function GetRawFilesSourceDir(solution As Solution, sourceProjectName As String) As String
            Dim baseRomProject As BaseRomProject = solution.GetProjectsByName(sourceProjectName).FirstOrDefault
            Return baseRomProject.GetRawFilesDir
        End Function

        Public Overridable Function GetModRootOutputDir() As String
            Return Path.Combine(GetRootDirectory, "Output")
        End Function

        Public Overridable Function GetModOutputDir(sourceProjectName As String)
            Return Path.Combine(GetModRootOutputDir, sourceProjectName)
        End Function

        Public Overridable Function GetModOutputFilename(sourceProjectName As String)
            Return Path.Combine(GetModOutputDir(sourceProjectName), ModName & ".mod")
        End Function

        Public Overridable Function GetRawFilesDir() As String
            Return Path.Combine(GetRootDirectory, "Raw Files")
        End Function

        Public Overridable Function GetModTempDir() As String
            Return Path.Combine(GetRootDirectory, "Mod Files")
        End Function
#End Region

        Public Async Function RunInitialize() As Task
            RequiresInitialization = True
            Await ParentSolution.Build({Me})
        End Function

        Private Property RequiresInitialization As Boolean

        Public NotOverridable Overrides Async Function Build() As Task
            If RequiresInitialization Then
                Await Initialize()
            Else
                Await DoBuild()
            End If

            Me.Progress = 1
            Me.IsIndeterminate = False
            Me.Message = My.Resources.Language.Complete
        End Function

        Public Overrides Async Function Initialize() As Task
            If Me.ProjectReferenceNames.Count > 0 Then
                Me.Progress = 0
                Me.Message = My.Resources.Language.LoadingCopyingFiles

                Dim filesToCopy = Me.GetFilesToCopy(ParentSolution, Me.ProjectReferenceNames(0))
                Dim sourceRoot = GetRawFilesSourceDir(ParentSolution, Me.ProjectReferenceNames(0))
                If filesToCopy.Count = 1 Then
                    Me.IsIndeterminate = True

                    Dim source As String = Path.Combine(sourceRoot, filesToCopy(0))
                    Dim dest As String = Path.Combine(GetRawFilesDir, filesToCopy(0))
                    If File.Exists(source) Then
                        If Not Directory.Exists(Path.GetDirectoryName(dest)) Then
                            Directory.CreateDirectory(Path.GetDirectoryName(dest))
                        End If
                        File.Copy(source, dest, True)
                    ElseIf Directory.Exists(source) Then
                        Await FileSystem.CopyDirectory(source, dest, CurrentPluginManager.CurrentIOProvider)
                    End If
                ElseIf filesToCopy.Count > 0 Then
                    Me.IsIndeterminate = False
                    Dim a As New AsyncFor
                    AddHandler a.LoadingStatusChanged, Sub(sender As Object, e As ProgressReportedEventArgs)
                                                           Me.Progress = e.Progress
                                                       End Sub
                    Await a.RunForEach(filesToCopy,
                                       Sub(item As String)
                                           Dim source As String = Path.Combine(sourceRoot, item)
                                           If File.Exists(source) Then
                                               Dim dest As String = Path.Combine(GetRawFilesDir, item)
                                               If Not Directory.Exists(Path.GetDirectoryName(dest)) Then
                                                   Directory.CreateDirectory(Path.GetDirectoryName(dest))
                                               End If
                                               File.Copy(source, dest, True)
                                           ElseIf Directory.Exists(source) Then

                                               For Each f In Directory.GetFiles(source, "*", SearchOption.AllDirectories)
                                                   Dim dest As String = f.Replace(sourceRoot, GetRawFilesDir)
                                                   If Not Directory.Exists(Path.GetDirectoryName(dest)) Then
                                                       Directory.CreateDirectory(Path.GetDirectoryName(dest))
                                                   End If
                                                   File.Copy(f, dest, True)
                                               Next
                                           End If
                                       End Sub)
                Else
                    Await FileSystem.CopyDirectory(sourceRoot, GetRawFilesDir, CurrentPluginManager.CurrentIOProvider)
                End If

                Me.Progress = 1
                Me.IsIndeterminate = False
                Me.Message = My.Resources.Language.Complete
            Else
                'Since there's no source project, we'll leave it up to the user to supply the needed files.
                If Not Directory.Exists(GetRawFilesDir) Then
                    Directory.CreateDirectory(GetRawFilesDir)
                End If
            End If
            RequiresInitialization = False
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>If this is overridden, do custom work, THEN use MyBase.Build</remarks>
        Protected Overridable Async Function DoBuild() As Task
            For Each sourceProjectName In Me.ProjectReferenceNames

                Dim sourceProject As Project = ParentSolution.GetProjectsByName(sourceProjectName).FirstOrDefault

                If sourceProject IsNot Nothing AndAlso TypeOf sourceProject Is BaseRomProject Then
                    Dim builder As New ModBuilder

                    'Set the custom patchers
                    builder.CustomFilePatchers = GetCustomFilePatchers.ToList

                    'Set metadata properties
                    builder.ModName = ModName
                    builder.ModVersion = ModVersion
                    builder.ModAuthor = ModAuthor
                    builder.ModDescription = ModDescription
                    builder.Homepage = Homepage
                    builder.ModDependenciesBefore = ModDependenciesBefore
                    builder.ModDependenciesAfter = ModDependenciesAfter
                    builder.GameCode = DirectCast(sourceProject, BaseRomProject).GameCode

                    'Set progress event handler
                    AddHandler builder.BuildStatusChanged, AddressOf OnModBuilderProgressed

                    'Ensure parent directory exists
                    Dim parentDir = Path.GetDirectoryName(GetModOutputFilename(sourceProjectName))
                    If Not Directory.Exists(parentDir) Then
                        Directory.CreateDirectory(parentDir)
                    End If

                    'Do the build
                    Await builder.BuildMod(GetRawFilesSourceDir(ParentSolution, sourceProjectName), GetRawFilesDir(), GetModOutputFilename(sourceProjectName), CurrentPluginManager.CurrentIOProvider)

                    'Remove progress event handler
                    RemoveHandler builder.BuildStatusChanged, AddressOf OnModBuilderProgressed
                End If

            Next
            Me.Progress = 1
            Me.Message = My.Resources.Language.Complete
        End Function

        Private Sub OnModBuilderProgressed(sender As Object, e As ProgressReportedEventArgs)
            Me.IsIndeterminate = e.IsIndeterminate
            Me.Progress = e.Progress
            Me.Message = e.Message
        End Sub

    End Class

End Namespace
