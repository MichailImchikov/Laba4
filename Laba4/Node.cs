class Node : Component
{
    public List<Component> Components = new List<Component>();

    public override List<City> GetAllCity()
    {
        var cities = new List<City>();
        foreach (var component in Components)
        {
            cities.AddRange(component.GetAllCity());
        }
        return cities;
    }

    public override Position GetPosition()
    {
        var x = Components.Average(c => c.GetPosition().X);
        var y = Components.Average(c => c.GetPosition().Y);
        return new Position { X = x, Y = y};
    }

}

