Imports SkyEditor.Core.IO

Namespace MysteryDungeon.PSMD
    Public Class BGRS
        Implements IOpenableFile

        Public Property Magic As String

        Public Property ReferencedBchFileName As String

        Public Property BgrsName As String

        Public Property PartNames As New List(Of String)

        Public Property SklaNames As New List(Of String)

        Public Async Function OpenFile(filename As String, provider As IIOProvider) As Task Implements IOpenableFile.OpenFile
            Using f As New GenericFile(filename, provider)
                Await OpenInternal(f)
            End Using
        End Function

        Public Async Function OpenFile(rawData As Byte()) As Task
            Using f As New GenericFile
                f.CreateFile(rawData)
                Await OpenInternal(f)
            End Using
        End Function

        Private Async Function OpenInternal(f As GenericFile) As Task
            Magic = Await f.ReadNullTerminatedStringAsync(0, Text.Encoding.ASCII)
            ReferencedBchFileName = Await f.ReadNullTerminatedStringAsync(Magic.Length + 1, Text.Encoding.ASCII)
            BgrsName = Await f.ReadNullTerminatedStringAsync(&H58, Text.Encoding.ASCII)

            If String.IsNullOrEmpty(ReferencedBchFileName) Then
                'Slightly different structure where counts aren't present. Need to guess.
                'Actually, BgrsName isn't present either, but is still being set above so a hack in Farc will work

                Dim sklaIndex = &H58
                While (Await f.ReadAsync(sklaIndex, 1))(0) <> 0
                    SklaNames.Add(Await f.ReadNullTerminatedStringAsync(sklaIndex, Text.Encoding.ASCII))
                    sklaIndex += &HC4
                End While
            Else
                Dim numSkla = Await f.ReadInt32Async(&H118)
                Dim numParts = Await f.ReadInt32Async(&H11C)
                For partIndex = &H140 To (&H80 * numParts) - 1 Step &H80
                    Dim partName = Await f.ReadNullTerminatedStringAsync(partIndex + &H18, Text.Encoding.ASCII)
                    PartNames.Add(partName)
                Next

                For sklaIndex = &H140 + (&H80 * numParts) To &H140 + (&H80 * numParts) + (&HC4 * numSkla) - 1 Step &HC4
                    Dim sklaName = Await f.ReadNullTerminatedStringAsync(sklaIndex + &H18, Text.Encoding.ASCII)
                    SklaNames.Add(sklaName)
                Next
            End If
        End Function
    End Class
End Namespace
