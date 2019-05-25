Imports System.IO
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Utilities
Imports SkyEditor.IO.FileSystem

Namespace Windows.FileFormats.Explorers.Script
    Public Class SSA
        Implements IOpenableFile
        Implements IOnDisk
        Implements INamed

        Public Property Filename As String Implements IOnDisk.Filename

        Public ReadOnly Property Name As String Implements INamed.Name
            Get
                Return Path.GetFileName(Filename)
            End Get
        End Property

        Public Async Function OpenFile(Filename As String, Provider As IFileSystem) As Task Implements IOpenableFile.OpenFile
            Me.Filename = Filename

            Using f As New GenericFile
                f.IsReadOnly = True
                Await f.OpenFile(Filename, Provider)

                f.Position = 0
                Dim numGroups = f.ReadUInt16
                Dim dataOffset = f.ReadUInt16 'Length of non-groups/start of groups (Z)
                Dim unkA = f.ReadUInt16 'Start of something, only in enter.sse
                Dim pokePos = f.ReadUInt16
                Dim objPos = f.ReadUInt16
                Dim backPos = f.ReadUInt16
                Dim unkE = f.ReadUInt16
                Dim movements = f.ReadUInt16
                Dim wordsGStart = f.ReadUInt16
            End Using
        End Function
    End Class

End Namespace
