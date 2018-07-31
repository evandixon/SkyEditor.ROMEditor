Imports System.ComponentModel

Namespace ProcessManagement
    Public Class Java
        Inherits ConsoleApp

        Public Sub New(jarFilename As String, arguments As String, Optional captureConsoleOutput As Boolean = True, Optional captureConsoleError As Boolean = True)
            MyBase.New("java",
                       $"-jar ""{jarFilename}""" &
                       If( 'If arguments are present, add a space before them; otherwise, don't
                           Not String.IsNullOrEmpty(arguments),
                           $" ""{arguments}""",
                           ""),
                       captureConsoleOutput, captureConsoleError
                       )
        End Sub

        Protected Overrides Sub Start()
            Try
                MyBase.Start()
            Catch ex As Win32Exception
                If ex.NativeErrorCode = 2 Then
                    '2 means "file not found". Additional reading: https://msdn.microsoft.com/en-us/library/cc231199.aspx
                    Throw New JavaNotFoundException(ex)
                Else
                    Throw
                End If
            End Try
        End Sub

    End Class
End Namespace

