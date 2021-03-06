﻿Imports System.Threading.Tasks
Imports SkyEditor.Core.IO
Imports SkyEditor.IO.FileSystem

''' <summary>
''' Models a basic file that stores Key/Value pairs.
''' </summary>
Public Class BasicIniFile
    Implements IOpenableFile

    Public Property Entries As Dictionary(Of String, String)

    Public Function OpenFile(Filename As String, Provider As IFileSystem) As Task Implements IOpenableFile.OpenFile
        For Each item In Provider.ReadAllText(Filename).Split(vbLf)
            Dim parts = item.Trim.Split("=".ToCharArray, 2)
            If Not Entries.ContainsKey(parts(0)) Then
                Entries.Add(parts(0), parts(1))
            End If
        Next
        Return Task.FromResult(0)
    End Function

    Public Sub CreateFile(Contents As String)
        For Each item In Contents.Split(vbLf)
            If Not String.IsNullOrWhiteSpace(item) Then
                Dim parts = item.Trim.Split("=".ToCharArray, 2)
                If Not Entries.ContainsKey(parts(0)) Then
                    Entries.Add(parts(0), parts(1))
                End If
            End If
        Next
    End Sub

    Public Sub New()
        Entries = New Dictionary(Of String, String)
    End Sub
End Class
