Imports System.IO

Namespace ProcessManagement

    ''' <summary>
    ''' A wrapper for a console application that redirects standard input and output
    ''' </summary>
    Public Class ConsoleApp
        Implements IDisposable

        ''' <summary>
        ''' Runs a program and returns the standard input and output, or null if disabled
        ''' </summary>
        ''' <exception cref="UnsuccessfulExitCodeException">Thrown if the process exit code is unsuccessful</exception>
        Public Shared Async Function RunProgram(filename As String, arguments As String, Optional redirectOutput As Boolean = True, Optional redirectError As Boolean = True) As Task(Of String)
            Using app As New ConsoleApp(filename, arguments, redirectOutput, redirectError)

                Dim output = Await app.GetAllOutput()

                If redirectOutput Then
                    Return output
                Else
                    Return Nothing
                End If
            End Using
        End Function

        Public Sub New(filename As String, arguments As String, Optional captureConsoleOutput As Boolean = True, Optional captureConsoleError As Boolean = True)
            Dim p As New Process
            p.StartInfo.FileName = filename
            p.StartInfo.WorkingDirectory = Path.GetDirectoryName(filename)
            p.StartInfo.Arguments = arguments
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            p.StartInfo.CreateNoWindow = True
            p.StartInfo.RedirectStandardOutput = captureConsoleOutput
            p.StartInfo.RedirectStandardError = captureConsoleError
            p.StartInfo.UseShellExecute = False

            If p.StartInfo.RedirectStandardOutput Then
                AddHandler p.OutputDataReceived, AddressOf Instance_OutputDataReceived
            End If
            If p.StartInfo.RedirectStandardError Then
                AddHandler p.ErrorDataReceived, AddressOf Instance_ErrorDataReceived
            End If

            _process = p
        End Sub

        Private WithEvents _process As Process

        Private Property StandardOut As New Text.StringBuilder
        Private Property StandardError As New Text.StringBuilder

        Private Sub Instance_OutputDataReceived(sender As Object, e As DataReceivedEventArgs) Handles _process.OutputDataReceived
            StandardOut.AppendLine(e.Data)
        End Sub

        Private Sub Instance_ErrorDataReceived(sender As Object, e As DataReceivedEventArgs) Handles _process.OutputDataReceived
            StandardError.AppendLine(e.Data)
        End Sub

        Protected Async Function WaitForExit() As Task(Of ExecutionResult)
            Await Task.Run(Sub() _process.WaitForExit())

            Return New ExecutionResult With {.ExitCode = _process.ExitCode, .ProgramName = Path.GetFileName(_process.StartInfo.FileName)}
        End Function

        Protected Overridable Sub Start()
            _process.Start()
            If _process.StartInfo.RedirectStandardOutput Then
                _process.BeginOutputReadLine()
            End If
            If _process.StartInfo.RedirectStandardError Then
                _process.BeginErrorReadLine()
            End If
        End Sub


        ''' <exception cref="UnsuccessfulExitCodeException">Thrown if the process exit code is unsuccessful</exception>
        Public Async Function GetAllOutput() As Task(Of String)
            Start()
            Dim result = Await WaitForExit()
            result.EnsureExitCodeSuccessful(StandardError.ToString)
            Return StandardOut.ToString()
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing AndAlso _process IsNot Nothing Then
                    If _process.StartInfo.RedirectStandardOutput Then
                        RemoveHandler _process.OutputDataReceived, AddressOf Instance_OutputDataReceived
                        RemoveHandler _process.ErrorDataReceived, AddressOf Instance_OutputDataReceived
                    End If

                    If _process.StartInfo.RedirectStandardError Then
                        RemoveHandler _process.ErrorDataReceived, AddressOf Instance_OutputDataReceived
                    End If

                    ' TODO: dispose managed state (managed objects).
                    _process.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class

End Namespace