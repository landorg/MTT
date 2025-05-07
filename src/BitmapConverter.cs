using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MTT
{
    public static class BitmapConverter
    {
        public static Bitmap CreatTestBitmap(ListView table, decimal sum, int length)
        {
            int width = 432;

            Bitmap objBmpImage = new Bitmap(width, length);

            // Create the Font object for the image text drawing.
            Font defaultFont = new Font("Arial", 21, FontStyle.Bold, GraphicsUnit.Pixel);
            Font sumFont = new Font("Arial", 32, FontStyle.Bold, GraphicsUnit.Pixel);

            // Add the colors to the new bitmap.
            Graphics objGraphics = Graphics.FromImage(objBmpImage);

            // Set Background color
            objGraphics.Clear(Color.White);
            objGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            objGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            int borderTop = 40;
            int borderBottom = 20;
            int borderLeft = 30;
            int borderRight = 40;
            int x0 = 0 + borderLeft;
            int y0 = 0 + borderTop;
            int w = width - borderRight - borderLeft;
            int h = length - borderTop - borderBottom;

            objGraphics.DrawImage(Image.FromFile("C:/MTT/src/logo.png"), x0 + w/4, y0, w/2, w/2);

            int yH = y0 + w / 2;

            StringFormat centered = new StringFormat();
            centered.LineAlignment = StringAlignment.Center;
            centered.Alignment = StringAlignment.Center;

            objGraphics.DrawString("Vielen Dank!", defaultFont, new SolidBrush(Color.Black), x0 + w / 2, yH + 25, centered);

            int yR = yH + 45;
            int hR = h - yH - 20 - 40;

            Bitmap bill = new Bitmap(table.Width, table.Height);
            table.DrawToBitmap(bill, new Rectangle(0, 0, table.Width, table.Height));
            objGraphics.DrawImage(bill, x0 , yR, w, hR);

            int yS = h - 30;
            int hS = 30;

            objGraphics.DrawString($"Summe: {sum:0.00}€", sumFont, new SolidBrush(Color.Black), x0 + w / 2, yS + 15, centered);

            //objGraphics.DrawRectangle(
            //    new Pen(new SolidBrush(Color.Black), 2),
            //    new Rectangle(x0, y0, w, h));

            //// top left
            //objGraphics.DrawRectangle(
            //    new Pen(new SolidBrush(Color.Black), 1),
            //    new Rectangle(x0, y0, w / 2, h / 2));
            //// top right
            //objGraphics.DrawRectangle(
            //    new Pen(new SolidBrush(Color.Black), 1),
            //    new Rectangle(x0 + w/2, y0, w / 2, h / 2));

            //// bottom right
            //objGraphics.DrawRectangle(
            //    new Pen(new SolidBrush(Color.Black), 1),
            //    new Rectangle(x0 + w / 2, y0 + h/2, w / 2, h / 2));

            //// bottom left
            //objGraphics.DrawRectangle(
            //    new Pen(new SolidBrush(Color.Black), 1),
            //    new Rectangle(x0, y0 + h / 2, w / 2, h / 2));

            //objGraphics.DrawString("h: "+length.ToString(), defaultFont, new SolidBrush(Color.Black), 150, 10);

            //objGraphics.DrawString("Gewicht", defaultFont, new SolidBrush(Color.Black), 50, 150);
            //objGraphics.DrawString("10 kg", titleFont, new SolidBrush(Color.Black), 210, 150);
            
            //objGraphics.DrawImage(img, 80, 280);
            objGraphics.Flush();

            return (objBmpImage);
        }

        public static Bitmap DrawReciept()
        {
            int width = 432;
            int length = 680;
            BarcodeLib.Barcode b = new BarcodeLib.Barcode();
            Image img = b.Encode(BarcodeLib.TYPE.EAN13, "872312898734", Color.Black, Color.White, 290, 80);

            Bitmap objBmpImage = new Bitmap(width, length);

            // Create the Font object for the image text drawing.
            Font titleFont = new Font("Arial", 28, FontStyle.Bold, GraphicsUnit.Pixel);
            Font defaultFont = new Font("Arial", 21, FontStyle.Bold, GraphicsUnit.Pixel);

            // Add the colors to the new bitmap.
            Graphics objGraphics = Graphics.FromImage(objBmpImage);

            // Set Background color
            objGraphics.Clear(Color.White);
            objGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            objGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            int borderTop = 10;
            int borderSide = 30;
            int x0 = 0 + borderSide;
            int y0 = 0 + borderTop;
            int w = width - borderSide * 2;
            int h = length - borderTop * 2;


            objGraphics.DrawRectangle(
                new Pen(new SolidBrush(Color.Black), 2),
                new Rectangle(x0, y0, w, h));

            // top left
            objGraphics.DrawRectangle(
                new Pen(new SolidBrush(Color.Black), 1),
                new Rectangle(x0, y0, w / 2, h / 2));
            // top right
            objGraphics.DrawRectangle(
                new Pen(new SolidBrush(Color.Black), 1),
                new Rectangle(x0 + w / 2, y0, w / 2, h / 2));

            // bottom right
            objGraphics.DrawRectangle(
                new Pen(new SolidBrush(Color.Black), 1),
                new Rectangle(x0 + w / 2, y0 + h / 2, w / 2, h / 2));

            // bottom left
            objGraphics.DrawRectangle(
                new Pen(new SolidBrush(Color.Black), 1),
                new Rectangle(x0, y0 + h / 2, w / 2, h / 2));

            objGraphics.DrawString("h: " + length.ToString(), defaultFont, new SolidBrush(Color.Black), 150, 10);

            objGraphics.DrawString("Gewicht", defaultFont, new SolidBrush(Color.Black), 50, 150);
            objGraphics.DrawString("10 kg", titleFont, new SolidBrush(Color.Black), 210, 150);

            objGraphics.DrawImage(img, 80, 280);
            objGraphics.Flush();

            return (objBmpImage);
        }

        public static Bitmap BitmapTo1Bpp2(Bitmap source)
        {
            int Width = source.Width;
            int Height = source.Height;
            source.RotateFlip(RotateFlipType.RotateNoneFlipY);

            Bitmap dest = new Bitmap(Width, Height, PixelFormat.Format1bppIndexed);
            BitmapData destBmpData = dest.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);

            byte[] destBytes = new byte[(Width + 7) / 8];//19 bytes

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Color c = source.GetPixel(x, y);

                    if (x % 8 == 0)
                    {
                        destBytes[x / 8] = 0;
                    }
                    if (c.GetBrightness() >= 0.8)
                    {
                        destBytes[x / 8] |= (byte)(0x80 >> (x % 8));
                    }
                }
                Marshal.Copy(destBytes, 0, (IntPtr)((long)destBmpData.Scan0 + destBmpData.Stride * y), destBytes.Length);
            }

            dest.UnlockBits(destBmpData);
            return dest;
        }

        public static byte[] Convert(Bitmap bmp)
        {
            var size = bmp.Width * bmp.Height / 8;
            var buffer = new byte[size];

            var i = 0;
            for (var y = 0; y < bmp.Height; y++)
            {
                for (var x = 0; x < bmp.Width; x++)
                {
                    var color = bmp.GetPixel(x, y);
                    if (color.B != 255 || color.G != 255 || color.R != 255)
                    {
                        var pos = i / 8;
                        var bitInByteIndex = x % 8;

                        buffer[pos] |= (byte)(1 << 7 - bitInByteIndex);
                    }
                    i++;
                }
            }

            return buffer;
        }
    }
}
