namespace SIMD.Experiments

module App =

    open BenchmarkDotNet.Running

    [<EntryPoint>]
    let main (args: string []) =

        BenchmarkRunner.Run<Distance.Benchmark>()
        |> ignore

        // BenchmarkRunner.Run<Average.Benchmark>()
        // |> ignore

        0