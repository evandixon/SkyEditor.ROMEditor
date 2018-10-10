using SkyEditor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyEditor.ROMEditor.BigTestsCSharp
{
    public class CorePlugin : CoreSkyEditorPlugin
    {
        public override string PluginName => "Specflow Integration Tests";

        public override string PluginAuthor => "";

        public override string Credits => "";

        public override bool IsPluginLoadingEnabled()
        {
            return false;
        }
    }
}
