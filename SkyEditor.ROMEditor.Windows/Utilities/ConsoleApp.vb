Imports System.IO

Public Class ConsoleApp
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
End Class
