``` ini

BenchmarkDotNet=v0.11.3, OS=macOS Mojave 10.14 (18A391) [Darwin 18.0.0]
Intel Core i7-4870HQ CPU 2.50GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.302
  [Host]     : .NET Core 2.1.2 (CoreCLR 4.6.26628.05, CoreFX 4.6.26629.01), 64bit RyuJIT  [AttachedDebugger]
  DefaultJob : .NET Core 2.1.2 (CoreCLR 4.6.26628.05, CoreFX 4.6.26629.01), 64bit RyuJIT


```
|                 Method | itemsToEnqueue | itemsToKeepInMemory |       Mean |     Error |    StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|----------------------- |--------------- |-------------------- |-----------:|----------:|----------:|------------:|------------:|------------:|--------------------:|
|       DiskQueueEnqueue |          10000 |                  10 | 1,440.5 ms | 100.95 ms | 294.47 ms |  18000.0000 |   1000.0000 |           - |           111.68 MB |
| NormalQueueDiskEnqueue |          10000 |                  10 |   588.1 ms |  11.08 ms |  20.53 ms |  14000.0000 |   6000.0000 |   2000.0000 |            72.97 MB |
