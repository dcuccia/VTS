using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements ITerminationTally&lt;double[]&gt;.  Tally for pMC estimation of reflectance 
    /// as a function of Rho.
    /// </summary>
    [KnownType(typeof(pMCROfRhoDetector))]
    public class pMCROfRhoDetector : IpMCSurfaceDetector<double[]>
    {
        private IList<OpticalProperties> _referenceOps;
        private IList<OpticalProperties> _perturbedOps;
        private IList<int> _perturbedRegionsIndices;
        private double _rhoDelta;  // need to keep this because DoubleRange adjusts deltas automatically
        private bool _tallySecondMoment;
        private Func<IList<long>, IList<double>, IList<OpticalProperties>, double> _absorbAction;

        /// <summary>
        /// Returns an instance of pMCROfRhoDetector. Tallies perturbed R(rho). Instantiate with reference optical properties. 
        /// When method Tally invoked, perturbed optical properties passed.
        /// </summary>
        /// <param name="rho"></param>
        /// <param name="tissue"></param>
        /// <param name="perturbedOps"></param>
        /// <param name="perturbedRegionIndices"></param>
        public pMCROfRhoDetector(
            DoubleRange rho,
            ITissue tissue,
            IList<OpticalProperties> perturbedOps,
            IList<int> perturbedRegionIndices,
            bool tallySecondMoment,
            String name)
        {
            Rho = rho;
            _tallySecondMoment = tallySecondMoment;
            Mean = new double[Rho.Count - 1]; // change from count -1
            SecondMoment = null;
            if (_tallySecondMoment)
            {
                SecondMoment = new double[Rho.Count - 1];
            }
            TallyType = TallyType.pMCROfRho;
            Name = name;
            _perturbedOps = perturbedOps;
            _referenceOps = tissue.Regions.Select(r => r.RegionOP).ToList();
            _perturbedRegionsIndices = perturbedRegionIndices;
            SetAbsorbAction(tissue.AbsorptionWeightingType);
            TallyCount = 0;
        }

        /// <summary>
        /// Returns a default instance of pMCMuaMusROfRhoDetector (for serialization purposes only)
        /// </summary>
        public pMCROfRhoDetector()
            : this(
            new DoubleRange(), 
            new MultiLayerTissue(), 
            new List<OpticalProperties>(), 
            new List<int>(), 
            true, // tallySecondMoment
            TallyType.pMCROfRho.ToString() )
        {
        }

        [IgnoreDataMember]
        public double[] Mean { get; set; }

        [IgnoreDataMember]
        public double[] SecondMoment { get; set; }

        public TallyType TallyType { get; set; }

        public String Name { get; set; }

        public long TallyCount { get; set; }

        public DoubleRange Rho { get; set; }
        
        protected void SetAbsorbAction(AbsorptionWeightingType awt)
        {
            switch (awt)
            {
                // note: pMC is not applied to analog processing,
                // only DAW and CAW
                case AbsorptionWeightingType.Continuous:
                    _absorbAction = AbsorbContinuous;
                    break;
                case AbsorptionWeightingType.Discrete:
                default:
                    _absorbAction = AbsorbDiscrete;
                    break;
            }
        }

        public void Tally(PhotonDataPoint dp, CollisionInfo infoList)
        {
            var ir = DetectorBinning.WhichBinExclusive(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            if (ir != -1)
            {
                double weightFactor = _absorbAction(
                    infoList.Select(c => c.NumberOfCollisions).ToList(),
                    infoList.Select(p => p.PathLength).ToList(),
                    _perturbedOps);

                Mean[ir] += dp.Weight * weightFactor;
                if (_tallySecondMoment)
                {
                    SecondMoment[ir] += dp.Weight * weightFactor * dp.Weight * weightFactor;
                }
                TallyCount++;
            }
        }

        private double AbsorbContinuous(IList<long> numberOfCollisions, IList<double> pathLength, IList<OpticalProperties> perturbedOps)
        {
            double weightFactor = 1.0;

            foreach (var i in _perturbedRegionsIndices)
            {
                weightFactor *=
                    Math.Exp(-(perturbedOps[i].Mua - _referenceOps[i].Mua) * pathLength[i]); // mua pert
                if (numberOfCollisions[i] > 0) // mus pert
                {
                    // the following is more numerically stable
                    weightFactor *= Math.Pow(
                        (_perturbedOps[i].Mus / _referenceOps[i].Mus) * Math.Exp(-(_perturbedOps[i].Mus - _referenceOps[i].Mus) *
                            pathLength[i] / numberOfCollisions[i]),
                        numberOfCollisions[i]);
                }
                else
                {
                    weightFactor *= Math.Exp(-(_perturbedOps[i].Mus - _referenceOps[i].Mus) * pathLength[i]);
                }
            }
            return weightFactor;
        }

        private double AbsorbDiscrete(IList<long> numberOfCollisions, IList<double> pathLength, IList<OpticalProperties> perturbedOps)
        {
            double weightFactor = 1.0;

            foreach (var i in _perturbedRegionsIndices)
            {
                weightFactor *=
                    Math.Pow(
                        (_perturbedOps[i].Mus / _referenceOps[i].Mus) * 
                            Math.Exp(-(_perturbedOps[i].Mus + _perturbedOps[i].Mua - _referenceOps[i].Mus - _referenceOps[i].Mua) *
                            pathLength[i] / numberOfCollisions[i]),
                        numberOfCollisions[i]);
            }
            return weightFactor;
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
                Mean[ir] /= areaNorm * numPhotons;
                // the above is pi(rmax*rmax-rmin*rmin) * rhoDelta * N
                if (_tallySecondMoment)
                {
                    SecondMoment[ir] /= areaNorm * areaNorm * numPhotons;
                }
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true; // or, possibly test for NA or confined position, etc
            // return (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionDomainTopBoundary));
        }
    }
}
