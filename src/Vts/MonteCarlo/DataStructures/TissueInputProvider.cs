using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Implements various commonly used SourceInput classes.
    /// </summary>
    public class TissueInputProvider
    {
        private static readonly IServiceProvider ServiceProvider;

        static TissueInputProvider()
        {
            var collection = new ServiceCollection()
                .Scan(scan =>
                    scan.FromApplicationDependencies()
                        //.AddClasses(classes => classes.AssignableTo<ISourceInput>())
                        //    .AsImplementedInterfaces()
                        //    .WithTransientLifetime()
                    .AddClasses(classes => classes.AssignableTo<ITissueInput>())
                        .AsSelfWithInterfaces()
                        .WithTransientLifetime()
                    );

            ServiceProvider = collection.BuildServiceProvider();
        }

        /// <summary>
        /// Method that provides instances of all ITissuenput implementations in all running assemblies, including those in client code
        /// </summary>
        /// <returns>a list of the ITissueInputs generated</returns>
        public static IList<ITissueInput> GetAllTissueInputs()
        {
            var tissueInputs = ServiceProvider.GetServices<ITissueInput>().ToArray();

            return tissueInputs;
        }

        /// <summary>
        /// Method that provides an instance of the specified ITissueInput implementation from any assembly, including those in client code
        /// </summary>
        /// <returns>the ITissueInput generated</returns>
        public static ITissueInput GetTissueInput<TTIssueInput>() where TTIssueInput : ITissueInput
        {
            var tissueInput = ServiceProvider.GetService<TTIssueInput>();

            return tissueInput;
        }

        #region Built-in TissueInput helpers (caller doesn't need to reference Vts.MonteCarlo.Tissues)

        public static ITissueInput MultiEllipsoidTissueInput() { return new Vts.MonteCarlo.Tissues.MultiEllipsoidTissueInput(); }
        public static ITissueInput MultiConcentricInfiniteCylinderTissueInput() { return new Vts.MonteCarlo.Tissues.MultiConcentricInfiniteCylinderTissueInput(); }
        public static ITissueInput MultiLayerTissueInput() { return new Vts.MonteCarlo.Tissues.MultiLayerTissueInput(); }
        public static ITissueInput SemiInfiniteTissueInput() { return new Vts.MonteCarlo.Tissues.SemiInfiniteTissueInput(); }
        public static ITissueInput SingleEllipsoidTissueInput() { return new Vts.MonteCarlo.Tissues.SingleEllipsoidTissueInput(); }
        public static ITissueInput SingleInfiniteCylinderTissueInput() { return new Vts.MonteCarlo.Tissues.SingleInfiniteCylinderTissueInput(); }
        public static ITissueInput SingleVoxelTissueInput() { return new Vts.MonteCarlo.Tissues.SingleVoxelTissueInput(); }

        #endregion
    }
}
