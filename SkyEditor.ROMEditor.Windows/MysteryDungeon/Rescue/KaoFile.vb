Imports System.Drawing
Imports System.IO
Imports PPMDU
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Utilities
Imports SkyEditor.ROMEditor.MysteryDungeon

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
                                       Portraits(paletteIndex) = BuildBitmap(palettes(paletteIndex), fileData)
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

    Private Function DrawTile(TileSize As Integer, pixels As List(Of Byte), palette As List(Of Color), pixelOffset As Integer) As Bitmap
        If TileSize = 2 Then
            Dim output As New Bitmap(2, 2)
            output.SetPixel(0, 1, palette(pixels(pixelOffset + 0)))
            output.SetPixel(1, 1, palette(pixels(pixelOffset + 1)))
            output.SetPixel(0, 0, palette(pixels(pixelOffset + 2)))
            output.SetPixel(1, 0, palette(pixels(pixelOffset + 3)))
            Return output
        Else
            Dim output As New Bitmap(TileSize, TileSize)
            Dim g = Graphics.FromImage(output)
            Dim half As Integer = TileSize / 2
            Dim childPixelCount As Integer = (TileSize / 2) ^ 2
            g.DrawImage(DrawTile(TileSize / 2, pixels, palette, pixelOffset + childPixelCount * 0), New Point(0, half))
            g.DrawImage(DrawTile(TileSize / 2, pixels, palette, pixelOffset + childPixelCount * 1), New Point(half, half))
            g.DrawImage(DrawTile(TileSize / 2, pixels, palette, pixelOffset + childPixelCount * 2), New Point(0, 0))
            g.DrawImage(DrawTile(TileSize / 2, pixels, palette, pixelOffset + childPixelCount * 3), New Point(half, 0))
            g.Save()
            Return output
        End If
    End Function

    Private Function BuildBitmap(palette As List(Of Color), data As Byte()) As Bitmap
        Dim tiles As New List(Of Bitmap)

        Dim colors As New List(Of Byte)
        For Each b In data
            colors.Add((b) And &HF)
            colors.Add((b >> 4) And &HF)
        Next

        'Build Tiles
        For count = 0 To 24
            Dim i As New Bitmap(8, 8)
            Dim g As Graphics = Graphics.FromImage(i)

            'Dim colorIndex = count * 64
            For y As Byte = 0 To 7
                For x As Byte = 0 To 7
                    If colors.Count <= count * 64 + y * 8 + x Then
                        Throw New BadImageFormatException("The tile size is too small.")
                    End If
                    g.FillRectangle(New SolidBrush(palette(colors(count * 64 + y * 8 + x))), x, y, 1, 1)
                    'colorIndex += 1
                Next
            Next
            g.Save()
            tiles.Add(i)
            'tiles.Add(DrawTile(8, colors, palette, count * 64))
        Next

        'Arrange Tiles
        Dim portrait As New Bitmap(40, 40)
        Dim portraitGraphics = Graphics.FromImage(portrait)
        For x As Byte = 0 To 4
            For y As Byte = 0 To 4
                portraitGraphics.DrawImage(tiles(y * 5 + x), x * 8, y * 8)
            Next
        Next
        portraitGraphics.Save()
        Return portrait
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
