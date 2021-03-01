``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
Intel Core i5-7200U CPU 2.50GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=5.0.101
  [Host]     : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT
  DefaultJob : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT


```
|                        Method |      Mean |    Error |    StdDev |    Median | Rank | Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------ |----------:|---------:|----------:|----------:|-----:|------:|------:|------:|----------:|
| FilterBitmapInitializedMedium |  21.86 ms | 1.393 ms |  4.109 ms |  22.16 ms |    1 |     - |     - |     - |      96 B |
|  FilterBitmapInitializedLarge |  27.83 ms | 0.497 ms |  1.336 ms |  27.26 ms |    2 |     - |     - |     - |      96 B |
|              FastBitmapMedium |  41.49 ms | 3.197 ms |  9.275 ms |  36.97 ms |    3 |     - |     - |     - |     112 B |
|               FastBitmapLarge |  54.47 ms | 0.939 ms |  0.832 ms |  54.31 ms |    4 |     - |     - |     - |     960 B |
|            FilterBitmapMedium |  63.25 ms | 1.262 ms |  1.119 ms |  63.03 ms |    5 |     - |     - |     - |     248 B |
|             FilterBitmapLarge | 123.97 ms | 8.797 ms | 25.239 ms | 120.17 ms |    6 |     - |     - |     - |     248 B |
