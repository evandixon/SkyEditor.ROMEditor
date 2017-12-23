Imports Force
Imports Force.Crc32

Namespace MysteryDungeon.PSMD
    Public NotInheritable Class PmdFunctions
        Private Shared ReadOnly Property Crc32 As Crc32Algorithm = New Crc32Algorithm
        Public Shared Function Crc32Hash(filename As String) As UInteger
            Return BitConverter.ToUInt32(Crc32.ComputeHash(Text.Encoding.Unicode.GetBytes(filename)).Reverse().ToArray(), 0)
        End Function
    End Class
End Namespace
