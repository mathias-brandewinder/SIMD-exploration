# SIMD-exploration

Exploration of dotnet `System.Numerics`.

Running the benchmarks: `dotnet run --configuration Release`

## 2024-08-31

`Distance.fs`: computing the euclidean distance between 2 vectors. Using dot 
product + `ReadOnlySpan` in the SIMD take 2 produced a massive speedup:  

```
| Method           | Mean        | Error     | StdDev    | Ratio |
|----------------- |------------:|----------:|----------:|------:|
| classic          | 30,549.7 us | 101.17 us |  89.69 us |  1.00 |
| classicOptimized | 28,676.7 us | 380.77 us | 337.55 us |  0.94 |
| simdV1           |  8,186.7 us | 131.10 us | 116.22 us |  0.27 |
| simdV2           |    741.5 us |  21.53 us |  63.49 us |  0.02 |
```
