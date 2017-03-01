Imports System.IO
Imports System.Security.Cryptography
Imports SkyEditor.Core
Imports SkyEditor.Core.ConsoleCommands
Imports SkyEditor.Core.Utilities
Imports SkyEditor.Core.Windows
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD

Namespace Windows.ConsoleCommands
    Public Class MessageFarcFilenameAnalysis
        Inherits ConsoleCommand

        Public Overrides Async Function MainAsync(Arguments() As String) As Task
            If Arguments.Length >= 2 Then
                If File.Exists(Arguments(0)) Then
                    If Directory.Exists(Arguments(1)) Then
                        Dim tmpDirectory = Path.Combine(Environment.CurrentDirectory, "tmp", "farcAnalysis")
                        Await FileSystem.EnsureDirectoryExistsEmpty(tmpDirectory, CurrentApplicationViewModel.CurrentIOProvider)

                        'Extract the farc
                        Dim f As New FarcF5
                        Await f.OpenFile(Arguments(0), CurrentApplicationViewModel.CurrentIOProvider)
                        Await f.Extract(tmpDirectory, CurrentApplicationViewModel.CurrentIOProvider, False)


                        Dim farcFiles = Directory.GetFiles(tmpDirectory)
                        Dim actualFiles = Directory.GetFiles(Arguments(1))

                        Dim farcHashes As New Dictionary(Of UInteger, Byte())
                        Dim actualHashes As New Dictionary(Of String, Byte())

                        'Calculate hashes for the files
                        Using hash = MD5.Create
                            For Each item In farcFiles
                                Dim filename = Path.GetFileNameWithoutExtension(item)
                                If IsNumeric(filename) Then
                                    farcHashes.Add(f.Header.FileData(CUInt(filename)).FilenamePointer, hash.ComputeHash(File.ReadAllBytes(item)))
                                End If
                            Next
                            For Each item In actualFiles
                                Dim filename = Path.GetFileNameWithoutExtension(item)
                                actualHashes.Add(filename, hash.ComputeHash(File.ReadAllBytes(item)))
                            Next
                        End Using
                        f.Dispose()
                        Dim matches As New Dictionary(Of UInteger, String)

                        'Compare the hashes, to find which file index matches which filename goes with which file number
                        For Each farcHash In (From kv In farcHashes Order By kv.Key Ascending)
                            For Each actualHash In actualHashes
                                If farcHash.Value.SequenceEqual(actualHash.Value) Then
                                    matches.Add(farcHash.Key, actualHash.Key)
                                    Console.WriteLine($"Matched {farcHash.Key} to {actualHash.Key}")
                                    Exit For
                                End If
                            Next
                        Next

                        'Save the matches to the appropriate file
                        Dim destFile = EnvironmentPaths.GetResourceName(Path.Combine("farc", Path.GetFileNameWithoutExtension(Arguments(0)) & ".txt"))
                        Dim output As New Text.StringBuilder
                        For Each item In matches
                            output.Append(item.Key.ToString)
                            output.Append("=")
                            output.Append(item.Value)
                            output.Append(vbCrLf)
                        Next
                        File.WriteAllText(destFile, output.ToString)
                        Console.WriteLine("All matches saved to " & destFile)
                    Else
                        Console.WriteLine($"Unable to find directory ""{Arguments(1)}"".")
                    End If
                Else
                    Console.WriteLine($"Unable to find file ""{Arguments(0)}"".")
                End If
            Else
                Console.WriteLine("Usage: MessageFarcFilenameAnalysis <farcFile> <extractedDirectory>")
            End If
        End Function
    End Class

End Namespace
