Imports System.Reflection
Imports DS_ROM_Patcher
Imports SkyEditor.Core
Imports SkyEditor.Core.Projects
Imports SkyEditor.Core.UI
Imports SkyEditor.ROMEditor.Projects

Public Class DSModSolution
    Inherits Solution

    Public Overrides Function CanCreateDirectory(Path As String) As Boolean
        Return True
    End Function

    Public Overrides Function CanCreateProject(Path As String) As Boolean
        Return (Path.Replace("\", "/").TrimStart("/") = "")
    End Function

    Public Overrides Function GetSupportedProjectTypes(Path As String, manager As PluginManager) As IEnumerable(Of TypeInfo)
        Dim baseRomProject As BaseRomProject = GetProjectsByName(Me.Settings("BaseRomProject")).FirstOrDefault
        If baseRomProject Is Nothing OrElse baseRomProject.RomSystem Is Nothing OrElse baseRomProject.GameCode Is Nothing Then
            Return {}
        Else
            Dim matches As New List(Of TypeInfo)
            For Each item In manager.GetRegisteredObjects(GetType(GenericModProject))
                Dim games = item.GetSupportedGameCodes
                Dim match As Boolean = False
                For Each t In games
                    Dim r As New Text.RegularExpressions.Regex(t)
                    If r.IsMatch(baseRomProject.GameCode) Then
                        matches.Add(item.GetType)
                    End If
                Next
            Next
            matches.Add(GetType(DSModPackProject))
            Return matches
        End If
    End Function

    Public Overrides Async Function Initialize() As Task
        Me.Settings("BaseRomProject") = "BaseRom"
        Me.Settings("ModPackProject") = "ModPack"
        Await AddNewProject("/", "BaseRom", GetType(BaseRomProject), CurrentPluginManager)
        Await AddNewProject("/", "ModPack", GetType(DSModPackProject), CurrentPluginManager)
    End Function

    Public Overrides Async Function Build() As Task
        Dim info As ModpackInfo = Me.Settings("ModpackInfo")
        If info Is Nothing Then
            info = New ModpackInfo
            Me.Settings("ModpackInfo") = info
        End If
        Dim baseRomProject As BaseRomProject = GetProjectsByName(Me.Settings("BaseRomProject")).FirstOrDefault
        If baseRomProject IsNot Nothing Then
            info.System = baseRomProject.RomSystem
            info.GameCode = baseRomProject.GameCode
            Me.Settings("System") = info.System
            Me.Settings("GameCode") = info.GameCode
        End If
        Await MyBase.Build()
        'Dim modPacks As New List(Of DSModPackProject)
        'Dim allProjects As New List(Of Project)(Me.GetAllProjects)
        'Dim built As Integer = 0
        'For Each item In allProjects
        '    PluginHelper.SetLoadingStatus(PluginHelper.GetLanguageItem("Building projects..."), built / allProjects.Count)
        '    If TypeOf item Is DSModPackProject Then
        '        modPacks.Add(item)
        '    Else
        '        Await item.Build(Me)
        '        built += 1
        '    End If
        'Next
        'For Each item In modPacks
        '    PluginHelper.SetLoadingStatus(PluginHelper.GetLanguageItem("Building projects..."), built / allProjects.Count)
        '    Await item.Build(Me)
        '    built += 1
        'Next
        'PluginHelper.SetLoadingStatusFinished()
    End Function

    Public Function GetBaseRomProject() As BaseRomProject
        Return GetProjectsByName(Me.Settings("BaseRomProject")).First
    End Function

    Public Overrides ReadOnly Property RequiresInitializationWizard As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides Function GetInitializationWizard() As Wizard
        Return New DsModSolutionInitializationWizard(Me)
    End Function
End Class
