using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;

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
            var si1 = new SimulationInput {N = 30};
            var si2 = new SimulationInput {N = 20};
            var sim1 = new MonteCarloSimulation(si1);
            var sim2 = new MonteCarloSimulation(si2);
            var sims = new[] {sim1, sim2};

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
            var si = new SimulationInput("demoInput") {N = 30}
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

        [Test]
        public void validate_user_specified_SourceInput_runs_simulation_without_crashing()
        {
            var sourceInput = new PointySourceInput();
            var si = new SimulationInput("demoInput") { N = 30 }
                .WithSourceInput(sourceInput)
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

        [Test]
        public void validate_user_specified_DetectorInput_runs_simulation_without_crashing()
        {
            var detectorInput = new DetectoryDetectorInput();
            var si = new SimulationInput("demoInput") { N = 30 }
                .WithSourceInput(SourceInputProvider.DirectionalPointSourceInput())
                .WithTissueInput(TissueInputProvider.MultiLayerTissueInput())
                .WithDetectorInputs(detectorInput);

            Assert.NotNull(si.SourceInput);
            Assert.NotNull(si.TissueInput);
            Assert.NotNull(si.DetectorInputs);
            Assert.IsTrue(si.DetectorInputs.Count == 1);

            var mc = new MonteCarloSimulation(si);
            var output = mc.Run();

            Assert.NotNull(output);
            Assert.True(output.Input.N == 30);
        }

        internal class PointySourceInput : ISourceInput
        {
            private readonly ISourceInput _sourceInput = new DirectionalPointSourceInput();

            public string SourceType { get; set; } = "Pointy";

            public int InitialTissueRegionIndex
            {
                get => _sourceInput.InitialTissueRegionIndex;
                set => _sourceInput.InitialTissueRegionIndex = value;
            }

            public ISource CreateSource(Random rng = null) => new PointySource();
        }

        internal class PointySource : ISource
        {
            private readonly ISource _source = new DirectionalPointSourceInput().CreateSource(null);
            public Photon GetNextPhoton(ITissue tissue) => _source?.GetNextPhoton(tissue);
            public Random Rng => _source?.Rng;
        }

        internal class DetectoryDetectorInput : IDetectorInput
        {
            public string TallyType { get; set; } = "Detectory";
            public string Name { get; set; } = "TestDetectoryDetectorInput";
            public TallyDetails TallyDetails { get; set; } = new TallyDetails();
            public IDetector CreateDetector()
            {
                return new DetectoryDetector();
            }
        }

        internal class DetectoryDetector : IDetector
        {
            private readonly IDetector _detector = new RDiffuseDetector();
            public string TallyType { get; } = "Detectory";
            public string Name => _detector?.Name;
            public bool TallySecondMoment => _detector.TallySecondMoment;
            public TallyDetails TallyDetails
            {
                get => _detector?.TallyDetails;
                set => _detector.TallyDetails = value;
            }
            public void Initialize(ITissue tissue = null, Random rng = null) => _detector.Initialize(tissue, rng);
            public void Tally(Photon photon) => _detector.Tally(photon);
            public void Normalize(long numPhotons) => _detector.Normalize(numPhotons);
            public BinaryArraySerializer[] GetBinarySerializers() => _detector.GetBinarySerializers();
        }
    }
}
