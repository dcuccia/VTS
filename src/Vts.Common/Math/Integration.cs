using System;

namespace Vts.Common.Math
{
    public static class Integration
    {
        /// <summary>
        /// Integration in one dimension
        /// </summary>
        /// <param name="f">Function to integrate</param>
        /// <param name="a">Lower limit of integration</param>
        /// <param name="b">Upper limit of integration</param>
        /// <param name="epsilon">Accuracy required)</param>
        /// 
        public static double IntegrateAdaptiveSimpsonRule(Func<double, double> f, double a, double b, double epsilon)
        {
            //calculates integral of f from a to b with max error of epsilon
            return AdaptiveRecursiveSimpson(f, a, b, epsilon, IntegrateSimpsonRule(f, a, b));
        }
        public static double IntegrateSimpsonRule(Func<double, double> f, double a, double b)
        {
            double c = (a + b) / 2.0;
            double h3 = System.Math.Abs(b - a) / 6.0;
            double S = h3 * (f(a) + 4.0 * f(c) + f(b));
            return S;
        }
        public static double AdaptiveRecursiveSimpson(Func<double, double> f, double a, double b, double epsilon, double sum)
        {
            double c = (a + b) / 2.0;
            double left = IntegrateSimpsonRule(f, a, c);
            double right = IntegrateSimpsonRule(f, c, b);
            double R;
            if (System.Math.Abs(left + right - sum) <= 15.0 * epsilon)
                R = left + right + (left + right - sum) / 15.0;
            else
                R = AdaptiveRecursiveSimpson(f, a, c, epsilon / 2.0, left) + AdaptiveRecursiveSimpson(f, c, b, epsilon / 2.0, right);
            return R;
        }
    }
}
