using System;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Tally for reflectance as a function of X and Y and Time, recessed in air.
    /// This works for Analog, DAW and CAW processing.
    /// </summary>
    public class ROfXAndYAndTimeRecessedDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for reflectance as a function of X and Y detector input
        /// </summary>
        public ROfXAndYAndTimeRecessedDetectorInput()
        {
            TallyType = "ROfXAndYAndTimeRecessed";
            Name = "ROfXAndYAndTimeRecessed";
            X = new DoubleRange(-10.0, 10.0, 101);
            Y = new DoubleRange(-10.0, 10.0, 101);
            Time = new DoubleRange(0.0, 1.0, 101);
            ZPlane = -1.0;
            NA = double.PositiveInfinity; // set default NA completely open regardless of detector region refractive index
            FinalTissueRegionIndex = 0; // assume detector is in air

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsReflectanceTally = true;
        }

        /// <summary>
        /// X binning
        /// </summary>
        public DoubleRange X { get; set; }
        /// <summary>
        /// Y binning
        /// </summary>
        public DoubleRange Y { get; set; }      
        /// <summary>
        /// Time binning
        /// </summary>
        public DoubleRange Time { get; set; }
        /// <summary>
        /// z plane above tissue in air
        /// </summary>
        public double ZPlane { get; set; }
        /// <summary>
        /// Detector region index
        /// </summary>
        public int FinalTissueRegionIndex { get; set; }

        /// <summary>
        /// detector numerical aperture
        /// </summary>
        public double NA { get; set; }

        /// <summary>
        /// Method to create detector from detector input
        /// </summary>
        /// <returns>created IDetector</returns>
        public IDetector CreateDetector()
        {
            return new ROfXAndYAndTimeRecessedDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                X = this.X,
                Y = this.Y,
                Time = this.Time,
                ZPlane = this.ZPlane,
                NA = this.NA,
                FinalTissueRegionIndex = this.FinalTissueRegionIndex
            };
        }
    }
    /// <summary>
    /// Implements IDetector.  Tally for reflectance as a function  of X and Y and Time.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class ROfXAndYAndTimeRecessedDetector : Detector, IDetector
    {
        private ITissue _tissue;

        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        /// <summary>
        /// X binning
        /// </summary>
        public DoubleRange X { get; set; }
        /// <summary>
        /// Y binning
        /// </summary>
        public DoubleRange Y { get; set; }        
        /// <summary>
        /// Time binning
        /// </summary>
        public DoubleRange Time { get; set; }
        /// <summary>
        /// z plane above tissue in air
        /// </summary>
        public double ZPlane { get; set; }
        /// <summary>
        /// Detector region index
        /// </summary>
        public int FinalTissueRegionIndex { get; set; }
        /// <summary>
        /// numerical aperture
        /// </summary>
        public double NA { get; set; }

        /* ==== Place user-defined output arrays here. They should be prepended with "[IgnoreDataMember]" attribute ==== */
        /* ==== Then, GetBinaryArrays() should be implemented to save them separately in binary format ==== */
        /// <summary>
        /// detector mean
        /// </summary>
        [IgnoreDataMember]
        public double[,,] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public double[,,] SecondMoment { get; set; }

        /* ==== Place optional/user-defined output properties here. They will be saved in text (JSON) format ==== */
        /// <summary>
        /// number of Ys detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }

        /// <summary>
        /// Method to initialize detector
        /// </summary>
        /// <param name="tissue">tissue definition</param>
        /// <param name="rng">random number generator</param>
        public void Initialize(ITissue tissue, Random rng)
        {
            // assign any user-defined outputs (except arrays...we'll make those on-demand)
            TallyCount = 0;

            // if the data arrays are null, create them (only create second moment if TallySecondMoment is true)
            Mean = Mean ?? new double[X.Count - 1, Y.Count - 1,Time.Count - 1];
            SecondMoment = SecondMoment ?? (TallySecondMoment ? new double[X.Count - 1, Y.Count - 1, Time.Count - 1] : null);

            // initialize any other necessary class fields here
            _tissue = tissue;
        }

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            if (!IsWithinDetectorAperture(photon)) return;

            // ray trace exit location and direction to location at ZPlane
            var positionAtZPlane = LayerTissueRegionToolbox.RayExtendToInfinitePlane(
                photon.DP.Position, photon.DP.Direction, ZPlane);

            var ix = DetectorBinning.WhichBin(positionAtZPlane.X, X.Count - 1, X.Delta, X.Start);
            var iy = DetectorBinning.WhichBin(positionAtZPlane.Y, Y.Count - 1, Y.Delta, Y.Start);
            var it = DetectorBinning.WhichBin(photon.DP.TotalTime, Time.Count - 1, Time.Delta, Time.Start);

            Mean[ix, iy, it] += photon.DP.Weight;
            TallyCount++;
            if (!TallySecondMoment) return;
            SecondMoment[ix, iy, it] += photon.DP.Weight * photon.DP.Weight;
        }
        /// <summary>
        /// method to normalize detector results after all photons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = X.Delta * Y.Delta * Time.Delta;
            for (var ix = 0; ix < X.Count - 1; ix++)
            {
                for (var iy = 0; iy < Y.Count - 1; iy++)
                {
                    for (var it = 0; it < Time.Count - 1; it++)
                    {
                        Mean[ix, iy, it] /= normalizationFactor * numPhotons;
                        if (TallySecondMoment)
                        {
                            SecondMoment[ix, iy, it] /= normalizationFactor * normalizationFactor * numPhotons;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// this is to allow saving of large arrays separately as a binary file
        /// </summary>
        /// <returns>BinaryArraySerializer[]</returns>
        public BinaryArraySerializer[] GetBinarySerializers()
        {
            return new[] {
                new BinaryArraySerializer {
                    DataArray = Mean,
                    Name = "Mean",
                    FileTag = "",
                    WriteData = binaryWriter => {
                        for (var i = 0; i < X.Count - 1; i++) {
                            for (var j = 0; j < Y.Count - 1; j++) {
                                for (var k = 0; k < Time.Count - 1; k++)
                                {
                                    binaryWriter.Write(Mean[i, j, k]);
                                }
                            }
                        }
                    },
                    ReadData = binaryReader => {
                        Mean = Mean ?? new double[ X.Count - 1, Y.Count - 1, Time.Count - 1];
                        for (var i = 0; i <  X.Count - 1; i++) {
                            for (var j = 0; j < Y.Count - 1; j++) {
                                for (var k = 0; k < Time.Count - 1; k++)
                                {
                                   Mean[i, j, k] = binaryReader.ReadDouble();
                                }
                            }
                        }
                    }
                },
                // return a null serializer, if we're not serializing the second moment
                !TallySecondMoment ? null :  new BinaryArraySerializer {
                    DataArray = SecondMoment,
                    Name = "SecondMoment",
                    FileTag = "_2",
                    WriteData = binaryWriter => {
                        if (!TallySecondMoment || SecondMoment == null) return;
                        for (var i = 0; i < X.Count - 1; i++) {
                            for (var j = 0; j < Y.Count - 1; j++) {
                                for (var k = 0; k < Time.Count - 1; k++)
                                {
                                    binaryWriter.Write(SecondMoment[i, j, k]);
                                }
                            }
                        }
                    },
                    ReadData = binaryReader => {
                        if (!TallySecondMoment || SecondMoment == null) return;
                        SecondMoment = new double[ X.Count - 1, Y.Count - 1, Time.Count - 1];
                        for (var i = 0; i < X.Count - 1; i++) {
                            for (var j = 0; j < Y.Count - 1; j++) {
                                for (var k = 0; k < Time.Count - 1; k++)
                                {
                                    SecondMoment[i, j, k] = binaryReader.ReadDouble();
                                }
                            }
			            }
                    },
                },
            };
        }
        /// <summary>
        /// Method to determine if photon is within detector NA
        /// </summary>
        /// <param name="photon">photon</param>
        /// <returns>Boolean indicating whether photon is within detector</returns>
        public bool IsWithinDetectorAperture(Photon photon)
        {
            if (photon.CurrentRegionIndex == FinalTissueRegionIndex)
            {
                var detectorRegionN = _tissue.Regions[photon.CurrentRegionIndex].RegionOP.N;
                return photon.DP.IsWithinNA(NA, Direction.AlongNegativeZAxis, detectorRegionN);
            }
            else // determine n of prior tissue region
            {
                var detectorRegionN = _tissue.Regions[FinalTissueRegionIndex].RegionOP.N;
                return photon.History.PreviousDP.IsWithinNA(NA, Direction.AlongNegativeZAxis, detectorRegionN);
            }
        }

    }
}
