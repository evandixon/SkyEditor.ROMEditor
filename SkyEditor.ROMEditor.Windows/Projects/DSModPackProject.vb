Imports System.IO
Imports DS_ROM_Patcher
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Projects
Imports SkyEditor.Core.Utilities

Namespace Projects
    Public Class DSModPackProject
        Inherits Project

        Public Property Info As ModpackInfo
            Get
                Return Me.Settings("ModpackInfo")
            End Get
            Set
                Me.Settings("ModpackInfo") = Value
            End Set
        End Property

        Public Property BaseRomProject As String
            Get
                Return Me.Settings("BaseRomProject")
            End Get
            Set
                Me.Settings("BaseRomProject") = Value
            End Set
        End Property

        Public Property OutputEnc3DSFile As Boolean
            Get
                If Me.Settings("OutputEnc3DSFile") Is Nothing Then
                    Me.Settings("OutputEnc3DSFile") = False
                End If
                Return Me.Settings("OutputEnc3DSFile")
            End Get
            Set
                Me.Settings("OutputEnc3DSFile") = Value
            End Set
        End Property

        Public Property OutputDec3DSFile As Boolean
            Get
                If Me.Settings("OutputDec3DSFile") Is Nothing Then
                    Me.Settings("OutputDec3DSFile") = False
                End If
                Return Me.Settings("OutputDec3DSFile")
            End Get
            Set
                Me.Settings("OutputDec3DSFile") = Value
            End Set
        End Property

        Public Property OutputCIAFile As Boolean
            Get
                If Me.Settings("OutputCIAFile") Is Nothing Then
                    Me.Settings("OutputCIAFile") = False
                End If
                Return Me.Settings("OutputCIAFile")
            End Get
            Set(value As Boolean)
                Me.Settings("OutputCIAFile") = value
            End Set
        End Property

        Public Property OutputHans As Boolean
            Get
                If Me.Settings("OutputHans") Is Nothing Then
                    Me.Settings("OutputHans") = False
                End If
                Return Me.Settings("OutputHans")
            End Get
            Set(value As Boolean)
                Me.Settings("OutputHans") = value
            End Set
        End Property

        Public Property OutputLuma As Boolean
            Get
                If Me.Settings("OutputLuma") Is Nothing Then
                    Me.Settings("OutputLuma") = True
                End If
                Return Me.Settings("OutputLuma")
            End Get
            Set(value As Boolean)
                Me.Settings("OutputLuma") = value
            End Set
        End Property

        Public Overrides ReadOnly Property CanBuild As Boolean
            Get
                Dim p As BaseRomProject = ParentSolution.GetProjectsByName(BaseRomProject).FirstOrDefault
                Return (p IsNot Nothing)
            End Get
        End Property


        ''' <summary>
        ''' Gets the directory non-project mods are stored in.
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Function GetSourceModsDir() As String
            Return IO.Path.Combine(GetRootDirectory, "Mods")
        End Function

        'Gets the directory mods in the current modpack build are stored in.
        Public Overridable Function GetModsDir() As String
            Return IO.Path.Combine(GetModPackDir, "Mods")
        End Function
        Public Overridable Function GetToolsDir() As String
            Return IO.Path.Combine(GetModPackDir, "Tools")
        End Function
        Public Overridable Function GetPatchersDir() As String
            Return IO.Path.Combine(GetModPackDir, "Tools", "Patchers")
        End Function
        Public Overridable Function GetModPackDir() As String
            Return IO.Path.Combine(IO.Path.GetDirectoryName(Me.Filename), "Modpack Files")
        End Function

        ''' <summary>
        ''' Gets the directory the modpack will be outputted to.
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Function GetOutputDir() As String
            Return IO.Path.Combine(IO.Path.GetDirectoryName(Me.Filename), "Output")
        End Function

        Public Overridable Function GetBaseRomFilename(solution As Solution) As String
            Dim p As BaseRomProject = solution.GetProjectsByName(BaseRomProject).FirstOrDefault
            Return p.GetRawFilesDir
            ' Return p.GetProjectItemByPath("/BaseRom").GetFilename
        End Function

        Public Overridable Function GetBaseRomSystem(solution As Solution) As String
            Dim p As BaseRomProject = solution.GetProjectsByName(BaseRomProject).FirstOrDefault
            Return p.RomSystem
        End Function
        Public Overridable Function GetBaseGameCode(solution As Solution) As String
            Dim p As BaseRomProject = solution.GetProjectsByName(BaseRomProject).FirstOrDefault
            Return p.GameCode
        End Function
        Public Overridable Function GetLocalSmdhPath() As String
            Return IO.Path.Combine(GetRootDirectory, "Modpack.smdh")
        End Function

        Public Overrides Async Function Initialize() As Task
            Await MyBase.Initialize()

            Me.Info = New ModpackInfo With {.Name = Me.Name}
            Me.Info.Name = Me.Name
            Me.Info.ShortName = Me.Name.Substring(0, Math.Min(Me.Name.Length, 10))
            Me.Info.Author = "Unknown"
            Me.Info.Version = "1.0.0"

            Me.BaseRomProject = ParentSolution.Settings("BaseRomProject")

            Dim baseRomProject As BaseRomProject = ParentSolution.GetProjectsByName(Me.BaseRomProject).FirstOrDefault
            If baseRomProject IsNot Nothing Then
                Me.Info.System = baseRomProject.RomSystem
                Me.Info.GameCode = baseRomProject.GameCode
            End If
        End Function

        Public Overrides Async Function Build() As Task
            Dim modpackDir = GetModPackDir()
            'Dim modpackModsDir = GetModsDir()
            'Dim modpackToolsDir = GetToolsDir()
            'Dim modpackToolsPatchersDir = GetPatchersDir()
            Dim modsSourceDir = GetSourceModsDir()
            Dim outputDir = GetOutputDir()

            'Create missing directories
            If Not Directory.Exists(modsSourceDir) Then
                Directory.CreateDirectory(modsSourceDir)
            End If
            If Not Directory.Exists(outputDir) Then
                Directory.CreateDirectory(outputDir)
            End If

            'Copy external mods
            For Each item In IO.Directory.GetFiles(modsSourceDir)
                ModBuilder.CopyMod(item, modpackDir, True)
            Next

            'Copy mods from other projects
            For Each item In Me.GetReferences()
                If TypeOf item Is GenericModProject Then
                    Dim sourceFilename = DirectCast(item, GenericModProject).GetModOutputFilename(BaseRomProject)
                    ModBuilder.CopyMod(sourceFilename, modpackDir, True)
                End If
            Next

            ModBuilder.CopyPatcherProgram(modpackDir)

            'Update modpack info
            Me.Info.GameCode = GetBaseGameCode(ParentSolution)
            Me.Info.System = GetBaseRomSystem(ParentSolution)
            ModBuilder.SaveModpackInfo(modpackDir, Me.Info)

            '-Zip it
            ModBuilder.ZipModpack(modpackDir, Path.Combine(outputDir, Me.Info.Name & " " & Me.Info.Version & ".zip"))

            'Apply patch
            Me.Progress = 0.9
            Me.Message = My.Resources.Language.LoadingApplyingPatch

            Await ApplyPatchAsync(ParentSolution)

            Me.Progress = 1
            Me.Message = My.Resources.Language.Complete
        End Function

        Public Overridable Async Function ApplyPatchAsync(solution As Solution) As Task
            Select Case GetBaseRomSystem(solution)
                Case "3DS"
                    If OutputDec3DSFile Then
                        Await ConsoleApp.RunProgram(Path.Combine(GetModPackDir, "DSPatcher.exe"), String.Format("""{0}"" ""{1}"" -output-3ds", GetBaseRomFilename(solution), Path.Combine(GetOutputDir, "PatchedRom.3ds")))
                    End If
                    If OutputEnc3DSFile Then
                        Await ConsoleApp.RunProgram(Path.Combine(GetModPackDir, "DSPatcher.exe"), String.Format("""{0}"" ""{1}"" -output-3ds -key0", GetBaseRomFilename(solution), Path.Combine(GetOutputDir, "PatchedRom.3ds")))
                    End If
                    If OutputCIAFile Then
                        Await ConsoleApp.RunProgram(Path.Combine(GetModPackDir, "DSPatcher.exe"), String.Format("""{0}"" ""{1}"" -output-cia", GetBaseRomFilename(solution), Path.Combine(GetOutputDir, "PatchedRom.cia")))
                    End If
                    If OutputHans Then
                        Await ConsoleApp.RunProgram(Path.Combine(GetModPackDir, "DSPatcher.exe"), String.Format("""{0}"" ""{1}"" -output-hans", GetBaseRomFilename(solution), Path.Combine(GetOutputDir, "SD Card")))
                    End If
                    If OutputLuma Then
                        Await ConsoleApp.RunProgram(Path.Combine(GetModPackDir, "DSPatcher.exe"), String.Format("""{0}"" ""{1}"" -output-luma", GetBaseRomFilename(solution), Path.Combine(GetOutputDir, "SD Card")))
                    End If
                Case "NDS"
                    Await ConsoleApp.RunProgram(Path.Combine(GetModPackDir, "DSPatcher.exe"), String.Format("""{0}"" ""{1}"" -output-nds", GetBaseRomFilename(solution), Path.Combine(GetOutputDir, "PatchedRom.nds")))
            End Select
        End Function
    End Class

End Namespace
