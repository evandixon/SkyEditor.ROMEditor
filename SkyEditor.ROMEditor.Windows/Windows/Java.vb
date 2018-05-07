Imports System.IO
Imports System.Windows.Forms

Namespace Windows
    Public Class Java

        Public Shared Async Function RunJar(JarPath As String, Arguments As String, WorkingDirectory As String) As Task
            Dim args As New Text.StringBuilder

            Try
                args.Append("-jar ")
                args.Append($"""{JarPath}""")
                Console.WriteLine("Try has Finished")
            Catch ex As FileNotFoundException
                MessageBox.Show("Java Not Found: " & ex.Message)
            Catch ex As Exception
                MessageBox.Show("Unknown Error occured when calling Java: " & ex.Message)
            Finally
                Console.WriteLine("Java exception handler finished")
            End Try

            If Not String.IsNullOrEmpty(Arguments) Then
                args.Append(" ")
                args.Append(Arguments)
            End If
            Await ConsoleApp.RunProgram("java", args.ToString)

        End Function
    End Class
End Namespace

