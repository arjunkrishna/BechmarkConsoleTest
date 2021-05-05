using System;
using BenchmarkConsole;
using Xunit;

namespace TestProject
{
    public class UnitTest1
    {
        [Fact]
        public void MVACheck()
        {
            var app = new MovingAverageImplementations(10,3);
            var data = app.Data;
            
            //var result6 = app.MovingAverageLinq();
            //var result7 = app.MovingAverageParallelLinq();
            //var result8 = app.MovingAverageNestedLoop();
            //var result9 = app.MovingAverageBufferAndFrameLengthSame();
            var result1 = app.MovingAverageQueue();
            var result2 = app.MovingAverageArray();
            var result5 = app.MovingAverageDeltaSum();

            var x = "";


        }
    }
}
