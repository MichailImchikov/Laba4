

class City : Component
{
    public double X;
    public double Y;
    private Position position;
    public City(double x, double y)
    {
        X = x;
        Y = y;
        position = new Position { X = x, Y = y };
    }

    public override List<City> GetAllCity()
    {
        return new List<City> { this };
    }

    public override Position GetPosition()
    {
        return position;
    }
}

