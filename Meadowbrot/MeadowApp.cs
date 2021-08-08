using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Displays.TftSpi;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.MyExtensions;
using Meadow.Hardware;
using System;
using System.Diagnostics;
using System.Threading;

namespace Meadowbrot
{
    /// <summary>
    /// The Portions of this application that are not the actual Mandelbrot Code are 
    // Copyright (c) 2020 Mark Alan Jones
    //
    //Permission is hereby granted, free of charge, to any person obtaining a copy
    //of this software and associated documentation files (the "Software"), to deal
    //in the Software without restriction, including without limitation the rights
    //to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    //copies of the Software, and to permit persons to whom the Software is
    //furnished to do so, subject to the following conditions:
    //
    //The above copyright notice and this permission notice shall be included in all
    //copies or substantial portions of the Software.
    //
    //THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    //IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    //FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    //AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    //LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    //OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    //SOFTWARE.
    /// </summary>
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        St7789 display;
        GraphicsLibraryEx graphics;
        const int displayWidth = 240;
        const int displayHeight = 240;
        byte[][] data;
        byte[] fdata;
        Color displayColor;
        int loop = 1;

        public MeadowApp()
        {
            Initialize();

            // Generate a 240x240 Mandelbrot image in each of the languages
            // Timing is output to the console - The timing does not include the time to load the dlls.
            while (true)
            {
                Console.WriteLine("--------------------------------------------------------------------------");
                Console.WriteLine($"{loop++}) {DateTime.Now:T}");

                var totmem = GC.GetAllocatedBytesForCurrentThread(); // running total of memory used, without counting freed memory
                displayColor = Color.CornflowerBlue;
                graphics.DrawBigCenteredText("C#", displayColor);
                data = MandelbrotC.Generate(displayWidth);
                Draw(data);
                Console.WriteLine($"C# allocated {GC.GetAllocatedBytesForCurrentThread() - totmem:N0} bytes ");
                Thread.Sleep(5000);

                totmem = GC.GetAllocatedBytesForCurrentThread();
                displayColor = Color.Firebrick;
                graphics.DrawBigCenteredText("VB", displayColor);
                data = MandelbrotVB.Meadowbrot.MandelbrotVB.Generate(displayWidth);
                Draw(data);
                Console.WriteLine($"VB allocated {GC.GetAllocatedBytesForCurrentThread() - totmem:N0} bytes ");
                Thread.Sleep(5000);

                totmem = GC.GetAllocatedBytesForCurrentThread();
                displayColor = Color.ForestGreen;
                graphics.DrawBigCenteredText("F#", displayColor);
                fdata = MandelbrotFSharp.Generate(displayWidth);
                Draw(fdata);
                Console.WriteLine($"F# allocated {GC.GetAllocatedBytesForCurrentThread() - totmem:N0} bytes ");
                Thread.Sleep(5000);

                Console.WriteLine($"Total inuse memory {GC.GetTotalMemory(false):N0} bytes ");
            }
        }

        void Initialize()
        {
            Console.WriteLine("Initializing...");

            var config = new SpiClockConfiguration(48000, SpiClockConfiguration.Mode.Mode3);
            var spiBus = Device.CreateSpiBus(Device.Pins.SCK, Device.Pins.MOSI, Device.Pins.MISO, config);

            display = new St7789(
                device: Device,
                spiBus: spiBus,
                chipSelectPin: null,
                dcPin: Device.Pins.D01,
                resetPin: Device.Pins.D00,
                width: displayWidth, height: displayHeight);

            // extended graphics library
            graphics = new GraphicsLibraryEx(display);
            graphics.Rotation = GraphicsLibrary.RotationType._270Degrees;
            graphics.CurrentFont = new Font12x20();

            graphics.Clear(true);
        }

        void Draw(byte[][] data)
        {
            Console.WriteLine($"  Drawing {data[0].Length * 8}x{data.Length}...");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            graphics.Clear(true);

            for (int y = 0; y < data.Length; y++)
            {
                // each line of data is a 1 bit bitmap
                graphics.DrawReverseBitmap(0, y, data[y].Length, 1, data[y], displayColor);
            }

            graphics.Show();

            sw.Stop();
            Console.WriteLine($"  Draw took {sw.ElapsedMilliseconds / 1000.0}s");
        }

        /// <summary>
        /// 1D array from f#
        /// </summary>
        void Draw(byte[] data)
        {
            int size = (int)Math.Sqrt(data.Length * 8);
            Console.WriteLine($"  Drawing {size}x{size}...");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            graphics.Clear(true);

            // one big multiline bitmap
            graphics.DrawReverseBitmap(0, 0, size / 8, size, data, displayColor);

            graphics.Show();

            sw.Stop();
            Console.WriteLine($"  Draw Took {sw.ElapsedMilliseconds / 1000.0}s");
        }
    }
}
