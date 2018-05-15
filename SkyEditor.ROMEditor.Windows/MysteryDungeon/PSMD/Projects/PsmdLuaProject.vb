Imports System.IO
Imports System.Text.RegularExpressions
Imports DS_ROM_Patcher
Imports SkyEditor.Core.Projects
Imports SkyEditor.Core.Utilities
Imports SkyEditor.ROMEditor.Windows
Imports SkyEditor.ROMEditor.Projects
Imports System.Collections.Concurrent

Namespace MysteryDungeon.PSMD.Projects
    ''' <summary>
    ''' A mod project that allows editing the scripts and associated language files of PSMD and GTI.
    ''' </summary>
    Public Class PsmdLuaProject
        Inherits GenericModProject
        Implements IPsmdMessageBinProject

        Public Sub New()
            MyBase.New
            ExistingLanguageIds = New ConcurrentBag(Of UInteger)
            AddScriptsToProject = True
        End Sub

#Region "Properties"

        ''' <summary>
        ''' List of all language IDs that are in use.
        ''' </summary>
        Private Property ExistingLanguageIds As ConcurrentBag(Of UInteger)

        ''' <summary>
        ''' Gets or sets whether or not the scripts will be visibly added to the project during initialization.
        ''' </summary>
        Protected Property AddScriptsToProject As Boolean

        ''' <summary>
        ''' Gets the task that loads the Language file IDs, to avoid duplicate keys.
        ''' </summary>
        Public ReadOnly Property LanguageLoadTask As Task
            Get
                Return _languageLoadTask
            End Get
        End Property
        Dim _languageLoadTask As Task

#End Region

