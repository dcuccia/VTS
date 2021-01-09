using System.Numerics;

namespace Vts.Gui.BlazorHybrid.Model
{
    public interface IDataPoint { double X { get; } }
    public record DoubleDataPoint(double X, double Y) : IDataPoint { }
    public record ComplexDataPoint(double X, Complex Y) : IDataPoint { }

}