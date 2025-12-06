using System.Globalization;

class Reader
{
    public List<Node> ReadAllTasks(string dataFolderPath)
    {
        var nodes = new List<Node>();
        
        if (!Directory.Exists(dataFolderPath))
        {
            Console.WriteLine($"Папка {dataFolderPath} не найдена.");
            return nodes;
        }

        var txtFiles = Directory.GetFiles(dataFolderPath, "*.txt");
        
        foreach (var file in txtFiles)
        {
            var node = ReadTask(file);
            if (node != null)
            {
                nodes.Add(node);
            }
        }

        return nodes;
    }

    public Node? ReadTask(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Файл {filePath} не найден.");
            return null;
        }

        var node = new Node();
        var lines = File.ReadAllLines(filePath);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            
            var partsNew =parts.Select(p => p.Replace(".",",")).ToList();
            if (partsNew.Count >= 3)
            {
                if (double.TryParse(partsNew[1], out double x) &&
                    double.TryParse(partsNew[2], out double y))
                {
                    var city = new City(x, y);
                    node.Components.Add(city);
                }
            }
            // Или формат: x y (без номера)
            else if (parts.Count >= 2)
            {
                if (float.TryParse(parts[0], CultureInfo.InvariantCulture, out float x) && 
                    float.TryParse(parts[1], CultureInfo.InvariantCulture, out float y))
                {
                    var city = new City(x, y);
                    node.Components.Add(city);
                }
            }
        }

        return node;
    }
}
