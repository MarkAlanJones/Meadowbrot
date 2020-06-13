// The Computer Language Benchmarks Game
// https://benchmarksgame-team.pages.debian.net/benchmarksgame/
//
// Adapted by Antti Lankila from the earlier Isaac Gouy's implementation
// Add multithread & tweaks from C++ by The Anh Tran
// Credits: Anthony Lloyd, Jomo Fisher, Peter Kese

// https://benchmarksgame-team.pages.debian.net/benchmarksgame/program/mandelbrot-fsharpcore-6.html

module MandelbrotFSharp

open System.Diagnostics

#nowarn "9"

open System.Numerics
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.NativeInterop

let inline padd p i = p+8n*i
let inline ptrGet p i = Unsafe.Read((p+8n*i).ToPointer())
let inline ptrSet p i v = Unsafe.Write((p+8n*i).ToPointer(), v)

let inline getByte (ciby:float) pcrbi =
    let rec calc i res =
        let vCrbx = ptrGet pcrbi i
        let vCiby = Vector ciby
        let mutable zr = vCrbx
        let mutable zi = vCiby
        let mutable zrzr = zr*zr
        let mutable zizi = zi*zi
        let mutable j = 49
        let mutable b = 0
        while b<>3 && j>0 do
            j <- j-1
            let zr' = zrzr - zizi + vCrbx
            zi <- let zrzi = zr * zi in zrzi + zrzi + vCiby
            zr <- zr'
            zrzr <- zr*zr
            zizi <- zi*zi
            let t = zrzr + zizi
            b <- b ||| if t.[0]>4.0 then 2 else 0
                   ||| if t.[1]>4.0 then 1 else 0
        let res' = (res <<< 2) + b
        let i' = i + 2n
        if i'=8n then res'
        else calc i' res'
    calc 0n 0 ^^^ -1 |> byte

let Generate dimensions:byte[] =
    let size = dimensions

    printfn "F# start%ix%i" size size
    let sw =  new Stopwatch();
    sw.Start()

    let lineLength = size >>> 3
    let data = Array.zeroCreate (size*lineLength)
    let crb = Array.zeroCreate (size+2)
    use pdata = fixed &data.[0]
    use pcrb = fixed &crb.[0]
    let pcrbi = NativePtr.toNativeInt pcrb
    let invN = Vector (2.0/float size)
    let onePtFive = Vector 1.5
    let step = Vector 2.0
    
    let rec loop i value =
        if i<size then
            ptrSet pcrbi (nativeint i) (value*invN-onePtFive)
            loop (i+2) (value+step)
    Vector [|0.0;1.0;0.0;0.0;0.0;0.0;0.0;0.0|] |> loop 0
    
    // Parallel.For didn't work
    for y = 0 to size do
        let ciby = NativePtr.get pcrb y+0.5
        for x = 0 to lineLength-1 do
            nativeint x*8n |> padd pcrbi |> getByte ciby
            |> NativePtr.set pdata (y*lineLength+x)

    sw.Stop()
    let elapsed = float sw.ElapsedMilliseconds / 1000.0
    printfn "F# Compute%fs" elapsed 

    data