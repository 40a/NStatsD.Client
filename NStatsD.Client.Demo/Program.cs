using System;
using System.Diagnostics;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Optional bucket prefix
            NStatsD.Client.GlobalBucketPrefix = NStatsD.Client.GetBucketName(Environment.MachineName, "NStatsDDemo");

            var timer = Stopwatch.StartNew();


            NStatsD.Client.With("test.increment").Increment();
            NStatsD.Client.With("test.decrement").Decrement();
            NStatsD.Client.With("test.increment").Timing(timer.ElapsedMilliseconds);
            NStatsD.Client.With("test.gauge").Gauge(25);
            NStatsD.Client.With("test","gauge").Gauge(25);

            var timedStat = NStatsD.Client.With("Timer").BeginTimer();

            NStatsD.Client.WithoutPrefix("prefixed.Timer").Timing(TimeSpan.FromMilliseconds(500));
            NStatsD.Client.WithoutPrefix("prefixed", "gauge").Gauge(89);

            timedStat.EndTimer();

            Console.Read();
        }
    }
}
