using System;
using System.Diagnostics;
using System.Threading;
using NStatsD;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            NStatsDClient.GlobalBucketPrefix = NStatsDClient.GetBucketName(Environment.MachineName, "NStatsDDemo");

            Random random = new Random();

            var timedStat = NStatsDClient.With("test.timer").BeginTimer();

            NStatsDClient.With("test.increment").Increment();
            NStatsDClient.With("test.decrement").Decrement();
            NStatsDClient.With("test", "gauge").Gauge(random.Next(0, 100));
            NStatsDClient.WithoutPrefix("NStatsDDemo.NoPrefix.Gauge").Gauge(89);

            var timeSpan = timedStat.EndTimer();

            //for (int i = 0; i < 100; i++)
            //{
            //    var ms = random.Next(100, 800);
            //    Console.WriteLine(ms);
            //    NStatsDClient.With("prefixed", "timing4").Timing(TimeSpan.FromMilliseconds(ms));
            //    Thread.Sleep(10000);
            //}

        }
    }
}
