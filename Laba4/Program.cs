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

            // Выводим информацию в консоль
            for (int i = 0; i < tasks.Count; i++)
            {
                var (fileName, node) = tasks[i];
                Console.WriteLine($"Задача {i + 1}: {fileName}");
                var clusters = reduction.Clusterize(node);

                var sw1 = System.Diagnostics.Stopwatch.StartNew();
                var recoveredNode1 = recovery.Recover(clusters);
                sw1.Stop();
                double sum1 = 0;
                for(int j = 0; j < recoveredNode1.Count - 1; j++)
                {
                    sum1 += recoveredNode1[j].DistanceTo(recoveredNode1[j + 1]);
                }
                sum1 += recoveredNode1[0].DistanceTo(recoveredNode1[recoveredNode1.Count - 1]);

                var sw2 = System.Diagnostics.Stopwatch.StartNew();
                var recoveredNode2 = recovery2.Recover(clusters);
                sw2.Stop();
                double sum2 = 0;
                for(int j = 0; j < recoveredNode2.Count - 1; j++)
                {
                    sum2 += recoveredNode2[j].DistanceTo(recoveredNode2[j + 1]);
                }
                sum2 += recoveredNode2[0].DistanceTo(recoveredNode2[recoveredNode2.Count - 1]);

                Console.WriteLine($"Recovery1: длина пути = {sum1:F2}, время = {sw1.ElapsedMilliseconds} ms");
                Console.WriteLine($"Recovery2: длина пути = {sum2:F2}, время = {sw2.ElapsedMilliseconds} ms");
                
                // Сохраняем график маршрута с отметкой кластеров (по первому алгоритму)
                var plotFileName1 = Path.Combine("Output", $"{fileName}_v1.png");
                visualizer.SaveRoutePlot(recoveredNode1, clusters, plotFileName1, $"Маршрут v1: {fileName} (длина: {sum1:F2})");

                // Сохраняем график маршрута для второй версии
                var plotFileName2 = Path.Combine("Output", $"{fileName}_v2.png");
                visualizer.SaveRoutePlot(recoveredNode2, clusters, plotFileName2, $"Маршрут v2: {fileName} (длина: {sum2:F2})");
                
                Console.WriteLine();
            }
        }
    }
}
