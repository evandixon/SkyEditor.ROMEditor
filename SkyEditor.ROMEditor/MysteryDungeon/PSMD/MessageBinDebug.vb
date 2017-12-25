Imports SkyEditor.Core.IO

Namespace MysteryDungeon.PSMD
    Public Class MessageBinDebug
        Inherits MessageBin

        Protected Overrides ReadOnly Property SkipMessageBinSave As Boolean = True

        Protected Overrides Sub ProcessData()
            Dim stringCount As Integer = BitConverter.ToInt32(ContentHeader, 0)
            Dim stringInfoPointer As Integer = BitConverter.ToInt32(ContentHeader, 4)

            For i = 0 To stringCount - 1
                Dim stringPointer As Integer = BitConverter.ToInt32(Read(stringInfoPointer + i * 8 + &H0, 4), 0)
                Dim stringHash As UInteger = BitConverter.ToUInt32(Read(stringInfoPointer + i * 8 + &H4, 4), 0)

                Dim s As New Text.StringBuilder()
                Dim e = Text.UnicodeEncoding.Unicode

                'Parse the null-terminated UTF-16 string
                Dim j As Integer = 0
                Dim cRaw As Byte()
                Dim doEnd As Boolean = False
                Do
                    cRaw = Read(stringPointer + j * 2, 2)

                    'TODO: parse escape characters, as described in these posts:
                    'http://projectpokemon.org/forums/showthread.php?46904-Pokemon-Super-Mystery-Dungeon-And-PMD-GTI-Research-And-Utilities&p=211018&viewfull=1#post211018
                    'http://projectpokemon.org/forums/showthread.php?46904-Pokemon-Super-Mystery-Dungeon-And-PMD-GTI-Research-And-Utilities&p=210986&viewfull=1#post210986

                    If cRaw(1) >= 128 Then 'Most significant bit is set
                        s.Append("\")
                        s.Append(Conversion.Hex(cRaw(1)).PadLeft(2, "0"c))
                        s.Append(Conversion.Hex(cRaw(0)).PadLeft(2, "0"c))
                        j += 1
                    Else
                        Dim c = e.GetString(cRaw, 0, cRaw.Length)

                        If cRaw.SequenceEqual({0, 0}) Then
                            doEnd = True
                        Else
                            s.Append(c)
                            j += 1
                        End If
                    End If

                Loop Until doEnd

                Dim newEntry = New MessageBinStringEntry With {.Hash = stringHash, .Entry = s.ToString.Trim, .Pointer = stringPointer}
                Strings.Add(newEntry)
            Next
            SetOriginalIndexes(Strings)
        End Sub

        Public Overrides Async Function Save(Destination As String, provider As IIOProvider) As Task
            Me.RelativePointers.Clear()
            'Sir0 header pointers
            Me.RelativePointers.Add(4)
            Me.RelativePointers.Add(4)

            'Generate sections
            Dim stringSection As New List(Of Byte)
            Dim infoSection As New List(Of Byte)
            For Each item In From s In Strings Order By s.Hash Ascending
                infoSection.AddRange(BitConverter.GetBytes(16 + stringSection.Count))
                infoSection.AddRange(BitConverter.GetBytes(item.Hash))
                stringSection.AddRange(item.GetStringBytes)
            Next

            'Add pointers
            Me.RelativePointers.Add(stringSection.Count + 8)
            For count = 0 To Strings.Count - 2
                Me.RelativePointers.Add(&H8)
            Next

            'Write sections to file
            Me.Length = 16 + stringSection.Count + infoSection.Count
            Await Me.WriteAsync(16, stringSection.Count, stringSection.ToArray)
            Await Me.WriteAsync(16 + stringSection.Count, infoSection.Count, infoSection.ToArray)

            'Update header
            Dim headerBytes As New List(Of Byte)
            headerBytes.AddRange(BitConverter.GetBytes(Strings.Count))
            headerBytes.AddRange(BitConverter.GetBytes(16 + stringSection.Count))
            Me.ContentHeader = headerBytes.ToArray
            Me.RelativePointers.Add(&H10)

            'Let the general SIR0 stuff happen
            Await MyBase.Save(Destination, provider)
        End Function
    End Class
End Namespace
