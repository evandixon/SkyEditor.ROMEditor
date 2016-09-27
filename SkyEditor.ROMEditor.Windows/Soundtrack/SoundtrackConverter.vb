Imports MediaToolkit
Imports MediaToolkit.Model
Imports MediaToolkit.Options
Imports SkyEditor.Core.Utilities
Imports SkyEditor.ROMEditor.Windows.Projects
Imports System.IO

Namespace Soundtrack
    Public Class SoundtrackConverter
        Implements IReportProgress

#Region "IReportProgress Support"
        Public Property IsCompleted As Boolean Implements IReportProgress.IsCompleted
            Get
                Return _isCompleted
            End Get
            Private Set(value As Boolean)
                If Not _isCompleted = value Then
                    _isCompleted = value
                    If _isCompleted Then
                        RaiseEvent Completed(Me, New EventArgs)
                    End If
                End If
            End Set
        End Property
        Dim _isCompleted As Boolean

        Public Property IsIndeterminate As Boolean Implements IReportProgress.IsIndeterminate
            Get
                Return _isIndeterminate
            End Get
            Private Set(value As Boolean)
                If Not _isIndeterminate = value Then
                    _isIndeterminate = value
                    UpdateLoadingStatus()
                End If
            End Set
        End Property
        Dim _isIndeterminate As Boolean

        Public Property Message As String Implements IReportProgress.Message
            Get
                Return _message
            End Get
            Private Set(value As String)
                If Not _message = value Then
                    _message = value
                    UpdateLoadingStatus()
                End If
            End Set
        End Property
        Dim _message As String

        Public Property Progress As Single Implements IReportProgress.Progress
            Get
                Return _progress
            End Get
            Private Set(value As Single)
                If Not _progress = value Then
                    _progress = value
                    UpdateLoadingStatus()
                End If
            End Set
        End Property
        Dim _progress As Single

        Private Sub UpdateLoadingStatus()
            RaiseEvent ProgressChanged(Me, New ProgressReportedEventArgs With {.IsIndeterminate = IsIndeterminate, .Message = Message, .Progress = Progress})
        End Sub

        Public Event Completed As IReportProgress.CompletedEventHandler Implements IReportProgress.Completed
        Public Event ProgressChanged As IReportProgress.ProgressChangedEventHandler Implements IReportProgress.ProgressChanged
#End Region

        Private Class FileAbstraction
            Implements TagLib.File.IFileAbstraction
            Implements IDisposable

            Private Filestream As IO.FileStream

            Public ReadOnly Property Name As String Implements TagLib.File.IFileAbstraction.Name
                Get
                    Return Filestream.Name
                End Get
            End Property

            Public ReadOnly Property ReadStream As Stream Implements TagLib.File.IFileAbstraction.ReadStream
                Get
                    Return Filestream
                End Get
            End Property

            Public ReadOnly Property WriteStream As Stream Implements TagLib.File.IFileAbstraction.WriteStream
                Get
                    Return Filestream
                End Get
            End Property

            Public Sub CloseStream(stream As Stream) Implements TagLib.File.IFileAbstraction.CloseStream
                stream.Close()
            End Sub

            Public Sub New(Filename As String)
                Filestream = IO.File.Open(Filename, IO.FileMode.Open, IO.FileAccess.ReadWrite)
            End Sub

#Region "IDisposable Support"
            Private disposedValue As Boolean ' To detect redundant calls

            ' IDisposable
            Protected Overridable Sub Dispose(disposing As Boolean)
                If Not Me.disposedValue Then
                    If disposing Then
                        ' TODO: dispose managed state (managed objects).
                        Filestream.Dispose()
                    End If

                    ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                    ' TODO: set large fields to null.
                End If
                Me.disposedValue = True
            End Sub

            ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
            'Protected Overrides Sub Finalize()
            '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            '    Dispose(False)
            '    MyBase.Finalize()
            'End Sub

            ' This code added by Visual Basic to correctly implement the disposable pattern.
            Public Sub Dispose() Implements IDisposable.Dispose
                ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                Dispose(True)
                ' TODO: uncomment the following line if Finalize() is overridden above.
                ' GC.SuppressFinalize(Me)
            End Sub
