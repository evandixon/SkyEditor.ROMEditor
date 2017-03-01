Imports System.Reflection
Imports System.Windows.Forms
Imports SkyEditor.Core.UI
Imports SkyEditor.ROMEditor.MysteryDungeon.Explorers

Namespace MenuActions
    Public Class BGPImageExport
        Inherits MenuAction

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuImage, My.Resources.Language.MenuImageExport})
            SaveFileDialog1 = New SaveFileDialog
            SaveFileDialog1.Filter = $"{My.Resources.Language.PNGImages} (*.png)|*.png|{My.Resources.Language.BitmapImages} (*.bmp)|*.bmp"
            SortOrder = 4.1
        End Sub

        Private WithEvents SaveFileDialog1 As SaveFileDialog

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of TypeInfo)
            Return {GetType(BGP).GetTypeInfo}
        End Function

        Public Overrides Sub DoAction(Targets As IEnumerable(Of Object))
            For Each item As BGP In Targets
                If SaveFileDialog1.ShowDialog = DialogResult.OK Then
                    Dim format As System.Drawing.Imaging.ImageFormat
                    Select Case SaveFileDialog1.FilterIndex
                        Case 0
                            format = System.Drawing.Imaging.ImageFormat.Png
                        Case 1
                            format = System.Drawing.Imaging.ImageFormat.Bmp
                        Case Else
                            format = System.Drawing.Imaging.ImageFormat.Png
                    End Select
                    item.GetImage.Save(SaveFileDialog1.FileName, format)
                End If
            Next
        End Sub
        
    End Class
End Namespace

