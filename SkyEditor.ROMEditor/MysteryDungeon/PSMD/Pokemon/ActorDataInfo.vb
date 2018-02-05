Imports System.Text
Imports SkyEditor.Core.IO

Namespace MysteryDungeon.PSMD.Pokemon
    ''' <summary>
    ''' Models the pokemon_actor_data_info.bin file in PSMD ROMs.
    ''' </summary>
    Public Class ActorDataInfo
        Implements IOpenableFile
        Implements ISavableAs
        Implements IOnDisk

#Region "Child Classes"
        Public Class ActorDataInfoEntry
            Public Sub New()
                Dim tmp(&H58 - 1) As Byte
                Data = tmp
            End Sub

            Public Sub New(RawData As Byte())
                Data = RawData

                Dim e As New ASCIIEncoding
                Name = e.GetString(RawData, 0, &H10).Trim(vbNullChar) 'Todo: verify this is really 16 bytes
                PokemonID = BitConverter.ToInt16(RawData, &H28)
            End Sub

            Private Property Data As Byte()

            Public Property Name As String

            Public Property PokemonID As Int16

            Public Function GetBytes() As Byte()
                Dim pid = BitConverter.GetBytes(PokemonID)

                For count = 0 To 1
                    '(Optional) Todo: save Pokemon name
                    Data(&H28 + count) = pid(count)
                Next
                Return Data
            End Function
            Public Overrides Function ToString() As String
                Return $"{PokemonID.ToString}: {Name}"
            End Function

        End Class
#End Region

        Public Sub New()
            MyBase.New
            Entries = New List(Of ActorDataInfoEntry)
        End Sub

        Public Event FileSaved As EventHandler Implements ISavable.FileSaved

        Public Property Entries As List(Of ActorDataInfoEntry)

        Private Property Footer As Byte()

        Public Property Filename As String Implements IOnDisk.Filename

        ''' <summary>
        ''' Gets the entry with the given name.
        ''' </summary>
        ''' <param name="name">Name of the entry to find.</param>
        ''' <returns>The entry with a <see cref="ActorDataInfoEntry.Name"/> equal to <paramref name="name"/>, or null if it does not exist.</returns>
        Public Function GetEntryByName(name As String) As ActorDataInfoEntry
            Return (From e In Entries Where e.Name = name).FirstOrDefault
        End Function

#Region "IO"
        Public Async Function OpenFile(Filename As String, Provider As IIOProvider) As Task Implements IOpenableFile.OpenFile
            Me.Filename = Filename
            Using f As New GenericFile(Filename, Provider)
                Dim numEntries = Math.Floor(f.Length / &H58)

                For count = 0 To numEntries - 1
                    Entries.Add(New ActorDataInfoEntry(Await f.ReadAsync(&H58 * count, &H58)))
                Next

                Footer = Await f.ReadAsync(f.Length - &H40, &H40)
            End Using
        End Function

        Public Function Save(Destination As String, provider As IIOProvider) As Task Implements ISavableAs.Save
            Me.Filename = Destination
            Dim dataBuffer As New List(Of Byte)(&H58 * Entries.Count)
            For Each item In Entries
                dataBuffer.AddRange(item.GetBytes)
            Next
            dataBuffer.AddRange(Footer)
            provider.WriteAllBytes(Destination, dataBuffer.ToArray)
            RaiseEvent FileSaved(Me, New EventArgs)
            Return Task.FromResult(0)
        End Function

        Public Async Function Save(provider As IIOProvider) As Task Implements ISavable.Save
            Await Save(Filename, provider)
        End Function

        Public Function GetDefaultExtension() As String Implements ISavableAs.GetDefaultExtension
            Return ".bin"
        End Function

        Public Function GetSupportedExtensions() As IEnumerable(Of String) Implements ISavableAs.GetSupportedExtensions
            Return {".bin"}
        End Function
#End Region

    End Class
End Namespace

