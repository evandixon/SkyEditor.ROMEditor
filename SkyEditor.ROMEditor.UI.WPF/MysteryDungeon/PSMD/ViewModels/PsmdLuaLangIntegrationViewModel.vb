Imports System.Collections.ObjectModel
Imports System.IO
Imports SkyEditor.CodeEditor
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Utilities
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD
Imports SkyEditor.UI.WPF

Namespace MysteryDungeon.PSMD.ViewModels
    Public Class PsmdLuaLangIntegrationViewModel
        Inherits GenericViewModel(Of LuaCodeFile)
        Implements INotifyModified
        Implements INamed

        Public Sub New(ioProvider As IIOProvider)
            If ioProvider Is Nothing Then
                Throw New ArgumentNullException(NameOf(ioProvider))
            End If

            CurrentIOProvider = ioProvider
        End Sub

        Public Overridable ReadOnly Property Name As String Implements INamed.Name
            Get
                Return My.Resources.Language.Message
            End Get
        End Property

        Public Property MessageTabs As ObservableCollection(Of TabItem)
        Protected Overridable ReadOnly Property TargetExtension As String = ".bin"
        Protected Property CurrentIOProvider As IIOProvider

        Public Overrides Function SupportsObject(Obj As Object) As Boolean
            Return MyBase.SupportsObject(Obj) AndAlso CurrentApplicationViewModel.GetFileViewModelForModel(Obj)?.ParentProject IsNot Nothing
        End Function

        Protected Overridable Function InitMessageBin() As MessageBin
            Return New MessageBin
        End Function

        Public Overrides Async Sub SetModel(model As Object)
            MyBase.SetModel(model)

            Dim codeFile As LuaCodeFile = model
            Dim project = CurrentApplicationViewModel.GetFileViewModelForModel(codeFile).ParentProject
            Dim scriptName = Path.GetFileNameWithoutExtension(codeFile.Filename)

            'Hack to show common strings in menu_common and related scripts
            If scriptName.Contains("_common") Then
                scriptName = "common"
            End If

            Dim messageFiles As New Dictionary(Of String, MessageBin)
            For Each item In Directory.GetDirectories(Path.Combine(project.GetRootDirectory, "Languages"), "*", SearchOption.TopDirectoryOnly)
                Dim msgfile = InitMessageBin()
                Dim filename = Path.Combine(item, scriptName)

                Dim exists As Boolean = False
                If File.Exists(filename) Then
                    exists = True
                ElseIf File.Exists(filename & TargetExtension) Then
                    filename &= TargetExtension
                    exists = True
                End If

                If exists Then
                    Await msgfile.OpenFile(filename, CurrentIOProvider)
                    messageFiles.Add(Path.GetFileName(item), msgfile)
                End If
            Next

            MessageTabs = New ObservableCollection(Of TabItem)
            For Each item In messageFiles
                Dim m As New MessageBinViewModel(CurrentIOProvider)
                m.SetApplicationViewModel(CurrentApplicationViewModel)
                m.SetModel(item.Value)
                AddHandler m.Modified, AddressOf Me.OnModified

                Dim debugFilename = Path.ChangeExtension(item.Value.Filename.Replace("\", "/").Replace("Languages/", "Languages/debug_"), ".dbin")
                If File.Exists(debugFilename) Then
                    Dim d As New MessageBinDebug
                    Await d.OpenFile(debugFilename, CurrentIOProvider)
                    m.LoadDebugSymbols(d)
                End If

                Dim p As New ObjectControlPlaceholder
                p.CurrentApplicationViewModel = Me.CurrentApplicationViewModel
                p.ObjectToEdit = m

                Dim t As New TabItem
                t.Header = item.Key
                t.Content = p
                MessageTabs.Add(t)
            Next
        End Sub

        Public Overrides Async Sub UpdateModel(model As Object)
            MyBase.UpdateModel(model)

            For Each item As TabItem In MessageTabs
                Await DirectCast(DirectCast(item.Content, ObjectControlPlaceholder).ObjectToEdit, MessageBinViewModel).Save(CurrentIOProvider)
            Next
        End Sub

        Public Event Modified As EventHandler Implements INotifyModified.Modified

        Protected Sub OnModified(sender As Object, e As EventArgs)
            RaiseEvent Modified(Me, e)
        End Sub

        Public Overrides ReadOnly Property SortOrder As Integer
            Get
                Return 2
            End Get
        End Property
    End Class
End Namespace

