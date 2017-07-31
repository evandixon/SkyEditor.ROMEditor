Imports System.IO
Imports SkyEditor.Core
Imports SkyEditor.Core.Processes
Imports SkyEditor.Core.Utilities

Namespace Windows
    Public Module DSIconTool
        Sub ExtractIcon(RomPath As String, OutputPath As String)
            Dim romDirectory As String = EnvironmentPaths.GetResourceDirectory
            'Dim extractTask = SkyEditorBase.PluginHelper.RunProgram(IO.Path.Combine(romDirectory, "DSIconTool.exe"),
            '                                      String.Format("{0} {1}",
            '                                                    RomPath, OutputPath))
            'extractTask.Wait()
            ConsoleApp.RunProgram(Path.Combine(romDirectory, "DSIconTool.exe"),
                                                  String.Format("{0} {1}",
                                                                RomPath, OutputPath))
        End Sub
    End Module
End Namespace
