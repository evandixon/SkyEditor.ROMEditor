﻿Imports SkyEditor.Core.Windows.Processes

Namespace Windows
    Public Class Java

        Public Shared Async Function RunJar(JarPath As String, Arguments As String, WorkingDirectory As String) As Task
            Dim args As New Text.StringBuilder
            args.Append("-jar ")
            args.Append($"""{JarPath}""")
            If Not String.IsNullOrEmpty(Arguments) Then
                args.Append(" ")
                args.Append(Arguments)
            End If
            Await ConsoleApp.RunProgram("java", args.ToString)
        End Function
    End Class
End Namespace
