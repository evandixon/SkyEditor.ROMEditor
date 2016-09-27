Namespace Windows.Soundtrack
    Public Class SoundtrackDefinition
        Public Shared Function FromDictionaryString(dictionaryString As String, albumName As String, albumArtist As String, year As UInteger, sourcePath As String, extension As String, system As String, gameID As String) As SoundtrackDefinition
            Dim definition As New SoundtrackDefinition
            definition.System = system
            definition.GameID = gameID
            definition.AlbumName = albumName
            definition.AlbumArtist = albumArtist
            definition.Year = year
            definition.SourcePath = sourcePath
            definition.OriginalExtension = extension

            Dim lines = dictionaryString.Split(vbLf)
            For Each item In lines
                Dim parts = item.Trim.Split("=".ToCharArray, 2)
                If parts.Count = 2 Then
                    definition.Tracks.Add(New SoundtrackTrack(parts(0), parts(1)))
                End If
            Next

            Return definition
        End Function

        Public Sub New()
            Tracks = New List(Of SoundtrackTrack)
        End Sub

        Public Property System As String
        Public Property GameID As String
        Public Property AlbumName As String
        Public Property AlbumArtist As String
        Public Property Year As UInteger
        Public Property Tracks As List(Of SoundtrackTrack)

        ''' <summary>
        ''' The file extension of the original, unconverted file.  Should not start with a dot.
        ''' </summary>
        Public Property OriginalExtension As String

        ''' <summary>
        ''' Path in the ROM of the soundtrack files, relative to the root of the ROM filesystem.
        ''' </summary>
        ''' <returns></returns>
        Public Property SourcePath As String

        Public Overridable ReadOnly Property MaxTrackNumber As Integer
            Get
                Return Tracks.Select(Function(x) x.TrackNumber).Max
            End Get
        End Property

    End Class
End Namespace
