Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Windows
Imports SkyEditor.Core.Windows.Processes

Namespace FileFormats.Explorers
    Public Class DecompressedFile
        Inherits GenericFile

        Protected Property IsAT4PX As Boolean

        Public Shared Async Function RunDecompress(sourceFilename As String, destinationFilename As String) As Task
            Using external As New ExternalProgramManager
                Await external.RunPPMDUnPX(String.Format("""{0}"" ""{1}""", sourceFilename, destinationFilename))
            End Using
        End Function

        Public Shared Async Function RunCompress(sourceFilename As String, destinationFilename As String) As Task
            Using external As New ExternalProgramManager
                'Todo: specify encryption
                Await external.RunPPMDPXComp(String.Format("""{0}"" ""{1}""", sourceFilename, destinationFilename))
            End Using
        End Function

        ''' <summary>
        ''' Saves and compresses the DecompressedFile.
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Save(Path As String, provider As IOProvider)
            Dim tempFilename = provider.GetTempFilename
            If IsAT4PX Then
                IO.File.Move(tempFilename, tempFilename & ".at4px")
                tempFilename = tempFilename & ".at4px"
            End If
            MyBase.Save(tempFilename, provider)
            RunCompress(tempFilename, Path).Wait()
            Me.OriginalFilename = Path
        End Sub

        Public Overrides Async Function OpenFile(filename As String, provider As IOProvider) As Task
            Dim tempFilename = provider.GetTempFilename
            Await RunDecompress(filename, tempFilename)
            Await MyBase.OpenFile(tempFilename, provider)
            Me.OriginalFilename = filename
        End Function

        Public Sub New()
            Me.EnableInMemoryLoad = True
            IsAT4PX = False
        End Sub
    End Class
End Namespace