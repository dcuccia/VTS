﻿using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class LambertianSurfaceEmittingTubeSource : SurfaceEmittingTubeSourceBase
    {      

        /// <summary>
        /// Returns an instance of Lambertian Surface Emitting tube Source with source axis rotation and translation
        /// </summary>
        /// <param name="tubeRadius">Tube radius</param>
        /// <param name="tubeHeightZ">Tube height</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        public LambertianSurfaceEmittingTubeSource(
            double tubeRadius,
            double tubeHeightZ,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null)
            : base(
            tubeRadius,
            tubeHeightZ,
            newDirectionOfPrincipalSourceAxis,
            translationFromOrigin)
        {
            if (newDirectionOfPrincipalSourceAxis == null)
                newDirectionOfPrincipalSourceAxis = SourceDefaults.DefaultDirectionOfPrincipalSourceAxis;
            if (translationFromOrigin == null)
                translationFromOrigin = SourceDefaults.DefaultPosition;
        }
    }
}
