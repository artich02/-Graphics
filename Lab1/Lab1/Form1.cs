using System;
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

        private class ComboItem
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

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var items = new List<ComboItem>
            {
                new ComboItem("Серый",       () => ConvertToGrayScale()),
                new ComboItem("Бинаризация", () => Binarize(), (0, 255)),
                new ComboItem("Яркость",     () => AdjustBrightness(), (-255, 255)),
                new ComboItem("Размытие (М)",() => Blur())
            };

            comboBox1.DisplayMember = "Name"; // will display Name property
            comboBox1.ValueMember = "Action"; // will select Value property
            comboBox1.DataSource = items;
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
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                // Сохраняем исходное изображение
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Images|*.png;*.bmp;*.jpg";
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
                                format = ImageFormat.Jpeg;
                                break;
                            case ".bmp":
                                format = ImageFormat.Bmp;
                                break;
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
        /*
        private void GrayscaleButton_Click(object sender, EventArgs e)
        {
            // Конвертация в оттенки серого
            Bitmap grayImage = ConvertToGrayScale(originalImage);
            pictureBox1.Image = grayImage;
        }
        */
        /*
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
        */
        /*
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
        */

        /// ////////////////////////////////////////////////////////////
        /*
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

        }*/
        /// ////////////////////////////////////////////////////////////

        //private Bitmap _originalImage;

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
                progressBar1.Invoke((Action)(() => progressBar1.Value = y * 100 / inputImage.Height));
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
                progressBar1.Invoke((Action)(() => progressBar1.Value = x * 100 / inputImage.Width));
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
                progressBar1.Invoke((Action)(() => progressBar1.Value = x * 100 / inputImage.Width));
            }
            pictureBox1.Image = adjustedImage;
            progressBar1.Invoke((Action)(() => { progressBar1.Value = 0; EnableButtons(true); }));
        }

        public void Blur()
        {

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

        private void ComboBoxChange(object sender, EventArgs e)
        {
            var item = (ComboItem)comboBox1.SelectedItem;
            if (item.Minmax != null)
            {
                thresholdSlider.Enabled = true;
                thresholdSlider.Minimum = item.Minmax.Value.Item1;
                thresholdSlider.Maximum = item.Minmax.Value.Item2;
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
            EnableButtons(false);
            Action handler = (Action)comboBox1.SelectedValue;
            var task = new Task(() => handler());
            task.Start();
        }

        private void EnableButtons(bool state)
        {
            button1.Enabled = state;
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
