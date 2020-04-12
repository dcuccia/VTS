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
    public class IsotropicVolumetricEllipsoidalSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of IsotropicVolumetricEllipsoidalSourceInput class
        /// </summary>
        /// <param name="aParameter">"a" parameter of the ellipsoid source</param>
        /// <param name="bParameter">"b" parameter of the ellipsoid source</param>
        /// <param name="cParameter">"c" parameter of the ellipsoid source</param>
        /// <param name="beamDiameterFWHM">Beam diameter FWHM (-1 for flat beam)</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public IsotropicVolumetricEllipsoidalSourceInput(
            double aParameter,
            double bParameter,
            double cParameter,
            double beamDiameterFWHM,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
            SourceType = "IsotropicVolumetricEllipsoidal";
            AParameter = aParameter;
            BParameter = bParameter;
            CParameter = cParameter;
            BeamDiameterFWHM = beamDiameterFWHM;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of IsotropicVolumetricEllipsoidalSourceInput class
        /// </summary>
        /// <param name="aParameter">"a" parameter of the ellipsoid source</param>
        /// <param name="bParameter">"b" parameter of the ellipsoid source</param>
        /// <param name="cParameter">"c" parameter of the ellipsoid source</param>
        /// <param name="beamDiameterFWHM">Beam diameter FWHM (-1 for flat beam)</param>
        public IsotropicVolumetricEllipsoidalSourceInput(
            double aParameter,
            double bParameter,
            double cParameter,
            double beamDiameterFWHM)
            : this(
                aParameter,
                bParameter,
                cParameter,
                beamDiameterFWHM,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Initializes the default constructor of IsotropicVolumetricEllipsoidalSourceInput class
        /// </summary>
        public IsotropicVolumetricEllipsoidalSourceInput()
            : this(
                1.0,
                1.0,
                2.0,
                -1.0, // flat profile
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Ellipsoidal source type
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// "a" parameter of the ellipsoid source
        /// </summary>
        public double AParameter { get; set; }
        /// <summary>
        /// "b" parameter of the ellipsoid source
        /// </summary>
        public double BParameter { get; set; }
        /// <summary>
        /// "c" parameter of the ellipsoid source
        /// </summary>
        public double CParameter { get; set; }
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

            return new IsotropicVolumetricEllipsoidalSource(
                this.AParameter,
                this.BParameter,
                this.CParameter,
                this.BeamDiameterFWHM,
                this.NewDirectionOfPrincipalSourceAxis,
                this.TranslationFromOrigin,
                this.InitialTissueRegionIndex) { Rng = rng };
        }
    }

    /// <summary>
    /// Implements IsotropicVolumetricEllipsoidalSource with a,b and c parameters, 
    /// source profile, direction, position, and initial tissue region index.
    /// </summary>
    public class IsotropicVolumetricEllipsoidalSource : VolumetricEllipsoidalSourceBase
    {

        /// <summary>
        /// Returns an instance of  Isotropic Ellipsoidal Source with a given source profile (Flat/Gaussian), 
        ///  new source axis direction, and translation.
        /// </summary>
        /// <param name="aParameter">"a" parameter of the ellipsoid source</param>
        /// <param name="bParameter">"b" parameter of the ellipsoid source</param>
        /// <param name="cParameter">"c" parameter of the ellipsoid source</param>
        /// <param name="beamDiameterFWHM">Beam diameter FWHM (-1 for flat beam)</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public IsotropicVolumetricEllipsoidalSource(
            double aParameter,
            double bParameter,
            double cParameter,
            double beamDiameterFWHM,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null,
            int initialTissueRegionIndex = 0)
            : base(
                aParameter,
                bParameter,
                cParameter,
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
