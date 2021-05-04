using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;

namespace BenchmarkConsole
{
    [Config(typeof(Config))]
    [MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter, CsvMeasurementsExporter]
    public class MovingAverageImplementations
    {

        private class Config : ManualConfig
        {
            public Config()
            {
                AddExporter(CsvMeasurementsExporter.Default);
                AddExporter(RPlotExporter.Default);
            }
        }

        public float[] Data { get; private set; }

        [Params(10, 20)] public int FrameSize { get; set; }

        [Params(100, 1000, 10000)] public int CountOfNumber { get; set; }

        public MovingAverageImplementations() : this(1000, 10)
        {
        }

        public MovingAverageImplementations(int countOfNumber, int frameSize)
        {
            FrameSize = frameSize;
            CountOfNumber = countOfNumber;
            Data = RandomNumberGenerator.GenerateRandomFloatArray(CountOfNumber);
        }

        [Benchmark]
        public float[] MovingAverageKeepingBufferAndFrameLengthSame()
        {
            var dataList = new List<float>();
            var buffer = new float[FrameSize];
            var currentIndex = 0;
            var isBufferFilledAtleastOnce = false;
            foreach (float dataValue in Data)
            {
                buffer[currentIndex] = dataValue;
                var circularBufferSum = 0.0f;
                for (var j = 0; j < FrameSize; j++)
                {
                    circularBufferSum += buffer[j];
                }

                var movingAverage = isBufferFilledAtleastOnce
                    ? (circularBufferSum / FrameSize)
                    : (circularBufferSum / (currentIndex + 1));
                dataList.Add(movingAverage);

                currentIndex = (currentIndex + 1) % FrameSize;
                if (!isBufferFilledAtleastOnce && currentIndex == FrameSize)
                    isBufferFilledAtleastOnce = true;
            }

            return dataList.ToArray();
        }

        [Benchmark]
        public float[] MovingAverageUsingNestedLoop()
        {
            var dataList = new List<float>();

            for (var outerLoopCounter = 0; outerLoopCounter < Data.Count(); outerLoopCounter++)
            {
                if (outerLoopCounter < FrameSize - 1) continue;
                var total = 0.0f;
                for (var innerLoopCounter = outerLoopCounter;
                    innerLoopCounter > (outerLoopCounter - FrameSize);
                    innerLoopCounter--)
                    total += Data[innerLoopCounter];
                var average = (total * 1.0f) / FrameSize;
                dataList.Add(average);
            }

            return dataList.ToArray();
        }

        [Benchmark]
        public float[] MovingAverageUsingQueue()
        {
            var dataList = new List<float>();
            var frameSizedQueue = new Queue<float>(FrameSize);
            foreach (var dataItem in Data)
            {
                if (frameSizedQueue.Count == FrameSize)
                {
                    frameSizedQueue.Dequeue();
                }

                frameSizedQueue.Enqueue(dataItem);
                dataList.Add(frameSizedQueue.Average());
            }

            return dataList.ToArray();
        }

        [Benchmark]
        public float[] MovingAverageUsingLinq()
        {
            return Enumerable
                .Range(0, Data.Length - FrameSize)
                .Select(n => Data.Skip(n).Take(FrameSize).Average())
                .ToArray();
        }

        [Benchmark]
        public float[] GetMovingAverageWithDeltaSumManipulation()
        {

            var dataList = new List<float>();
            var currentFrameSize = 0;
            float cumulativeSum = 0;
            for (int dataItemIndexCounter = 0; dataItemIndexCounter < Data.Length; dataItemIndexCounter++)
            {
                var indexForDataItemToBeRemoved = dataItemIndexCounter - FrameSize;

                if (indexForDataItemToBeRemoved >= 0)
                {
                    cumulativeSum -= Data[indexForDataItemToBeRemoved];
                    currentFrameSize--;
                }

                if (dataItemIndexCounter < Data.Length)
                {
                    cumulativeSum += Data[dataItemIndexCounter];
                    currentFrameSize++;
                }

                //Console.WriteLine(cumulativeSum + " : " + currentFrameSize);
                dataList.Add(cumulativeSum / (currentFrameSize * 1.0f));
            }

            return dataList.ToArray();
        }


        [Benchmark]
        public float[] GetMovingAverageUsingArray()
        {

            var movingAverage = new float[Data.Length];
            if (Data.Length == 0) return movingAverage; //return empty array

            movingAverage[0] = Data[0];

            for (var i = 1; i < Data.Length; i++)
            {
                if (i <= FrameSize - 1)
                {
                    movingAverage[i] = movingAverage[i - 1] * (i / (i + 1f)) + (Data[i] / (i + 1f));
                }
                else
                {
                    movingAverage[i] = movingAverage[i - 1] + (Data[i] / (FrameSize * 1.0f)) -
                                       Data[i - FrameSize] / FrameSize * 1.0f;
                }
            }

            return movingAverage;
        }

        [Benchmark]
        public float[] GetMovingAverageUsingParallelLinq()
        {
            float[] result = new float[Data.Length];

            Parallel.ForEach(Data,
                (value, pls, index) => { result[index] = Data.Skip((int) index).Take(FrameSize).Average(); });

            return result;

        }
    }
}
