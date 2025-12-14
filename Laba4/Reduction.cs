class Reduction : IReduction
{
    private const int MaxComponentsPerCluster = 16;

    public List<Node> Clusterize(Node inputNode, int clusterCount = 8)
    {
        return ClusterizeRecursive(inputNode, clusterCount);
    }

    private List<Node> ClusterizeRecursive(Node inputNode, int clusterCount)
    {
        var components = inputNode.Components.ToList();

        // Находим наиболее отдалённые точки
        var seedComponents = FindMostDistantComponents(components, clusterCount);
        
        // Создаём новые Node и добавляем в каждый по одной seed-точке
        var clusters = new List<Node>();
        foreach (var seed in seedComponents)
        {
            var node = new Node();
            node.Components.Add(seed);
            clusters.Add(node);
        }

        // Оставшиеся компоненты
        var remaining = components.Except(seedComponents).ToList();

        // Распределяем оставшиеся компоненты по ближайшим кластерам
        foreach (var component in remaining)
        {
            var nearestCluster = clusters
                .OrderBy(cluster => component.DistanceTo(cluster))
                .First();
            
            nearestCluster.Components.Add(component);
        }

        var finalClusters = new List<Node>();
        foreach (var cluster in clusters)
        {
            if (cluster.Components.Count > MaxComponentsPerCluster)
            {
                // Рекурсивно кластеризуем
                var subClusters = ClusterizeRecursive(cluster, clusterCount);
                
                // Создаём Node с подкластерами внутри
                var parentNode = new Node();
                foreach (var subCluster in subClusters)
                {
                    parentNode.Components.Add(subCluster);
                }
                finalClusters.Add(parentNode);
            }
            else
            {
                finalClusters.Add(cluster);
            }
        }

        return finalClusters;
    }

    private List<Component> FindMostDistantComponents(List<Component> components, int count)
    {
        var result = new List<Component>();
        var available = components.ToList();

        // Берём первую точку (можно взять любую, например с максимальной суммой расстояний)
        var first = available
            .OrderByDescending(c => available.Sum(other => c.DistanceTo(other)))
            .First();
        
        result.Add(first);
        available.Remove(first);

        // Находим остальные точки, максимально отдалённые от уже выбранных
        while (result.Count < count && available.Count > 0)
        {
            var farthest = available
                .OrderByDescending(c => result.Min(selected => c.DistanceTo(selected)))
                .First();
            
            result.Add(farthest);
            available.Remove(farthest);
        }

        return result;
    }
}

