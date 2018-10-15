Imports System.IO
Imports SkyEditor.Core
Imports SkyEditor.Core.Utilities

Namespace ProcessManagement
    ''' <summary>
    ''' Wrapper for unluac
    ''' </summary>
    Public Class Unluac
        Inherits Java

        ''' <summary>
        ''' Creates a new instance of the lua decompiler.
        ''' </summary>
        ''' <param name="scriptFilename">Filename of the compiled lua script.</param>
        Public Sub New(jarFilename As String, scriptFilename As String)
            MyBase.New(jarFilename, scriptFilename)
        End Sub

        ''' <summary>
        ''' Saves the result of the decompilation to the given file path
        ''' </summary>
        Public Async Function SaveAllOutput(destinationFilename As String) As Task
            File.WriteAllText(destinationFilename, Await GetAllOutput())
        End Function
    End Class
End Namespace

