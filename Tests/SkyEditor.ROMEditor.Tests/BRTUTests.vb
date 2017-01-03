Imports System.Security.Cryptography
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Windows.Providers

Public Class BRTUTests

    Private Const EosTestCategory As String = "BRT (U) Files"

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
End Class
