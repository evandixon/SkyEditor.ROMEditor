Imports SkyEditor.Core.IO

Namespace MysteryDungeon.GTI
    Public Class ActorDataInfoGti
        Implements IOpenableFile
        Implements IOnDisk
        Implements ISavableAs

        Public Event FileSaved As EventHandler Implements ISavable.FileSaved

        Public Property Filename As String Implements IOnDisk.Filename

        Public Property Entries As List(Of Entry)

        ''' <summary>
        ''' Gets the entry with the given name.
        ''' </summary>
        ''' <param name="name">Name of the entry to find.</param>
        Public Function GetEntryByName(name As String) As Entry
            Return (From e In Entries Where e.ActorName = name).FirstOrDefault
        End Function

        Public Async Function OpenFile(filename As String, provider As IIOProvider) As Task Implements IOpenableFile.OpenFile
            Entries = New List(Of Entry)
            Me.Filename = filename

            Using f As New GenericFile
                Await f.OpenFile(filename, provider)

                Dim subHeaderPointer = Await f.ReadInt32Async(4)
                Dim sir0RelativePointerOffset = Await f.ReadInt32Async(8)
                Dim numEntries = Await f.ReadInt32Async(subHeaderPointer)
                Dim dataOffset = Await f.ReadInt32Async(subHeaderPointer + 4)

                For i = 0 To numEntries - 1
                    Dim entryPointerOffset = dataOffset + i * 4
                    Dim entryPointer = Await f.ReadInt32Async(entryPointerOffset)

                    Dim e As New Entry()
                    e.ActorName = Await f.ReadNullTerminatedStringAsync(Await f.ReadInt32Async(entryPointer), Text.Encoding.ASCII)
                    e.PokemonID = Await f.ReadInt32Async(entryPointer + 4)
                    Entries.Add(e)
                Next

            End Using
        End Function

        Public Async Function Save(filename As String, provider As IIOProvider) As Task Implements ISavableAs.Save
            Using sir0 As New Sir0
                sir0.AutoAddSir0HeaderRelativePointers = True
                sir0.AutoAddSir0SubHeaderRelativePointers = True
                sir0.CreateFile()

                Dim pointerData As New List(Of Byte)
                Dim entryData As New List(Of Byte)
                Dim stringData As New List(Of Byte)
                For Each item In Entries
                    Dim stringPointer = &H10 + 8 + Entries.Count * 12 + stringData.Count
                    Dim entryPointer = &H10 + 8 + 4 * Entries.Count + entryData.Count

                    pointerData.AddRange(BitConverter.GetBytes(entryPointer))

                    entryData.AddRange(BitConverter.GetBytes(stringPointer))
                    entryData.AddRange(BitConverter.GetBytes(item.PokemonID))

                    stringData.AddRange(Text.Encoding.ASCII.GetBytes(item.ActorName))
                    stringData.Add(0)

                Next

                Dim concatenatedData As New List(Of Byte)(pointerData.Count + stringData.Count + entryData.Count)
                concatenatedData.AddRange(BitConverter.GetBytes(Entries.Count))
                concatenatedData.AddRange(BitConverter.GetBytes(&H18))
                concatenatedData.AddRange(pointerData)
                concatenatedData.AddRange(entryData)
                concatenatedData.AddRange(stringData)

                sir0.ContentHeader = concatenatedData.ToArray()

                'We have two more 4's than 8's
                For i = 0 To Entries.Count + 1
                    sir0.SubHeaderRelativePointers.Add(4) 'Each pointer to each data item
                Next
                For i = 0 To Entries.Count - 2 'The original file had one less 8 than entries
                    sir0.SubHeaderRelativePointers.Add(8) 'Each pointer to each data item
                Next

                Await sir0.Save(filename, provider)
            End Using
        End Function

        Public Async Function Save(provider As IIOProvider) As Task Implements ISavable.Save
            Await Save(Filename, provider)
        End Function

        Public Function GetDefaultExtension() As String Implements ISavableAs.GetDefaultExtension
            Return "*.bin"
        End Function

        Public Function GetSupportedExtensions() As IEnumerable(Of String) Implements ISavableAs.GetSupportedExtensions
            Return {"*.bin"}
        End Function

        Public Class Entry
            Public Property PokemonID As Integer
            Public Property ActorName As String
        End Class
    End Class
End Namespace
