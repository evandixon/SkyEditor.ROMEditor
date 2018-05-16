Imports System.IO

Namespace ProcessManagement

    ''' <summary>
    ''' A wrapper for a console application that redirects standard input and output
    ''' </summary>
    Public Class ConsoleApp
        Implements IDisposable

        Public Shared Async Function RunProgram(Filename As String, Arguments As String) As Task
            Using p As New Process()
                p.StartInfo.FileName = Filename
                p.StartInfo.Arguments = Arguments
                p.StartInfo.RedirectStandardOutput = False
                p.StartInfo.UseShellExecute = False
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                p.StartInfo.CreateNoWindow = True
                p.StartInfo.WorkingDirectory = Path.GetDirectoryName(Filename)

                p.Start()

                Await WaitForProcess(p).ConfigureAwait(False)
            End Using
        End Function

        Private Shared Async Function WaitForProcess(p As Process) As Task
            Await Task.Run(Sub()
                               p.WaitForExit()
                           End Sub)
        End Function

        ''' <summary>
        ''' Creates a new instance of the lua decompiler.
        ''' </summary>
        ''' <param name="Filename">Filename of the compiled lua script.</param>
        Public Sub New(filename As String, arguments As String)
            Dim args As New Text.StringBuilder

            _process = New Process
            _process.StartInfo.FileName = filename
            _process.StartInfo.Arguments = arguments
            _process.StartInfo.RedirectStandardOutput = True
            _process.StartInfo.UseShellExecute = False
            _process.StartInfo.CreateNoWindow = True
        End Sub

        Private WithEvents _process As Process

        Private Property Output As New Text.StringBuilder

        Private Sub Instance_OutputDataReceived(sender As Object, e As DataReceivedEventArgs) Handles _process.OutputDataReceived
            Output.AppendLine(e.Data)
        End Sub

        Protected Overridable Sub Start()
            _process.Start()
            _process.BeginOutputReadLine()
        End Sub

        Public Function GetAllOutput() As String
            Start()
            _process.WaitForExit()
            Return Output.ToString()
        End Function

        Public Async Function GetAllOutputAsync() As Task(Of String)
            Start()
            Await WaitForProcess(_process)
            Return Output.ToString()
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
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