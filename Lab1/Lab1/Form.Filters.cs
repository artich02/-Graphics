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
        public void ConvertToGrayScale()
        {
            Filters filter = new ToGrayScale();
            filter.processImage((Bitmap)pictureBox1.Image);
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
        public void Binarize()
        {
            Filters filter = new Binarization();
            filter.processImage((Bitmap)pictureBox1.Image);
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
        public void AdjustBrightness()
        {
            Filters filter = new Brightness();
            filter.processImage((Bitmap)pictureBox1.Image);
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
        public void ApplySepiaFilter()
        {
            Filters filter = new Sepia();
            filter.processImage((Bitmap)pictureBox1.Image);
        }

        public void Glass()
        {
            Bitmap inputImage = new Bitmap(pictureBox1.Image);
            Bitmap resultImage = inputImage;
            var rand = new Random();

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    Color pixelColor = inputImage.GetPixel(x, y);

                    int xAxis = x + rand.Next(9) - 4;
                    int yAxis = y + rand.Next(9) - 4;
                    if (inputImage.Width <= xAxis || xAxis < 0 || inputImage.Height <= yAxis || yAxis < 0)
                        break;
                    resultImage.SetPixel(xAxis, yAxis, pixelColor);
                }
                progressBar1.Invoke((Action)(() => progressBar1.Value = y * progressbarMaxVal / inputImage.Height));
            }
            pictureBox1.Image = resultImage;
            progressBar1.Invoke((Action)(() => { progressBar1.Value = 0; EnableButtons(true); }));
        }

        public void MoveImageX()
        {
            int move = slider;
            Bitmap inputImage = new Bitmap(pictureBox1.Image);
            Bitmap resultImage = new Bitmap(inputImage.Width, inputImage.Height);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    int tx = x - move;
                    if (tx < 0 || tx >= resultImage.Width)
                    {
                        resultImage.SetPixel(x, y, Color.Black);
                        continue;
                    }

                    Color pixelColor = inputImage.GetPixel(tx, y);
                    resultImage.SetPixel(x, y, pixelColor);
                }
                progressBar1.Invoke((Action)(() => progressBar1.Value = y * progressbarMaxVal / inputImage.Height));
            }
            pictureBox1.Image = resultImage;
            progressBar1.Invoke((Action)(() => { progressBar1.Value = 0; EnableButtons(true); }));
        }

        public void MoveImageY()
        {
            int move = slider;
            Bitmap inputImage = new Bitmap(pictureBox1.Image);
            Bitmap resultImage = new Bitmap(inputImage.Width, inputImage.Height);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    int ty = y - move;
                    if (ty < 0 || ty >= resultImage.Height)
                    {
                        resultImage.SetPixel(x, y, Color.Black);
                        continue;
                    }

                    Color pixelColor = inputImage.GetPixel(x, ty);
                    resultImage.SetPixel(x, y, pixelColor);
                }
                progressBar1.Invoke((Action)(() => progressBar1.Value = y * progressbarMaxVal / inputImage.Height));
            }
            pictureBox1.Image = resultImage;
            progressBar1.Invoke((Action)(() => { progressBar1.Value = 0; EnableButtons(true); }));
        }

        public void RotateImage()
        {
            float μ = (float)Math.PI * slider / 180.0f;
            Bitmap inputImage = new Bitmap(pictureBox1.Image);
            Bitmap resultImage = new Bitmap(inputImage.Width, inputImage.Height);
            int x0 = inputImage.Width / 2;
            int y0 = inputImage.Height / 2;

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    int tx = (int)((x - x0) * Math.Cos(μ) - (y - y0) * Math.Sin(μ) + x0);
                    int ty = (int)((x - x0) * Math.Sin(μ) + (y - y0) * Math.Cos(μ) + y0);

                    if (ty < 0 || ty >= resultImage.Height || tx < 0 || tx >= resultImage.Width)
                    {
                        resultImage.SetPixel(x, y, Color.Black);
                        continue;
                    }
                    Color pixelColor = inputImage.GetPixel(tx, ty);
                    resultImage.SetPixel(x, y, pixelColor);
                }
                progressBar1.Invoke((Action)(() => progressBar1.Value = y * progressbarMaxVal / inputImage.Height));
            }
            pictureBox1.Image = resultImage;
            progressBar1.Invoke((Action)(() => { progressBar1.Value = 0; EnableButtons(true); }));
        }

        public void WaveXImage()
        {
            Bitmap inputImage = new Bitmap(pictureBox1.Image);
            Bitmap resultImage = new Bitmap(inputImage.Width, inputImage.Height);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    int tx = (int)(x + 20 * Math.Sin(2 * Math.PI * y /60));

                    if (tx < 0 || tx >= resultImage.Width)
                    {
                        resultImage.SetPixel(x, y, Color.Black);
                        continue;
                    }
                    Color pixelColor = inputImage.GetPixel(tx, y);
                    resultImage.SetPixel(x, y, pixelColor);
                }
                progressBar1.Invoke((Action)(() => progressBar1.Value = y * progressbarMaxVal / inputImage.Height));
            }
            pictureBox1.Image = resultImage;
            progressBar1.Invoke((Action)(() => { progressBar1.Value = 0; EnableButtons(true); }));
        }

        public void WaveYImage()
        {
            Bitmap inputImage = new Bitmap(pictureBox1.Image);
            Bitmap resultImage = new Bitmap(inputImage.Width, inputImage.Height);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    int ty = (int)(y + 20 * Math.Sin(2 * Math.PI * x /60));

                    if (ty < 0 || ty >= resultImage.Height)
                    {
                        resultImage.SetPixel(x, y, Color.Black);
                        continue;
                    }
                    Color pixelColor = inputImage.GetPixel(x, ty);
                    resultImage.SetPixel(x, y, pixelColor);
                }
                progressBar1.Invoke((Action)(() => progressBar1.Value = y * progressbarMaxVal / inputImage.Height));
            }
            pictureBox1.Image = resultImage;
            progressBar1.Invoke((Action)(() => { progressBar1.Value = 0; EnableButtons(true); }));
        }

        // Фильтр размытие
        public void Blur()
        {
            Filters filter = new BlurFilter();
            filter.processImage((Bitmap)pictureBox1.Image);
        }

        // Фильтр размытие по Гауссу
        public void GaussBlur()
        {
            Filters filter = new GaussianFilter();
            filter.processImage((Bitmap)pictureBox1.Image);
        }

        // Фильтр Собебя
        public void SobelX()
        {
            Filters filter = new SobelOperator(false);
            filter.processImage((Bitmap)pictureBox1.Image);
        }

        // Фильтр Собуля
        public void SobelY()
        {
            Filters filter = new SobelOperator(true);
            filter.processImage((Bitmap)pictureBox1.Image);
        }

        // Фильтр резкости
        public void Sharpness()
        {
            Filters filter = new SharpnessFilter();
            filter.processImage((Bitmap)pictureBox1.Image);
        }
    }
}
