using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Lab1
{
    public partial class Form1 : Form
    {
        abstract class Filters
        {
            protected unsafe abstract void CalculateNewPixelColor(byte* ptrIn, byte* ptrOut, ref int stride, ref int x, ref int y);

            public unsafe void ProcessImage(Image initialImage)
            {
                It.histogram.available = false;

                Bitmap inputImage = new Bitmap(initialImage);
                Bitmap outputImage = new Bitmap(inputImage.Width, inputImage.Height);

                BitmapData inputData = inputImage.LockBits(new Rectangle(0, 0, inputImage.Width, inputImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                BitmapData outputData = outputImage.LockBits(new Rectangle(0, 0, inputImage.Width, inputImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                int stride = inputData.Stride;

                byte* ptrIn = (byte*)inputData.Scan0;
                byte* ptrOut = (byte*)outputData.Scan0;

                for (int x = 0; x < inputImage.Width; ++x)
                {
                    for (int y = 0; y < inputImage.Height; ++y)
                    {
                        CalculateNewPixelColor(ptrIn, ptrOut, ref stride, ref x, ref y);
                    }
                    It.progressBar1.Invoke((Action)(() => It.progressBar1.Value = x * progressbarMaxVal / inputImage.Width + 1));
                }
                inputImage.UnlockBits(inputData);
                outputImage.UnlockBits(outputData);
                It.pictureBox1.Image = outputImage;
                It.progressBar1.Invoke((Action)(() => { It.progressBar1.Value = 0; It.EnableButtons(true); }));
            }

            public unsafe Bitmap PartialProcessImage(Image initialImage, int endProgress)
            {
                It.histogram.available = false;
                int startProgress = It.progressBar1.Value;
                int totalProgress = endProgress - startProgress;

                Bitmap inputImage = new Bitmap(initialImage);
                Bitmap outputImage = new Bitmap(inputImage.Width, inputImage.Height);

                BitmapData inputData = inputImage.LockBits(new Rectangle(0, 0, inputImage.Width, inputImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                BitmapData outputData = outputImage.LockBits(new Rectangle(0, 0, inputImage.Width, inputImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                int stride = inputData.Stride;

                byte* ptrIn = (byte*)inputData.Scan0;
                byte* ptrOut = (byte*)outputData.Scan0;

                for (int x = 0; x < inputImage.Width; ++x)
                {
                    for (int y = 0; y < inputImage.Height; ++y)
                    {
                        CalculateNewPixelColor(ptrIn, ptrOut, ref stride, ref x, ref y);
                    }
                    It.progressBar1.Invoke((Action)(() => It.progressBar1.Value = x * totalProgress / inputImage.Width + startProgress));
                }
                inputImage.UnlockBits(inputData);
                outputImage.UnlockBits(outputData);
                It.progressBar1.Invoke((Action)(() => { It.progressBar1.Value = endProgress; }));
                return outputImage;
            }

            public int Clamp(int value, int min, int max)
            {
                if (value < min)
                    return min;
                if (value > max)
                    return max;
                return value;
            }
        }

        class GrayScaleFilter : Filters
        {
            protected unsafe override void CalculateNewPixelColor(byte* ptrIn, byte* ptrOut, ref int stride, ref int x, ref int y)
            {
                byte gray = (byte)(ptrIn[(x * 3) + y * stride + 2] * 0.2989f
                                 + ptrIn[(x * 3) + y * stride + 1] * 0.5870f
                                 + ptrIn[(x * 3) + y * stride]     * 0.1141f);
                // it's BGR !
                ptrOut[(x * 3) + y * stride] = ptrOut[(x * 3) + y * stride + 1] = ptrOut[(x * 3) + y * stride + 2] = (byte)gray;
            }
        }

        class BinarizationFilter : Filters
        {
            protected unsafe override void CalculateNewPixelColor(byte* ptrIn, byte* ptrOut, ref int stride, ref int x, ref int y)
            {
                int gray = (ptrIn[(x * 3) + y * stride + 2]
                          + ptrIn[(x * 3) + y * stride + 1]
                          + ptrIn[(x * 3) + y * stride]) / 3;

                gray = gray > It.slider ? 255 : 0;

                ptrOut[(x * 3) + y * stride] = ptrOut[(x * 3) + y * stride + 1] = ptrOut[(x * 3) + y * stride + 2] = (byte)gray;
            }
        }

        class BrightnessFilter : Filters
        {
            protected unsafe override void CalculateNewPixelColor(byte* ptrIn, byte* ptrOut, ref int stride, ref int x, ref int y)
            {
                int red = ptrIn[(x * 3) + y * stride + 2] + It.slider;
                int green = ptrIn[(x * 3) + y * stride + 1] + It.slider;
                int blue = ptrIn[(x * 3) + y * stride] + It.slider;

                red = Clamp(red, 0, 255);
                green = Clamp(green, 0, 255);
                blue = Clamp(blue, 0, 255);

                ptrOut[(x * 3) + y * stride + 2] = (byte)red;
                ptrOut[(x * 3) + y * stride + 1] = (byte)green;
                ptrOut[(x * 3) + y * stride] = (byte)blue;
            }
        }

        class SepiaFilter : Filters
        {
            protected unsafe override void CalculateNewPixelColor(byte* ptrIn, byte* ptrOut, ref int stride, ref int x, ref int y)
            {
                int r = ptrIn[(x * 3) + y * stride + 2];
                int g = ptrIn[(x * 3) + y * stride + 1];
                int b = ptrIn[(x * 3) + y * stride];

                r = (int)(0.393f * r + 0.769f * g + 0.189f * b);
                g = (int)(0.349f * r + 0.686f * g + 0.168f * b);
                b = (int)(0.272f * r + 0.534f * g + 0.131f * b);

                r = Math.Min(255, r);
                g = Math.Min(255, g);
                b = Math.Min(255, b);

                ptrOut[(x * 3) + y * stride + 2] = (byte)Math.Min(255, r);
                ptrOut[(x * 3) + y * stride + 1] = (byte)Math.Min(255, g);
                ptrOut[(x * 3) + y * stride] = (byte)Math.Min(255, b);
            }
        }

        class GlassFilter : Filters
        {
            static Random rand;

            public GlassFilter()
            {
                rand = new Random();
            }

            protected unsafe override void CalculateNewPixelColor(byte* ptrIn, byte* ptrOut, ref int stride, ref int x, ref int y)
            {
                int xAxis = x + rand.Next(9) - 4;
                int yAxis = y + rand.Next(9) - 4;
                if (It.pictureBox1.Image.Width <= xAxis || xAxis < 0 || It.pictureBox1.Image.Height <= yAxis || yAxis < 0)
                {
                    ptrOut[(x * 3) + y * stride + 2] = ptrIn[(x * 3) + y * stride + 2];
                    ptrOut[(x * 3) + y * stride + 1] = ptrIn[(x * 3) + y * stride + 1];
                    ptrOut[(x * 3) + y * stride] = ptrIn[(x * 3) + y * stride];
                    return;
                }

                ptrOut[(x * 3) + y * stride + 2] = ptrIn[(xAxis * 3) + yAxis * stride + 2];
                ptrOut[(x * 3) + y * stride + 1] = ptrIn[(xAxis * 3) + yAxis * stride + 1];
                ptrOut[(x * 3) + y * stride] = ptrIn[(xAxis * 3) + yAxis * stride];
            }
        }

        class MoveImageXFilter : Filters
        {
            protected unsafe override void CalculateNewPixelColor(byte* ptrIn, byte* ptrOut, ref int stride, ref int x, ref int y)
            {
                int tx = x - It.slider;
                if (tx < 0 || tx >= It.pictureBox1.Image.Width)
                {
                    ptrOut[(x * 3) + y * stride + 2] 
                        = ptrOut[(x * 3) + y * stride + 1] 
                        = ptrOut[(x * 3) + y * stride] = 0;
                    return;
                }

                ptrOut[(x * 3) + y * stride + 2] = ptrIn[(tx * 3) + y * stride + 2];
                ptrOut[(x * 3) + y * stride + 1] = ptrIn[(tx * 3) + y * stride + 1];
                ptrOut[(x * 3) + y * stride] = ptrIn[(tx * 3) + y * stride];
            }
        }

        class MoveImageYFilter : Filters
        {
            protected unsafe override void CalculateNewPixelColor(byte* ptrIn, byte* ptrOut, ref int stride, ref int x, ref int y)
            {
                int ty = y - It.slider;
                if (ty < 0 || ty >= It.pictureBox1.Image.Height)
                {
                    ptrOut[(x * 3) + y * stride + 2]
                        = ptrOut[(x * 3) + y * stride + 1]
                        = ptrOut[(x * 3) + y * stride] = 0;
                    return;
                }

                ptrOut[(x * 3) + y * stride + 2] = ptrIn[(x * 3) + ty * stride + 2];
                ptrOut[(x * 3) + y * stride + 1] = ptrIn[(x * 3) + ty * stride + 1];
                ptrOut[(x * 3) + y * stride] = ptrIn[(x * 3) + ty * stride];
            }
        }

        class RotateImageFilter : Filters
        {
            static private readonly int x0 = It.pictureBox1.Image.Width / 2;
            static private readonly int y0 = It.pictureBox1.Image.Height / 2;
            static private readonly float μ = (float)Math.PI * It.slider / 180.0f;

            protected unsafe override void CalculateNewPixelColor(byte* ptrIn, byte* ptrOut, ref int stride, ref int x, ref int y)
            {
                int tx = (int)((x - x0) * Math.Cos(μ) - (y - y0) * Math.Sin(μ) + x0);
                int ty = (int)((x - x0) * Math.Sin(μ) + (y - y0) * Math.Cos(μ) + y0);

                if (ty < 0 || ty >= It.pictureBox1.Image.Height || tx < 0 || tx >= It.pictureBox1.Image.Width)
                {
                    ptrOut[(x * 3) + y * stride + 2]
                        = ptrOut[(x * 3) + y * stride + 1]
                        = ptrOut[(x * 3) + y * stride] = 0;
                    return;
                }


                ptrOut[(x * 3) + y * stride + 2] = ptrIn[(tx * 3) + ty * stride + 2];
                ptrOut[(x * 3) + y * stride + 1] = ptrIn[(tx * 3) + ty * stride + 1];
                ptrOut[(x * 3) + y * stride] = ptrIn[(tx * 3) + ty * stride];
            }
        }

        class WaveXImageFilter : Filters
        {
            protected unsafe override void CalculateNewPixelColor(byte* ptrIn, byte* ptrOut, ref int stride, ref int x, ref int y)
            {
                int tx = (int)(x + 20 * Math.Sin(2 * Math.PI * y / 60));

                if (tx < 0 || tx >= It.pictureBox1.Image.Width)
                {
                    ptrOut[(x * 3) + y * stride + 2]
                        = ptrOut[(x * 3) + y * stride + 1]
                        = ptrOut[(x * 3) + y * stride] = 0;
                    return;
                }
                ptrOut[(x * 3) + y * stride + 2] = ptrIn[(tx * 3) + y * stride + 2];
                ptrOut[(x * 3) + y * stride + 1] = ptrIn[(tx * 3) + y * stride + 1];
                ptrOut[(x * 3) + y * stride] = ptrIn[(tx * 3) + y * stride];
            }
        }

        class WaveYImageFilter : Filters
        {
            protected unsafe override void CalculateNewPixelColor(byte* ptrIn, byte* ptrOut, ref int stride, ref int x, ref int y)
            {
                int ty = (int)(y + 20 * Math.Sin(2 * Math.PI * x / 60));

                if (ty < 0 || ty >= It.pictureBox1.Image.Height)
                {
                    ptrOut[(x * 3) + y * stride + 2]
                        = ptrOut[(x * 3) + y * stride + 1]
                        = ptrOut[(x * 3) + y * stride] = 0;
                    return;
                }
                ptrOut[(x * 3) + y * stride + 2] = ptrIn[(x * 3) + ty * stride + 2];
                ptrOut[(x * 3) + y * stride + 1] = ptrIn[(x * 3) + ty * stride + 1];
                ptrOut[(x * 3) + y * stride] = ptrIn[(x * 3) + ty * stride];
            }
        }

        class InverseFilter : Filters
        {
            protected unsafe override void CalculateNewPixelColor(byte* ptrIn, byte* ptrOut, ref int stride, ref int x, ref int y)
            {
                ptrOut[(x * 3) + y * stride + 2] = (byte)(255 - ptrIn[(x * 3) + y * stride + 2]);
                ptrOut[(x * 3) + y * stride + 1] = (byte)(255 - ptrIn[(x * 3) + y * stride + 1]);
                ptrOut[(x * 3) + y * stride] = (byte)(255 - ptrIn[(x * 3) + y * stride]);
            }
        }

        class LinearHistogramStretchFilter : Filters
        {
            protected unsafe override void CalculateNewPixelColor(byte* ptrIn, byte* ptrOut, ref int stride, ref int x, ref int y)
            {
                int red = (int)((ptrIn[(x * 3) + y * stride + 2] - It.histogram.min.Item1) * 255.0f / (It.histogram.max.Item1 - It.histogram.min.Item1));
                int green = (int)((ptrIn[(x * 3) + y * stride + 1] - It.histogram.min.Item2) * 255.0f / (It.histogram.max.Item2 - It.histogram.min.Item2));
                int blue = (int)((ptrIn[(x * 3) + y * stride] - It.histogram.min.Item3) * 255.0f / (It.histogram.max.Item3 - It.histogram.min.Item3));

                ptrOut[(x * 3) + y * stride + 2] = (byte)red;
                ptrOut[(x * 3) + y * stride + 1] = (byte)green;
                ptrOut[(x * 3) + y * stride] = (byte)blue;
            }
        }

        class GrayWorldFilter : Filters
        {
            UInt64 sumR = 0, sumG = 0, sumB = 0;
            int avg = 0;

            public unsafe GrayWorldFilter(Bitmap originalImg)
            {
                BitmapData inputData = originalImg.LockBits(new Rectangle(0, 0, originalImg.Width, originalImg.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                int stride = inputData.Stride;
                byte* ptrIn = (byte*)inputData.Scan0;
                UInt64 pixels = (ulong)originalImg.Width * (ulong)originalImg.Height;

                for (UInt64 i = 0; i < pixels; i+=3)
                {
                    sumR += ptrIn[i + 2];
                    sumG += ptrIn[i + 1];
                    sumB += ptrIn[i + 0];
                }
                sumR /= pixels; sumG /= pixels; sumB /= pixels;
                avg = (int)(sumR + sumG + sumB) / 3;

                originalImg.UnlockBits(inputData);
            }

            protected unsafe override void CalculateNewPixelColor(byte* ptrIn, byte* ptrOut, ref int stride, ref int x, ref int y)
            {
                int red = Clamp(ptrIn[(x * 3) + y * stride + 2] * avg / (int)sumR, 0, 255);
                int green = Clamp(ptrIn[(x * 3) + y * stride + 1] * avg / (int)sumG, 0, 255);
                int blue = Clamp(ptrIn[(x * 3) + y * stride] * avg / (int)sumB, 0, 255);

                ptrOut[(x * 3) + y * stride + 2] = (byte)red;
                ptrOut[(x * 3) + y * stride + 1] = (byte)green;
                ptrOut[(x * 3) + y * stride] = (byte)blue;
            }
        }

        class PerfectReflectorFilter : Filters
        {
            protected unsafe override void CalculateNewPixelColor(byte* ptrIn, byte* ptrOut, ref int stride, ref int x, ref int y)
            {
                int red = Math.Min((int)(ptrIn[x * 3 + y * stride + 2] * (255.0f / It.histogram.max.Item1)), 255);
                int green = Math.Min((int)(ptrIn[x * 3 + y * stride + 1] * (255.0f / It.histogram.max.Item2)), 255);
                int blue = Math.Min((int)(ptrIn[x * 3 + y * stride] * (255.0f / It.histogram.max.Item3)), 255);

                ptrOut[x * 3 + y * stride + 2] = (byte)red;
                ptrOut[x * 3 + y * stride + 1] = (byte)green;
                ptrOut[x * 3 + y * stride] = (byte)blue;
            }
        }

        public void CorrectionWithReferenceColor()
        {
            string text = It.Text;
            It.Text = "PIPETTE";
            pipette = true;
        }
    }
}
