using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Order;

namespace BenchmarkConsole
{
    //[Config(typeof(Config))]
    //[MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter, CsvMeasurementsExporter]
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class MovingAverageImplementations
    {
        private ImmutableQueue<double> _immutableQueue;

        //private class Config : ManualConfig
        //{
        //    public Config()
        //    {
        //        AddExporter(CsvMeasurementsExporter.Default);
        //        AddExporter(RPlotExporter.Default);
        //    }
        //}

        public double[] Data { get; private set; }

        //[Params(10, 20)] 
        public int FrameSize { get; set; }

        //[Params(100, 1000, 10000)] 
        public int CountOfNumber { get; set; }

        public MovingAverageImplementations() : this(1000, 20)
        {
        }

        public MovingAverageImplementations(int countOfNumber, int frameSize)
        {
            FrameSize = frameSize;
            CountOfNumber = countOfNumber;
            Data = RandomNumberGenerator.GenerateRandomArray(CountOfNumber);
        }

        /*

        //[Benchmark]
        public double[] MovingAverageLinq()
        {
            return Enumerable
                .Range(0, Data.Length - FrameSize)
                .Select(n => Data.Skip(n).Take(FrameSize).Average())
                .ToArray();
        }

        //[Benchmark]
        public double[] MovingAverageParallelLinq()
        {
            double[] result = new double[Data.Length];

            Parallel.ForEach(Data,
                (value, pls, index) => { result[index] = Data.Skip((int)index).Take(FrameSize).Average(); });

            return result;
        }

        //[Benchmark]
        public double[] MovingAverageNestedLoop()
        {
            var dataList = new List<double>();

            for (var outerLoopCounter = 0; outerLoopCounter < Data.Count(); outerLoopCounter++)
            {
                if (outerLoopCounter < FrameSize - 1) continue;
                var total = 0.0d;
                for (var innerLoopCounter = outerLoopCounter;
                    innerLoopCounter > (outerLoopCounter - FrameSize);
                    innerLoopCounter--)
                    total += Data[innerLoopCounter];
                var average = (total * 1.0f) / FrameSize;
                dataList.Add(average);
            }

            return dataList.ToArray();
        }

        //[Benchmark]
        public double[] MovingAverageBufferAndFrameLengthSame()
        {
            var dataList = new List<double>();
            var buffer = new double[FrameSize];
            var currentIndex = 0;
            var isBufferFilledAtleastOnce = false;
            foreach (double dataValue in Data)
            {
                buffer[currentIndex] = dataValue;
                var circularBufferSum = 0.0d;
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
        */

        [Benchmark]
        public double[] MovingAverageQueue()
        {
            var dataArray = new double[Data.Length];
            var frameSizedQueue = new Queue<double>(FrameSize);
            
            for(int i = 0; i < Data.Length; i++)
            {
                if (frameSizedQueue.Count == FrameSize)
                {
                    frameSizedQueue.Dequeue();
                }

                frameSizedQueue.Enqueue(Data[i]);
                dataArray[i] = frameSizedQueue.Average();
            }

            return dataArray;
        }

        [Benchmark]
        public double[] MovingAverageArray()
        {

            var movingAverage = new double[Data.Length];
            if (Data.Length == 0) return movingAverage; //return empty array

            movingAverage[0] = Data[0];

            for (var dataItemIndexCounter = 1; dataItemIndexCounter < Data.Length; dataItemIndexCounter++)
            {
                if (dataItemIndexCounter <= FrameSize - 1)
                {
                    movingAverage[dataItemIndexCounter] = movingAverage[dataItemIndexCounter - 1] * (dataItemIndexCounter / (dataItemIndexCounter + 1.0d)) + (Data[dataItemIndexCounter] / (dataItemIndexCounter + 1.0d));
                }
                else
                {
                    movingAverage[dataItemIndexCounter] = movingAverage[dataItemIndexCounter - 1] + (Data[dataItemIndexCounter] / (FrameSize * 1.0d)) -
                                       Data[dataItemIndexCounter - FrameSize] / FrameSize * 1.0d;
                }
            }

            return movingAverage;
        }

        [Benchmark]
        public double[] MovingAverageDeltaSum()
        {

            var currentFrameSize = 0;
            double cumulativeSum = 0;
            ReadOnlySpan<double> dataSpan = Data.AsSpan();
            var samePool = ArrayPool<double>.Shared;
            var dataList = samePool.Rent(Data.Length);
            var result = new double[Data.Length];
            for (int dataItemIndexCounter = 0; dataItemIndexCounter < dataSpan.Length; dataItemIndexCounter++)
            {
                var indexForDataItemToBeRemoved = dataItemIndexCounter - FrameSize;

                if (indexForDataItemToBeRemoved >= 0)
                {
                    cumulativeSum -= dataSpan[indexForDataItemToBeRemoved];
                    currentFrameSize--;
                }

                if (dataItemIndexCounter < dataSpan.Length)
                {
                    //if (double.IsInfinity(cumulativeSum+dataSpan[dataItemIndexCounter])) throw new Exception("overflow error");
                    cumulativeSum += dataSpan[dataItemIndexCounter];
                    //if (double.IsInfinity(cumulativeSum)) throw new Exception("overflow error");
                    currentFrameSize++;
                }

                dataList[dataItemIndexCounter] = (cumulativeSum / (currentFrameSize * 1.0d));
            }
            
            Array.Copy(dataList, result, Data.Length);
            samePool.Return(dataList);

            return result;
        }
    }
}
