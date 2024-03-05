using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Lab1
{
    public partial class Form1 : Form
    {
        int mathmorohologyStructuringElement = 0;
        readonly List<float[,]> mathmorohologyKernels = new List<float[,]>
        {
            new float[,]{
                 { 0, 1, 0},
                 { 1, 1, 1},
                 { 0, 1, 0}},

            new float[,]{
                 { 1, 1, 1},
                 { 1, 1, 1},
                 { 1, 1, 1}},

            new float[,]{
                 { 0, 1, 1, 1, 0},
                 { 1, 1, 1, 1, 1},
                 { 1, 1, 1, 1, 1},
                 { 1, 1, 1, 1, 1},
                 { 0, 1, 1, 1, 0}}
        };

        class DilationFilter : MatrixFilter // A(+)B
        {
            public DilationFilter()
            {
                kernel = It.mathmorohologyKernels[It.mathmorohologyStructuringElement];
            }

            protected unsafe override void CalculateNewPixelColor(byte* ptrIn, byte* ptrOut, ref int stride, ref int x, ref int y)
            {
                int radiusX = kernel.GetLength(0) / 2;
                int radiusY = kernel.GetLength(1) / 2;

                byte maxR = 0, maxG = 0, maxB = 0;

                for (int l = -radiusY; l <= radiusY; l++)
                {
                    for (int k = -radiusX; k <= radiusX; k++)
                    {
                        int idX = Clamp(x + k, 0, imgWidth - 1);
                        int idY = Clamp(y + l, 0, imgHeight - 1);

                        if (kernel[k + radiusX, l + radiusY] == 1.0f && ptrIn[idX * 3 + idY * stride + 2] > maxR)
                            maxR = ptrIn[idX * 3 + idY * stride + 2];

                        if (kernel[k + radiusX, l + radiusY] == 1.0f && ptrIn[idX * 3 + idY * stride + 1] > maxG)
                            maxG = ptrIn[idX * 3 + idY * stride + 1];

                        if (kernel[k + radiusX, l + radiusY] == 1.0f && ptrIn[idX * 3 + idY * stride + 0] > maxB)
                            maxB = ptrIn[idX * 3 + idY * stride + 0];
                    }
                }

                ptrOut[x * 3 + y * stride + 2] = maxR;
                ptrOut[x * 3 + y * stride + 1] = maxG;
                ptrOut[x * 3 + y * stride] = maxB;
            }
        }
        public void MathmorphDilation()
        {
            ShowMathmorphologyChoiceWindow();
            Filters filter = new DilationFilter();
            filter.ProcessImage(pictureBox1.Image);
        }

        class ErosionFilter : MatrixFilter // A(-)B
        {
            public ErosionFilter()
            {
                kernel = It.mathmorohologyKernels[It.mathmorohologyStructuringElement];
            }

            protected unsafe override void CalculateNewPixelColor(byte* ptrIn, byte* ptrOut, ref int stride, ref int x, ref int y)
            {
                int radiusX = kernel.GetLength(0) / 2;
                int radiusY = kernel.GetLength(1) / 2;

                byte minR = byte.MaxValue, minG = byte.MaxValue, minB = byte.MaxValue;

                for (int l = -radiusY; l <= radiusY; l++)
                {
                    for (int k = -radiusX; k <= radiusX; k++)
                    {
                        int idX = Clamp(x + k, 0, imgWidth - 1);
                        int idY = Clamp(y + l, 0, imgHeight - 1);

                        if (kernel[k + radiusX, l + radiusY] == 1.0f && ptrIn[idX * 3 + idY * stride + 2] < minR)
                            minR = ptrIn[idX * 3 + idY * stride + 2];

                        if (kernel[k + radiusX, l + radiusY] == 1.0f && ptrIn[idX * 3 + idY * stride + 1] < minG)
                            minG = ptrIn[idX * 3 + idY * stride + 1];

                        if (kernel[k + radiusX, l + radiusY] == 1.0f && ptrIn[idX * 3 + idY * stride + 0] < minB)
                            minB = ptrIn[idX * 3 + idY * stride + 0];
                    }
                }

                ptrOut[x * 3 + y * stride + 2] = minR;
                ptrOut[x * 3 + y * stride + 1] = minG;
                ptrOut[x * 3 + y * stride] = minB;
            }
        }
        public void MathmorphErosion()
        {
            ShowMathmorphologyChoiceWindow();
            Filters filter = new ErosionFilter();
            filter.ProcessImage(pictureBox1.Image);
        }

        public void MathmorphOpening() // (A (-) B) (+) B
        {
            ShowMathmorphologyChoiceWindow();
            Filters filter = new ErosionFilter();
            var img = filter.PartialProcessImage(pictureBox1.Image, progressbarMaxVal / 2);
            filter = new DilationFilter();
            img = filter.PartialProcessImage(img, progressbarMaxVal);
            It.pictureBox1.Image = img;
            It.progressBar1.Invoke((Action)(() => { It.progressBar1.Value = 0; It.EnableButtons(true); }));
        }
        public void MathmorphClosing() // (A (+) B) (-) B
        {
            ShowMathmorphologyChoiceWindow();
            Filters filter = new DilationFilter();
            var img = filter.PartialProcessImage(pictureBox1.Image, progressbarMaxVal / 2);
            filter = new ErosionFilter();
            img = filter.PartialProcessImage(img, progressbarMaxVal);
            It.pictureBox1.Image = img;
            It.progressBar1.Invoke((Action)(() => { It.progressBar1.Value = 0; It.EnableButtons(true); }));
        }

        class SubtractFilter : Filters
        {
            Bitmap imgL, imgR;

            public SubtractFilter(Bitmap img1, Bitmap img2)
            {
                imgL = img1;
                imgR = img2;
            }

            protected unsafe override void CalculateNewPixelColor(byte* ptrIn, byte* ptrOut, ref int stride, ref int x, ref int y)
            {
                Color L = imgL.GetPixel(x, y);
                Color R = imgR.GetPixel(x, y);

                ptrOut[(x * 3) + y * stride + 2] = (byte)Clamp(L.R - R.R, 0, 255);//(byte)Clamp(L.R - R.R, 0, 255);
                ptrOut[(x * 3) + y * stride + 1] = (byte)Clamp(L.G - R.G, 0, 255);
                ptrOut[(x * 3) + y * stride] = (byte)Clamp(L.B - R.B, 0, 255);
            }
        }

        public void MathmorphGrad() // (A(+)B) - (A(-)B)
        {
            ShowMathmorphologyChoiceWindow();
            Filters filter = new DilationFilter();
            var imgL = filter.PartialProcessImage(pictureBox1.Image, progressbarMaxVal / 3);
            filter = new ErosionFilter();
            var imgR = filter.PartialProcessImage(pictureBox1.Image, progressbarMaxVal / 3 * 2);
            filter = new SubtractFilter(imgL, imgR);
            var img = filter.PartialProcessImage(pictureBox1.Image, progressbarMaxVal);
            It.pictureBox1.Image = img;
            It.progressBar1.Invoke((Action)(() => { It.progressBar1.Value = 0; It.EnableButtons(true); }));
        }

        public void MathmorphTopHat() // A-open(A)
        {
            ShowMathmorphologyChoiceWindow();
            Bitmap imgA = (Bitmap)It.pictureBox1.Image;

            Filters filter = new ErosionFilter();
            var openImg = filter.PartialProcessImage(pictureBox1.Image, progressbarMaxVal / 3);
            filter = new DilationFilter();
            openImg = filter.PartialProcessImage(openImg, progressbarMaxVal / 3*2);

            filter = new SubtractFilter(imgA, openImg);
            var img = filter.PartialProcessImage(pictureBox1.Image, progressbarMaxVal);
            It.pictureBox1.Image = img;
            It.progressBar1.Invoke((Action)(() => { It.progressBar1.Value = 0; It.EnableButtons(true); }));
        }

        public void MathmorphBlackHat() // close(A)-A
        {
            ShowMathmorphologyChoiceWindow();
            Bitmap imgA = (Bitmap)It.pictureBox1.Image;

            Filters filter = new DilationFilter();
            var closeImg = filter.PartialProcessImage(pictureBox1.Image, progressbarMaxVal / 3);
            filter = new ErosionFilter();
            closeImg = filter.PartialProcessImage(closeImg, progressbarMaxVal / 3 *2);

            filter = new SubtractFilter(closeImg, imgA);
            var img = filter.PartialProcessImage(pictureBox1.Image, progressbarMaxVal);
            It.pictureBox1.Image = img;
            It.progressBar1.Invoke((Action)(() => { It.progressBar1.Value = 0; It.EnableButtons(true); }));
        }
        /*
        /// <summary>
        /// .....................................................................
        /// </summary>

        public void MathmorphTopHat()
        {
            // Use existing function for opening operation
            Filters filter = new DilationFilter();
            Bitmap openingResult = filter.ProcessImage(pictureBox1.Image);

            // Calculate Top Hat
            Bitmap topHat = new Bitmap(imgWidth, imgHeight);
            for (int x = 0; x < Image.Width; x++)
            {
                for (int y = 0; y < Image.Height; y++)
                {
                    Color originalPixel = Image.GetPixel(x, y);
                    Color openingPixel = openingResult.GetPixel(x, y);
                    int r = Math.Max(0, originalPixel.R - openingPixel.R);
                    int g = Math.Max(0, originalPixel.G - openingPixel.G);
                    int b = Math.Max(0, originalPixel.B - openingPixel.B);
                    topHat.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            // Update UI (similar to existing functions)
            pictureBox1.Image = topHat;
            progressBar1.Invoke((Action)(() => { progressBar1.Value = 0; EnableButtons(true); }));
        }
        
        public void MathmorphBlackHat()
        {
            // Create structuring element (similar to Top Hat)
            int kernelSize = 5;
            int[,] kernel = new int[kernelSize, kernelSize];

            // Perform closing operation
            Filters filter = new ErosionFilter();
            Bitmap closingResult = filter.ProcessImage(Image);

            Bitmap blackHat = new Bitmap(imgWidth, imgHeight);
            for (int x = 0; x < Image.Width; x++)
            {
                for (int y = 0; y < Image.Height; y++)
                {
                    Color originalPixel = Image.GetPixel(x, y);
                    Color closingPixel = closingResult.GetPixel(x, y);
                    int r = Math.Max(0, originalPixel.R - closingPixel.R);
                    int g = Math.Max(0, originalPixel.G - closingPixel.G);
                    int b = Math.Max(0, originalPixel.B - closingPixel.B);
                    blackHat.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
        }
        */
    }
}
