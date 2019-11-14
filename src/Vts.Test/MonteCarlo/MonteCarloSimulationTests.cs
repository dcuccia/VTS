using System.Collections.Generic;
using NUnit.Framework;
using Vts.IO;
using Vts.MonteCarlo;

namespace Vts.Test.MonteCarlo
{
    /// <summary>
    /// Unit tests for MonteCarloSimulation
    /// </summary>
    [TestFixture]
    public class MonteCarloSimulationTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "file.txt", // file that capture screen output of MC simulation
        };
        /// <summary>
        /// clear all generated folders and files
        /// </summary>
        [OneTimeSetUp]
        [OneTimeTearDown]
        public void clear_folders_and_files()
        {
            foreach (var file in listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
        }

        /// <summary>
        /// Validate RunAll Given Two Simulations Runs Without Crashing
        /// </summary>
        [Test]
        public void validate_RunAll_given_two_simulations_runs_without_crashing()
        {
            var si1 = new SimulationInput { N = 30 };
            var si2 = new SimulationInput { N = 20 };
            var sim1 = new MonteCarloSimulation(si1);
            var sim2 = new MonteCarloSimulation(si2);
            var sims = new[] { sim1, sim2 };

            var outputs = MonteCarloSimulation.RunAll(sims);

            Assert.NotNull(outputs[0]);
            Assert.NotNull(outputs[1]);
            Assert.True(outputs[0].Input.N == 30);
            Assert.True(outputs[1].Input.N == 20);
        }

        /// <summary>
        /// Validate fluent-constructed SimulationInput runs simulation without crashing
        /// </summary>
        [Test]
        public void validate_fluent_constructed_SimulationInput_runs_simulation_without_crashing()
        {
            var si = new SimulationInput("demoInput") { N = 30 }
                .WithSourceInput(SourceInputProvider.DirectionalPointSourceInput())
                .WithTissueInput(TissueInputProvider.MultiLayerTissueInput())
                .WithDetectorInputs(DetectorInputProvider.RDiffuseDetectorInput());

            Assert.NotNull(si.SourceInput);
            Assert.NotNull(si.TissueInput);
            Assert.NotNull(si.DetectorInputs);
            Assert.IsTrue(si.DetectorInputs.Count == 1);

            var mc = new MonteCarloSimulation(si);
            var output = mc.Run();

            Assert.NotNull(output);
            Assert.True(output.Input.N == 30);
        }
    }
}
