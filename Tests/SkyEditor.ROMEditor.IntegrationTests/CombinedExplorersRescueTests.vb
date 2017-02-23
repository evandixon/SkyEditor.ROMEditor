Imports System.IO
Imports SkyEditor.Core.IO
Imports SkyEditor.ROMEditor.MysteryDungeon.Explorers
Imports SkyEditor.ROMEditor.MysteryDungeon.Rescue

<TestClass> Public Class CombinedExplorersRescueTests

    Private Const TestCategory As String = "Combined EOS/BRT Tests"

    Dim provider As IIOProvider

    <TestInitialize> Public Sub TestInit()
        provider = New PhysicalIOProvider
        If Not BRTUTests.IsTestInitialized Then
            BRTUTests.UnpackFiles(provider)
        End If
        If Not EoSUTests.IsTestInitialized Then
            EoSUTests.UnpackFiles(provider)
        End If
    End Sub

    <TestCleanup> Public Sub TestCleanup()
        BRTUTests.CleanupFiles(provider)
        EoSUTests.CleanupFiles(provider)
    End Sub

    <TestMethod> <TestCategory(TestCategory)> <TestCategory(TestHelpers.ManualTestCategory)> Public Sub CopyEosToBRTKaomado()
        Dim monster As New SBin
        monster.OpenFile(Path.Combine(BRTUTests.romDir, "data", "monster.sbin"), provider).Wait()

        Using kaomado As New Kaomado
            kaomado.OpenFile(Path.Combine(EoSUTests.romDir, "data", "font", "kaomado.kao"), provider).Wait()
            kaomado.CopyToRescueTeam(monster, True).Wait()
        End Using

        If Not Directory.Exists("ManualTests") Then
            Directory.CreateDirectory("ManualTests")
        End If

        monster.Save(Path.Combine("ManualTests", "eos-copied-monster.sbin"), provider).Wait()

        'Extract
        Dim monster1 As New SBin
        monster1.OpenFile(Path.Combine("ManualTests", "eos-copied-monster.sbin"), provider).Wait()

        For Each item In monster1.Files.Where(Function(x) x.Key.StartsWith("kao"))
            File.WriteAllBytes(Path.Combine("ManualTests", item.Key & ".bin"), item.Value)
            Using kao As New KaoFile
                kao.Initialize(item.Value).Wait()
                For count = 0 To kao.Portraits.Count - 1
                    Dim targetFilename = Path.Combine("ManualTests", "Extracted", item.Key, count & ".png")
                    If Not Directory.Exists(Path.GetDirectoryName(targetFilename)) Then
                        Directory.CreateDirectory(Path.GetDirectoryName(targetFilename))
                    End If
                    kao.Portraits(count)?.Save(targetFilename, Drawing.Imaging.ImageFormat.Png)
                Next
            End Using
        Next
    End Sub
End Class
