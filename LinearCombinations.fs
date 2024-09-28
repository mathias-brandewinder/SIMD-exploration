namespace SIMD.Experiments

[<RequireQualifiedAccess>]
module LinearCombinations =

    open System
    open System.Runtime.InteropServices
    open System.Numerics
    open BenchmarkDotNet.Attributes

    module Classic =

        let naive (vectors: (float * float[]) []) =
            let dim = (vectors.[0] |> snd).Length
            Array.init dim (fun col ->
                vectors
                |> Seq.sumBy (fun (coeff, v) -> coeff * v.[col])
                )

    module SIMD =

        // This is NOT CORRECT.
        // Work in progress: attempting to sum up an arbitrary list of vectors.
        // The sum part is missing: I am just overwriting the result with the
        // latest vector computed.
        let take1 (vectors: (float * float[]) []) =

            let dim = (vectors.[0] |> snd).Length

            let result = Array.zeroCreate<float> dim
            let blockSize = Vector<float>.Count

            for (coeff, vector) in vectors do

                let asVectors = MemoryMarshal.Cast<float, Vector<float>>(ReadOnlySpan(vector))
                let blocks = asVectors.Length
                for i in 0 .. (blocks - 1) do
                    let v = asVectors.[i]
                    let x = Vector.Multiply(coeff, v)
                    x.CopyTo(result, i * blockSize)

            result

    type Benchmark () =

        let rng = Random 0
        // Sample vector size
        // Needs to be a multiple of the vector size at the moment.
        let vSize = Vector<float>.Count
        let size = 10 * vSize
        let count = 20

        let vectors =
            Array.init count (fun _ ->
                rng.NextDouble(),
                Array.init size (fun _ -> rng.NextDouble())
                )

        [<Benchmark(Baseline=true)>]
        member this.classic () =
            Classic.naive vectors

        [<Benchmark>]
        member this.simdV1 () =
            SIMD.take1 vectors
