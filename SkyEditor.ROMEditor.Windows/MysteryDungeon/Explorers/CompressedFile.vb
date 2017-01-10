Imports PPMDU
Imports SkyEditor.Core.IO

Namespace MysteryDungeon.Explorers
    Public Class CompressedFile
        Inherits GenericFile

        Public Sub New()
            Me.EnableInMemoryLoad = True
            IsAT4PX = False
        End Sub

        Protected Property IsAT4PX As Boolean

        Protected Property TempFilename As String

        Public Overrides Async Function OpenFile(filename As String, provider As IOProvider) As Task
            TempFilename = provider.GetTempFilename

            Using external As New UtilityManager
                Await external.RunUnPX(filename, TempFilename)
            End Using

            Await MyBase.OpenFile(TempFilename, provider)

            Me.OriginalFilename = filename
        End Function

        ''' <summary>
        ''' Saves and compresses the DecompressedFile.
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Async Function Save(filename As String, provider As IOProvider) As Task

            Await MyBase.Save(TempFilename, provider)

            Using external As New UtilityManager
                Dim format As PXFormat
                If IsAT4PX Then
                    format = PXFormat.AT4PX
                Else
                    format = PXFormat.PKDPX
                End If

                Await external.RunDoPX(TempFilename, filename, format)
            End Using

            Me.OriginalFilename = filename
        End Function

    End Class
End Namespace