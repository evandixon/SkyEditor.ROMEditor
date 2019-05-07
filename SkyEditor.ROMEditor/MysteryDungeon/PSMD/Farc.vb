Imports System.Collections.Concurrent
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Force.Crc32
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.IO.PluginInfrastructure
Imports SkyEditor.Core.Utilities
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD.Pokemon

Namespace MysteryDungeon.PSMD
    Public Class Farc
        Implements IOpenableFile
        Implements IOnDisk
        Implements ISavableAs
        Implements IDetectableFileType
        Implements IDisposable
        Implements IIOProvider
        Implements IReportProgress

        Private Shared Function GetFileSearchRegexQuestionMarkOnly(searchPattern As String) As StringBuilder
            Dim parts = searchPattern.Split("?"c)
            Dim regexString = New StringBuilder()
            For Each item In parts
                regexString.Append(Regex.Escape(item))
                If item <> parts(parts.Length - 1) Then
                    regexString.Append(".?")
                End If
            Next

            Return regexString
        End Function

        Private Shared Function GetFileSearchRegex(searchPattern As String) As String
            Dim asteriskParts = searchPattern.Split("*"c)
            Dim regexString = New StringBuilder()
            For Each part In asteriskParts
                If String.IsNullOrEmpty(part) Then
                    regexString.Append(".*")
                Else
                    regexString.Append(GetFileSearchRegexQuestionMarkOnly(part))
                End If
            Next

            Return regexString.ToString()
        End Function

        Public Shared Async Function Pack(sourceDirectory As String, outputFile As String, provider As IIOProvider) As Task
            Using f As New Farc
                For Each item In provider.GetFiles(sourceDirectory, "*", True)
                    f.WriteAllBytes(Path.GetFileName(item), provider.ReadAllBytes(item))
                Next

                Await f.Save(outputFile, provider)
            End Using
        End Function

        Private Shared Function CreateByteArrayHashCode(data As Byte()) As Integer
            Dim hashCode As Integer = 0
            Dim byteShift = 0
            For Each b In data
                hashCode = hashCode Xor (b << byteShift * 8)

                If byteShift >= 3 Then
                    byteShift = 0
                Else
                    byteShift += 1
                End If
            Next
            Return hashCode
        End Function

        Public Sub New()
            ResetWorkingDirectory()
            Entries = New ConcurrentDictionary(Of UInteger, FarcEntry)
        End Sub

        Public Sub New(sir0Type As Integer, useFilenames As Boolean)
            Me.New
            If sir0Type <> 4 AndAlso sir0Type <> 5 Then
                Throw New ArgumentOutOfRangeException(NameOf(sir0Type))
            End If
            Me.Sir0Type = sir0Type
            Me.UseFilenames = useFilenames
        End Sub

        ''' <summary>
        ''' Raised when the file has been saved
        ''' </summary>
        Public Event FileSaved As EventHandler Implements ISavable.FileSaved

        ''' <summary>
        ''' Raised when the progress of archive extraction has progressed
        ''' </summary>
        Public Event ProgressChanged As EventHandler(Of ProgressReportedEventArgs) Implements IReportProgress.ProgressChanged

        ''' <summary>
        ''' Raised when the archive extraction has completed
        ''' </summary>
        Public Event Completed As EventHandler Implements IReportProgress.Completed

        Protected Property InnerData As GenericFile
        Protected Property DataOffset As Integer
        Protected Property UnknownHeaderData As Byte()
        Protected Property Sir0Type As Integer
        Protected Property Sir0FatType As Integer
        Protected Property UseFilenames As Boolean

        Public Property PreLoadFiles As Boolean = False
        Public Property EnableInMemoryLoad As Boolean = True

        ''' <summary>
        ''' Whether or not to allow multiple files to reference the same data when saving. If enabled, files containing the same data will be stored once with two or more references.
        ''' </summary>
        Public Property EnableMultiReferenceOnSave As Boolean = True
        Public Property Filename As String Implements IOnDisk.Filename
        Private Property Entries As ConcurrentDictionary(Of UInteger, FarcEntry) 'Key: hash, value: entry

        Public Async Function OpenFile(filename As String, provider As IIOProvider) As Task Implements IOpenableFile.OpenFile
            Dim f As New GenericFile
            f.EnableInMemoryLoad = Me.EnableInMemoryLoad
            Await f.OpenFile(filename, provider)

            UnknownHeaderData = Await f.ReadAsync(4, &H1C)
            Sir0Type = Await f.ReadInt32Async(&H20)
            Dim sir0Offset = Await f.ReadInt32Async(&H24)
            Dim sir0Length = Await f.ReadInt32Async(&H28)
            DataOffset = Await f.ReadInt32Async(&H2C)
            Dim dataLength = Await f.ReadInt32Async(&H30)

            If Sir0Type <> 5 AndAlso Sir0Type <> 4 Then
                Throw New NotSupportedException("Only FARC v4 and v5 are supported for the time being")
            End If

            Dim header = New FarcFat5
            Await header.OpenFile(Await f.ReadAsync(sir0Offset, sir0Length))
            Sir0FatType = header.Sir0Fat5Type
            UseFilenames = header.UsesFilenames

            For Each item In header.Entries
                Dim fileEntry As New FarcEntry
                If item.IsFilenameSet Then
                    fileEntry.Filename = item.Filename
                Else
                    fileEntry.FilenameHash = item.FilenameHash
                End If
                fileEntry.DataEntry = item

                If PreLoadFiles Then
                    Await GetFileDataAsync(fileEntry)
                Else
                    'Don't load the file data yet, to save time and resources
                End If

                Entries(fileEntry.FilenameHash.Value) = fileEntry
            Next

            Me.InnerData = f
            Me.Filename = filename

            'Try to load filenames
            If Not header.UsesFilenames Then
                Select Case Path.GetFileName(filename).ToLower()
                    Case "image_2d.bin"
                        Dim dbPath = Path.Combine(Path.GetDirectoryName(filename), "image_2d_database.bin")
                        If provider.FileExists(dbPath) Then
                            Dim dbFile As New SAJD
                            Await dbFile.OpenFile(dbPath, provider)
                            SetFilenames(dbFile.Entries.Select(Function(e) e.FileName & ".img"))
                        End If
                    Case "pokemon_graphic.bin"
                        Dim dbPath = Path.Combine(Path.GetDirectoryName(filename), "pokemon_graphics_database.bin")
                        If provider.FileExists(dbPath) Then
                            Dim dbFile As New PGDB
                            Await dbFile.OpenFile(dbPath, provider)

                            SetFilenames(dbFile.Entries.Select(Function(e) e.PrimaryBgrsFilename).Distinct())
                            SetFilenames(dbFile.Entries.Select(Function(e) e.SecondaryBgrsName & ".bgrs").Distinct())

                            'Identify BGRS files that were not referenced, and infer the names
                            Dim af As New AsyncFor
                            Await af.RunForEach(Entries.Values.Where(Function(e) e.Filename Is Nothing),
                                         Async Function(unmatchedFile As FarcEntry) As Task
                                             Dim data = Await GetFileDataAsync(unmatchedFile)
                                             Using dataFile As New GenericFile()
                                                 dataFile.CreateFile(data)

                                                 Dim bgrs As New BGRS
                                                 If Await bgrs.IsOfType(dataFile) Then
                                                     Await bgrs.OpenFile(data)

                                                     SetFilenames({bgrs.BgrsName & ".bgrs"}, False)
                                                 End If
                                             End Using
                                         End Function)

                            'Infer BCH files from BGRS
                            Dim bchFilenames As New List(Of String)
                            For Each bgrsFilename In Entries.Values.Where(Function(e) e.Filename IsNot Nothing AndAlso e.Filename.EndsWith(".bgrs")).Select(Function(e) e.Filename)
                                Dim bgrs As New BGRS
                                Await bgrs.OpenFile("/" & bgrsFilename, Me)
                                bchFilenames.Add(bgrs.ReferencedBchFileName)
                                For Each item In bgrs.Animations
                                    bchFilenames.Add(item.Name & ".bchmata")
                                    bchFilenames.Add(item.Name & ".bchskla")
                                Next
                            Next

                            SetFilenames(bchFilenames.Distinct(), False)
                        End If
                    Case "message.bin",
                         "message_en.bin",
                         "message_fr.bin",
                         "message_ge.bin",
                         "message_it.bin",
                         "message_sp.bin",
                         "message_us.bin",
                         "message_debug.bin",
                         "message_debug_en.bin",
                         "message_debug_fr.bin",
                         "message_debug_ge.bin",
                         "message_debug_it.bin",
                         "message_debug_sp.bin",
                         "message_debug_us.bin"
                        Dim dbPath = Path.ChangeExtension(filename, ".lst")
                        If provider.FileExists(dbPath) Then
                            Dim lines = provider.ReadAllText(dbPath).Split(vbLf)
                            SetFilenames(lines.Select(Function(l) Path.GetFileName(l.Trim)))
                        End If
                    Case "face_graphic.bin"
                        Dim msgDebugPath = Path.Combine(Path.GetDirectoryName(filename), "message_debug.bin")
                        Dim graphicsDbPath = Path.Combine(Path.GetDirectoryName(filename), "pokemon_graphics_database.bin")
                        Dim actorDataPath = Path.Combine(Path.GetDirectoryName(filename), "pokemon", "pokemon_actor_data_info.bin")

                        Dim pokemonNames As New List(Of String)
                        If provider.FileExists(msgDebugPath) Then
                            Dim debugMsg As New Farc
                            Await debugMsg.OpenFile(msgDebugPath, provider)

                            Dim commonDebugMsg As New MessageBinDebug
                            Await commonDebugMsg.OpenFile("common.dbin", debugMsg)

                            pokemonNames.AddRange(commonDebugMsg.GetPsmdCommonPokemonNames().Select(Function(p) p.Value.ToLower().Replace("pokemon_", "")))
                        End If

                        If provider.FileExists(graphicsDbPath) Then
                            Dim graphicsDb As New PGDB
                            Await graphicsDb.OpenFile(graphicsDbPath, provider)
                            pokemonNames.AddRange(graphicsDb.Entries.Select(Function(x) x.ActorName))
                        End If

                        If provider.FileExists(actorDataPath) Then
                            Dim actorInfo As New ActorDataInfo
                            Await actorInfo.OpenFile(actorDataPath, provider)
                            pokemonNames.AddRange(actorInfo.Entries.Select(Function(x) x.Name.ToLower()))
                        End If

                        pokemonNames = pokemonNames.Distinct().ToList()

                        Dim potentialFilenames As New List(Of String)
                        For Each pokemonName In pokemonNames
                            For emotionNumber = 0 To 50
                                potentialFilenames.Add($"{pokemonName}_{emotionNumber.ToString().PadLeft(2, "0")}.bin")
                                potentialFilenames.Add($"{pokemonName}_hanten_{emotionNumber.ToString().PadLeft(2, "0")}.bin")
                                potentialFilenames.Add($"{pokemonName}_f_{emotionNumber.ToString().PadLeft(2, "0")}.bin")
                                potentialFilenames.Add($"{pokemonName}_f{emotionNumber.ToString().PadLeft(2, "0")}.bin")
                                potentialFilenames.Add($"{pokemonName}_f_hanten_{emotionNumber.ToString().PadLeft(2, "0")}.bin")
                                potentialFilenames.Add($"{pokemonName}_r_{emotionNumber.ToString().PadLeft(2, "0")}.bin")
                            Next
                        Next
                        SetFilenames(potentialFilenames)
                End Select

            End If
        End Function

        Protected Sub RefreshEntryCache()
            Dim newCache As New ConcurrentDictionary(Of UInteger, FarcEntry)
            For Each item In Entries.Values
                newCache(item.FilenameHash) = item
            Next

            Dim oldCache = Entries
            Entries = newCache

            oldCache.Clear()
        End Sub

        Protected Function GetFileEntry(filename As String) As FarcEntry
            Dim hex As UInteger
            Dim hash As UInteger
            If UInteger.TryParse(filename, NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, hex) Then
                'Filename is unknown, use raw hash
                hash = hex
            Else
                'Hash the filename
                hash = PmdFunctions.Crc32Hash(filename)
            End If

            Dim entry As FarcEntry = Nothing
            If Entries.ContainsKey(hash) Then
                entry = Entries(hash)
            Else
                'We couldn't find it in the cache, and they contain the same number of items (as expected)
                'So let's not do a full search to save on time
                Return Nothing
            End If

            If entry IsNot Nothing AndAlso Not String.IsNullOrEmpty(entry.Filename) Then
                entry.Filename = filename
            End If

            Return entry
        End Function

        Protected Function GetFileData(entry As FarcEntry) As Byte()
            If entry.FileData Is Nothing Then
                entry.FileData = InnerData.Read(DataOffset + entry.DataEntry.DataOffset, entry.DataEntry.DataLength)
            End If
            Return entry.FileData
        End Function

        Public Function GetFileData(filename As String) As Byte()
            Dim entry = GetFileEntry(filename)
            If entry IsNot Nothing Then
                Return GetFileData(entry)
            Else
                Return Nothing
            End If
        End Function

        Protected Async Function GetFileDataAsync(entry As FarcEntry) As Task(Of Byte())
            If entry.FileData Is Nothing Then
                entry.FileData = Await InnerData.ReadAsync(DataOffset + entry.DataEntry.DataOffset, entry.DataEntry.DataLength)
            End If
            Return entry.FileData
        End Function

        Public Async Function GetFileDataAsync(filename As String) As Task(Of Byte())
            Dim entry = GetFileEntry(filename)
            If entry IsNot Nothing Then
                Return Await GetFileDataAsync(entry)
            Else
                Return Nothing
            End If
        End Function

        Public Function GetEntries() As IEnumerable(Of FarcEntry)
            Return Entries.Values
        End Function

        Public Sub ResizeFileData(filename As String, newSize As Integer)
            Dim entry = GetFileEntry(filename)
            GetFileData(entry) 'Ensure file data is loaded
            Array.Resize(entry.FileData, newSize)
        End Sub

        Public Function SetFilenames(filenames As IEnumerable(Of String), Optional ClearCache As Boolean = True) As Integer
            If ClearCache Then
                RefreshEntryCache()
            End If

            Dim numberSet As Integer = 0
            For Each item In filenames
                Dim hash = PmdFunctions.Crc32Hash(item)
                If Entries.ContainsKey(hash) Then
                    Entries(hash).TrySetFilename(item)
                    numberSet += 1
                End If
            Next
            Return numberSet
        End Function

        Public Async Function Extract(outputDirectory As String, provider As IIOProvider) As Task
            IsCompleted = False

            Dim onProgressed = Sub(sender As Object, e As ProgressReportedEventArgs)
                                   Progress = e.Progress
                                   Message = e.Message
                                   IsIndeterminate = e.IsIndeterminate
                                   RaiseEvent ProgressChanged(Me, New ProgressReportedEventArgs With {.Progress = e.Progress, .Message = Message, .IsIndeterminate = IsIndeterminate})
                               End Sub

            Dim a As New AsyncFor
            a.RunSynchronously = Not InnerData.IsThreadSafe
            AddHandler a.ProgressChanged, onProgressed
            Await a.RunForEach(Entries.Values, Async Function(item As FarcEntry) As Task
                                                   provider.WriteAllBytes(Path.Combine(outputDirectory, If(item.Filename, item.FilenameHash.Value.ToString("X"))), Await GetFileDataAsync(item))
                                               End Function)
            RemoveHandler a.ProgressChanged, onProgressed
            IsCompleted = True
            RaiseEvent Completed(Me, New EventArgs)
        End Function

        Public Async Function Save(filename As String, provider As IIOProvider) As Task Implements ISavableAs.Save
            'Analyze data to identify duplicate entries (i.e. make sure files with the same data are not added multiple times, instead having multiple references to the same data)
            Dim condensedEntries As New List(Of EntryMapping)

            Dim ordered As IOrderedEnumerable(Of FarcEntry)
            If Me.UseFilenames Then
                ordered = Entries.Values.OrderBy(Function(e) e.Filename)
            Else
                ordered = Entries.Values.OrderBy(Function(e) e.FilenameHash)
            End If
            If EnableMultiReferenceOnSave Then
                For Each item In ordered
                    Dim data = Await GetFileDataAsync(item)
                    Dim hashCode = CreateByteArrayHashCode(data)

                    '"Where" criteria first compares hash code to disregard incorrect matches
                    'THEN it double-checks equality by checking the array reference if the hashes match
                    'THEN it compares the actual data if the object reference is different and the hashes match
                    'If changing this, take care to not change this order, because short-circuit logic should greatly improve performance
                    Dim mapping = condensedEntries.
                        Where(Function(x) x.FileDataHashCode = hashCode AndAlso (data Is x.FileData OrElse x.FileData.SequenceEqual(data))).
                        FirstOrDefault

                    If mapping IsNot Nothing Then
                        mapping.PossibleFilenames.Add(item.FilenameOrHash)
                    Else
                        Dim newMapping As New EntryMapping
                        newMapping.FileData = item.FileData
                        newMapping.PossibleFilenames = New List(Of String)
                        newMapping.PossibleFilenames.Add(item.FilenameOrHash)
                        newMapping.Filename = item.FilenameOrHash
                        condensedEntries.Add(newMapping)
                    End If
                Next
            Else
                For Each item In ordered
                    Dim newMapping As New EntryMapping
                    newMapping.FileData = Await GetFileDataAsync(item)
                    newMapping.PossibleFilenames = New List(Of String)
                    newMapping.PossibleFilenames.Add(item.FilenameOrHash)
                    newMapping.Filename = item.FilenameOrHash
                    condensedEntries.Add(newMapping)
                Next
            End If

            'Write the data to the file
            Using f As New GenericFile
                f.EnableInMemoryLoad = False
                Await f.OpenFile(filename, provider)

                Dim fat As New FarcFat5
                fat.Sir0Fat5Type = If(Me.UseFilenames, 0, 1)
                Dim fileData As New List(Of Byte)

                For Each item In condensedEntries
                    'Add all filenames/hashes to this particular file at the same time
                    'fat.GetRawData should properly order these further on
                    For Each filename In item.PossibleFilenames
                        fat.Entries.Add(New FarcFat5.Entry With {
                                    .Filename = filename,
                                    .DataOffset = fileData.Count,
                                    .DataLength = item.FileData.Length
                                    })
                    Next

                    fileData.AddRange(item.FileData)

                    Dim paddingLength = 16 - (fileData.Count Mod 16)
                    If paddingLength < 16 Then
                        fileData.AddRange(Enumerable.Repeat(Of Byte)(0, paddingLength))
                    End If
                Next

                Dim fatData = Await fat.GetRawData()
                Dim farcHeader As New List(Of Byte)
                farcHeader.AddRange({&H46, &H41, &H52, &H43}) 'Magic: FARC

                If UnknownHeaderData?.Length = &H1C Then
                    farcHeader.AddRange(UnknownHeaderData)
                Else
                    farcHeader.AddRange(BitConverter.GetBytes(0)) '0x4
                    farcHeader.AddRange(BitConverter.GetBytes(0)) '0x8
                    farcHeader.AddRange(BitConverter.GetBytes(2)) '0xC
                    farcHeader.AddRange(BitConverter.GetBytes(0)) '0x10
                    farcHeader.AddRange(BitConverter.GetBytes(0)) '0x14
                    farcHeader.AddRange(BitConverter.GetBytes(7)) '0x18
                    farcHeader.AddRange(BitConverter.GetBytes(&H77EA3CA4)) '0x1C
                End If

                farcHeader.AddRange(BitConverter.GetBytes(Sir0Type)) '0x20
                farcHeader.AddRange(BitConverter.GetBytes(&H80)) '0x24
                farcHeader.AddRange(BitConverter.GetBytes(fatData.Length)) '0x28
                farcHeader.AddRange(BitConverter.GetBytes(&H80 + fatData.Length)) '0x2C
                farcHeader.AddRange(BitConverter.GetBytes(fileData.Count)) '0x30

                f.SetLength(farcHeader.Count + &H4C + fatData.Length + fileData.Count)
                Await f.WriteAsync(0, farcHeader.ToArray())
                Await f.WriteAsync(farcHeader.Count, Array.CreateInstance(GetType(Byte), &H4C))
                Await f.WriteAsync(farcHeader.Count + &H4C, fatData)
                Await f.WriteAsync(farcHeader.Count + &H4C + fatData.Length, fileData.ToArray())

                Await f.Save(provider)
            End Using
        End Function

        Public Async Function Save(provider As IIOProvider) As Task Implements ISavable.Save
            Await Save(Filename, provider)
        End Function

        Public Async Function IsOfType(file As GenericFile) As Task(Of Boolean) Implements IDetectableFileType.IsOfType
            Return file.Length > &H50 AndAlso
                (Await file.ReadAsync(0, 4)).SequenceEqual({&H46, &H41, &H52, &H43}) AndAlso
                (Await file.ReadInt32Async(&H20) = 5 OrElse Await file.ReadInt32Async(&H20) = 4)
        End Function

        Public Function GetDefaultExtension() As String Implements ISavableAs.GetDefaultExtension
            Return "*.bin"
        End Function

        Public Function GetSupportedExtensions() As IEnumerable(Of String) Implements ISavableAs.GetSupportedExtensions
            Return {"*.bin"}
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            InnerData?.Dispose()
            InnerData = Nothing

            Entries?.Clear()
            Entries = Nothing
        End Sub

