Imports System.IO
Imports SkyEditor.Core.ConsoleCommands
Imports SkyEditor.Core.IO
Imports SkyEditor.IO.FileSystem
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD

Namespace Windows.ConsoleCommands
    Public Class BatchCteConvert
        Inherits ConsoleCommand

        Public Sub New(FileSystem As IFileSystem)
            If FileSystem Is Nothing Then
                Throw New ArgumentNullException(NameOf(FileSystem))
            End If

            CurrentFileSystem = FileSystem
        End Sub

        Protected Property CurrentFileSystem As IFileSystem

        Public Overrides Async Function MainAsync(Arguments() As String) As Task
            Dim SourceDir = Arguments(0)
            If Not Directory.Exists(SourceDir) Then
                Console.WriteLine($"Invalid dir ""{SourceDir}""")
                Exit Function
            End If
            For Each item In Directory.GetFiles(SourceDir)
                Try
                    Using c As New CteImage
                        Await c.OpenFile(item, CurrentFileSystem)
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
