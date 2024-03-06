using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Lab1
{
    public partial class Form1 : Form
    {
        class MatrixFilter : Filters
        {
            protected float[,] kernel = null;
            protected int imgWidth, imgHeight;

            public MatrixFilter(float[,] kernel)
            {
                this.kernel = kernel;
                imgWidth = It.pictureBox1.Image.Width;
                imgHeight = It.pictureBox1.Image.Height;
            }

            public MatrixFilter()
            {
                imgWidth = It.pictureBox1.Image.Width;
                imgHeight = It.pictureBox1.Image.Height;
            }

            protected unsafe override void CalculateNewPixelColor(byte* ptrIn, byte* ptrOut, ref int stride, ref int x, ref int y)
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
                        int idX = Clamp(x + k, 0, imgWidth - 1);
                        int idY = Clamp(y + l, 0, imgHeight - 1);

                        resultR += ptrIn[idX * 3 + idY * stride + 2] * kernel[k + radiusX, l + radiusY];
                        resultG += ptrIn[idX * 3 + idY * stride + 1] * kernel[k + radiusX, l + radiusY];
                        resultB += ptrIn[idX * 3 + idY * stride] * kernel[k + radiusX, l + radiusY];
                    }
                }

                ptrOut[x * 3 + y * stride + 2] = (byte)Clamp((int)resultR, 0, 255);
                ptrOut[x * 3 + y * stride + 1] = (byte)Clamp((int)resultG, 0, 255);
                ptrOut[x * 3 + y * stride] = (byte)Clamp((int)resultB, 0, 255);
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

        class SobelFilter : MatrixFilter
        {
            public SobelFilter(bool Yaxis)
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

        class MotionBlurFilter : MatrixFilter
        {
            public MotionBlurFilter()
            {
                CreateMotionBlurFilter(7); // must be odd number
            }

            public void CreateMotionBlurFilter(int size)
            {
                kernel = new float[size, size];

                float t = 1.0f / size;
                for (int i = 0; i < size; i++)
                    kernel[i, i] = t;
            }
        }

        class EmbossingFilter : MatrixFilter
        {
            public EmbossingFilter()
            {
                kernel = new float[,]{{ 0,  1,  0},
                                      { 1,  0, -1},
                                      { 0, -1,  0}};
            }

            protected unsafe override void CalculateNewPixelColor(byte* ptrIn, byte* ptrOut, ref int stride, ref int x, ref int y)
            {
                int radiusX = kernel.GetLength(0) / 2;
                int radiusY = kernel.GetLength(1) / 2;

                float resultW = 0;

                for (int l = -radiusY; l <= radiusY; l++)
                {
                    for (int k = -radiusX; k <= radiusX; k++)
                    {
                        int idX = Clamp(x + k, 0, imgWidth - 1);
                        int idY = Clamp(y + l, 0, imgHeight - 1);

                        float resultR = ptrIn[(idX * 3) + idY * stride + 2] * kernel[k + radiusX, l + radiusY];
                        float resultG = ptrIn[(idX * 3) + idY * stride + 1] * kernel[k + radiusX, l + radiusY];
                        float resultB = ptrIn[(idX * 3) + idY * stride] * kernel[k + radiusX, l + radiusY];

                        resultW += (resultR + resultG + resultB)/3;
                    }
                }
                resultW += 255;
                resultW /= 2;
                ptrOut[(x * 3) + y * stride + 2] = (byte)Clamp((int)resultW, 0, 255);
                ptrOut[(x * 3) + y * stride + 1] = (byte)Clamp((int)resultW, 0, 255);
                ptrOut[(x * 3) + y * stride] = (byte)Clamp((int)resultW, 0, 255);
            }
        }

        class MedianFilter : MatrixFilter
        {
            protected unsafe override void CalculateNewPixelColor(byte* ptrIn, byte* ptrOut, ref int stride, ref int x, ref int y)
            {
                int radiusX = 1;
                int radiusY = 1;

                List<byte> numsR = new List<byte>(9);
                List<byte> numsG = new List<byte>(9);
                List<byte> numsB = new List<byte>(9);

                for (int l = -radiusY; l <= radiusY; l++)
                {
                    for (int k = -radiusX; k <= radiusX; k++)
                    {
                        int idX = Clamp(x + k, 0, imgWidth - 1);
                        int idY = Clamp(y + l, 0, imgHeight - 1);

                        numsR.Add(ptrIn[idX * 3 + idY * stride + 2]);
                        numsG.Add(ptrIn[idX * 3 + idY * stride + 1]);
                        numsB.Add(ptrIn[idX * 3 + idY * stride]);
                    }
                }
                numsR.Sort(); numsG.Sort(); numsB.Sort();

                ptrOut[(x * 3) + y * stride + 2] = numsR[4];
                ptrOut[(x * 3) + y * stride + 1] = numsG[4];
                ptrOut[(x * 3) + y * stride] = numsB[4];
            }
        }

        class MaximumFilter : MatrixFilter
        {
            protected unsafe override void CalculateNewPixelColor(byte* ptrIn, byte* ptrOut, ref int stride, ref int x, ref int y)
            {
                int radiusX = 1;
                int radiusY = 1;

                List<byte> numsR = new List<byte>(9);
                List<byte> numsG = new List<byte>(9);
                List<byte> numsB = new List<byte>(9);

                for (int l = -radiusY; l <= radiusY; l++)
                {
                    for (int k = -radiusX; k <= radiusX; k++)
                    {
                        int idX = Clamp(x + k, 0, imgWidth - 1);
                        int idY = Clamp(y + l, 0, imgHeight - 1);

                        numsR.Add(ptrIn[(idX * 3) + idY * stride + 2]);
                        numsG.Add(ptrIn[(idX * 3) + idY * stride + 1]);
                        numsB.Add(ptrIn[(idX * 3) + idY * stride]);
                    }
                }
                numsR.Sort(); numsG.Sort(); numsB.Sort();

                ptrOut[(x * 3) + y * stride + 2] = numsR[4];
                ptrOut[(x * 3) + y * stride + 1] = numsG[4];
                ptrOut[(x * 3) + y * stride] = numsB[4];
            }
        }

        // Фильтр размытие
        public void Blur()
        {
            Filters filter = new BlurFilter();
            filter.ProcessImage((Bitmap)pictureBox1.Image);
        }

        // Фильтр размытие по Гауссу
        public void GaussBlur()
        {
            Filters filter = new GaussianFilter();
            filter.ProcessImage(pictureBox1.Image);
        }

        // Фильтр Собебя
        public void SobelX()
        {
            Filters filter = new SobelFilter(false);
            filter.ProcessImage(pictureBox1.Image);
        }

        // Фильтр Собуля
        public void SobelY()
        {
            Filters filter = new SobelFilter(true);
            filter.ProcessImage(pictureBox1.Image);
        }

        class SobelMergeFilter : Filters
        {
            Bitmap imgL, imgR;

            public SobelMergeFilter(Bitmap img1, Bitmap img2)
            {
                imgL = img1;
                imgR = img2;
            }


            protected unsafe override void CalculateNewPixelColor(byte* ptrIn, byte* ptrOut, ref int stride, ref int x, ref int y)
            {
                Color L = imgL.GetPixel(x, y);
                Color R = imgR.GetPixel(x, y);

                byte col = (byte)(Math.Sqrt((int)L.R * (int)L.R + (int)R.R * (int)R.R));

                ptrOut[(x * 3) + y * stride + 2] = col;
                ptrOut[(x * 3) + y * stride + 1] = col;
                ptrOut[(x * 3) + y * stride] = col;
            }
        }
        public void Sobel()
        {
            Filters filter = new SobelFilter(false);
            var imgX = filter.PartialProcessImage(pictureBox1.Image, progressbarMaxVal / 3);
            filter = new SobelFilter(true);
            var imgY = filter.PartialProcessImage(pictureBox1.Image, progressbarMaxVal / 3 * 2);
            filter = new SobelMergeFilter(imgX, imgY);
            var img = filter.PartialProcessImage(pictureBox1.Image, progressbarMaxVal);
            It.pictureBox1.Image = img;
            It.progressBar1.Invoke((Action)(() => { It.progressBar1.Value = 0; It.EnableButtons(true); }));
        }

        // Фильтр резкости
        public void Sharpness()
        {
            Filters filter = new SharpnessFilter();
            filter.ProcessImage(pictureBox1.Image);
        }

        // Размытие в движении
        public void MotionBlur()
        {
            Filters filter = new MotionBlurFilter();
            filter.ProcessImage(pictureBox1.Image);
        }

        // Медианный фильтр
        public void Median()
        {
            Filters filter = new MedianFilter();
            filter.ProcessImage(pictureBox1.Image);
        }

        // Максимум
        public void Maximum()
        {
            Filters filter = new MaximumFilter();
            filter.ProcessImage(pictureBox1.Image);
        }

        // Тиснение
        public void Embossing()
        {
            Filters filter = new GrayScaleFilter();
            Bitmap resultImg = filter.PartialProcessImage(pictureBox1.Image, progressbarMaxVal/3);
            filter = new EmbossingFilter();
            resultImg = filter.PartialProcessImage(resultImg, progressbarMaxVal);
            It.pictureBox1.Image = resultImg;
            It.progressBar1.Invoke((Action)(() => { It.progressBar1.Value = 0; It.EnableButtons(true); }));
        }

        // Светящиеся края
        public void GlowingEdges()
        {
            Filters filter = new MedianFilter();
            Bitmap resultImg = filter.PartialProcessImage(pictureBox1.Image, progressbarMaxVal / 3);
            filter = new SobelFilter(false);
            resultImg = filter.PartialProcessImage(resultImg, progressbarMaxVal / 3 * 2);
            filter = new MaximumFilter();
            resultImg = filter.PartialProcessImage(resultImg, progressbarMaxVal);
            It.pictureBox1.Image = resultImg;
            It.progressBar1.Invoke((Action)(() => { It.progressBar1.Value = 0; It.EnableButtons(true); }));
        }
    }
}