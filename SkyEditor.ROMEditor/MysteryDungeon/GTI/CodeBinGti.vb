Imports SkyEditor.Core.IO

Namespace MysteryDungeon.GTI
    Public Class CodeBinGti
        Inherits GenericFile

        Public Overrides Function OpenFile(file As GenericFile) As Task
            Return MyBase.OpenFile(file)

        End Function

        Public Overrides Function OpenFile(filename As String, provider As IIOProvider) As Task
            Return MyBase.OpenFile(filename, provider)

        End Function

        Protected Sub ProcessData()

        End Sub

        Public Overrides Function Save(filename As String, provider As IIOProvider) As Task

            Return MyBase.Save(filename, provider)
        End Function
    End Class
End Namespace
