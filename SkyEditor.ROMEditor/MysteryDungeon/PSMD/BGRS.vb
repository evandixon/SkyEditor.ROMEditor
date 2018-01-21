Imports SkyEditor.Core.IO

Namespace MysteryDungeon.PSMD
    Public Class BGRS
        Implements IOpenableFile

        Public Enum BgrsType As Integer
            Extension = 0
            Normal = 1
        End Enum

        Public Enum AnimationType As Integer
            Unknown = 0
            SkeletalAnimation = 1
            TextureAnimation = 2
            Extension = &H80000001
        End Enum

        Public Class ModelPart

            Public Sub New(rawData As Byte(), partName As String)
                Me.RawData = rawData
                Me.PartName = partName
            End Sub

            Public Property RawData As Byte()

            Public Property PartName As String
        End Class

        Public Class Animation

            Public Sub New(name As String, animationType As AnimationType)
                Me.Name = name
                Me.AnimationType = animationType
            End Sub

            Public Property Name As String

            Public Property AnimationType As Integer

            Public Overrides Function ToString() As String
                Return If(Name, MyBase.ToString())
            End Function
        End Class

        Public Property Magic As String

        Public Property ReferencedBchFileName As String

        Public Property Animations As New List(Of Animation)

        Public Property BgrsName As String

        Public Property Type As BgrsType

        Public Property Parts As New List(Of ModelPart)

        Public Async Function OpenFile(filename As String, provider As IIOProvider) As Task Implements IOpenableFile.OpenFile
            Using f As New GenericFile(filename, provider)
                Await OpenInternal(f)
            End Using
        End Function

        Public Async Function OpenFile(rawData As Byte()) As Task
            Using f As New GenericFile
                f.CreateFile(rawData)
                Await OpenInternal(f)
            End Using
        End Function

        Private Async Function OpenInternal(f As GenericFile) As Task
            Magic = Await f.ReadNullTerminatedStringAsync(0, Text.Encoding.ASCII)
            ReferencedBchFileName = (Await f.ReadStringAsync(&H8, &H40, Text.Encoding.ASCII)).TrimEnd(vbNullChar)
            Type = Await f.ReadInt32Async(&H48)

            Select Case Type
                Case BgrsType.Normal
                    Await OpenInternalNormal(f)
                Case BgrsType.Extension
                    Await OpenInternalExtended(f)
                Case Else
                    Throw New NotSupportedException("Unsupported BGRS type: " & Type.ToString())
            End Select
        End Function

        Private Async Function OpenInternalNormal(f As GenericFile) As Task
            BgrsName = (Await f.ReadStringAsync(&H58, &HC0, Text.Encoding.ASCII)).TrimEnd(vbNullChar)

            'Yes, the counts of these two sections are in a different order than the sections themselves
            Dim animationCount = Await f.ReadInt32Async(&H118)
            Dim partCount = Await f.ReadInt32Async(&H11C)

            For partIndex = &H140 To (&H80 * partCount) - 1 Step &H80
                Dim partName = Await f.ReadNullTerminatedStringAsync(partIndex + &H18, Text.Encoding.ASCII)
                Parts.Add(New ModelPart(Await f.ReadAsync(partIndex, &H80), partName))
            Next
            Await OpenInternalAnimations(f, &H140 + (&H80 * partCount) + &H18, animationCount)
        End Function

        Private Async Function OpenInternalExtended(f As GenericFile) As Task
            Dim animationCount = Await f.ReadInt32Async(&H4C)

            Await OpenInternalAnimations(f, &H58, animationCount)

            'Set BGRS name, inferred from the animation names. Animation names are in the form of bgrs_name__animation_name
            BgrsName = Animations.FirstOrDefault?.Name.Replace("__", "!").Split("!")(0) '! is just a temporary character that won't appear in these names
        End Function

        Private Async Function OpenInternalAnimations(f As GenericFile, animationIndex As Integer, animationCount As Integer) As Task
            For i = animationIndex To animationIndex + (&HC4 * animationCount) - 1 Step &HC4
                Dim animName = (Await f.ReadStringAsync(i, &HC0, Text.Encoding.ASCII)).TrimEnd(vbNullChar)
                Dim animType = Await f.ReadInt32Async(i + &HC0)
                Dim anim As New Animation(animName, animType)
                Animations.Add(anim)
            Next
        End Function
    End Class
End Namespace
