﻿Imports SkyEditor.Core.IO
Imports SkyEditor.IO.FileSystem

Namespace MysteryDungeon.PSMD
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Credit to Andibad for research.  https://projectpokemon.org/forums/showthread.php?46904-Pokemon-Super-Mystery-Dungeon-And-PMD-GTI-Research-And-Utilities&amp;p=211199&amp;viewfull=1#post211199
    ''' </remarks>

    Public Class ItemDataInfo
        Implements IOpenableFile
        Public Class ItemDataInfoEntry
            Public Property BuyPrice As UInt16
            Public Property SellPrice As UInt16
            Public Sub New(RawData As Byte())
                BuyPrice = BitConverter.ToUInt16(RawData, 2)
                SellPrice = BitConverter.ToUInt16(RawData, 4)
            End Sub
        End Class
        Public Property Entries As List(Of ItemDataInfoEntry)

        Public Async Function OpenFile(Filename As String, Provider As IFileSystem) As Task Implements IOpenableFile.OpenFile
            Const entryLength = &H24
            Using f As New GenericFile
                f.EnableInMemoryLoad = True
                Await f.OpenFile(Filename, Provider)

                For count = 0 To ((f.Length / entryLength) - 1)
                    Entries.Add(New ItemDataInfoEntry(Await f.ReadAsync(count * entryLength, entryLength)))
                Next
            End Using
        End Function

        Public Sub New()
            Entries = New List(Of ItemDataInfoEntry)
        End Sub
    End Class
End Namespace