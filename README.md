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

(Note I compiled in release mode, to maximize any of the inline optimizations)

### Early results were disapointing, but keep scrolling to see the amazing improvement approaching Release 1.0

 C#      | VB      | F#    |
|--- |---| ---|
|  1 threads         |  1 threads  | F# start 240 x 240 |
|C# Compute **8.838 s**   | VB Compute **33.634 s** | F# Compute **228.503 s** |
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
|C# Compute **10.756 s**   | VB Compute **39.558 s** | F# Compute **300.190 s** |
| C# allocated 7,848 bytes  | VB allocated 7,864 bytes  | F# allocated 23,075,240 bytes  |
| 22% slower than 3.11  | 17.6% slower than 3.11 | 31% slower than 3.11 |

Cross the board slowdown - F# is allocating lots of memory! 

### Update 5.2 (.net standard 2.1)
 C#      | VB      | F#    |
|--- |---| ---|
|C# Compute **10.916 s**   | VB Compute **28.076 s** | F# Compute **178.096 s** |
| C# allocated 1,166,120 bytes  | VB allocated 1,169,592 bytes  | F# allocated 24,232,768 bytes  |
|  Draw took 4.438s  |   Draw took 4.467s |  Draw Took 4.424s |
| 1.5% slower than 3.12  | 29.0% faster than 3.12 | 40.6% faster than 3.12 |

F# deploy has been broken for a few versions, but is working again now.

F# is still slowest, but it deploys and runs now, and is much faster that it used to be. VB improved, but still lags behind C#.
Many extraneous files were deleted converting to .net standard.

Drawing the bitmaps is more than twice as slow as it was with 3.11.

There is more memory allocated across the board, but perhaps the library is taking more things into consideration.

### Update 6.0 (.net standard 2.1)
 C#      | VB      | F#    |
|--- |---| ---|
|C# Compute **11.278 s**   | VB Compute **33.883 s** | F# Compute **197.781 s** |
| C# allocated 8,320 bytes  | VB allocated 8,344 bytes  | F# allocated 23,075,832 bytes  |
|  Draw took 1.712s  |   Draw took 1.763s |  Draw Took 1.690s |
| 3.3% slower than 5.2  | 20.7% slower than 5.2 | 11.1% slower than 5.2 |
| 27.6% slower than 3.11  | 0.7% slower than 3.11 | 13.4% faster than 3.11 |

Bitmap drawing is now faster than ever

Memory usage is reduced by about 1.1 Megabytes for each language

All languages are slower than with 5.2. Only F# is faster than with 3.11 (VB is almost the same)

### Update 6.3 
 C#      | VB      | F#    |
|--- |---| ---|
|C# Compute **9.658 s**   | VB Compute **29.536 s** | F# Compute **fail** |
| C# allocated 9,688 bytes  | VB allocated 8,504 bytes  | F# threw FileNotFoundException  |
|  Draw took 1.509s  |   Draw took 1.548s |   |
| 14.6% faster than 6.0  | 12.8% faster than 6.0 |  |

Draw times are 12% faster than 6.0

The Fsharp.Core.dll is too big to fit on the V1 meadow boards. It fails to deploy (getting stuck at 90%) and then is not available at run time.

### Update RC1-1 JIT :collision:
 C#      | VB      | F#    |
|--- |---| ---|
|C# Compute **0.906 s**   | VB Compute **1.16 s** | F# Compute **7.181 s** |
| C# allocated 6,576 bytes  | VB allocated 6,592 bytes  | F# allocated 10,632 bytes  |
|  Draw took 0.284s  |   Draw took 0.286s |  Draw Took 0.283s |
| 11X Faster  | 25X Faster | 28X Faster |

Drawing 5X Faster, Memory allocation is reasonable now.

### Update RC2-2 
 C#      | VB      | F# (7.0.200)   |
|--- |---| ---|
|C# Compute **0.906 s**   | VB Compute **1.161 s** | F# Compute **6.305 s** |
| *Same*  | *Same*  | 12% Faster  |

### Update RC3-1 
 C#      | VB      | F# (7.0.200)   |
|--- |---| ---|
|C# Compute **0.907 s**   | VB Compute **1.158 s** | F# Compute **6.378 s** |
| *Same*  | 0.25% faster  | 1.16% slower  |
Keeps running without crashing for much longer, but still a small memory leak somewhere

# Use standard wiring for Meadow F7 and LCD
![Meadow Frizing](/Meadowbrot/st7789_fritzing.jpg)

I also wanted to highlight my method of extending the GraphicsLibrary through inheritance.
I include some new functionality to draw centered large text on the display,
and reverse the bits of the generated bitmaps so the base GraphicsLibrary displays them correctly.

