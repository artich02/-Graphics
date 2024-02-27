using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab1
{
    public partial class Form1 : Form
    {
        // Функция для преобразования изображения в оттенки серого
        private void ConvertToGrayScale()
        {
            Bitmap inputImage = new Bitmap(pictureBox1.Image);
            Bitmap grayBitmap = new Bitmap(inputImage.Width, inputImage.Height);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    Color originalColor = inputImage.GetPixel(x, y);
                    byte grayValue = (byte)(0.2989f * originalColor.R
                                          + 0.5870f * originalColor.G
                                          + 0.1141f * originalColor.B);
                    grayBitmap.SetPixel(x, y, Color.FromArgb(grayValue, grayValue, grayValue));
                }
                progressBar1.Invoke((Action)(() => progressBar1.Value = y * progressbarMaxVal / inputImage.Height));
            }
            pictureBox1.Image = grayBitmap;
            progressBar1.Invoke((Action)(() => { progressBar1.Value = 0; EnableButtons(true); }));
        }

        public void Binarize()
        {
            Bitmap inputImage = new Bitmap(pictureBox1.Image);
            Bitmap binaryImage = new Bitmap(inputImage.Width, inputImage.Height);
            int threshold = slider;

            for (int x = 0; x < inputImage.Width; ++x)
            {
                for (int y = 0; y < inputImage.Height; ++y)
                {
                    Color pixelColor = inputImage.GetPixel(x, y);

                    // Среднее значение RGB
                    int gray = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;

                    // Бинаризация порогом
                    int binaryValue = gray > threshold ? 255 : 0;

                    // Замена цвета пикселя
                    binaryImage.SetPixel(x, y, Color.FromArgb(binaryValue, binaryValue, binaryValue));
                }
                progressBar1.Invoke((Action)(() => progressBar1.Value = x * progressbarMaxVal / inputImage.Width));
            }
            pictureBox1.Image = binaryImage;
            progressBar1.Invoke((Action)(() => { progressBar1.Value = 0; EnableButtons(true); }));
        }

        public void AdjustBrightness()
        {
            Bitmap inputImage = new Bitmap(pictureBox1.Image);
            Bitmap adjustedImage = new Bitmap(inputImage.Width, inputImage.Height);
            int brightnessLevel = slider;

            for (int x = 0; x < inputImage.Width; ++x)
            {
                for (int y = 0; y < inputImage.Height; ++y)
                {
                    Color pixelColor = inputImage.GetPixel(x, y);

                    // Изменение каждого компонента RGB
                    int red = (int)(pixelColor.R + brightnessLevel);
                    int green = (int)(pixelColor.G + brightnessLevel);
                    int blue = (int)(pixelColor.B + brightnessLevel);

                    // Ограничение значений RGB в диапазоне 0-255
                    red = Math.Min(255, Math.Max(0, red));
                    green = Math.Min(255, Math.Max(0, green));
                    blue = Math.Min(255, Math.Max(0, blue));

                    // Замена цвета пикселя
                    adjustedImage.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
                progressBar1.Invoke((Action)(() => progressBar1.Value = x * progressbarMaxVal / inputImage.Width));
            }
            pictureBox1.Image = adjustedImage;
            progressBar1.Invoke((Action)(() => { progressBar1.Value = 0; EnableButtons(true); }));
        }

        // Фильтр Сепия
        public void ApplySepiaFilter()
        {
            Bitmap inputImage = new Bitmap(pictureBox1.Image);
            Bitmap SepiaImage = new Bitmap(inputImage.Width, inputImage.Height);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    Color pixelColor = inputImage.GetPixel(x, y);
                    int r = pixelColor.R;
                    int g = pixelColor.G;
                    int b = pixelColor.B;

                    int newR = (int)(0.393 * r + 0.769 * g + 0.189 * b);
                    int newG = (int)(0.349 * r + 0.686 * g + 0.168 * b);
                    int newB = (int)(0.272 * r + 0.534 * g + 0.131 * b);

                    newR = Math.Min(255, newR);
                    newG = Math.Min(255, newG);
                    newB = Math.Min(255, newB);

                    Color newColor = Color.FromArgb(newR, newG, newB);
                    SepiaImage.SetPixel(x, y, newColor);
                }
                progressBar1.Invoke((Action)(() => progressBar1.Value = y * progressbarMaxVal / inputImage.Height));
            }
            pictureBox1.Image = SepiaImage;
            progressBar1.Invoke((Action)(() => { progressBar1.Value = 0; EnableButtons(true); }));
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
            Bitmap inputImage = new Bitmap(pictureBox1.Image);
            Filters filter = new BlurFilter();
            pictureBox1.Image = filter.processImage(inputImage);
            progressBar1.Invoke((Action)(() => { progressBar1.Value = 0; EnableButtons(true); }));
        }

        // Фильтр размытие по Гауссу
        public void GaussBlur()
        {
            Bitmap inputImage = new Bitmap(pictureBox1.Image);
            Filters filter = new GaussianFilter();
            pictureBox1.Image = filter.processImage(inputImage);
            progressBar1.Invoke((Action)(() => { progressBar1.Value = 0; EnableButtons(true); }));
        }

        // Фильтр Собебя
        public void SobelX()
        {
            Bitmap inputImage = new Bitmap(pictureBox1.Image);
            Filters filter = new SobelOperator(false);
            pictureBox1.Image = filter.processImage(inputImage);
            progressBar1.Invoke((Action)(() => { progressBar1.Value = 0; EnableButtons(true); }));
        }

        // Фильтр Собуля
        public void SobelY()
        {
            Bitmap inputImage = new Bitmap(pictureBox1.Image);
            Filters filter = new SobelOperator(true);
            pictureBox1.Image = filter.processImage(inputImage);
            progressBar1.Invoke((Action)(() => { progressBar1.Value = 0; EnableButtons(true); }));
        }

        // Фильтр резкости
        public void Sharpness()
        {
            Bitmap inputImage = new Bitmap(pictureBox1.Image);
            Filters filter = new SharpnessFilter();
            pictureBox1.Image = filter.processImage(inputImage);
            progressBar1.Invoke((Action)(() => { progressBar1.Value = 0; EnableButtons(true); }));
        }
    }
}
