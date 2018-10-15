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

        Public Shared Async Function IsFileOfType(file As GenericFile) As Task(Of Boolean)
            If file.Length < 32 Then
                'File is too small
                Return False
            End If

            If Not ((Await file.ReadAsync(0)) = &H53 AndAlso 'S
                (Await file.ReadAsync(1)) = &H49 AndAlso 'I
                (Await file.ReadAsync(2)) = &H52 AndAlso 'R
                (Await file.ReadAsync(3)) = &H30) Then    '0
                'Magic doesn't match
                Return False
            End If

            Dim HeaderOffset = Await file.ReadInt32Async(&H4)
            Dim PointerOffset = Await file.ReadInt32Async(&H8)

            If file.Length <= HeaderOffset Then
                'Header pointer is outside the file
                Return False
            End If

            If file.Length <= PointerOffset Then
                'Pointer section is outside the file
                Return False
            End If

            Return True
        End Function

        ''' <summary>
        ''' Creates a new instance of <see cref="Sir0"/>
        ''' </summary>
        Public Sub New()
            MyBase.New
            PaddingByte = &H0
            ResizeFileOnLoad = True
            AutoAddSir0HeaderRelativePointers = False
            RelativePointers = New List(Of Integer)
            SubHeaderRelativePointers = New List(Of Integer)
            EnableInMemoryLoad = True
        End Sub

        ''' <summary>
        ''' Creates a new, read-only instance of <see cref="Sir0"/>
        ''' </summary>
        ''' <param name="file"></param>
        Public Sub New(file As GenericFile)
            MyBase.New(file)
            PaddingByte = &H0
            ResizeFileOnLoad = False
            AutoAddSir0HeaderRelativePointers = False
            RelativePointers = New List(Of Integer)
            SubHeaderRelativePointers = New List(Of Integer)
            EnableInMemoryLoad = True
            ProcessData()
        End Sub

#Region "Properties"

        ''' <summary>
        ''' The byte used to pad blocks that aren't divisible by 0x10.
        ''' </summary>
        Protected Property PaddingByte As Byte

        ''' <summary>
        ''' Offset of the sub header
        ''' </summary>
        Protected Property HeaderOffset As Integer

        ''' <summary>
        ''' Offset of the pointers block
        ''' </summary>
        Protected Property PointerOffset As Integer

        ''' <summary>
        ''' Length of the pointers block
        ''' </summary>
        Private ReadOnly Property PointerLength As Integer
            Get
                Return Length - PointerOffset
            End Get
        End Property

        ''' <summary>
        ''' Length of the padding at the end of the header.
        ''' </summary>
        Private Property HeaderPadding As Integer

        ''' <summary>
        ''' Length of the sub header
        ''' </summary>
        Private ReadOnly Property HeaderLength As Integer
            Get
                Return PointerOffset - HeaderOffset - HeaderPadding
            End Get
        End Property

        ''' <summary>
        ''' Length of the data block
        ''' </summary>
        Private ReadOnly Property DataLength As Integer
            Get
                Return Length - 16 - HeaderLength - PointerLength
            End Get
        End Property

        ''' <summary>
        ''' Contents of the sub header
        ''' </summary>
        Public Property ContentHeader As Byte()

        ''' <summary>
        ''' The decoded pointers in the pointers block.
        ''' Each number is the number of bytes after the previous pointer in the file.
        ''' </summary>
        Public Property RelativePointers As List(Of Integer)

        ''' <summary>
        ''' Indexes of pointers in the content header
        ''' </summary>
        Public Property SubHeaderRelativePointers As List(Of Integer)

        ''' <summary>
        ''' Whether or not to trim the pointers and sub header blocks after the file is loaded.
        ''' If true, it is easier to append data to the data block.
        ''' </summary>
        Protected Property ResizeFileOnLoad As Boolean

        ''' <summary>
        ''' Whether or not to automatically add the SIR0 header relative pointers.
        ''' Defaults to false for backwards compatibility.
        ''' </summary>
        Public Property AutoAddSir0HeaderRelativePointers As Boolean

        ''' <summary>
        ''' Whether or not to automatically add the SIR0 header relative pointers for the content header.
        ''' Defaults to false for backwards compatibility.
        ''' </summary>
        Public Property AutoAddSir0SubHeaderRelativePointers As Boolean

