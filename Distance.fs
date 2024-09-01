namespace SIMD.Experiments

[<RequireQualifiedAccess>]
module Distance =

    open System
    open System.Numerics
    open BenchmarkDotNet.Attributes

    module Classic =

        let naive (v1: float[], v2: float[]) =
            (v1, v2)
            ||> Array.map2 (fun x y -> (x - y) ** 2)
            |> Array.sum
            |> sqrt

        let optimized (v1: float[], v2: float[]) =
            let size = v1.Length
            let mutable total = 0.0
            for i in 0 .. (size - 1) do
                total <- total + ((v1.[i] - v2.[i]) ** 2)
            sqrt total

    module SIMD =

        // TODO handle cleanly sizes that are not multiples of 4
        let take1 (v1: float[], v2: float[]) =
            let size = v1.Length
            let mutable total = 0.0
            for i in 0 .. (size - 1) / 4 do
                let s = 4 * i
                let v1 = Vector<float>(v1.[s .. s + 3])
                let v2 = Vector<float>(v2.[s .. s + 3])
                let diff = v1 - v2
                total <- total + Vector.Dot(diff, diff)
            sqrt total

        let take2 (v1: float[], v2: float[]) =
            let size = v1.Length
            let s1 = ReadOnlySpan(v1)
            let s2 = ReadOnlySpan(v2)
            let mutable total = 0.0
            for i in 0 .. (size - 1) / 4 do
                let s = 4 * i
                let v1 = Vector<float>(s1.Slice(s, 4))
                let v2 = Vector<float>(s2.Slice(s, 4))
                let diff = v1 - v2
                total <- total + Vector.Dot(diff, diff)
            sqrt total

    type Benchmark () =

        let rng = Random 0
        // Sample vector size
        // Needs to be a multiple of 4 at the moment.
        let size = 8 //40_000

        let v1 = Array.init size (fun _ -> rng.NextDouble())
        let v2 = Array.init size (fun _ -> rng.NextDouble())

        [<Benchmark(Baseline=true)>]
        member this.classic () =
            Classic.naive (v1, v2)

        [<Benchmark>]
        member this.classicOptimized () =
            Classic.optimized (v1, v2)

        [<Benchmark>]
        member this.simdV1 () =
            SIMD.take1 (v1, v2)

        [<Benchmark>]
        member this.simdV2 () =
            SIMD.take2 (v1, v2)
