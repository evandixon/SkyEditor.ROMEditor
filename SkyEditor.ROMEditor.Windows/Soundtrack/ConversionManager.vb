Imports System.Text.RegularExpressions
Imports SkyEditor.ROMEditor.Windows.Projects

Namespace Soundtrack
    Public Class ConversionManager
        Public Shared Function SupportsProject(project As BaseRomProject) As Boolean
            Dim psmd As New Regex(GameStrings.PSMDCode)
            Dim gti As New Regex(GameStrings.GTICode)

            Return project.RomSystem = "3DS" AndAlso (psmd.IsMatch(project.GameCode) OrElse gti.IsMatch(project.GameCode))
        End Function

        Public Shared Function StartConversion(project As BaseRomProject) As SoundtrackConverter
            Dim destDir As String = IO.Path.Combine(project.GetRootDirectory, "Soundtrack")
            Dim psmd As New Regex(GameStrings.PSMDCode)
            Dim gti As New Regex(GameStrings.GTICode)
            If psmd.IsMatch(project.GameCode) Then
                Dim definition = SoundtrackDefinition.FromDictionaryString(My.Resources.SoundtrackLists.PSMD, My.Resources.Language.PSMDSoundTrackAlbum, My.Resources.Language.PSMDSoundTrackArtist, 2015, IO.Path.Combine("romfs", "sound", "stream"), "dspadpcm.bcstm", "3DS", GameStrings.PSMDCode)
                Dim converter As New SoundtrackConverter
                converter.StartConvert(project, definition, destDir)
                Return converter
            ElseIf gti.IsMatch(project.GameCode) Then
                Dim definition = SoundtrackDefinition.FromDictionaryString(My.Resources.SoundtrackLists.PSMD, My.Resources.Language.GTISoundTrackAlbum, My.Resources.Language.GTISoundTrackArtist, 2013, IO.Path.Combine("romfs", "sound", "stream"), "bcstm", "3DS", GameStrings.GTICode)
                Dim converter As New SoundtrackConverter
                converter.StartConvert(project, definition, destDir)
                Return converter
            Else
                Return Nothing
            End If
        End Function
    End Class
End Namespace
