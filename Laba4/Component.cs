
using System.Numerics;

abstract class Component
{
    public abstract Position GetPosition();
    public double DistanceTo(Component other)
    {
        var pos1 = GetPosition();
        var pos2 = other.GetPosition();
        var dx = pos1.X - pos2.X;
        var dy = pos1.Y - pos2.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
    public abstract List<City> GetAllCity();
}
public class Position
{
    public double X;
    public double Y;
}
