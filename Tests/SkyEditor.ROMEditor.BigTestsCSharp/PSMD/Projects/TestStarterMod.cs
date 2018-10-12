using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkyEditor.Core;
using SkyEditor.Core.Projects;
using SkyEditor.ROMEditor.MysteryDungeon.PSMD.Projects;
using SkyEditor.ROMEditor.ProcessManagement;
using SkyEditor.ROMEditor.Projects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyEditor.ROMEditor.BigTestsCSharp.PSMD.Projects
{
    [TestClass]
    public class TestStarterMod
    {
        private PluginManager PluginManager { get; set; }
        private ApplicationViewModel ApplicationViewModel { get; set; }
        private List<string> CleanupFiles { get; set; }

        [TestInitialize]
        public async Task BeforeScenario()
        {
            var corePlugin = new CorePlugin();

            PluginManager = new PluginManager();
            await PluginManager.LoadCore(corePlugin);

            ApplicationViewModel = new ApplicationViewModel(PluginManager);

            this.CleanupFiles = new List<string>();
        }

        [TestCleanup]
        public void AfterScenario()
        {
            ApplicationViewModel.Dispose();
            PluginManager.Dispose();
            foreach (var path in CleanupFiles)
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

        #region Steps
        private async Task GivenIHaveADSModSolution()
        {
            var pluginManager = PluginManager;
            var applicationViewModel = ApplicationViewModel;

            var solutionBasePath = Path.Combine(Environment.CurrentDirectory, "Projects");
            CleanupFiles.Add(solutionBasePath);

            var solution = await ProjectBase.CreateProject<DSModSolution>(solutionBasePath, "psmd-mod-solution", pluginManager);

            await solution.Initialize();

            var allProjects = solution.GetAllProjects().ToList();
            Assert.AreEqual(2, allProjects.Count, "Incorrect number of initial projects. Expecting a BaseROM project and a ModPack project");
            Assert.IsTrue(allProjects.Any(p => p is BaseRomProject), "Failed to find a BaseROM project in the solution");
            Assert.IsTrue(allProjects.Any(p => p is DSModPackProject), "Failed to find a ModPack project in the solution");

            applicationViewModel.CurrentSolution = solution;
        }

        private string GetRomFilename(string romName)
        {
            var filename = Path.Combine("TestRoms", romName);
            if (File.Exists(filename))
            {
                return Path.Combine(Environment.CurrentDirectory, filename);
            }
            else
            {
                Assert.Inconclusive("Could not find file: " + filename);
                throw new FileNotFoundException();
            }
        }

        private async Task IInitializeTheSolutionWithAPSMDUSROM(string romName)
        {
            var pluginManager = PluginManager;
            var applicationViewModel = ApplicationViewModel;

            var solution = applicationViewModel.CurrentSolution;
            if (solution == null)
            {
                Assert.Fail("A DS Mod Solution must already have been created");
            }

            Assert.IsTrue(solution.RequiresInitializationWizard, "The DS Mod Solution should require initialization");
            var wizard = solution.GetInitializationWizard() as DsModSolutionInitializationWizard;
            if (wizard == null)
            {
                Assert.Fail("The initialization wizard is either null or not of type DsModSolutionInitializationWizard");
            }

            wizard.GoForward(); // Skip the initialization step
            Assert.IsInstanceOfType(wizard.CurrentStep, typeof(DsModSolutionInitializationWizard.BaseRomStep), "The initialization wizard is not on the BaseRom step");

            var baseRomStep = wizard.CurrentStep as DsModSolutionInitializationWizard.BaseRomStep;
            baseRomStep.RomFilename = GetRomFilename(romName);
            await baseRomStep.ExtractRom();

            Assert.IsTrue(wizard.IsComplete, "Wizard should be complete after extracting ROM");
        }

        private async Task GivenTheSolutionHasAPsmdStarterModProject()
        {
            await ApplicationViewModel.CurrentSolution.AddNewProject("/", "starters", typeof(PsmdStarterMod), PluginManager);
        }

        private void TheModpackProjectWillOutputADecryptedROM()
        {
            var modpackProject = ApplicationViewModel.CurrentSolution.GetAllProjects().FirstOrDefault(p => p is DSModPackProject) as DSModPackProject;
            if (modpackProject == null)
            {
                Assert.Fail("Failed to find a DSModPackProject");
            }

            modpackProject.OutputDec3DSFile = true;

            modpackProject.OutputEnc3DSFile = false;
            modpackProject.OutputCIAFile = false;
            modpackProject.OutputHans = false;
            modpackProject.OutputLuma = false;
        }

        private async Task WhenIBuildTheProject()
        {
            Assert.IsTrue(ApplicationViewModel.CurrentSolution.CanBuild, "Solution should be able to build");
            await ApplicationViewModel.CurrentSolution.Build();
        }

        private async Task WhenIUnpackTheResultingROM()
        {
            var modpackProject = ApplicationViewModel.CurrentSolution.GetAllProjects().FirstOrDefault(p => p is DSModPackProject) as DSModPackProject;
            if (modpackProject == null)
            {
                Assert.Fail("Failed to find a DSModPackProject");
            }

            var outputDirectory = modpackProject.GetOutputDir();
            var outputFilename = Path.Combine(outputDirectory, "PatchedRom.3ds");
            Assert.IsTrue(File.Exists(outputFilename), "Failed to find output file: " + outputFilename);

            var extractedDirectory = Path.Combine(Environment.CurrentDirectory, "extracted-rom");
            CleanupFiles.Add(extractedDirectory);

            using (var converter = new DotNet3dsToolkit.Converter())
            {
                await converter.ExtractCCI(outputFilename, extractedDirectory);
            }
        }

        private async Task ThePersonalityTestShouldHaveBeenProperlyPatched()
        {
            var baseromProject = ApplicationViewModel.CurrentSolution.GetAllProjects().FirstOrDefault(p => p is BaseRomProject) as BaseRomProject;
            if (baseromProject == null)
            {
                Assert.Fail("Failed to find a BaseRomProject");
            }

            var originalScript = Path.Combine(baseromProject.GetRawFilesDir(), "RomFS", "script", "event", "other", "seikakushindan", "seikakushindan.lua");
            if (!File.Exists(originalScript))
            {
                Assert.Inconclusive("Failed to find original personality test script: " + originalScript);
            }

            var modifiedScript = Path.Combine(Environment.CurrentDirectory, "extracted-rom", "RomFS", "script", "event", "other", "seikakushindan", "seikakushindan.lua");
            if (!File.Exists(modifiedScript))
            {
                Assert.Fail("Failed to find modified personality test script: " + modifiedScript);
            }

            using (var unluacManager = new UnluacManager())
            {
                var originalScriptContents = await unluacManager.DecompileScript(originalScript);
                var modifiedScriptContents = await unluacManager.DecompileScript(modifiedScript);

                Assert.AreNotEqual(originalScriptContents, modifiedScriptContents, "The personality test script should be different than what it was before being patched.");
                Assert.IsTrue(modifiedScriptContents.Contains("WINDOW:SysMsg(200000)"), "The modified script should contain 'WINDOW:SysMsg(200000)', which is evidence of Sky Editor's modifications.");
            }
        }

        #endregion

        [TestMethod]
        public async Task StarterModBuildsSuccessfully()
        {
            // This was originally a SpecFlow test, but I abandoned SpecFlow, so this is the minimum-effort way of replicating it.

            try
            {
                // Given
                await GivenIHaveADSModSolution();
                await IInitializeTheSolutionWithAPSMDUSROM("PSMD-US.3ds");
                await GivenTheSolutionHasAPsmdStarterModProject();
                TheModpackProjectWillOutputADecryptedROM();

                // When
                await WhenIBuildTheProject();
                await WhenIUnpackTheResultingROM();

                // Then
                await ThePersonalityTestShouldHaveBeenProperlyPatched();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Encountered exception: " + ex.ToString());
                throw;
            }            
        }
    }
}
