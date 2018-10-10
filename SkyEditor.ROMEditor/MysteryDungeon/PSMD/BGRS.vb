Imports SkyEditor.Core.IO
Imports SkyEditor.Core.IO.PluginInfrastructure

Namespace MysteryDungeon.PSMD
    Public Class BGRS
        Implements IOpenableFile
        Implements IOnDisk
        Implements ISavableAs
        Implements IDetectableFileType

#Region "Child Classes"
        Public Class ModelPart

            Public Sub New(rawData As Byte(), partName As String)
                Me.RawData = rawData
                Me.PartName = partName
            End Sub

            Public Property RawData As Byte()

            Public Property PartName As String

            Public Function GetRawData() As Byte()
                Dim partNameBytes = Text.Encoding.ASCII.GetBytes(PartName)
                partNameBytes.CopyTo(RawData, &H18)
                RawData(&H18 + partNameBytes.Length) = 0
                Return RawData
            End Function
        End Class

        Public Class Animation

            Public Sub New(name As String, animationType As AnimationType)
                Me.Name = name
                Me.AnimationType = animationType
            End Sub

            ''' <summary>
            ''' The raw name of the animation. Ex. bgrs_name__animation_name
            ''' </summary>
            Public Property Name As String

            ''' <summary>
            ''' The name of the BGRS to which this animation belongs. If the raw name is bgrs_name__animation_name, this returns bgrs_name
            ''' </summary>
            Public ReadOnly Property BgrsName As String
                Get
                    Return Name.Replace("__", "!").Split("!")(0) '! is just a temporary character that won't appear in these names
                End Get
            End Property

            ''' <summary>
            ''' The name of the animation action. If the raw name is bgrs_name__animation_name, this returns animation_name
            ''' </summary>
            Public ReadOnly Property AnimationName As String
                Get
                    Return Name.Replace("__", "!").Split("!")(1) '! is just a temporary character that won't appear in these names
                End Get
            End Property

            Public Property AnimationType As Integer

            Public Function Clone() As Animation
                Return New Animation(Name, AnimationType)
            End Function

            Public Overrides Function ToString() As String
                Return If(Name, MyBase.ToString())
            End Function
        End Class
#End Region

        Public Enum BgrsType As Integer
            Extension = 0
            Normal = 1
        End Enum

        Public Enum AnimationType As Integer
            Unknown = 0
            SkeletalAnimation = 1
            MaterialAnimation = 2

            ''' <summary>
            ''' A skeletal animation that belongs to another BGRS file
            ''' </summary>
            Remote = &H8000000
        End Enum

        Public Event FileSaved As EventHandler Implements ISavable.FileSaved

        Public Property Magic As String

        Public Property ReferencedBchFileName As String

        Public Property Animations As New List(Of Animation)

        Public Property BgrsName As String

        Public Property Type As BgrsType

        Public Property Parts As New List(Of ModelPart)

        Public Property UnknownModelPartsFooter As Byte()

        Public Property Filename As String Implements IOnDisk.Filename

        Public Async Function OpenFile(filename As String, provider As IIOProvider) As Task Implements IOpenableFile.OpenFile
            Using f As New GenericFile(filename, provider)
                Await OpenInternal(f)
            End Using
            Me.Filename = filename
        End Function

        Public Async Function OpenFile(rawData As Byte()) As Task
            Using f As New GenericFile
                f.CreateFile(rawData)
                Await OpenInternal(f)
            End Using
        End Function

        Private Async Function OpenInternal(f As GenericFile) As Task
            Magic = Await f.ReadNullTerminatedStringAsync(0, Text.Encoding.ASCII)
            ReferencedBchFileName = (Await f.ReadNullTerminatedStringAsync(&H8, Text.Encoding.ASCII)).TrimEnd(vbNullChar) 'Max length: &H40
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
            BgrsName = (Await f.ReadNullTerminatedStringAsync(&H58, Text.Encoding.ASCII)) 'Max length: &HC0

            'Yes, the counts of these two sections are in a different order than the sections themselves
            Dim animationCount = Await f.ReadInt32Async(&H118)
            Dim partCount = Await f.ReadInt32Async(&H11C)

            For partIndex = &H140 To &H140 + (&H80 * partCount) - 1 Step &H80
                Dim partName = Await f.ReadNullTerminatedStringAsync(partIndex + &H18, Text.Encoding.ASCII)
                Parts.Add(New ModelPart(Await f.ReadAsync(partIndex, &H80), partName))
            Next
            UnknownModelPartsFooter = Await f.ReadAsync(&H140 + (&H80 * partCount), &H18)
            Await OpenInternalAnimations(f, &H140 + (&H80 * partCount) + &H18, animationCount)
        End Function

        Private Async Function OpenInternalExtended(f As GenericFile) As Task
            Dim animationCount = Await f.ReadInt32Async(&H4C)

            Await OpenInternalAnimations(f, &H58, animationCount)

            'Set BGRS name, inferred from the animation names. Animation names are in the form of bgrs_name__animation_name
            BgrsName = Animations.FirstOrDefault?.AnimationName
        End Function

        Private Async Function OpenInternalAnimations(f As GenericFile, animationIndex As Integer, animationCount As Integer) As Task
            For i = animationIndex To animationIndex + (&HC4 * animationCount) - 1 Step &HC4
                Dim animName = (Await f.ReadNullTerminatedStringAsync(i, Text.Encoding.ASCII)) 'Max length: &HC0
                Dim animType = Await f.ReadInt32Async(i + &HC0)
                Dim anim As New Animation(animName, animType)
                Animations.Add(anim)
            Next
        End Function

        Public Async Function IsOfType(file As GenericFile) As Task(Of Boolean) Implements IDetectableFileType.IsOfType
            Return file.Length >= 8 AndAlso Await file.ReadStringAsync(0, 7, Text.Encoding.ASCII) = "BGRS0.5"
        End Function

        Public Function GetRawData() As Byte()
            Dim rawData As New List(Of Byte)

            rawData.AddRange(Text.Encoding.ASCII.GetBytes("BGRS0.5"))
            rawData.Add(0)

            Dim bchNameBytes As Byte()
            If Type = BgrsType.Normal Then
                bchNameBytes = Text.Encoding.ASCII.GetBytes(ReferencedBchFileName)
            Else
                bchNameBytes = {}
            End If
            rawData.AddRange(bchNameBytes)
            rawData.AddRange(Enumerable.Repeat(Of Byte)(0, &H40 - bchNameBytes.Length))

            rawData.AddRange(BitConverter.GetBytes(Type))

            Select Case Type
                Case BgrsType.Normal
                    rawData.AddRange(Enumerable.Repeat(Of Byte)(0, &HC))

                    Dim bgrsNameBytes = Text.Encoding.ASCII.GetBytes(BgrsName)
                    rawData.AddRange(bgrsNameBytes)
                    rawData.AddRange(Enumerable.Repeat(Of Byte)(0, &HC0 - bgrsNameBytes.Length))

                    rawData.AddRange(BitConverter.GetBytes(Animations.Count))
                    rawData.AddRange(BitConverter.GetBytes(Parts.Count))

                    rawData.AddRange(Enumerable.Repeat(Of Byte)(0, &H20))
                    rawData.AddRange(GetRawData_ModelPartsSection())
                    rawData.AddRange(UnknownModelPartsFooter)

                    rawData.AddRange(GetRawData_AnimationSection)
                Case BgrsType.Extension
                    rawData.AddRange(BitConverter.GetBytes(Animations.Count))
                    rawData.AddRange(Enumerable.Repeat(Of Byte)(0, 8))
                    rawData.AddRange(GetRawData_AnimationSection)
                Case Else
                    Throw New NotSupportedException("Unsupported BGRS type: " & Type.ToString())
            End Select

            Return rawData.ToArray()
        End Function

        Private Function GetRawData_ModelPartsSection() As IEnumerable(Of Byte)
            Dim modelPartsSection As New List(Of Byte)(&H80 * Parts.Count)

            For Each item In Parts
                modelPartsSection.AddRange(item.GetRawData)
            Next

            Return modelPartsSection.ToArray()
        End Function

        Private Function GetRawData_AnimationSection() As IEnumerable(Of Byte)
            Dim animSection As New List(Of Byte)

            For Each item In Animations.OrderBy(Function(a) a.Name)
                Dim nameBytes = Text.Encoding.ASCII.GetBytes(item.Name)

                animSection.AddRange(nameBytes)
                animSection.AddRange(Enumerable.Repeat(Of Byte)(0, &HC0 - nameBytes.Length))

                animSection.AddRange(BitConverter.GetBytes(item.AnimationType))
            Next

            animSection.AddRange(Enumerable.Repeat(Of Byte)(0, &HC0))

            Return animSection
        End Function

        Public Function Save(filename As String, provider As IIOProvider) As Task Implements ISavableAs.Save
            provider.WriteAllBytes(filename, GetRawData())
            Return Task.CompletedTask
        End Function

        Public Async Function Save(provider As IIOProvider) As Task Implements ISavable.Save
            Await Save(Me.Filename, provider)
        End Function

        Public Function GetDefaultExtension() As String Implements ISavableAs.GetDefaultExtension
            Return "*.bgrs"
        End Function

        Public Function GetSupportedExtensions() As IEnumerable(Of String) Implements ISavableAs.GetSupportedExtensions
            Return {"*.bgrs"}
        End Function

    End Class
End Namespace
