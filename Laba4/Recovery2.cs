using System.Xml;

class Recovery2 : IRecovery
{
    public List<City> Recover(List<Node> nodes)
    {
        var newNode = new Node();
        newNode.Components = SearchLastNode(nodes);
        return newNode.GetAllCity();

    }
    private List<Component> SearchLastNode(List<Node> nodes)
    {
        List<Node> clasters = new ();
        foreach (var node in nodes)
        {
            if (node.Components[0] is City)
            {
                var cities = node.GetAllCity();
                var greadCity = Greed(cities);
                var newNode = new Node();
                newNode.Components = greadCity.ToList();
                clasters.Add(newNode);
            }
            else
            {
                var newListCity = SearchLastNode(node.Components.OfType<Node>().ToList());
                var newNode = new Node();
                newNode.Components = newListCity.ToList();
                clasters.Add(newNode);
            }
        }
        return SumNode(clasters);
    }
    
    private List<Node> GreedNodes(List<Node> nodes)
    {
        if (nodes.Count == 0)
            return new List<Node>();

        var route = new List<Node>();
        var remaining = nodes.ToList();

        Node current = remaining
                    .OrderBy(n =>
                    {
                        var dists = remaining.Where(o => !ReferenceEquals(o, n)).Select(o => n.DistanceTo(o)).ToList();
                        if (dists.Count == 0) return 0; // если одна нода
                        double mean = dists.Average();
                        double variance = dists.Select(d => (d - mean) * (d - mean)).Average();
                        return variance;
                    })
                    .First();
        route.Add(current);
        remaining.Remove(current);

            while (remaining.Count > 0)
            {
                Node nearest = remaining[0];
                double minDistance = current.DistanceTo(nearest);

                foreach (var node in remaining)
                {
                    double distance = current.DistanceTo(node);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearest = node;
                    }
                }

                route.Add(nearest);
                remaining.Remove(nearest);
                current = nearest;
        }

        return route;
    }

    private List<Component> SumNode(List<Node> nodes)
    {
        if (nodes.Count == 0)
            return new List<Component>();

        List<Node> orderedNodes = GreedNodes(nodes);
        var result = new List<Component>();

        var firstCluster = orderedNodes[0];
        result.AddRange(firstCluster.Components);

        for (int i = 1; i < orderedNodes.Count; i++)
        {

            var lastCity = result.Last() as City;
            if (lastCity == null) continue;

            var currentCities = orderedNodes[i].GetAllCity().ToList();
            if (currentCities.Count == 0) continue;

            City nearestCity = currentCities[0];
            double minDistance = lastCity.DistanceTo(nearestCity);
            int nearestIndex = 0;

            for (int j = 0; j < currentCities.Count; j++)
            {
                double distance = lastCity.DistanceTo(currentCities[j]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestCity = currentCities[j];
                    nearestIndex = j;
                }
            }

            if(i == orderedNodes.Count -1 )
            {
                for (int j = nearestIndex; j < currentCities.Count; j++)
                {
                    result.Add(currentCities[j]);
                }

                for (int j = 0; j < nearestIndex; j++)
                {
                    result.Add(currentCities[j]);
                }
            }
            else
            {
                var nextCluster = orderedNodes[i +1];

                int nextIndex = (nearestIndex + 1) % currentCities.Count;
                int prevIndex = (nearestIndex - 1 + currentCities.Count) % currentCities.Count;

                double toNext = nextCluster.GetAllCity().Select(x=> x.DistanceTo(currentCities[nextIndex])).Min() ;
                toNext += currentCities[nearestIndex].DistanceTo(currentCities[(nearestIndex - 1 + currentCities.Count) % currentCities.Count]);
                double toPrev = nextCluster.GetAllCity().Select(x => x.DistanceTo(currentCities[prevIndex])).Min();
                toPrev += currentCities[nearestIndex].DistanceTo(currentCities[(nearestIndex + 1) % currentCities.Count]);

                bool goForward = toNext <= toPrev;

                if (goForward)
                {
                    for (int j = 0; j < currentCities.Count; j++)
                    {
                        int idx = (nearestIndex - j + currentCities.Count) % currentCities.Count;
                        
                        result.Add(currentCities[idx]);
                    }
                }
                else
                {
                    for (int j = 0; j < currentCities.Count; j++)
                    {
                        int idx = (nearestIndex + j) % currentCities.Count;
                        result.Add(currentCities[idx]);
                    }
                }
            }

        }

        return result;
    }

    private List<Component> Greed(List<City> cities)
    {
        if (cities.Count == 0)
            return new List<Component>();

        var route = new List<Component>();
        var remaining = cities.ToList();


        route.Add(remaining.First());
        remaining.Remove(remaining.First());
        while (remaining.Count > 0)
        {
            City nearest = remaining[0];
            double minDistance = route.Last().DistanceTo(nearest);

            foreach (var city in remaining)
            {
                double distance = route.Last().DistanceTo(city);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = city;
                }
            }

            route.Add(nearest);
            remaining.Remove(nearest);
        }

        return route;
    }
}
