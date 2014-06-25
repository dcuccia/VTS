using System;
using System.Runtime.Serialization;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Tally for diffuse reflectance.
    /// This implementation works for Analog, DAW and CAW.
    /// </summary>
    public class TDiffuseDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for diffuse reflectance detector input
        /// </summary>
        public TDiffuseDetectorInput()
        {
            TallyType = "TDiffuse";
            Name = "TDiffuse";

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsTransmittanceTally = true;
        }

        public IDetector CreateDetector()
        {
            return new ROfRhoDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
            };
        }
    }

    /// <summary>
    /// Implements IDetector.  Tally for diffuse transmittance.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class TDiffuseDetector : Detector, IDetector
    {
        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */

        /* ==== Place user-defined output arrays here. They should be prepended with "[IgnoreDataMember]" attribute ==== */
        /* ==== Then, GetBinaryArrays() should be implemented to save them separately in binary format ==== */
        /// <summary>
        /// detector mean
        /// </summary>
        [IgnoreDataMember]
        public double Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public double SecondMoment { get; set; }

        /* ==== Place optional/user-defined output properties here. They will be saved in text (JSON) format ==== */
        /// <summary>
        /// number of times detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }

        public void Initialize(ITissue tissue)
        {
            // assign any user-defined outputs (except arrays...we'll make those on-demand)
            TallyCount = 0;

            // if the data arrays are null, create them (only create second moment if TallySecondMoment is true)
            //Mean = Mean ?? new double();
            //SecondMoment = SecondMoment ?? (TallySecondMoment ? new double() : null);
            Mean = new double();
            if (TallySecondMoment)
            {
                SecondMoment = new double();
            }

            // intialize any other necessary class fields here
        }

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            Mean += photon.DP.Weight;
            if (TallySecondMoment)
            {
                SecondMoment += photon.DP.Weight * photon.DP.Weight;
            }
            TallyCount++;
        }

        /// <summary>
        /// method to normalize detector results after numPhotons are launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            Mean /= numPhotons;
            if (TallySecondMoment)
            {
                SecondMoment /= numPhotons;
            }
        }

        // this is to allow saving of large arrays separately as a binary file
        // NEED TO ASK DC if writing double to binary is to be done or rather write to json should be done
        public BinaryArraySerializer[] GetBinarySerializers()
        {
            return new[] {
                new BinaryArraySerializer {
                    Name = "Mean",
                    FileTag = "",
                    WriteData = binaryWriter => { binaryWriter.Write(Mean);
                    },
                    ReadData = binaryReader => {
                        //Mean = Mean ?? new double[1];
                        Mean = binaryReader.ReadDouble();
                    }
                },
                new BinaryArraySerializer {
                    Name = "SecondMoment",
                    FileTag = "_2",
                    WriteData = binaryWriter => {
                        if(!TallySecondMoment) return;
                        binaryWriter.Write(SecondMoment);
                    },
                    ReadData = binaryReader => {
                        if(!TallySecondMoment) return;
                        //SecondMoment = SecondMoment ?? new double();
                        SecondMoment = binaryReader.ReadDouble();
                    },
                },
            };
        }
        /// <summary>
        /// Method to determine if photon is within detector
        /// </summary>
        /// <param name="dp">photon data point</param>
        /// <returns>method always returns true</returns>
        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true; // or, possibly test for NA or confined position, etc
        }
    }
}
