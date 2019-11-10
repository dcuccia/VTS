using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Tally for Transmitted dynamic MT as a function of Fx using blood volume fraction in each tissue region.
    /// This detector also tallies the total and dynamic MT as a function of Z.   If a random number is less
    /// than blood volume fraction for the tissue region in which the collision occurred, then hit blood and considered
    /// "dynamic" event.  Otherwise, it is a "static" event.
    /// This works for Analog and DAW processing.
    /// </summary>
    public class TransmittedDynamicMTOfFxAndSubregionHistDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for TransmittedMT as a function of fx and tissue subregion detector input
        /// </summary>
        public TransmittedDynamicMTOfFxAndSubregionHistDetectorInput()
        {
            TallyType = "TransmittedDynamicMTOfFxAndSubregionHist";
            Name = "TransmittedDynamicMTOfFxAndSubregionHist";
            Fx = new DoubleRange(0.0, 10, 101);
            Z = new DoubleRange(0.0, 10, 101);
            MTBins = new DoubleRange(0.0, 500.0, 51);

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsTransmittanceTally = true;
            TallyDetails.IsCylindricalTally = false;
        }

        /// <summary>
        /// Fx binning
        /// </summary>
        public DoubleRange Fx { get; set; }
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
            return new TransmittedDynamicMTOfFxAndSubregionHistDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                Fx = this.Fx,
                Z = this.Z,
                MTBins = this.MTBins,
                BloodVolumeFraction = this.BloodVolumeFraction,
                FractionalMTBins = this.FractionalMTBins
            };
        }
    }

    /// <summary>
    /// Implements IDetector.  Tally for momentum transfer as a function  of Fx and tissue subregion
    /// using blood volume fraction in each tissue subregion.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class TransmittedDynamicMTOfFxAndSubregionHistDetector : Detector, IDetector
    {
        private ITissue _tissue;
        //private IList<OpticalProperties> _ops;
        private IList<double> _bloodVolumeFraction;
        private Random _rng;

        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        /// <summary>
        /// Fx binning
        /// </summary>
        public DoubleRange Fx { get; set; }
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
        public Complex[,] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public Complex[,] SecondMoment { get; set; }
        /// <summary>
        /// total MT as a function of Z multiplied by final photon weight
        /// </summary>
        [IgnoreDataMember]
        public Complex[,] TotalMTOfZ { get; set; }
        /// <summary>
        /// total MT Second Moment as a function of Z multiplied by final photon weight
        /// </summary>
        [IgnoreDataMember]
        public Complex[,] TotalMTOfZSecondMoment { get; set; }
        /// <summary>
        /// dynamic MT as a function of Z multiplied by final photon weight
        /// </summary>
        [IgnoreDataMember]
        public Complex[,] DynamicMTOfZ { get; set; }
        /// <summary>
        /// dynamic MT Second Moment as a function of Z multiplied by final photon weight
        /// </summary>
        [IgnoreDataMember]
        public Complex[,] DynamicMTOfZSecondMoment { get; set; }
        /// <summary>
        /// fraction of DYNAMIC MT spent in tissue
        /// </summary>
        [IgnoreDataMember]
        public Complex[,,] FractionalMT { get; set; }
        /// <summary>
        /// number of dynamic and static collisions in each subregion
        /// </summary>
        [IgnoreDataMember]
        public double[,] SubregionCollisions { get; set; }

        /* ==== Place optional/user-defined output properties here. They will be saved in text (JSON) format ==== */
        /// <summary>
        /// number of tallies to detector
        /// </summary>
        public long TallyCount { get; set; }
        /// <summary>
        /// number of tissue subregions
        /// </summary>
        public int NumSubregions { get; set; }

        public void Initialize(ITissue tissue, Random rng)
        {
            // intialize any necessary class fields here
            _tissue = tissue;
            _rng = rng;

            // assign any user-defined outputs (except arrays...we'll make those on-demand)
            TallyCount = 0;
            NumSubregions = _tissue.Regions.Count;

            // if the data arrays are null, create them (only create second moment if TallySecondMoment is true)
            Mean = Mean ?? new Complex[Fx.Count, MTBins.Count - 1];
            SecondMoment = SecondMoment ?? (TallySecondMoment ? new Complex[Fx.Count, MTBins.Count - 1] : null);

            TotalMTOfZ = TotalMTOfZ ?? new Complex[Fx.Count, Z.Count - 1];
            DynamicMTOfZ = DynamicMTOfZ ?? new Complex[Fx.Count, Z.Count - 1];
            TotalMTOfZSecondMoment = TotalMTOfZSecondMoment ?? new Complex[Fx.Count, Z.Count - 1];
            DynamicMTOfZSecondMoment = DynamicMTOfZSecondMoment ?? new Complex[Fx.Count, Z.Count - 1];

            // Fractional MT has FractionalMTBins.Count numnber of bins PLUS 2, one for =1, an d one for =0
            FractionalMT = FractionalMT ?? new Complex[Fx.Count, MTBins.Count - 1, FractionalMTBins.Count + 1];

            SubregionCollisions = new double[NumSubregions, 2]; // 2nd index: 0=static, 1=dynamic  

            // initialize any other necessary class fields here
            _bloodVolumeFraction = BloodVolumeFraction;
  
        }

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            // calculate the radial bin to attribute the deposition
            var tissueMT = new double[2]; // 2 is for [static, dynamic] tally separation
            bool talliedMT = false;
            double totalMT = 0;
            var totalMTOfZForOnePhoton = new Complex[Fx.Count, Z.Count - 1];
            var dynamicMTOfZForOnePhoton = new Complex[Fx.Count, Z.Count - 1];
            var fxArray = Fx.AsEnumerable().ToArray();
            var x = photon.DP.Position.X; // use final exiting x position
            var sinNegativeTwoPiFX = fxArray.Select(fx => Math.Sin(-2 * Math.PI * fx * x)).ToArray();
            var cosNegativeTwoPiFX = fxArray.Select(fx => Math.Cos(-2 * Math.PI * fx * x)).ToArray();

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
                    for (int ifx = 0; ifx < Fx.Count; ifx++)
                    {
                        var deltaWeight = photon.DP.Weight * cosNegativeTwoPiFX[ifx] +
                                          Complex.ImaginaryOne * sinNegativeTwoPiFX[ifx];
                        TotalMTOfZ[ifx, iz] += deltaWeight * momentumTransfer;
                        totalMTOfZForOnePhoton[ifx, iz] += deltaWeight * momentumTransfer;
                    }
                    if (_rng.NextDouble() < _bloodVolumeFraction[csr]) // hit blood 
                    {
                        tissueMT[1] += momentumTransfer;
                        for (int ifx = 0; ifx < Fx.Count; ifx++)
                        {
                            var deltaWeight = photon.DP.Weight * cosNegativeTwoPiFX[ifx] +
                                              Complex.ImaginaryOne * sinNegativeTwoPiFX[ifx];
                            DynamicMTOfZ[ifx, iz] += deltaWeight * momentumTransfer;
                            dynamicMTOfZForOnePhoton[ifx, iz] += deltaWeight * momentumTransfer;
                        }
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
                for (int ifx = 0; ifx < Fx.Count; ifx++)
                {
                    var deltaWeight = photon.DP.Weight * cosNegativeTwoPiFX[ifx] +
                                      Complex.ImaginaryOne * sinNegativeTwoPiFX[ifx];
                    Mean[ifx, imt] += deltaWeight;
                    if (TallySecondMoment)
                    {
                        SecondMoment[ifx, imt] += deltaWeight * deltaWeight;
                        for (int i = 0; i < Fx.Count - 1; i++)
                        {
                            for (int j = 0; j < Z.Count - 1; j++)
                            {
                                TotalMTOfZSecondMoment[i, j] +=
                                    totalMTOfZForOnePhoton[i, j] * totalMTOfZForOnePhoton[i, j];
                                DynamicMTOfZSecondMoment[i, j] +=
                                    dynamicMTOfZForOnePhoton[i, j] * dynamicMTOfZForOnePhoton[i, j];
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

                        FractionalMT[ifx, imt, ifrac] += deltaWeight;
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
            for (int ifx = 0; ifx < Fx.Count; ifx++)
            {
                for (int imt = 0; imt < MTBins.Count - 1; imt++)
                {
                    Mean[ifx, imt] /= numPhotons;
                    if (TallySecondMoment)
                    {
                        SecondMoment[ifx, imt] /= numPhotons;
                    }
                    for (int ifrac = 0; ifrac < FractionalMTBins.Count + 1; ifrac++)
                    {
                        FractionalMT[ifx, imt, ifrac] /= numPhotons;
                    } 
                }
                for (int iz = 0; iz < Z.Count - 1; iz++)
                {
                    TotalMTOfZ[ifx, iz] /= numPhotons;
                    DynamicMTOfZ[ifx, iz] /= numPhotons;
                    if (TallySecondMoment)
                    {
                        TotalMTOfZSecondMoment[ifx, iz] /= numPhotons;
                        DynamicMTOfZSecondMoment[ifx, iz] /= numPhotons;
                    }
                }
            }
        }
        // this is to allow saving of large arrays separately as a binary file
        public BinaryArraySerializer[] GetBinarySerializers()
        {
            return new[] {
                new BinaryArraySerializer {
                    DataArray = Mean,
                    Name = "Mean",
                    FileTag = "",
                    WriteData = binaryWriter => {
                        for (int i = 0; i < Fx.Count; i++) {
                            for (int j = 0; j < MTBins.Count - 1; j++)
                            {
                                binaryWriter.Write(Mean[i, j].Real);
                                binaryWriter.Write(Mean[i, j].Imaginary);
                            }
                        }
                    },
                    ReadData = binaryReader => {
                        Mean = Mean ?? new Complex[ Fx.Count, MTBins.Count - 1];
                        for (int i = 0; i <  Fx.Count; i++) {
                            for (int j = 0; j < MTBins.Count - 1; j++)
                            {
                                var real = binaryReader.ReadDouble();
                                var imag = binaryReader.ReadDouble();
                                Mean[i, j] = new Complex(real, imag); 
                            }
                        }
                    }
                },
                new BinaryArraySerializer {
                    DataArray = FractionalMT,
                    Name = "FractionalMT",
                    FileTag = "_FractionalMT",
                    WriteData = binaryWriter =>
                    {
                        for (int i = 0; i < Fx.Count; i++)
                        {
                            for (int k = 0; k < MTBins.Count - 1; k++)
                            {
                                for (int m = 0; m < FractionalMTBins.Count + 1; m++)
                                {
                                    binaryWriter.Write(FractionalMT[i, k, m].Real);
                                    binaryWriter.Write(FractionalMT[i, k, m].Imaginary);
                                }
                            }
                        }
                    },
                    ReadData = binaryReader => {
                        FractionalMT = FractionalMT ?? new Complex[ Fx.Count, MTBins.Count - 1, FractionalMTBins.Count + 1];
                        for (int i = 0; i < Fx.Count; i++)
                        {
                            for (int k = 0; k < MTBins.Count - 1; k++)
                            {
                                for (int m = 0; m < FractionalMTBins.Count + 1; m++)
                                {
                                    var real = binaryReader.ReadDouble();
                                    var imag = binaryReader.ReadDouble();
                                    FractionalMT[i, k, m] = new Complex(real, imag);
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
                        for (int i = 0; i < Fx.Count; i++)
                        {
                            for (int l = 0; l < Z.Count - 1; l++)
                            {
                                binaryWriter.Write(TotalMTOfZ[i, l].Real);
                                binaryWriter.Write(TotalMTOfZ[i, l].Imaginary);
                            }
                        }
                    },
                    ReadData = binaryReader =>
                    {
                        TotalMTOfZ = TotalMTOfZ ?? new Complex[Fx.Count, Z.Count - 1];
                        for (int i = 0; i < Fx.Count; i++)
                        {
                            for (int l = 0; l < Z.Count - 1; l++)
                            {
                                var real = binaryReader.ReadDouble();
                                var imag = binaryReader.ReadDouble();
                                TotalMTOfZ[i, l] = new Complex(real, imag);
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
                        for (int i = 0; i < Fx.Count; i++)
                        {
                            for (int l = 0; l < Z.Count - 1; l++)
                            {
                                binaryWriter.Write(DynamicMTOfZ[i, l].Real);
                                binaryWriter.Write(DynamicMTOfZ[i, l].Imaginary);
                            }
                        }
                    },
                    ReadData = binaryReader =>
                    {
                        DynamicMTOfZ = DynamicMTOfZ ?? new Complex[Fx.Count, Z.Count - 1];
                        for (int i = 0; i < Fx.Count; i++)
                        {
                            for (int l = 0; l < Z.Count - 1; l++)
                            {
                                var real = binaryReader.ReadDouble();
                                var imag = binaryReader.ReadDouble();
                                DynamicMTOfZ[i, l] = new Complex(real, imag);
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
                new BinaryArraySerializer {
                    DataArray = TotalMTOfZSecondMoment,
                    Name = "TotalMTOfZSecondMoment",
                    FileTag = "_TotalMTOfZ_2",
                    WriteData = binaryWriter => {
                        if (!TallySecondMoment || TotalMTOfZSecondMoment == null) return;
                        for (int i = 0; i < Fx.Count; i++) {
                            for (int j = 0; j < Z.Count - 1; j++)
                            {
                                binaryWriter.Write(TotalMTOfZSecondMoment[i, j].Real);
                                binaryWriter.Write(TotalMTOfZSecondMoment[i, j].Imaginary);
                            }                            
                        }
                    },
                    ReadData = binaryReader => {
                        if (!TallySecondMoment || TotalMTOfZSecondMoment == null) return;
                        TotalMTOfZSecondMoment = new Complex[ Fx.Count, Z.Count - 1];
                        for (int i = 0; i < Fx.Count; i++) {
                            for (int j = 0; j < Z.Count - 1; j++)
                            {
                                var real = binaryReader.ReadDouble();
                                var imag = binaryReader.ReadDouble();
                                TotalMTOfZSecondMoment[i, j] = new Complex(real, imag);
                            }                       
                        }
                    },
                },
                new BinaryArraySerializer {
                    DataArray = DynamicMTOfZSecondMoment,
                    Name = "DynamicMTOfZSecondMoment",
                    FileTag = "_DynamicMTOfZ_2",
                    WriteData = binaryWriter => {
                        if (!TallySecondMoment || DynamicMTOfZSecondMoment == null) return;
                        for (int i = 0; i < Fx.Count; i++) {
                            for (int j = 0; j < Z.Count - 1; j++)
                            {
                                binaryWriter.Write(DynamicMTOfZSecondMoment[i, j].Real);
                                binaryWriter.Write(DynamicMTOfZSecondMoment[i, j].Imaginary);
                            }                            
                        }
                    },
                    ReadData = binaryReader => {
                        if (!TallySecondMoment || DynamicMTOfZSecondMoment == null) return;
                        DynamicMTOfZSecondMoment = new Complex[ Fx.Count, Z.Count - 1];
                        for (int i = 0; i < Fx.Count; i++) {
                            for (int j = 0; j < Z.Count - 1; j++)
                            {
                                var real = binaryReader.ReadDouble();
                                var imag = binaryReader.ReadDouble();
                                DynamicMTOfZSecondMoment[i, j] = new Complex(real, imag);
                            }                       
			            }
                    },
                },
                new BinaryArraySerializer {
                    DataArray = SecondMoment,
                    Name = "SecondMoment",
                    FileTag = "_2",
                    WriteData = binaryWriter => {
                        if (!TallySecondMoment || SecondMoment == null) return;
                        for (int i = 0; i < Fx.Count; i++) {
                            for (int j = 0; j < MTBins.Count - 1; j++)
                            {
                                binaryWriter.Write(SecondMoment[i, j].Real);
                                binaryWriter.Write(SecondMoment[i, j].Imaginary);
                            }                            
                        }
                    },
                    ReadData = binaryReader => {
                        if (!TallySecondMoment || SecondMoment == null) return;
                        SecondMoment = new Complex[ Fx.Count, MTBins.Count - 1];
                        for (int i = 0; i < Fx.Count - 1; i++) {
                            for (int j = 0; j < MTBins.Count - 1; j++)
                            {
                                var real = binaryReader.ReadDouble();
                                var imag = binaryReader.ReadDouble();
                                SecondMoment[i, j] = new Complex(real, imag);
                            }                       
			            }
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
            //return (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionDomainTopBoundary));
        }

    }
}
