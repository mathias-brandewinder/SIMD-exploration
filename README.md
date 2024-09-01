# SIMD-exploration

Exploration of dotnet `System.Numerics`.

Running the benchmarks: `dotnet run --configuration Release`
Running the tests: `dotnet test`

## Questions

- Does the size of the array / vector matter
- How does float32 compare to float

## TODO

- [ ] Parameterize Benchmarks (ex: test various sizes)
- [ ] Handle vectors that are not multiples of 4

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

`Average.fs`: computing the average of a vector. Modest improvement.

```
| Method  | Mean       | Error    | StdDev   | Ratio | RatioSD |
|-------- |-----------:|---------:|---------:|------:|--------:|
| classic |   726.2 us |  3.97 us |  3.71 us |  1.00 |    0.01 |
| simdV1  | 3,828.5 us | 62.38 us | 58.35 us |  5.27 |    0.08 |
| simdV2  |   310.0 us |  5.23 us |  4.89 us |  0.43 |    0.01 |
```