Public Module Conversion
    Public Function Hex(value As Integer) As String
        Return String.Join("", BitConverter.ToString(BitConverter.GetBytes(value)).Split("-").Reverse)
    End Function
End Module
