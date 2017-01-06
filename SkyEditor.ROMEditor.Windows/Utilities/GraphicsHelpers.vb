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

                        Dim colorIndex = count * 64 + y * 8 + x
                        Dim pixelColor As Color
                        If colors(colorIndex) = 0 Then
                            pixelColor = Color.Transparent
                        Else
                            pixelColor = palette(colors(colorIndex))
                        End If

                        g.FillRectangle(New SolidBrush(pixelColor), x, y, 1, 1)
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

        ''' <summary>
        ''' Gets raw 4bpp image data for DS Mystery Dungeon portraits.
        ''' </summary>
        ''' <param name="image">The portrait image</param>
        ''' <param name="palette">The portrait palette used to generate the raw data</param>
        ''' <returns>4bpp raw image data representing <paramref name="image"/></returns>
        ''' <exception cref="BadImageFormatException">Thrown when a color in <paramref name="image"/> is not in <paramref name="palette"/></exception>
        Public Shared Function Get4bppPortraitData(image As Bitmap, palette As List(Of Color)) As Byte()
            Dim data As New List(Of Byte)
            Dim colors As New List(Of Color)

            'Convert the colors to a 1-D list
            For tileY = 0 To 4
                For tileX = 0 To 4
                    For y = 0 To 7
                        For x = 0 To 7
                            colors.Add(image.GetPixel(tileX * 8 + x, tileY * 8 + y))
                        Next
                    Next
                Next
            Next

            'Convert the 1-D list to bytes
            For count = 0 To colors.Count - 1 Step 2
                Dim color0 As Integer
                Dim color1 As Integer

                If colors(count).A = 0 Then 'Transparent color
                    color0 = 0
                Else
                    Try
                        color0 = palette.IndexOf(colors(count))
                    Catch ex As Exception
                        Throw
                    End Try

                End If

                If colors(count + 1).A = 0 Then 'Transparent color
                    color1 = 0
                Else
                    color1 = palette.IndexOf(colors(count + 1))
                End If

                If color0 < 0 OrElse color1 < 0 Then
                    Throw New BadImageFormatException("Color not found in the desired palette")
                End If

                data.Add(CByte(color0) Or CByte(color1) << 4)
            Next

            Return data.ToArray
        End Function

        ''' <summary>
        ''' Gets the palette of the image.
        ''' </summary>
        ''' <param name="image">Image from which to get the palette</param>
        ''' <param name="paletteSize">Maximum size of the palette.</param>
        ''' <returns>A list of color representing the palette</returns>
        ''' <exception cref="BadImageFormatException">Thrown if <paramref name="image"/> has more colors than allowed by <paramref name="paletteSize"/></exception>
        Public Shared Function GetPalette(image As Bitmap, paletteSize As Integer) As List(Of Color)
            Dim output As New List(Of Color)(paletteSize)
            For x = 0 To image.Size.Width - 1
                For y = 0 To image.Size.Height - 1
                    Dim color = image.GetPixel(x, y)
                    If color.A <> 0 AndAlso Not output.Contains(color) Then
                        If output.Count < paletteSize Then
                            output.Add(color)
                        Else
                            Throw New BadImageFormatException("Too many colors in the image.")
                        End If
                    End If
                Next
            Next
            While output.Count < paletteSize
                output.Add(Color.Black)
            End While
            Return output
        End Function

        ''' <summary>
        ''' Gets a 16 color palette, the first color of which is transparent, for use with saving kao files.
        ''' </summary>
        Public Shared Function GetKaoPalette(image As Bitmap) As List(Of Color)
            Dim palette = GraphicsHelpers.GetPalette(image, 15) 'Palette can store 16 colors, but the first color is transparent, so there's really only 15

            'Find a color that's not in the palette (placeholder color to be the transparent color)
            Dim transparent = Color.FromArgb(0, 0, 0) 'Black is the default color in the games
            While palette.Contains(transparent)
                transparent = Color.FromArgb(transparent.R + 1, 0, 0)
            End While

            'Put the transparent color in the beginning of the palette
            Return {transparent}.Concat(palette).ToList
        End Function
    End Class
End Namespace

