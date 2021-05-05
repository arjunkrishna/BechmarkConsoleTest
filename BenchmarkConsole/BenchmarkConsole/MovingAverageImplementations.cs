using System;
using System.Buffers;
using System.Collections.Generic;
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

        //private class Config : ManualConfig
        //{
        //    public Config()
        //    {
        //        AddExporter(CsvMeasurementsExporter.Default);
        //        AddExporter(RPlotExporter.Default);
        //    }
        //}

        public float[] Data { get; private set; }

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
            Data = RandomNumberGenerator.GenerateRandomFloatArray(CountOfNumber);
        }

        [Benchmark]
        public float[] MovingAverageQueue()
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
        public float[] MovingAverageArray()
        {

            var movingAverage = new float[Data.Length];
            if (Data.Length == 0) return movingAverage; //return empty array

            movingAverage[0] = Data[0];

            for (var dataItemIndexCounter = 1; dataItemIndexCounter < Data.Length; dataItemIndexCounter++)
            {
                if (dataItemIndexCounter <= FrameSize - 1)
                {
                    movingAverage[dataItemIndexCounter] = movingAverage[dataItemIndexCounter - 1] * (dataItemIndexCounter / (dataItemIndexCounter + 1f)) + (Data[dataItemIndexCounter] / (dataItemIndexCounter + 1f));
                }
                else
                {
                    movingAverage[dataItemIndexCounter] = movingAverage[dataItemIndexCounter - 1] + (Data[dataItemIndexCounter] / (FrameSize * 1.0f)) -
                                       Data[dataItemIndexCounter - FrameSize] / FrameSize * 1.0f;
                }
            }

            return movingAverage;
        }

        [Benchmark]
        public float[] GetMovingAverageDeltaSum()
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
        public float[] MovingAverageDeltaSumManipulationSpan()
        {

            var dataList = new List<float>();
            var currentFrameSize = 0;
            float cumulativeSum = 0;
            ReadOnlySpan<float> dataSpan = Data.AsSpan();
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
                    cumulativeSum += dataSpan[dataItemIndexCounter];
                    currentFrameSize++;
                }

                //Console.WriteLine(cumulativeSum + " : " + currentFrameSize);
                dataList.Add(cumulativeSum / (currentFrameSize * 1.0f));
            }

            return dataList.ToArray();
        }

        [Benchmark]
        public float[] MovingAverageDeltaSumSpanArrayPool()
        {

            var currentFrameSize = 0;
            float cumulativeSum = 0;
            ReadOnlySpan<float> dataSpan = Data.AsSpan();
            var samePool = ArrayPool<float>.Shared;
            var dataList = samePool.Rent(Data.Length);
            var result = new float[Data.Length];
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
                    cumulativeSum += dataSpan[dataItemIndexCounter];
                    currentFrameSize++;
                }

                //Console.WriteLine(cumulativeSum + " : " + currentFrameSize);
                dataList[dataItemIndexCounter] = (cumulativeSum / (currentFrameSize * 1.0f));
            }
            
            Array.Copy(dataList, result, Data.Length);
            samePool.Return(dataList);

            return result;
        }
    }
}
