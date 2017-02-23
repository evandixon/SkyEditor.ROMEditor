Imports System.Text
Imports SkyEditor.Core.IO

Namespace MysteryDungeon.Rescue
    Public Class SBin
        Implements IOpenableFile
        Implements ISavableAs
        Implements IOnDisk

        Public Event FileSaved As EventHandler Implements ISavable.FileSaved

        Public Sub New()
            Files = New Dictionary(Of String, Byte())
        End Sub

        Public Property Files As Dictionary(Of String, Byte())
        Public Property Filename As String Implements IOnDisk.Filename

        Public Function GetDefaultExtension() As String Implements ISavableAs.GetDefaultExtension
            Return ".sbin"
        End Function

        Public Function GetSupportedExtensions() As IEnumerable(Of String) Implements ISavableAs.GetSupportedExtensions
            Return {".sbin"}
        End Function

        Public Function OpenFile(filename As String, provider As IIOProvider) As Task Implements IOpenableFile.OpenFile
            Dim rawData = provider.ReadAllBytes(filename)
            Dim currentRange = rawData.Take(16).ToArray
            Files.Clear()

            While Not currentRange.All(Function(x) x = 0)
                'Read the current header
                Dim name As String = Encoding.ASCII.GetString(currentRange, 0, 8).TrimEnd(vbNullChar)
                Dim index As Integer = BitConverter.ToInt32(currentRange, 8)
                Dim length As Integer = BitConverter.ToInt32(currentRange, 12)

                'Read the file
                Files.Add(name, rawData.Skip(index).Take(length).ToArray)

                'Get the next file and loop until we hit a null entry
                currentRange = rawData.Skip(Files.Count * 16).Take(16).ToArray
            End While

            Return Task.FromResult(0)
        End Function

        Public Async Function Save(provider As IIOProvider) As Task Implements ISavable.Save
            Await Save(Filename, provider)
        End Function

        Public Function GetBytes() As Byte()
            Dim headerSection As New List(Of Byte)
            Dim dataSection As New List(Of Byte)

            Dim dataStart As Integer = (Files.Count + 1) * 16
            While Not dataStart Mod 64 = 0
                dataStart += 16
            End While

            For Each item In Files.OrderBy(Function(x) BitConverter.ToUInt64(Encoding.ASCII.GetBytes(x.Key.PadRight(8, vbNullChar)).Reverse.ToArray, 0))

                'Prepare the name
                Dim nameBytes As New List(Of Byte)
                nameBytes.AddRange(Encoding.ASCII.GetBytes(item.Key))

                'Ensure the name is at least 8 bytes long
                While nameBytes.Count < 8
                    nameBytes.Add(0)
                End While

                'Write no more than 8 bytes
                headerSection.AddRange(nameBytes.Take(8))

                'Write the index
                headerSection.AddRange(BitConverter.GetBytes(dataStart))

                'Write the length
                headerSection.AddRange(BitConverter.GetBytes(item.Value.Length))

                'Write the data
                dataSection.AddRange(item.Value)

                'Pad the data
                While dataSection.Count Mod 16 <> 0
                    dataSection.Add(&HFF)
                    dataStart += 1
                End While

                'Increment the index for next time
                dataStart += item.Value.Length
            Next

            For count = 0 To 15
                headerSection.Add(0)
            Next

            If headerSection.Count Mod 16 <> 0 Then
                Throw New Exception("Failed to get bytes.  Header section not divisible by 16")
            End If

            While headerSection.Count Mod 64 <> 0
                For count = 0 To 15
                    headerSection.Add(&HFF)
                Next
            End While

            Return headerSection.Concat(dataSection).ToArray
        End Function

        Public Function Save(filename As String, provider As IIOProvider) As Task Implements ISavableAs.Save
            provider.WriteAllBytes(filename, GetBytes)
            RaiseEvent FileSaved(Me, New EventArgs)
            Return Task.FromResult(0)
        End Function
    End Class
End Namespace
