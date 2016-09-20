Imports System.IO
Imports System.Text.RegularExpressions
Imports MediaToolkit
Imports MediaToolkit.Model
Imports MediaToolkit.Options
Imports SkyEditor.Core.Utilities
Imports SkyEditor.Core.Windows
Imports SkyEditor.ROMEditor.Windows.Projects

Namespace MysteryDungeon.PSMD
    Public Class PSMDSoundtrackConverter
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

        Public Shared Function SupportsProject(project As BaseRomProject) As Boolean
            Dim psmd As New Regex(GameStrings.PSMDCode)

            Return project.RomSystem = "3DS" AndAlso psmd.IsMatch(project.GameCode)
        End Function

        Public Shared Async Function Convert(project As BaseRomProject) As Task
            If SupportsProject(project) Then
                Dim sourceDir As String = IO.Path.Combine(project.GetRawFilesDir, "romfs", "sound", "stream")
                Dim destDir As String = IO.Path.Combine(project.GetRootDirectory, "Soundtrack")

                'Todo: do error checks on input file
                Dim trackNames As New Dictionary(Of String, String)
                If IO.File.Exists(EnvironmentPaths.GetResourceName("PSMD English Soundtrack.txt")) Then
                    Dim lines = IO.File.ReadAllLines(EnvironmentPaths.GetResourceName("PSMD English Soundtrack.txt"))
                    For Each item In lines
                        Dim parts = item.Split("=".ToCharArray, 2)
                        If parts.Count = 2 Then
                            trackNames.Add(parts(0), parts(1))
                        End If
                    Next
                End If

                If Not IO.Directory.Exists(destDir) Then
                    IO.Directory.CreateDirectory(destDir)
                End If

                For Each item In IO.Directory.GetFiles(destDir)
                    IO.File.Delete(item)
                Next

                'PluginHelper.SetLoadingStatus(My.Resources.Language.ConvertingStreams)
                Using external As New ExternalProgramManager
                    Dim vgmPath = external.GetVgmStreamPath()

                    Dim f As New AsyncFor '(My.Resources.Language.ConvertingStreams)
                    f.BatchSize = Environment.ProcessorCount * 2
                    Await f.RunForEach(Async Function(Item As String) As Task
                                           Dim source = IO.Path.Combine(sourceDir, Item) & ".dspadpcm.bcstm"

                                           'Create the wav
                                           Dim destinationWav = source.Replace(sourceDir, destDir).Replace("dspadpcm.bcstm", "wav")

                                           Dim filename = IO.Path.GetFileNameWithoutExtension(destinationWav)

                                           If trackNames.ContainsKey(filename) Then
                                               destinationWav = destinationWav.Replace(filename, trackNames(filename).Replace(":", "").Replace("é", "e"))
                                           End If

                                           For Each c In "!?,".ToCharArray
                                               destinationWav = destinationWav.Replace(c, "")
                                           Next

                                           Dim destinationMp3 = destinationWav.Replace(".wav", ".mp3")

                                           Await vgmstream.RunVGMStream(vgmPath, source, destinationWav)

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
                                                   .Album = My.Resources.Language.PSMDSoundTrackAlbum
                                                   .AlbumArtists = {My.Resources.Language.PSMDSoundTrackArtist}
#Disable Warning
                                                   'Disabling warning because this tag needs to be set to ensure compatibility, like with Windows Explorer and Windows Media Player.
                                                   .Artists = {My.Resources.Language.PSMDSoundTrackArtist}
#Enable Warning
                                                   .Year = 2015
                                                   Dim filenameParts = trackNames(filename).Split(" ".ToCharArray, 2)
                                                   If filenameParts.Count = 2 Then
                                                       If IsNumeric(filenameParts(0)) Then
                                                           .Track = CInt(filenameParts(0))
                                                       End If

                                                       .Title = filenameParts(1)
                                                   End If
                                               End With
                                               t.Save()
                                           End Using
                                       End Function, trackNames.Keys)
                End Using
            End If
        End Function
    End Class
End Namespace

