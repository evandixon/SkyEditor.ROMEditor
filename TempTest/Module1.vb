Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization

Module Module1

    Sub Main()
        Dim path = "C:\Tests\EoS-EU-ChallengeModeDev\LevelUp\Pokemon\pokemon_data\0489_Riolu_edit.xml"
        Dim path2 = "C:\Tests\EoS-EU-ChallengeModeDev\LevelUp\Pokemon\pokemon_data\0489_Riolu_edit_saved.xml"
        Dim deserializer As New XmlSerializer(GetType(SkyEditor.ROMEditor.MysteryDungeon.Explorers.PPMDU.Pokemon))
        Dim reader As New XmlTextReader(File.OpenRead(path))
        Dim pokemon = deserializer.Deserialize(reader)


        deserializer.Serialize(File.OpenWrite(path2), pokemon)
    End Sub

End Module
