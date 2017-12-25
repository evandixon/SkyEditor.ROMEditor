Imports System.Drawing
Imports EveryFileExplorer

Namespace MysteryDungeon.PSMD
    Public Class PmdGraphics
        Public Shared Function ReadPortrait(data As Byte()) As Bitmap
            Dim portrait = Textures.ToBitmap(data, 0, 64, 64, Textures.ImageFormat.RGB8)
            portrait.RotateFlip(RotateFlipType.Rotate180FlipX)
            Return portrait
        End Function

        Public Shared Function SavePortrait(image As Bitmap) As Byte()
            Dim rotated = image.Clone()
            rotated.RotateFlip(RotateFlipType.Rotate180FlipX)
            Return Textures.FromBitmap(rotated, Textures.ImageFormat.RGB8)
        End Function
    End Class
End Namespace
