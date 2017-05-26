Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Utilities

Namespace MysteryDungeon.PSMD
    ''' <summary>
    ''' Models a .bin file in the message directory of PMD: GTI and PSMD.
    ''' </summary>
    ''' <remarks>Credit to psy_commando for researching the format.</remarks>
    Public Class MessageBin
        Inherits Sir0
        Implements IOpenableFile

        Public Class EntryAddedEventArgs
            Inherits EventArgs

            Public Property NewID As UInteger
        End Class

        Public Sub New()
            MyBase.New
            Strings = New ObservableCollection(Of MessageBinStringEntry)
        End Sub

        Public Sub New(OpenReadOnly As Boolean)
            Me.New
            IsReadOnly = OpenReadOnly
            Strings = New ObservableCollection(Of MessageBinStringEntry)
        End Sub

        Public Event EntryAdded(sender As Object, e As EntryAddedEventArgs)

        ''' <summary>
        ''' Matches string hashes to the strings contained in the file.
        ''' </summary>
        ''' <returns>The games' scripts refer to the strings by this hash.</returns>
        Public Property Strings As ObservableCollection(Of MessageBinStringEntry) ' Dictionary(Of Integer, String)

        Public Overrides Sub CreateFile(Name As String, FileContents() As Byte)
            MyBase.CreateFile(Name, FileContents)

            ProcessData()
        End Sub

        Public Overrides Async Function OpenFile(Filename As String, Provider As IIOProvider) As Task Implements IOpenableFile.OpenFile
            Await MyBase.OpenFile(Filename, Provider)

            ProcessData()
        End Function

        Private Sub SetOriginalIndexes(strings As IEnumerable(Of MessageBinStringEntry))
            Dim index = 0
            For Each item In strings.OrderBy(Function(x) x.Pointer)
                item.OriginalIndex = index
                index += 1
            Next
        End Sub

        Private Sub ProcessData()
            Dim stringCount As Integer = BitConverter.ToInt32(ContentHeader, 0)
            Dim stringInfoPointer As Integer = BitConverter.ToInt32(ContentHeader, 4)

            For i = 0 To stringCount - 1
                Dim stringPointer As Integer = BitConverter.ToInt32(Read(stringInfoPointer + i * 12 + &H0, 4), 0)
                Dim stringHash As UInteger = BitConverter.ToUInt32(Read(stringInfoPointer + i * 12 + &H4, 4), 0)
                Dim unk As UInt32 = BitConverter.ToUInt32(Read(stringInfoPointer + i * 12 + &H8, 4), 0)

                Dim s As New Text.StringBuilder()
                Dim e = Text.UnicodeEncoding.Unicode

                'Parse the null-terminated UTF-16 string
                Dim j As Integer = 0
                Dim cRaw As Byte()
                Dim doEnd As Boolean = False
                Do
                    cRaw = Read(stringPointer + j * 2, 2)

                    'TODO: parse escape characters, as described in these posts:
                    'http://projectpokemon.org/forums/showthread.php?46904-Pokemon-Super-Mystery-Dungeon-And-PMD-GTI-Research-And-Utilities&p=211018&viewfull=1#post211018
                    'http://projectpokemon.org/forums/showthread.php?46904-Pokemon-Super-Mystery-Dungeon-And-PMD-GTI-Research-And-Utilities&p=210986&viewfull=1#post210986

                    If cRaw(1) >= 128 Then 'Most significant bit is set
                        s.Append("\")
                        s.Append(Conversion.Hex(cRaw(1)).PadLeft(2, "0"c))
                        s.Append(Conversion.Hex(cRaw(0)).PadLeft(2, "0"c))
                        j += 1
                    Else
                        Dim c = e.GetString(cRaw, 0, cRaw.Length)

                        If cRaw.SequenceEqual({0, 0}) Then
                            doEnd = True
                        Else
                            s.Append(c)
                            j += 1
                        End If
                    End If

                Loop Until doEnd

                Dim newEntry = New MessageBinStringEntry With {.Hash = stringHash, .Entry = s.ToString.Trim, .Unknown = unk, .Pointer = stringPointer}
                Strings.Add(newEntry)
            Next
            SetOriginalIndexes(Strings)
        End Sub

        Public Async Function OpenFileOnlyIDs(Filename As String, provider As IIOProvider) As Task
            Await MyBase.OpenFile(Filename, provider)

            Dim stringCount As Integer = BitConverter.ToInt32(ContentHeader, 0)
            Dim stringInfoPointer As Integer = BitConverter.ToInt32(ContentHeader, 4)

            For i = 0 To stringCount - 1
                Dim stringPointer As Integer = BitConverter.ToInt32(Await ReadAsync(stringInfoPointer + i * 12 + &H0, 4), 0)
                Dim stringHash As UInteger = BitConverter.ToUInt32(Await ReadAsync(stringInfoPointer + i * 12 + &H4, 4), 0)
                Dim unk As UInt32 = BitConverter.ToUInt32(Await ReadAsync(stringInfoPointer + i * 12 + &H8, 4), 0)

                'We're skipping reading the string, since this function only loads the IDs

                Dim newEntry = New MessageBinStringEntry With {.Hash = stringHash, .Entry = "", .Unknown = unk, .Pointer = stringPointer}
                Strings.Add(newEntry)
            Next
            SetOriginalIndexes(Strings)
        End Function

        Public Overrides Async Function Save(Destination As String, provider As IIOProvider) As Task
            Me.RelativePointers.Clear()
            'Sir0 header pointers
            Me.RelativePointers.Add(4)
            Me.RelativePointers.Add(4)

            'Generate sections
            Dim stringSection As New List(Of Byte)
            Dim infoSection As New List(Of Byte)
            For Each item In From s In Strings Order By s.Hash Ascending
                infoSection.AddRange(BitConverter.GetBytes(16 + stringSection.Count))
                infoSection.AddRange(BitConverter.GetBytes(item.Hash))
                infoSection.AddRange(BitConverter.GetBytes(item.Unknown))
                stringSection.AddRange(item.GetStringBytes)
            Next

            'Add pointers
            Me.RelativePointers.Add(stringSection.Count + 8)
            For count = 0 To Strings.Count - 2
                Me.RelativePointers.Add(&HC)
            Next

            'Write sections to file
            Me.Length = 16 + stringSection.Count + infoSection.Count
            Await Me.WriteAsync(16, stringSection.Count, stringSection.ToArray)
            Await Me.WriteAsync(16 + stringSection.Count, infoSection.Count, infoSection.ToArray)

            'Update header
            Dim headerBytes As New List(Of Byte)
            headerBytes.AddRange(BitConverter.GetBytes(Strings.Count))
            headerBytes.AddRange(BitConverter.GetBytes(16 + stringSection.Count))
            Me.ContentHeader = headerBytes.ToArray
            Me.RelativePointers.Add(&H10)

            'Let the general SIR0 stuff happen
            Await MyBase.Save(Destination, provider)
        End Function

        ''' <summary>
        ''' Gets the Pokemon names, if the current instance of <see cref="MessageBin"/> is the common file. 
        ''' </summary>
        ''' <returns>A dictionary matching the IDs of Pokemon to Pokemon names.</returns>
        ''' <exception cref="InvalidOperationException">Thrown if the current instance of <see cref="MessageBin"/> is not the common file.</exception>
        Public Function GetCommonPokemonNames() As Dictionary(Of Integer, String)
            'Get the hashes from the resources
            Dim pokemonNameHashes As New List(Of Integer)
            For Each item In My.Resources.PSMD_Pokemon_Name_Hashes.Replace(VBConstants.vbCrLf, VBConstants.vbLf).Split(VBConstants.vbLf).Select(Function(x) x.Trim)
                Dim hash As Integer
                If Integer.TryParse(item, hash) Then
                    pokemonNameHashes.Add(item)
                Else
                    Throw New Exception($"Invalid resource item: ""{item}""")
                End If
            Next

            'Get the corresponding names
            Dim pokemonNames As New Dictionary(Of Integer, String)
            pokemonNames.Add(0, My.Resources.Language.NonePokemon)
            For count = 0 To pokemonNameHashes.Count - 1
                Dim count2 = count 'Helps avoid potential weirdness from having an iterator variable in the lambda expression below
                pokemonNames.Add(count + 1, ((From s In Strings Where s.HashSigned = pokemonNameHashes(count2)).First).Entry)
            Next

            Return pokemonNames
        End Function

        ''' <summary>
        ''' Gets the move names, if the current instance of <see cref="MessageBin"/> is the common file. 
        ''' </summary>
        ''' <returns>A dictionary matching the IDs of moves to move names.</returns>
        ''' <exception cref="InvalidOperationException">Thrown if the current instance of <see cref="MessageBin"/> is not the common file.</exception>
        Public Function GetCommonMoveNames() As Dictionary(Of Integer, String)
            'Get the hashes from the resources
            Dim pokemonNameHashes As New List(Of Integer)
            For Each item In My.Resources.PSMD_Move_Name_Hashes.Replace(VBConstants.vbCrLf, VBConstants.vbLf).Split(VBConstants.vbLf)
                Dim trimmed = item.Trim
                Dim hash As Integer
                If Integer.TryParse(trimmed, hash) Then
                    pokemonNameHashes.Add(trimmed)
                Else
                    Throw New Exception($"Invalid resource item: ""{trimmed}""")
                End If
            Next

            'Get the corresponding names
            Dim pokemonNames As New Dictionary(Of Integer, String)
            pokemonNames.Add(0, My.Resources.Language.NonePokemon)
            For count = 0 To pokemonNameHashes.Count - 1
                Dim count2 = count 'Helps avoid potential weirdness from having an iterator variable in the lambda expression below
                pokemonNames.Add(count + 1, ((From s In Strings Where s.HashSigned = pokemonNameHashes(count2)).First).Entry)
            Next

            Return pokemonNames
        End Function

    End Class
End Namespace

