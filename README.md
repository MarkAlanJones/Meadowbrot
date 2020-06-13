# Meadowbrot
A Meadow F7 triple language benchmark utilizing the Mandelbrot set (C# VB F#)

This benchmark demonstrates computing the <a href="https://en.wikipedia.org/wiki/Mandelbrot_set">Mandelbrot Set</a>
in the 3 main .net languages; **C#**, **VB**, and **F#**

![Mandelbrot Set](https://upload.wikimedia.org/wikipedia/en/thumb/e/ef/Mandelbrot_black_itr20.png/320px-Mandelbrot_black_itr20.png)

The original benchmarks came from the <a href="https://salsa.debian.org/benchmarksgame-team/benchmarksgame/">Debian Benchmarks game project</a>, 
where they had the C# and F# code, specifically for .net core console apps. In their tests, they demonstrated F# running faster than C#.
They didn't include VB code, but I was able to start the translation from c# with telerik's <a href="https://converter.telerik.com/">Converter</a>.
The converter crashed so I had to hand convert some of the logic. On the F# side, the Parallel.For had to be removed as it just crashed the program.
On the VB side the arrays were overrunning for some reason, so there is an extra byte in each bitmap.

The only way I know to have more than one language in the same project, is to create dlls. the VB and F# are in seperate dlls.
The timing for the algorithm starts inside the generate method, so the time to load the dll is not included in the measurement.
I left the C# inside the main executable for this reason.

Timings are written to the console by each language. F# uses printfn for this.
F# in particular includes a large runtime, that is slow to deploy the first time.

Here are the deployed project files:
-  Deploying to Meadow on COM4...
-  Initializing Meadow                                                                                                     
-  Device  MeadowOS Version: 0.3.11 (May 22 2020 21:40:16)                                                                
-  Checking files on device (may take several seconds)  
-   Found System.dll                                                                                                        
-   Found System.Core.dll                                                                                                   
-   Found mscorlib.dll                                                                                                      
-   Found Meadow.dll                                                                                                        
-   Found App.exe                                                                                                           
-   Found Meadow.Foundation.dll                                                                                             
-   Found GraphicsLibrary.dll                                                                                               
-   Found TftSpi.dll                                                                                                       
-   Found FSharp.Core.dll                                                                                                
-   Found MandelbrotF.dll                                                                                                  
-   Found MandelbrotVB.dll                                                                                                  
-   Found System.Numerics.Vectors.dll                                                                                        
-   Found System.Runtime.CompilerServices.Unsafe.dll                                                                         
-  Writing App.exe                                                                                                         
-  Writing MandelbrotF.dll                                                                                                  
-  Resetting Meadow and starting app (30-60s)                                                                              
-  Deployment Duration: 00:00:19.1825112                                                                                   

And now for the rather disappointing results...
(Note I compiled in release mode, to maximize any of the inline optimizations)

 C#      | VB      | F#    |
|--- |---| ---|
|  1 threads         |  1 threads  | F# start 240 x 240 |
|C# Compute **8.838s**   | VB Compute **33.634s** | F# Compute **228.503000 s** |
|  Drawing 240x240...| Drawing 248x240...|   Drawing 240x240... |
|  Draw took 2.058s  |   Draw took 2.015s |  Draw Took 1.983s |
|  1X           | 3.8X slower | 25.9X slower |

The F# bitmap displays a little bit faster because it is a single bitmap, instead of 240 1 line bitmaps that C#/VB produce.

**Yes it is more than 25 times slower !!**

My desktop machine generates 240x240 images in 0.076s (VB) and 0.059s (C#) (in debug mode) 

That clocks the Meadow at 150X slower than a i5 (Granted there is alot of double precision floating point math) 


# Use standard wiring for Meadow F7 and LCD
![Meadow Frizing](/Meadowbrot/st7789_fritzing.jpg)

I also wanted to highlight my method of extending the GraphicsLibrary through inheritance.
I include some new functionality to draw centered large text on the display,
and reverse the bits of the generated bitmaps so the base GraphicsLibrary displays them correctly.

