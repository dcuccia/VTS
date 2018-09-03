using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Vts.Common;
using System;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Implements various commonly used DetectorInput classes.
    /// </summary>
    public class DetectorInputProvider
    {
        /// <summary>
        /// Method that provides instances of all detector inputs in this class.
        /// </summary>
        /// <returns>a list of the IDetectorInputs generated</returns>
        public static IList<IDetectorInput> GenerateAllDetectorInputs()
        {
            return new IDetectorInput[]
            {
                // units space[mm], time[ns], temporal-freq[GHz], abs./scat. coeff[/mm]    
                new AOfRhoAndZDetectorInput() {Rho=new DoubleRange(0.0, 10, 101),Z=new DoubleRange(0.0, 10, 101)},
                new AOfXAndYAndZDetectorInput(){X=new DoubleRange(-10, 10, 201),Y=new DoubleRange(-10, 10, 2),Z=new DoubleRange(0, 10, 101)},
                new ATotalDetectorInput(),
                new FluenceOfRhoAndZAndTimeDetectorInput(){Rho=new DoubleRange(0.0, 10, 101),Z=new DoubleRange(0.0, 10, 101),Time= new DoubleRange(0.0, 10, 101)},
                new FluenceOfRhoAndZDetectorInput() {Rho=new DoubleRange(0.0, 10, 101),Z=new DoubleRange(0.0, 10, 101)},
                new FluenceOfXAndYAndZDetectorInput(){X=new DoubleRange(-10, 10, 201),Y=new DoubleRange(-10, 10, 2),Z=new DoubleRange(0, 10, 101)},
                new FluenceOfXAndYAndZAndOmegaDetectorInput(){X=new DoubleRange(-10, 10, 201),Y=new DoubleRange(-10, 10, 2),Z=new DoubleRange(0, 10, 101),Omega=new DoubleRange(0.0, 1, 21)},
                new RadianceOfRhoAndZAndAngleDetectorInput(){Rho=new DoubleRange(0.0, 10, 101),Z=new DoubleRange(0.0, 10, 101),Angle=new DoubleRange(0, Math.PI, 3)},
                new RadianceOfFxAndZAndAngleDetectorInput(){Fx=new DoubleRange(0.0, 0.5, 51),Z=new DoubleRange(0.0, 10, 101),Angle=new DoubleRange(0, Math.PI, 3)},
                new RadianceOfXAndYAndZAndThetaAndPhiDetectorInput(){
                    X=new DoubleRange(-10.0, 10.0, 101),
                    Y= new DoubleRange(-10.0, 10.0, 101),
                    Z= new DoubleRange(0.0, 10.0, 101),
                    Theta=new DoubleRange(0.0, Math.PI, 5), // theta (polar angle)
                    Phi=new DoubleRange(-Math.PI, Math.PI, 5)}, // phi (azimuthal angle)
                new RadianceOfRhoAtZDetectorInput() {Rho = new DoubleRange(0.0, 10, 101), ZDepth = 3},
                new RDiffuseDetectorInput(),
                new ROfAngleDetectorInput() {Angle=new DoubleRange(Math.PI / 2 , Math.PI, 5)},
                new ROfFxAndTimeDetectorInput() {Fx = new DoubleRange(0.0, 0.5, 51), Time= new DoubleRange(0.0, 10, 101)},
                new ROfFxDetectorInput() {Fx = new DoubleRange(0.0, 0.5, 51)},
                new ROfRhoAndAngleDetectorInput() {Rho=new DoubleRange(0.0, 10, 101),Angle=new DoubleRange(Math.PI / 2 , Math.PI, 5)},
                new ROfRhoAndOmegaDetectorInput() {Rho=new DoubleRange(0.0, 10, 101),Omega=new DoubleRange(0.0, 1, 21)}, // GHz
                new ROfRhoAndTimeDetectorInput() {Rho= new DoubleRange(0.0, 10, 101),Time=new DoubleRange(0.0, 10, 101)},
                new ROfRhoDetectorInput() {Rho =new DoubleRange(0.0, 10, 101)},
                new ROfXAndYDetectorInput() {X=new DoubleRange(-100.0, 100.0, 21), Y= new DoubleRange(-100.0, 100.0, 21)},
                new RSpecularDetectorInput(),
                new TDiffuseDetectorInput(),
                new TOfAngleDetectorInput() {Angle=new DoubleRange(0.0, Math.PI / 2, 5)},
                new TOfRhoAndAngleDetectorInput() {Rho=new DoubleRange(0.0, 10, 101),Angle=new DoubleRange(0.0, Math.PI / 2, 5)},
                new TOfRhoDetectorInput() {Rho=new DoubleRange(0.0, 10, 101)},
                new TOfXAndYDetectorInput() {X=new DoubleRange(-100.0, 100.0, 21), Y= new DoubleRange(-100.0, 100.0, 21)},
                new TOfFxDetectorInput() {Fx = new DoubleRange(0.0, 0.5, 51)},
            };
            //Func<Type, IDetectorInput> createInstanceOrReturnNull = type =>
            // {
            //     try
            //     {
            //         return (IDetectorInput)type.GetConstructor(new Type[] { }).Invoke(new object[] { });
            //         //return (IDetectorInput)Activator.CreateInstance(type);
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
            //    where type.IsClass && type.Namespace == nameof(Vts.MonteCarlo.Detectors) && type.Name.EndsWith("DetectorInput")
            //    select type;

            //var detectorInputs = validTypes.Select(type => createInstanceOrReturnNull(type));

            //return detectorInputs.ToArray();
        }
    }
}
