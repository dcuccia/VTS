﻿using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for DirectionalRectangularSource implementation 
    /// including converging/diverging angle, length, width, source profile, direction, position, 
    /// inward normal beam rotation and initial tissue region index.
    /// </summary>
    public class DirectionalRectangularSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of the DirectionalRectangularSourceInput class
        /// </summary>
        /// <param name="thetaConvOrDiv">Covergence or Divergance Angle {= 0, for a collimated beam}</param>
        /// <param name="rectLengthX">The length of the Rectangular Source</param>
        /// <param name="rectWidthY">The width of the Rectangular Source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="beamRotationFromInwardNormal">beam rotation angle</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public DirectionalRectangularSourceInput(
            double thetaConvOrDiv,
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            PolarAzimuthalAngles beamRotationFromInwardNormal,
            int initialTissueRegionIndex)
        {
            SourceType = "DirectionalRectangular";
            ThetaConvOrDiv = thetaConvOrDiv;
            RectLengthX = rectLengthX;
            RectWidthY = rectWidthY;
            SourceProfile = sourceProfile;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            BeamRotationFromInwardNormal = beamRotationFromInwardNormal;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of the DirectionalRectangularSourceInput class
        /// </summary>
        /// <param name="thetaConvOrDiv">Covergence or Divergance Angle {= 0, for a collimated beam}</param>
        /// <param name="rectLengthX">The length of the Rectangular Source</param>
        /// <param name="rectWidthY">The width of the Rectangular Source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        public DirectionalRectangularSourceInput(
            double thetaConvOrDiv,
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile)
            : this(
                thetaConvOrDiv,
                rectLengthX,
                rectWidthY,
                sourceProfile,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone(),
                0) { }

        /// <summary>
        /// Initializes a new instance of the DirectionalRectangularSourceInput class
        /// </summary>
        public DirectionalRectangularSourceInput()
            : this(
                0.0,
                1.0,
                2.0,
                new FlatSourceProfile(),
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone(),
                0) { }

        /// <summary>
        /// Covergence or Divergance Angle {= 0, for a collimated beam}
        /// </summary>
        public double ThetaConvOrDiv { get; set; }
        /// <summary>
        /// Rectangular source type
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// The length of the Rectangular Source
        /// </summary>
        public double RectLengthX { get; set; }
        /// <summary>
        /// The width of the Rectangular Source
        /// </summary>
        public double RectWidthY { get; set; }
        /// <summary>
        /// Source profile type
        /// </summary>
        public ISourceProfile SourceProfile { get; set; }
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

            return new DirectionalRectangularSource(
                this.ThetaConvOrDiv,
                this.RectLengthX,
                this.RectWidthY,
                this.SourceProfile,
                this.NewDirectionOfPrincipalSourceAxis,
                this.TranslationFromOrigin,
                this.BeamRotationFromInwardNormal,
                this.InitialTissueRegionIndex) { Rng = rng };
        }
    }

    /// <summary>
    /// Implements DirectionalRectangularSource with converging/diverging angle, length, width,
    /// source profile, direction, position, inward normal beam rotation and initial tissue 
    /// region index.
    /// </summary>
    public class DirectionalRectangularSource : RectangularSourceBase
    {
      private double _thetaConvOrDiv;  //convergence:positive, divergence:negative, collimated:zero;

        /// <summary>
        /// Returns an instance of directional (diverging/converging/collimated) Rectangular Source with specified length and width, 
        /// source profile (Flat/Gaussian), polar and azimuthal angle range, new source axis direction, translation, and  inward normal ray rotation
        /// </summary>
        /// <param name="thetaConvOrDiv">Covergence or Divergance Angle {= 0, for a collimated beam}</param>
        /// <param name="rectLengthX">The length of the Rectangular Source</param>
        /// <param name="rectWidthY">The width of the Rectangular Source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>    
        /// <param name="beamRotationFromInwardNormal">Polar Azimuthal Rotational Angle of inward Normal</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public DirectionalRectangularSource(
            double thetaConvOrDiv,
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null,
            PolarAzimuthalAngles beamRotationFromInwardNormal = null,
            int initialTissueRegionIndex = 0)
            : base(
                rectLengthX,
                rectWidthY,
                sourceProfile,
                newDirectionOfPrincipalSourceAxis,
                translationFromOrigin,
                beamRotationFromInwardNormal,
                initialTissueRegionIndex)
        {
            _thetaConvOrDiv = thetaConvOrDiv;
            if (newDirectionOfPrincipalSourceAxis == null)
                newDirectionOfPrincipalSourceAxis = SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone();
            if (translationFromOrigin == null)
                translationFromOrigin = SourceDefaults.DefaultPosition.Clone();
            if (beamRotationFromInwardNormal == null)
                beamRotationFromInwardNormal = SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone();
        }

        /// <summary>
        /// Returns direction for a given position
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>new direction</returns>  
        protected override Direction GetFinalDirection(Position position)
        {
            

            if ((_rectLengthX == 0.0) && (_rectWidthY == 0.0))
                return (SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(
                            new DoubleRange(0.0, Math.Abs(_thetaConvOrDiv)),
                            SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                            Rng));
            else
            {
                // sign is negative for diverging and positive positive for converging 
                var polarAngle = SourceToolbox.UpdatePolarAngleForDirectionalSources(
                    _rectLengthX,
                    Math.Sqrt(position.X * position.X + position.Y * position.Y),
                    _thetaConvOrDiv);              
                return (SourceToolbox.GetDirectionForGiven2DPositionAndGivenPolarAngle(polarAngle, position));
            }
        }
    }
}
