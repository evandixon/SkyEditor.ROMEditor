﻿Imports SkyEditor.Core
Imports SkyEditor.ROMEditor

Namespace MysteryDungeon.PSMD
    Public Class MessageBinStringEntry
        Public Sub New()
            Me.Entry = ""
            Me.Unknown = 0
        End Sub

        Public Property OriginalIndex As Integer
        Public Property Pointer As Integer
        Public Property Entry As String
        Public Property Hash As UInteger
        Public Property HashSigned As Integer
            Get
                Return BitConverter.ToInt32(BitConverter.GetBytes(Hash), 0)
            End Get
            Set(value As Integer)
                Hash = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0)
            End Set
        End Property

        Public Property Unknown As UInteger

        Public Overrides Function ToString() As String
            Return BitConverter.ToInt32(BitConverter.GetBytes(Hash), 0).ToString & ": " & Entry
        End Function

        Public Function GetStringBytes() As Byte()
            Dim output As New List(Of Byte)
            Dim skip As Integer = 0
            For count = 0 To Entry.Length - 1
                If skip > 0 Then
                    skip -= 1
                Else
                    Dim item = Entry(count)
                    If Not item = vbCr Then
                        If item = "\"c AndAlso Entry.Length > count + 4 Then
                            Dim escapeString1 As String = Entry(count + 1) & Entry(count + 2)
                            Dim escapeString2 As String = Entry(count + 3) & Entry(count + 4)
                            If Utilities.Hex.IsHex(escapeString1) AndAlso Utilities.Hex.IsHex(escapeString2) Then
                                output.Add(Byte.Parse(escapeString2, Globalization.NumberStyles.HexNumber))
                                output.Add(Byte.Parse(escapeString1, Globalization.NumberStyles.HexNumber))
                                skip += 4
                            End If
                        Else
                            output.AddRange(Text.Encoding.Unicode.GetBytes(item))
                        End If
                    End If
                End If
            Next
            output.Add(0)
            output.Add(0)
            Return output.ToArray
        End Function


    End Class
End Namespace