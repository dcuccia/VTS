using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for IsotropicVolumetricCuboidalSource
    /// implementation including length, width, height, source profile, direction, position, 
    /// and initial tissue region index.
    /// </summary>
    public class IsotropicVolumetricCuboidalSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of IsotropicVolumetricCuboidalSourceInput class
        /// </summary>
        /// <param name="cubeLengthX">Length of the cuboid</param>
        /// <param name="cubeWidthY">Width of the cuboid</param>
        /// <param name="cubeHeightZ">Height of the cuboid</param>
        /// <param name="beamDiameterFWHM">Beam diameter FWHM (-1 for flat beam)</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public IsotropicVolumetricCuboidalSourceInput(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            double beamDiameterFWHM,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
            SourceType = "IsotropicVolumetricCubiodal";
            CubeLengthX = cubeLengthX;
            CubeWidthY = cubeWidthY;
            CubeHeightZ = cubeHeightZ;
            BeamDiameterFWHM = beamDiameterFWHM;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of IsotropicVolumetricCuboidalSourceInput class
        /// </summary>
        /// <param name="cubeLengthX">Length of the cuboid</param>
        /// <param name="cubeWidthY">Width of the cuboid</param>
        /// <param name="cubeHeightZ">Height of the cuboid</param>
        /// <param name="beamDiameterFWHM">Beam diameter FWHM (-1 for flat beam)</param>
        public IsotropicVolumetricCuboidalSourceInput(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            double beamDiameterFWHM)
            : this(
                cubeLengthX,
                cubeWidthY,
                cubeHeightZ,
                beamDiameterFWHM,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Initializes the default constructor of IsotropicVolumetricCuboidalSourceInput class
        /// </summary>
        public IsotropicVolumetricCuboidalSourceInput()
            : this(
                1.0,
                1.0,
                1.0,
                -1.0, // flat profile
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Volumetric Cuboidal source type
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// The length of the cube (along x axis)
        /// </summary>
        public double CubeLengthX { get; set; }
        /// <summary>
        /// The width of the cube (along y axis)
        /// </summary>
        public double CubeWidthY { get; set; }
        /// <summary>
        /// The height of the cube (along z axis)
        /// </summary>
        public double CubeHeightZ { get; set; }
        /// <summary>
        /// Source beam diameter FWHM (-1 for flat beam)
        /// </summary>
        public double BeamDiameterFWHM { get; set; }
        /// <summary>
        /// New source axis direction
        /// </summary>
        public Direction NewDirectionOfPrincipalSourceAxis { get; set; }
        /// <summary>
        /// New source location
        /// </summary>
        public Position TranslationFromOrigin { get; set; }
        /// <summary>
        /// Initial tissue region index
        /// </summary>
        public int InitialTissueRegionIndex { get; set; }

        /// <summary>
        /// Required code to create a source based on the input values
        /// </summary>
        /// <param name="rng"></param>
        /// <returns></returns>
        public ISource CreateSource(Random rng = null)
        {
            rng = rng ?? new Random();

            return new IsotropicVolumetricCuboidalSource(
                this.CubeLengthX,
                this.CubeWidthY,
                this.CubeHeightZ,
                this.BeamDiameterFWHM,
                this.NewDirectionOfPrincipalSourceAxis,
                this.TranslationFromOrigin,
                this.InitialTissueRegionIndex) { Rng = rng };
        }
    }

    /// <summary>
    /// Implements IsotropicVolumetricCuboidalSource with length, width, height, source
    /// profile, direction, position, and initial tissue region index.
    /// </summary>
    public class IsotropicVolumetricCuboidalSource : VolumetricCuboidalSourceBase
    {

        /// <summary>
        /// Returns an instance of  Isotropic Cuboidal Source with a given source profile (Flat/Gaussian), 
        /// translation, and source axis rotation
        /// </summary>
        /// <param name="cubeLengthX">The length of the cuboid</param>
        /// <param name="cubeWidthY">The width of the cuboid</param>
        /// <param name="cubeHeightZ">The height of the cuboid</param>
        /// <param name="beamDiameterFWHM">Beam diameter FWHM (-1 for flat beam)</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public IsotropicVolumetricCuboidalSource(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            double beamDiameterFWHM,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null,
            int initialTissueRegionIndex = 0)
            : base(
                cubeLengthX,
                cubeWidthY,
                cubeHeightZ,
                beamDiameterFWHM,
                newDirectionOfPrincipalSourceAxis,
                translationFromOrigin,
                initialTissueRegionIndex)
        {
        }

        /// <summary>
        /// Returns direction
        /// </summary>
        /// <returns>new direction</returns>
        protected override Direction GetFinalDirection()
        {
            return SourceToolbox.GetDirectionForIsotropicDistributionRandom(Rng);
        }
    }

}
