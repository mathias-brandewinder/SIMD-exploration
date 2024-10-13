namespace SIMD.Experiments

module App =

    open BenchmarkDotNet.Running

    [<EntryPoint>]
    let main (args: string []) =

        // BenchmarkRunner.Run<Distance.Benchmark>()
        // |> ignore

        // BenchmarkRunner.Run<Average.Benchmark>()
        // |> ignore

        BenchmarkRunner.Run<MoveTowards.Benchmark>()
        |> ignore

        // BenchmarkRunner.Run<LinearCombinations.Benchmark>()
        // |> ignore

        // BenchmarkRunner.Run<LogLikelihood.Benchmark>()
        // |> ignore

        0