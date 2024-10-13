namespace SIMD.Experiments

[<RequireQualifiedAccess>]
module MoveTowards =

    open System
    open System.Runtime.InteropServices
    open System.Numerics
    open BenchmarkDotNet.Attributes

    module Classic =

        let naive (origin: float[], target: float[], coeff: float) =
            let dim = origin.Length
            Array.init dim (fun col ->
                origin.[col] + coeff * (target.[col] - origin.[col])
                )

    module SIMD =

        let take1 (origin: float[], target: float[], coeff: float) =

            let origins = MemoryMarshal.Cast<float, Vector<float>>(ReadOnlySpan(origin))
            let targets = MemoryMarshal.Cast<float, Vector<float>>(ReadOnlySpan(target))

            let result = Array.zeroCreate<float> origin.Length
            let blockSize = Vector<float>.Count

            let multiplier = 1.0 - coeff
            for i in 0 .. (origins.Length - 1) do
                let o = origins.[i]
                let t = targets.[i]
                let movedTo = Vector.Multiply(multiplier, o) -  t
                movedTo.CopyTo(result, i * blockSize)
            result

        let take2 (origin: float[], target: float[], coeff: float) =

            let origins = MemoryMarshal.Cast<float, Vector<float>>(ReadOnlySpan(origin))
            let targets = MemoryMarshal.Cast<float, Vector<float>>(ReadOnlySpan(target))

            let result = Array.zeroCreate<float> origin.Length
            let blockSize = Vector<float>.Count
            let buffer = Array.zeroCreate<float> blockSize

            let multiplier = 1.0 - coeff
            for i in 0 .. (origins.Length - 1) do
                let o = origins.[i]
                let t = targets.[i]
                let movedTo = Vector.Multiply(multiplier, o) -  t
                movedTo.CopyTo(buffer)
                Buffer.BlockCopy(buffer, 0, result, i * blockSize, blockSize)
            result

        let take3 (origin: float[], target: float[], coeff: float) =

            let origins = MemoryMarshal.Cast<float, Vector<float>>(ReadOnlySpan(origin))
            let targets = MemoryMarshal.Cast<float, Vector<float>>(ReadOnlySpan(target))

            let result = Array.zeroCreate<float> origin.Length
            let blockSize = Vector<float>.Count

            let multiplier = 1.0 - coeff
            for i in 0 .. (origins.Length - 1) do
                let o = origins.[i]
                let t = targets.[i]
                let movedTo = Vector.Multiply(multiplier, o) -  t
                for j in 0 .. (blockSize - 1) do
                    result.[i * blockSize + j] <- movedTo.Item j
            result

        let add (v1: float [], v2: float []) =

            let result = Array.zeroCreate<float> v1.Length
            let blockSize = Vector<float>.Count

            let s1 = MemoryMarshal.Cast<float, Vector<float>>(ReadOnlySpan(v1))
            let s2 = MemoryMarshal.Cast<float, Vector<float>>(ReadOnlySpan(v2))

            for i in 0 .. (s1.Length - 1) do
                let sum = s1.[i] + s2.[i]
                sum.CopyTo(result, i * blockSize)
            result

        let sub (v1: float [], v2: float []) =

            let result = Array.zeroCreate<float> v1.Length
            let blockSize = Vector<float>.Count

            let s1 = MemoryMarshal.Cast<float, Vector<float>>(ReadOnlySpan(v1))
            let s2 = MemoryMarshal.Cast<float, Vector<float>>(ReadOnlySpan(v2))

            for i in 0 .. (s1.Length - 1) do
                let diff = s1.[i] - s2.[i]
                diff.CopyTo(result, i * blockSize)
            result

        let mult (scalar: float, v: float []) =

            let result = Array.zeroCreate<float> v.Length
            let blockSize = Vector<float>.Count

            let s = MemoryMarshal.Cast<float, Vector<float>>(ReadOnlySpan(v))

            for i in 0 .. (s.Length - 1) do
                let multiplied = scalar * s.[i]
                multiplied.CopyTo(result, i * blockSize)
            result

        let take4  (origin: float[], target: float[], coeff: float) =

            sub (mult (1.0 - coeff, origin), target)

    type Benchmark () =

        let rng = Random 0
        // Sample vector size
        // Needs to be a multiple of the vector size at the moment.
        let vSize = Vector<float>.Count
        let size = 10 * vSize

        let origin = Array.init size (fun _ -> rng.NextDouble())
        let target = Array.init size (fun _ -> rng.NextDouble())
        let coeff = 1.5

        [<Benchmark(Baseline=true)>]
        member this.classic () =
            Classic.naive (origin, target, coeff)

        [<Benchmark>]
        member this.simdV1 () =
            SIMD.take1 (origin, target, coeff)

        [<Benchmark>]
        member this.simdV2 () =
            SIMD.take2 (origin, target, coeff)

        [<Benchmark>]
        member this.simdV3 () =
            SIMD.take3 (origin, target, coeff)

        [<Benchmark>]
        member this.simdV4 () =
            SIMD.take4 (origin, target, coeff)
