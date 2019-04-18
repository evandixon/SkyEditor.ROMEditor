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

        Public Overrides Async Function OpenFile(filename As String, provider As IIOProvider) As Task
            Dim tempFilename = GetTempFilename(provider)

            Using external As New UtilityManager
                Await external.RunUnPX(filename, TempFilename)
            End Using

            Await MyBase.OpenFile(TempFilename, provider)

            Me.Filename = filename
        End Function

        ''' <summary>
        ''' Saves and compresses the DecompressedFile.
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Async Function Save(filename As String, provider As IIOProvider) As Task
            Dim tempFilename = GetTempFilename(provider)

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

            Me.Filename = filename
        End Function

        Private Function GetTempFilename(provider As IIOProvider)
            If String.IsNullOrEmpty(_tempFilename) Then
                _ioProvider = provider
                _tempFilename = provider.GetTempFilename
            End If
            Return _tempFilename
        End Function
        Private _tempFilename As String
        Private _ioProvider As IIOProvider

        Protected Overrides Sub Dispose(disposing As Boolean)
            MyBase.Dispose(disposing)
            If Not String.IsNullOrEmpty(_tempFilename) AndAlso _ioProvider IsNot Nothing AndAlso _ioProvider.FileExists(_tempFilename) Then
                _ioProvider.DeleteFile(_tempFilename)
                _tempFilename = Nothing
                _ioProvider = Nothing
            End If
        End Sub

    End Class
End Namespace