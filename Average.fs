namespace SIMD.Experiments

[<RequireQualifiedAccess>]
module Average =

    open System
    open System.Numerics
    open BenchmarkDotNet.Attributes

    module Classic =

        let naive (v: float[]) =
            v
            |> Array.average

    module SIMD =

        // TODO handle cleanly sizes that are not multiples of 4
        let take1 (v: float[]) =
            let size = v.Length
            let mutable total = 0.0
            for i in 0 .. (size - 1) / 4 do
                let s = 4 * i
                let v = Vector<float>(v.[s .. s + 3])
                total <- total + Vector.Sum(v)
            total / float size

        let take2 (v: float[]) =
            let size = v.Length
            let v = ReadOnlySpan v
            let mutable total = 0.0
            for i in 0 .. (size - 1) / 4 do
                let s = 4 * i
                let v = Vector<float>(v.Slice(s, 4))
                total <- total + Vector.Sum(v)
            total / float size

    type Benchmark () =

        let rng = Random 0
        // Sample vector size
        // Needs to be a multiple of 4 at the moment.
        let size = 1_000_000

        let v = Array.init size (fun _ -> rng.NextDouble())

        [<Benchmark(Baseline=true)>]
        member this.classic () =
            Classic.naive v

        [<Benchmark>]
        member this.simdV1 () =
            SIMD.take1 v

        [<Benchmark>]
        member this.simdV2 () =
            SIMD.take2 v