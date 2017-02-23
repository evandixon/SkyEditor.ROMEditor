Imports SkyEditor.Core.IO

Namespace MysteryDungeon.PSMD
    Public Class FlowData
        Inherits Sir0

        Public Property Strings As List(Of String)

        Public Property Data1 As List(Of ULong)
        Public Property Data2 As List(Of ULong)

        Public Overrides Async Function OpenFile(Filename As String, Provider As IIOProvider) As Task
            Await MyBase.OpenFile(Filename, Provider)

            Dim numEntries1 As UInteger = Await Me.ReadUInt32Async(&H18)
            Dim entryPtr1 As UInteger = Await Me.ReadUInt32Async(&H1C)
            Dim numEntries2 As UInteger = Await Me.ReadUInt32Async(&H20)
            Dim entryPtr2 As UInteger = Await Me.ReadUInt32Async(&H24)

            For count As UInteger = 0 To numEntries1 - 1
                Dim data As New List(Of Byte)
                Dim len = Await Me.ReadUInt32Async(entryPtr1 + count * 8)
                Dim ptr = Await Me.ReadUInt32Async(entryPtr1 + count * 8 + 4)
                For i As UInteger = 0 To len - 1
                    data.Add(Await Me.ReadAsync(ptr + i))
                Next
                While data.Count < 8
                    data.Add(0)
                End While
                Data1.Add(BitConverter.ToUInt64(data.ToArray, 0))
            Next

            For count As UInteger = 0 To numEntries2 - 1
                Dim data As New List(Of Byte)
                Dim len = Await Me.ReadUInt32Async(entryPtr2 + count * 8)
                Dim ptr = Await Me.ReadUInt32Async(entryPtr2 + count * 8 + 4)
                For i As UInteger = 0 To len - 1
                    data.Add(Await Me.ReadAsync(ptr + i))
                Next
                While data.Count < 8
                    data.Add(0)
                End While
                Data2.Add(BitConverter.ToUInt64(data.ToArray, 0))
            Next

            'Debug
            Dim d1 As New List(Of String)
            Dim d2 As New List(Of String)
            For Each item In Data1
                d1.Add(Conversion.Hex(item))
            Next
            For Each item In Data2
                d2.Add(Conversion.Hex(item))
            Next
            Dim s1 As New Text.StringBuilder
            For Each item In d1
                s1.AppendLine(item)
            Next
            Dim s2 As New Text.StringBuilder
            For Each item In d2
                s2.AppendLine(item)
            Next
        End Function

        Public Sub New()
            MyBase.New
            Data1 = New List(Of ULong)
            Data2 = New List(Of ULong)
            Me.ResizeFileOnLoad = False
        End Sub
    End Class

End Namespace
