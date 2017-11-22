Imports System.IO
Imports SkyEditor.Core
Imports SkyEditor.Core.Utilities

Namespace Windows
    Public Module DSIconTool
        Async Function ExtractIcon(RomPath As String, OutputPath As String) As Task
            Dim romDirectory As String = EnvironmentPaths.GetResourceDirectory
            'Dim extractTask = SkyEditorBase.PluginHelper.RunProgram(IO.Path.Combine(romDirectory, "DSIconTool.exe"),
            '                                      String.Format("{0} {1}",
            '                                                    RomPath, OutputPath))
            'extractTask.Wait()
            Await ConsoleApp.RunProgram(Path.Combine(romDirectory, "DSIconTool.exe"),
                                                  String.Format("{0} {1}",
                                                                RomPath, OutputPath))
        End Function
    End Module
End Namespace
