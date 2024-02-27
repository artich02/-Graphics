﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab1
{
    public partial class Form1 : Form
    {
        private Bitmap originalImage;
        private int slider = 125;
        private const int progressbarMaxVal = 1000;

        public static Form1 It;

        private struct ComboItem
        {
            public string Name { get; private set; }
            public Action Action { get; private set; }
            public (int, int)? Minmax { get; private set; }

            public ComboItem(string name, Action action)
            {
                Action = action; Name = name; Minmax = null;
            }

            public ComboItem(string name, Action action, (int, int) minmax)
            {
                Action = action; Name = name; Minmax = minmax;
            }
        }

        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public Form1()
        {
            InitializeComponent();
            It = this;
            progressBar1.Maximum = progressbarMaxVal;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var items = new List<ComboItem>
            {
                new ComboItem("Серый",          () => ConvertToGrayScale()),
                new ComboItem("Бинаризация",    () => Binarize(),            (   0, 255)),
                new ComboItem("Яркость",        () => AdjustBrightness(),    (-255, 255)),
                new ComboItem("Сепия",          () => ApplySepiaFilter()),
                new ComboItem("Стекло",         () => Glass()),
                new ComboItem("Размытие",       () => Blur()),
                new ComboItem("Размытие Гаусс", () => GaussBlur()),
                new ComboItem("Собель X",       () => SobelX()),
                new ComboItem("Собель Y",       () => SobelY()),
                new ComboItem("Резкость",       () => Sharpness()),
                new ComboItem("Перенос X",      () => MoveImageX(),          (0, 0)),
                new ComboItem("Перенос Y",      () => MoveImageY(),          (0, 0)),
                new ComboItem("Вращение",       () => RotateImage(),         (-180, 180))

            };

            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "Action";
            comboBox1.DataSource = items;

            EnableButtons(false);
            loadButton.Enabled = true;
        }

        private void LoadImageButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string imagePath = openFileDialog.FileName;
                    originalImage = new Bitmap(imagePath);
                    pictureBox1.Image = originalImage;
                }
            }
            EnableButtons(true);
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                // Сохраняем исходное изображение
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Images|*.png;*.bmp;*.jpg;*.jpeg;";
                    if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        string imagePath = saveFileDialog.FileName;
                        ImageFormat format = ImageFormat.Png;
                        switch (imagePath)
                        {
                            case ".png":
                                format = ImageFormat.Png;
                                break;
                            case ".jpg":
                            case ".jpeg":
                                format = ImageFormat.Jpeg;
                                break;
                            case ".bmp":
                                format = ImageFormat.Bmp;
                                break;
                            default:
                                return;
                        }
                        pictureBox1.Image.Save(saveFileDialog.FileName, format);
                    }
                }
            }
        }
        private void RestoreButton_Click(object sender, EventArgs e)
        {// Возвращаем исходное изображение
            pictureBox1.Image = originalImage;
        }

        /// ////////////////////////////////////////////////////////////

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
                progressBar1.Invoke((Action)(() => progressBar1.Value = y * progressbarMaxVal / inputImage.Width));
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
                progressBar1.Invoke((Action)(() => progressBar1.Value = y * progressbarMaxVal / inputImage.Width));
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
                progressBar1.Invoke((Action)(() => progressBar1.Value = y * progressbarMaxVal / inputImage.Width));
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
                progressBar1.Invoke((Action)(() => progressBar1.Value = y * progressbarMaxVal / inputImage.Width));
            }
            pictureBox1.Image = resultImage;
            progressBar1.Invoke((Action)(() => { progressBar1.Value = 0; EnableButtons(true); }));
        }

        public void RotateImage()
        {
            float μ = (float)Math.PI * slider / 180.0f;
            Bitmap inputImage = new Bitmap(pictureBox1.Image);
            Bitmap resultImage = new Bitmap(inputImage.Width, inputImage.Height);
            int x0 = inputImage.Width  / 2;
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
                progressBar1.Invoke((Action)(() => progressBar1.Value = y * progressbarMaxVal / inputImage.Width));
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

        /// ////////////////////////////////////////////////////////////

        abstract class Filters
        {
            protected abstract Color calculateNewPixelColor(Bitmap sourceImage, int x, int y);

            public Bitmap processImage(Bitmap sourceImage)
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
                return resultImg;
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

        class MatrixFilter : Filters
        {
            protected float[,] kernel = null;
            protected MatrixFilter() { }
            public MatrixFilter(float[,] kernel)
            {
                this.kernel = kernel;
            }

            protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
            {
                int radiusX = kernel.GetLength(0) / 2;
                int radiusY = kernel.GetLength(1) / 2;

                float resultR = 0;
                float resultG = 0;
                float resultB = 0;

                for (int l = -radiusY; l <= radiusY; l++)
                {
                    for (int k = -radiusX; k <= radiusX; k++)
                    {
                        int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                        int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                        Color neighborColor = sourceImage.GetPixel(idX, idY);
                        resultR += neighborColor.R * kernel[k + radiusX, l + radiusY];
                        resultG += neighborColor.G * kernel[k + radiusX, l + radiusY];
                        resultB += neighborColor.B * kernel[k + radiusX, l + radiusY];
                    }
                }
                return Color.FromArgb(
                    Clamp((int)resultR, 0, 255),
                    Clamp((int)resultG, 0, 255),
                    Clamp((int)resultB, 0, 255));
            }
        }

        class BlurFilter : MatrixFilter
        {
            public BlurFilter()
            {
                int sizeX = 7;
                int sizeY = 7;
                kernel = new float[sizeX, sizeY];
                for (int i = 0; i < sizeX; i++)
                {
                    for (int j = 0; j < sizeY; j++)
                        kernel[i, j] = 1.0f / (float)(sizeX * sizeY);
                }
            }
        }

        class GaussianFilter : MatrixFilter
        {
            public GaussianFilter()
            {
                CreateGaussanKernel(3, 2);
            }

            public void CreateGaussanKernel(int radius, float sigma)
            {
                int size = 2 * radius + 1;
                kernel = new float[size, size];

                float norm = 0;

                for (int i = -radius; i <= radius; i++)
                {
                    for (int j = -radius; j <= radius; j++)
                    {
                        kernel[i + radius, j + radius] = (float)(Math.Exp(-(i * i + j * j) / (2 * sigma * sigma)));
                        norm += kernel[i + radius, j + radius];
                    }
                }

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                        kernel[i, j] /= norm;
                }
            }
        }

        class SobelOperator : MatrixFilter
        {
            public SobelOperator(bool Yaxis)
            {
                CreateSobelKernel(Yaxis);
            }

            public void CreateSobelKernel(bool Yaxis)
            {
                int a = 2, b = 1;
                if (Yaxis)
                    kernel = new float[,]{{-b, -a, -b},
                                          { 0,  0,  0},
                                          { b,  a,  b}};
                else
                    kernel = new float[,]{{-b,  0,  b},
                                          {-a,  0,  a},
                                          {-b,  0,  b}};
            }
        }

        class SharpnessFilter : MatrixFilter
        {
            public SharpnessFilter()
            {
                CreateSharpKernel();
            }

            public void CreateSharpKernel()
            {
                kernel = new float[,]{{ 0, -1,  0},
                                      {-1,  5, -1},
                                      { 0, -1,  0}};
            }
        }

        /// ////////////////////////////////////////////////////////////

        private void ComboBoxChange(object sender, EventArgs e)
        {
            var item = (ComboItem)comboBox1.SelectedItem;
            if (item.Minmax != null && pictureBox1.Image != null)
            {
                thresholdSlider.Enabled = true;
                if (item.Name == "Перенос X" )
                {
                    thresholdSlider.Minimum = -pictureBox1.Image.Width;
                    thresholdSlider.Maximum = pictureBox1.Image.Width;
                }
                else if (item.Name == "Перенос Y")
                {
                    thresholdSlider.Minimum = -pictureBox1.Image.Height;
                    thresholdSlider.Maximum = pictureBox1.Image.Height;
                }
                else
                {
                    thresholdSlider.Minimum = item.Minmax.Value.Item1;
                    thresholdSlider.Maximum = item.Minmax.Value.Item2;
                }
                SliderChange(sender, e);
            }
            else
            {
                thresholdSlider.Enabled = false;
            }
        }

        private void SliderChange(object sender, EventArgs e)
        {
            textBox3.Text = thresholdSlider.Value.ToString();
            slider = thresholdSlider.Value;
        }


        private delegate void Action();

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
                return;
            EnableButtons(false);
            Action handler = (Action)comboBox1.SelectedValue;
            handler();
            //var task = new Task(() => handler());
            //task.Start();
        }

        private void EnableButtons(bool state)
        {
            loadButton.Enabled = state;
            button5.Enabled = state;
            button7.Enabled = state;
            comboBox1.Enabled = state;
            textBox3.Enabled = state;
            thresholdSlider.Enabled = state;
            OKbutton.Enabled = state;
            ComboBoxChange(null, null);
        }

    }
}
