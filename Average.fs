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

        let average (v: float[]) =

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
        member this.simd () =
            SIMD.average v