``` ini

BenchmarkDotNet=v0.11.3, OS=macOS Mojave 10.14 (18A391) [Darwin 18.0.0]
Intel Core i7-4870HQ CPU 2.50GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.302
  [Host]     : .NET Core 2.1.2 (CoreCLR 4.6.26628.05, CoreFX 4.6.26629.01), 64bit RyuJIT  [AttachedDebugger]
  DefaultJob : .NET Core 2.1.2 (CoreCLR 4.6.26628.05, CoreFX 4.6.26629.01), 64bit RyuJIT


```
|                 Method | itemsToEnqueue | itemsToKeepInMemory |       Mean |    Error |   StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|----------------------- |--------------- |-------------------- |-----------:|---------:|---------:|------------:|------------:|------------:|--------------------:|
|       DiskQueueEnqueue |         100000 |                1000 | 7,726.4 us | 95.59 us | 89.41 us |   1265.6250 |   1226.5625 |   1210.9375 |            10.06 MB |
| NormalQueueDiskEnqueue |         100000 |                1000 |   711.4 us | 13.11 us | 11.62 us |    285.1563 |    285.1563 |    285.1563 |                1 MB |
