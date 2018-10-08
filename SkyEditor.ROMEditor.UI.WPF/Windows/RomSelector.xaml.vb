Imports System.IO
Imports System.Windows.Forms
Imports SkyEditor.Core
Imports SkyEditor.Core.Utilities

Public Class RomSelector
    Public Overloads Function ShowDialog(Roms As List(Of String)) As Boolean
        For Each item In Roms
            lvRoms.Items.Add(New ROM(item))
        Next
        Return Me.ShowDialog
    End Function
    Public Property RomName As String
    Sub OnOK()
        Dim romDirectory As String = Core.Utilities.EnvironmentPaths.GetResourceName("Roms/NDS/")
        If Not Directory.Exists(romDirectory) Then
            Directory.CreateDirectory(romDirectory)
        End If
        Dim filename As String = Path.Combine(romDirectory, DirectCast(lvRoms.SelectedItem, ROM).Name.Replace(":", ""))
        If File.Exists(filename) Then
            RomName = filename
            DialogResult = True
            Me.Close()
        Else
            Dim x As New OpenFileDialog
            x.Filter = $"{My.Resources.Language.NDSRomFiles} (*.nds)|*.nds|{My.Resources.Language.AllFiles} (*.*)|*.*"
            If x.ShowDialog = Forms.DialogResult.OK Then
                File.Copy(x.FileName, filename)
                RomName = filename
                DialogResult = True
                Me.Close()
            End If
        End If
    End Sub

    Private Sub btnOK_Click(sender As Object, e As RoutedEventArgs) Handles btnOK.Click
        OnOK()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As RoutedEventArgs) Handles btnCancel.Click
        DialogResult = False
        Me.Close()
    End Sub

    Private Sub RomSelector_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If lvRoms.Items.Count = 1 Then
            lvRoms.SelectedIndex = 0
            OnOK()
        End If
    End Sub

    Private Sub lvRoms_MouseDoubleClick(sender As Object, e As Input.MouseButtonEventArgs) Handles lvRoms.MouseDoubleClick
        If lvRoms.SelectedIndex > -1 Then
            OnOK()
        End If
    End Sub
End Class