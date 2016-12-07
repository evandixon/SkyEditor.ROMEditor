Imports Message_FARC_Patcher

Namespace MysteryDungeon.PSMD.Projects
    Public Interface IPsmdMessageBinProject
        Function GetLanguageFile(name As String) As Task(Of MessageBin)
        Function GetPokemonNames() As Task(Of Dictionary(Of Integer, String))
        Function GetMoveNames() As Task(Of Dictionary(Of Integer, String))
    End Interface
End Namespace

