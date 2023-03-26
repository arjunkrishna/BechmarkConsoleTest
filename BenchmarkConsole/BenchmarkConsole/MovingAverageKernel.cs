using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputeSharp;
using ComputeSharp.__Internals;

namespace BenchmarkConsole
{

    struct DataItem
    {
        public double Value;
        public double Average;
    }

    [AutoConstructor]
    [EmbeddedBytecode(DispatchAxis.X)]
    readonly partial struct MovingAverageKernel : IComputeShader
    {
        public readonly ReadOnlyBuffer<DataItem> Input;
        public readonly ReadWriteBuffer<DataItem> Output;
        public readonly int WindowSize;

        //public void Execute()
        //{
        //    var index = ThreadIds.X;
        //    double sum = 0;

        //    // Use ArrayPool to allocate a temporary buffer for the input data
        //    var values = ArrayPool<double>.Shared.Rent(WindowSize);

        //    // Use ReadOnlySpan to access the input data
        //    var inputSpan = Input.Span;

        //    // Fill the buffer with input data
        //    for (var i = 0; i < WindowSize; i++)
        //    {
        //        var j = index - WindowSize + 1 + i;
        //        if (j >= 0 && j < Input.Length)
        //        {
        //            values[i] = inputSpan[j].Value;
        //        }
        //        else
        //        {
        //            values[i] = 0;
        //        }
        //    }

        //    // Calculate the moving average
        //    for (var i = 0; i < WindowSize; i++)
        //    {
        //        sum += values[i];
        //    }

        //    Output[index].Value = inputSpan[index].Value;
        //    Output[index].Average = sum / Math.Min(WindowSize, index + 1);

        //    // Return the temporary buffer to the ArrayPool
        //    ArrayPool<double>.Shared.Return(values);
        //}



        public void Execute()
        {
            int index = ThreadIds.X;
            double sum = 0;

            for (int i = index - WindowSize + 1; i <= index; i++)
            {
                if (i >= 0 && i < Input.Length)
                {
                    sum += Input[i].Value;
                }
            }

            Output[index].Value = Input[index].Value;
            Output[index].Average = sum / Math.Min(WindowSize, index + 1);
        }
    }




}
