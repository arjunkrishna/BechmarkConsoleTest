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
