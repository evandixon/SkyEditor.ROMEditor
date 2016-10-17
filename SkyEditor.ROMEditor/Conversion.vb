Public Module Conversion
    Public Function Hex(value As Integer) As String
        Return value.ToString("X")
    End Function
    Public Function Hex(value As UInteger) As String
        Return value.ToString("X")
    End Function
    Public Function Hex(value As ULong) As String
        Return value.ToString("X")
    End Function
End Module
