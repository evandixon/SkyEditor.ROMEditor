Imports System.IO
Imports SkyEditor.Core
Imports SkyEditor.Core.Utilities
Imports SkyEditor.ROMEditor.ProcessManagement

Namespace Windows
    Public Module DSIconTool
        Async Function ExtractIcon(RomPath As String, OutputPath As String) As Task
            Dim romDirectory As String = Core.Utilities.EnvironmentPaths.GetResourceDirectory
            'Dim extractTask = SkyEditorBase.PluginHelper.RunProgram(IO.Path.Combine(romDirectory, "DSIconTool.exe"),
            '                                      String.Format("{0} {1}",
            '                                                    RomPath, OutputPath))
            'extractTask.Wait()
            Await ConsoleApp.RunProgram(Path.Combine(romDirectory, "DSIconTool.exe"),
                                                  String.Format("{0} {1}",
                                                                RomPath, OutputPath), False)
        End Function
    End Module
End Namespace
