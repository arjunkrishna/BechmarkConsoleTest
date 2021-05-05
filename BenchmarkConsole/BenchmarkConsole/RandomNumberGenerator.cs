using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkConsole
{
    public class RandomNumberGenerator
    {
        public static double[] GenerateRandomArray(int countOfNumber)
        {
            var random = new Random(GenerateSeed());
            var result = new double[countOfNumber];
            var array = ArrayPool<double>.Shared.Rent(countOfNumber);

            foreach (ref var x in array.AsSpan())
            {
                x = random.NextDouble();
            }

            Array.Copy(array, result, countOfNumber);
            ArrayPool<double>.Shared.Return(array);

            return result;
        }

        public static int GenerateSeed()
        {
            byte[] bytes = new byte[4];
            var rngCsp = new RNGCryptoServiceProvider();
            rngCsp.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
}
}
