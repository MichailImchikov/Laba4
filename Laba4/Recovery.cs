class Recovery : IRecovery
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

        // Начинаем с первой ноды
        var current = remaining[0];
        route.Add(current);
        remaining.Remove(current);

        // Жадно выбираем ближайшую ноду
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

        // Добавляем все города из первого кластера
        var firstCluster = orderedNodes[0];
        result.AddRange(firstCluster.Components);

        // Соединяем остальные кластера
        for (int i = 1; i < orderedNodes.Count; i++)
        {
            var prevCluster = orderedNodes[i - 1];
            var currentCluster = orderedNodes[i];

            // Последний город из предыдущего кластера
            var lastCity = result.Last() as City;
            if (lastCity == null) continue;

            // Находим ближайший город в текущем кластере к последнему городу
            var currentCities = currentCluster.GetAllCity().ToList();
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

            // Добавляем города начиная с ближайшего и до конца
            for (int j = nearestIndex; j < currentCities.Count; j++)
            {
                result.Add(currentCities[j]);
            }
            // Затем добавляем города с начала до ближайшего
            for (int j = 0; j < nearestIndex; j++)
            {
                result.Add(currentCities[j]);
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
            City nearest = remaining.First();
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

