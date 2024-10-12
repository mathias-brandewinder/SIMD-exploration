namespace SIMD.Experiments

[<RequireQualifiedAccess>]
module Distance =

    open System
    open System.Runtime.InteropServices
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
            let vSize = Vector<float>.Count

            let mutable total = 0.0
            for i in 0 .. (size - 1) / vSize do
                let s = vSize * i
                let v1 = Vector<float>(v1.[s .. s + vSize - 1])
                let v2 = Vector<float>(v2.[s .. s + vSize - 1])
                let diff = v1 - v2
                total <- total + Vector.Dot(diff, diff)

            sqrt total

        let take2 (v1: float[], v2: float[]) =

            let size = v1.Length
            let vSize = Vector<float>.Count

            let s1 = ReadOnlySpan(v1)
            let s2 = ReadOnlySpan(v2)

            let mutable total = 0.0
            for i in 0 .. (size - 1) / vSize do
                let s = vSize * i
                let v1 = Vector<float>(s1.Slice(s, vSize))
                let v2 = Vector<float>(s2.Slice(s, vSize))
                let diff = v1 - v2
                total <- total + Vector.Dot(diff, diff)

            sqrt total

        // Feedback from Alexandre Mutel:
        // https://mastodon.social/@xoofx/113066600743080384
        // https://gist.github.com/xoofx/2fc4e25ed32732bcfe0559e8c07076bb
        let take3 (v1: float[], v2: float[]) =

            let s1 = MemoryMarshal.Cast<float, Vector<float>>(ReadOnlySpan(v1))
            let s2 = MemoryMarshal.Cast<float, Vector<float>>(ReadOnlySpan(v2))

            let mutable total = 0.0
            for i in 0 .. (s1.Length - 1) do
                let v1 = s1.[i]
                let v2 = s2.[i]
                let diff = v1 - v2
                total <- total + Vector.Dot(diff, diff)
            sqrt total

        // Same as take3, but handle the case of arrays that are not clean
        // multiples of Vector<float>.Count.
        let full (v1: float[], v2: float[]) =

            let remainingBlocks = v1.Length % Vector<float>.Count
            let fullBlocks =
                (v1.Length - remainingBlocks) / Vector<float>.Count

            let s1 = MemoryMarshal.Cast<float, Vector<float>>(ReadOnlySpan(v1))
            let s2 = MemoryMarshal.Cast<float, Vector<float>>(ReadOnlySpan(v2))

            let mutable total = 0.0
            for i in 0 .. (fullBlocks - 1) do
                let v1 = s1.[i]
                let v2 = s2.[i]
                let diff = v1 - v2
                total <- total + Vector.Dot(diff, diff)

            if remainingBlocks > 0
            then
                for i in (v1.Length - remainingBlocks) .. (v1.Length - 1) do
                    total <- total + (pown (v1.[i] - v2.[i]) 2)

            sqrt total

    type Benchmark () =

        let rng = Random 0
        // Sample vector size
        // Needs to be a multiple of the vector size at the moment.
        let vSize = Vector<float>.Count
        let size = 2 * vSize

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

        [<Benchmark>]
        member this.simdV3 () =
            SIMD.take3 (v1, v2)

        [<Benchmark>]
        member this.full () =
            SIMD.full (v1, v2)