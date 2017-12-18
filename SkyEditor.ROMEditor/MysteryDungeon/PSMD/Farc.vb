Imports System.Text
Imports System.Text.RegularExpressions
Imports Force.Crc32
Imports SkyEditor.Core.IO

Namespace MysteryDungeon.PSMD
    Public Class Farc
        Implements IOpenableFile
        Implements IOnDisk
        Implements ISavableAs
        Implements IDetectableFileType
        Implements IDisposable
        Implements IIOProvider

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

        Private ReadOnly Crc32 As Crc32Algorithm = New Crc32Algorithm

        Protected Class Entry
            ''' <summary>
            ''' Gets or sets the filename, updating the filename hash on set
            ''' </summary>
            Public Property Filename As String
                Get
                    Return _filename
                End Get
                Set(value As String)
                    _filename = value
                    FilenameHash = BitConverter.ToUInt32(Crc32.ComputeHash(Text.Encoding.Unicode.GetBytes(value)).Reverse().ToArray(), 0)
                End Set
            End Property
            Dim _filename As String

            Public Property FilenameHash As UInteger?
            Public Property FileData As Byte()
            Friend Property DataEntry As FarcFat5.Entry
            Friend Property Crc32 As Crc32Algorithm

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
                    Dim hash = BitConverter.ToUInt32(Crc32.ComputeHash(Text.Encoding.Unicode.GetBytes(filename)).Reverse().ToArray(), 0)
                    If hash = FilenameHash.Value Then
                        Me.Filename = filename
                        Return True
                    Else
                        Return False
                    End If
                End If
            End Function
        End Class

        Public Sub New()
            ResetWorkingDirectory()
        End Sub

        Public Event FileSaved As EventHandler Implements ISavable.FileSaved

        Protected Property InnerData As GenericFile
        Protected Property DataOffset As Integer
        Protected Property UnknownHeaderData As Byte()
        Protected Property Sir0Type As Integer

        Public Property PreLoadFiles As Boolean = False
        Public Property EnableInMemoryLoad As Boolean = False
        Public Property Filename As String Implements IOnDisk.Filename
        Private Property Entries As List(Of Entry)

        Public Async Function OpenFile(filename As String, provider As IIOProvider) As Task Implements IOpenableFile.OpenFile
            Entries = New List(Of Entry)
            Dim f As New GenericFile
            f.EnableInMemoryLoad = Me.EnableInMemoryLoad
            Await f.OpenFile(filename, provider)

            UnknownHeaderData = Await f.ReadAsync(4, &H1C)
            Sir0Type = Await f.ReadInt32Async(&H20)
            Dim sir0Offset = Await f.ReadInt32Async(&H24)
            Dim sir0Length = Await f.ReadInt32Async(&H28)
            DataOffset = Await f.ReadInt32Async(&H2C)
            Dim dataLength = Await f.ReadInt32Async(&H30)

            If Sir0Type <> 5 Then
                Throw New NotSupportedException("Only FARC v5 is supported for the time being")
            End If

            Dim header = New FarcFat5
            Await header.OpenFile(Await f.ReadAsync(sir0Offset, sir0Length))

            For Each item In header.Entries
                Dim fileEntry As New Entry
                fileEntry.Crc32 = Crc32
                If item.IsFilenameSet Then
                    fileEntry.Filename = item.Filename
                Else
                    fileEntry.FilenameHash = item.FilenameHash
                End If
                fileEntry.DataEntry = item

                If PreLoadFiles Then
                    Await GetFileData(fileEntry)
                Else
                    'Don't load the file data yet, to save time and resources
                End If

                Entries.Add(fileEntry)
            Next

            Me.InnerData = f
            Me.Filename = filename
        End Function

        Protected Function HashFilename(filename As String) As UInteger
            Return BitConverter.ToUInt32(Crc32.ComputeHash(Text.Encoding.Unicode.GetBytes(filename)).Reverse().ToArray(), 0)
        End Function

        Protected Async Function GetFileData(entry As Entry) As Task(Of Byte())
            If entry.FileData Is Nothing Then
                entry.FileData = Await InnerData.ReadAsync(DataOffset + entry.DataEntry.DataOffset, entry.DataEntry.DataLength).ConfigureAwait(False)
            End If
            Return entry.FileData
        End Function

        Protected Function GetFileEntry(filename As String) As Entry
            Dim hash = HashFilename(filename)
            Dim entry = Entries.FirstOrDefault(Function(x) x.FilenameHash = hash)
            If entry IsNot Nothing AndAlso String.IsNullOrEmpty(entry.Filename) Then
                entry.Filename = filename
            End If
            Return entry
        End Function

        Public Async Function GetFileData(filename As String) As Task(Of Byte())
            Dim entry = GetFileEntry(filename)
            If entry IsNot Nothing Then
                Return Await GetFileData(entry).ConfigureAwait(False)
            Else
                Return Nothing
            End If
        End Function

        Public Async Function Save(filename As String, provider As IIOProvider) As Task Implements ISavableAs.Save
            Using f As New GenericFile
                f.CreateFile({})

                Await f.Save(filename, provider)
            End Using
        End Function

        Public Async Function Save(provider As IIOProvider) As Task Implements ISavable.Save
            Await Save(Filename, provider)
        End Function

        Public Async Function IsOfType(file As GenericFile) As Task(Of Boolean) Implements IDetectableFileType.IsOfType
            Return file.Length > &H50 AndAlso
                (Await file.ReadAsync(0, 4)).SequenceEqual({&H46, &H41, &H52, &H43}) AndAlso
                (Await file.ReadInt32Async(&H20) = 5)
        End Function

        Public Function GetDefaultExtension() As String Implements ISavableAs.GetDefaultExtension
            Return "*.bin"
        End Function

        Public Function GetSupportedExtensions() As IEnumerable(Of String) Implements ISavableAs.GetSupportedExtensions
            Return {"*.bin"}
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            InnerData?.Dispose()
            Crc32?.Dispose()
        End Sub

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
            Dim files = Entries.Select(Function(entry)
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
            Throw New NotImplementedException
            Return GetFileData(FixPath(filename)).ConfigureAwait(False).GetAwaiter.GetResult
        End Function

        Public Function ReadAllText(filename As String) As String Implements IIOProvider.ReadAllText
            Throw New NotImplementedException()
        End Function

        Public Sub WriteAllBytes(filename As String, data() As Byte) Implements IIOProvider.WriteAllBytes
            Dim entry = GetFileEntry(FixPath(filename))
            If entry IsNot Nothing Then
                entry.FileData = data
            Else
                entry = New Entry
                entry.Filename = FixPath(filename)
                entry.FileData = data
                Entries.Add(entry)
            End If
        End Sub

        Public Sub WriteAllText(filename As String, data As String) Implements IIOProvider.WriteAllText
            Throw New NotImplementedException()
        End Sub

        Public Sub CopyFile(sourceFilename As String, destinationFilename As String) Implements IIOProvider.CopyFile
            WriteAllBytes(destinationFilename, ReadAllBytes(sourceFilename))
        End Sub

        Public Sub DeleteFile(filename As String) Implements IIOProvider.DeleteFile
            Entries.Remove(GetFileEntry(FixPath(filename)))
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
            Throw New NotImplementedException()
        End Function

        Public Function OpenFileReadOnly(filename As String) As Stream Implements IIOProvider.OpenFileReadOnly
            Throw New NotImplementedException()
        End Function

        Public Function OpenFileWriteOnly(filename As String) As Stream Implements IIOProvider.OpenFileWriteOnly
            Throw New NotImplementedException()
        End Function
#End Region
    End Class
End Namespace
