Imports SkyEditor.Core.IO

Namespace MysteryDungeon
    ''' <summary>
    ''' Common Pokémon Mystery Dungeon File Wrapper
    ''' </summary>
    ''' <remarks>
    ''' File format documentation: https://projectpokemon.org/wiki/Pmd2_SIR0
    ''' 
    ''' When saving in this class, child classes must first populate <see cref="Sir0.RelativePointers"/>, including the pointers in the SIR0 header (so the first two items should be 4 and 4, then add relative offsets of subsequent pointers).
    ''' </remarks>
    Public Class Sir0
        Inherits GenericFile
        Implements IOpenableFile

        Public Sub New()
            MyBase.New
            PaddingByte = &H0
            ResizeFileOnLoad = True
            AutoAddSir0HeaderRelativePointers = False
            RelativePointers = New List(Of Integer)
            EnableInMemoryLoad = True
        End Sub

        ''' <summary>
        ''' The byte used to pad blocks that aren't divisible by 0x10.
        ''' </summary>
        ''' <returns></returns>
        Protected Property PaddingByte As Byte

        ''' <summary>
        ''' Offset of the sub header
        ''' </summary>
        ''' <returns></returns>
        Protected Property HeaderOffset As Integer

        ''' <summary>
        ''' Offset of the pointers block
        ''' </summary>
        ''' <returns></returns>
        Protected Property PointerOffset As Integer

        ''' <summary>
        ''' Length of the pointers block
        ''' </summary>
        ''' <returns></returns>
        Private ReadOnly Property PointerLength As Integer
            Get
                Return Length - PointerOffset
            End Get
        End Property

        ''' <summary>
        ''' Length of the padding at the end of the header.
        ''' </summary>
        ''' <returns></returns>
        Private Property HeaderPadding As Integer

        ''' <summary>
        ''' Length of the sub header
        ''' </summary>
        ''' <returns></returns>
        Private ReadOnly Property HeaderLength As Integer
            Get
                Return PointerOffset - HeaderOffset - HeaderPadding
            End Get
        End Property

        ''' <summary>
        ''' Length of the data block
        ''' </summary>
        ''' <returns></returns>
        Private ReadOnly Property DataLength As Integer
            Get
                Return Length - 16 - HeaderLength - PointerLength
            End Get
        End Property

        ''' <summary>
        ''' Contents of the sub header
        ''' </summary>
        ''' <returns></returns>
        Protected Property ContentHeader As Byte()

        ''' <summary>
        ''' The decoded pointers in the pointers block.
        ''' Each number is the number of bytes after the previous pointer in the file.
        ''' </summary>
        ''' <returns></returns>
        Public Property RelativePointers As List(Of Integer)

        ''' <summary>
        ''' Whether or not to trim the pointers and sub header blocks after the file is loaded.
        ''' If true, it is easier to append data to the data block.
        ''' </summary>
        ''' <returns></returns>
        Protected Property ResizeFileOnLoad As Boolean

        ''' <summary>
        ''' Whether or not to automatically add the SIR0 header relative pointers.
        ''' Defaults to false for backwards compatibility.
        ''' </summary>
        Protected Property AutoAddSir0HeaderRelativePointers As Boolean

        Public Overridable Async Function IsOfType(File As GenericFile) As Task(Of Boolean)
            'Todo: possible parsing of file to ensure file contents are OK.
            'Checking the magic should be good enough.
            Return File.Length >= 32 AndAlso
                (Await File.Read(0)) = &H53 AndAlso 'S
                (Await File.Read(1)) = &H49 AndAlso 'I
                (Await File.Read(2)) = &H52 AndAlso 'R
                (Await File.Read(3)) = &H30         '0
        End Function

        Public Overrides Sub CreateFile(Name As String, FileContents() As Byte)
            MyBase.CreateFile(Name, FileContents)

            If FileContents.Length > 0 Then
                ProcessData()
            End If
        End Sub

        Public Overrides Async Function OpenFile(Filename As String, Provider As IOProvider) As Task Implements IOpenableFile.OpenFile
            Await MyBase.OpenFile(Filename, Provider)
            ProcessData()
        End Function

        Protected Overridable Function DoPreSave() As Task
            'The header and relative pointers must be set by child classes

            Me.RawData(0, 4) = {&H53, &H49, &H52, &H30}

            'Update subheader length
            Dim oldLength = Me.Length 'the new header offset
            Me.Length += Me.ContentHeader.Length 'Change the file length
            Me.Int32(&H4) = oldLength 'Update the header pointer
            Me.HeaderOffset = oldLength

            'Update subHeader
            RawData(HeaderOffset, ContentHeader.Length) = ContentHeader

            'Pad the footer
            While Not Length Mod 16 = 0
                Length += 1
                RawData(Me.Length - 1) = PaddingByte
            End While

            'Write the pointers
            Dim pointerSection As New List(Of Byte)
            Dim pointers As IEnumerable(Of Integer)
            If AutoAddSir0HeaderRelativePointers Then
                pointers = {4, 4}.Concat(RelativePointers)
            Else
                pointers = RelativePointers
            End If
            For Each item In RelativePointers
                If item < 128 Then 'If the most significant bit is not 1
                    pointerSection.Add(CByte(item))
                Else
                    Dim workingBytes As New List(Of Byte)
                    Dim workingItem = item

                    workingBytes.Add(workingItem And &H7F)
                    workingItem = workingItem >> 7

                    While workingItem > 0
                        workingBytes.Add((workingItem And &H7F) Or &H80)
                        workingItem = workingItem >> 7
                    End While

                    For count = workingBytes.Count - 1 To 0 Step -1
                        pointerSection.Add(workingBytes(count))
                    Next
                End If
            Next
            pointerSection.Add(0) 'Seems to mark the end of the pointers

            oldLength = Me.Length
            Me.Length += pointerSection.Count
            Me.Int32(&H8) = oldLength
            Me.PointerOffset = oldLength
            Me.RawData(oldLength, pointerSection.Count) = pointerSection.ToArray

            While Not Length Mod 16 = 0
                Length += 1
                RawData(Me.Length - 1) = PaddingByte
            End While
            Return Task.FromResult(0)
        End Function

        Public Overrides Async Function Save(Destination As String, provider As IOProvider) As Task
            Await DoPreSave()

            Await MyBase.Save(Destination, provider)

            'Saving multiple times like this will make the second time fail, because the file length is changing.  
            'To change it back to a good working size, we'll reload the SIR0 portions.
            ProcessData()
        End Function

        Public Overrides Function GetDefaultExtension() As String
            Return ".bin"
        End Function

        Public Overrides Function GetSupportedExtensions() As IEnumerable(Of String)
            Return {".bin"}
        End Function

        Public Async Function GetRawData() As Task(Of Byte())
            Await DoPreSave()
            Dim data = RawData
            ProcessData()
            Return data
        End Function

        Private Sub ProcessData()
            RelativePointers = New List(Of Integer)
            HeaderOffset = Me.Int32(&H4)
            PointerOffset = Me.Int32(&H8)

            'Adjust header length to ignore padding
            If PaddingByte > 0 Then 'Check this so we don't accidentally remove a valid 0 value
                For i = HeaderOffset + HeaderLength - 1 To HeaderOffset Step -1
                    If RawData(i) = PaddingByte Then
                        HeaderPadding += 1
                    Else
                        Exit For
                    End If
                Next
            End If
            ContentHeader = RawData(HeaderOffset, HeaderLength)

            Dim isConstructing As Boolean = False
            Dim constructedPointer As Integer = 0
            For count = PointerOffset To Length - 1
                Dim current = RawData(count)
                If current >= 128 Then 'if the most significant bit is 1
                    isConstructing = True
                    constructedPointer = constructedPointer << 7 Or (current And &H7F)
                Else
                    If isConstructing Then
                        constructedPointer = constructedPointer << 7 Or (current And &H7F)
                        RelativePointers.Add(constructedPointer)
                        isConstructing = False
                        constructedPointer = 0
                    Else
                        If current = 0 Then
                            Exit For
                        Else
                            RelativePointers.Add(current)
                        End If
                    End If
                End If
            Next

            'Remove the header and pointer sections, because it will be reconstructed on save
            If Not Me.IsReadOnly AndAlso ResizeFileOnLoad Then
                Me.Length = Me.Length - Me.PointerLength - Me.HeaderLength
            End If

        End Sub

    End Class
End Namespace

