using System;
using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.Controllers;

namespace Vts.MonteCarlo.Factories
{
    public static class DetectorControllerFactory
    {
        /// <summary>
        /// method that returns an IDetectorController given the VBType, list of detectors and tissue
        /// </summary>
        /// <param name="virtualBoundaryType">virtual boundary type</param>
        /// <param name="detectors">IEnumerable of IDetector</param>
        /// <param name="tissue">ITissue</param>
        /// <returns>IDetectorController</returns>
        public static IDetectorController GetDetectorController(VirtualBoundaryType virtualBoundaryType, IEnumerable<IDetector> detectors, ITissue tissue)
        {
            switch (virtualBoundaryType)
            {
                case VirtualBoundaryType.DiffuseReflectance:
                case VirtualBoundaryType.DiffuseTransmittance:
                case VirtualBoundaryType.SpecularReflectance:
                case VirtualBoundaryType.SurfaceRadiance:
                case VirtualBoundaryType.pMCDiffuseReflectance:
                    return new DetectorController(detectors);
                case VirtualBoundaryType.GenericVolumeBoundary:
                    return new HistoryDetectorController((from d in detectors where d is IHistoryDetector select (IHistoryDetector)d).ToList(), tissue);
                default:
                    throw new ArgumentOutOfRangeException("virtualBoundaryType"); 
            }
        }
    }
}