using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo
{
    public class MockSourceInput : ISourceInput
    {
        public string SourceType { get; set; }
        public int InitialTissueRegionIndex { get; set; }
        public ISource CreateSource(Random rng = null)
        {
            throw new NotImplementedException();
        }
    }

    [TestFixture]
    public class SourceInputProviderTests
    {
        [Test]
        public void validate_GetAllSourceInputs_returns_non_empty_list_of_ISourceInputs()
        {
            var allSourceInputs = Vts.MonteCarlo.SourceInputProvider.GetAllSourceInputs();

            Assert.NotNull(allSourceInputs);
            Assert.True(allSourceInputs.Count > 0);
        }

        [Test]
        public void validate_GetSourceInput_returns_non_null_ISourceInput_for_built_in_implementation()
        {
            var isotropicSourceInput = Vts.MonteCarlo.SourceInputProvider.GetSourceInput<IsotropicPointSourceInput>();

            Assert.NotNull(isotropicSourceInput);
        }

        [Test]
        public void validate_GetSourceInput_returns_non_null_ISourceInput_for_client_implementation()
        {
            var isotropicSourceInput = Vts.MonteCarlo.SourceInputProvider.GetSourceInput<MockSourceInput>();

            Assert.NotNull(isotropicSourceInput);
        }

        [Test]
        public void validate_explicit_construction_of_all_ISourceInputs_does_not_throw()
        {    
            var allSourceInputs = ConstructAllSourceInputsWithDefaultValuesExplicitly();

            Assert.NotNull(allSourceInputs);
            Assert.True(allSourceInputs.Count > 0);
        }

        private static IList<ISourceInput> ConstructAllSourceInputsWithDefaultValuesExplicitly()
        {
            // auto-generated from Object Dumper(https://gist.github.com/DTTerastar/6642655)
            //var inputs = Vts.MonteCarlo.SourceInputProvider.GenerateAllSourceInputs();
            //var serializedInputs = new Dumper(inputs, 15, false).ToString();
            //Console.WriteLine(serializedInputs);
            //Console.ReadKey();
            return new ISourceInput[] {
              new DirectionalPointSourceInput {
                SourceType = "DirectionalPoint",
                PointLocation = new Position {
                  X = 0,
                  Y = 0,
                  Z = 0
                },
                Direction = new Direction {
                  Ux = 0,
                  Uy = 0,
                  Uz = 1
                },
                InitialTissueRegionIndex = 0
              },
              new IsotropicPointSourceInput {
                SourceType = "IsotropicPoint",
                PointLocation = new Position {
                  X = 0,
                  Y = 0,
                  Z = 0
                },
                InitialTissueRegionIndex = 0
              },
              new CustomPointSourceInput {
                SourceType = "CustomPoint",
                PolarAngleEmissionRange = new DoubleRange {
                  Start = 0,
                  Stop = 0,
                  Delta = 0,
                  Count = 2
                },
                AzimuthalAngleEmissionRange = new DoubleRange {
                  Start = 0,
                  Stop = 0,
                  Delta = 0,
                  Count = 2
                },
                PointLocation = new Position {
                  X = 0,
                  Y = 0,
                  Z = 0
                },
                Direction = new Direction {
                  Ux = 0,
                  Uy = 0,
                  Uz = 1
                },
                InitialTissueRegionIndex = 0
              },
              new DirectionalLineSourceInput {
                ThetaConvOrDiv = 0,
                SourceType = "DirectionalLine",
                LineLength = 1,
                SourceProfile = new FlatSourceProfile(),
                NewDirectionOfPrincipalSourceAxis = new Direction {
                  Ux = 0,
                  Uy = 0,
                  Uz = 1
                },
                TranslationFromOrigin = new Position {
                  X = 0,
                  Y = 0,
                  Z = 0
                },
                BeamRotationFromInwardNormal = new PolarAzimuthalAngles {
                  Theta = 0,
                  Phi = 0
                },
                InitialTissueRegionIndex = 0
              },
              new IsotropicLineSourceInput {
                SourceType = "IsotropicLine",
                LineLength = 1,
                SourceProfile = new FlatSourceProfile(),
                NewDirectionOfPrincipalSourceAxis = new Direction {
                  Ux = 0,
                  Uy = 0,
                  Uz = 1
                },
                TranslationFromOrigin = new Position {
                  X = 0,
                  Y = 0,
                  Z = 0
                },
                BeamRotationFromInwardNormal = new PolarAzimuthalAngles {
                  Theta = 0,
                  Phi = 0
                },
                InitialTissueRegionIndex = 0
              },
              new CustomLineSourceInput {
                SourceType = "CustomLine",
                LineLength = 1,
                SourceProfile = new FlatSourceProfile (),
                PolarAngleEmissionRange = new DoubleRange {
                  Start = 0,
                  Stop = 3.14159265358979,
                  Delta = 3.14159265358979,
                  Count = 2
                },
                AzimuthalAngleEmissionRange = new DoubleRange {
                  Start = 0,
                  Stop = 6.28318530717959,
                  Delta = 6.28318530717959,
                  Count = 2
                },
                NewDirectionOfPrincipalSourceAxis = new Direction {
                  Ux = 0,
                  Uy = 0,
                  Uz = 1
                },
                TranslationFromOrigin = new Position {
                  X = 0,
                  Y = 0,
                  Z = 0
                },
                BeamRotationFromInwardNormal = new PolarAzimuthalAngles {
                  Theta = 0,
                  Phi = 0
                },
                InitialTissueRegionIndex = 0
              },
              new DirectionalCircularSourceInput {
                ThetaConvOrDiv = 0,
                SourceType = "DirectionalCircular",
                OuterRadius = 1,
                InnerRadius = 0,
                SourceProfile = new FlatSourceProfile (),
                NewDirectionOfPrincipalSourceAxis = new Direction {
                  Ux = 0,
                  Uy = 0,
                  Uz = 1
                },
                TranslationFromOrigin = new Position {
                  X = 0,
                  Y = 0,
                  Z = 0
                },
                BeamRotationFromInwardNormal = new PolarAzimuthalAngles {
                  Theta = 0,
                  Phi = 0
                },
                InitialTissueRegionIndex = 0
              },
              new CustomCircularSourceInput {
                SourceType = "CustomCircular",
                OuterRadius = 1,
                InnerRadius = 0,
                SourceProfile = new FlatSourceProfile (),
                PolarAngleEmissionRange = new DoubleRange {
                  Start = 0,
                  Stop = 1.5707963267949,
                  Delta = 1.5707963267949,
                  Count = 2
                },
                AzimuthalAngleEmissionRange = new DoubleRange {
                  Start = 0,
                  Stop = 6.28318530717959,
                  Delta = 6.28318530717959,
                  Count = 2
                },
                NewDirectionOfPrincipalSourceAxis = new Direction {
                  Ux = 0,
                  Uy = 0,
                  Uz = 1
                },
                TranslationFromOrigin = new Position {
                  X = 0,
                  Y = 0,
                  Z = 0
                },
                BeamRotationFromInwardNormal = new PolarAzimuthalAngles {
                  Theta = 0,
                  Phi = 0
                },
                InitialTissueRegionIndex = 0
              },
              new DirectionalEllipticalSourceInput {
                ThetaConvOrDiv = 0,
                SourceType = "DirectionalElliptical",
                AParameter = 1,
                BParameter = 2,
                SourceProfile = new FlatSourceProfile(),
                NewDirectionOfPrincipalSourceAxis = new Direction {
                  Ux = 0,
                  Uy = 0,
                  Uz = 1
                },
                TranslationFromOrigin = new Position {
                  X = 0,
                  Y = 0,
                  Z = 0
                },
                BeamRotationFromInwardNormal = new PolarAzimuthalAngles {
                  Theta = 0,
                  Phi = 0
                },
                InitialTissueRegionIndex = 0
              },
              new CustomEllipticalSourceInput {
                SourceType = "CustomElliptical",
                AParameter = 1,
                BParameter = 2,
                SourceProfile = new FlatSourceProfile (),
                PolarAngleEmissionRange = new DoubleRange {
                  Start = 0,
                  Stop = 1.5707963267949,
                  Delta = 1.5707963267949,
                  Count = 2
                },
                AzimuthalAngleEmissionRange = new DoubleRange {
                  Start = 0,
                  Stop = 6.28318530717959,
                  Delta = 6.28318530717959,
                  Count = 2
                },
                NewDirectionOfPrincipalSourceAxis = new Direction {
                  Ux = 0,
                  Uy = 0,
                  Uz = 1
                },
                TranslationFromOrigin = new Position {
                  X = 0,
                  Y = 0,
                  Z = 0
                },
                BeamRotationFromInwardNormal = new PolarAzimuthalAngles {
                  Theta = 0,
                  Phi = 0
                },
                InitialTissueRegionIndex = 0
              },
              new DirectionalRectangularSourceInput {
                ThetaConvOrDiv = 0,
                SourceType = "DirectionalRectangular",
                RectLengthX = 1,
                RectWidthY = 2,
                SourceProfile = new FlatSourceProfile(),
                NewDirectionOfPrincipalSourceAxis = new Direction {
                  Ux = 0,
                  Uy = 0,
                  Uz = 1
                },
                TranslationFromOrigin = new Position {
                  X = 0,
                  Y = 0,
                  Z = 0
                },
                BeamRotationFromInwardNormal = new PolarAzimuthalAngles {
                  Theta = 0,
                  Phi = 0
                },
                InitialTissueRegionIndex = 0
              },
              new CustomRectangularSourceInput {
                SourceType = "CustomRectangular",
                RectLengthX = 1,
                RectWidthY = 2,
                SourceProfile = new FlatSourceProfile(),
                PolarAngleEmissionRange = new DoubleRange {
                  Start = 0,
                  Stop = 1.5707963267949,
                  Delta = 1.5707963267949,
                  Count = 2
                },
                AzimuthalAngleEmissionRange = new DoubleRange {
                  Start = 0,
                  Stop = 6.28318530717959,
                  Delta = 6.28318530717959,
                  Count = 2
                },
                NewDirectionOfPrincipalSourceAxis = new Direction {
                  Ux = 0,
                  Uy = 0,
                  Uz = 1
                },
                TranslationFromOrigin = new Position {
                  X = 0,
                  Y = 0,
                  Z = 0
                },
                BeamRotationFromInwardNormal = new PolarAzimuthalAngles {
                  Theta = 0,
                  Phi = 0
                },
                InitialTissueRegionIndex = 0
              },
              new LambertianSurfaceEmittingCylindricalFiberSourceInput {
                SourceType = "LambertianSurfaceEmittingCylindricalFiber",
                FiberRadius = 1,
                FiberHeightZ = 1,
                CurvedSurfaceEfficiency = 1,
                BottomSurfaceEfficiency = 1,
                NewDirectionOfPrincipalSourceAxis = new Direction {
                  Ux = 0,
                  Uy = 0,
                  Uz = 1
                },
                TranslationFromOrigin = new Position {
                  X = 0,
                  Y = 0,
                  Z = 0
                },
                InitialTissueRegionIndex = 0
              },
              new LambertianSurfaceEmittingSphericalSourceInput {
                SourceType = "LambertianSurfaceEmittingSpherical",
                Radius = 1,
                TranslationFromOrigin = new Position {
                  X = 0,
                  Y = 0,
                  Z = 0
                },
                InitialTissueRegionIndex = 0
              },
              new CustomSurfaceEmittingSphericalSourceInput {
                SourceType = "CustomSurfaceEmittingSpherical",
                Radius = 1,
                PolarAngleRangeToDefineSphericalSurface = new DoubleRange {
                  Start = 0,
                  Stop = 1.5707963267949,
                  Delta = 1.5707963267949,
                  Count = 2
                },
                AzimuthalAngleRangeToDefineSphericalSurface = new DoubleRange {
                  Start = 0,
                  Stop = 6.28318530717959,
                  Delta = 6.28318530717959,
                  Count = 2
                },
                NewDirectionOfPrincipalSourceAxis = new Direction {
                  Ux = 0,
                  Uy = 0,
                  Uz = 1
                },
                TranslationFromOrigin = new Position {
                  X = 0,
                  Y = 0,
                  Z = 0
                },
                InitialTissueRegionIndex = 0
              },
              new LambertianSurfaceEmittingCuboidalSourceInput {
                SourceType = "LambertianSurfaceEmittingCubiodal",
                CubeLengthX = 1,
                CubeWidthY = 1,
                CubeHeightZ = 1,
                SourceProfile = new FlatSourceProfile(),
                NewDirectionOfPrincipalSourceAxis = new Direction {
                  Ux = 0,
                  Uy = 0,
                  Uz = 1
                },
                TranslationFromOrigin = new Position {
                  X = 0,
                  Y = 0,
                  Z = 0
                },
                InitialTissueRegionIndex = 0
              },
              new LambertianSurfaceEmittingTubularSourceInput {
                SourceType = "LambertianSurfaceEmittingTubular",
                TubeRadius = 1,
                TubeHeightZ = 1,
                NewDirectionOfPrincipalSourceAxis = new Direction {
                  Ux = 0,
                  Uy = 0,
                  Uz = 1
                },
                TranslationFromOrigin = new Position {
                  X = 0,
                  Y = 0,
                  Z = 0
                },
                InitialTissueRegionIndex = 0
              },
              new IsotropicVolumetricCuboidalSourceInput {
                SourceType = "IsotropicVolumetricCubiodal",
                CubeLengthX = 1,
                CubeWidthY = 1,
                CubeHeightZ = 1,
                SourceProfile = new FlatSourceProfile(),
                NewDirectionOfPrincipalSourceAxis = new Direction {
                  Ux = 0,
                  Uy = 0,
                  Uz = 1
                },
                TranslationFromOrigin = new Position {
                  X = 0,
                  Y = 0,
                  Z = 0
                },
                InitialTissueRegionIndex = 0
              },
              new CustomVolumetricCuboidalSourceInput {
                SourceType = "CustomVolumetricCubiodal",
                CubeLengthX = 1,
                CubeWidthY = 1,
                CubeHeightZ = 1,
                SourceProfile = new FlatSourceProfile(),
                PolarAngleEmissionRange = new DoubleRange {
                  Start = 0,
                  Stop = 3.14159265358979,
                  Delta = 3.14159265358979,
                  Count = 2
                },
                AzimuthalAngleEmissionRange = new DoubleRange {
                  Start = 0,
                  Stop = 6.28318530717959,
                  Delta = 6.28318530717959,
                  Count = 2
                },
                NewDirectionOfPrincipalSourceAxis = new Direction {
                  Ux = 0,
                  Uy = 0,
                  Uz = 1
                },
                TranslationFromOrigin = new Position {
                  X = 0,
                  Y = 0,
                  Z = 0
                },
                InitialTissueRegionIndex = 0
              },
              new IsotropicVolumetricEllipsoidalSourceInput {
                SourceType = "IsotropicVolumetricEllipsoidal",
                AParameter = 1,
                BParameter = 1,
                CParameter = 2,
                SourceProfile = new FlatSourceProfile(),
                NewDirectionOfPrincipalSourceAxis = new Direction {
                  Ux = 0,
                  Uy = 0,
                  Uz = 1
                },
                TranslationFromOrigin = new Position {
                  X = 0,
                  Y = 0,
                  Z = 0
                },
                InitialTissueRegionIndex = 0
              },
              new CustomVolumetricEllipsoidalSourceInput {
                SourceType = "CustomVolumetricEllipsoidal",
                AParameter = 1,
                BParameter = 1,
                CParameter = 2,
                SourceProfile = new FlatSourceProfile (),
                PolarAngleEmissionRange = new DoubleRange {
                  Start = 0,
                  Stop = 3.14159265358979,
                  Delta = 3.14159265358979,
                  Count = 2
                },
                AzimuthalAngleEmissionRange = new DoubleRange {
                  Start = 0,
                  Stop = 6.28318530717959,
                  Delta = 6.28318530717959,
                  Count = 2
                },
                NewDirectionOfPrincipalSourceAxis = new Direction {
                  Ux = 0,
                  Uy = 0,
                  Uz = 1
                },
                TranslationFromOrigin = new Position {
                  X = 0,
                  Y = 0,
                  Z = 0
                },
                InitialTissueRegionIndex = 0
              }
            };
        }
    }
}
