Imports System.Collections.Concurrent
Imports System.Drawing
Imports System.IO
Imports PPMDU
Imports SkyEditor.Core.IO
Imports SkyEditor.ROMEditor.Utilities

Namespace MysteryDungeon.Explorers
    Public Class Kaomado
        Implements IOpenableFile
        Implements IDisposable

        Public Property Portraits As List(Of Bitmap())

        Public Async Function Initialize(data As Byte()) As Task
            Portraits = New List(Of Bitmap())

            '====================
            'Part 1: Reading the table of contents
            '====================

            'This is the first pointer in the first non-null entry.
            'It will be used to determine the length of the ToC
            Dim firstPointer As Integer = BitConverter.ToInt32(data, &HA0)

            'Read the TOC
            Dim toc As New List(Of List(Of Integer))
            For blockIndex = &HA0 To firstPointer - 1 Step &HA0
                Dim block As New List(Of Integer)
                For pointerIndex = 0 To &HA0 Step 4
                    block.Add(BitConverter.ToInt32(data, blockIndex + pointerIndex))
                Next
                toc.Add(block)
            Next

            '====================
            'Part 2: Allocate space in Portraits
            '====================
            'This makes it so that each pokemon/portrait can be processed asynchronously with no threading issues
            For count = 0 To toc.Count - 1
                Dim array(40) As Bitmap
                Portraits.Add(array)
            Next

            '====================
            'Part 3: Start converting data into bitmaps
            '====================
            'This might take a while, so each portrait will be processed asynchronously
            Using manager As New UtilityManager
                Await manager.UnPX("", "") 'Ensure files are written
                Dim tasks As New List(Of Task)(toc.Count * 40)
                For pokemon = 0 To toc.Count - 1
                    For portrait = 0 To 39
                        Dim pokemonInner = pokemon
                        Dim portraitInner = portrait
                        tasks.Add(Task.Run(Async Function()
                                               Await ProcessBitmap(data, toc, pokemonInner, portraitInner, manager)
                                           End Function))
                    Next
                Next
                Await Task.WhenAll(tasks)
            End Using

        End Function

        Private Async Function ProcessBitmap(data As Byte(), toc As List(Of List(Of Integer)), pokemonID As Integer, portrait As Integer, manager As UtilityManager) As Task
            '====================
            'Part 4: Select the correct data
            '====================

            'Read the pointers
            Dim firstPointer = toc(pokemonID)(portrait)
            If firstPointer < 0 Then
                'This is a null pointer; nothing to convert
                Exit Function
            End If

            Dim secondPointer As Integer
            If portrait < toc(pokemonID).Count Then
                secondPointer = toc(pokemonID)(portrait + 1)
            Else
                secondPointer = toc(pokemonID + 1)(portrait - toc(pokemonID).Count)
            End If

            If secondPointer < 0 Then
                'The next pointer is a null pointer, but still marks the end of the current portrait
                secondPointer *= -1
            End If

            Dim length As Integer
            length = secondPointer - firstPointer

            'Read the palette
            Dim palette As New List(Of Color)
            For count = 0 To 47 Step 3
                Dim index = firstPointer + count
                palette.Add(Color.FromArgb(data(index + 0), data(index + 1), data(index + 2)))
            Next

            'Read the compressed data
            Dim compressedData = data.Skip(firstPointer + 48).Take(length - 48).ToArray

            '====================
            'Part 5: Decompress the tile data
            '====================
            Dim decompressedData As Byte()
            'Create a temporary file
            Dim tempCompressed = Path.GetTempFileName
            File.WriteAllBytes(tempCompressed, compressedData)

            'Decompress the file
            Dim tempDecompressed = Path.GetTempFileName
            Await manager.UnPX(tempCompressed, tempDecompressed)

            'Read the decompressed file
            decompressedData = File.ReadAllBytes(tempDecompressed)

            'Cleanup
            File.Delete(tempCompressed)
            File.Delete(tempDecompressed)

            '====================
            'Part 6: Generate the bitmap and put it in Portraits
            '====================
            If decompressedData.Length > 0 Then
                Portraits(pokemonID)(portrait) = GraphicsHelpers.BuildPokemonPortraitBitmap(palette, decompressedData)
            End If
        End Function

        Public Async Function OpenFile(Filename As String, Provider As IOProvider) As Task Implements IOpenableFile.OpenFile
            Await Initialize(Provider.ReadAllBytes(Filename))
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
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
