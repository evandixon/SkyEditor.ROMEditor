Imports SkyEditor.Core.IO
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD

<TestClass>
Public Class PsmdTests
    <TestMethod>
    Public Sub ExtractImage2d()
        Dim provider = New SkyEditor.Core.IO.PhysicalIOProvider
        Dim farcFilename = "C:\Users\evanl\Desktop\SkyEditor\Test 2017-11-22\BaseRom\Raw Files\RomFS\image_2d.bin"
        Dim dbFilename = "C:\Users\evanl\Desktop\SkyEditor\Test 2017-11-22\BaseRom\Raw Files\RomFS\image_2d_database.bin"

        Dim dbFile = New DatabaseBin()
        dbFile.OpenFile(dbFilename, provider).Wait()

        Dim farcFile = New FarcF5()
        farcFile.OpenFile(farcFilename, provider).Wait()

        farcFile.Extract("image2dExtract", provider, dbFile.GenerateHashDictionary).Wait()
    End Sub
End Class
