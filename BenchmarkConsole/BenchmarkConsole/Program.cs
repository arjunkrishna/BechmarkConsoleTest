using System;
using BenchmarkDotNet.Running;

namespace BenchmarkConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<MovingAverageImplementations>();
            Console.ReadLine();
        }
    }
}
