Imports System.IO
Imports System.Text
Imports SkyEditor.Core.IO
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD

Module Module1

    Sub Main()
        MainAsync.Wait()
    End Sub

    Async Function MainAsync() As Task
        Dim provider As New PhysicalIOProvider
        Dim output As New StringBuilder

        Dim naFile As New FarcF5
        Await naFile.OpenFile("C:\Users\evanl\Desktop\Tests\psmd-us\BaseRom\Raw Files\RomFS\message_sp.bin", provider)

        Dim euFile As New FarcF5
        Await euFile.OpenFile("C:\Users\evanl\Desktop\Tests\psmd-2-28\BaseRom\Raw Files\RomFS\message_sp.bin", provider)

        output.Append("<html><head><meta charset=""UTF-8"" /></head><body>")

        output.Append("<h1>Language Comparison</h1>")

        output.Append("<style>td{border:1px solid}</style>")

        For Each item In naFile.GetFileDictionary
            Dim naFileEntry As New MessageBin
            naFileEntry.CreateFile(naFile.GetFileData(item.Value))

            Dim euFileEntry As New MessageBin
            euFileEntry.CreateFile(euFile.GetFileData(item.Value))

            output.Append($"<h2>{item.Value}</h2>")
            output.Append("<table>")
            output.Append("<tr><td>Hash</td><td>NA</td><td>EU</td></tr>")
            For Each naString In naFileEntry.Strings
                Dim euString = euFileEntry.Strings.Where(Function(x) x.Hash = naString.Hash).FirstOrDefault

                If naString?.Entry <> euString?.Entry Then
                    output.AppendLine($"<tr><td>{naString.HashSigned}</td><td>{naString.Entry}</td><td>{euString.Entry}</td></tr>")
                End If
            Next
            output.AppendLine("</table>")
        Next

        output.Append("</body>")

        File.WriteAllText("results.htm", output.ToString)
    End Function

End Module
