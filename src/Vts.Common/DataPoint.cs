using System.Numerics;

namespace Vts
{
    public interface IDataPoint { double X { get; } }
    public record DoubleDataPoint(double X, double Y) : IDataPoint { }
    public record ComplexDataPoint(double X, Complex Y) : IDataPoint { }
}