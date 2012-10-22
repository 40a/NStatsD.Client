using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NStatsD
{
    public class StatBucket
    {
        #region Fields

        Stopwatch _timer;
        public string Name { get; private set; }
        private readonly StatsDConfigurationSection _config;

        /// <summary>
        /// Support rerouting the messages to Nlog/log4net etc http://nlog-project.org/2010/09/02/routing-system-diagnostics-trace-and-system-diagnostics-tracesource-logs-through-nlog.html
        /// </summary>
        static readonly TraceSource TraceSource = new TraceSource("NStatsD");
        #endregion

        #region Ctor
        public StatBucket(StatsDConfigurationSection config, string name)
        {
            Name = name;
            _config = config;
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Return this instance to support fluent style syntax
        /// </summary>
        public StatBucket BeginTimer()
        {
            _timer = Stopwatch.StartNew();
            return this;
        }

        public TimeSpan EndTimer(double sampleRate = 1)
        {
            _timer.Stop();
            Timing(_timer.ElapsedMilliseconds, sampleRate);
            return _timer.Elapsed;
        }

        public void Timing(TimeSpan timeSpan, double sampleRate = 1)
        {
            Timing((long) timeSpan.TotalMilliseconds, sampleRate);
        }

        public void Timing(long time, double sampleRate = 1)
        {
            var data = new Dictionary<string, string> { { Name, string.Format("{0}|ms", time) } };
            SendData(data, sampleRate);
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
            SendData(data, sampleRate);
        }

        public void UpdateStats(int delta = 1, double sampleRate = 1)
        {
            var dictionary = new Dictionary<string, string> { { Name, string.Format("{0}|c", delta) } };
            SendData(dictionary, sampleRate);
        }

        private void SendData(Dictionary<string, string> data, double sampleRate = 1)
        {
            if (_config == null)
            {
                // silently exit if the user hasn't setup the stats due to unit test environment etc
                return;
            }
            
            var sampledData = new Dictionary<string, string>();
            if (sampleRate < 1)
            {
                var random = new Random();
                if (random.Next(0, 1) <= sampleRate)
                {
                    foreach (var stat in data.Keys)
                    {
                        sampledData.Add(stat, string.Format("{0}|@{1}", data[stat], sampleRate));
                    }       
                }
            }
            
            if (sampledData.Keys.Count == 0)
            {
                sampledData = data;
            }
            
            SendUdp(sampledData);

            LogVerbose("NStatsD.Send returning");
        }

        private void SendUdp(Dictionary<string, string> sampledData)
        {
            var host = _config.Server.Host;
            var port = _config.Server.Port;

            UdpClient client;
            
            try
            {
                client = new UdpClient(host, port);
            }
            catch(SocketException e)
            {
                /*
                 * This happened when the host was a fully qualified domain name that pointed back to the same local sever
                 * and the Internet connection went down. Wrap some clearer messaging around it.    
                 
                   System.Net.Sockets.SocketException (0x80004005): The requested name is valid, but no data of the requested type was found
                   at System.Net.Dns.GetAddrInfo(String name)
                   at System.Net.Dns.InternalGetHostByName(String hostName, Boolean includeIPv6)
                   at System.Net.Dns.GetHostAddresses(String hostNameOrAddress)
                   at System.Net.Sockets.UdpClient.Connect(String hostname, Int32 port)
                   at System.Net.Sockets.UdpClient..ctor(String hostname, Int32 port)
                 */
                string message = string.Format("NStatsD failed to transmit to '{0}:{1}' ({2})", host, port, e.Message);
                LogWarn(message);

                // Typically wouldn't want the code to throw an exception up the chain just because of a failing stat
                #if DEBUG
                    throw new ApplicationException("Thowing Exception due to DEBUG compile: " + message, e);
                #endif

                // Abort sending the data since the connection failed.
                return;
            }

            foreach (var stat in sampledData.Keys)
            {
                var encoding = new ASCIIEncoding();
                var stringToSend = string.Format("{0}:{1}", stat, sampledData[stat]);
                var sendData = encoding.GetBytes(stringToSend);
                LogVerbose("NStatsD sending {0}", stringToSend);
                client.BeginSend(sendData, sendData.Length, UdpClientCallback, client);
            }
        }

        #endregion

        #region Static Methods

        private static void LogWarn(string messageFormat, params object[] formatArgs)
        {
            Log(TraceEventType.Warning, messageFormat, formatArgs);
        }

        private static void LogVerbose(string messageFormat, params object[] formatArgs)
        {
            Log(TraceEventType.Verbose, messageFormat, formatArgs);
        }

        private static void Log(TraceEventType traceEventType, string messageFormat, params object[] formatArgs)
        {
#if DEBUG
            Trace.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff\t") + string.Format(messageFormat, formatArgs));
#endif
            TraceSource.TraceEvent(traceEventType, 20121023, messageFormat, formatArgs);
        }

        private static void UdpClientCallback(IAsyncResult result)
        {
            UdpClient udpClient = (UdpClient) result.AsyncState;
            LogVerbose("NStatsD sent {0} bytes", udpClient.EndSend(result));
        }

        #endregion

    }
}