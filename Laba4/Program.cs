namespace Laba4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var reader = new Reader();
            var tasks = reader.ReadAllTasks("Data");

            var reduction = new Reduction();
            var recovery = new Recovery();
            var visualizer = new Visualizer();

            // Создаём папку для графиков
            if (!Directory.Exists("Output"))
            {
                Directory.CreateDirectory("Output");
            }

            // Выводим информацию в консоль
            for (int i = 0; i < tasks.Count; i++)
            {
                var (fileName, node) = tasks[i];
                Console.WriteLine($"Задача {i + 1}: {fileName}");
                var clusters = reduction.Clusterize(node);
                var recoveredNode = recovery.Recover(clusters);
                double sum = 0;
                for(int j = 0; j < recoveredNode.Count - 1; j++)
                {
                    sum += recoveredNode[j].DistanceTo(recoveredNode[j + 1]);
                }
                sum += recoveredNode[0].DistanceTo(recoveredNode[recoveredNode.Count - 1]);

                Console.WriteLine($"Сумма пути: {sum}");
                
                // Сохраняем график маршрута с отметкой кластеров
                var plotFileName = Path.Combine("Output", $"{fileName}.png");
                visualizer.SaveRoutePlot(recoveredNode, clusters, plotFileName, $"Маршрут: {fileName} (длина: {sum:F2})");
                
                Console.WriteLine();
            }
        }
    }
}
