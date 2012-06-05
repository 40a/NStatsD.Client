# NStatsD.Client

A .NET 4.0 client for [Etsy](http://etsy.com)'s [StatsD](https://github.com/etsy/statsd) server.

This client will let you fire stats at your StatsD server from a .NET application. 
This is a fork of Rob Bihun's client with some adjusted syntax to support dynamic buckets.

## Requirements
.NET 4.0 (Websocket support)

## Installation

Add the Client.cs and the StatsDConfigurationSection.cs files in your project. 
Add the following to your config's configSections node.

```xml
<section name="statsD" type="NStatsD.StatsDConfigurationSection, NStatsD.Client" />
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

            NStatsD.Client.With("test.increment").Increment();
            NStatsD.Client.With("test.decrement").Decrement();
            NStatsD.Client.With("test", "gauge").Gauge(random.Next(0, 100));
            NStatsD.Client.WithoutPrefix("NStatsDDemo.NoPrefix.Gauge").Gauge(89);
```
# License

NStatsD.Client is licensed under the MIT license.
