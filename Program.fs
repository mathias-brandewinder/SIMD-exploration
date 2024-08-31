namespace SIMD.Experiments

module App =

    open BenchmarkDotNet.Running

    [<EntryPoint>]
    let main (args: string []) =

        BenchmarkRunner.Run<Distance.Benchmark>()
        |> ignore

        printfn "Done"
        0