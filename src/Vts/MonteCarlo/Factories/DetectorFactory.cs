using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Instantiates appropriate detector tally given TallyType.
    /// </summary>
    public class DetectorFactory
    {
        /// <summary>
        /// Method to instantiate all detectors given list of IDetectorInputs.  This method calls
        /// the method below that instantiates a single detector.
        /// </summary>
        /// <param name="detectorInputs">IEnumerable of IDetectorInput</param>
        /// <param name="tissue">ITissue</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment or not</param>
        /// <returns>List of IDetector</returns>
        public static IList<IDetector> GetDetectors(IEnumerable<IDetectorInput> detectorInputs, ITissue tissue)
        {
            if (detectorInputs == null)
            {
                return null;
            }
            
            var detectors = detectorInputs.Select(detectorInput => GetDetector(detectorInput, tissue)).ToList();

            return detectors;
        }
        
        /// <summary>
        /// Method to instantiate a single IDetectorInput.  This method is called by
        /// the method below that instantiates a list of detectors.
        /// </summary>
        /// <param name="detectorInputs">IEnumerable of IDetectorInput</param>
        /// <param name="tissue">ITissue</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment or not</param>
        /// <returns>List of IDetector</returns>
        public static IDetector GetDetector(IDetectorInput detectorInput, ITissue tissue)
        {
            if (detectorInput == null)
            {
                return null;
            }

            var detector = detectorInput.CreateDetector();

            detector.Initialize(tissue);

            return detector;
        }
    }
}
