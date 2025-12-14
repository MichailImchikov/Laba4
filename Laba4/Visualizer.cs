using ScottPlot;

class Visualizer
{
    private static readonly ScottPlot.Color[] ClusterColors = new[]
    {
        Colors.Red,
        Colors.Blue,
        Colors.Green,
        Colors.Orange,
        Colors.Purple,
        Colors.Cyan,
        Colors.Magenta,
        Colors.Brown,
        Colors.Pink,
        Colors.Lime,
        Colors.Teal,
        Colors.Navy,
        Colors.Olive,
        Colors.Maroon,
        Colors.Aqua,
        Colors.Silver
    };

    public void SaveRoutePlot(List<City> route, List<Node> clusters, string fileName, string title = "Маршрут")
    {
        var plot = new Plot();
        plot.Title(title);
        plot.XLabel("X");
        plot.YLabel("Y");

        if (route.Count == 0)
        {
            plot.SavePng(fileName, 800, 600);
            return;
        }

        // Создаём словарь: город -> индекс кластера
        var cityToCluster = new Dictionary<City, int>();
        var allClusters = GetAllLeafClusters(clusters);
        
        for (int clusterIndex = 0; clusterIndex < allClusters.Count; clusterIndex++)
        {
            var clusterCities = allClusters[clusterIndex].GetAllCity();
            foreach (var city in clusterCities)
            {
                cityToCluster[city] = clusterIndex;
            }
        }

        // Рисуем линии маршрута (серым цветом)
        var xs = route.Select(c => c.X).ToArray();
        var ys = route.Select(c => c.Y).ToArray();
        
        var line = plot.Add.Scatter(xs, ys);
        line.Color = Colors.Gray;
        line.LineWidth = 1;
        line.MarkerSize = 0;

        // Соединяем последнюю точку с первой
        if (route.Count > 1)
        {
            var closeXs = new double[] { xs[^1], xs[0] };
            var closeYs = new double[] { ys[^1], ys[0] };
            var closeLine = plot.Add.Scatter(closeXs, closeYs);
            closeLine.Color = Colors.Gray;
            closeLine.LineWidth = 1;
            closeLine.MarkerSize = 0;
        }

        // Добавляем номера кластеров вместо точек
        foreach (var city in route)
        {
            int clusterIndex = cityToCluster.GetValueOrDefault(city, 0);
            var color = ClusterColors[clusterIndex % ClusterColors.Length];
            
            var text = plot.Add.Text($"{clusterIndex + 1}", city.X, city.Y);
            text.LabelFontSize = 12;
            text.LabelFontColor = color;
            text.LabelBold = true;
            text.LabelAlignment = Alignment.MiddleCenter;
        }

        // Выделяем начальную точку
        var startMarker = plot.Add.Marker(xs[0], ys[0]);
        startMarker.Color = Colors.Black;
        startMarker.Size = 18;
        startMarker.Shape = MarkerShape.OpenSquare;

        // Выделяем конечную точку
        var endMarker = plot.Add.Marker(xs[^1], ys[^1]);
        endMarker.Color = Colors.Black;
        endMarker.Size = 18;
        endMarker.Shape = MarkerShape.OpenTriangleUp;

        plot.SavePng(fileName, 1200, 800);
    }

    private List<Node> GetAllLeafClusters(List<Node> nodes)
    {
        var result = new List<Node>();
        
        foreach (var node in nodes)
        {
            if (node.Components.Count > 0 && node.Components[0] is City)
            {
                // Это листовой кластер (содержит города напрямую)
                result.Add(node);
            }
            else
            {
                // Рекурсивно получаем листовые кластеры
                var subNodes = node.Components.OfType<Node>().ToList();
                result.AddRange(GetAllLeafClusters(subNodes));
            }
        }
        
        return result;
    }
}
