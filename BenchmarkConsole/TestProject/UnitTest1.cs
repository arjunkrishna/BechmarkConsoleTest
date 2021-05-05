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
            var result1 = app.MovingAverageQueue();
            //var result2 = app.MovingAverageArray();
            var result3 = app.GetMovingAverageDeltaSum();
            var result4 = app.MovingAverageDeltaSumManipulationSpan();
            var result5 = app.MovingAverageDeltaSumSpanArrayPool();

            var x = "";


        }
    }
}
