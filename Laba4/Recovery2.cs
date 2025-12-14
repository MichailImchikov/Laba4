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

        // Стартовая нода: та, у которой расстояния до остальных как можно ближе друг к другу (минимальная дисперсия расстояний)
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

            // Выбираем направление обхода внутри кластера, чтобы соединение было выгоднее
            // Сравниваем расстояния до соседних городов вокруг ближайшего
            int nextIndex = (nearestIndex + 1) % currentCities.Count;
            int prevIndex = (nearestIndex - 1 + currentCities.Count) % currentCities.Count;

            double toNext = nearestCity.DistanceTo(currentCities[nextIndex]);
            double toPrev = nearestCity.DistanceTo(currentCities[prevIndex]);

            bool goForward = toNext <= toPrev; // если следующий ближе или равен, идём вперёд, иначе назад

            if (goForward)
            {
                // Добавляем города начиная с ближайшего и двигаясь вперёд с циклическим обходом
                for (int j = 0; j < currentCities.Count; j++)
                {
                    int idx = (nearestIndex + j) % currentCities.Count;
                    result.Add(currentCities[idx]);
                }
            }
            else
            {
                // Добавляем города начиная сnearestCity и двигаясь назад с циклическим обходом
                for (int j = 0; j < currentCities.Count; j++)
                {
                    int idx = (nearestIndex - j + currentCities.Count) % currentCities.Count;
                    result.Add(currentCities[idx]);
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

        // Жадно выбираем ближайший город
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
