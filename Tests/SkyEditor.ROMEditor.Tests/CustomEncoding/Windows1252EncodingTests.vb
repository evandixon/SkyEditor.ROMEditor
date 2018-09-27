Imports System.Linq
Imports System.Text
Imports SkyEditor.ROMEditor.CustomEncoding

Namespace CustomEncoding
    <TestClass>
    Public Class EncodingTests
        Private myEncoding As Windows1252Encoding
        Private referenceEncoding As Encoding

        <TestInitialize>
        Public Sub Init()
            myEncoding = New Windows1252Encoding()
            referenceEncoding = Encoding.GetEncoding(1252)
        End Sub

        Private Shared Function GetByteRange(ByVal range As Integer) As Byte()
            Dim data As Byte() = New Byte(range - 1) {}

            For i As Integer = 0 To range - 1
                data(i) = CByte(i)
            Next

            Return data
        End Function

        Private Function GetCharacters(ByVal range As Integer) As Char()
            Dim bytes = GetByteRange(range)
            Return referenceEncoding.GetChars(bytes)
        End Function

        <TestMethod>
        Public Sub Decoding_Bytes_Should_Return_Same_Characters()
            Dim customChars = myEncoding.GetChars(GetByteRange(Windows1252Encoding.CharacterCount))
            Dim winChars = referenceEncoding.GetChars(GetByteRange(Windows1252Encoding.CharacterCount))
            Assert.IsTrue(customChars.SequenceEqual(winChars))
        End Sub

        <TestMethod>
        Public Sub Decoding_Bytes_Should_Return_Same_Strings()
            Dim customString = myEncoding.GetString(GetByteRange(Windows1252Encoding.CharacterCount))
            Dim winString = referenceEncoding.GetString(GetByteRange(Windows1252Encoding.CharacterCount))
            Assert.AreEqual(customString, winString)
        End Sub

        <TestMethod>
        Public Sub Encoding_Characters_Should_Return_Same_Bytes()
            Dim customBytes = myEncoding.GetBytes(GetCharacters(Windows1252Encoding.CharacterCount))
            Dim winBytes = referenceEncoding.GetBytes(GetCharacters(Windows1252Encoding.CharacterCount))
            Assert.IsTrue(winBytes.SequenceEqual(customBytes))
        End Sub

        <TestMethod>
        Public Sub Encoding_Strings_Should_Return_Same_Bytes()
            Dim s = New String(GetCharacters(Windows1252Encoding.CharacterCount))
            Dim customBytes = myEncoding.GetBytes(s)
            Dim winBytes = referenceEncoding.GetBytes(s)
            Assert.IsTrue(winBytes.SequenceEqual(customBytes))
        End Sub

        <TestMethod>
        Public Sub Roundtrip_Should_Work()
            Dim original As String = New String(GetCharacters(Windows1252Encoding.CharacterCount))
            Dim bytes = myEncoding.GetBytes(original)
            Dim chars = myEncoding.GetChars(bytes)
            bytes = myEncoding.GetBytes(chars)
            Dim s = myEncoding.GetString(bytes)
            Assert.AreEqual(original, s)
            bytes = referenceEncoding.GetBytes(s)
            chars = referenceEncoding.GetChars(bytes)
            bytes = referenceEncoding.GetBytes(chars)
            s = referenceEncoding.GetString(bytes)
            Assert.AreEqual(original, s)
        End Sub

        <TestMethod>
        <ExpectedException(GetType(EncoderFallbackException))>
        Public Sub Setting_Fallback_Character_That_Is_Not_Supported_Itself_Should_Fail()
            Dim c As Char = ChrW(34355)
            myEncoding.FallbackCharacter = c
        End Sub

        <TestMethod>
        Public Sub Setting_The_Fallback_Character_Should_Update_Fallback_Byte_For_Decoding()
            Dim c As Char = "x"c
            Dim b As Byte = myEncoding.GetBytes("x").Single()
            Assert.AreNotEqual(b, myEncoding.FallbackByte)
            myEncoding.FallbackCharacter = c
            Assert.AreEqual(b, myEncoding.FallbackByte)
        End Sub

        <TestMethod>
        Public Sub Submitting_Unsupported_Char_Should_Use_Fallback_Character_If_Configured()
            Dim c As Char = ChrW(34355)
            myEncoding.FallbackCharacter = "x"c
            Assert.AreEqual("x", myEncoding.GetString(myEncoding.GetBytes(c.ToString())))
            myEncoding.FallbackCharacter = "y"c
            Assert.AreEqual("y", myEncoding.GetString(myEncoding.GetBytes(c.ToString())))
        End Sub

        <TestMethod>
        <ExpectedException(GetType(EncoderFallbackException))>
        Public Sub Submitting_Unsupported_Char_Should_Throw_Exception_If_No_Fallback_Char_Was_Configured()
            Dim c As Char = ChrW(34355)
            myEncoding.FallbackCharacter = Nothing
            myEncoding.GetBytes(c.ToString())
        End Sub

        '<TestMethod>
        'Public Sub Submitting_Unsupported_Byte_Should_Use_Fallback_Character_If_Configured()
        '    If Windows1252Encoding.CharacterCount = 256 Then
        '        Assert.Inconclusive("The encoding under test supports 256 characters, which means that every possible byte value is covered for.")
        '    End If

        '    Dim byteValue As Byte = CByte(Windows1252Encoding.CharacterCount)
        '    myEncoding.FallbackCharacter = "x"c
        '    Assert.AreEqual("x", myEncoding.GetString({byteValue}))
        '    myEncoding.FallbackCharacter = "y"c
        '    Assert.AreEqual("y", myEncoding.GetString({byteValue}))
        'End Sub

        '<TestMethod>
        '<ExpectedException(GetType(EncoderFallbackException))>
        'Public Sub Submitting_Unsupported_Byte_Should_Throw_Exception_If_No_Fallback_Char_Was_Configured()
        '    If Windows1252Encoding.CharacterCount = 256 Then
        '        Assert.Inconclusive("The encoding under test supports 256 characters, which means that every possible byte value is covered for.")
        '    End If

        '    Dim byteValue As Byte = CByte(Windows1252Encoding.CharacterCount)
        '    myEncoding.FallbackCharacter = Nothing
        '    myEncoding.GetString({byteValue})
        'End Sub
    End Class
End Namespace
