using System.Drawing;
using System.Linq;

namespace Vts.Gui.BlazorHybrid.Model
{
    /// <summary>
    ///     Class to hold all data necessary for describing a single line plot
    ///     Needs to be bolstered to allow for multiple descriptors, powerful enough
    ///     to support a view of a particular data set
    /// </summary>
    public class PlotData
    {
        public PlotData(IDataPoint[] points, string title)
        {
            Points = points;
            Title = title;
        }

        // todo: temp to get things working...want to eveutally remove
        public PlotData((double x, double y)[] points, string title)
            : this(points.Select(point => new DoubleDataPoint(point.x, point.y)).ToArray(), title)
        {
        }

        public IDataPoint[] Points { get; set; }
        public string Title { get; set; }
    }
}