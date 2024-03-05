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
        private HistogramContainer histogram = new HistogramContainer();

        public static Form1 It;

        class HistogramContainer
        {
            public UInt64[] data = new UInt64[256];
            public bool available = false;
            public (int, int, int) max;
            public (int, int, int) min;
        }

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
            pictureBox1.AllowDrop = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Точечные фильтры
            ComboMenuItem[] spotMenuItems = new ComboMenuItem[]
            {
                new ComboMenuItem("Инверсия",       () => Inverse()),
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
                new ComboMenuItem("Размытие в движении", () => MotionBlur()),
                new ComboMenuItem("Медианный",      () => Median()),
                new ComboMenuItem("Максимум",       () => Maximum()),
            };

            // Комбинируемые фильтры
            ComboMenuItem[] combinedMenuItems = new ComboMenuItem[]
            {
                new ComboMenuItem("Тиснение",       () => Embossing()),
                new ComboMenuItem("Светящиеся края",() => GlowingEdges()),
            };

            // Комбинируемые фильтры
            ComboMenuItem[] colCorrMenuItems = new ComboMenuItem[]
            {
                new ComboMenuItem("Лин. раст-е гистограммы", () => LinearHistogramStretch()),
                new ComboMenuItem("Серый мир",               () => GrayWorld()),
                new ComboMenuItem("Идеальный отражатель",    () => PerfectReflector()),
            };

            // Матморфология
            ComboMenuItem[] mathmorMenuItems = new ComboMenuItem[]
            {
                new ComboMenuItem("Расширение", () => MathmorphDilation()),
                new ComboMenuItem("Сужение",    () => MathmorphErosion()),
                new ComboMenuItem("Закрытие",   () => MathmorphClosing()),
                new ComboMenuItem("Открытие",   () => MathmorphOpening()),
                new ComboMenuItem("Grad",       () => MathmorphGrad()),
                new ComboMenuItem("Top Hat",    () => MathmorphTopHat()),
                new ComboMenuItem("Black Hat",  () => MathmorphBlackHat()),
            };

            EnableButtons(false);
            loadButton.Enabled = true;

            this.точечныеToolStripMenuItem.DropDownItems.AddRange(spotMenuItems);
            this.матричныеToolStripMenuItem1.DropDownItems.AddRange(matrixMenuItems);
            this.комбинированныеToolStripMenuItem.DropDownItems.AddRange(combinedMenuItems);
            this.цветокоррекцияToolStripMenuItem.DropDownItems.AddRange(colCorrMenuItems);
            this.матморфологияToolStripMenuItem.DropDownItems.AddRange(mathmorMenuItems);
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
                else
                    return;
            }
            EnableButtons(true);
            histogram.available = false;
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                // Сохраняем исходное изображение
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "png image|*.png|jpeg image|*.jpg;*.jpeg|bitmap image|*.bmp";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string imagePath = saveFileDialog.FileName;
                        ImageFormat format = ImageFormat.Png;
                        string ext = System.IO.Path.GetExtension(imagePath);
                        switch (ext)
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
            histogram.available = false;
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
            if (pictureBox1.Image == null || currentFilter == null)
                return;
            EnableButtons(false);
            Action handler = currentFilter.Action;
            //handler();
            var task = new Task(() => handler());
            task.Start();
        }

        private void EnableButtons(bool state)
        {
            ToolStripMenuChange();
            loadButton.Enabled = state;
            button5.Enabled = state;
            button7.Enabled = state;
            textBox3.Enabled = state;
            OKbutton.Enabled = state;
            menuStrip1.Enabled = state;
            if (!state)
                thresholdSlider.Enabled = false;
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

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle && pictureBox1.Image != null && OKbutton.Enabled)
            {
                using (Form form = new Form())
                {
                    int width = 256 * 2, height = 256;
                    Bitmap img = new Bitmap(256, height);

                    if (!histogram.available)
                        CalculateHistogram();

                    for (int x = 0; x < 256; x++)
                    {
                        for (int y = (int)histogram.data[x]; y > 0; y--)
                        {
                            img.SetPixel(x, y, Color.FromArgb(x, x, x));
                        }
                    }
                    img.RotateFlip(RotateFlipType.Rotate180FlipX);

                    form.StartPosition = FormStartPosition.CenterScreen;
                    form.Width = width;
                    form.Height = height;
                    form.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    form.Text = "Histogram";
                    form.BackColor = Color.FromArgb(255, 0, 255);

                    form.BackgroundImage = img;
                    form.BackgroundImageLayout = ImageLayout.Stretch;
                    form.ShowDialog();
                }
            }
        }

        private void ChangeStructingElement(object sender, EventArgs e)
        {
            Button pictureBox = sender as Button;
            mathmorohologyStructuringElement = Int32.Parse(pictureBox.Name);
        }

        private void ShowMathmorphologyChoiceWindow()
        {
            Form form = new Form();

            for (int i = 0; i < 3; i++)
            {
                Button button03 = new Button
                {
                    Image = new Bitmap((Bitmap)Properties.Resources.ResourceManager.GetObject(i + ".bmp")),
                    Size = new Size(116, 116),
                    Location = new Point(10 + i * 140, 10),
                    Name = i.ToString(),
                    DialogResult = DialogResult.OK,
                };
                button03.Click += new EventHandler(ChangeStructingElement);
                form.Controls.Add(button03);
            }

            form.BackColor = Color.White;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.Width = 438;
            form.Height = 180;
            form.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            form.Text = "Выберите структурный элемент";

            DialogResult dr = form.ShowDialog();

            if (dr == DialogResult.Cancel)
            {
                form.Close();
            }
            else if (dr == DialogResult.OK)
            {
                form.Close();
            }
        }

        private unsafe void CalculateHistogram()
        {
            histogram.data = new UInt64[256];
            histogram.max = (0, 0, 0); histogram.min = (255, 255, 255);
            Bitmap originalImg = (Bitmap)pictureBox1.Image;
            BitmapData inputData = originalImg.LockBits(new Rectangle(0, 0, originalImg.Width, originalImg.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            int stride = inputData.Stride;
            byte* ptrIn = (byte*)inputData.Scan0;

            UInt64 maxVal = 0;
            for (int x = 0; x < originalImg.Width; x++)
            {
                for (int y = 0; y < originalImg.Height; y++)
                {
                    byte colR = ptrIn[(x * 3) + y * stride + 2];
                    byte colG = ptrIn[(x * 3) + y * stride + 1];
                    byte colB = ptrIn[(x * 3) + y * stride + 0];

                    int mono = (colR + colG + colB) / 3;
                    histogram.data[mono]++;
                    maxVal = Math.Max(histogram.data[mono], maxVal);

                    histogram.min.Item1 = Math.Min(colR, histogram.min.Item1);
                    histogram.min.Item2 = Math.Min(colG, histogram.min.Item2);
                    histogram.min.Item3 = Math.Min(colB, histogram.min.Item3);

                    histogram.max.Item1 = Math.Max(colR, histogram.max.Item1);
                    histogram.max.Item2 = Math.Max(colG, histogram.max.Item2);
                    histogram.max.Item3 = Math.Max(colB, histogram.max.Item3);
                }
            }

            originalImg.UnlockBits(inputData);
            UInt64 multiplier = (UInt64)Math.Ceiling((double)maxVal / 256.0);
            for (int x = 0; x < 256; x++)
                histogram.data[x] /= multiplier;
            histogram.available = true;
        }

        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string imagePath = files[0];
            originalImage = new Bitmap(imagePath);
            pictureBox1.Image = originalImage;
            EnableButtons(true);
            histogram.available = false;
        }

        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
    }
}
