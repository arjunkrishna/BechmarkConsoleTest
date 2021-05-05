using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkConsole
{
    public class RandomNumberGenerator
    {
        //public static float[] GenerateRandomFloatArray(int countOfNumber)
        //{

        //    float[] result = new float[countOfNumber];
        //    for (int i = 0; i < countOfNumber; i++)
        //    {
        //        result[i] = i;
        //    }

        //    return result;
        //}

        public static float[] GenerateRandomFloatArray(int countOfNumber)
        {
            var random = new Random();
            float[] result = new float[countOfNumber];
            float[] array = ArrayPool<float>.Shared.Rent(countOfNumber);

            foreach (ref float x in array.AsSpan())
            {
                //x = NextFloat(random);
                x = (float)random.NextDouble();
            }

            Array.Copy(array, result, countOfNumber);
            ArrayPool<float>.Shared.Return(array);

            return result;
        }

        private static float NextFloat(Random random)
        {
            double mantissa = (random.NextDouble() * 2.0) - 1.0;
            double exponent = Math.Pow(2.0, random.Next(-126, 128));
            return (float)(mantissa * exponent);
        }
    }
}
