using System.Collections.Generic;
using NUnit.Framework;
using Vts.IO;
using Vts.MonteCarlo;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class SimulationInputTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        List<string> listOfTestGeneratedFolders = new List<string>()
        {
            "results"
        };
        List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "test.txt"
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
            foreach (var folder in listOfTestGeneratedFolders)
            {
                FileIO.DeleteDirectory(folder);
            }
        }

        [Test]
        public void validate_deserialized_class_is_correct()
        {
            var i = new SimulationInput { N = 10 };

            var iCloned = i.Clone();

            Assert.AreEqual(iCloned.N, 10);
        }

        [Test]
        public void validate_deserialized_class_is_correct_when_using_FileIO()
        {
            new SimulationInput { N = 10 }.ToFile("test.txt");
            var iCloned = SimulationInput.FromFile("test.txt");

            Assert.AreEqual(iCloned.N, 10);
        }

        [Test]
        public void validate_null_detector_input_gets_converted_to_empty_list_correctly()
        {
            var si = new SimulationInput(
                100,
                "results",
                new SimulationOptions(),
                new Vts.MonteCarlo.Sources.DirectionalPointSourceInput(),
                new Vts.MonteCarlo.Tissues.MultiLayerTissueInput(),
                null
                );
            Assert.IsTrue(si.DetectorInputs.Count == 0);
        }

        [Test]
        public void validate_dependencies_can_be_constructed_by_providers()
        {
            var si = new SimulationInput(
                100,
                "results",
                new SimulationOptions(),
                SourceInputProvider.DirectionalPointSourceInput(),
                TissueInputProvider.MultiLayerTissueInput(),
                new[] { DetectorInputProvider.RDiffuseDetectorInput() }
            );
            Assert.NotNull(si.SourceInput);
            Assert.NotNull(si.TissueInput);
            Assert.NotNull(si.DetectorInputs);
            Assert.IsTrue(si.DetectorInputs.Count == 1);
        }
    }
}
