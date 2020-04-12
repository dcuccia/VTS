using System;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.IO
{
    [TestFixture]
    public class VtsMonteCarloJsonSerializerTests
    {
        [Test]
        public void validate_deserialization_of_SimulationOptions()
        {
            var jsonSerialized = VtsJsonSerializer.WriteToJson(new SimulationOptions());
            var optionsDerialized = VtsJsonSerializer.ReadFromJson<SimulationOptions>(jsonSerialized);
            Assert.IsTrue(optionsDerialized != null);
        }

        [Test]
        public void validate_serialization_of_SimulationInput()
        {
            var jsonSerialized = VtsMonteCarloJsonSerializer.WriteToJson(new SimulationInput());
            Assert.IsTrue(jsonSerialized != null && jsonSerialized.Length > 0);
        }

        [Test]
        public void validate_deserialization_of_SimulationInput()
        {
            var jsonSerialized = VtsMonteCarloJsonSerializer.WriteToJson(new SimulationInput());
            var inputDeserialized = VtsMonteCarloJsonSerializer.ReadFromJson<SimulationInput>(jsonSerialized);
            Assert.IsTrue(inputDeserialized != null);
        }

        [Test]
        public void validate_deserialization_of_SimulationInput_from_file()
        {
            VtsMonteCarloJsonSerializer.WriteToJsonFile(new SimulationInput(), "VtsJsonSerializerTests_file3.txt");
            var objectDeserialized = VtsMonteCarloJsonSerializer.ReadFromJsonFile<SimulationInput>("VtsJsonSerializerTests_file3.txt");
            Assert.IsTrue(objectDeserialized != null);
        }

        [Test]
        public void validate_serialization_of_LayerRegion()
        {
            var layer = new LayerTissueRegion(
                zRange: new DoubleRange(3, 4, 2),
                op: new OpticalProperties(mua: 0.011, musp: 1.1, g: 0.99, n: 1.44));

            var jsonSerialized = VtsMonteCarloJsonSerializer.WriteToJson(layer);

            Assert.IsTrue(jsonSerialized != null && jsonSerialized.Length > 0);
        }

        [Test]
        public void validate_deserialization_of_LayerRegion()
        {
            Func<double, double, bool> areRoughlyEqual = (a, b) => Math.Abs(a - b) < 0.001;

            var layer = new LayerTissueRegion(
                zRange: new DoubleRange(3.0, 4.0, 2),
                op: new OpticalProperties(mua: 0.011, musp: 1.1, g: 0.99, n: 1.44));

            var jsonSerialized = VtsMonteCarloJsonSerializer.WriteToJson(layer);
            var layerDeserialized = VtsMonteCarloJsonSerializer.ReadFromJson<LayerTissueRegion>(jsonSerialized);
            Assert.IsTrue(layerDeserialized != null);
            Assert.IsTrue(areRoughlyEqual(layerDeserialized.ZRange.Start, 3.0));
            Assert.IsTrue(areRoughlyEqual(layerDeserialized.ZRange.Stop, 4.0));
            Assert.IsTrue(areRoughlyEqual(layerDeserialized.ZRange.Count, 2));
            Assert.IsTrue(areRoughlyEqual(layerDeserialized.RegionOP.Mua, 0.011));
            Assert.IsTrue(areRoughlyEqual(layerDeserialized.RegionOP.Musp, 1.1));
            Assert.IsTrue(areRoughlyEqual(layerDeserialized.RegionOP.G, 0.99));
            Assert.IsTrue(areRoughlyEqual(layerDeserialized.RegionOP.N, 1.44));
        }

        [Test]
        public void validate_serialization_of_MultiLayerTissueInput()
        {
            var layer0 = new LayerTissueRegion(
                zRange: new DoubleRange(2.0, 3.0, 2),
                op: new OpticalProperties(mua: 0.011, musp: 1.1, g: 0.99, n: 1.44));

            var layer1 = new LayerTissueRegion(
                zRange: new DoubleRange(3.0, 4.0, 2),
                op: new OpticalProperties(mua: 0.0111, musp: 1.11, g: 0.999, n: 1.444));

            var multiRegionInput = new MultiLayerTissueInput(new[] { layer0, layer1 });

            var jsonSerialized = VtsMonteCarloJsonSerializer.WriteToJson(multiRegionInput);

            Assert.IsTrue(jsonSerialized != null && jsonSerialized.Length > 0);
        }

        [Test]
        public void validate_deserialization_of_MultiLayerTissueInput()
        {
            Func<double, double, bool> areRoughlyEqual = (a, b) => Math.Abs(a - b) < 0.001;

            var layer0 = new LayerTissueRegion(
                zRange: new DoubleRange(2.0, 3.0, 2),
                op: new OpticalProperties(mua: 0.011, musp: 1.1, g: 0.99, n: 1.44));

            var layer1 = new LayerTissueRegion(
                zRange: new DoubleRange(3.0, 4.0, 2),
                op: new OpticalProperties(mua: 0.0111, musp: 1.11, g: 0.999, n: 1.444));

            var multiRegionInput = new MultiLayerTissueInput(new[] {layer0, layer1});

            var jsonSerialized = VtsMonteCarloJsonSerializer.WriteToJson(multiRegionInput);
            var multiRegionInputDeserialized = VtsMonteCarloJsonSerializer.ReadFromJson<MultiLayerTissueInput>(jsonSerialized);
            Assert.IsTrue(multiRegionInputDeserialized != null);

            var region0Deserialized = (LayerTissueRegion) multiRegionInputDeserialized.Regions[0];
            Assert.IsTrue(areRoughlyEqual(region0Deserialized.ZRange.Start, 2.0));
            Assert.IsTrue(areRoughlyEqual(region0Deserialized.ZRange.Stop, 3.0));
            Assert.IsTrue(areRoughlyEqual(region0Deserialized.ZRange.Count, 2));
            Assert.IsTrue(areRoughlyEqual(region0Deserialized.RegionOP.Mua, 0.011));
            Assert.IsTrue(areRoughlyEqual(region0Deserialized.RegionOP.Musp, 1.1));
            Assert.IsTrue(areRoughlyEqual(region0Deserialized.RegionOP.G, 0.99));
            Assert.IsTrue(areRoughlyEqual(region0Deserialized.RegionOP.N, 1.44));

            var region1Deserialized = (LayerTissueRegion)multiRegionInputDeserialized.Regions[1];
            Assert.IsTrue(areRoughlyEqual(region1Deserialized.ZRange.Start, 3.0));
            Assert.IsTrue(areRoughlyEqual(region1Deserialized.ZRange.Stop, 4.0));
            Assert.IsTrue(areRoughlyEqual(region1Deserialized.ZRange.Count, 2));
            Assert.IsTrue(areRoughlyEqual(region1Deserialized.RegionOP.Mua, 0.0111));
            Assert.IsTrue(areRoughlyEqual(region1Deserialized.RegionOP.Musp, 1.11));
            Assert.IsTrue(areRoughlyEqual(region1Deserialized.RegionOP.G, 0.999));
            Assert.IsTrue(areRoughlyEqual(region1Deserialized.RegionOP.N, 1.444));
        }

        /// <summary>
        /// test to verify serialization and deserialization of gaussiansourceprofile runs successfully
        /// </summary>
        [Test]
        public void validate_serialization_and_deserialization_of_gaussiansourceprofile_runs_successfully()
        {
            var source = new CustomCircularSourceInput
            {
                BeamDiameterFWHM = 1.0,
            };

            var sourceSerialized = VtsMonteCarloJsonSerializer.WriteToJson(source);

            var sourceDeserialized = VtsMonteCarloJsonSerializer.ReadFromJson<CustomCircularSourceInput>(sourceSerialized);

            Assert.IsTrue(sourceDeserialized.BeamDiameterFWHM == 1.0);
        }
    }
}
