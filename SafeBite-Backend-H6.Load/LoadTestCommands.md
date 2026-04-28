Kør api med LoadTest evnironment

køre load test i terminal med kommando:
dotnet run -c Release

eksempel resultat:

scenario: get_allergy_paged_scenario
  - ok count: 750
  - fail count: 0
  - all data: 0 MB
  - duration: 00:00:30

load simulations:
  - inject, rate: 25, interval: 00:00:01, during: 00:00:30

┌─────────────────────────┬─────────────────────────────────────────────────────┐
│                    step │ ok stats                                            │
├─────────────────────────┼─────────────────────────────────────────────────────┤
│                    name │ global information                                  │
│           request count │ all = 750, ok = 750, RPS = 25                       │
│            latency (ms) │ min = 1.53, mean = 2.22, max = 73.34, StdDev = 2.88 │
│ latency percentile (ms) │ p50 = 1.97, p75 = 2.21, p95 = 2.5, p99 = 5.02       │
└─────────────────────────┴─────────────────────────────────────────────────────┘

all = antal request sendt.
ok = antal request som fik en 200 status kode.
rps = requests per seconds

min = hurtigste response tid.
mean = gennemsnitlige response tid.
max = langsomste response tid.
StdDev = standard deviation, hvor meget response tiderne varierer.

p50 = 50% af requestene er hurtigere end denne tid.
p75 = 75% af requestene er hurtigere end denne tid.
p95 = 95% af requestene er hurtigere end denne tid.
p99 = 99% af requestene er hurtigere end denne tid.

man kigger normalt mest på:
failures
p95 
rps


note: 
endpointet blev testet med 25 rps over 30 sekunder.
Alle 750 requests gik igennem med statuscode 200.
Gennemsnitlige responsetid var 2.22ms og p95 var 2.5ms.
Dette indikere en stabil og hurtig performance under belastning.


