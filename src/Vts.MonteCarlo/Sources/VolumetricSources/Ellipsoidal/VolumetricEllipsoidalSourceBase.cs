using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Abstract class for VolumetricEllipsoidalSourceBase
    /// </summary>
    public abstract class VolumetricEllipsoidalSourceBase : ISource
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
        /// Source rotation and translation flags
        /// </summary>
        protected SourceFlags _rotationAndTranslationFlags;
        /// <summary>
        /// "a" parameter of the ellipsoid source
        /// </summary>
        protected double _aParameter;
        /// <summary>
        /// "b" parameter of the ellipsoid source
        /// </summary>
        protected double _bParameter;
        /// <summary>
        /// "c" parameter of the ellipsoid source
        /// </summary>
        protected double _cParameter;
        /// <summary>
        /// Initial tissue region index
        /// </summary>
        protected int _initialTissueRegionIndex;

        /// <summary>
        /// Defines VolumetricEllipsoidalSourceBase class
        /// </summary>
        /// <param name="aParameter">"a" parameter of the ellipsoid source</param>
        /// <param name="bParameter">"b" parameter of the ellipsoid source</param>
        /// <param name="cParameter">"c" parameter of the ellipsoid source</param>
        /// <param name="beamDiameterFWHM">Beam diameter FWHM (-1 for flat beam)</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        protected VolumetricEllipsoidalSourceBase(
            double aParameter,
            double bParameter,
            double cParameter,
            double beamDiameterFWHM,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
            _rotationAndTranslationFlags = new SourceFlags(
               newDirectionOfPrincipalSourceAxis != SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
               translationFromOrigin != SourceDefaults.DefaultPosition.Clone(),
               false);

            _aParameter = aParameter;
            _bParameter = bParameter;
            _cParameter = cParameter;
            _beamDiameterFWHM = beamDiameterFWHM;
            _newDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis.Clone();
            _translationFromOrigin = translationFromOrigin.Clone();
            _initialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Implements Get next photon
        /// </summary>
        /// <param name="tissue">tissue</param>
        /// <returns>photon</returns>
        public Photon GetNextPhoton(ITissue tissue)
        {
            //Source starts from anywhere in the ellipsoid
            Position finalPosition = GetFinalPosition(_beamDiameterFWHM, _aParameter, _bParameter, _cParameter, Rng);

            // sample angular distribution
            Direction finalDirection = GetFinalDirection();

            //Find the relevent polar and azimuthal pair for the direction
            PolarAzimuthalAngles _rotationalAnglesOfPrincipalSourceAxis = SourceToolbox.GetPolarAzimuthalPairFromDirection(_newDirectionOfPrincipalSourceAxis);

            //Rotation and translation
            SourceToolbox.UpdateDirectionPositionAfterGivenFlags(
                ref finalPosition,
                ref finalDirection,
                _rotationalAnglesOfPrincipalSourceAxis,
                _translationFromOrigin,
                _rotationAndTranslationFlags);

            var photon = new Photon(finalPosition, finalDirection, tissue, _initialTissueRegionIndex, Rng);

            return photon;
        }

        /// <summary>
        /// Returns direction
        /// </summary>
        /// <returns>new direction</returns>
        protected abstract Direction GetFinalDirection(); // position may or may not be needed

        private static Position GetFinalPosition(double beamDiameterFWHM, double aParameter, double bParameter, double cParameter, Random rng)
        {
            return beamDiameterFWHM < 0.0
                ? SourceToolbox.GetPositionInAnEllipsoidRandomFlat(
                    SourceDefaults.DefaultPosition.Clone(),
                    2 * aParameter,
                    2 * bParameter,
                    2 * cParameter,
                    rng)
                : SourceToolbox.GetPositionInAnEllipsoidRandomGaussian(
                    SourceDefaults.DefaultPosition.Clone(),
                    2 * aParameter,
                    2 * bParameter,
                    2 * cParameter,
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
