Imports System.IO
Imports System.Security.Cryptography
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Windows.Providers
Imports SkyEditor.ROMEditor
Imports SkyEditor.ROMEditor.MysteryDungeon.Rescue

<TestClass> Public Class BRTUTests

    Private Const Category As String = "BRT (U) Files"

    'Files for all tests
    Dim romFilename As String = "brt-u.nds"
    Dim romDir As String = "extracted-BRT-U"

    Dim provider As IOProvider


    <TestInitialize()> Public Sub TestInit()
        'Set up
        provider = New WindowsIOProvider
        Try
            Using md5 As New MD5CryptoServiceProvider
                Dim hash = md5.ComputeHash(My.Resources.brt_u)
                If Not hash.SequenceEqual(My.Resources.BRT_U_MD5) Then
                    Assert.Inconclusive("Incorrect test ROM specified.  Should be a trimmed North America PMD: Blue Rescue Team ROM.")
                End If
            End Using

            provider.WriteAllBytes(romFilename, My.Resources.brt_u)
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

    <TestMethod> <TestCategory(Category)> Public Sub SBinFileFormat()
        Dim dungeon = TestHelpers.GetAndTestFile(Of SBin)(Path.Combine(romDir, "data", "dungeon.sbin"), True, provider)
        Dim effect = TestHelpers.GetAndTestFile(Of SBin)(Path.Combine(romDir, "data", "effect.sbin"), True, provider)
        Dim ground = TestHelpers.GetAndTestFile(Of SBin)(Path.Combine(romDir, "data", "ground.sbin"), True, provider)
        Dim monster = TestHelpers.GetAndTestFile(Of SBin)(Path.Combine(romDir, "data", "monster.sbin"), True, provider)
        Dim ornament = TestHelpers.GetAndTestFile(Of SBin)(Path.Combine(romDir, "data", "ornament.sbin"), True, provider)
        Dim sample = TestHelpers.GetAndTestFile(Of SBin)(Path.Combine(romDir, "data", "sample.sbin"), True, provider)
        Dim system = TestHelpers.GetAndTestFile(Of SBin)(Path.Combine(romDir, "data", "system.sbin"), True, provider)
        Dim titlemenu = TestHelpers.GetAndTestFile(Of SBin)(Path.Combine(romDir, "data", "titlemenu.sbin"), True, provider)
    End Sub

    <TestMethod> <TestCategory("Temporary Test")> Public Sub MonsterTest()
        Dim monster As New SBin
        monster.OpenFile(Path.Combine(romDir, "data", "monster.sbin"), provider).Wait()

        For Each item In monster.Files.Where(Function(x) x.Key.StartsWith("kao"))
            Dim kao As New KaoFile
            kao.Initialize(item.Value).Wait()
            For count = 0 To kao.Portraits.Count - 1
                Dim targetFilename = Path.Combine("portraits", item.Key, count & ".png")
                If Not Directory.Exists(Path.GetDirectoryName(targetFilename)) Then
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFilename))
                End If
                kao.Portraits(count)?.Save(targetFilename, Drawing.Imaging.ImageFormat.Png)
            Next
            kao.Dispose()
        Next
    End Sub
End Class
