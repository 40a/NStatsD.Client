using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;

namespace NStatsD
{


    public class StatBucket
    {
        Stopwatch _timer;
        public string Name { get; private set; }
        private readonly StatsDConfigurationSection _config;

        public StatBucket(StatsDConfigurationSection config, string name)
        {
            Name = name;
            _config = config;
        }

        public StatBucket BeginTimer()
        {
            _timer = Stopwatch.StartNew();
            return this;
        }

        public void EndTimer(double sampleRate = 1)
        {
            _timer.Stop();
            Timing(_timer.ElapsedMilliseconds, sampleRate);
        }

        public void Timing(TimeSpan timeSpan, double sampleRate = 1)
        {
            Timing((long) timeSpan.TotalMilliseconds, sampleRate);
        }

        public void Timing(long time, double sampleRate = 1)
        {
            var data = new Dictionary<string, string> { { Name, string.Format("{0}|ms", time) } };
            Send(data, sampleRate);
        }

        public void Increment(double sampleRate = 1)
        {
            UpdateStats(1, sampleRate);
        }

        public void Decrement(double sampleRate = 1)
        {
            UpdateStats(-1, sampleRate);
        }

        public void Gauge(int value, double sampleRate = 1)
        {
            var data = new Dictionary<string, string> { { Name, string.Format("{0}|g", value) } };
            Send(data, sampleRate);
        }

        public void UpdateStats(int delta = 1, double sampleRate = 1)
        {
            var dictionary = new Dictionary<string, string> { { Name, string.Format("{0}|c", delta) } };
            Send(dictionary, sampleRate);
        }

        private void Send(Dictionary<string, string> data, double sampleRate = 1)
        {
            if (_config == null)
            {
                // silently exit if the user hasn't setup the stats due to unit test environment etc
                return;
            }
            var random = new Random();
            var sampledData = new Dictionary<string, string>();
            if (sampleRate < 1 && random.Next(0, 1) <= sampleRate)
            {
                foreach (var stat in data.Keys)
                {
                    sampledData.Add(stat, string.Format("{0}|@{1}", data[stat], sampleRate));
                }
            }
            else
            {
                sampledData = data;
            }
            var host = _config.Server.Host;
            var port = _config.Server.Port;
            using (var client = new UdpClient(host, port))
            {
                foreach (var stat in sampledData.Keys)
                {
                    var encoding = new System.Text.ASCIIEncoding();
                    var stringToSend = string.Format("{0}:{1}", stat, sampledData[stat]);
                    var sendData = encoding.GetBytes(stringToSend);
                    client.BeginSend(sendData, sendData.Length, Callback, null);
                }
            }
        }

        private static void Callback(IAsyncResult result)
        {
            // dont really want to do anything here since, would rather miss metrics than cause a site/app failure
        }
    }
}