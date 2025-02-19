using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Tally for radiance as a function of Rho and Z and Angle (theta).
    /// This works for Analog and DAW processing.
    /// </summary>
    public class RadianceOfRhoAndZAndAngleDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for radiance as a function of rho, z and angle detector input
        /// </summary>
        public RadianceOfRhoAndZAndAngleDetectorInput()
        {
            TallyType = "RadianceOfRhoAndZAndAngle";
            Name = "RadianceOfRhoAndZAndAngle";
            Rho = new DoubleRange(0.0, 10, 101);
            Z = new DoubleRange(0.0, 10, 101);
            Angle = new DoubleRange(0.0, Math.PI, 5);

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsVolumeTally = true;
            TallyDetails.IsCylindricalTally = true;
            TallyDetails.IsNotImplementedForCAW = true;
        }

        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }

        /// <summary>
        /// Z binning
        /// </summary>
        public DoubleRange Z { get; set; }

        /// <summary>
        /// Angle binning
        /// </summary>
        public DoubleRange Angle { get; set; }

        /// <summary>
        /// Method to create detector from detector input
        /// </summary>
        /// <returns>created IDetector</returns>
        public IDetector CreateDetector()
        {
            return new RadianceOfRhoAndZAndAngleDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                Rho = this.Rho,
                Z = this.Z,
                Angle = this.Angle
            };
        }
    }
    /// <summary>
    /// Implements IDetector.  Tally for radiance as a function  of Rho and Z and Angle.
    /// This implementation works for Analog, DAW processing.
    /// </summary>
    public class RadianceOfRhoAndZAndAngleDetector : Detector, IHistoryDetector
    {
        private Func<PhotonDataPoint, PhotonDataPoint, int, double> _absorptionWeightingMethod;
        private ITissue _tissue;
        private IList<OpticalProperties> _ops;
        private double[,,] _tallyForOnePhoton;

        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// Z binning
        /// </summary>
        public DoubleRange Z { get; set; }
        /// <summary>
        /// Angle binning
        /// </summary>
        public DoubleRange Angle { get; set; }

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
        /// number of Zs detector gets tallied to
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
            Mean = Mean ?? new double[Rho.Count - 1, Z.Count - 1, Angle.Count - 1];
            SecondMoment = SecondMoment ?? (TallySecondMoment ? new double[Rho.Count - 1, Z.Count - 1, Angle.Count - 1] : null);

            // initialize any other necessary class fields here
            _absorptionWeightingMethod = AbsorptionWeightingMethods.GetVolumeAbsorptionWeightingMethod(tissue, this);
            _tissue = tissue;
            _ops = _tissue.Regions.Select(r => r.RegionOP).ToArray();
            _tallyForOnePhoton = _tallyForOnePhoton ?? (TallySecondMoment ? new double[Rho.Count - 1, Z.Count - 1, Angle.Count - 1] : null);
        }

        /// <summary>
        /// method to tally given two consecutive photon data points
        /// </summary>
        /// <param name="previousDP">previous data point</param>
        /// <param name="dp">current data point</param>
        /// <param name="currentRegionIndex">index of region photon current is in</param>
        public void TallySingle(PhotonDataPoint previousDP, PhotonDataPoint dp, int currentRegionIndex)
        {
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            var iz = DetectorBinning.WhichBin(dp.Position.Z, Z.Count - 1, Z.Delta, Z.Start);
            // using Acos, -1<Uz<1 goes to pi<theta<0, so first bin is most forward directed angle
            var ia = DetectorBinning.WhichBin(Math.Acos(dp.Direction.Uz), Angle.Count - 1, Angle.Delta, Angle.Start);

            var weight = _absorptionWeightingMethod(previousDP, dp, currentRegionIndex);

            var regionIndex = currentRegionIndex;

            if (weight == 0.0) return;
            Mean[ir, iz, ia] += weight / _ops[regionIndex].Mua;
            TallyCount++;
            if (!TallySecondMoment) return;
            _tallyForOnePhoton[ir, iz, ia] += weight / _ops[regionIndex].Mua;
        }
        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            // second moment is calculated AFTER the entire photon biography has been processed
            if (TallySecondMoment)
            {
                Array.Clear(_tallyForOnePhoton, 0, _tallyForOnePhoton.Length);
            }
            var previousDp = photon.History.HistoryData.First();
            foreach (var dp in photon.History.HistoryData.Skip(1))
            {
                TallySingle(previousDp, dp, _tissue.GetRegionIndex(dp.Position)); // unoptimized version, but HistoryDataController calls this once
                previousDp = dp;
            }
            // second moment determined after all tallies to each detector bin for ONE photon has been complete
            if (!TallySecondMoment) return;
            for (var ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (var iz = 0; iz < Z.Count - 1; iz++)
                {
                    for (var ia = 0; ia < Angle.Count - 1; ia++)
                    {
                        SecondMoment[ir, iz, ia] += _tallyForOnePhoton[ir, iz, ia] * _tallyForOnePhoton[ir, iz, ia];
                    }
                }
            }
        }

        /// <summary>
        /// method to normalize detector results after all photons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta * Z.Delta * 2.0 * Math.PI * Angle.Delta;
            for (var ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (var iz = 0; iz < Z.Count - 1; iz++)
                {
                    for (var ia = 0; ia < Angle.Count - 1; ia++)
                    {
                        var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * Math.Sin((ia+0.5)*Angle.Delta) * normalizationFactor;
                        Mean[ir, iz, ia] /= areaNorm * numPhotons;
                        if (!TallySecondMoment) continue;
                        SecondMoment[ir, iz, ia] /= areaNorm * areaNorm * numPhotons;
                    }
                }
            }
        }
        /// <summary>
        /// this is to allow saving of large arrays separately as a binary file
        /// </summary>
        /// <returns>none</returns>
        public BinaryArraySerializer[] GetBinarySerializers()
        {
            return new[] {
                new BinaryArraySerializer {
                    DataArray = Mean,
                    Name = "Mean",
                    FileTag = "",
                    WriteData = binaryWriter => {
                        for (var i = 0; i < Rho.Count - 1; i++) {
                            for (var j = 0; j < Z.Count - 1; j++) {
                                for (var k = 0; k < Angle.Count - 1; k++)
                                {
                                    binaryWriter.Write(Mean[i, j, k]);
                                }
                            }
                        }
                    },
                    ReadData = binaryReader => {
                        Mean = Mean ?? new double[ Rho.Count - 1, Z.Count - 1, Angle.Count -1];
                        for (var i = 0; i <  Rho.Count - 1; i++) {
                            for (var j = 0; j < Z.Count - 1; j++) {
                                for (var k = 0; k < Angle.Count - 1; k++)
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
                        for (var i = 0; i < Rho.Count - 1; i++) {
                            for (var j = 0; j < Z.Count - 1; j++) {
                                for (var k = 0; k < Angle.Count - 1; k++)
                                {
                                    binaryWriter.Write(SecondMoment[i, j, k]);
                                }
                            }                            
                        }
                    },
                    ReadData = binaryReader => {
                        if (!TallySecondMoment || SecondMoment == null) return;
                        SecondMoment = new double[ Rho.Count - 1, Z.Count - 1, Angle.Count - 1];
                        for (var i = 0; i < Rho.Count - 1; i++) {
                            for (var j = 0; j < Z.Count - 1; j++) {
                                for (var k = 0; k < Angle.Count - 1; k++)
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
        /// Method to determine if photon is within detector
        /// </summary>
        /// <param name="photon">photon</param>
        /// <returns>method always returns true</returns>
        public bool IsWithinDetectorAperture(Photon photon)
        {
            return true; // or, possibly test for NA or confined position, etc
        }

    }
}
