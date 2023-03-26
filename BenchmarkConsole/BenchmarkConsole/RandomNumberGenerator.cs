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
            //var rngCsp = new RNGCryptoServiceProvider();
            //rngCsp.GetBytes(bytes);
            using (System.Security.Cryptography.RandomNumberGenerator rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return BitConverter.ToInt32(bytes, 0);
        }

        static int GenerateRandomNumber(int minValue, int maxValue)
        {
            // Create a byte array to store the random number
            byte[] randomNumberBuffer = new byte[4];

            // Fill the byte array with a random number
            using (System.Security.Cryptography.RandomNumberGenerator rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumberBuffer);
            }

            // Convert the byte array to a 32-bit integer
            int result = BitConverter.ToInt32(randomNumberBuffer, 0);

            // Adjust the random number to be within the specified range
            result = Math.Abs(result % (maxValue - minValue)) + minValue;

            return result;
        }
    }
}
