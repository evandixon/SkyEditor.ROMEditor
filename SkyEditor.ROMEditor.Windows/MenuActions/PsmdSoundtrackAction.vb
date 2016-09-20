Imports System.IO
Imports System.Reflection
Imports System.Text.RegularExpressions
Imports MediaToolkit
Imports MediaToolkit.Model
Imports MediaToolkit.Options
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Projects
Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Utilities
Imports SkyEditor.Core.Windows
Imports SkyEditor.ROMEditor.Windows.MysteryDungeon.PSMD
Imports SkyEditor.ROMEditor.Windows.Projects
Imports TagLib

Namespace MenuActions
    Public Class PsmdSoundtrackMenuAction
        Inherits MenuAction

        Public Overrides Function SupportedTypes() As IEnumerable(Of TypeInfo)
            Return {GetType(BaseRomProject).GetTypeInfo}
        End Function

        Public Overrides Function SupportsObject(Obj As Object) As Boolean
            If TypeOf Obj Is BaseRomProject Then
                Return PSMDSoundtrackConverter.SupportsProject(Obj)
            Else
                Return False
            End If
        End Function

        Public Overrides Async Sub DoAction(Targets As IEnumerable(Of Object))
            For Each project As BaseRomProject In Targets
                Await PSMDSoundtrackConverter.Convert(project)
            Next
        End Sub

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuUtilities, My.Resources.Language.MenuUtilitiesExportSoundtrack})
            SortOrder = 4.1
            IsContextBased = True
        End Sub
    End Class

End Namespace
