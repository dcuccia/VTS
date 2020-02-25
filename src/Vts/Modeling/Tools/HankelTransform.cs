using System;
using System.Reflection;
using Vts.IO;

namespace Vts.Modeling
{
    /// <summary>
    /// Evaluate the Hankel transform using a digital fitler method for the quadrature points.
    /// Necessary to have a continuous function for transformation.
    /// </summary>
    public class HankelTransform
    {

        /// <summary>
        /// Zero order Hankel Transform of Anayltic expression by digital filtering. 
        /// Based on a Matlab code by Prof. Brian Borcher which is based on a Fortran program by Walt Anderson 
        /// which was published as
        /// Anderson, W.L., 1979, Computer Program Numerical Integration of Related Hankel
        /// Transforms of Orders 0 and 1 by Adaptive Digital Filtering.
        /// Geophysic, 44(7):1287-1305.
        /// Actual weights used in this code are from a later updated version of the code
        /// </summary>
        /// <returns>IEnumerable corresponding to points being transformed</returns>
        const int dataLength = 801;
        static double[] hankelPoints = new double[dataLength];
        static double[] hankelWeights = new double[dataLength];

        static HankelTransform()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            string dataLocation = "Modeling/Resources/HankelData/";
            //read input quadrature data
            hankelPoints = (double[])FileIO.ReadArrayFromBinaryInResources<double>
               (dataLocation + @"basepoints.dat", assemblyName, dataLength);
            hankelWeights = (double[])FileIO.ReadArrayFromBinaryInResources<double>
                (dataLocation + @"hankelweights.dat", assemblyName, dataLength);
        }

        public static double DigitalFilterOfOrderZero(double varInt, Func<double, double> func)
        {
            if (varInt == 0)
                varInt = 1e-9;
            double sum = 0.0;
            double scaledHankelPoint;
            for (int i = 0; i < dataLength; i++)
            {
                scaledHankelPoint = hankelPoints[i] / varInt;
                sum += func(scaledHankelPoint) * hankelWeights[i] * scaledHankelPoint;
            }
            return sum / varInt;
        }
    }
}
