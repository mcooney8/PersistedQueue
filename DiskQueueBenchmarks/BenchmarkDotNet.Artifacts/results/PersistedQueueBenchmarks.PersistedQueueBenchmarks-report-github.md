``` ini

BenchmarkDotNet=v0.11.3, OS=macOS Mojave 10.14 (18A391) [Darwin 18.0.0]
Intel Core i7-4870HQ CPU 2.50GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.302
  [Host]     : .NET Core 2.1.2 (CoreCLR 4.6.26628.05, CoreFX 4.6.26629.01), 64bit RyuJIT  [AttachedDebugger]
  DefaultJob : .NET Core 2.1.2 (CoreCLR 4.6.26628.05, CoreFX 4.6.26629.01), 64bit RyuJIT


```
|                   Method | itemsToEnqueue | itemsToKeepInMemory |       Mean |     Error |     StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|------------------------- |--------------- |-------------------- |-----------:|----------:|-----------:|------------:|------------:|------------:|--------------------:|
| **FlatFileDiskQueueEnqueue** |          **10000** |                  **10** |   **445.8 ms** |  **7.099 ms** |   **6.640 ms** |  **13000.0000** |   **5000.0000** |   **1000.0000** |            **73.61 MB** |
| InMemoryDiskQueueEnqueue |          10000 |                  10 |   439.7 ms |  5.973 ms |   5.587 ms |  13000.0000 |   5000.0000 |   1000.0000 |            73.61 MB |
|       NormalQueueEnqueue |          10000 |                  10 |   457.7 ms | 16.871 ms |  29.101 ms |  13000.0000 |   5000.0000 |   1000.0000 |            72.96 MB |
| **FlatFileDiskQueueEnqueue** |          **10000** |               **10000** |   **439.2 ms** |  **5.277 ms** |   **4.937 ms** |  **13000.0000** |   **5000.0000** |   **1000.0000** |            **73.69 MB** |
| InMemoryDiskQueueEnqueue |          10000 |               10000 |   438.3 ms |  5.690 ms |   5.322 ms |  13000.0000 |   5000.0000 |   1000.0000 |            73.69 MB |
|       NormalQueueEnqueue |          10000 |               10000 |   445.1 ms |  8.635 ms |  10.279 ms |  13000.0000 |   5000.0000 |   1000.0000 |            72.96 MB |
| **FlatFileDiskQueueEnqueue** |         **100000** |                  **10** | **4,546.0 ms** | **46.782 ms** |  **39.065 ms** | **126000.0000** |  **45000.0000** |   **5000.0000** |           **735.16 MB** |
| InMemoryDiskQueueEnqueue |         100000 |                  10 | 4,612.6 ms | 92.054 ms |  90.410 ms | 126000.0000 |  45000.0000 |   5000.0000 |           735.15 MB |
|       NormalQueueEnqueue |         100000 |                  10 | 4,638.9 ms | 91.327 ms | 121.919 ms | 126000.0000 |  45000.0000 |   5000.0000 |            729.1 MB |
| **FlatFileDiskQueueEnqueue** |         **100000** |               **10000** | **4,595.6 ms** | **90.492 ms** |  **70.650 ms** | **126000.0000** |  **45000.0000** |   **5000.0000** |           **735.23 MB** |
| InMemoryDiskQueueEnqueue |         100000 |               10000 | 4,629.2 ms | 91.961 ms | 116.301 ms | 126000.0000 |  45000.0000 |   5000.0000 |           735.23 MB |
|       NormalQueueEnqueue |         100000 |               10000 | 4,583.6 ms | 78.849 ms |  61.560 ms | 126000.0000 |  45000.0000 |   5000.0000 |           729.09 MB |
