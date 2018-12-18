``` ini

BenchmarkDotNet=v0.11.3, OS=macOS Mojave 10.14 (18A391) [Darwin 18.0.0]
Intel Core i7-4870HQ CPU 2.50GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.302
  [Host]     : .NET Core 2.1.2 (CoreCLR 4.6.26628.05, CoreFX 4.6.26629.01), 64bit RyuJIT  [AttachedDebugger]
  DefaultJob : .NET Core 2.1.2 (CoreCLR 4.6.26628.05, CoreFX 4.6.26629.01), 64bit RyuJIT


```
|                   Method | itemsToEnqueue | itemsToKeepInMemory | enqueueOnly |       Mean |      Error |       StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|------------------------- |--------------- |-------------------- |------------ |-----------:|-----------:|-------------:|------------:|------------:|------------:|--------------------:|
| **FlatFileDiskQueueEnqueue** |          **10000** |                  **10** |       **False** | **5,598.7 ms** | **353.723 ms** | **1,037.408 ms** | **605000.0000** |  **98000.0000** |           **-** |          **3635.12 MB** |
| InMemoryDiskQueueEnqueue |          10000 |                  10 |       False |   430.0 ms |   4.563 ms |     4.268 ms |  13000.0000 |   5000.0000 |   1000.0000 |            73.61 MB |
|       NormalQueueEnqueue |          10000 |                  10 |       False |   430.2 ms |   2.377 ms |     2.107 ms |  13000.0000 |   5000.0000 |   1000.0000 |            72.96 MB |
| **FlatFileDiskQueueEnqueue** |          **10000** |                  **10** |        **True** |   **773.6 ms** |   **8.690 ms** |     **7.703 ms** |  **18000.0000** |           **-** |           **-** |           **111.57 MB** |
| InMemoryDiskQueueEnqueue |          10000 |                  10 |        True |   429.5 ms |   1.498 ms |     1.401 ms |  13000.0000 |   5000.0000 |   1000.0000 |            73.61 MB |
|       NormalQueueEnqueue |          10000 |                  10 |        True |   427.9 ms |   3.462 ms |     2.891 ms |  13000.0000 |   5000.0000 |   1000.0000 |            72.96 MB |
