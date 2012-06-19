using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("NStatsD Demo")]
[assembly: AssemblyDescription("A .NET 4.0 client for Etsy's StatsD server.")]

#if DEBUG
[assembly: AssemblyProduct("NStatsD Demo (Debug)")]
#else
[assembly: AssemblyProduct("NStatsD Demo (Release)")]
#endif

