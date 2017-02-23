Imports System.Drawing
Imports System.Reflection
Imports System.Windows.Forms
Imports SkyEditor.Core.UI
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD

Namespace MenuActions
    Public Class CteImageImport
        Inherits MenuAction
        Private WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
        Public Overrides Function GetSupportedTypes() As IEnumerable(Of TypeInfo)
            Return {GetType(CteImage).GetTypeInfo}
        End Function
        Public Overrides Sub DoAction(Targets As IEnumerable(Of Object))
            For Each item As CteImage In Targets
                If OpenFileDialog1.ShowDialog = DialogResult.OK Then
                    item.ContainedImage = Bitmap.FromFile(OpenFileDialog1.FileName)
                End If
            Next
        End Sub
        Public Sub New()
            MyBase.New({My.Resources.Language.MenuImage, My.Resources.Language.MenuImageImport})
            OpenFileDialog1 = New OpenFileDialog
            OpenFileDialog1.Filter = $"{My.Resources.Language.PNGImages} (*.png)|*.png|{My.Resources.Language.BitmapImages} (*.bmp)|*.bmp|{My.Resources.Language.AllFiles} (*.*)|*.*"
            SortOrder = 4.2
        End Sub
    End Class
End Namespace

