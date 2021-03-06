﻿Imports System.IO
Imports System.Text
Imports SkyEditor.Core.ConsoleCommands
Imports SkyEditor.Core.IO
Imports SkyEditor.IO.FileSystem
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD

Namespace Windows.ConsoleCommands
    Public Class PSMDExpTableCsv
        Inherits ConsoleCommand

        Public Sub New(FileSystem As IFileSystem)
            If FileSystem Is Nothing Then
                Throw New ArgumentNullException(NameOf(FileSystem))
            End If

            CurrentFileSystem = FileSystem
        End Sub

        Protected Property CurrentFileSystem As IFileSystem

        Public Overrides Async Function MainAsync(Arguments() As String) As Task
            If Arguments.Length > 0 Then
                If File.Exists(Arguments(0)) Then
                    Dim exp As New Experience
                    Await exp.OpenFile(Arguments(0), CurrentFileSystem)

                    For Each item In exp.Entries
                        Dim s As New StringBuilder
                        Dim hp = 0
                        Dim attack = 0
                        Dim spAttack = 0
                        Dim defense = 0
                        Dim spDefense = 0
                        Dim speed = 0
                        s.AppendLine("Level,Experience,HP,Attack,Sp. Attack,Defense,Sp. Defense,Speed")
                        For count = 0 To item.Value.Count - 1
                            Dim entry = item.Value(count)
                            hp += entry.AddedHP
                            attack += entry.AddedAttack
                            spAttack += entry.AddedSpAttack
                            defense += entry.AddedDefense
                            spDefense += entry.AddedSpDefense
                            speed += entry.AddedSpeed
                            s.AppendLine($"{count},{entry.Exp},{hp},{attack},{spAttack},{defense},{spDefense},{speed}")
                        Next
                        File.WriteAllText(Path.Combine(Path.GetDirectoryName(Arguments(0)), "Exp Table " & item.Key & ".csv"), s.ToString)
                    Next
                    Console.WriteLine("Done")
                Else
                    Console.WriteLine("File doesn't exist.")
                End If
            Else
                Console.WriteLine("Usage: PSMDExpTableCsv <Filename>")
            End If
        End Function
    End Class

End Namespace
