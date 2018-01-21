Imports SkyEditor.Core.IO

Namespace MysteryDungeon.PSMD
    Public Class BGRS
        Implements IOpenableFile

        Public Property Magic As String

        Public Property ReferencedBchFileName As String

        Public Property PartNames As New List(Of String)

        Public Property SklaNames As New List(Of String)

        Public Async Function OpenFile(filename As String, provider As IIOProvider) As Task Implements IOpenableFile.OpenFile
            Using f As New GenericFile(filename, provider)
                Magic = Await f.ReadNullTerminatedStringAsync(0, Text.Encoding.ASCII)
                ReferencedBchFileName = Await f.ReadNullTerminatedStringAsync(Magic.Length + 1, Text.Encoding.ASCII)

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

            End Using
        End Function
    End Class
End Namespace
