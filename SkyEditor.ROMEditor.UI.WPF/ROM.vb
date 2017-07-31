Imports System.IO
Imports System.Windows.Media.Imaging
Imports SkyEditor.Core
Imports SkyEditor.Core.Utilities
Imports SkyEditor.Core.Windows
Imports SkyEditor.ROMEditor.Windows

Public Class ROM
    Public Property Name As String
    Public ReadOnly Property Filename As String
        Get
            Dim romDirectory As String = EnvironmentPaths.GetResourceName("Roms/NDS/")
            Return Path.Combine(romDirectory, Name.Replace(":", ""))
        End Get
    End Property
    Public ReadOnly Property ImageUri As Uri
        Get
            If File.Exists(Filename) Then
                Dim newpath = Path.Combine(EnvironmentPaths.GetResourceName("Temp"), Path.GetFileNameWithoutExtension(Name.Replace(":", "")) & ".bmp")
                If Not File.Exists(newpath) Then
                    If Not Directory.Exists(Path.GetDirectoryName(newpath)) Then
                        Directory.CreateDirectory(Path.GetDirectoryName(newpath))
                    End If
                    DSIconTool.ExtractIcon(Filename, newpath)
                End If
                Return New Uri(newpath)
            Else
                Return Nothing
            End If
        End Get
    End Property
    Public ReadOnly Property ImageSource As BitmapImage
        Get
            Dim u = ImageUri
            If u IsNot Nothing AndAlso File.Exists(u.AbsolutePath) Then
                Return New BitmapImage(u)
            Else
                Return New BitmapImage
            End If
        End Get
    End Property
    Public Sub New(Name As String)
        Me.Name = Name
    End Sub
    Public Sub New()
        Me.New("")
    End Sub
End Class