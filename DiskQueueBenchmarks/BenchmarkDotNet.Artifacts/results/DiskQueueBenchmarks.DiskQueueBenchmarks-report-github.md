``` ini

BenchmarkDotNet=v0.11.3, OS=macOS Mojave 10.14 (18A391) [Darwin 18.0.0]
Intel Core i7-4870HQ CPU 2.50GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.302
  [Host]     : .NET Core 2.1.2 (CoreCLR 4.6.26628.05, CoreFX 4.6.26629.01), 64bit RyuJIT  [AttachedDebugger]
  DefaultJob : .NET Core 2.1.2 (CoreCLR 4.6.26628.05, CoreFX 4.6.26629.01), 64bit RyuJIT


```
|                 Method | itemsToEnqueue | itemsToKeepInMemory |       Mean |     Error |    StdDev |
|----------------------- |--------------- |-------------------- |-----------:|----------:|----------:|
|       **DiskQueueEnqueue** |        **1000000** |                **1000** | **172.504 ms** | **3.4345 ms** | **3.6749 ms** |
|   OptimizedDiskEnqueue |        1000000 |                1000 |  90.586 ms | 1.3901 ms | 1.3003 ms |
| NormalQueueDiskEnqueue |        1000000 |                1000 |   9.757 ms | 0.1844 ms | 0.2050 ms |
|       **DiskQueueEnqueue** |        **1000000** |             **1000000** | **132.479 ms** | **2.4881 ms** | **2.4436 ms** |
|   OptimizedDiskEnqueue |        1000000 |             1000000 |  27.657 ms | 0.3636 ms | 0.3223 ms |
| NormalQueueDiskEnqueue |        1000000 |             1000000 |   9.705 ms | 0.1391 ms | 0.1233 ms |
