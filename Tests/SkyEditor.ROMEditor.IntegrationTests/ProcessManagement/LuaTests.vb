Imports System.IO
Imports SkyEditor.ROMEditor.ProcessManagement

Namespace ProcessManagement

    <TestClass>
    Public Class LuaTests

        <TestMethod>
        Public Sub TestLuaDecompile()
            Dim decompiled = My.Resources.compiled
            Dim testFilename = "LuaTestCompiled.lua"
            Dim outputFilename = "LuaTestDecomp.lua"
            File.WriteAllBytes(testFilename, decompiled)

            Try
                Using unluacManager As New UnluacManager
                    unluacManager.DecompileScript(testFilename, outputFilename).Wait()

                    Assert.IsTrue(File.Exists(outputFilename), "Output file was not created.")

                    Dim actual = File.ReadAllText(outputFilename)
                    Assert.AreEqual(My.Resources.testScript, actual, "Decompiled script does not match source")
                End Using
            Finally
                If File.Exists("LuaTestScript.lua") Then
                    File.Delete("LuaTestScript.lua")
                End If
            End Try
        End Sub
    End Class

End Namespace
