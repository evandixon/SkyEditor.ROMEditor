using SkyEditor.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SkyEditor.ROMEditor.IntegrationTestsCSharp
{
    [Binding]
    public sealed class SpecflowFramework
    {
        private class CorePlugin : CoreSkyEditorPlugin
        {
            public override string PluginName => "Specflow Integration Tests";

            public override string PluginAuthor => "";

            public override string Credits => "";

            public override bool IsPluginLoadingEnabled()
            {
                return false;
            }
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            var corePlugin = new CorePlugin();
            var pluginManager = new PluginManager();
            await pluginManager.LoadCore(corePlugin);

            var applicationViewModel = new ApplicationViewModel(pluginManager);

            ScenarioContext.Current.Add("PluginManager", pluginManager);
            ScenarioContext.Current.Add("ApplicationViewModel", applicationViewModel);
            ScenarioContext.Current.Add("CleanupFiles", new List<string>());
        }

        [AfterScenario]
        public void AfterScenario()
        {
            ScenarioContext.Current.Get<ApplicationViewModel>("ApplicationViewModel").Dispose();
            ScenarioContext.Current.Get<PluginManager>("PluginManager").Dispose();
            foreach (var path in ScenarioContext.Current.Get<List<string>>("CleanupFiles"))
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                else if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
        }
    }
}
