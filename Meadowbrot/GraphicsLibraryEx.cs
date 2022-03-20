using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;
using System;

namespace Meadow.Foundation.MyExtensions
{

    // Extend the GraphicsLibrary 
    public class GraphicsLibraryEx : MicroGraphics
    {
        private readonly IGraphicsDisplay display;
        public GraphicsLibraryEx(IGraphicsDisplay display) : base(display)
        {
            this.display = display;
        }

        public void DrawBigCenteredText(string text, Color color, bool clear = true, bool show = true)
        {
            if (clear)
                Clear(true);

            ScaleFactor big = ScaleFactor.X3;
            DrawText(((int)display.Width - CurrentFont.Width * text.Length * (int)big) / 2,
                      ((int)display.Height - CurrentFont.Height * (int)big) / 2,
                      text, color, big);
            if (show)
                Show();
        }

        // DrawBitmap but reverse each byte first
        public void DrawReverseBitmap(int x, int y, int width, int height, byte[] bitmap, Color color, ScaleFactor scaleFactor = ScaleFactor.X1)
        {
            for (int b = 0; b < bitmap.Length; b++)
            {
                // reverse bits in byte 
                bitmap[b] = Reverse(bitmap[b]);
            }

            base.DrawBitmap(x, y, width * 8, height, bitmap, color, scaleFactor);
        }

        // Reverses bits in a byte
        private static byte Reverse(byte inByte)
        {
            byte result = 0x00;

            for (byte mask = 0x80; Convert.ToInt32(mask) > 0; mask >>= 1)
            {
                // shift right current result
                result = (byte)(result >> 1);

                // tempbyte = 1 if there is a 1 in the current position
                var tempbyte = (byte)(inByte & mask);
                if (tempbyte != 0x00)
                {
                    // Insert a 1 in the left
                    result = (byte)(result | 0x80);
                }
            }

            return (result);
        }
    }
}