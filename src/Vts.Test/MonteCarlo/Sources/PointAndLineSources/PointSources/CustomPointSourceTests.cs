﻿using System;
using MathNet.Numerics.Random;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for Custom Point Sources
    /// </summary>
    [TestFixture]
    public class CustomPointSourceTests
    {
        private static PointSourcesValidationData _validationData;

        [OneTimeSetUp]
        public void setup_validation_data()
        {
            if (_validationData == null)
            {
                _validationData = new PointSourcesValidationData();
                _validationData.ReadData();
            }
        }
        /// <summary>
        /// test source input
        /// </summary>
        [Test]
        public void validate_source_input_with_flat_profile_type()
        {
            // check default constructor
            var si = new CustomPointSourceInput();
            Assert.IsNotNull(si);
            // check full definition
            si = new CustomPointSourceInput(

                    new DoubleRange(0.0, 0.0),
                    new DoubleRange(0.0, 0.0),
                    SourceDefaults.DefaultPosition.Clone(),
                    SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                    0
            );
            Assert.IsNotNull(si);
            // validate CreateSource
            var source = si.CreateSource(new MersenneTwister(0));
            Assert.IsNotNull(source);
        }
        /// <summary>
        /// Validate General Constructor of Custom Point Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_flat_profiletype_for_custom_point_source_test()
        {
            Random rng =
                new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue(); // todo: remove

            var ps = new CustomPointSource(_validationData.PolRange, 
                _validationData.AziRange, 
                _validationData.Direction, 
                _validationData.Translation)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[16]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[17]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[18]), _validationData.AcceptablePrecision);

            Assert.Less(Math.Abs(photon.DP.Position.X - _validationData.Tp[19]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _validationData.Tp[20]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _validationData.Tp[21]), _validationData.AcceptablePrecision);
        }
    }
}
