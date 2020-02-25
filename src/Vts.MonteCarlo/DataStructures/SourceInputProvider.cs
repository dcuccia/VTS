using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Implements various commonly used SourceInput classes.
    /// </summary>
    public class SourceInputProvider
    {
        private static IServiceProvider _serviceProvider;
        private static IServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider == null)
                {
                    var collection = new ServiceCollection()
                        .Scan(scan =>
                            scan.FromApplicationDependencies()
                                .AddClasses(classes => classes.AssignableTo<ISourceInput>())
                                .AsSelfWithInterfaces()
                                .WithTransientLifetime()
                        );
                    _serviceProvider = collection.BuildServiceProvider();
                }

                return _serviceProvider;
            }
        }

        /// <summary>
        /// Method that provides instances of all ISourceInput implementations in all running assemblies, including those in client code
        /// </summary>
        /// <returns>a list of the ISourceInputs generated</returns>
        public static IList<ISourceInput> GetAllSourceInputs()
        {
            var sourceInputs = ServiceProvider.GetServices<ISourceInput>().ToArray();

            return sourceInputs;
        }

        /// <summary>
        /// Method that provides an instance of the specified ISourceInput implementation from any assembly, including those in client code
        /// </summary>
        /// <returns>the ISourceInput generated</returns>
        public static ISourceInput GetSourceInput<TSourceInput>() where TSourceInput : ISourceInput
        {
            var sourceInput = ServiceProvider.GetService<TSourceInput>();

            return sourceInput;
        }

        #region Built-in SourceInput helpers (caller doesn't need to reference Vts.MonteCarlo.Sources)

        public static ISourceInput DirectionalPointSourceInput() { return new Vts.MonteCarlo.Sources.DirectionalPointSourceInput(); }
        public static ISourceInput IsotropicPointSourceInput() { return new Vts.MonteCarlo.Sources.IsotropicPointSourceInput(); }
        public static ISourceInput CustomPointSourceInput() { return new Vts.MonteCarlo.Sources.CustomPointSourceInput(); }
        public static ISourceInput DirectionalLineSourceInput() { return new Vts.MonteCarlo.Sources.DirectionalLineSourceInput(); }
        public static ISourceInput IsotropicLineSourceInput() { return new Vts.MonteCarlo.Sources.IsotropicLineSourceInput(); }
        public static ISourceInput CustomLineSourceInput() { return new Vts.MonteCarlo.Sources.CustomLineSourceInput(); }
        public static ISourceInput DirectionalCircularSourceInput() { return new Vts.MonteCarlo.Sources.DirectionalCircularSourceInput(); }
        public static ISourceInput CustomCircularSourceInput() { return new Vts.MonteCarlo.Sources.CustomCircularSourceInput(); }
        public static ISourceInput DirectionalEllipticalSourceInput() { return new Vts.MonteCarlo.Sources.DirectionalEllipticalSourceInput(); }
        public static ISourceInput CustomEllipticalSourceInput() { return new Vts.MonteCarlo.Sources.CustomEllipticalSourceInput(); }
        public static ISourceInput DirectionalRectangularSourceInput() { return new Vts.MonteCarlo.Sources.DirectionalRectangularSourceInput(); }
        public static ISourceInput CustomRectangularSourceInput() { return new Vts.MonteCarlo.Sources.CustomRectangularSourceInput(); }
        public static ISourceInput LambertianSurfaceEmittingCylindricalFiberSourceInput() { return new Vts.MonteCarlo.Sources.LambertianSurfaceEmittingCylindricalFiberSourceInput(); }
        public static ISourceInput LambertianSurfaceEmittingSphericalSourceInput() { return new Vts.MonteCarlo.Sources.LambertianSurfaceEmittingSphericalSourceInput(); }
        public static ISourceInput CustomSurfaceEmittingSphericalSourceInput() { return new Vts.MonteCarlo.Sources.CustomSurfaceEmittingSphericalSourceInput(); }
        public static ISourceInput LambertianSurfaceEmittingCuboidalSourceInput() { return new Vts.MonteCarlo.Sources.LambertianSurfaceEmittingCuboidalSourceInput(); }
        public static ISourceInput LambertianSurfaceEmittingTubularSourceInput() { return new Vts.MonteCarlo.Sources.LambertianSurfaceEmittingTubularSourceInput(); }
        public static ISourceInput IsotropicVolumetricCuboidalSourceInput() { return new Vts.MonteCarlo.Sources.IsotropicVolumetricCuboidalSourceInput(); }
        public static ISourceInput CustomVolumetricCuboidalSourceInput() { return new Vts.MonteCarlo.Sources.CustomVolumetricCuboidalSourceInput(); }
        public static ISourceInput IsotropicVolumetricEllipsoidalSourceInput() { return new Vts.MonteCarlo.Sources.IsotropicVolumetricEllipsoidalSourceInput(); }
        public static ISourceInput CustomVolumetricEllipsoidalSourceInput() { return new Vts.MonteCarlo.Sources.CustomVolumetricEllipsoidalSourceInput(); }

        #endregion
    }
}
