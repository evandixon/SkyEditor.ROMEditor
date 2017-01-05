Imports System.Drawing

Namespace Utilities
    Public Class GraphicsHelpers

        ''' <summary>
        ''' Builds a 40x40 bitmap image, as seen in the DS Mystery Dungeon games' kao files.
        ''' </summary>
        ''' <param name="palette">The portrait's palette.</param>
        ''' <param name="data">4bpp raw data of the image.</param>
        ''' <returns>A bitmap representing the given data.</returns>
        ''' <exception cref="BadImageFormatException">Thrown if <paramref name="data"/>'s length is not long enough to represent a 40x40 4bpp bitmap.</exception>
        Public Shared Function BuildPokemonPortraitBitmap(palette As List(Of Color), data As Byte()) As Bitmap
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

                For y As Byte = 0 To 7
                    For x As Byte = 0 To 7
                        If colors.Count <= count * 64 + y * 8 + x Then
                            Throw New BadImageFormatException("The tile size is too small.")
                        End If
                        g.FillRectangle(New SolidBrush(palette(colors(count * 64 + y * 8 + x))), x, y, 1, 1)
                    Next
                Next
                g.Save()
                tiles.Add(i)
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
    End Class
End Namespace

