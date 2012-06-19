using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("NStatsD")]
[assembly: AssemblyDescription("A .NET 4.0 client for Etsy's StatsD server.")]

#if DEBUG
[assembly: AssemblyProduct("NStatsD (Debug)")]
#else
[assembly: AssemblyProduct("NStatsD (Release)")]
#endif

