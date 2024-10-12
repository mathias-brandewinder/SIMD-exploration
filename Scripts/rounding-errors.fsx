open System

let rng = Random 0

let data = Array.init 16 (fun _ -> rng.NextDouble())

// directly summing all the numbers together:
let sum = data |> Array.sum
// summing the numbers by groups of 4,
// then summing the groups together:
let sumByBlocks =
    data
    |> Array.chunkBySize 4
    |> Array.map (fun chunk -> chunk |> Array.sum)
    |> Array.sum

printfn $"Equal: {sum = sumByBlocks} ({sum}, {sumByBlocks})"