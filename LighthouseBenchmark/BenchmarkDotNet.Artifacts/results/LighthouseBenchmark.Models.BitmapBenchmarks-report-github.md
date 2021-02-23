``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
Intel Core i5-7200U CPU 2.50GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=5.0.101
  [Host]     : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT
  DefaultJob : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT


```
|                      Method |         Mean |      Error |     StdDev |       Median | Rank | Gen 0 | Gen 1 | Gen 2 | Allocated |
|---------------------------- |-------------:|-----------:|-----------:|-------------:|-----:|------:|------:|------:|----------:|
|           SmallDirectBitmap |     1.802 ms |  0.0566 ms |  0.1539 ms |     1.734 ms |    1 |     - |     - |     - |         - |
|             SmallFastBitmap |     2.641 ms |  0.0275 ms |  0.0257 ms |     2.631 ms |    2 |     - |     - |     - |     112 B |
|            MediumFastBitmap |    30.958 ms |  0.2292 ms |  0.2144 ms |    30.952 ms |    3 |     - |     - |     - |     112 B |
|             LargeFastBitmap |    50.729 ms |  0.5966 ms |  0.5289 ms |    50.628 ms |    4 |     - |     - |     - |    1054 B |
|          MediumDirectBitmap |    56.284 ms |  0.5165 ms |  0.4832 ms |    56.428 ms |    5 |     - |     - |     - |         - |
| Simple2000X2000DirectBitmap |    58.491 ms |  1.0993 ms |  0.9180 ms |    58.239 ms |    6 |     - |     - |     - |      81 B |
|   Simple2000X2000FastBitmap |    63.818 ms |  0.9047 ms |  0.8019 ms |    63.639 ms |    7 |     - |     - |     - |    1367 B |
|           LargeDirectBitmap |    64.509 ms |  0.5045 ms |  0.4473 ms |    64.421 ms |    7 |     - |     - |     - |         - |
|                 SmallBitmap |   301.090 ms |  3.8400 ms |  3.5920 ms |   300.170 ms |    8 |     - |     - |     - |     668 B |
|         MaximumDirectBitmap |   310.869 ms |  6.1410 ms |  8.8072 ms |   308.527 ms |    9 |     - |     - |     - |     152 B |
|           MaximumFastBitmap |   379.236 ms |  5.6993 ms | 10.1306 ms |   378.522 ms |   10 |     - |     - |     - |     112 B |
|                MediumBitmap | 2,730.856 ms | 19.3799 ms | 16.1831 ms | 2,726.563 ms |   11 |     - |     - |     - |    1240 B |
|      Simple2000X2000SBitmap | 2,796.757 ms | 29.7405 ms | 26.3642 ms | 2,784.811 ms |   12 |     - |     - |     - |         - |
|                 LargeBitmap | 4,260.924 ms | 20.0975 ms | 16.7823 ms | 4,263.184 ms |   13 |     - |     - |     - |    5528 B |
