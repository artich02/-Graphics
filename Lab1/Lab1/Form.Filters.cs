using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab1
{
    public partial class Form1 : Form
    {
        abstract class Filters
        {
            protected abstract Color calculateNewPixelColor(Bitmap sourceImage, int x, int y);

            public void processImage(Bitmap sourceImage)
            {
                Bitmap resultImg = new Bitmap(sourceImage.Width, sourceImage.Height);
                for (int i = 0; i < sourceImage.Width; i++)
                {
                    for (int j = 0; j < sourceImage.Height; j++)
                    {
                        resultImg.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                    }
                    It.progressBar1.Invoke((Action)(() => It.progressBar1.Value = i * progressbarMaxVal / sourceImage.Width + 1));
                }
                It.pictureBox1.Image = resultImg;
                It.progressBar1.Invoke((Action)(() => { It.progressBar1.Value = 0; It.EnableButtons(true); }));
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

        class ToGrayScale : Filters
        {
            protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
            {
                Color originalColor = sourceImage.GetPixel(x, y);
                byte grayValue = (byte)(0.2989f * originalColor.R
                                      + 0.5870f * originalColor.G
                                      + 0.1141f * originalColor.B);
                return Color.FromArgb(grayValue, grayValue, grayValue);
            }
        }

        class Binarization : Filters
        {
            protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
            {
                Color originalColor = sourceImage.GetPixel(x, y);

                int gray = (originalColor.R + originalColor.G + originalColor.B) / 3;
                int binaryValue = gray > It.slider ? 255 : 0;

                return Color.FromArgb(binaryValue, binaryValue, binaryValue);
            }
        }

        class Brightness : Filters
        {
            protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
            {
                Color originalColor = sourceImage.GetPixel(x, y);
                int red = (int)(originalColor.R + It.slider);
                int green = (int)(originalColor.G + It.slider);
                int blue = (int)(originalColor.B + It.slider);

                red = Math.Min(255, Math.Max(0, red));
                green = Math.Min(255, Math.Max(0, green));
                blue = Math.Min(255, Math.Max(0, blue));

                return Color.FromArgb(red, green, blue);
            }
        }

        class Sepia : Filters
        {
            protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
            {
                Color pixelColor = sourceImage.GetPixel(x, y);
                int r = pixelColor.R;
                int g = pixelColor.G;
                int b = pixelColor.B;

                int newR = (int)(0.393 * r + 0.769 * g + 0.189 * b);
                int newG = (int)(0.349 * r + 0.686 * g + 0.168 * b);
                int newB = (int)(0.272 * r + 0.534 * g + 0.131 * b);

                newR = Math.Min(255, newR);
                newG = Math.Min(255, newG);
                newB = Math.Min(255, newB);

                return Color.FromArgb(newR, newG, newB);
            }
        }

        class GlassFilter : Filters
        {
            static Random rand;

            public GlassFilter()
            {
                rand = new Random();
            }

            protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
            {
                int xAxis = x + rand.Next(9) - 4;
                int yAxis = y + rand.Next(9) - 4;
                if (sourceImage.Width <= xAxis || xAxis < 0 || sourceImage.Height <= yAxis || yAxis < 0)
                    return sourceImage.GetPixel(x, y);

                return sourceImage.GetPixel(xAxis, yAxis);
            }
        }

        class MoveImageFilterX : Filters
        {
            protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
            {
                int tx = x - It.slider;
                if (tx < 0 || tx >= sourceImage.Width)
                    return Color.Black;
                return sourceImage.GetPixel(tx, y);
            }
        }

        class MoveImageFilterY : Filters
        {
            protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
            {
                int ty = y - It.slider;
                if (ty < 0 || ty >= sourceImage.Height)
                    return Color.Black;
                return sourceImage.GetPixel(x, ty);
            }
        }

        class RotateImageFilter : Filters
        {
            static private readonly int x0 = It.pictureBox1.Image.Width / 2;
            static private readonly int y0 = It.pictureBox1.Image.Height / 2;
            static private readonly float μ = (float)Math.PI * It.slider / 180.0f;

            protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
            {
                int tx = (int)((x - x0) * Math.Cos(μ) - (y - y0) * Math.Sin(μ) + x0);
                int ty = (int)((x - x0) * Math.Sin(μ) + (y - y0) * Math.Cos(μ) + y0);

                if (ty < 0 || ty >= sourceImage.Height || tx < 0 || tx >= sourceImage.Width)
                    return Color.Black;
                return sourceImage.GetPixel(tx, ty);
            }
        }

        class WaveXImageFilter : Filters
        {
            protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
            {
                int tx = (int)(x + 20 * Math.Sin(2 * Math.PI * y / 60));

                if (tx < 0 || tx >= sourceImage.Width)
                    return Color.Black;
                return sourceImage.GetPixel(tx, y);
            }
        }

        class WaveYImageFilter : Filters
        {
            protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
            {
                int ty = (int)(y + 20 * Math.Sin(2 * Math.PI * x / 60));

                if (ty < 0 || ty >= sourceImage.Height)
                    return Color.Black;
                return sourceImage.GetPixel(x, ty);
            }
        }

    }
}
