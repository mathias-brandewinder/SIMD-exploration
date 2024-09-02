namespace SIMD.Experiments

[<RequireQualifiedAccess>]
module Average =

    open System
    open System.Runtime.InteropServices
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
            let vSize = Vector<float>.Count
            let mutable total = 0.0
            for i in 0 .. (size - 1) / vSize do
                let s = vSize * i
                let v = Vector<float>(v.[s .. s + vSize - 1])
                total <- total + Vector.Sum(v)
            total / float size

        let take2 (v: float[]) =
            let size = v.Length
            let vSize = Vector<float>.Count
            let v = ReadOnlySpan v
            let mutable total = 0.0
            for i in 0 .. (size - 1) / vSize do
                let s = vSize * i
                let v = Vector<float>(v.Slice(s, vSize))
                total <- total + Vector.Sum(v)
            total / float size

        let take3 (v: float[]) =

            let vectors = MemoryMarshal.Cast<float, Vector<float>>(ReadOnlySpan v)
            let mutable total = 0.0
            for i in 0 .. (vectors.Length - 1) do
                let v = vectors.[i]
                total <- total + Vector.Sum(v)
            total / float v.Length

    type Benchmark () =

        let rng = Random 0
        // Sample vector size
        // Needs to be a multiple of 4 at the moment.
        let vSize = Vector<float>.Count
        let size = vSize * 10_000 // 1_000_000

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

        [<Benchmark>]
        member this.simdV3 () =
            SIMD.take3 v