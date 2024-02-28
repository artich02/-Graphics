using System.Drawing;
using System.Windows.Forms;

namespace Lab1
{
    public partial class Form1 : Form
    {
        public void ConvertToGrayScale()
        {
            Filters filter = new ToGrayScale();
            filter.ProcessImage((Bitmap)pictureBox1.Image);
        }
        public void Binarize()
        {
            Filters filter = new BinarizationFilter();
            filter.ProcessImage((Bitmap)pictureBox1.Image);
        }
        public void AdjustBrightness()
        {
            Filters filter = new BrightnessFilter();
            filter.ProcessImage((Bitmap)pictureBox1.Image);
        }
        public void ApplySepiaFilter()
        {
            Filters filter = new SepiaFilter();
            filter.ProcessImage((Bitmap)pictureBox1.Image);
        }
        public void Glass()
        {
            Filters filter = new GlassFilter();
            filter.ProcessImage((Bitmap)pictureBox1.Image);
        }
        public void MoveImageX()
        {
            Filters filter = new MoveImageXFilter();
            filter.ProcessImage((Bitmap)pictureBox1.Image);
        }
        public void MoveImageY()
        {
            Filters filter = new MoveImageYFilter();
            filter.ProcessImage((Bitmap)pictureBox1.Image);
        }
        public void RotateImage()
        {
            Filters filter = new RotateImageFilter();
            filter.ProcessImage((Bitmap)pictureBox1.Image);
        }
        public void WaveXImage()
        {
            Filters filter = new WaveXImageFilter();
            filter.ProcessImage((Bitmap)pictureBox1.Image);
        }
        public void WaveYImage()
        {
            Filters filter = new WaveYImageFilter();
            filter.ProcessImage((Bitmap)pictureBox1.Image);
        }
    }
}
