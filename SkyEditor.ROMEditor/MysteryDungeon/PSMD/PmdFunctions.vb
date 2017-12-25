Imports Force
Imports Force.Crc32

Namespace MysteryDungeon.PSMD
    Public NotInheritable Class PmdFunctions
        Private Shared ReadOnly Property Crc32 As New Crc32Algorithm
        Private Shared ReadOnly Property Crc32Lock As New Object
        Public Shared Function Crc32Hash(filename As String) As UInteger
            SyncLock Crc32
                Return BitConverter.ToUInt32(Crc32.ComputeHash(Text.Encoding.Unicode.GetBytes(filename)).Reverse().ToArray(), 0)
            End SyncLock
        End Function
    End Class
End Namespace
