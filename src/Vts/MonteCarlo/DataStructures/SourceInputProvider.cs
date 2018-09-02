using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using System;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Implements various commonly used SourceInput classes.
    /// </summary>
    public class SourceInputProvider
    {
        /// <summary>
        /// Method that provides instances of all inputs in this class.
        /// </summary>
        /// <returns>a list of the ISourceInputs generated</returns>
        public static IList<ISourceInput> GenerateAllSourceInputs()
        {
            return new ISourceInput[]
            {
                new DirectionalPointSourceInput(),
                new IsotropicPointSourceInput(),
                new CustomPointSourceInput(),
                new DirectionalLineSourceInput(),
                new IsotropicLineSourceInput(),
                new CustomLineSourceInput(),
                new DirectionalCircularSourceInput(),
                new CustomCircularSourceInput(),
                new DirectionalEllipticalSourceInput(),
                new CustomEllipticalSourceInput(),
                new DirectionalRectangularSourceInput(),
                new CustomRectangularSourceInput(),
                new LambertianSurfaceEmittingCylindricalFiberSourceInput(),
                new LambertianSurfaceEmittingSphericalSourceInput(),
                new CustomSurfaceEmittingSphericalSourceInput(),
                new LambertianSurfaceEmittingCuboidalSourceInput(),
                new LambertianSurfaceEmittingTubularSourceInput(),
                new IsotropicVolumetricCuboidalSourceInput(),
                new CustomVolumetricCuboidalSourceInput(),
                new IsotropicVolumetricEllipsoidalSourceInput(),
                new CustomVolumetricEllipsoidalSourceInput(),
            };
            //Func<Type, ISourceInput> createInstanceOrReturnNull = type =>
            // {
            //     try
            //     {
            //         return (ISourceInput)type.GetConstructor(new Type[] { }).Invoke(new object[] { });
            //         //return (ISourceInput)Activator.CreateInstance(type);
            //     }
            //     catch
            //     {
            //         return null;
            //     }
            // };

            //var allTypes = Assembly.GetExecutingAssembly().GetTypes();

            //foreach (var type in allTypes)
            //{
            //    Console.WriteLine(type);
            //}

            //var validTypes = 
            //    from type in allTypes
            //    where type.IsClass && type.Namespace == nameof(Vts.MonteCarlo.Sources) && type.Name.EndsWith("SourceInput")
            //    select type;

            //var sourceInputs = validTypes.Select(type => createInstanceOrReturnNull(type));

            //return sourceInputs.ToArray();
        }
    }
}
