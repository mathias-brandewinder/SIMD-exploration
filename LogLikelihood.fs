namespace SIMD.Experiments

[<RequireQualifiedAccess>]
module LogLikelihood =

    open System
    open System.Runtime.InteropServices
    open System.Numerics
    open BenchmarkDotNet.Attributes

    module Classic =

        let naive (v: float[]) =
            v
            |> Array.sumBy (fun x -> log x)
            |> fun total -> - total

    module SIMD =

        let take1 (v: float[]) =

            let s = MemoryMarshal.Cast<float, Vector<float>>(ReadOnlySpan(v))

            let mutable total = 0.0
            for i in 0 .. (s.Length - 1) do
                let v = s.[i]
                total <- total - Vector.Sum(Vector.Log(v))
            total

    type Benchmark () =

        let rng = Random 0
        // Sample vector size
        // Needs to be a multiple of the vector size at the moment.
        let vSize = Vector<float>.Count
        let size = 100 * vSize

        let v = Array.init size (fun _ -> rng.NextDouble())

        [<Benchmark(Baseline=true)>]
        member this.classic () =
            Classic.naive (v)

        [<Benchmark>]
        member this.simdV1 () =
            SIMD.take1 (v)
