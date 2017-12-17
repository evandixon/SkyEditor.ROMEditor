Imports System.IO
Imports SkyEditor.ROMEditor.MysteryDungeon.PSMD

Class Application

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.

    Private Async Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup

        Try

            Dim provider = New SkyEditor.Core.IO.PhysicalIOProvider
            Dim farcFilename = "C:\Users\evanl\Desktop\SkyEditor\Test 2017-11-22\BaseRom\Raw Files\RomFS\image_2d.bin"
            Dim dbFilename = "C:\Users\evanl\Desktop\SkyEditor\Test 2017-11-22\BaseRom\Raw Files\RomFS\image_2d_database.bin"

            Dim dbFile = New DatabaseBin()
            Await dbFile.OpenFile(dbFilename, provider)

            Dim farcFile = New FarcF5()
            Await farcFile.OpenFile(farcFilename, provider)

            Await farcFile.Extract("image2dExtract", provider, dbFile.GenerateHashDictionary)


            For Each item In Directory.GetFiles("image2dExtract")
                Try
                    Using cte As New CteImage
                        Await cte.OpenFile(item, provider)
                        cte.ContainedImage.Save(item & ".png", System.Drawing.Imaging.ImageFormat.Png)
                    End Using
                Catch ex As Exception
                End Try
            Next
            Exit Sub
            SkyEditor.UI.WPF.StartupHelpers.EnableErrorDialog()
            Await SkyEditor.UI.WPF.StartupHelpers.RunWPFStartupSequence(New SkyEditor.UI.WPF.WPFCoreSkyEditorPlugin(New PluginDefinition))
        Catch ex As Exception
            Debugger.Break()
            Throw
        End Try
    End Sub

End Class
