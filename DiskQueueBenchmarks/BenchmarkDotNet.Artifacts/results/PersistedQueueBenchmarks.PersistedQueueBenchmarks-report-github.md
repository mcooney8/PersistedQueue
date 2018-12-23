``` ini

BenchmarkDotNet=v0.11.3, OS=macOS Mojave 10.14 (18A391) [Darwin 18.0.0]
Intel Core i7-4870HQ CPU 2.50GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.302
  [Host] : .NET Core 2.1.2 (CoreCLR 4.6.26628.05, CoreFX 4.6.26629.01), 64bit RyuJIT  [AttachedDebugger]


```
|                               Method | totalItems | itemsToKeepInMemory | useLargeData | Mean | Error |
|------------------------------------- |----------- |-------------------- |------------- |-----:|------:|
| PersistentQueueSqliteFilePersistence |       1000 |                 100 |         True |   NA |    NA |
|   PersistentQueueInMemoryPersistence |       1000 |                 100 |         True |   NA |    NA |
|                          NormalQueue |       1000 |                 100 |         True |   NA |    NA |

Benchmarks with issues:
  PersistedQueueBenchmarks.PersistentQueueSqliteFilePersistence: DefaultJob [totalItems=1000, itemsToKeepInMemory=100, useLargeData=True]
  PersistedQueueBenchmarks.PersistentQueueInMemoryPersistence: DefaultJob [totalItems=1000, itemsToKeepInMemory=100, useLargeData=True]
  PersistedQueueBenchmarks.NormalQueue: DefaultJob [totalItems=1000, itemsToKeepInMemory=100, useLargeData=True]
