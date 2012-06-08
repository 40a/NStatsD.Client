# NStatsDClient

A .NET 4.0 client for [Etsy](http://etsy.com)'s [StatsD](https://github.com/etsy/statsd) server.

This client will let you fire stats at your StatsD server from a .NET application. 
This is a fork of Rob Bihun's client with some adjusted syntax to support dynamic buckets.

## Requirements
.NET 4.0

## Installation

Reference the NStatsD.dll assembly in your project. 
Add the following to your config's configSections node.

```xml
<section name="statsD" type="NStatsD.StatsDConfigurationSection, NStatsD" />
```

Then add the following to your app config's configuration node.

```xml
<statsD>
	<server host="localhost" port="8125" />
</statsD>
```
## Usage
```csharp

            var timedStat = NStatsD.Client.With("test.timer").BeginTimer();

            NStatsDClient.With("test.increment").Increment();
            NStatsDClient.With("test.decrement").Decrement();
            NStatsDClient.With("test", "gauge").Gauge(random.Next(0, 100));
            NStatsDClient.WithoutPrefix("NStatsDDemo.NoPrefix.Gauge").Gauge(89);
```
# License

NStatsDClient is licensed under the MIT license.
