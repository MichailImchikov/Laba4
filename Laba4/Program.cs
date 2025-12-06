using System.Reflection.PortableExecutable;

namespace Laba4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var reader = new Reader();
            var nodes = reader.ReadAllTasks("Data");
        }
    }
}
