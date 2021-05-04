using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkConsole
{
    public class RandomNumberGenerator
    {
        public static float[] GenerateRandomFloatArray(int countOfNumber)
        {
            var randNum = new Random();
            return Enumerable
                .Repeat(0, countOfNumber)
                .Select<int, float>(i => NextFloat(randNum))
                .ToArray();
        }

        private static float NextFloat(Random random)
        {
            double mantissa = (random.NextDouble() * 2.0) - 1.0;
            double exponent = Math.Pow(2.0, random.Next(-126, 128));
            return (float)(mantissa * exponent);
        }
    }
}