#Region "IPsmdMessageBinProject Implementation"
        ''' <summary>
        ''' Gets the localized language file with the given name.
        ''' </summary>
        ''' <param name="name">Name of the language file of which to get (e.g. "common").</param>
        ''' <returns>The <see cref="MessageBin"/> with the given <paramref name="name"/> for the language appropriate for the current culture.</returns>
        Public Async Function GetLanguageFile(name As String) As Task(Of MessageBin) Implements IPsmdMessageBinProject.GetLanguageFile
            Dim dir = Path.Combine(Me.GetRootDirectory, "Languages")
            Dim availableDirs = Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly)

            Dim languageDir As String
            If availableDirs.Any(Function(x) Path.GetFileName(x.ToLower) = My.Resources.Language.LocalizedPsmdLanguageFileLanguage) Then
                languageDir = Path.Combine(dir, My.Resources.Language.LocalizedPsmdLanguageFileLanguage)
            ElseIf availableDirs.Any(Function(x) Path.GetFileName(x.ToLower) = "us") Then
                languageDir = Path.Combine(dir, "us")
            ElseIf availableDirs.Any(Function(x) Path.GetFileName(x.ToLower) = "en") Then
                languageDir = Path.Combine(dir, "en")
            ElseIf availableDirs.Any(Function(x) Path.GetFileName(x.ToLower) = "jp") Then
                languageDir = Path.Combine(dir, "jp")
            Else
                Throw New IO.DirectoryNotFoundException(String.Format(My.Resources.Language.ErrorLocalizedPsmdLanguageFileLanguageNotFound, My.Resources.Language.LocalizedPsmdLanguageFileLanguage))
            End If

            Dim message As New MessageBin
            Await message.OpenFile(Path.Combine(languageDir, name), CurrentPluginManager.CurrentIOProvider)
            Return message
        End Function

        Public Async Function GetPokemonNames() As Task(Of Dictionary(Of Integer, String)) Implements IPsmdMessageBinProject.GetPokemonNames
            If _pokemonNames Is Nothing Then
                _pokemonNames = (Await GetLanguageFile("common.bin")).GetCommonPokemonNames
            End If
            Return _pokemonNames
        End Function
        Dim _pokemonNames As Dictionary(Of Integer, String)

        Public Async Function GetMoveNames() As Task(Of Dictionary(Of Integer, String)) Implements IPsmdMessageBinProject.GetMoveNames
            If _moveNames Is Nothing Then
                _moveNames = (Await GetLanguageFile("common.bin")).GetCommonMoveNames
            End If
            Return _moveNames
        End Function
        Dim _moveNames As Dictionary(Of Integer, String)
#End Region

        Public Overrides Function CanCreateDirectory(Path As String) As Boolean
            Return True
        End Function

        Public Overrides Function GetCustomFilePatchers() As IEnumerable(Of FilePatcher)
            Dim patchers = New List(Of FilePatcher)
            If patchers Is Nothing Then
                patchers = New List(Of FilePatcher)
            End If
            Dim msPatcher As New FilePatcherJson()
            Dim patcherPath = Path.GetFileName(GetType(Message_FARC_Patcher.FarcF5).Assembly.Location)
            Dim toolsDir = Path.GetDirectoryName(GetType(Message_FARC_Patcher.FarcF5).Assembly.Location)
            With msPatcher
                .CreatePatchProgram = patcherPath
                .CreatePatchArguments = "-c ""{0}"" ""{1}"" ""{2}"""
                .ApplyPatchProgram = patcherPath
                .ApplyPatchArguments = "-a ""{0}"" ""{1}"" ""{2}"""
                .IsPatchMergeSafe = True
                .PatchExtension = "msgFarcT5"
                .FilePath = ".*message_?[A-Za-z]*\.bin"
                .Dependencies = New List(Of String)
            End With
            patchers.Add(New FilePatcher(msPatcher, toolsDir))
            Return patchers
        End Function

        Public Function IsLanguageLoaded() As Boolean
            Return LanguageLoadTask IsNot Nothing AndAlso LanguageLoadTask.IsCompleted
        End Function

        Public Async Function GetNewLanguageId() As Task(Of UInteger)
            'Note: with the current system of logging every ID in use, excessively large numbers of IDs in use will cause the computer to run out of memory.
            'If this happens, we won't be able to store the IDs in memory, and we will instead need to check everything one file at a time.
            'Chances are this won't be a problem.  GTI contains something over 5,000,000 strings.  Just the IDs will take about 20MB of RAM, which should be easy to come by.
            'This could only be a problem because the range of IDs is 0 to UInt32.MaxValue, and besides storage space, there's no technical limitations to having that many strings.

            Dim newId As Integer = 0

            If LanguageLoadTask Is Nothing Then
                LoadLanguageIDs()
            End If

            Await LanguageLoadTask

            'Find a unique ID
            While ExistingLanguageIds.Contains(newId)
                newId += 1
            End While
            ExistingLanguageIds.Add(newId)  'Register the new ID so it can't be used again

            Return newId
        End Function

        Private Sub LoadLanguageIDs()
            'Load language IDs
            Dim dir = IO.Path.Combine(Me.GetRootDirectory, "Languages")
            If IO.Directory.Exists(dir) Then
                Dim langDirs = IO.Directory.GetDirectories(dir)
                Dim f1 As New AsyncFor
                f1.BatchSize = langDirs.Length
                _languageLoadTask = f1.RunForEach(langDirs,
                                                  Async Function(item As String) As Task
                                                      Dim f2 As New AsyncFor
                                                      Await f2.RunForEach(Directory.GetFiles(item),
                                                                          Async Function(file As String) As Task
                                                                              Using msg As New MessageBin(True)
                                                                                  Await msg.OpenFileOnlyIDs(file, CurrentPluginManager.CurrentIOProvider)

                                                                                  For Each entry In msg.Strings
                                                                                      ExistingLanguageIds.Add(entry.Hash)
                                                                                  Next
                                                                              End Using
                                                                          End Function)
                                                      ExistingLanguageIds = New ConcurrentBag(Of UInteger)(ExistingLanguageIds.Distinct())
                                                  End Function)
            End If
        End Sub

        Private Async Function StartExtractLanguages() As Task
            'PSMD style
            Dim languageNameRegex As New Regex(".*message_?(.*)\.bin", RegexOptions.IgnoreCase)
            Dim languageFileNames = Directory.GetFiles(IO.Path.Combine(Me.GetRawFilesDir, "romfs"), "message*.bin", IO.SearchOption.TopDirectoryOnly)
            Dim f As New AsyncFor
            AddHandler f.ProgressChanged, Sub(sender As Object, e As ProgressReportedEventArgs)
                                              Me.Progress = e.Progress
                                          End Sub
            f.RunSynchronously = True
            Await f.RunForEach(languageFileNames, Async Function(item As String) As Task
                                                      Dim lang = "jp"

                                                      Dim match = languageNameRegex.Match(item)
                                                      If match.Success AndAlso Not String.IsNullOrEmpty(match.Groups(1).Value) Then
                                                          lang = match.Groups(1).Value
                                                      End If

                                                      Dim destDir = IO.Path.Combine(Me.GetRootDirectory, "Languages", lang)
                                                      Await FileSystem.EnsureDirectoryExistsEmpty(destDir, CurrentPluginManager.CurrentIOProvider)

                                                      Dim farc As New Farc
                                                      Await farc.OpenFile(item, CurrentPluginManager.CurrentIOProvider)
                                                      Await farc.Extract(destDir, CurrentPluginManager.CurrentIOProvider)
                                                  End Function)

            'GTI style
            Dim languageDirNameRegex As New Regex(".*message_?(.*)", RegexOptions.IgnoreCase)
            Dim languageDirFilenames = Directory.GetDirectories(IO.Path.Combine(Me.GetRawFilesDir, "romfs"), "message*", IO.SearchOption.TopDirectoryOnly)
            Dim f2 As New AsyncFor
            AddHandler f2.ProgressChanged, Sub(sender As Object, e As ProgressReportedEventArgs)
                                               Me.Progress = e.Progress
                                           End Sub
            f.RunSynchronously = True
            Await f2.RunForEach(languageDirFilenames, Async Function(item As String) As Task
                                                          Dim lang = "en"

                                                          Dim match = languageDirNameRegex.Match(item)
                                                          If match.Success AndAlso Not String.IsNullOrEmpty(match.Groups(1).Value) Then
                                                              lang = match.Groups(1).Value
                                                          End If

                                                          Dim destDir = IO.Path.Combine(Me.GetRootDirectory, "Languages", lang)
                                                          Await FileSystem.EnsureDirectoryExistsEmpty(destDir, CurrentPluginManager.CurrentIOProvider)
                                                          Await FileSystem.CopyDirectory(item, destDir, CurrentPluginManager.CurrentIOProvider)
                                                      End Function)
        End Function


        ''' <summary>
        ''' Extracts the language files.
        ''' If called multiple times, only extracts once.
        ''' </summary>
        ''' <returns></returns>
        Protected Function ExtractLanguages() As Task
            If _languageExtractTask Is Nothing Then
                _languageExtractTask = StartExtractLanguages()
            End If
            Return _languageExtractTask
        End Function
        Private _languageExtractTask As Task

        Public Overrides Async Function Initialize() As Task
            Await MyBase.Initialize

            Me.Message = My.Resources.Language.LoadingExtractingLanguages
            Me.Progress = 0
            Me.IsIndeterminate = False

            Await ExtractLanguages()

            Me.Message = My.Resources.Language.LoadingDecompilingScripts
            Me.Progress = 0

            Dim scriptSource As String = IO.Path.Combine(Me.GetRawFilesDir, "romfs", "script")
            Dim scriptDestination As String = IO.Path.Combine(Me.GetRootDirectory, "script")
            Dim filesToOpen As New ConcurrentBag(Of String)

            Dim f As New AsyncFor
            AddHandler f.ProgressChanged, Sub(sender As Object, e As ProgressReportedEventArgs)
                                              Me.Progress = e.Progress
                                          End Sub

            f.BatchSize = Environment.ProcessorCount * 2

            Await f.RunForEach(Directory.GetFiles(scriptSource, "*.lua", IO.SearchOption.AllDirectories),
                               Sub(Item As String)
                                   Dim dest = Item.Replace(scriptSource, scriptDestination)
                                   If Not IO.Directory.Exists(IO.Path.GetDirectoryName(dest)) Then
                                       IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(dest))
                                   End If

                                   unluac.DecompileToFile(Item, dest)
                                   IO.File.Copy(dest, dest & ".original")
                                   filesToOpen.Add(dest)

                                   'Add the file to the project
                                   'Dim d = IO.Path.GetDirectoryName(dest).Replace(scriptDestination, "script")
                                   'Me.CreateDirectory(d)
                                   'Await Me.AddExistingFile(d, Item, False)
                               End Sub)

            'Me.BuildStatusMessage = My.Resources.Language.LoadingAddingFiles
            'Me.BuildProgress = 0
            'Dim f2 As New AsyncFor()
            'f2.RunSynchronously = True

            'AddHandler f2.LoadingStatusChanged, Sub(sender As Object, e As ProgressReportedEventArgs)
            '                                        Me.Progress = e.Progress
            '                                    End Sub

            'Await f2.RunForEach(Async Function(Item As String) As Task

            '                        Me.CreateDirectory(d)
            '                        Await Me.AddExistingFile(d, Item, CurrentPluginManager.CurrentIOProvider)
            '                    End Function, filesToOpen)

            If AddScriptsToProject Then
                For Each item In filesToOpen
                    Dim d = IO.Path.GetDirectoryName(item).Replace(scriptDestination, "script")
                    Me.AddExistingFile(d, item, CurrentPluginManager.CurrentIOProvider)
                Next
            End If

            Me.Progress = 1
            Me.Message = My.Resources.Language.Complete
            Me.IsIndeterminate = False
        End Function

        Public Overrides Async Function Build() As Task
            Dim farcMode As Boolean = False

            If IO.Directory.GetFiles(IO.Path.Combine(Me.GetRawFilesDir, "romfs"), "message*").Length > 0 Then
                farcMode = True
            End If

            If farcMode Then
                Await Task.Run(Async Function() As Task
                                   Dim dirs = Directory.GetDirectories(IO.Path.Combine(Me.GetRootDirectory, "Languages"))
                                   Me.Message = My.Resources.Language.LoadingBuildingLanguageFiles
                                   For count = 0 To dirs.Length - 1
                                       Me.Progress = count / dirs.Length
                                       Dim newFilename As String = "message_" & IO.Path.GetFileNameWithoutExtension(dirs(count)) & ".bin"
                                       Dim newFilePath As String = IO.Path.Combine(IO.Path.Combine(Me.GetRawFilesDir, "romfs", newFilename.Replace("_jp", "")))
                                       Await Farc.Pack(dirs(count), newFilePath, CurrentPluginManager.CurrentIOProvider)
                                   Next
                                   Me.Progress = 1
                               End Function)
            Else
                'Then we're in GTI directory mode
                Await Task.Run(Async Function() As Task
                                   Dim dirs = IO.Directory.GetDirectories(IO.Path.Combine(Me.GetRootDirectory, "Languages"))
                                   Me.Message = My.Resources.Language.LoadingBuildingLanguageFiles
                                   For count = 0 To dirs.Length - 1
                                       Me.Progress = count / dirs.Length
                                       Dim newFilename As String = "message_" & IO.Path.GetFileNameWithoutExtension(dirs(count))
                                       Dim newFilePath As String = IO.Path.Combine(IO.Path.Combine(Me.GetRawFilesDir, "romfs", newFilename.Replace("_en", "")))
                                       Await FileSystem.CopyDirectory(dirs(count), newFilePath, CurrentPluginManager.CurrentIOProvider)
                                   Next
                                   Me.Progress = 1
                               End Function)
            End If

            Dim scriptDestination As String = IO.Path.Combine(Me.GetRawFilesDir, "romfs", "script")
            Dim scriptSource As String = IO.Path.Combine(Me.GetRootDirectory, "script")

            Dim toCompile = From d In IO.Directory.GetFiles(scriptSource, "*.lua", IO.SearchOption.AllDirectories) Where Not d.StartsWith(scriptDestination) Select d

            Me.Message = My.Resources.Language.LoadingCompilingScripts
            Dim f As New AsyncFor
            Dim onProgressChanged = Sub(sender As Object, e As ProgressReportedEventArgs)
                                        Me.Progress = e.Progress
                                    End Sub
            AddHandler f.ProgressChanged, onProgressChanged
            Await f.RunForEach(toCompile,
                               Async Function(Item As String) As Task
                                   Dim sourceText = IO.File.ReadAllText(Item)
                                   Dim sourceOrig = IO.File.ReadAllText(Item & ".original")

                                   If Not sourceText = sourceOrig Then
                                       Dim dest = Item.Replace(scriptSource, scriptDestination)
                                       Await ConsoleApp.RunProgram(EnvironmentPaths.GetResourceName("lua/luac5.1.exe"), $"-o ""{dest}"" ""{Item}""")
                                   End If
                               End Function)
            RemoveHandler f.ProgressChanged, onProgressChanged
            Await MyBase.Build
        End Function

        Public Overrides Function GetFilesToCopy(Solution As Solution, BaseRomProjectName As String) As IEnumerable(Of String)
            Dim project As Project = Solution.GetProjectsByName(BaseRomProjectName).FirstOrDefault
            If project IsNot Nothing AndAlso TypeOf project Is BaseRomProject Then
                Dim code = DirectCast(project, BaseRomProject).GameCode
                Dim psmd As New Regex(GameStrings.PSMDCode)
                Dim gti As New Regex(GameStrings.GTICode)
                If psmd.IsMatch(code) Then
                    Return {IO.Path.Combine("romfs", "script"),
                            IO.Path.Combine("romfs", "message_en.bin"),
                            IO.Path.Combine("romfs", "message_fr.bin"),
                            IO.Path.Combine("romfs", "message_ge.bin"),
                            IO.Path.Combine("romfs", "message_it.bin"),
                            IO.Path.Combine("romfs", "message_sp.bin"),
                            IO.Path.Combine("romfs", "message_us.bin"),
                            IO.Path.Combine("romfs", "message.bin"),
                            IO.Path.Combine("romfs", "message_en.lst"),
                            IO.Path.Combine("romfs", "message_fr.lst"),
                            IO.Path.Combine("romfs", "message_ge.lst"),
                            IO.Path.Combine("romfs", "message_it.lst"),
                            IO.Path.Combine("romfs", "message_sp.lst"),
                            IO.Path.Combine("romfs", "message_us.lst"),
                            IO.Path.Combine("romfs", "message.lst"),
                            IO.Path.Combine("romfs", "message_debug_en.bin"),
                            IO.Path.Combine("romfs", "message_debug_fr.bin"),
                            IO.Path.Combine("romfs", "message_debug_ge.bin"),
                            IO.Path.Combine("romfs", "message_debug_it.bin"),
                            IO.Path.Combine("romfs", "message_debug_sp.bin"),
                            IO.Path.Combine("romfs", "message_debug_us.bin"),
                            IO.Path.Combine("romfs", "message_debug.bin"),
                            IO.Path.Combine("romfs", "message_debug_en.lst"),
                            IO.Path.Combine("romfs", "message_debug_fr.lst"),
                            IO.Path.Combine("romfs", "message_debug_ge.lst"),
                            IO.Path.Combine("romfs", "message_debug_it.lst"),
                            IO.Path.Combine("romfs", "message_debug_sp.lst"),
                            IO.Path.Combine("romfs", "message_debug_us.lst"),
                            IO.Path.Combine("romfs", "message_debug.lst")}
                ElseIf gti.IsMatch(code) Then
                    Return {IO.Path.Combine("romfs", "script"),
                            IO.Path.Combine("romfs", "message_fr"),
                            IO.Path.Combine("romfs", "message_ge"),
                            IO.Path.Combine("romfs", "message_it"),
                            IO.Path.Combine("romfs", "message_sp"),
                            IO.Path.Combine("romfs", "message")}
                Else
                    Return {IO.Path.Combine("romfs", "script")}
                End If
            Else
                Return {IO.Path.Combine("romfs", "script")}
            End If
        End Function

        Public Overrides Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {GameStrings.GTICode, GameStrings.PSMDCode}
        End Function


    End Class

End Namespace
