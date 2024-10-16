namespace SIMD.Experiments

module Tests =

    open System
    open Expecto

    [<Tests>]
    let tests =

        let equalWithin (digits: int) (x1: float) (x2: float) =
            Math.Round(x1, digits) = Math.Round(x2, digits)

        testList "benchmarks self-checks" [

            test "distance" {
                let benchmark = Distance.Benchmark()
                Expect.isTrue (benchmark.classic() = benchmark.classicOptimized()) "classic = optimized classic"
                Expect.isTrue (equalWithin 6 (benchmark.classic()) (benchmark.simdV1())) "classic and SIMD v1"
                Expect.isTrue (equalWithin 6 (benchmark.classic()) (benchmark.simdV2())) "classic and SIMD v2"
                Expect.isTrue (equalWithin 6 (benchmark.classic()) (benchmark.simdV3())) "classic and SIMD v3"
                Expect.isTrue (equalWithin 6 (benchmark.classic()) (benchmark.full())) "classic and SIMD with arbitrary length"
                }

            test "average" {
                let benchmark = Average.Benchmark()
                Expect.isTrue (equalWithin 6 (benchmark.classic()) (benchmark.simd())) "classic and SIMD"
                }

            test "log-likelihood" {
                let benchmark = LogLikelihood.Benchmark()
                Expect.isTrue (equalWithin 6 (benchmark.classic()) (benchmark.simdV1())) "classic and SIMD v1"
                }
            ]
