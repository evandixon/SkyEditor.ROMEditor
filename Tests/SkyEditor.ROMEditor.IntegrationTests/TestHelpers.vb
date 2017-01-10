Imports System.IO
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Utilities

Public Class TestHelpers
    Public Const AutomatedTestCategory As String = "Automated"
    Public Const ManualTestCategory As String = "Manual"
    Public Shared Function GetAndTestFile(Of T As IOpenableFile)(filePath As String, enableResaveTest As Boolean, provider As IOProvider) As T
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
            Assert.AreEqual(originalData.Length, newData.Length, "Length of file altered with no logical changes.  Filename: " & Path.GetFileName(filePath))
            For count = 0 To originalData.Length - 1
                Assert.AreEqual(originalData(count), newData(count), "Data altered starting at index " & count & ".  Filename: " & Path.GetFileName(filePath))
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
End Class
