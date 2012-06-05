using System;
using System.Diagnostics;
using System.Threading;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            NStatsD.Client.GlobalBucketPrefix = NStatsD.Client.GetBucketName(Environment.MachineName, "NStatsDDemo");

            Random random = new Random();

            var timedStat = NStatsD.Client.With("test.timer").BeginTimer();

            NStatsD.Client.With("test.increment").Increment();
            NStatsD.Client.With("test.decrement").Decrement();
            NStatsD.Client.With("test", "gauge").Gauge(random.Next(0, 100));
            NStatsD.Client.WithoutPrefix("NStatsDDemo.NoPrefix.Gauge").Gauge(89);

            timedStat.EndTimer();

            //for (int i = 0; i < 100; i++)
            //{
            //    var ms = random.Next(100, 800);
            //    Console.WriteLine(ms);
            //    NStatsD.Client.With("prefixed", "timing4").Timing(TimeSpan.FromMilliseconds(ms));
            //    Thread.Sleep(10000);
            //}

        }
    }
}