#Region "IReportProgress Properties"
        Public Property Progress As Single Implements IReportProgress.Progress

        Public Property Message As String Implements IReportProgress.Message

        Public Property IsIndeterminate As Boolean Implements IReportProgress.IsIndeterminate

        Public Property IsCompleted As Boolean Implements IReportProgress.IsCompleted
#End Region

#Region "IIOProvider Implementation"
        Public Property WorkingDirectory As String Implements IIOProvider.WorkingDirectory

        Public Sub ResetWorkingDirectory() Implements IIOProvider.ResetWorkingDirectory
            WorkingDirectory = "/"
        End Sub
        Protected Function FixPath(filePath As String) As String
            Return filePath.TrimStart("/")
        End Function

        Public Function GetFileLength(filename As String) As Long Implements IIOProvider.GetFileLength
            Return GetFileEntry(FixPath(filename)).DataLength
        End Function

        Public Function FileExists(filename As String) As Boolean Implements IIOProvider.FileExists
            Return GetFileEntry(FixPath(filename)) IsNot Nothing
        End Function

        Public Function DirectoryExists(path As String) As Boolean Implements IIOProvider.DirectoryExists
            Return False
        End Function

        Public Sub CreateDirectory(path As String) Implements IIOProvider.CreateDirectory
            Throw New NotSupportedException()
        End Sub

        Public Function GetFiles(path As String, searchPattern As String, topDirectoryOnly As Boolean) As String() Implements IIOProvider.GetFiles
            Dim filter = New Regex(GetFileSearchRegex(searchPattern))
            Dim files = Entries.Values.Select(Function(entry)
                                                  If Not String.IsNullOrEmpty(entry.Filename) Then
                                                      Return "/" & entry.Filename
                                                  Else
                                                      Return "/" & entry.FilenameHash.Value.ToString("X")
                                                  End If
                                              End Function).
                        Where(Function(filename) filter.IsMatch(filename))
            Return files.ToArray()
        End Function

        Public Function GetDirectories(path As String, topDirectoryOnly As Boolean) As String() Implements IIOProvider.GetDirectories
            Return {}
        End Function

        Public Function ReadAllBytes(filename As String) As Byte() Implements IIOProvider.ReadAllBytes
            Return GetFileData(FixPath(filename))
        End Function

        Public Async Function ReadAllBytesAsync(filename As String) As Task(Of Byte())
            Return Await GetFileDataAsync(FixPath(filename))
        End Function

        Public Function ReadAllText(filename As String) As String Implements IIOProvider.ReadAllText
            Using f = OpenFileReadOnly(filename)
                Using reader As New StreamReader(f)
                    Return reader.ReadToEnd()
                End Using
            End Using
        End Function

        Public Sub WriteAllBytes(filename As String, data() As Byte) Implements IIOProvider.WriteAllBytes
            Dim entry = GetFileEntry(FixPath(filename))
            If entry IsNot Nothing Then
                entry.FileData = data
            Else
                entry = New FarcEntry

                Dim hex As UInteger
                If UInteger.TryParse(filename, NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, hex) Then
                    'Filename is unknown, use raw hash
                    entry.FilenameHash = hex
                Else
                    entry.Filename = FixPath(filename)
                End If

                entry.FileData = data
                Entries(entry.FilenameHash) = entry
            End If
        End Sub

        Public Sub WriteAllText(filename As String, data As String) Implements IIOProvider.WriteAllText
            WriteAllBytes(filename, Text.Encoding.Unicode.GetBytes(data))
        End Sub

        Public Sub CopyFile(sourceFilename As String, destinationFilename As String) Implements IIOProvider.CopyFile
            Dim source = ReadAllBytes(sourceFilename)
            If source IsNot Nothing Then
                WriteAllBytes(destinationFilename, source)
            End If
        End Sub

        Public Async Function CopyFileAsync(sourceFilename As String, destinationFilename As String) As Task
            Dim source = Await ReadAllBytesAsync(sourceFilename)
            If source IsNot Nothing Then
                WriteAllBytes(destinationFilename, source)
            End If
        End Function

        Public Sub DeleteFile(filename As String) Implements IIOProvider.DeleteFile
            Dim entry = GetFileEntry(FixPath(filename))
            Dim tries = 5
            While tries > 0 AndAlso Not Entries.TryRemove(entry.FilenameHash, entry)
                Thread.Sleep(250 + tries)
                tries -= 1
            End While
        End Sub

        Public Sub DeleteDirectory(path As String) Implements IIOProvider.DeleteDirectory
            Throw New NotSupportedException()
        End Sub

        Public Function GetTempFilename() As String Implements IIOProvider.GetTempFilename
            Throw New NotImplementedException()
        End Function

        Public Function GetTempDirectory() As String Implements IIOProvider.GetTempDirectory
            Throw New NotSupportedException()
        End Function

        Public Function OpenFile(filename As String) As Stream Implements IIOProvider.OpenFile
            Return New FarcFileStream(Me, FixPath(filename), True, True)
        End Function

        Public Function OpenFileReadOnly(filename As String) As Stream Implements IIOProvider.OpenFileReadOnly
            Return New FarcFileStream(Me, FixPath(filename), True, False)
        End Function

        Public Function OpenFileWriteOnly(filename As String) As Stream Implements IIOProvider.OpenFileWriteOnly
            Return New FarcFileStream(Me, FixPath(filename), False, True)
        End Function
