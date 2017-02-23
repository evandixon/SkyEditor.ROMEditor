Imports System.Text
''' <summary>
''' Helper class containing VB constants, since .Net Standard doesn't include VB libraries
''' </summary>
Public Class VBConstants

    ''' <summary>
    ''' Initializing the class is disabled
    ''' </summary>
    Private Sub New()
    End Sub

    Public Shared ReadOnly Property vbNullChar As String
        Get
            Return Encoding.ASCII.GetChars({0})(0)
        End Get
    End Property

    Public Shared ReadOnly Property vbCr As Char
        Get
            Return Encoding.ASCII.GetChars({13})(0)
        End Get
    End Property

    Public Shared ReadOnly Property vbLf As Char
        Get
            Return Encoding.ASCII.GetChars({10})(0)
        End Get
    End Property

    Public Shared ReadOnly Property vbCrLf As String
        Get
            Return Encoding.ASCII.GetChars({13, 10})
        End Get
    End Property
End Class
