using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkyEditor.Core;
using SkyEditor.Core.Projects;
using SkyEditor.ROMEditor.MysteryDungeon.PSMD.Projects;
using SkyEditor.ROMEditor.Projects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SkyEditor.ROMEditor.IntegrationTestsCSharp.SpecflowStepDefinitions
{
    [Binding]
    public sealed class SpecflowStepDefinitions
    {
        // For additional details on SpecFlow step definitions see http://go.specflow.org/doc-stepdef

        [Given("I have a DS Mod solution")]
        public async Task GivenIHaveADSModSolution()
        {
            var pluginManager = ScenarioContext.Current.Get<PluginManager>("PluginManager");
            var applicationViewModel = ScenarioContext.Current.Get<ApplicationViewModel>("ApplicationViewModel");

            var solutionBasePath = Path.Combine(Environment.CurrentDirectory, "Projects");
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

        [Given("I initialize the solution with a (.*) ROM")]
        public async Task IInitializeTheSolutionWithAPSMDUSROM(string romName)
        {
            var pluginManager = ScenarioContext.Current.Get<PluginManager>("PluginManager");
            var applicationViewModel = ScenarioContext.Current.Get<ApplicationViewModel>("ApplicationViewModel");

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

        [Given("The solution has a PsmdStarterMod project")]
        public async Task GivenTheSolutionHasAPsmdStarterModProject()
        {
            var pluginManager = ScenarioContext.Current.Get<PluginManager>("PluginManager");
            var applicationViewModel = ScenarioContext.Current.Get<ApplicationViewModel>("ApplicationViewModel");
            await applicationViewModel.CurrentSolution.AddNewProject("/", "starters", typeof(PsmdStarterMod), pluginManager);
        }

        [Given("The modpack project will output a decrypted ROM only")]
        public void TheModpackProjectWillOutputADecryptedROM()
        {
            var applicationViewModel = ScenarioContext.Current.Get<ApplicationViewModel>("ApplicationViewModel");
            var modpackProject = applicationViewModel.CurrentSolution.GetAllProjects().FirstOrDefault(p => p is DSModPackProject) as DSModPackProject;
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

        [When("I build the project")]
        public async Task WhenIBuildTheProject()
        {
            var applicationViewModel = ScenarioContext.Current.Get<ApplicationViewModel>("ApplicationViewModel");
            Assert.IsTrue(applicationViewModel.CurrentSolution.CanBuild, "Solution should be able to build");
            await applicationViewModel.CurrentSolution.Build();
        }

        [When("I unpack the resulting ROM")]
        public async Task WhenIUnpackTheResultingROM()
        {
            var applicationViewModel = ScenarioContext.Current.Get<ApplicationViewModel>("ApplicationViewModel");
            var modpackProject = applicationViewModel.CurrentSolution.GetAllProjects().FirstOrDefault(p => p is DSModPackProject) as DSModPackProject;
            if (modpackProject == null)
            {
                Assert.Fail("Failed to find a DSModPackProject");
            }

            var outputDirectory = modpackProject.GetOutputDir();
            var outputFilename = Path.Combine(outputDirectory, "PatchedRom.3ds");
            Assert.IsTrue(File.Exists(outputFilename), "Failed to find output file: " + outputFilename);

            using (var converter = new DotNet3dsToolkit.Converter())
            {
                await converter.ExtractCCI(outputFilename, Path.Combine(Environment.CurrentDirectory, "extracted-rom"));
            }
        }

        [Then("The personality test script should have been properly patched")]
        public void ThePersonalityTestShouldHaveBeenProperlyPatched()
        {
            //TODO: implement assert (verification) logic

            ScenarioContext.Current.Pending();
        }
    }
}
