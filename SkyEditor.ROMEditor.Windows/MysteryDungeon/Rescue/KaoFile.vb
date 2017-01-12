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
        AutoAddSir0HeaderRelativePointers = False
        Portraits = New List(Of Bitmap)
    End Sub

    Public Property Portraits As List(Of Bitmap)

    Public Overloads Sub CreateFile()
        CreateFile("")
    End Sub

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
            If index > 0 Then
                Dim nextPointer = pointers(count + 1)
                If nextPointer < 0 Then
                    nextPointer *= -1
                End If
                Dim length As Integer = pointers(count + 1) - index
                sections.Add(RawData(index, length))
            Else
                sections.Add(Nothing)
            End If
        Next
        '- The last section
        Dim lastPointer = pointers.Last
        If lastPointer > 0 Then
            sections.Add(RawData(pointers.Last, HeaderOffset - pointers.Last))
        End If

        '====================
        'Part 2: Parse the palettes
        '====================
        Dim palettes As New List(Of List(Of Color))
        For i = 0 To sections.Count - 1 Step 2
            If sections(i) IsNot Nothing Then
                Dim palette As New List(Of Color)
                For count = 0 To sections(i).Length - 1 Step 4
                    palette.Add(Color.FromArgb(sections(i)(count + 0), sections(i)(count + 1), sections(i)(count + 2)))
                Next
                palettes.Add(palette)
            Else
                palettes.Add(Nothing)
            End If
        Next

        '====================
        'Part 3: Decompress & Build the Bitmaps
        '====================

        'Add placeholders to avoid threading issues further on
        For count = 0 To sections.Count - 2 Step 2
            Portraits.Add(Nothing)
        Next

        'Decompress each portrait
        Using manager As New UtilityManager
            Dim tasks As New List(Of Task)
            For count = 0 To sections.Count - 1 Step 2
                Dim countInner = count
                tasks.Add(Task.Run(Async Function() As Task
                                       Dim portraitIndex = Math.Floor(countInner / 2)
                                       If sections(countInner + 1) IsNot Nothing Then
                                           'Create a temporary file
                                           Dim tempCompressed = Path.GetTempFileName
                                           File.WriteAllBytes(tempCompressed, sections(countInner + 1))

                                           'Decompress the file
                                           Dim tempDecompressed = Path.GetTempFileName
                                           Await manager.RunUnPX(tempCompressed, tempDecompressed)

                                           'Read the decompressed file
                                           Dim fileData = File.ReadAllBytes(tempDecompressed)

                                           'Cleanup
                                           File.Delete(tempCompressed)
                                           File.Delete(tempDecompressed)

                                           'Build the bitmap
                                           If fileData.Length > 0 Then
                                               Portraits(portraitIndex) = GraphicsHelpers.BuildPokemonPortraitBitmap(palettes(portraitIndex), fileData)
                                           End If
                                       Else
                                           Portraits(portraitIndex) = Nothing
                                       End If
                                   End Function))
            Next
            Await Task.WhenAll(tasks)
        End Using

    End Function

    Public Overrides Async Function OpenFile(filename As String, provider As IIOProvider) As Task
        Await MyBase.OpenFile(filename, provider)
        Await Initialize(provider.ReadAllBytes(filename))
    End Function

    Protected Overrides Async Function DoPreSave() As Task
        'Generate the palettes
        Dim palettes As New List(Of List(Of Color))
        For Each item In Portraits
            If item Is Nothing Then
                palettes.Add(Nothing)
            Else
                palettes.Add(GraphicsHelpers.GetKaoPalette(item))
            End If
        Next

        'Generate the decompressed data
        Dim decompressedPortraits As New List(Of Byte())
        For count = 0 To Portraits.Count - 1
            If Portraits(count) Is Nothing Then
                decompressedPortraits.Add(Nothing)
            Else
                decompressedPortraits.Add(GraphicsHelpers.Get4bppPortraitData(Portraits(count), palettes(count)))
            End If
        Next

        'Compress the portraits
        Dim compressedPortraits As New List(Of Byte())
        Using manager As New UtilityManager
            'Allocate space
            compressedPortraits.AddRange(Enumerable.Repeat(Of Byte())(Nothing, decompressedPortraits.Count))
            Dim f As New AsyncFor
            f.BatchSize = Environment.ProcessorCount * 2
            Await f.RunFor(Async Function(count As Integer) As Task
                               If decompressedPortraits(count) IsNot Nothing Then
                                   compressedPortraits(count) = Await manager.RunDoPX(decompressedPortraits(count), PXFormat.AT4PX)
                               End If
                           End Function, 0, decompressedPortraits.Count - 1)
        End Using

        'Generate the data to write to the file
        Dim dataSection As New List(Of Byte)
        Dim pointersSection As New List(Of Byte)
        Dim dataIndex = &H10
        For count = 0 To Portraits.Count - 1
            If Portraits(count) Is Nothing Then
                'This is how explorers does it, but this has not been observed in rescue team
                pointersSection.AddRange(BitConverter.GetBytes(dataIndex * -1))
            Else
                'Write the palette
                pointersSection.AddRange(BitConverter.GetBytes(dataIndex))

                For Each item In palettes(count)
                    dataSection.Add(item.R)
                    dataSection.Add(item.G)
                    dataSection.Add(item.B)
                    dataSection.Add(&H80) 'The purpose of this byte is unknown, but needs to remain in order to preserve alignment
                    dataIndex += 4
                Next

                'Write the data
                pointersSection.AddRange(BitConverter.GetBytes(dataIndex))
                dataSection.AddRange(compressedPortraits(count))

                dataIndex += compressedPortraits(count).Length

                'Integer-align the data
                While dataSection.Count Mod 4 <> 0
                    dataSection.Add(&HAA)
                    dataIndex += 1
                End While
            End If
        Next

        While pointersSection.Count < &H68
            pointersSection.Add(0)
        End While

        While pointersSection.Count Mod &H10 <> 0
            pointersSection.Add(&HAA)
        End While

        While dataSection.Count Mod 4 <> 0
            dataSection.Add(&HAA)
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
        If disposing AndAlso Portraits IsNot Nothing Then
            For Each item In Portraits
                If item IsNot Nothing Then
                    item.Dispose()
                End If
            Next
            Portraits = Nothing
        End If
    End Sub

End Class
