using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for DirectionalellipticalSource implementation 
    /// including converging/diverging angle, a and b parameters, source profile, direction, 
    /// position, inward normal beam rotation and initial tissue region index.
    /// </summary>
    public class DirectionalEllipticalSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of DirectionalEllipticalSourceInput class
        /// </summary>
        /// <param name="thetaConvOrDiv">Covergence or Divergance Angle {= 0, for a collimated beam}</param>
        /// <param name="aParameter">"a" parameter of the ellipse source</param>
        /// <param name="bParameter">"b" parameter of the ellipse source</param>
        /// <param name="beamDiameterFWHM">Beam diameter FWHM (-1 for flat beam)</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="beamRotationFromInwardNormal">beam rotation angle</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public DirectionalEllipticalSourceInput(
            double thetaConvOrDiv,
            double aParameter,
            double bParameter,
            double beamDiameterFWHM,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            PolarAzimuthalAngles beamRotationFromInwardNormal,
            int initialTissueRegionIndex)
        {
            SourceType = "DirectionalElliptical";
            ThetaConvOrDiv = thetaConvOrDiv;
            AParameter = aParameter;
            BParameter = bParameter;
            BeamDiameterFWHM = beamDiameterFWHM;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            BeamRotationFromInwardNormal = beamRotationFromInwardNormal;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of DirectionalEllipticalSourceInput class
        /// </summary>
        /// <param name="thetaConvOrDiv">Covergence or Divergance Angle {= 0, for a collimated beam}</param>
        /// <param name="aParameter">"a" parameter of the ellipse source</param>
        /// <param name="bParameter">"b" parameter of the ellipse source</param>
        /// <param name="beamDiameterFWHM">Beam diameter FWHM (-1 for flat beam)</param>
        public DirectionalEllipticalSourceInput(
            double thetaConvOrDiv,
            double aParameter,
            double bParameter,
            double beamDiameterFWHM)
            : this(
                thetaConvOrDiv,
                aParameter,
                bParameter,
                beamDiameterFWHM,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone(),
                0) { }

        /// <summary>
        /// Initializes the default constructor of DirectionalEllipticalSourceInput class
        /// </summary>
        public DirectionalEllipticalSourceInput()
            : this(
                0.0,
                1.0,
                2.0,
                -1.0,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone(),
                0) { }

        /// <summary>
        /// Covergence or Divergance Angle {= 0, for a collimated beam}
        /// </summary>
        public double ThetaConvOrDiv { get; set; }
        /// <summary>
        /// Elliptical source type
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// "a" parameter of the ellipse source
        /// </summary>
        public double AParameter { get; set; }
        /// <summary>
        /// "b" parameter of the ellipse source
        /// </summary>
        public double BParameter { get; set; }
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
        /// Beam rotation from inward normal
        /// </summary>
        public PolarAzimuthalAngles BeamRotationFromInwardNormal { get; set; }
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

            return new DirectionalEllipticalSource(
                this.ThetaConvOrDiv,
                this.AParameter,
                this.BParameter,
                this.BeamDiameterFWHM,
                this.NewDirectionOfPrincipalSourceAxis,
                this.TranslationFromOrigin,
                this.BeamRotationFromInwardNormal,
                this.InitialTissueRegionIndex) { Rng = rng };
        }
    }

    /// <summary>
    /// Implements DirectionalellipticalSource with converging/diverging angle, a and b
    /// parameters, source profile, direction, position, inward normal beam rotation and
    /// initial tissue region index.
    /// </summary>
    public class DirectionalEllipticalSource : EllipticalSourceBase
    {
        private double _thetaConvOrDiv;   //convergence:positive, divergence:negative, collimated:zero;     

        /// <summary>
        /// Returns an instance of directional (diverging/converging/collimated) Elliptical Source with specified length and width, 
        /// source profile (Flat/Gaussian), polar and azimuthal angle range, new source axis direction, translation, and  inward normal ray rotation
        /// </summary>
        /// <param name="thetaConvOrDiv">Covergence or Divergance Angle {= 0, for a collimated beam}</param>
        /// <param name="aParameter">"a" parameter of the ellipse source</param>
        /// <param name="bParameter">"b" parameter of the ellipse source</param>
        /// <param name="beamDiameterFWHM">Beam diameter FWHM (-1 for flat beam)</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>    
        /// <param name="beamRotationFromInwardNormal">Polar Azimuthal Rotational Angle of inward Normal</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public DirectionalEllipticalSource(
            double thetaConvOrDiv,
            double aParameter,
            double bParameter,
            double beamDiameterFWHM,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null,
            PolarAzimuthalAngles beamRotationFromInwardNormal = null,
            int initialTissueRegionIndex = 0)
            : base(
                aParameter,
                bParameter,
                beamDiameterFWHM,
                newDirectionOfPrincipalSourceAxis,
                translationFromOrigin,
                beamRotationFromInwardNormal,
                initialTissueRegionIndex)
        {
            _thetaConvOrDiv = thetaConvOrDiv;
        }


        /// <summary>
        /// Returns direction for a given position
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>new direction</returns>  
        protected override Direction GetFinalDirection(Position position)
        {
            if (_aParameter == 0.0 && _bParameter == 0.0)
            {
                return (SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(
                            new DoubleRange(0.0, Math.Abs(_thetaConvOrDiv)),
                            SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                            Rng));
            }

            // sign is negative for diverging and positive positive for converging 
            var polarAngle = SourceToolbox.UpdatePolarAngleForDirectionalSources(
                _aParameter,
                Math.Sqrt(position.X * position.X + position.Y * position.Y),
                _thetaConvOrDiv);
            return (SourceToolbox.GetDirectionForGiven2DPositionAndGivenPolarAngle(polarAngle, position));
        }
    }
}
