using System;
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

        class ComboMenuItem : ToolStripMenuItem
        {
            public string Naming { get; private set; }
            public Action Action { get; private set; }
            public (int, int)? Minmax { get; private set; }

            public ComboMenuItem(string name, Action action)
            {
                Action = action; Name = name; Minmax = null;
                base.Text = Name;
            }
            public ComboMenuItem(string name, Action action, (int, int) minmax)
            {
                Action = action; Name = name; Minmax = minmax;
                base.Text = Name;
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
            // Точечные фильтры
            ComboMenuItem[] spotMenuItems = new ComboMenuItem[]
            {
                new ComboMenuItem("Серый",          () => ConvertToGrayScale()),
                new ComboMenuItem("Бинаризация",    () => Binarize(),            (   0, 255)),
                new ComboMenuItem("Яркость",        () => AdjustBrightness(),    (-255, 255)),
                new ComboMenuItem("Сепия",          () => ApplySepiaFilter()),
                new ComboMenuItem("Стекло",         () => Glass()),
                new ComboMenuItem("Перенос X",      () => MoveImageX(),          (0, 0)),
                new ComboMenuItem("Перенос Y",      () => MoveImageY(),          (0, 0)),
                new ComboMenuItem("Вращение",       () => RotateImage(),         (-180, 180)),
                new ComboMenuItem("Волны X",        () => WaveXImage()),
                new ComboMenuItem("Волны Y",        () => WaveYImage()),
            };

            // Матричные фильтры
            ComboMenuItem[] matrixMenuItems = new ComboMenuItem[]
            {
                new ComboMenuItem("Размытие",       () => Blur()),
                new ComboMenuItem("Размытие Гаусс", () => GaussBlur()),
                new ComboMenuItem("Собель X",       () => SobelX()),
                new ComboMenuItem("Собель Y",       () => SobelY()),
                new ComboMenuItem("Резкость",       () => Sharpness()),
            };

            EnableButtons(false);
            loadButton.Enabled = true;

            this.точечныеToolStripMenuItem.DropDownItems.AddRange(spotMenuItems);
            this.матричныеToolStripMenuItem1.DropDownItems.AddRange(matrixMenuItems);
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

        private void SliderChange(object sender, EventArgs e)
        {
            textBox3.Text = thresholdSlider.Value.ToString();
            slider = thresholdSlider.Value;
        }


        private delegate void Action();
        ComboMenuItem currentFilter;

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
                return;
            EnableButtons(false);
            Action handler = currentFilter.Action;
            //handler();
            var task = new Task(() => handler());
            task.Start();
        }

        private void EnableButtons(bool state)
        {
            loadButton.Enabled = state;
            button5.Enabled = state;
            button7.Enabled = state;
            textBox3.Enabled = state;
            thresholdSlider.Enabled = state;
            OKbutton.Enabled = state;
            menuStrip1.Enabled = state;
            ToolStripMenuChange();
        }

        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        private void ToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            currentFilter = (ComboMenuItem)e.ClickedItem;
            this.фильтрыToolStripMenuItem.Text = e.ClickedItem.ToString();
            ToolStripMenuChange();
        }

        private void ToolStripMenuChange()
        {
            if (currentFilter != null && currentFilter.Minmax != null && pictureBox1.Image != null)
            {
                thresholdSlider.Enabled = true;
                if (currentFilter.Name == "Перенос X")
                {
                    thresholdSlider.Minimum = -pictureBox1.Image.Width;
                    thresholdSlider.Maximum = pictureBox1.Image.Width;
                }
                else if (currentFilter.Name == "Перенос Y")
                {
                    thresholdSlider.Minimum = -pictureBox1.Image.Height;
                    thresholdSlider.Maximum = pictureBox1.Image.Height;
                }
                else
                {
                    thresholdSlider.Minimum = currentFilter.Minmax.Value.Item1;
                    thresholdSlider.Maximum = currentFilter.Minmax.Value.Item2;
                }
                SliderChange(null, null);
            }
            else
            {
                thresholdSlider.Enabled = false;
            }
        }
    }
}
