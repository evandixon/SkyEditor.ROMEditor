Imports System.IO

Namespace ProcessManagement
    Public Class UnluacManager
        Implements IDisposable

        Private Shared Function GetUnluacPath() As String
            Dim tempPath = Path.Combine(Path.GetTempPath, "SkyEditor.ROMEditor-" & Guid.NewGuid.ToString(), "unluac.jar")
            If Not Directory.Exists(Path.GetDirectoryName(tempPath)) Then
                Directory.CreateDirectory(Path.GetDirectoryName(tempPath))
            End If
            Return tempPath
        End Function

        Public Sub New()
            Filename = GetUnluacPath()
            If Not File.Exists(Filename) Then
                File.WriteAllBytes(Filename, My.Resources.Resources.unluac)
            End If
        End Sub

        Public ReadOnly Property Filename As String

        Public Async Function DecompileScript(sourceScriptFilename As String, destinationScriptFilename As String) As Task
            Using unluac As New Unluac(Filename, sourceScriptFilename)
                Await unluac.SaveAllOutput(destinationScriptFilename)
            End Using
        End Function

        Public Async Function DecompileScript(sourceScriptFilename As String) As Task(Of String)
            Using unluac As New Unluac(Filename, sourceScriptFilename)
                Return Await unluac.GetAllOutput()
            End Using
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    File.Delete(Filename)
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace

