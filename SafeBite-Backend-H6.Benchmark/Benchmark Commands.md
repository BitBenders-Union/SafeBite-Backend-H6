Du skal befinde dig i benchmark-projektets root folder
..\SafeBite-Backend-H6\SafeBite-Backend-H6.Benchmark\

benchmark skal altid køres som release da dette er optimeret for performance.

dotnet build -c Release
dotnet run -c Release



Eksempel på output:

// * Detailed results *
AllergyServiceBenchmark.GetAllergyByName: DefaultJob
Runtime = .NET 10.0.6 (10.0.6, 10.0.626.17701), X64 RyuJIT x86-64-v3; GC = Concurrent Workstation
Mean = 514.970 us, StdErr = 2.697 us (0.52%), N = 24, StdDev = 13.214 us
Min = 492.763 us, Q1 = 506.796 us, Median = 515.470 us, Q3 = 524.717 us, Max = 534.411 us
IQR = 17.921 us, LowerFence = 479.914 us, UpperFence = 551.598 us
ConfidenceInterval = [504.807 us; 525.132 us] (CI 99.9%), Margin = 10.163 us (1.97% of Mean)
Skewness = -0.17, Kurtosis = 1.78, MValue = 2
-------------------- Histogram --------------------
[490.145 us ; 504.476 us) | @@@@@
[504.476 us ; 518.372 us) | @@@@@@@@
[518.372 us ; 540.424 us) | @@@@@@@@@@@
---------------------------------------------------

// * Summary *

BenchmarkDotNet v0.15.8, Windows 11 (10.0.22631.6199/23H2/2023Update/SunValley3)
AMD Ryzen 9 5900X 3.70GHz, 1 CPU, 24 logical and 12 physical cores
.NET SDK 10.0.202
  [Host]     : .NET 10.0.6 (10.0.6, 10.0.626.17701), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.6 (10.0.6, 10.0.626.17701), X64 RyuJIT x86-64-v3


| Method           | Mean     | Error    | StdDev   |
|----------------- |---------:|---------:|---------:|
| GetAllergyByName | 515.0 us | 10.16 us | 13.21 us |

// * Legends *
  Mean   : Arithmetic mean of all measurements
  Error  : Half of 99.9% confidence interval
  StdDev : Standard deviation of all measurements
  1 us   : 1 Microsecond (0.000001 sec)

// ***** BenchmarkRunner: End *****
Run time: 00:00:24 (24.29 sec), executed benchmarks: 1

Global total time: 00:00:35 (35.39 sec), executed benchmarks: 1
// * Artifacts cleanup *
Artifacts cleanup is finished


forklaring:
| Method           | Mean     | Error    | StdDev   |
|----------------- |---------:|---------:|---------:|
| GetAllergyByName | 515.0 us | 10.16 us | 13.21 us |

mean tager 515 mikrosekunder
error er en confidence interval på 10.16 mikrosekunder
srddev er variation mellem målinger. 13.21 mikrosekunder

1 ms = 1000 us
us = mikrosekunder

