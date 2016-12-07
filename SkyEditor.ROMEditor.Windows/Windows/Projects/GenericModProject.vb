Imports System.Security.Cryptography
Imports System.Text.RegularExpressions
Imports DS_ROM_Patcher
Imports SkyEditor.Core.Projects
Imports SkyEditor.Core.Utilities
Imports SkyEditor.Core.Windows
Imports SkyEditor.Core.Windows.Processes

Namespace Windows.Projects
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
                Return Setting("ModName")
            End Get
            Set(value As String)
                Setting("ModName") = value
            End Set
        End Property

        Public Property ModVersion As String
            Get
                Return Setting("ModVersion")
            End Get
            Set(value As String)
                Setting("ModVersion") = value
            End Set
        End Property

        Public Property ModAuthor As String
            Get
                Return Setting("ModAuthor")
            End Get
            Set(value As String)
                Setting("ModAuthor") = value
            End Set
        End Property

        Public Property ModDescription As String
            Get
                Return Setting("ModDescription")
            End Get
            Set(value As String)
                Setting("ModDescription") = value
            End Set
        End Property

        Public Property Homepage As String
            Get
                Return Setting("Homepage")
            End Get
            Set(value As String)
                Setting("Homepage") = value
            End Set
        End Property

        Public Property ModDependenciesBefore As List(Of String)
            Get
                Return Setting("Dependencies-Before")
            End Get
            Set(value As List(Of String))
                Setting("Dependencies-Before") = value
            End Set
        End Property

        Public Property ModDependenciesAfter As List(Of String)
            Get
                Return Setting("Dependencies-After")
            End Get
            Set(value As List(Of String))
                Setting("Dependencies-After") = value
            End Set
        End Property
#End Region

#Region "Project Type Properties"
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
            Return False
        End Function

        Public Overrides Function CanDeleteFile(FilePath As String) As Boolean
            Return False
        End Function

        Public Overridable Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {".*"}
        End Function

        Public Overrides Function CanBuild() As Boolean
            Return True
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
            Return IO.Path.Combine(GetRootDirectory, "Output")
        End Function

        Public Overridable Function GetModOutputDir(sourceProjectName As String)
            Return IO.Path.Combine(GetModRootOutputDir, sourceProjectName)
        End Function

        Public Overridable Function GetModOutputFilename(sourceProjectName As String)
            Return IO.Path.Combine(GetModOutputDir(sourceProjectName), ModName & ".mod")
        End Function

        Public Overridable Function GetRawFilesDir() As String
            Return IO.Path.Combine(GetRootDirectory, "Raw Files")
        End Function

        Public Overridable Function GetModTempDir() As String
            Return IO.Path.Combine(GetRootDirectory, "Mod Files")
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

            Me.BuildProgress = 1
            Me.IsBuildProgressIndeterminate = False
            Me.BuildStatusMessage = My.Resources.Language.Complete
        End Function

        Protected Overridable Async Function Initialize() As Task
            If Me.ProjectReferences.Count > 0 Then
                Me.BuildProgress = 0
                Me.BuildStatusMessage = My.Resources.Language.LoadingCopyingFiles

                Dim filesToCopy = Me.GetFilesToCopy(ParentSolution, Me.ProjectReferences(0))
                Dim sourceRoot = GetRawFilesSourceDir(ParentSolution, Me.ProjectReferences(0))
                If filesToCopy.Count = 1 Then
                    Me.IsBuildProgressIndeterminate = True

                    Dim source As String = IO.Path.Combine(sourceRoot, filesToCopy(0))
                    Dim dest As String = IO.Path.Combine(GetRawFilesDir, filesToCopy(0))
                    If IO.File.Exists(source) Then
                        If Not IO.Directory.Exists(IO.Path.GetDirectoryName(dest)) Then
                            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(dest))
                        End If
                        IO.File.Copy(source, dest, True)
                    ElseIf IO.Directory.Exists(source) Then
                        Await FileSystem.CopyDirectory(source, dest, CurrentPluginManager.CurrentIOProvider)
                    End If
                ElseIf filesToCopy.Count > 0 Then
                    Me.IsBuildProgressIndeterminate = False
                    Dim a As New AsyncFor
                    AddHandler a.LoadingStatusChanged, Sub(sender As Object, e As LoadingStatusChangedEventArgs)
                                                           Me.BuildProgress = e.Progress
                                                       End Sub
                    Await a.RunForEach(Sub(item As String)
                                           Dim source As String = IO.Path.Combine(sourceRoot, item)
                                           If IO.File.Exists(source) Then
                                               Dim dest As String = IO.Path.Combine(GetRawFilesDir, item)
                                               If Not IO.Directory.Exists(IO.Path.GetDirectoryName(dest)) Then
                                                   IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(dest))
                                               End If
                                               IO.File.Copy(source, dest, True)
                                           ElseIf IO.Directory.Exists(source) Then

                                               For Each f In IO.Directory.GetFiles(source, "*", IO.SearchOption.AllDirectories)
                                                   Dim dest As String = f.Replace(sourceRoot, GetRawFilesDir)
                                                   If Not IO.Directory.Exists(IO.Path.GetDirectoryName(dest)) Then
                                                       IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(dest))
                                                   End If
                                                   IO.File.Copy(f, dest, True)
                                               Next
                                           End If
                                       End Sub, filesToCopy)
                Else
                    Await FileSystem.CopyDirectory(sourceRoot, GetRawFilesDir, CurrentPluginManager.CurrentIOProvider)
                End If

                Me.BuildProgress = 1
                Me.IsBuildProgressIndeterminate = False
                Me.BuildStatusMessage = My.Resources.Language.Complete
            Else
                'Since there's no source project, we'll leave it up to the user to supply the needed files.
                If Not IO.Directory.Exists(GetRawFilesDir) Then
                    IO.Directory.CreateDirectory(GetRawFilesDir)
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
            For Each sourceProjectName In Me.ProjectReferences

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

                'Set progress event handler
                AddHandler builder.BuildStatusChanged, AddressOf OnModBuilderProgressed

                'Do the build
                Await builder.BuildMod(GetRawFilesSourceDir(ParentSolution, sourceProjectName), GetRawFilesDir(), GetModOutputFilename(sourceProjectName), CurrentPluginManager.CurrentIOProvider)

                'Remove progress event handler
                RemoveHandler builder.BuildStatusChanged, AddressOf OnModBuilderProgressed

            Next
            Me.BuildProgress = 1
            Me.BuildStatusMessage = My.Resources.Language.Complete
        End Function

        Private Sub OnModBuilderProgressed(sender As Object, e As ProgressReportedEventArgs)
            Me.IsBuildProgressIndeterminate = e.IsIndeterminate
            Me.BuildProgress = e.Progress
            Me.BuildStatusMessage = e.Message
        End Sub

    End Class

End Namespace
