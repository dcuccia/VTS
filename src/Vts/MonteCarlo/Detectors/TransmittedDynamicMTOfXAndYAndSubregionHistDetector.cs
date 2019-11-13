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
    /// Tally for Transmited dynamic MT as a function of X and Y using blood volume fraction in each tissue region.
    /// This detector also tallies the total and dynamic MT as a function of Z.   If a random number is less
    /// than blood volume fraction for the tissue region in which the collision occurred, then hit blood and considered
    /// "dynamic" event.  Otherwise, it is a "static" event.
    /// This works for Analog and DAW processing.
    /// </summary>
    public class TransmittedDynamicMTOfXAndYAndSubregionHistDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for TransmittedMT as a function of rho and tissue subregion detector input
        /// </summary>
        public TransmittedDynamicMTOfXAndYAndSubregionHistDetectorInput()
        {
            TallyType = "TransmittedDynamicMTOfXAndYAndSubregionHist";
            Name = "TransmittedDynamicMTOfXAndYAndSubregionHist";
            X = new DoubleRange(-10.0, 10.0, 101);
            Y = new DoubleRange(-10.0, 10.0, 101);
            Z = new DoubleRange(0.0, 10.0, 51);
            BloodVolumeFraction = new List<double>() { 0, 0.5, 0.5, 0 };
            MTBins = new DoubleRange(0.0, 500.0, 51);
            FractionalMTBins = new DoubleRange(0.0, 1.0, 11);

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsTransmittanceTally = true;
            TallyDetails.IsCylindricalTally = true;
        }

        /// <summary>
        /// x binning
        /// </summary>
        public DoubleRange X { get; set; }
        /// <summary>
        /// y binning
        /// </summary>
        public DoubleRange Y { get; set; }
        /// <summary>
        /// z binning
        /// </summary>
        public DoubleRange Z { get; set; }
        /// <summary>
        /// subregion blood volume fraction
        /// </summary>
        public IList<double> BloodVolumeFraction { get; set; }
        /// <summary>
        /// momentum transfer binning
        /// </summary>
        public DoubleRange MTBins { get; set; }
        /// <summary>
        /// fractional momentum transfer binning
        /// </summary>
        public DoubleRange FractionalMTBins { get; set; }
        /// <summary>
        /// number of dynamic and static collisions in each subregion
        /// </summary>
        [IgnoreDataMember]
        public double[,] SubregionCollisions { get; set; }
        
        public IDetector CreateDetector()
        {
            return new TransmittedDynamicMTOfXAndYAndSubregionHistDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                X = this.X,
                Y = this.Y,
                Z = this.Z,
                MTBins = this.MTBins,
                BloodVolumeFraction = this.BloodVolumeFraction,
                FractionalMTBins = this.FractionalMTBins
            };
        }
    }

    /// <summary>
    /// Implements IDetector.  Tally for momentum transfer as a function  of X, Y and tissue subregion
    /// using blood volume fraction in each tissue subregion.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class TransmittedDynamicMTOfXAndYAndSubregionHistDetector : Detector, IDetector
    {
        private ITissue _tissue;
        //private IList<OpticalProperties> _ops;
        private IList<double> _bloodVolumeFraction;
        private Random _rng;

        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        /// <summary>
        /// x binning
        /// </summary>
        public DoubleRange X { get; set; }
        /// <summary>
        /// y binning
        /// </summary>
        public DoubleRange Y { get; set; }
        /// <summary>
        /// z binning
        /// </summary>
        public DoubleRange Z { get; set; }
        /// <summary>
        /// momentum transfer binning
        /// </summary>
        public DoubleRange MTBins { get; set; }
        /// <summary>
        /// subregion blood volume fraction
        /// </summary>
        public IList<double> BloodVolumeFraction { get; set; }
        /// <summary>
        /// fractional momentum transfer binning
        /// </summary>
        public DoubleRange FractionalMTBins { get; set; }

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
        /// <summary>
        /// total MT as a function of Z multiplied by final photon weight
        /// </summary>
        [IgnoreDataMember]
        public double[, ,] TotalMTOfZ { get; set; }
        /// <summary>
        /// total MT Second Moment as a function of Z multiplied by final photon weight
        /// </summary>
        [IgnoreDataMember]
        public double[, ,] TotalMTOfZSecondMoment { get; set; }
        /// <summary>
        /// dynamic MT as a function of Z multiplied by final photon weight
        /// </summary>
        [IgnoreDataMember]
        public double[, ,] DynamicMTOfZ { get; set; }
        /// <summary>
        /// dynamic MT Second Moment as a function of Z multiplied by final photon weight
        /// </summary>
        [IgnoreDataMember]
        public double[, ,] DynamicMTOfZSecondMoment { get; set; }
        /// <summary>
        /// fraction of DYNAMIC MT spent in tissue
        /// </summary>
        [IgnoreDataMember]
        public double[,,,] FractionalMT { get; set; }
        /// <summary>
        /// number of dynamic and static collisions in each subregion
        /// </summary>
        [IgnoreDataMember]
        public double[,] SubregionCollisions { get; set; }

        /* ==== Place optional/user-defined output properties here. They will be saved in text (JSON) format ==== */
        /// <summary>
        /// number of Zs detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }
        /// <summary>
        /// Z binning
        /// </summary>
        public int NumSubregions { get; set; }

        public void Initialize(ITissue tissue, Random rng)
        {
            // intialize any necessary class fields here
            _tissue = tissue;
            _rng = rng;
            //_ops = _tissue.Regions.Select(r => r.RegionOP).ToArray();

            // assign any user-defined outputs (except arrays...we'll make those on-demand)
            TallyCount = 0;
            NumSubregions = _tissue.Regions.Count;

            // if the data arrays are null, create them (only create second moment if TallySecondMoment is true)
            Mean = Mean ?? new double[X.Count - 1, Y.Count - 1, MTBins.Count - 1];
            SecondMoment = SecondMoment ?? (TallySecondMoment ? new double[X.Count - 1, Y.Count - 1, MTBins.Count - 1] : null);

            TotalMTOfZ = TotalMTOfZ ?? new double[X.Count - 1, Y.Count - 1, Z.Count - 1];
            DynamicMTOfZ = DynamicMTOfZ ?? new double[X.Count - 1, Y.Count - 1, Z.Count - 1];
            TotalMTOfZSecondMoment = TotalMTOfZSecondMoment ?? new double[X.Count - 1, Y.Count - 1, Z.Count - 1];
            DynamicMTOfZSecondMoment = DynamicMTOfZSecondMoment ?? new double[X.Count - 1, Y.Count - 1, Z.Count - 1];

            // Fractional MT has FractionalMTBins.Count numnber of bins PLUS 2, one for =1, an d one for =0
            FractionalMT = FractionalMT ?? new double[X.Count - 1, Y.Count - 1, MTBins.Count - 1, FractionalMTBins.Count + 1];

            SubregionCollisions = new double[NumSubregions, 2]; // 2nd index: 0=static, 1=dynamic 

            // intialize any other necessary class fields here
            _bloodVolumeFraction = BloodVolumeFraction;
 
        }

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            // calculate the radial bin to attribute the deposition
            var ix = DetectorBinning.WhichBin(photon.DP.Position.X, X.Count - 1, X.Delta, X.Start);
            var iy = DetectorBinning.WhichBin(photon.DP.Position.Y, Y.Count - 1, Y.Delta, Y.Start);  
          
            var tissueMT = new double[2]; // 2 is for [static, dynamic] tally separation
            bool talliedMT = false;
            double totalMT = 0;
            var totalMTOfZForOnePhoton = new double[X.Count - 1, Y.Count - 1, Z.Count - 1];
            var dynamicMTOfZForOnePhoton = new double[X.Count - 1, Y.Count - 1, Z.Count - 1];

            // go through photon history and claculate momentum transfer
            // assumes that no MT tallied at pseudo-collisions (reflections and refractions)
            // this algorithm needs to look ahead to angle of next DP, but needs info from previous to determine whether real or pseudo-collision
            PhotonDataPoint previousDP = photon.History.HistoryData.First();
            PhotonDataPoint currentDP = photon.History.HistoryData.Skip(1).Take(1).First();
            foreach (PhotonDataPoint nextDP in photon.History.HistoryData.Skip(2))
            {
                if (previousDP.Weight != currentDP.Weight) // only for true collision points
                {
                    var csr = _tissue.GetRegionIndex(currentDP.Position); // get current region index
                    // get z bin of current position
                    var iz = DetectorBinning.WhichBin(currentDP.Position.Z, Z.Count - 1, Z.Delta, Z.Start);
                    // get angle between current and next
                    double cosineBetweenTrajectories = Direction.GetDotProduct(currentDP.Direction, nextDP.Direction);
                    var momentumTransfer = 1 - cosineBetweenTrajectories;
                    totalMT += momentumTransfer;
                    TotalMTOfZ[ix, iy, iz] += photon.DP.Weight * momentumTransfer;
                    totalMTOfZForOnePhoton[ix, iy, iz] += photon.DP.Weight * momentumTransfer;
                    if (_rng.NextDouble() < _bloodVolumeFraction[csr]) // hit blood 
                    {
                        tissueMT[1] += momentumTransfer;
                        DynamicMTOfZ[ix, iy, iz] += photon.DP.Weight * momentumTransfer;
                        dynamicMTOfZForOnePhoton[ix, iy, iz] += photon.DP.Weight * momentumTransfer;
                        SubregionCollisions[csr, 1] += 1; // add to dynamic collision count
                    }
                    else // index 0 captures static events
                    {
                        tissueMT[0] += momentumTransfer;
                        SubregionCollisions[csr, 0] += 1; // add to static collision count
                    }
                    talliedMT = true;
                }
                previousDP = currentDP;
                currentDP = nextDP;
            }
            if (totalMT > 0.0)  // only tally if momentum transfer accumulated
            {
                var imt = DetectorBinning.WhichBin(totalMT, MTBins.Count - 1, MTBins.Delta, MTBins.Start);
                Mean[ix, iy, imt] += photon.DP.Weight;
                if (TallySecondMoment)
                {
                    SecondMoment[ix, iy, imt] += photon.DP.Weight * photon.DP.Weight;
                    for (int i = 0; i < X.Count - 1; i++)
                    {
                        for (int j = 0; j < Y.Count - 1; j++)
                        {
                            for (int k = 0; k < Z.Count - 1; k++)
                            {
                                TotalMTOfZSecondMoment[i, j, k] += totalMTOfZForOnePhoton[i, j, k] *
                                                                totalMTOfZForOnePhoton[i, j, k];
                                DynamicMTOfZSecondMoment[i, j, k] += dynamicMTOfZForOnePhoton[i, j, k] *
                                                                  dynamicMTOfZForOnePhoton[i, j, k];
                            }
                        }
                    }
                }

                if (talliedMT) TallyCount++;

                // tally DYNAMIC fractional MT in each subregion
                int ifrac;
                for (int isr = 0; isr < NumSubregions; isr++)
                {
                    // add 1 to ifrac to offset bin 0 added for =0 only tallies
                    ifrac = DetectorBinning.WhichBin(tissueMT[1] / totalMT,
                        FractionalMTBins.Count - 1, FractionalMTBins.Delta, FractionalMTBins.Start) + 1;
                    // put identically 0 fractional MT into separate bin at index 0
                    if (tissueMT[1] / totalMT == 0.0)
                    {
                        ifrac = 0;
                    }
                    // put identically 1 fractional MT into separate bin at index Count+1 -1
                    if (tissueMT[1] / totalMT == 1.0)
                    {
                        ifrac = FractionalMTBins.Count;
                    }
                    FractionalMT[ix, iy, imt, ifrac] += photon.DP.Weight;
                }
            }       
        }

        /// <summary>
        /// method to normalize detector results after all photons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var areaNorm = X.Delta * Y.Delta;
            for (int ix = 0; ix < X.Count - 1; ix++)
            {
                for (int iy = 0; iy < Y.Count - 1; iy++)
                {
                    for (int imt = 0; imt < MTBins.Count - 1; imt++)
                    {
                        // normalize by area dx * dy and N
                        Mean[ix, iy, imt] /= areaNorm*numPhotons;
                        if (TallySecondMoment)
                        {
                            SecondMoment[ix, iy, imt] /= areaNorm*areaNorm*numPhotons;
                        }
                        for (int ifrac = 0; ifrac < FractionalMTBins.Count + 1; ifrac++)
                        {
                            FractionalMT[ix, iy, imt, ifrac] /= areaNorm * numPhotons;
                        }
                    }
                    for (int iz = 0; iz < Z.Count - 1; iz++)
                    {
                        TotalMTOfZ[ix, iy, iz] /= areaNorm * numPhotons;
                        DynamicMTOfZ[ix, iy, iz] /= areaNorm * numPhotons;
                        if (TallySecondMoment)
                        {
                            TotalMTOfZSecondMoment[ix, iy, iz] /= areaNorm * areaNorm * numPhotons;
                            DynamicMTOfZSecondMoment[ix, iy, iz] /= areaNorm * areaNorm * numPhotons;
                        }
                    }
                }
            }
        }
        // this is to allow saving of large arrays separately as a binary file
        public BinaryArraySerializer[] GetBinarySerializers()
        {
            return new[]
            {
                new BinaryArraySerializer
                {
                    DataArray = Mean,
                    Name = "Mean",
                    FileTag = "",
                    WriteData = binaryWriter =>
                    {
                        for (int i = 0; i < X.Count - 1; i++)
                        {
                            for (int j = 0; j < Y.Count - 1; j++)
                            {
                                for (int k = 0; k < MTBins.Count - 1; k++)
                                {
                                    binaryWriter.Write(Mean[i, j, k]);
                                }
                            }
                        }
                    },
                    ReadData = binaryReader =>
                    {
                        Mean = Mean ?? new double[X.Count - 1, Y.Count - 1, MTBins.Count - 1];
                        for (int i = 0; i < X.Count - 1; i++)
                        {
                            for (int j = 0; j < Y.Count - 1; j++)
                            {
                                for (int k = 0; k < MTBins.Count - 1; k++)
                                {
                                    Mean[i, j, k] = binaryReader.ReadDouble();
                                }
                            }
                        }
                    }
                },
                new BinaryArraySerializer
                {
                    DataArray = FractionalMT,
                    Name = "FractionalMT",
                    FileTag = "_FractionalMT",
                    WriteData = binaryWriter =>
                    {
                        for (int i = 0; i < X.Count - 1; i++)
                        {
                            for (int j = 0; j < Y.Count - 1; j++)
                            {
                                for (int l = 0; l < MTBins.Count - 1; l++)
                                {
                                    for (int n = 0; n < FractionalMTBins.Count + 1; n++)
                                    {
                                        binaryWriter.Write(FractionalMT[i, j, l, n]);
                                    }
                                }
                            }
                        }
                    },
                    ReadData = binaryReader =>
                    {
                        FractionalMT = FractionalMT ??
                                       new double[X.Count - 1, Y.Count - 1, MTBins.Count - 1, 
                                           FractionalMTBins.Count + 1];
                        for (int i = 0; i < X.Count - 1; i++)
                        {
                            for (int j = 0; j < Y.Count - 1; j++)
                            {
                                for (int l = 0; l < MTBins.Count - 1; l++)
                                {
                                    for (int n = 0; n < FractionalMTBins.Count + 1; n++)
                                    {
                                        FractionalMT[i, j, l, n] = binaryReader.ReadDouble();
                                    }
                                }
                            }
                        }
                    }
                },
                new BinaryArraySerializer
                {
                    DataArray = TotalMTOfZ,
                    Name = "TotalMTOfZ",
                    FileTag = "_TotalMTOfZ",
                    WriteData = binaryWriter =>
                    {
                        for (int i = 0; i < X.Count - 1; i++)
                        {
                            for (int j = 0; j < Y.Count - 1; j++)
                            {
                                for (int l = 0; l < Z.Count - 1; l++)
                                {
                                    binaryWriter.Write(TotalMTOfZ[i, j, l]);
                                }
                            }
                        }
                    },
                    ReadData = binaryReader =>
                    {
                        TotalMTOfZ = TotalMTOfZ ??
                                       new double[X.Count - 1, Y.Count - 1, Z.Count - 1];
                        for (int i = 0; i < X.Count - 1; i++)
                        {
                            for (int j = 0; j < Y.Count - 1; j++)
                            {
                                for (int l = 0; l < Z.Count - 1; l++)
                                {
                                        TotalMTOfZ[i, j, l] = binaryReader.ReadDouble();
                                }
                            }
                        }
                    }
                },
                new BinaryArraySerializer
                {
                    DataArray = DynamicMTOfZ,
                    Name = "DynamicMTOfZ",
                    FileTag = "_DynamicMTOfZ",
                    WriteData = binaryWriter =>
                    {
                        for (int i = 0; i < X.Count - 1; i++)
                        {
                            for (int j = 0; j < Y.Count - 1; j++)
                            {
                                for (int l = 0; l < Z.Count - 1; l++)
                                {
                                    binaryWriter.Write(DynamicMTOfZ[i, j, l]);
                                }
                            }
                        }
                    },
                    ReadData = binaryReader =>
                    {
                        DynamicMTOfZ = DynamicMTOfZ ??
                                       new double[X.Count - 1, Y.Count - 1, Z.Count - 1];
                        for (int i = 0; i < X.Count - 1; i++)
                        {
                            for (int j = 0; j < Y.Count - 1; j++)
                            {
                                for (int l = 0; l < Z.Count - 1; l++)
                                {
                                        DynamicMTOfZ[i, j, l] = binaryReader.ReadDouble();
                                }
                            }
                        }
                    }
                },
                new BinaryArraySerializer
                {
                    DataArray = SubregionCollisions,
                    Name = "SubregionCollisions",
                    FileTag = "_SubregionCollisions",
                    WriteData = binaryWriter =>
                    {
                        for (int i = 0; i < NumSubregions; i++)
                        {
                            for (int l = 0; l < 2; l++)
                            {
                                binaryWriter.Write(SubregionCollisions[i, l]);
                            }
                        }
                    },
                    ReadData = binaryReader =>
                    {
                        SubregionCollisions = SubregionCollisions ??
                                       new double[NumSubregions, 2];
                        for (int i = 0; i < NumSubregions; i++)
                        {
                            for (int l = 0; l < 2; l++)
                            {
                                SubregionCollisions[i, l] = binaryReader.ReadDouble();
                            }
                        }
                    }
                },
               // return a null serializer, if we're not serializing the second moment
                !TallySecondMoment ? null : 
                 new BinaryArraySerializer
                    {
                        DataArray = TotalMTOfZSecondMoment,
                        Name = "TotalMTOfZSecondMoment",
                        FileTag = "_TotalMTOfZ_2",
                        WriteData = binaryWriter =>
                        {
                            if (!TallySecondMoment || TotalMTOfZSecondMoment == null) return;
                            for (int i = 0; i < X.Count - 1; i++)
                            {
                                for (int j = 0; j < Y.Count - 1; j++)
                                {
                                    for (int k = 0; k < Z.Count - 1; k++)
                                    {
                                        binaryWriter.Write(TotalMTOfZSecondMoment[i, j, k]);
                                    }
                                }
                            }
                        },
                        ReadData = binaryReader =>
                        {
                            if (!TallySecondMoment || TotalMTOfZSecondMoment == null) return;
                            SecondMoment = new double[X.Count - 1, Y.Count - 1, Z.Count - 1];
                            for (int i = 0; i < X.Count - 1; i++)
                            {
                                for (int j = 0; j < Y.Count - 1; j++)
                                {
                                    for (int k = 0; k < Z.Count - 1; k++)
                                    {
                                        TotalMTOfZSecondMoment[i, j, k] = binaryReader.ReadDouble();
                                    }
                                }
                            }
                        },
                    },
                    new BinaryArraySerializer
                    {
                        DataArray = DynamicMTOfZSecondMoment,
                        Name = "DynamicMTOfZSecondMoment",
                        FileTag = "_DynamicMTOfZ_2",
                        WriteData = binaryWriter =>
                        {
                            if (!TallySecondMoment || DynamicMTOfZSecondMoment == null) return;
                            for (int i = 0; i < X.Count - 1; i++)
                            {
                                for (int j = 0; j < Y.Count - 1; j++)
                                {
                                    for (int k = 0; k < Z.Count - 1; k++)
                                    {
                                        binaryWriter.Write(SecondMoment[i, j, k]);
                                    }
                                }
                            }
                        },
                        ReadData = binaryReader =>
                        {
                            if (!TallySecondMoment || SecondMoment == null) return;
                            DynamicMTOfZSecondMoment = new double[X.Count - 1, Y.Count - 1, MTBins.Count - 1];
                            for (int i = 0; i < X.Count - 1; i++)
                            {
                                for (int j = 0; j < Y.Count - 1; j++)
                                {
                                    for (int k = 0; k < Z.Count - 1; k++)
                                    {
                                        DynamicMTOfZSecondMoment[i, j, k] = binaryReader.ReadDouble();
                                    }
                                }
                            }
                        },
                    },
                    new BinaryArraySerializer
                    {
                        DataArray = SecondMoment,
                        Name = "SecondMoment",
                        FileTag = "_2",
                        WriteData = binaryWriter =>
                        {
                            if (!TallySecondMoment || SecondMoment == null) return;
                            for (int i = 0; i < X.Count - 1; i++)
                            {
                                for (int j = 0; j < Y.Count - 1; j++)
                                {
                                    for (int k = 0; k < MTBins.Count - 1; k++)
                                    {
                                        binaryWriter.Write(SecondMoment[i, j, k]);
                                    }
                                }
                            }
                        },
                        ReadData = binaryReader =>
                        {
                            if (!TallySecondMoment || SecondMoment == null) return;
                            SecondMoment = new double[X.Count - 1, Y.Count - 1, MTBins.Count - 1];
                            for (int i = 0; i < X.Count - 1; i++)
                            {
                                for (int j = 0; j < Y.Count - 1; j++)
                                {
                                    for (int k = 0; k < MTBins.Count - 1; k++)
                                    {
                                        SecondMoment[i, j, k] = binaryReader.ReadDouble();
                                    }
                                }
                            }
                        },
                    }
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
            //return (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionDomainTopBoundary));
        }

    }
}
