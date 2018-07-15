Imports SkyEditor.Core.ConsoleCommands
Imports SkyEditor.Core.IO
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD

Namespace Windows.ConsoleCommands
    Public Class BatchCteConvert
        Inherits ConsoleCommand

        Public Sub New(ioProvider As IIOProvider)
            If ioProvider Is Nothing Then
                Throw New ArgumentNullException(NameOf(ioProvider))
            End If

            CurrentIOProvider = ioProvider
        End Sub

        Protected Property CurrentIOProvider As IIOProvider

        Public Overrides Async Function MainAsync(Arguments() As String) As Task
            Dim SourceDir = Arguments(0)
            If Not IO.Directory.Exists(SourceDir) Then
                Console.WriteLine($"Invalid dir ""{SourceDir}""")
                Exit Function
            End If
            For Each item In IO.Directory.GetFiles(SourceDir)
                Try
                    Using c As New CteImage
                        Await c.OpenFile(item, CurrentIOProvider)
                        c.ContainedImage.Save(item & ".png", Drawing.Imaging.ImageFormat.Png)
                        Console.WriteLine("Converted " & item)
                    End Using
                Catch ex As Exception
                    Console.WriteLine("Failed " & item)
                    Console.WriteLine(ex)
                End Try
            Next
        End Function
    End Class

End Namespace