#End Region

#Region "Child Classes"
        Public Class FarcEntry
            ''' <summary>
            ''' Gets or sets the filename, updating the filename hash on set
            ''' </summary>
            Public Property Filename As String
                Get
                    Return _filename
                End Get
                Set(value As String)
                    _filename = value
                    FilenameHash = PmdFunctions.Crc32Hash(value)
                End Set
            End Property
            Dim _filename As String

            Public Property FilenameHash As UInteger?

            Public ReadOnly Property FilenameOrHash As String
                Get
                    Return If(Filename,
                    If(FilenameHash?.ToString("X"),
                    MyBase.ToString))
                End Get
            End Property

            Public Property FileData As Byte()

            Friend Property DataEntry As FarcFat5.Entry

            Public ReadOnly Property DataLength As Integer
                Get
                    If FileData IsNot Nothing Then
                        Return FileData.Length
                    Else
                        Return DataEntry.DataLength
                    End If
                End Get
            End Property

            ''' <summary>
            ''' Sets the filename only if the given filename matches the existing filename hash
            ''' </summary>
            ''' <returns>A boolean indictating whether or not the set was successful</returns>
            Public Function TrySetFilename(filename As String) As Boolean
                If Not FilenameHash.HasValue Then
                    Me.Filename = filename
                    Return True
                Else
                    Dim hash = PmdFunctions.Crc32Hash(filename)
                    If hash = FilenameHash.Value Then
                        _filename = filename
                        Return True
                    Else
                        Return False
                    End If
                End If
            End Function

            Public Overrides Function ToString() As String
                Return FilenameOrHash
            End Function
        End Class

        Protected Class EntryMapping
            Public Property FileData As Byte()

            Public ReadOnly Property FileDataHashCode As Integer
                Get
                    If Not _fileDataHashCode.HasValue Then

                        _fileDataHashCode = CreateByteArrayHashCode(FileData)
                    End If
                    Return _fileDataHashCode.Value
                End Get
            End Property
            Dim _fileDataHashCode As Integer?

            Public Property PossibleFilenames As List(Of String)

            Public Property Filename As String
        End Class

        Public Class FarcFileStream
            Inherits Stream

            Public Sub New(farc As Farc, filename As String, canRead As Boolean, canWrite As Boolean)
                Me.Farc = farc
                Me.Filename = filename
                Me.CanRead = canRead
                Me.CanWrite = canWrite
            End Sub

            Protected Property Farc As Farc

            Protected Property Filename As String

            Protected ReadOnly Property UnderlyingArray As Byte()
                Get
                    Return Farc.GetFileData(Filename)
                End Get
            End Property

            Public Overrides ReadOnly Property CanRead As Boolean

            Public Overrides ReadOnly Property CanSeek As Boolean
                Get
                    Return True
                End Get
            End Property

            Public Overrides ReadOnly Property CanWrite As Boolean

            Public Overrides ReadOnly Property Length As Long
                Get
                    Return UnderlyingArray.Length
                End Get
            End Property

            Public Overrides Property Position As Long

            Public Overrides Sub Flush()
            End Sub

            Public Overrides Sub SetLength(value As Long)
                Farc.ResizeFileData(Filename, value)
            End Sub

            Public Overrides Sub Write(buffer() As Byte, offset As Integer, count As Integer)
                Array.Copy(buffer, Position, UnderlyingArray, offset, count)
            End Sub

            Public Overrides Function Read(buffer() As Byte, offset As Integer, count As Integer) As Integer
                count = Math.Min(count, UnderlyingArray.Length - Position)
                Array.Copy(UnderlyingArray, offset, buffer, Position, count)
                Return count
            End Function

            Public Overrides Function Seek(offset As Long, origin As SeekOrigin) As Long
                Select Case origin
                    Case SeekOrigin.Begin
                        Position = offset
                    Case SeekOrigin.Current
                        Position += offset
                    Case SeekOrigin.End
                        Position = Length - 1 - offset
                    Case Else
                        Throw New ArgumentException(NameOf(origin))
                End Select
                Return Position
            End Function
        End Class

#End Region

    End Class
End Namespace