#End Region
        End Class

        Public Async Sub StartConvert(project As BaseRomProject, soundtrackDefinition As SoundtrackDefinition, outputDirectory As String)
            Dim sourceDir As String = IO.Path.Combine(project.GetRawFilesDir, soundtrackDefinition.SourcePath)

            If Not IO.Directory.Exists(outputDirectory) Then
                IO.Directory.CreateDirectory(outputDirectory)
            End If

            For Each item In IO.Directory.GetFiles(outputDirectory)
                IO.File.Delete(item)
            Next

            Me.Message = String.Format(My.Resources.Language.LoadingConvertingSoundtrackXofY, 0, soundtrackDefinition.Tracks.Count)
            Me.Progress = 0
            Me.IsCompleted = False
            Me.IsIndeterminate = False

            Using external As New ExternalProgramManager
                Dim vgmPath = external.GetVgmStreamPath()

                Dim f As New AsyncFor
                f.BatchSize = Environment.ProcessorCount * 2
                AddHandler f.LoadingStatusChanged, Sub(sender As Object, e As LoadingStatusChangedEventArgs)
                                                       Me.Message = String.Format(My.Resources.Language.LoadingConvertingSoundtrackXofY, e.Completed, e.Total)
                                                       Me.Progress = e.Progress
                                                       Me.IsCompleted = e.Complete
                                                   End Sub
                Await f.RunForEach(Async Function(Item As SoundtrackTrack) As Task
                                       Dim source = IO.Path.Combine(sourceDir, Item.OriginalName) & "." & soundtrackDefinition.OriginalExtension
                                       Dim destinationWav As String = source.
                                                                        Replace(sourceDir, outputDirectory).
                                                                        Replace(soundtrackDefinition.OriginalExtension, "wav").
                                                                        Replace(Item.OriginalName, Item.GetFilename(soundtrackDefinition.MaxTrackNumber))

                                       'Remove bad characters
                                       For Each c In "!?,".ToCharArray
                                           destinationWav = destinationWav.Replace(c, "")
                                       Next
                                       destinationWav = destinationWav.Replace("é", "e")

                                       Dim destinationMp3 = destinationWav.Replace(".wav", ".mp3")

                                       'Create the wav file
                                       Await vgmstream.RunVGMStream(vgmPath, source, destinationWav)

                                       'Check to see if the conversion completed successfully
                                       If IO.File.Exists(destinationWav) Then
                                           'Convert to mp3
                                           Using e As New Engine(True)
                                               Dim wav As New MediaFile(destinationWav)
                                               Dim mp3 As New MediaFile(destinationMp3)
                                               Dim options = New ConversionOptions
                                               options.AudioSampleRate = AudioSampleRate.Hz48000
                                               e.Convert(wav, mp3, options)
                                           End Using

                                           IO.File.Delete(destinationWav)

                                           'Add the tag
                                           Using abs As New FileAbstraction(destinationMp3)
                                               Dim t As New TagLib.Mpeg.AudioFile(abs)
                                               With t.Tag
                                                   .Album = soundtrackDefinition.AlbumName
                                                   .AlbumArtists = {soundtrackDefinition.AlbumArtist}
                                                   .Title = Item.TrackName
                                                   .Track = Item.TrackNumber
                                                   .Year = soundtrackDefinition.Year
#Disable Warning
                                                   'Disabling warning because this tag needs to be set to ensure compatibility, like with Windows Explorer and Windows Media Player.
                                                   .Artists = {soundtrackDefinition.AlbumArtist}
#Enable Warning
                                               End With
                                               t.Save()
                                           End Using
                                       Else
                                           'Todo: log error somehow
                                       End If
                                   End Function, soundtrackDefinition.Tracks)
            End Using
            IsCompleted = True
        End Sub

    End Class

End Namespace
