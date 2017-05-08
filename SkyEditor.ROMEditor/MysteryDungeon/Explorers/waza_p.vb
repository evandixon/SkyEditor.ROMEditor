Imports SkyEditor.Core.IO

Namespace MysteryDungeon.Explorers
    Public Class waza_p
        Inherits ExplorersSir0

        Public Enum MoveCategory
            Physical = 0
            Special = 1
            Status = 2
        End Enum

        Public Class MoveData
            Public Sub New(data As Byte())
                BasePower = BitConverter.ToUInt16(data, 0)
                Type = data(2)
                Category = data(3)
                Bitfield1 = BitConverter.ToUInt16(data, 4)
                Bitfield2 = BitConverter.ToUInt16(data, 6)
                BasePP = data(8)

                Unk6 = data(9)
                Unk7 = data(&HA)
                MoveAccuracy = data(&HB)
                Unk9 = data(&HC)
                Unk10 = data(&HD)
                Unk11 = data(&HE)
                Unk12 = data(&HF)
                Unk13 = data(&H10)
                Unk14 = data(&H11)
                Unk15 = data(&H12)
                Unk16 = data(&H13)
                Unk17 = data(&H14)
                Unk18 = data(&H15)
                ResourceID = BitConverter.ToUInt16(data, &H16)
                Unk19 = data(&H18)
            End Sub
            Public Property BasePower As UInt16
            Public Property Type As Byte
            Public Property Category As MoveCategory
            Public Property Bitfield1 As UInt16
            Public Property Bitfield2 As UInt16
            Public Property BasePP As Byte
            Public Property Unk6 As Byte
            Public Property Unk7 As Byte
            Public Property MoveAccuracy As Byte
            Public Property Unk9 As Byte
            Public Property Unk10 As Byte
            Public Property Unk11 As Byte
            Public Property Unk12 As Byte
            Public Property Unk13 As Byte
            Public Property Unk14 As Byte
            Public Property Unk15 As Byte
            Public Property Unk16 As Byte
            Public Property Unk17 As Byte
            Public Property Unk18 As Byte
            Public Property ResourceID As UInt16
            Public Property Unk19 As Byte
        End Class

        Public Class PokemonMoves
            Public Property LevelUpMoves As Dictionary(Of Byte, UInt16)
            Public Property TMMoves As List(Of UInt16)
            Public Property EggMoves As List(Of UInt16)
            Public Sub New()
                LevelUpMoves = New Dictionary(Of Byte, UInt16)
                TMMoves = New List(Of UInt16)
                EggMoves = New List(Of UInt16)
            End Sub
            Public Function Clone() As PokemonMoves
                Dim m As New PokemonMoves
                For Each item In Me.LevelUpMoves
                    m.LevelUpMoves.Add(item.Key, item.Value)
                Next
                For Each item In Me.TMMoves
                    m.TMMoves.Add(item)
                Next
                For Each item In Me.EggMoves
                    m.EggMoves.Add(item)
                Next
                Return m
            End Function
        End Class

        Public Overrides Async Function OpenFile(Filename As String, Provider As IIOProvider) As Task
            Await MyBase.OpenFile(Filename, Provider)

            PokemonLearnsets = New List(Of PokemonMoves)

            Dim moveDataBlockPtr = BitConverter.ToInt32(ContentHeader, 0)
            Dim learnPtrTablePtr = BitConverter.ToInt32(ContentHeader, 4)

            'Parse Move data
            Dim moveDataBlockEnd = Await ReadInt32Async(learnPtrTablePtr)
            While moveDataBlockPtr + 26 < moveDataBlockEnd
                Moves.Add(New MoveData(Await ReadAsync(moveDataBlockPtr, 26)))
                moveDataBlockPtr += 26
            End While

            'Parse Learn data
            Dim currentLearnPtr As Integer
            Do
                'Read a pointer (Levelup data)
                currentLearnPtr = Await ReadInt32Async(learnPtrTablePtr)
                learnPtrTablePtr += 4

                If currentLearnPtr = &HAAAAAAAA Then
                    Exit Do
                End If

                Dim moves = New PokemonMoves

                'Level-up data
                If currentLearnPtr > 0 Then
                    Dim moveID As UInt16 = 0

                    Dim currentByte = Await ReadAsync(currentLearnPtr)
                    While currentByte > 0
                        'Read move ID
                        If currentByte > 128 Then
                            'Multi-byte move ID
                            Dim part1 = (currentByte - 128) << 7
                            Dim part2 = Await ReadAsync(currentLearnPtr + 1)
                            moveID = part1 Or part2
                            currentLearnPtr += 2
                        Else
                            'Single-byte move ID
                            moveID = currentByte
                            currentLearnPtr += 1
                        End If

                        'Read learn level
                        currentByte = Await ReadAsync(currentLearnPtr)

                        'Set current learnset
                        If Not moves.LevelUpMoves.ContainsKey(currentByte) Then
                            moves.LevelUpMoves.Add(currentByte, moveID)
                        End If

                        currentLearnPtr += 1
                    End While
                End If

                currentLearnPtr = Await ReadInt32Async(learnPtrTablePtr)
                learnPtrTablePtr += 4

                'TM Data
                If currentLearnPtr > 0 Then
                    Dim moveID As UInt16 = 0

                    Dim currentByte = Await ReadAsync(currentLearnPtr)
                    While currentByte > 0
                        'Read move ID
                        If currentByte > 128 Then
                            'Multi-byte move ID
                            Dim part1 = (currentByte - 128) << 7
                            Dim part2 = Await ReadAsync(currentLearnPtr + 1)
                            moveID = part1 Or part2
                            currentLearnPtr += 2
                        Else
                            'Single-byte move ID
                            moveID = currentByte
                            currentLearnPtr += 1
                        End If

                        'Set current learnset
                        If Not moves.TMMoves.Contains(moveID) Then
                            moves.TMMoves.Add(moveID)
                        End If

                        currentLearnPtr += 1
                    End While
                End If

                currentLearnPtr = Await ReadInt32Async(learnPtrTablePtr)
                learnPtrTablePtr += 4

                'Egg Data
                If currentLearnPtr > 0 Then
                    Dim moveID As UInt16 = 0

                    Dim currentByte = Await ReadAsync(currentLearnPtr)
                    While currentByte > 0
                        'Read move ID
                        If currentByte > 128 Then
                            'Multi-byte move ID
                            Dim part1 = (currentByte - 128) << 7
                            Dim part2 = Await ReadAsync(currentLearnPtr + 1)
                            moveID = part1 Or part2
                            currentLearnPtr += 2
                        Else
                            'Single-byte move ID
                            moveID = currentByte
                            currentLearnPtr += 1
                        End If

                        'Set current learnset
                        If Not moves.EggMoves.Contains(moveID) Then
                            moves.EggMoves.Add(moveID)
                        End If

                        currentLearnPtr += 1
                    End While
                End If

                PokemonLearnsets.Add(moves)
            Loop
        End Function

        Public Property Moves As List(Of MoveData)
        Public Property PokemonLearnsets As List(Of PokemonMoves)
    End Class
End Namespace

