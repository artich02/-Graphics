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
                new ComboItem("Вращение",       () => RotateImage(),         (-180, 180)),
                new ComboItem("Волны X",        () => WaveXImage()),
                new ComboItem("Волны Y",        () => WaveYImage()),

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
        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}
