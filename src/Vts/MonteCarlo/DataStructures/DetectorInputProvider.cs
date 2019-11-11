using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Implements various commonly used DetectorInput classes.
    /// </summary>
    public class DetectorInputProvider
    {
        private static readonly IServiceProvider ServiceProvider;

        static DetectorInputProvider()
        {
            var collection = new ServiceCollection()
                .Scan(scan =>
                    scan.FromApplicationDependencies()
                    //.AddClasses(classes => classes.AssignableTo<ISourceInput>())
                    //    .AsImplementedInterfaces()
                    //    .WithTransientLifetime()
                    .AddClasses(classes => classes.AssignableTo<IDetectorInput>())
                        .AsSelfWithInterfaces()
                        .WithTransientLifetime()
                    );

            ServiceProvider = collection.BuildServiceProvider();
        }

        /// <summary>
        /// Method that provides instances of all IDetectorInput implementations in all running assemblies, including those in client code
        /// </summary>
        /// <returns>a list of the IDetectorInputs generated</returns>
        public static IList<IDetectorInput> GetAllDetectorInputs()
        {
            var detectorInputs = ServiceProvider.GetServices<IDetectorInput>().ToArray();

            return detectorInputs;
        }

        /// <summary>
        /// Method that provides an instance of the specified IDetectorInput implementation from any assembly, including those in client code
        /// </summary>
        /// <returns>the IDetectorInput generated</returns>
        public static IDetectorInput GetDetectorInput<TDetectorInput>() where TDetectorInput : IDetectorInput
        {
            var detectorInput = ServiceProvider.GetService<TDetectorInput>();

            return detectorInput;
        }

        #region Built-in DetectorInput helpers (caller doesn't need to reference Vts.MonteCarlo.Detectors)

        public static IDetectorInput AOfRhoAndZDetectorInput() { return new Vts.MonteCarlo.Detectors.AOfRhoAndZDetectorInput(); }
        public static IDetectorInput AOfXAndYAndZDetectorInput() { return new Vts.MonteCarlo.Detectors.AOfXAndYAndZDetectorInput(); }
        public static IDetectorInput ATotalDetectorInput() { return new Vts.MonteCarlo.Detectors.ATotalDetectorInput(); }
        public static IDetectorInput FluenceOfRhoAndZAndTimeDetectorInput() { return new Vts.MonteCarlo.Detectors.FluenceOfRhoAndZAndTimeDetectorInput(); }
        public static IDetectorInput FluenceOfRhoAndZDetectorInput() { return new Vts.MonteCarlo.Detectors.FluenceOfRhoAndZDetectorInput(); }
        public static IDetectorInput FluenceOfXAndYAndZDetectorInput() { return new Vts.MonteCarlo.Detectors.FluenceOfXAndYAndZDetectorInput(); }
        public static IDetectorInput FluenceOfXAndYAndZAndOmegaDetectorInput() { return new Vts.MonteCarlo.Detectors.FluenceOfXAndYAndZAndOmegaDetectorInput(); }
        public static IDetectorInput RadianceOfRhoAndZAndAngleDetectorInput() { return new Vts.MonteCarlo.Detectors.RadianceOfRhoAndZAndAngleDetectorInput(); }
        public static IDetectorInput RadianceOfFxAndZAndAngleDetectorInput() { return new Vts.MonteCarlo.Detectors.RadianceOfFxAndZAndAngleDetectorInput(); }
        public static IDetectorInput RadianceOfXAndYAndZAndThetaAndPhiDetectorInput() { return new Vts.MonteCarlo.Detectors.RadianceOfXAndYAndZAndThetaAndPhiDetectorInput(); }
        public static IDetectorInput RadianceOfRhoAtZDetectorInput() { return new Vts.MonteCarlo.Detectors.RadianceOfRhoAtZDetectorInput(); }
        public static IDetectorInput RDiffuseDetectorInput() { return new Vts.MonteCarlo.Detectors.RDiffuseDetectorInput(); }
        public static IDetectorInput ROfAngleDetectorInput() { return new Vts.MonteCarlo.Detectors.ROfAngleDetectorInput(); }
        public static IDetectorInput ROfFxAndTimeDetectorInput() { return new Vts.MonteCarlo.Detectors.ROfFxAndTimeDetectorInput(); }
        public static IDetectorInput ROfFxDetectorInput() { return new Vts.MonteCarlo.Detectors.ROfFxDetectorInput(); }
        public static IDetectorInput ROfRhoAndAngleDetectorInput() { return new Vts.MonteCarlo.Detectors.ROfRhoAndAngleDetectorInput(); }
        public static IDetectorInput ROfRhoAndTimeDetectorInput() { return new Vts.MonteCarlo.Detectors.ROfRhoAndTimeDetectorInput(); }
        public static IDetectorInput ROfRhoDetectorInput() { return new Vts.MonteCarlo.Detectors.ROfRhoDetectorInput(); }
        public static IDetectorInput ROfXAndYDetectorInput() { return new Vts.MonteCarlo.Detectors.ROfXAndYDetectorInput(); }
        public static IDetectorInput RSpecularDetectorInput() { return new Vts.MonteCarlo.Detectors.RSpecularDetectorInput(); }
        public static IDetectorInput TDiffuseDetectorInput() { return new Vts.MonteCarlo.Detectors.TDiffuseDetectorInput(); }
        public static IDetectorInput TOfAngleDetectorInput() { return new Vts.MonteCarlo.Detectors.TOfAngleDetectorInput(); }
        public static IDetectorInput TOfRhoAndAngleDetectorInput() { return new Vts.MonteCarlo.Detectors.TOfRhoAndAngleDetectorInput(); }
        public static IDetectorInput TOfRhoDetectorInput() { return new Vts.MonteCarlo.Detectors.TOfRhoDetectorInput(); }
        public static IDetectorInput TOfXAndYDetectorInput() { return new Vts.MonteCarlo.Detectors.TOfXAndYDetectorInput(); }
        public static IDetectorInput TOfFxDetectorInput() { return new Vts.MonteCarlo.Detectors.TOfFxDetectorInput(); }

        #endregion
    }
}
