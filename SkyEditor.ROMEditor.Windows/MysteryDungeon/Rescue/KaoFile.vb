Imports System.Drawing
Imports System.IO
Imports PPMDU
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Utilities
Imports SkyEditor.ROMEditor.MysteryDungeon
Imports SkyEditor.ROMEditor.MysteryDungeon.Explorers
Imports SkyEditor.ROMEditor.Utilities

Public Class KaoFile
    Inherits ExplorersSir0
    Implements IDisposable

    Public Sub New()
        ThingsToDispose = New List(Of IDisposable)
        AutoAddSir0HeaderRelativePointers = False
        Portraits = New List(Of Bitmap)
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

    Protected Overrides Async Function DoPreSave() As Task
        'Generate the palettes
        Dim palettes As New List(Of List(Of Color))
        For Each item In Portraits
            Dim palette = GraphicsHelpers.GetPalette(item, 16)
            'To-do: Order the colors as done in the game.  There should be no side-effect of leaving them unsorted, however

            ''This didn't have any effect
            'palette = (From p In palette
            'Order By p.R, p.G, p.B).ToList

            palettes.Add(palette)
        Next

        'Generate the decompressed data
        Dim decompressedPortraits As New List(Of Byte())
        For count = 0 To Portraits.Count - 1
            decompressedPortraits.Add(GraphicsHelpers.Get4bppPortraitData(Portraits(count), palettes(count)))
        Next

        'Compress the portraits
        Dim compressedPortraits As New List(Of Byte())
        Dim manager As New UtilityManager
        For Each item In decompressedPortraits
            'Create a temporary file
            Dim tempDecompressed = Path.GetTempFileName
            File.WriteAllBytes(tempDecompressed, item)

            'Decompress the file
            Dim tempCompressed = Path.GetTempFileName
            Await manager.DoPX(tempDecompressed, tempCompressed, PXFormat.AT4PX)

            'Read the decompressed file
            compressedPortraits.Add(File.ReadAllBytes(tempCompressed))

            'Cleanup
            File.Delete(tempCompressed)
            File.Delete(tempDecompressed)
        Next
        Try
            manager.Dispose()
        Catch ex As Exception

        End Try

        'Generate the data to write to the file
        Dim dataSection As New List(Of Byte)
        Dim pointersSection As New List(Of Byte)
        Dim dataIndex = &H10
        For count = 0 To Portraits.Count - 1
            'Write the palette
            pointersSection.AddRange(BitConverter.GetBytes(dataIndex))

            For Each item In palettes(count)
                dataSection.Add(item.R)
                dataSection.Add(item.G)
                dataSection.Add(item.B)
                dataSection.Add(&H80) 'The purpose of this byte is unknown, but needs to remain in order to preserve alignment
            Next

            dataIndex += 64

            pointersSection.AddRange(BitConverter.GetBytes(dataIndex))
            dataSection.AddRange(compressedPortraits(count))

            dataIndex += compressedPortraits(count).Length
        Next

        While pointersSection.Count < &H68
            pointersSection.Add(0)
        End While

        While pointersSection.Count Mod &H10 <> 0
            pointersSection.Add(&HAA)
        End While

        'Write the sections to the file
        Me.Length = &H10 + dataSection.Count
        RawData(&H10, dataSection.Count) = dataSection.ToArray
        ContentHeader = pointersSection.ToArray

        'Add the pointer offsets, as needed by the SIR0 file format
        RelativePointers.Clear()
        'SIR0 Header
        RelativePointers.Add(4)
        RelativePointers.Add(4)
        RelativePointers.Add(8 + dataSection.Count)
        For count = 1 To Portraits.Count - 1
            RelativePointers.Add(4)
            RelativePointers.Add(4)
        Next
        RelativePointers.Add(4) 'Not sure about this one - it was in the original file

        Await MyBase.DoPreSave()
    End Function

    Protected Overrides Sub Dispose(disposing As Boolean)
        MyBase.Dispose(disposing)
        For Each item In ThingsToDispose
            item.Dispose()
        Next
    End Sub

End Class
