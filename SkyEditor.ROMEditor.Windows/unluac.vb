Imports SkyEditor.Core.Windows

Public Class unluac
    ''' <summary>
    ''' Creates a new instance of the lua decompiler.
    ''' </summary>
    ''' <param name="Filename">Filename of the compiled lua script.</param>
    Public Sub New(Filename As String)
        Dim args As New Text.StringBuilder

        Instance = New Process
        Instance.StartInfo.FileName = "java"
        Instance.StartInfo.Arguments = $"-jar ""{EnvironmentPaths.GetResourceName("unluac.jar")}"" ""{Filename}"""
        Instance.StartInfo.RedirectStandardOutput = True
        Instance.StartInfo.UseShellExecute = False
        Instance.StartInfo.CreateNoWindow = True
    End Sub

    Private Sub Start()
        Instance.Start()
        Instance.BeginOutputReadLine()
    End Sub

    Public Function Decompile() As String
        Start()
        Instance.WaitForExit()
        Return Output.ToString
    End Function

    Private Sub Instance_OutputDataReceived(sender As Object, e As DataReceivedEventArgs) Handles Instance.OutputDataReceived
        Output.AppendLine(e.Data)
    End Sub

    Private WithEvents Instance As Process
    Private Property Output As New Text.StringBuilder

    Public Shared Sub DecompileToFile(SourceFilename As String, DestinationFilename As String)
        Dim un As New unluac(SourceFilename)
        IO.File.WriteAllText(DestinationFilename, un.Decompile)
    End Sub
End Class
