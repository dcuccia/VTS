namespace Vts.SiteVisit.ViewModel
{
    /// <summary>
    /// Data structure to hold data for plot information
    /// </summary>
    public class PlotAxesLabels
    {
        public string IndependentAxisUnits { get; set; }
        public string IndependentAxisName { get; set; }
        public string DependentAxisUnits { get; set; }
        public string DependentAxisName { get; set; }
        public string ConstantAxisName { get; set; }
        public string ConstantAxisUnits { get; set; }
        public double ConstantAxisValue { get; set; }

        public PlotAxesLabels(
            string independentAxisName, string independentAxisUnits,
            string dependentAxisName, string dependentAxisUnits,
            string constantAxisName, string constantAxisUnits, double constantAxisValue)
        {
            IndependentAxisName = independentAxisName;
            IndependentAxisUnits = independentAxisUnits;
            DependentAxisName = dependentAxisName;
            DependentAxisUnits = dependentAxisUnits;
            ConstantAxisName = constantAxisName;
            ConstantAxisUnits = constantAxisUnits;
            ConstantAxisValue = constantAxisValue;
        }
        public PlotAxesLabels(
            string independentAxisName, string independentAxisUnits,
            string dependentAxisName, string dependentAxisUnits)
            : this(independentAxisName, independentAxisUnits, dependentAxisName, dependentAxisUnits, "", "", 0) { }
    }
}
