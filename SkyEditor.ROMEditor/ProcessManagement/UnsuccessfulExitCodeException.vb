Namespace ProcessManagement
    Public Class UnsuccessfulExitCodeException
        Inherits Exception

        Public Sub New(exitCode As Integer, programName As String)
            MyBase.New(String.Format(My.Resources.Language.ProcessManagement_UnsuccessfulExitCodeExceptionMessage, programName, exitCode))

            Me.ExitCode = exitCode
            Me.ProgramName = programName
        End Sub

        Public Sub New(exitCode As Integer, programName As String, output As String)
            MyBase.New(String.Format(My.Resources.Language.ProcessManagement_UnsuccessfulExitCodeExceptionMessage, programName, exitCode))

            Me.ExitCode = exitCode
            Me.ProgramName = programName
            Me.Output = output
        End Sub

        Public Property ExitCode As Integer
        Public Property ProgramName As String
        Public Property Output As String

        Public Overrides Function ToString() As String
            If Output IsNot Nothing Then
                Return "Exit code '" & ExitCode.ToString() & "' was not successful. Output: " & Output & Environment.NewLine & MyBase.ToString()
            Else
                Return "Exit code '" & ExitCode.ToString() & "' was not successful." & Environment.NewLine & MyBase.ToString()
            End If
        End Function
    End Class
End Namespace