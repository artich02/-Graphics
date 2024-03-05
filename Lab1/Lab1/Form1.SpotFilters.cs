using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Lab1
{
    public partial class Form1 : Form
    {
        public void ConvertToGrayScale()
        {
            Filters filter = new GrayScaleFilter();
            filter.ProcessImage(pictureBox1.Image);
        }
        public void Binarize()
        {
            Filters filter = new BinarizationFilter();
            filter.ProcessImage(pictureBox1.Image);
        }
        public void AdjustBrightness()
        {
            Filters filter = new BrightnessFilter();
            filter.ProcessImage(pictureBox1.Image);
        }
        public void ApplySepiaFilter()
        {
            Filters filter = new SepiaFilter();
            filter.ProcessImage(pictureBox1.Image);
        }
        public void Glass()
        {
            Filters filter = new GlassFilter();
            filter.ProcessImage(pictureBox1.Image);
        }
        public void MoveImageX()
        {
            Filters filter = new MoveImageXFilter();
            filter.ProcessImage(pictureBox1.Image);
        }
        public void MoveImageY()
        {
            Filters filter = new MoveImageYFilter();
            filter.ProcessImage(pictureBox1.Image);
        }
        public void RotateImage()
        {
            Filters filter = new RotateImageFilter();
            filter.ProcessImage(pictureBox1.Image);
        }
        public void WaveXImage()
        {
            Filters filter = new WaveXImageFilter();
            filter.ProcessImage(pictureBox1.Image);
        }
        public void WaveYImage()
        {
            Filters filter = new WaveYImageFilter();
            filter.ProcessImage(pictureBox1.Image);
        }
        public void Inverse()
        {
            Filters filter = new InverseFilter();
            filter.ProcessImage(pictureBox1.Image);
        }

        public void LinearHistogramStretch()
        {
            if (!It.histogram.available)
                CalculateHistogram();
            Filters filter = new LinearHistogramStretchFilter();
            filter.ProcessImage(pictureBox1.Image);
        }

        public void GrayWorld()
        {
            Filters filter = new GrayWorldFilter((Bitmap)pictureBox1.Image);
            filter.ProcessImage(pictureBox1.Image);
        }

        public void PerfectReflector()
        {
            if (!It.histogram.available)
                CalculateHistogram();
            Filters filter = new PerfectReflectorFilter();
            filter.ProcessImage(pictureBox1.Image);
        }
    }
}
