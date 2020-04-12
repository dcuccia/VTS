﻿using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Abstract class for LineSourceBase
    /// </summary>
    public abstract class LineSourceBase : ISource
    {
        /// <summary>
        /// Source beam diameter FWHM (-1 for flat beam)
        /// </summary>
        protected double _beamDiameterFWHM;
        /// <summary>
        /// New source axis direction
        /// </summary>
        protected Direction _newDirectionOfPrincipalSourceAxis;
        /// <summary>
        /// New source location
        /// </summary>
        protected Position _translationFromOrigin;
        /// <summary>
        /// Beam rotation from inward normal
        /// </summary>
        protected PolarAzimuthalAngles _beamRotationFromInwardNormal;  
        /// <summary>
        /// Source rotation and translation flags
        /// </summary>
        protected SourceFlags _rotationAndTranslationFlags;
        /// <summary>
        /// The length of the line source
        /// </summary>
        protected double _lineLength;
        /// <summary>
        /// Initial tissue region index
        /// </summary>
        protected int _initialTissueRegionIndex;

        /// <summary>
        /// Defines LineSourceBase class
        /// </summary>
        /// <param name="lineLength">The length of the line source</param>
        /// <param name="beamDiameterFWHM">Beam diameter FWHM (-1 for flat beam)</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="beamRotationFromInwardNormal">Beam rotation from inward normal</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        protected LineSourceBase(
            double lineLength,
            double beamDiameterFWHM,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            PolarAzimuthalAngles beamRotationFromInwardNormal,
            int initialTissueRegionIndex)
        {
            if (newDirectionOfPrincipalSourceAxis == null)
                newDirectionOfPrincipalSourceAxis = SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone();
            if (translationFromOrigin == null)
                translationFromOrigin = SourceDefaults.DefaultPosition.Clone();
            if (beamRotationFromInwardNormal == null)
                beamRotationFromInwardNormal = SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone();

            _rotationAndTranslationFlags = new SourceFlags(
                newDirectionOfPrincipalSourceAxis != SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                translationFromOrigin != SourceDefaults.DefaultPosition.Clone(),
                beamRotationFromInwardNormal != SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone());

            _lineLength = lineLength;
            _beamDiameterFWHM = beamDiameterFWHM;
            _newDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis.Clone();
            _translationFromOrigin = translationFromOrigin.Clone();
            _beamRotationFromInwardNormal = beamRotationFromInwardNormal.Clone();
            _initialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Implements Get next photon
        /// </summary>
        /// <param name="tissue">tissue</param>
        /// <returns>photon</returns>
        public Photon GetNextPhoton(ITissue tissue)
        {
            //Source starts from anywhere in the line
            Position finalPosition = GetFinalPosition(_beamDiameterFWHM, _lineLength, Rng);

            // sample angular distribution
            Direction finalDirection = GetFinalDirection(finalPosition);

            //Find the relevent polar and azimuthal pair for the direction
            PolarAzimuthalAngles _rotationalAnglesOfPrincipalSourceAxis = SourceToolbox.GetPolarAzimuthalPairFromDirection(_newDirectionOfPrincipalSourceAxis);

            //Rotation and translation
            SourceToolbox.UpdateDirectionPositionAfterGivenFlags(
                ref finalPosition,
                ref finalDirection,
                _rotationalAnglesOfPrincipalSourceAxis,
                _translationFromOrigin,
                _beamRotationFromInwardNormal,
                _rotationAndTranslationFlags);

            var photon = new Photon(finalPosition, finalDirection, tissue, _initialTissueRegionIndex, Rng);

            return photon;
        }

        /// <summary>
        /// Returns final direction for a given position
        /// </summary>
        /// <param name="position">Current position</param>
        /// <returns>new direction</returns>
        protected abstract Direction GetFinalDirection(Position position); // position may or may not be needed

        private static Position GetFinalPosition(double beamDiameterFWHM, double lineLength, Random rng)
        {
            Position finalPosition = SourceDefaults.DefaultPosition.Clone();
            return beamDiameterFWHM < 0.0
                ? SourceToolbox.GetPositionInALineRandomFlat(
                        finalPosition,
                        lineLength,
                        rng)
                : SourceToolbox.GetPositionInALineRandomGaussian(
                        finalPosition,
                        0.5*lineLength,
                        beamDiameterFWHM,
                        rng);
        }

        #region Random number generator code (copy-paste into all sources)
        /// <summary>
        /// The random number generator used to create photons. If not assigned externally,
        /// a Mersenne Twister (MathNet.Numerics.Random.MersenneTwister) will be created with
        /// a seed of zero.
        /// </summary>
        public Random Rng
        {
            get
            {
                if (_rng == null)
                {
                    _rng = new MathNet.Numerics.Random.MersenneTwister(0);
                }
                return _rng;
            }
            set { _rng = value; }
        }
        private Random _rng;
        #endregion
    }
}
