``` ini

BenchmarkDotNet=v0.11.3, OS=macOS Mojave 10.14 (18A391) [Darwin 18.0.0]
Intel Core i7-4870HQ CPU 2.50GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.302
  [Host]     : .NET Core 2.1.2 (CoreCLR 4.6.26628.05, CoreFX 4.6.26629.01), 64bit RyuJIT  [AttachedDebugger]
  DefaultJob : .NET Core 2.1.2 (CoreCLR 4.6.26628.05, CoreFX 4.6.26629.01), 64bit RyuJIT


```
|                   Method | itemsToEnqueue | itemsToKeepInMemory | enqueueOnly |       Mean |       Error |        StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|------------------------- |--------------- |-------------------- |------------ |-----------:|------------:|--------------:|------------:|------------:|------------:|--------------------:|
| **FlatFileDiskQueueEnqueue** |          **10000** |                  **10** |       **False** | **6,960.3 ms** | **487.6374 ms** | **1,399.1231 ms** | **605000.0000** |  **98000.0000** |           **-** |          **3635.12 MB** |
| InMemoryDiskQueueEnqueue |          10000 |                  10 |       False |   536.1 ms |  10.1673 ms |     9.5105 ms |  13000.0000 |   5000.0000 |   1000.0000 |            73.61 MB |
|       NormalQueueEnqueue |          10000 |                  10 |       False |   522.1 ms |   1.2931 ms |     1.2096 ms |  13000.0000 |   5000.0000 |   1000.0000 |            72.96 MB |
| **FlatFileDiskQueueEnqueue** |          **10000** |                  **10** |        **True** |   **979.9 ms** |  **18.6573 ms** |    **18.3240 ms** |  **18000.0000** |           **-** |           **-** |           **111.56 MB** |
| InMemoryDiskQueueEnqueue |          10000 |                  10 |        True |   525.2 ms |   0.8769 ms |     0.8202 ms |  13000.0000 |   5000.0000 |   1000.0000 |            73.61 MB |
|       NormalQueueEnqueue |          10000 |                  10 |        True |   521.7 ms |   1.2982 ms |     1.0841 ms |  13000.0000 |   5000.0000 |   1000.0000 |            72.96 MB |