#End Region

        Public Overridable Overloads Sub CreateFile()
            CreateFile("", {})
        End Sub

        Public Overrides Sub CreateFile(Name As String, FileContents() As Byte)
            MyBase.CreateFile(Name, FileContents)

            If FileContents.Length > 0 Then
                ProcessData()
            End If
        End Sub

        Public Overrides Async Function OpenFile(Filename As String, Provider As IIOProvider) As Task Implements IOpenableFile.OpenFile
            Await MyBase.OpenFile(Filename, Provider)
            ProcessData()
        End Function

        Public Overrides Async Function OpenFile(file As GenericFile) As Task
            Await MyBase.OpenFile(file)
            ProcessData()
        End Function

        Protected Overridable Async Function DoPreSave() As Task
            'Set the length to be the minimum size for the header, in case it wasn't already set
            If Me.Length = 0 Then
                SetLength(&H10)
            End If

            'The header and relative pointers must be set by child classes
            Await Me.WriteAsync(0, 4, {&H53, &H49, &H52, &H30})

            'Update subheader length
            Dim oldLength = Me.Length 'the new header offset
            SetLength(Me.Length + Me.ContentHeader.Length) 'Change the file length
            Await Me.WriteInt32Async(&H4, oldLength) 'Update the header pointer
            Me.HeaderOffset = oldLength

            'Update subHeader
            Await Me.WriteAsync(HeaderOffset, ContentHeader.Length, ContentHeader)

            'Pad the footer
            While Not Length Mod 16 = 0
                SetLength(Length + 1)
                Await Me.WriteAsync(Me.Length - 1, PaddingByte)
            End While

            'Write the pointers
            Dim pointerSection As New List(Of Byte)
            Dim pointers As IEnumerable(Of Integer)

            If AutoAddSir0HeaderRelativePointers Then
                If RelativePointers.Any() Then
                    Dim firstPointer = RelativePointers.First()
                    firstPointer += 8
                    pointers = {4, 4}.Concat({firstPointer}).Concat(RelativePointers.Skip(1))
                Else
                    pointers = {4, 4}.Concat(RelativePointers.Skip(1))
                End If
            Else
                pointers = RelativePointers
            End If

            If AutoAddSir0SubHeaderRelativePointers AndAlso SubHeaderRelativePointers.Any() Then
                'Assume pointers doesn't contain any pointers in the sub-header
                Dim lastPointerOffset = pointers.Sum()
                Dim contentHeaderPointerBase = HeaderOffset - lastPointerOffset
                pointers = pointers.Concat({contentHeaderPointerBase + SubHeaderRelativePointers.First()})
                If SubHeaderRelativePointers.Count > 1 Then
                    pointers = pointers.Concat(SubHeaderRelativePointers.Skip(1))
                End If
            End If

            For Each item In pointers
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
            SetLength(Me.Length + pointerSection.Count)
            Await Me.WriteInt32Async(&H8, oldLength)
            Me.PointerOffset = oldLength
            Await Me.WriteAsync(oldLength, pointerSection.Count, pointerSection.ToArray)

            While Not Length Mod 16 = 0
                SetLength(Length + 1)
                Await WriteAsync(Me.Length - 1, PaddingByte)
            End While
        End Function

        Public Overrides Async Function Save(Destination As String, provider As IIOProvider) As Task
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

        ''' <summary>
        ''' Sets the body of the content to the given data.
        ''' </summary>
        Public Async Function SetContent(content As Byte()) As Task
            SetLength(&H10 + content.Length)
            Await Me.WriteAsync(&H10, content.Length, content)
        End Function

        Public Async Function GetRawData() As Task(Of Byte())
            Await DoPreSave()
            Dim data = Await ReadAsync()
            ProcessData()
            Return data
        End Function

        Private Sub ProcessData()
            RelativePointers = New List(Of Integer)
            HeaderOffset = Me.ReadInt32(&H4)
            PointerOffset = Me.ReadInt32(&H8)

            'Adjust header length to ignore padding
            If PaddingByte > 0 Then 'Check this so we don't accidentally remove a valid 0 value
                For i = HeaderOffset + HeaderLength - 1 To HeaderOffset Step -1
                    If Read(i) = PaddingByte Then
                        HeaderPadding += 1
                    Else
                        Exit For
                    End If
                Next
            End If
            ContentHeader = Read(HeaderOffset, HeaderLength)

            Dim isConstructing As Boolean = False
            Dim constructedPointer As Integer = 0
            For count = PointerOffset To Length - 1
                Dim current = Read(count)
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
                SetLength(Me.Length - Me.PointerLength - Me.HeaderLength)
            End If

        End Sub
    End Class
End Namespace

