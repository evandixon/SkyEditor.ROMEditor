Imports System.Drawing
Imports System.IO
Imports PPMDU
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Utilities
Imports SkyEditor.ROMEditor.MysteryDungeon
Imports SkyEditor.ROMEditor.Utilities

Public Class KaoFile
    Inherits Sir0
    Implements IDisposable

    Public Sub New()
        ThingsToDispose = New List(Of IDisposable)
    End Sub

    Protected Property ThingsToDispose As List(Of IDisposable)

    Public Property Portraits As List(Of Bitmap)

    Public Async Function Initialize(data As Byte()) As Task
        MyBase.CreateFile(data)
        Portraits = New List(Of Bitmap)

        '====================
        'Part 1: Reading the raw sections
        '====================

        'Read the pointers from the content header
        Dim pointers As New List(Of Integer)
        For count = 0 To ContentHeader.Length - 1 Step 4
            Dim pointer = BitConverter.ToInt32(ContentHeader, count)
            If pointer > 0 AndAlso pointer <> &HAAAAAAAA Then
                pointers.Add(pointer)
            End If
        Next

        'Read the individual sections
        '- All the sections except the last one
        Dim sections As New List(Of Byte())
        For count = 0 To pointers.Count - 2
            Dim index As Integer = pointers(count)
            Dim length As Integer = pointers(count + 1) - index
            sections.Add(RawData(index, length))
        Next
        '- The last section
        sections.Add(RawData(pointers.Last, HeaderOffset - pointers.Last))

        '====================
        'Part 2: Parse the palettes
        '====================
        Dim palettes As New List(Of List(Of Color))
        For i = 0 To sections.Count - 1 Step 2
            Dim palette As New List(Of Color)
            For count = 0 To sections(i).Length - 1 Step 4
                palette.Add(Color.FromArgb(sections(i)(count + 0), sections(i)(count + 1), sections(i)(count + 2)))
            Next
            palettes.Add(palette)
        Next

        '====================
        'Part 3: Decompress & Build the Bitmaps
        '====================

        'Add placeholders to avoid threading issues further on
        For count = 0 To sections.Count - 2 Step 2
            Portraits.Add(Nothing)
        Next

        'Decompress each portrait
        Dim manager As New UtilityManager
        Await manager.UnPX("", "") 'Ensure files are written
        Dim tasks As New List(Of Task)
        For count = 0 To sections.Count - 1 Step 2
            Dim countInner = count
            tasks.Add(Task.Run(Async Function() As Task

                                   'Create a temporary file
                                   Dim tempCompressed = Path.GetTempFileName
                                   File.WriteAllBytes(tempCompressed, sections(countInner + 1))

                                   'Decompress the file
                                   Dim tempDecompressed = Path.GetTempFileName
                                   Await manager.UnPX(tempCompressed, tempDecompressed)

                                   'Read the decompressed file
                                   Dim fileData = File.ReadAllBytes(tempDecompressed)

                                   'Cleanup
                                   File.Delete(tempCompressed)
                                   File.Delete(tempDecompressed)

                                   'Build the bitmap
                                   If fileData.Length > 0 Then
                                       Dim paletteIndex = Math.Floor(countInner / 2)
                                       Portraits(paletteIndex) = GraphicsHelpers.BuildPokemonPortraitBitmap(palettes(paletteIndex), fileData)
                                   End If

                               End Function))
        Next
        Await Task.WhenAll(tasks)

        Try
            manager.Dispose()
        Catch ex As Exception
            'Failed to dispose; try again when the current object is disposed
            ThingsToDispose.Add(manager)
        End Try

    End Function

    Public Overrides Async Function OpenFile(filename As String, provider As IOProvider) As Task
        Await MyBase.OpenFile(filename, provider)
        Await Initialize(provider.ReadAllBytes(filename))
    End Function

    Protected Overrides Sub DoPreSave()
        Throw New NotImplementedException
        MyBase.DoPreSave()
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        MyBase.Dispose(disposing)
        For Each item In ThingsToDispose
            item.Dispose()
        Next
    End Sub

End Class
