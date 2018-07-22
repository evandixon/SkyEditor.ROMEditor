Imports SkyEditor.Core.IO

Namespace MysteryDungeon.GTI
    Public Class CodeBinGtiUS
        Inherits GenericFile

        Public Async Function SetStarter1(pokemonId As Int16) As Task
            Await Me.WriteInt16Async(&H6F9E14, pokemonId)
            Await Me.WriteInt16Async(&H702F48, pokemonId)
            Await Me.WriteInt16Async(&H702F5C, pokemonId)
        End Function

        Public Async Function SetStarter39(pokemonId As Integer) As Task
            Await Me.WriteInt16Async(&H6F9E18, pokemonId)
            Await Me.WriteInt16Async(&H702F4C, pokemonId)
            Await Me.WriteInt16Async(&H702F60, pokemonId)
        End Function

        Public Async Function SetStarter42(pokemonId As Integer) As Task
            Await Me.WriteInt16Async(&H6F9E1C, pokemonId)
            Await Me.WriteInt16Async(&H702F50, pokemonId)
            Await Me.WriteInt16Async(&H702F64, pokemonId)
        End Function

        Public Async Function SetStarter45(pokemonId As Integer) As Task
            Await Me.WriteInt16Async(&H6F9E20, pokemonId)
            Await Me.WriteInt16Async(&H702F54, pokemonId)
            Await Me.WriteInt16Async(&H702F68, pokemonId)
        End Function

        Public Async Function SetStarter122(pokemonId As Integer) As Task
            Await Me.WriteInt16Async(&H6F9E24, pokemonId)
            Await Me.WriteInt16Async(&H702F58, pokemonId)
            Await Me.WriteInt16Async(&H702F6C, pokemonId)
        End Function
    End Class
End Namespace
