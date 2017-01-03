Imports System.IO
Imports DS_ROM_Patcher
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Projects
Imports SkyEditor.ROMEditor.MysteryDungeon.Explorers.ViewModels
Imports SkyEditor.ROMEditor.Projects

Namespace MysteryDungeon.Explorers.Projects
    Public Class SkyStarterModProject
        Inherits GenericModProject

        Public Overrides Function GetFilesToCopy(solution As Solution, baseRomProjectName As String) As IEnumerable(Of String)
            Return {IO.Path.Combine("overlay", "overlay_0013.bin"),
                    IO.Path.Combine("data", "MESSAGE", "text_e.str"),
                    IO.Path.Combine("data", "MESSAGE", "text_f.str"),
                    IO.Path.Combine("data", "MESSAGE", "text_s.str"),
                    IO.Path.Combine("data", "MESSAGE", "text_i.str"),
                    IO.Path.Combine("data", "MESSAGE", "text_g.str"),
                    IO.Path.Combine("data", "MESSAGE", "text_j.str")}
        End Function
        Public Overrides Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {GameStrings.SkyCode}
        End Function

        Public Overrides Function GetCustomFilePatchers() As IEnumerable(Of FilePatcher)
            Dim patchers = New List(Of FilePatcher)
            If patchers Is Nothing Then
                patchers = New List(Of FilePatcher)
            End If
            Dim LSPatcher As New FilePatcherJson()
            Dim lsFilename = Path.GetFileName(GetType(LanguageStringPatcher.LanguageString).Assembly.Location)
            Dim toolsDir = Path.GetDirectoryName(GetType(LanguageStringPatcher.LanguageString).Assembly.Location)
            With LSPatcher
                .CreatePatchProgram = lsFilename
                .CreatePatchArguments = "-c ""{0}"" ""{1}"" ""{2}"""
                .ApplyPatchProgram = lsFilename
                .ApplyPatchArguments = "-a ""{0}"" ""{1}"" ""{2}"""
                .IsPatchMergeSafe = True
                .PatchExtension = "textstrlsp"
                .FilePath = ".*text_.\.str"
                .Dependencies = New List(Of String)
            End With
            patchers.Add(New FilePatcher(LSPatcher, toolsDir))
            Return patchers
        End Function

        Protected Overrides Async Function Initialize() As Task
            Await MyBase.Initialize

            Dim rawDir = GetRawFilesDir()
            Dim projDir = GetRootDirectory()


            Me.BuildProgress = 0
            Me.IsBuildProgressIndeterminate = True
            Me.BuildStatusMessage = My.Resources.Language.LoadingConvertingLanguages

            'Convert Languages
            Dim languageDictionary As New Dictionary(Of String, String)
            languageDictionary.Add("text_e.str", "English")
            languageDictionary.Add("text_f.str", "Français")
            languageDictionary.Add("text_s.str", "Español")
            languageDictionary.Add("text_i.str", "Italiano")
            languageDictionary.Add("text_g.str", "Deutsche") 'German
            languageDictionary.Add("text_j.str", "日本語") 'Japanese

            If Not IO.Directory.Exists(IO.Path.Combine(projDir, "Languages")) Then
                IO.Directory.CreateDirectory(IO.Path.Combine(projDir, "Languages"))
            End If
            For Each item In languageDictionary
                If IO.File.Exists(IO.Path.Combine(rawDir, "Data", "MESSAGE", item.Key)) Then
                    Using langString = New LanguageString()
                        Await langString.OpenFile(IO.Path.Combine(rawDir, "Data", "MESSAGE", item.Key), CurrentPluginManager.CurrentIOProvider)
                        Dim langList As New ObjectFile(Of List(Of String))(CurrentPluginManager.CurrentIOProvider)
                        langList.ContainedObject = langString.Items
                        Await langList.Save(IO.Path.Combine(projDir, "Languages", item.Value), CurrentPluginManager.CurrentIOProvider)
                    End Using
                End If
            Next

            'Add Personality Test
            Me.AddExistingFileToPath("/Starter Pokemon", Path.Combine(rawDir, "Overlay", "overlay_0013.bin"), GetType(Overlay13), CurrentPluginManager.CurrentIOProvider)

            Me.BuildProgress = 1
            Me.IsBuildProgressIndeterminate = False
            Me.BuildStatusMessage = My.Resources.Language.Complete
        End Function

        Protected Overrides Async Function DoBuild() As Task
            Dim rawDir = GetRawFilesDir()
            Dim projDir = GetRootDirectory()

            'Open Personality Test
            Dim personalityTest As New PersonalityTestContainer(Await Me.GetFileByPath("/Starter Pokemon", CurrentPluginManager, AddressOf IOHelper.PickFirstDuplicateMatchSelector))

            'Convert Languages
            Dim languageDictionary As New Dictionary(Of String, String)
            languageDictionary.Add("text_e.str", "English")
            languageDictionary.Add("text_f.str", "Français")
            languageDictionary.Add("text_s.str", "Español")
            languageDictionary.Add("text_i.str", "Italiano")
            languageDictionary.Add("text_g.str", "Deutsche") 'German
            languageDictionary.Add("text_j.str", "日本語") 'Japanese
            For Each item In languageDictionary
                If IO.File.Exists(IO.Path.Combine(projDir, "Languages", item.Value)) Then
                    Dim langFile As New ObjectFile(Of List(Of String))(CurrentPluginManager.CurrentIOProvider, IO.Path.Combine(projDir, "Languages", item.Value))
                    Using langString As New LanguageString
                        langString.CreateFile("")
                        langString.Items = langFile.ContainedObject

                        If personalityTest IsNot Nothing Then
                            langString.UpdatePersonalityTestResult(personalityTest)
                        End If

                        Await langString.Save(IO.Path.Combine(rawDir, "Data", "MESSAGE", item.Key), CurrentPluginManager.CurrentIOProvider)
                    End Using
                End If
            Next

            Await MyBase.DoBuild
        End Function

        Private Sub SkyStarterModProject_ProjectOpened(sender As Object, e As EventArgs) Handles Me.ProjectOpened

        End Sub
    End Class

End Namespace
