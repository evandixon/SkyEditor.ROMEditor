Namespace ProcessManagement
    Public Class JavaNotFoundException
        Inherits Exception

        Public Sub New(innerException As Exception)
            MyBase.New(My.Resources.Language.ProcessManagement_JavaNotFoundMessage, innerException)
        End Sub
    End Class
End Namespace