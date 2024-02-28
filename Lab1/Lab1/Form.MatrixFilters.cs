using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab1
{
    public partial class Form1 : Form
    {
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

                for (int l = -radiusY; l <= radiusY; l++)
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

        class SobelOperator : MatrixFilter
        {
            public SobelOperator(bool Yaxis)
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
        // Фильтр размытие
        public void Blur()
        {
            Filters filter = new BlurFilter();
            filter.processImage((Bitmap)pictureBox1.Image);
        }

        // Фильтр размытие по Гауссу
        public void GaussBlur()
        {
            Filters filter = new GaussianFilter();
            filter.processImage((Bitmap)pictureBox1.Image);
        }

        // Фильтр Собебя
        public void SobelX()
        {
            Filters filter = new SobelOperator(false);
            filter.processImage((Bitmap)pictureBox1.Image);
        }

        // Фильтр Собуля
        public void SobelY()
        {
            Filters filter = new SobelOperator(true);
            filter.processImage((Bitmap)pictureBox1.Image);
        }

        // Фильтр резкости
        public void Sharpness()
        {
            Filters filter = new SharpnessFilter();
            filter.processImage((Bitmap)pictureBox1.Image);
        }
    }
}