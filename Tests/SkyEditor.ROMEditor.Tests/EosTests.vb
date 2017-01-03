Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports SkyEditor.Core.Windows.Providers
Imports SkyEditor.ROMEditor
Imports SkyEditor.ROMEditor.MysteryDungeon.Explorers

<TestClass()> Public Class EosTests

    Private Const EosTestCategory As String = "EOS Files"

    'Files for all tests
    Dim romFilename As String = "eos-u.nds"
    Dim romDir As String = "extracted-EOS-U"

    Dim provider As New WindowsIOProvider


    <TestInitialize()> Public Sub TestInit()
        'Set up
        Try
            Using md5 As New MD5CryptoServiceProvider
                Dim hash = md5.ComputeHash(My.Resources.eos_u)
                If Not hash.SequenceEqual(My.Resources.EoS_U_MD5) Then
                    Assert.Inconclusive("Incorrect test ROM specified.  Should be a trimmed North America PMD: Explorers of Sky ROM.")
                End If
            End Using

            File.WriteAllBytes(romFilename, My.Resources.eos_u)
            Using nds As New GenericNDSRom
                nds.OpenFile(romFilename, provider).Wait()
                nds.Unpack(romDir, provider).Wait()
            End Using
        Catch ex As Exception
            Assert.Inconclusive("Failed to set up.  Exception message: " & ex.Message)
        End Try
    End Sub

    <TestCleanup> Public Sub Cleanup()
        If File.Exists(romFilename) Then
            File.Delete(romFilename)
        End If
        If Directory.Exists(romDir) Then
            Directory.Delete(romDir, True)
        End If
    End Sub

    <TestMethod> <TestCategory(EosTestCategory)> Public Sub item_p()
        Dim item_pPath = Path.Combine(romDir, "data", "balance", "item_p.bin")
        Dim originalData = File.ReadAllBytes(item_pPath)
        Dim tempFilename As String = Path.GetTempFileName

        'Open the file
        Dim testFile As New item_p
        testFile.OpenFile(item_pPath, provider).Wait()

        'Ensure data is at least somewhat valid
        Assert.AreEqual(1400, testFile.Items.Count, "Incorrect number of items")

        'Save the file
        testFile.Save(tempFilename, provider).Wait()

        'Ensure the data is the same
        Dim newData = File.ReadAllBytes(tempFilename)
        Assert.AreEqual(originalData.Length, newData.Length, "Length of file altered with no logical changes")
        For count = 0 To originalData.Length - 1
            Assert.AreEqual(originalData(count), newData(count), "Data altered starting at index " & count)
        Next

        'Cleanup
        testFile.Dispose()
        File.Delete(tempFilename)
    End Sub

End Class