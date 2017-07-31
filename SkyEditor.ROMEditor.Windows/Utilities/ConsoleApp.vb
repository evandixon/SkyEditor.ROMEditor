Imports System.IO

Public Class ConsoleApp
    Public Shared Async Function RunProgram(Filename As String, Arguments As String) As Task
        ''Todo: figure out why this style isn't properly waiting for exit
        'Using program As New ConsoleApp(Filename, Arguments)
        '    program.WorkingDirectory = Path.GetDirectoryName(Filename)
        '    program.StartConsole()
        '    Await program.WaitForExit
        'End Using

        'Set up the process
        Dim p As New Process()
        p.StartInfo.FileName = Filename
        p.StartInfo.Arguments = Arguments
        p.StartInfo.RedirectStandardOutput = False
        p.StartInfo.UseShellExecute = False
        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        p.StartInfo.CreateNoWindow = True
        p.StartInfo.WorkingDirectory = Path.GetDirectoryName(Filename)

        'Start the process
        p.Start()

        ''Needed to prevent infinite wait at WaitForExit
        'AddHandler p.OutputDataReceived, Sub(sender As Object, e As DataReceivedEventArgs)
        '                                     Console.WriteLine(e.Data)
        '                                 End Sub

        'p.BeginOutputReadLine()

        'Wait for the process to close
        Await WaitForProcess(p).ConfigureAwait(False)

        p.Dispose()
    End Function

    Private Shared Async Function WaitForProcess(p As Process) As Task
        Await Task.Run(Sub()
                           p.WaitForExit()
                       End Sub)
    End Function
End Class
