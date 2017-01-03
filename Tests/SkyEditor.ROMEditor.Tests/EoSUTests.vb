Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.TestComponents
Imports SkyEditor.Core.Utilities
Imports SkyEditor.Core.Windows.Providers
Imports SkyEditor.ROMEditor
Imports SkyEditor.ROMEditor.MysteryDungeon.Explorers

<TestClass()> Public Class EoSUTests

    Private Const EosTestCategory As String = "EOS (U) Files"

    'Files for all tests
    Dim romFilename As String = "eos-u.nds"
    Dim romDir As String = "extracted-EOS-U"

    Dim provider As IOProvider


    <TestInitialize()> Public Sub TestInit()
        'Set up
        provider = New WindowsIOProvider
        Try
            Using md5 As New MD5CryptoServiceProvider
                Dim hash = md5.ComputeHash(My.Resources.eos_u)
                If Not hash.SequenceEqual(My.Resources.EoS_U_MD5) Then
                    Assert.Inconclusive("Incorrect test ROM specified.  Should be a trimmed North America PMD: Explorers of Sky ROM.")
                End If
            End Using

            provider.WriteAllBytes(romFilename, My.Resources.eos_u)
            Using nds As New GenericNDSRom
                nds.OpenFile(romFilename, provider).Wait()
                nds.Unpack(romDir, provider).Wait()
            End Using
        Catch ex As Exception
            Assert.Inconclusive("Failed to set up.  Exception message: " & ex.Message)
        End Try
    End Sub

    <TestCleanup> Public Sub Cleanup()
        If provider.FileExists(romFilename) Then
            provider.DeleteFile(romFilename)
        End If
        If provider.DirectoryExists(romDir) Then
            provider.DeleteDirectory(romDir)
        End If
    End Sub

    Protected Function GetAndTestFile(Of T As IOpenableFile)(filePath As String, enableResaveTest As Boolean) As T
        Dim tempFilename As String = provider.GetTempFilename
        Dim originalData = provider.ReadAllBytes(filePath)

        'Part 1: Basic Open and Save test

        'Open the file
        Dim testFile = ReflectionHelpers.CreateInstance(GetType(T))
        testFile.OpenFile(filePath, provider).Wait()

        If enableResaveTest AndAlso TypeOf testFile Is ISavableAs Then
            'Save the file
            testFile.Save(tempFilename, provider).Wait()

            'Ensure the data is the same
            Dim newData = provider.ReadAllBytes(tempFilename)
            Assert.AreEqual(originalData.Length, newData.Length, "Length of file altered with no logical changes")
            For count = 0 To originalData.Length - 1
                Assert.AreEqual(originalData(count), newData(count), "Data altered starting at index " & count)
            Next
        End If

        'Cleanup
        If TypeOf testFile Is IDisposable Then
            testFile.Dispose()
        End If
        provider.DeleteFile(tempFilename)

        'Part 2: More advanced tests (handled by calling function)
        testFile = ReflectionHelpers.CreateInstance(GetType(T))
        testFile.OpenFile(filePath, provider).Wait()
        Return testFile
    End Function

    <TestMethod> <TestCategory(EosTestCategory)> Public Sub Overlay13()
        Dim testFile = GetAndTestFile(Of Overlay13)(Path.Combine(romDir, "overlay", "overlay_0013.bin"), True)
        'Starters
        'Bulbasaur
        Assert.AreEqual(CUShort(1), testFile.LonelyMale, "Incorrect starter for Lonely Male")
        Assert.AreEqual(CUShort(1 + 600), testFile.DocileFemale, "Incorrect starter for Docile Female")
        'Charamander
        Assert.AreEqual(CUShort(4), testFile.DocileMale, "Incorrect starter for Docile Male")
        Assert.AreEqual(CUShort(4 + 600), testFile.BraveFemale, "Incorrect starter for Brave Female")
        'Squirtle
        Assert.AreEqual(CUShort(7), testFile.QuirkyMale, "Incorrect starter for Quirky Male")
        Assert.AreEqual(CUShort(7 + 600), testFile.BoldFemale, "Incorrect starter for Bold Female")
        'Pikachu
        Assert.AreEqual(CUShort(25), testFile.BraveMale, "Incorrect starter for Brave Male")
        Assert.AreEqual(CUShort(25 + 600), testFile.HastyFemale, "Incorrect starter for Hasty Female")
        'Vulpix
        Assert.AreEqual(CUShort(37 + 600), testFile.RelaxedFemale, "Incorrect starter for Relaxed Female")
        'Eevee
        Assert.AreEqual(CUShort(133 + 600), testFile.JollyFemale, "Incorrect starter for Jolly Female")
        'Chikorita                 
        Assert.AreEqual(CUShort(152), testFile.CalmMale, "Incorrect starter for Calm Male")
        Assert.AreEqual(CUShort(152 + 600), testFile.QuietFemale, "Incorrect starter for Quiet Female")
        'Cyndaquil                 
        Assert.AreEqual(CUShort(155), testFile.TimidMale, "Incorrect starter for Timid Male")
        Assert.AreEqual(CUShort(155 + 600), testFile.CalmFemale, "Incorrect starter for Calm Female")
        'Totodile                  
        Assert.AreEqual(CUShort(158), testFile.JollyMale, "Incorrect starter for Jolly Male")
        Assert.AreEqual(CUShort(158 + 600), testFile.SassyFemale, "Incorrect starter for Sassy Female")
        'Phanpy                    
        Assert.AreEqual(CUShort(258), testFile.RelaxedMale, "Incorrect starter for Relaxed Male")
        'Treecko                   
        Assert.AreEqual(CUShort(280), testFile.QuietMale, "Incorrect starter for Quiet Male")
        Assert.AreEqual(CUShort(280 + 600), testFile.HardyFemale, "Incorrect starter for Hardy Female")
        'Torchic                   
        Assert.AreEqual(CUShort(283), testFile.HardyMale, "Incorrect starter for Hardy Male")
        Assert.AreEqual(CUShort(283 + 600), testFile.RashFemale, "Incorrect starter for Rash Female")
        'Mudkip                    
        Assert.AreEqual(CUShort(286), testFile.RashMale, "Incorrect starter for Rash Male")
        Assert.AreEqual(CUShort(286 + 600), testFile.LonelyFemale, "Incorrect starter for Lonely Female")
        'Skitty                    
        Assert.AreEqual(CUShort(328 + 600), testFile.NaiveFemale, "Incorrect starter for Naïve Female")
        'Turtwig                   
        Assert.AreEqual(CUShort(422), testFile.BoldMale, "Incorrect starter for Bold Male")
        Assert.AreEqual(CUShort(422 + 600), testFile.TimidFemale, "Incorrect starter for Timid Female")
        'Chimchar                  
        Assert.AreEqual(CUShort(425), testFile.NaiveMale, "Incorrect starter for Naïve Male")
        Assert.AreEqual(CUShort(425 + 600), testFile.ImpishFemale, "Incorrect starter for Impish Female")
        'Piplup                    
        Assert.AreEqual(CUShort(428), testFile.ImpishMale, "Incorrect starter for Impish Male")
        Assert.AreEqual(CUShort(428 + 600), testFile.QuirkyFemale, "Incorrect starter for Quirky Female")
        'Shinx                     
        Assert.AreEqual(CUShort(438), testFile.HastyMale, "Incorrect starter for Hasty Male")
        'Riolu                     
        Assert.AreEqual(CUShort(489), testFile.SassyMale, "Incorrect starter for Sassy Male")

        'Partners
        'Bulbasaur
        Assert.AreEqual(CUShort(1), testFile.Partner01, "Incorrect partner 01")
        'Charmander
        Assert.AreEqual(CUShort(4), testFile.Partner02, "Incorrect partner 02")
        'Squirtle
        Assert.AreEqual(CUShort(7), testFile.Partner03, "Incorrect partner 03")
        'Pikachu
        Assert.AreEqual(CUShort(25), testFile.Partner04, "Incorrect partner 04")
        'Vulpix
        Assert.AreEqual(CUShort(37 + 600), testFile.Partner18, "Incorrect partner 18")
        'Meowth
        Assert.AreEqual(CUShort(52), testFile.Partner20, "Incorrect partner 20")
        'Eevee
        Assert.AreEqual(CUShort(133 + 600), testFile.Partner14, "Incorrect partner 14")
        'Chikorita
        Assert.AreEqual(CUShort(752), testFile.Partner05, "Incorrect partner 05")
        'Cyndaquil
        Assert.AreEqual(CUShort(155), testFile.Partner06, "Incorrect partner 06")
        'Totodile
        Assert.AreEqual(CUShort(158), testFile.Partner07, "Incorrect partner 07")
        'Phanpy
        Assert.AreEqual(CUShort(258), testFile.Partner17, "Incorrect partner 17")
        'Treecko
        Assert.AreEqual(CUShort(280), testFile.Partner08, "Incorrect partner 08")
        'Torchic
        Assert.AreEqual(CUShort(283 + 600), testFile.Partner09, "Incorrect partner 09")
        'Mudkip
        Assert.AreEqual(CUShort(286), testFile.Partner10, "Incorrect partner 10")
        'Skitty
        Assert.AreEqual(CUShort(328+600), testFile.Partner19, "Incorrect partner 19")
        'Turtwig
        Assert.AreEqual(CUShort(422), testFile.Partner11, "Incorrect partner 11")
        'Chimchar
        Assert.AreEqual(CUShort(425), testFile.Partner12, "Incorrect partner 12")
        'Piplup
        Assert.AreEqual(CUShort(428), testFile.Partner13, "Incorrect partner 13")
        'Shinx
        Assert.AreEqual(CUShort(438), testFile.Partner15, "Incorrect partner 15")
        'Munchlax
        Assert.AreEqual(CUShort(488), testFile.Partner21, "Incorrect partner 21")
        'Riolu
        Assert.AreEqual(CUShort(489), testFile.Partner16, "Incorrect partner 16")
    End Sub

    <TestMethod> <TestCategory(EosTestCategory)> Public Sub item_p()
        Using testFile = GetAndTestFile(Of item_p)(Path.Combine(romDir, "data", "balance", "item_p.bin"), True)
            'Ensure data is at least somewhat valid
            Assert.AreEqual(1400, testFile.Items.Count, "Incorrect number of items")
        End Using
    End Sub

    <TestMethod> <TestCategory(EosTestCategory)> Public Sub item_s_p()
        Using testFile = GetAndTestFile(Of item_s_p)(Path.Combine(romDir, "data", "balance", "item_s_p.bin"), True)
            'Ensure data is at least somewhat valid
            Assert.AreEqual(956, testFile.Items.Count, "Incorrect number of items")
        End Using
    End Sub

    <TestMethod> <TestCategory(EosTestCategory)> Public Sub LanguageString()
        Using testFile = GetAndTestFile(Of LanguageString)(Path.Combine(romDir, "data", "message", "text_e.str"), True)
            'Ensure data is at least somewhat valid
            Assert.AreEqual(18451, testFile.Items.Count, "Incorrect number of items")
        End Using
    End Sub

    <TestMethod> <TestCategory(EosTestCategory)> Public Sub mappa_s()
        Using testFile = GetAndTestFile(Of mappa)(Path.Combine(romDir, "data", "balance", "mappa_s.bin"), False)
            'Ensure data is at least somewhat valid
            Assert.AreEqual(100, testFile.Dungeons.Count, "Incorrect number of items")
        End Using
    End Sub

    <TestMethod> <TestCategory(EosTestCategory)> Public Sub mappa_t()
        Using testFile = GetAndTestFile(Of mappa)(Path.Combine(romDir, "data", "balance", "mappa_t.bin"), False)
            'Ensure data is at least somewhat valid
            Assert.AreEqual(64, testFile.Dungeons.Count, "Incorrect number of items")
        End Using
    End Sub

    <TestMethod> <TestCategory(EosTestCategory)> Public Sub mappa_y()
        Using testFile = GetAndTestFile(Of mappa)(Path.Combine(romDir, "data", "balance", "mappa_y.bin"), False)
            'Ensure data is at least somewhat valid
            Assert.AreEqual(64, testFile.Dungeons.Count, "Incorrect number of items")
        End Using
    End Sub

End Class