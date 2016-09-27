Namespace Soundtrack
    Public Class SoundtrackTrack
        Public Sub New()

        End Sub

        Public Sub New(originalFilename As String, destinationFilename As String)
            Me.OriginalName = originalFilename
            Dim parts = destinationFilename.Split(" ".ToCharArray, 2)
            If IsNumeric(parts(0)) Then
                TrackNumber = CInt(parts(0))
                If parts.Length > 2 Then
                    TrackName = parts(1)
                Else
                    TrackName = String.Empty
                End If
            Else
                TrackName = destinationFilename
            End If
        End Sub
        ''' <summary>
        ''' The name of the unconverted track.  Usually for internal game use only.
        ''' </summary>
        Public Property OriginalName As String

        ''' <summary>
        ''' Number of the track.
        ''' </summary>
        ''' <returns></returns>
        Public Property TrackNumber As Integer

        ''' <summary>
        ''' The user-friendly name of the track.
        ''' </summary>
        Public Property TrackName As String

        ''' <summary>
        ''' Gets the desired filename of the track after conversion, without an extension.
        ''' </summary>
        ''' <param name="maxTrackNumber">Largest track number in the soundtrack.</param>
        ''' <returns>The filename of the converted track.</returns>
        ''' <remarks>If <see cref="TrackNumber"/> is 1, <see cref="TrackName"/> is "Track", and <paramref name="maxTrackNumber"/> is 99, then this function will return "01 Track".</remarks>
        Public Function GetFilename(maxTrackNumber As Integer) As String
            Return TrackNumber.ToString.PadLeft(maxTrackNumber.ToString.Length, "0"c) & " " & TrackName
        End Function
    End Class
End Namespace
