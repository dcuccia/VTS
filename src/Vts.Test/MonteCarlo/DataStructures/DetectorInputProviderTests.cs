using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class DetectorInputProviderTests
    {
        [Test]
        public void validate_GenerateAllDetectorInputs_returns_non_empty_list_of_ISourceInputs()
        {
            var allDetectorInputs = Vts.MonteCarlo.DetectorInputProvider.GenerateAllDetectorInputs();

            Assert.NotNull(allDetectorInputs);
            Assert.True(allDetectorInputs.Count > 0);
        }

        [Test]
        public void validate_explicit_construction_of_all_IDetectorInputs_does_not_throw()
        {    
            var allDetectorInputs = ConstructAllDetectorInputsWithDefaultValuesExplicitly();

            Assert.NotNull(allDetectorInputs);
            Assert.True(allDetectorInputs.Count > 0);
        }

        private static IList<IDetectorInput> ConstructAllDetectorInputsWithDefaultValuesExplicitly()
        {
            // auto - generated from Object Dumper(https://gist.github.com/DTTerastar/6642655)
            //var inputs = Vts.MonteCarlo.DetectorInputProvider.GenerateAllDetectorInputs();
            //var serializedInputs = new Dumper(inputs, 15, false).ToString();
            //Console.WriteLine(serializedInputs);
            //Console.ReadKey();
            return new IDetectorInput[] {
                new AOfRhoAndZDetectorInput {
                Rho = new DoubleRange {
                    Start = 0,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 101
                },
                Z = new DoubleRange {
                    Start = 0,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 101
                },
                TallyType = "AOfRhoAndZ",
                Name = "AOfRhoAndZ",
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = false,
                    IsTransmittanceTally = false,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = true,
                    IsCylindricalTally = true,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = true,
                    IsNotImplementedYet = false
                }
                },
                new AOfXAndYAndZDetectorInput {
                TallyType = "AOfXAndYAndZ",
                Name = "AOfXAndYAndZ",
                X = new DoubleRange {
                    Start = -10,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 201
                },
                Y = new DoubleRange {
                    Start = -10,
                    Stop = 10,
                    Delta = 20,
                    Count = 2
                },
                Z = new DoubleRange {
                    Start = 0,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 101
                },
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = false,
                    IsTransmittanceTally = false,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = true,
                    IsCylindricalTally = false,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = true,
                    IsNotImplementedYet = false
                }
                },
                new ATotalDetectorInput {
                TallyType = "ATotal",
                Name = "ATotal",
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = false,
                    IsTransmittanceTally = false,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = true,
                    IsCylindricalTally = false,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = false,
                    IsNotImplementedYet = false
                }
                },
                new FluenceOfRhoAndZAndTimeDetectorInput {
                Rho = new DoubleRange {
                    Start = 0,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 101
                },
                Z = new DoubleRange {
                    Start = 0,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 101
                },
                Time = new DoubleRange {
                    Start = 0,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 101
                },
                TallyType = "FluenceOfRhoAndZAndTime",
                Name = "FluenceOfRhoAndZAndTime",
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = false,
                    IsTransmittanceTally = false,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = true,
                    IsCylindricalTally = true,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = true,
                    IsNotImplementedYet = false
                }
                },
                new FluenceOfRhoAndZDetectorInput {
                Rho = new DoubleRange {
                    Start = 0,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 101
                },
                Z = new DoubleRange {
                    Start = 0,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 101
                },
                TallyType = "FluenceOfRhoAndZ",
                Name = "FluenceOfRhoAndZ",
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = false,
                    IsTransmittanceTally = false,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = true,
                    IsCylindricalTally = true,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = true,
                    IsNotImplementedYet = false
                }
                },
                new FluenceOfXAndYAndZDetectorInput {
                TallyType = "FluenceOfXAndYAndZ",
                Name = "FluenceOfXAndYAndZ",
                X = new DoubleRange {
                    Start = -10,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 201
                },
                Y = new DoubleRange {
                    Start = -10,
                    Stop = 10,
                    Delta = 20,
                    Count = 2
                },
                Z = new DoubleRange {
                    Start = 0,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 101
                },
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = false,
                    IsTransmittanceTally = false,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = true,
                    IsCylindricalTally = false,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = true,
                    IsNotImplementedYet = false
                }
                },
                new FluenceOfXAndYAndZAndOmegaDetectorInput {
                TallyType = "FluenceOfXAndYAndZAndOmega",
                Name = "FluenceOfXAndYAndZAndOmega",
                X = new DoubleRange {
                    Start = -10,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 201
                },
                Y = new DoubleRange {
                    Start = -10,
                    Stop = 10,
                    Delta = 20,
                    Count = 2
                },
                Z = new DoubleRange {
                    Start = 0,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 101
                },
                Omega = new DoubleRange {
                    Start = 0,
                    Stop = 1,
                    Delta = 0.05,
                    Count = 21
                },
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = false,
                    IsTransmittanceTally = false,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = true,
                    IsCylindricalTally = false,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = true,
                    IsNotImplementedYet = false
                }
                },
                new RadianceOfRhoAndZAndAngleDetectorInput {
                Rho = new DoubleRange {
                    Start = 0,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 101
                },
                Z = new DoubleRange {
                    Start = 0,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 101
                },
                Angle = new DoubleRange {
                    Start = 0,
                    Stop = 3.14159265358979,
                    Delta = 1.5707963267949,
                    Count = 3
                },
                TallyType = "RadianceOfRhoAndZAndAngle",
                Name = "RadianceOfRhoAndZAndAngle",
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = false,
                    IsTransmittanceTally = false,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = true,
                    IsCylindricalTally = true,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = true,
                    IsNotImplementedYet = false
                }
                },
                new RadianceOfFxAndZAndAngleDetectorInput {
                Fx = new DoubleRange {
                    Start = 0,
                    Stop = 0.5,
                    Delta = 0.01,
                    Count = 51
                },
                Z = new DoubleRange {
                    Start = 0,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 101
                },
                Angle = new DoubleRange {
                    Start = 0,
                    Stop = 3.14159265358979,
                    Delta = 1.5707963267949,
                    Count = 3
                },
                TallyType = "RadianceOfFxAndZAndAngle",
                Name = "RadianceOfFxAndZAndAngle",
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = false,
                    IsTransmittanceTally = false,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = true,
                    IsCylindricalTally = true,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = true,
                    IsNotImplementedYet = false
                }
                },
                new RadianceOfXAndYAndZAndThetaAndPhiDetectorInput {
                TallyType = "RadianceOfXAndYAndZAndThetaAndPhi",
                Name = "RadianceOfXAndYAndZAndThetaAndPhi",
                X = new DoubleRange {
                    Start = -10,
                    Stop = 10,
                    Delta = 0.2,
                    Count = 101
                },
                Y = new DoubleRange {
                    Start = -10,
                    Stop = 10,
                    Delta = 0.2,
                    Count = 101
                },
                Z = new DoubleRange {
                    Start = 0,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 101
                },
                Theta = new DoubleRange {
                    Start = 0,
                    Stop = 3.14159265358979,
                    Delta = 0.785398163397448,
                    Count = 5
                },
                Phi = new DoubleRange {
                    Start = -3.14159265358979,
                    Stop = 3.14159265358979,
                    Delta = 1.5707963267949,
                    Count = 5
                },
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = false,
                    IsTransmittanceTally = false,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = true,
                    IsCylindricalTally = false,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = true,
                    IsNotImplementedYet = false
                }
                },
                new RadianceOfRhoAtZDetectorInput {
                Rho = new DoubleRange {
                    Start = 0,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 101
                },
                ZDepth = 3,
                TallyType = "RadianceOfRhoAtZ",
                Name = "RadianceOfRhoAtZ",
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = false,
                    IsTransmittanceTally = false,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = true,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = false,
                    IsCylindricalTally = true,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = false,
                    IsNotImplementedYet = false
                }
                },
                new RDiffuseDetectorInput {
                TallyType = "RDiffuse",
                Name = "RDiffuse",
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = true,
                    IsTransmittanceTally = false,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = false,
                    IsCylindricalTally = false,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = false,
                    IsNotImplementedYet = false
                }
                },
                new ROfAngleDetectorInput {
                Angle = new DoubleRange {
                    Start = 1.5707963267949,
                    Stop = 3.14159265358979,
                    Delta = 0.392699081698724,
                    Count = 5
                },
                TallyType = "ROfAngle",
                Name = "ROfAngle",
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = true,
                    IsTransmittanceTally = false,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = false,
                    IsCylindricalTally = false,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = false,
                    IsNotImplementedYet = false
                }
                },
                new ROfFxAndTimeDetectorInput {
                Fx = new DoubleRange {
                    Start = 0,
                    Stop = 0.5,
                    Delta = 0.01,
                    Count = 51
                },
                Time = new DoubleRange {
                    Start = 0,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 101
                },
                TallyType = "ROfFxAndTime",
                Name = "ROfFxAndTime",
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = true,
                    IsTransmittanceTally = false,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = false,
                    IsCylindricalTally = false,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = false,
                    IsNotImplementedYet = false
                }
                },
                new ROfFxDetectorInput {
                Fx = new DoubleRange {
                    Start = 0,
                    Stop = 0.5,
                    Delta = 0.01,
                    Count = 51
                },
                TallyType = "ROfFx",
                Name = "ROfFx",
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = true,
                    IsTransmittanceTally = false,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = false,
                    IsCylindricalTally = false,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = false,
                    IsNotImplementedYet = false
                }
                },
                new ROfRhoAndAngleDetectorInput {
                Rho = new DoubleRange {
                    Start = 0,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 101
                },
                Angle = new DoubleRange {
                    Start = 1.5707963267949,
                    Stop = 3.14159265358979,
                    Delta = 0.392699081698724,
                    Count = 5
                },
                TallyType = "ROfRhoAndAngle",
                Name = "ROfRhoAndAngle",
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = true,
                    IsTransmittanceTally = false,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = false,
                    IsCylindricalTally = true,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = false,
                    IsNotImplementedYet = false
                }
                },
                new ROfRhoAndOmegaDetectorInput {
                Rho = new DoubleRange {
                    Start = 0,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 101
                },
                Omega = new DoubleRange {
                    Start = 0,
                    Stop = 1,
                    Delta = 0.05,
                    Count = 21
                },
                TallyType = "ROfRhoAndOmega",
                Name = "ROfRhoAndOmega",
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = true,
                    IsTransmittanceTally = false,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = false,
                    IsCylindricalTally = true,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = false,
                    IsNotImplementedYet = false
                }
                },
                new ROfRhoAndTimeDetectorInput {
                Rho = new DoubleRange {
                    Start = 0,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 101
                },
                Time = new DoubleRange {
                    Start = 0,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 101
                },
                TallyType = "ROfRhoAndTime",
                Name = "ROfRhoAndTime",
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = true,
                    IsTransmittanceTally = false,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = false,
                    IsCylindricalTally = true,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = false,
                    IsNotImplementedYet = false
                }
                },
                new ROfRhoDetectorInput {
                Rho = new DoubleRange {
                    Start = 0,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 101
                },
                TallyType = "ROfRho",
                Name = "ROfRho",
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = true,
                    IsTransmittanceTally = false,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = false,
                    IsCylindricalTally = true,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = false,
                    IsNotImplementedYet = false
                }
                },
                new ROfXAndYDetectorInput {
                X = new DoubleRange {
                    Start = -100,
                    Stop = 100,
                    Delta = 10,
                    Count = 21
                },
                Y = new DoubleRange {
                    Start = -100,
                    Stop = 100,
                    Delta = 10,
                    Count = 21
                },
                TallyType = "ROfXAndY",
                Name = "ROfXAndY",
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = true,
                    IsTransmittanceTally = false,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = false,
                    IsCylindricalTally = false,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = false,
                    IsNotImplementedYet = false
                }
                },
                new RSpecularDetectorInput {
                TallyType = "RSpecular",
                Name = "RSpecular",
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = false,
                    IsTransmittanceTally = false,
                    IsSpecularReflectanceTally = true,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = false,
                    IsCylindricalTally = false,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = false,
                    IsNotImplementedYet = false
                }
                },
                new TDiffuseDetectorInput {
                TallyType = "TDiffuse",
                Name = "TDiffuse",
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = false,
                    IsTransmittanceTally = true,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = false,
                    IsCylindricalTally = false,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = false,
                    IsNotImplementedYet = false
                }
                },
                new TOfAngleDetectorInput {
                Angle = new DoubleRange {
                    Start = 0,
                    Stop = 1.5707963267949,
                    Delta = 0.392699081698724,
                    Count = 5
                },
                TallyType = "TOfAngle",
                Name = "TOfAngle",
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = false,
                    IsTransmittanceTally = true,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = false,
                    IsCylindricalTally = false,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = false,
                    IsNotImplementedYet = false
                }
                },
                new TOfRhoAndAngleDetectorInput {
                Rho = new DoubleRange {
                    Start = 0,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 101
                },
                Angle = new DoubleRange {
                    Start = 0,
                    Stop = 1.5707963267949,
                    Delta = 0.392699081698724,
                    Count = 5
                },
                TallyType = "TOfRhoAndAngle",
                Name = "TOfRhoAndAngle",
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = false,
                    IsTransmittanceTally = true,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = false,
                    IsCylindricalTally = true,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = false,
                    IsNotImplementedYet = false
                }
                },
                new TOfRhoDetectorInput {
                Rho = new DoubleRange {
                    Start = 0,
                    Stop = 10,
                    Delta = 0.1,
                    Count = 101
                },
                TallyType = "TOfRho",
                Name = "TOfRho",
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = false,
                    IsTransmittanceTally = true,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = false,
                    IsCylindricalTally = true,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = false,
                    IsNotImplementedYet = false
                }
                },
                new TOfXAndYDetectorInput {
                X = new DoubleRange {
                    Start = -100,
                    Stop = 100,
                    Delta = 10,
                    Count = 21
                },
                Y = new DoubleRange {
                    Start = -100,
                    Stop = 100,
                    Delta = 10,
                    Count = 21
                },
                TallyType = "TOfXAndY",
                Name = "TOfXAndY",
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = false,
                    IsTransmittanceTally = true,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = false,
                    IsCylindricalTally = false,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = false,
                    IsNotImplementedYet = false
                }
                },
                new TOfFxDetectorInput {
                Fx = new DoubleRange {
                    Start = 0,
                    Stop = 0.5,
                    Delta = 0.01,
                    Count = 51
                },
                TallyType = "TOfFx",
                Name = "TOfFx",
                TallySecondMoment = false,
                TallyDetails = new TallyDetails {
                    IsReflectanceTally = false,
                    IsTransmittanceTally = true,
                    IsSpecularReflectanceTally = false,
                    IsInternalSurfaceTally = false,
                    IspMCReflectanceTally = false,
                    IsDosimetryTally = false,
                    IsVolumeTally = false,
                    IsCylindricalTally = false,
                    IsNotImplementedForDAW = false,
                    IsNotImplementedForCAW = false,
                    IsNotImplementedYet = false
                }
                }
            };
        }
    }
}
