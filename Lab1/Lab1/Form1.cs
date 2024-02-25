using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab1
{
    public partial class Nazvanie : Form
    {

        private Bitmap originalImage;
        private Bitmap grayImage;
        public Nazvanie()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //LoadImageFromFolder("C:\\Users\tar88\\Pictures\\IMG_0095.JPG");
        }

        private void LoadImageFromFolder(string imagePath)
        {
            try
            {
                originalImage = new Bitmap(imagePath);
                pictureBox1.Image = originalImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
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
        }

        private void GrayscaleButton_Click(object sender, EventArgs e)
        {
            // Конвертация в оттенки серого
            grayImage = ConvertToGrayScale(originalImage);
            pictureBox1.Image = grayImage;
        }

        private void BinarizeButton_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                // Бинаризация изображения по порогу
                if (Convert.ToInt32(textBox2.Text) > 255)
                    MessageBox.Show("Введите чило =<255");
                else
                {
                    Bitmap binaryImage = Binarize((Bitmap)pictureBox1.Image, Convert.ToInt32(textBox2.Text));
                    pictureBox1.Image = binaryImage;
                }
            }
        }

        private void BrightnessButton_Click(object sender, EventArgs e)
        {

            if (pictureBox1.Image != null)
            {
                // Изменение яркости изображения на
                if (Convert.ToInt32(textBox1.Text) > 100)
                    MessageBox.Show("Введите чило =<100");
                else
                {
                    Bitmap brightImage = AdjustBrightness((Bitmap)pictureBox1.Image, Convert.ToInt32(textBox1.Text));
                    pictureBox1.Image = brightImage;
                }
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                // Сохраняем исходное изображение
                if (_originalImage == null)
                {
                    _originalImage = (Bitmap)pictureBox1.Image.Clone();
                }

                // Возвращаем исходное изображение
                pictureBox1.Image = _originalImage;
            }
        }

        /// ////////////////////////////////////////////////////////////

        private void button6_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                // Матричное размытие
                if (Convert.ToInt32(textBox1.Text) > 100)
                    MessageBox.Show("Введите чило =<100");
                else
                {
                    Bitmap brightImage = AdjustBrightness((Bitmap)pictureBox1.Image, Convert.ToInt32(textBox1.Text));
                    pictureBox1.Image = brightImage;
                }
            }

        }
        /// ////////////////////////////////////////////////////////////

        private Bitmap _originalImage;

        // Функция для преобразования изображения в оттенки серого
        private Bitmap ConvertToGrayScale(Bitmap inputImage)
        {
            Bitmap grayBitmap = new Bitmap(inputImage.Width, inputImage.Height);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    Color originalColor = inputImage.GetPixel(x, y);
                    byte grayValue = (byte)(0.3 * originalColor.R + 0.59 * originalColor.G + 0.11 * originalColor.B);
                    grayBitmap.SetPixel(x, y, Color.FromArgb(grayValue, grayValue, grayValue));
                }
            }

            return grayBitmap;
        }

        public static Bitmap Binarize(Bitmap inputImage, int threshold)
        {

            Bitmap binaryImage = new Bitmap(inputImage.Width, inputImage.Height);

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
            }

            return binaryImage;

        }

        public static Bitmap AdjustBrightness(Bitmap inputImage, int brightnessLevel)
        {
            Bitmap adjustedImage = new Bitmap(inputImage.Width, inputImage.Height);

            for (int x = 0; x < inputImage.Width; ++x)
            {
                for (int y = 0; y < inputImage.Height; ++y)
                {
                    Color pixelColor = inputImage.GetPixel(x, y);

                    // Изменение каждого компонента RGB
                    int red = (int)(pixelColor.R + (brightnessLevel * 2.55));
                    int green = (int)(pixelColor.G + (brightnessLevel * 2.55));
                    int blue = (int)(pixelColor.B + (brightnessLevel * 2.55));

                    // Ограничение значений RGB в диапазоне 0-255
                    red = Math.Min(255, Math.Max(0, red));
                    green = Math.Min(255, Math.Max(0, green));
                    blue = Math.Min(255, Math.Max(0, blue));

                    // Замена цвета пикселя
                    adjustedImage.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }

            return adjustedImage;
        }

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

                for (int l = -radiusY; l < -radiusY; l++)
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
                int sizeX = 3;
                int sizeY = 3;
                kernel = new float[sizeX, sizeY];
                for (int i = 0; i < sizeX; i++)
                {
                    for (int j = 0; j < sizeY; j++)
                        kernel[i, j] = 1.0f / (float)(sizeX * sizeY);
                }
            }
        }


    }
}
