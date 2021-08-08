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

Here are the deployed project files (3.11 deployment):
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
|C# Compute **8.838s**   | VB Compute **33.634s** | F# Compute **228.503000s** |
|  Drawing 240x240...| Drawing 248x240...|   Drawing 240x240... |
|  Draw took 2.058s  |   Draw took 2.015s |  Draw Took 1.983s |
|  1X           | 3.8X slower | 25.9X slower |

The F# bitmap displays a little bit faster because it is a single bitmap, instead of 240 1 line bitmaps that C#/VB produce.

**Yes it is more than 25 times slower !!**

My desktop machine generates 240x240 images in 0.076s (VB) and 0.059s (C#) (in debug mode) 

That clocks the Meadow at 150X slower than an i5 (Granted there is alot of double precision floating point math) 

### Update 3.12 
 C#      | VB      | F#    |
|--- |---| ---|
|C# Compute **10.756s**   | VB Compute **39.558s** | F# Compute **300.190000s** |
| C# allocated 7,848 bytes  | VB allocated 7,864 bytes  | F# allocated 23,075,240 bytes  |
| 22% slower than 3.11  | 17.6% slower than 3.11 | 31% slower than 3.11 |

Cross the board slowdown - F# is allocating lots of memory! 

### Update 5.2 (.net standard 2.1)
 C#      | VB      | F#    |
|--- |---| ---|
|C# Compute **10.916s**   | VB Compute **28.076s** | F# Compute **178.096 s** |
| C# allocated 1,166,120 bytes  | VB allocated 1,169,592 bytes  | F# allocated 24,232,768 bytes  |
|  Draw took 4.438s  |   Draw took 4.467s |  Draw Took 4.424s |
| 1.5% slower than 3.12  | 29.0% faster than 3.12 | 40.6% faster than 3.12 |

F# deploy has been broken for a few versions, but is working again now.

F# is till slowest, but it deploys and runs now, and is much faster that it used to be. VB improved, but still lags behind C#.
Many extraneous files could be deleted converting to .net standard.

Drawing the bitmaps is more than twice as slow as it was with 3.11.

There is more memory allocated across the board, but perhaps the library is taking more things into consideration.

# Use standard wiring for Meadow F7 and LCD
![Meadow Frizing](/Meadowbrot/st7789_fritzing.jpg)

I also wanted to highlight my method of extending the GraphicsLibrary through inheritance.
I include some new functionality to draw centered large text on the display,
and reverse the bits of the generated bitmaps so the base GraphicsLibrary displays them correctly.

