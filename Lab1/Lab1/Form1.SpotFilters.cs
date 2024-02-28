using System.Drawing;
using System.Windows.Forms;

namespace Lab1
{
    public partial class Form1 : Form
    {
        public void ConvertToGrayScale()
        {
            Filters filter = new ToGrayScale();
            filter.processImage((Bitmap)pictureBox1.Image);
        }
        public void Binarize()
        {
            Filters filter = new Binarization();
            filter.processImage((Bitmap)pictureBox1.Image);
        }
        public void AdjustBrightness()
        {
            Filters filter = new Brightness();
            filter.processImage((Bitmap)pictureBox1.Image);
        }
        public void ApplySepiaFilter()
        {
            Filters filter = new Sepia();
            filter.processImage((Bitmap)pictureBox1.Image);
        }
        public void Glass()
        {
            Filters filter = new GlassFilter();
            filter.processImage((Bitmap)pictureBox1.Image);
        }
        public void MoveImageX()
        {
            Filters filter = new MoveImageFilterX();
            filter.processImage((Bitmap)pictureBox1.Image);
        }
        public void MoveImageY()
        {
            Filters filter = new MoveImageFilterY();
            filter.processImage((Bitmap)pictureBox1.Image);
        }
        public void RotateImage()
        {
            Filters filter = new RotateImageFilter();
            filter.processImage((Bitmap)pictureBox1.Image);
        }
        public void WaveXImage()
        {
            Filters filter = new WaveXImageFilter();
            filter.processImage((Bitmap)pictureBox1.Image);
        }
        public void WaveYImage()
        {
            Filters filter = new WaveYImageFilter();
            filter.processImage((Bitmap)pictureBox1.Image);
        }
    }
}
