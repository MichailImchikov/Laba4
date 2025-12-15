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
            var recovery2 = new Recovery2();
            var visualizer = new Visualizer();

            // Создаём папку для графиков
            if (!Directory.Exists("Output"))
            {
                Directory.CreateDirectory("Output");
            }

            for (int i = 0; i < tasks.Count; i++)
            {
                var (fileName, node) = tasks[i];
                Console.WriteLine($"Задача {i + 1}: {fileName}");

                // Сценарии на базе Reduction
                RunScenario(node, fileName, reduction, recovery, visualizer, "Reduction+Recovery1");
                RunScenario(node, fileName, reduction, recovery2, visualizer, "Reduction+Recovery2");


                Console.WriteLine();
            }
        }

        private static void RunScenario(Node node, string fileName, IReduction reduction, IRecovery recovery, Visualizer visualizer, string label)
        {
            var clusters = reduction.Clusterize(node);
            var route = recovery.Recover(clusters);
            double length = 0;
            for (int j = 0; j < route.Count - 1; j++)
                length += route[j].DistanceTo(route[j + 1]);
            if (route.Count > 1)
                length += route[0].DistanceTo(route[^1]);

            Console.WriteLine($"{label}: длина пути = {length:F2}");

            var plotFileName = Path.Combine("Output", $"{fileName}_{label.Replace('+', '_').ToLower()}.png");
            visualizer.SaveRoutePlot(route, clusters, plotFileName, $"{label}: {fileName} (длина: {length:F2})");
        }
    }
}
