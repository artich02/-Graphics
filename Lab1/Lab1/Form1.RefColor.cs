using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Lab1
{
    public partial class Form1 : Form
    {
        private void SetResultColor(Color pipettePixelColor)
        {
            Form form = new Form();

            MyTrackBar trackBarR = new MyTrackBar
            {
                Orientation = Orientation.Vertical,
                Minimum = 1,
                Maximum = 255,
                TickFrequency = 64,
                Location = new Point(30, 40),
                Size = new Size(50, 300),
                BarColor = Color.Red,
            };
            MyTrackBar trackBarG = new MyTrackBar
            {
                Orientation = Orientation.Vertical,
                Minimum = 1,
                Maximum = 255,
                TickFrequency = 64,
                Location = new Point(130, 40),
                Size = new Size(50, 300),
                BarColor = Color.Lime,
            };
            MyTrackBar trackBarB = new MyTrackBar
            {
                Orientation = Orientation.Vertical,
                Minimum = 1,
                Maximum = 255,
                TickFrequency = 64,
                Location = new Point(230, 40),
                Size = new Size(50, 300),
                BarColor = Color.Blue,
            };
            Label labelR = new Label
            {
                Location = new Point(0, 10),
                Text = trackBarR.Value.ToString(),
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize = false,
                Width = 60
            };
            Label labelG = new Label
            {
                Location = new Point(100, 10),
                Text = trackBarG.Value.ToString(),
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize = false,
                Width = 60
            };
            Label labelB = new Label
            {
                Location = new Point(200, 10),
                Text = trackBarB.Value.ToString(),
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize = false,
                Width = 60
            };
            Button ok = new Button
            {
                Location = new Point(350, 250),
                Size = new Size(50, 50),
                Text = "OK",
                DialogResult = DialogResult.OK,
            };
            trackBarR.ValueChanged += new EventHandler(TrackbarChange);
            trackBarG.ValueChanged += new EventHandler(TrackbarChange);
            trackBarB.ValueChanged += new EventHandler(TrackbarChange);
            form.Controls.Add(trackBarR);
            form.Controls.Add(trackBarG);
            form.Controls.Add(trackBarB);
            form.Controls.Add(labelR);
            form.Controls.Add(labelG);
            form.Controls.Add(labelB);
            form.Controls.Add(ok);

            form.BackColor = Color.White;
            form.StartPosition = FormStartPosition.CenterParent;
            form.Width = 450;
            form.Height = 400;
            form.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            form.Text = "Выберите целевой цвет    исходный цвет: (" 
                + pipettePixelColor.R + " " + pipettePixelColor.G + " " + pipettePixelColor.B + ")";

            DialogResult dr = form.ShowDialog();

            (float, float, float) ratio = (
                (float)trackBarR.Value / pipettePixelColor.R, 
                (float)trackBarG.Value / pipettePixelColor.G, 
                (float)trackBarB.Value / pipettePixelColor.B);
            Filters filter = new CorrectionWithReferenceColorFilter(ratio);

            if (dr == DialogResult.Cancel)
            {
                form.Close();
                filter.ProcessImage(pictureBox1.Image);
            }
            else if (dr == DialogResult.OK)
            {
                form.Close();
                filter.ProcessImage(pictureBox1.Image);
            }

            void TrackbarChange(object sender, EventArgs e)
            {
                labelR.Text = trackBarR.Value.ToString();
                labelG.Text = trackBarG.Value.ToString();
                labelB.Text = trackBarB.Value.ToString();
            }
        }

        class CorrectionWithReferenceColorFilter : Filters
        {
            (float, float, float) ratio;
            public CorrectionWithReferenceColorFilter((float, float, float) ratio)
            {
                this.ratio = ratio;
            }
            protected unsafe override void CalculateNewPixelColor(byte* ptrIn, byte* ptrOut, ref int stride, ref int x, ref int y)
            {
                int red = Math.Min((int)(ptrIn[x * 3 + y * stride + 2] * ratio.Item1), 255);
                int green = Math.Min((int)(ptrIn[x * 3 + y * stride + 1] * ratio.Item2), 255);
                int blue = Math.Min((int)(ptrIn[x * 3 + y * stride] * ratio.Item3), 255);

                ptrOut[x * 3 + y * stride + 2] = (byte)red;
                ptrOut[x * 3 + y * stride + 1] = (byte)green;
                ptrOut[x * 3 + y * stride] = (byte)blue;
            }
        }

        partial class MyTrackBar : TrackBar
        {
            Color barColor;

            public Color BarColor { get => barColor; set => barColor = value; }

            public MyTrackBar()
            {
            }

            // custom draw item specs  
            private const int TBCD_TICS = 0x1;
            private const int TBCD_THUMB = 0x2;
            private const int TBCD_CHANNEL = 0x3;

            [StructLayout(LayoutKind.Sequential)]
            public struct NMHDR
            {
                public IntPtr hwndFrom;
                public IntPtr idFrom;
                public int code;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct NMCUSTOMDRAW
            {
                public NMHDR hdr;
                public int dwDrawStage;
                public IntPtr hdc;
                public RECT rc;
                public IntPtr dwItemSpec;
                public uint uItemState;
                public IntPtr lItemlParam;
            }

            [Flags]
            public enum CDRF
            {
                CDRF_DODEFAULT = 0x0,
                CDRF_NEWFONT = 0x2,
                CDRF_SKIPDEFAULT = 0x4,
                CDRF_DOERASE = 0x8,
                CDRF_SKIPPOSTPAINT = 0x100,
                CDRF_NOTIFYPOSTPAINT = 0x10,
                CDRF_NOTIFYITEMDRAW = 0x20,
                CDRF_NOTIFYSUBITEMDRAW = 0x20,
                CDRF_NOTIFYPOSTERASE = 0x40
            }

            [Flags]
            public enum CDDS
            {
                CDDS_PREPAINT = 0x1,
                CDDS_POSTPAINT = 0x2,
                CDDS_PREERASE = 0x3,
                CDDS_POSTERASE = 0x4,
                CDDS_ITEM = 0x10000,
                CDDS_ITEMPREPAINT = (CDDS.CDDS_ITEM | CDDS.CDDS_PREPAINT),
                CDDS_ITEMPOSTPAINT = (CDDS.CDDS_ITEM | CDDS.CDDS_POSTPAINT),
                CDDS_ITEMPREERASE = (CDDS.CDDS_ITEM | CDDS.CDDS_PREERASE),
                CDDS_ITEMPOSTERASE = (CDDS.CDDS_ITEM | CDDS.CDDS_POSTERASE),
                CDDS_SUBITEM = 0x20000
            }

            [DllImport("User32.dll", SetLastError = true)]
            public static extern int FillRect(IntPtr hDC, ref RECT lpRect, IntPtr hBR);

            [DllImport("Gdi32.dll", SetLastError = true)]
            public static extern IntPtr CreateSolidBrush(int crColor);

            [DllImport("Gdi32.dll", SetLastError = true)]
            public static extern bool DeleteObject(IntPtr hObject);

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_REFLECT + WM_NOFITY)
                {
                    var pnmhdr = (NMHDR)m.GetLParam(typeof(NMHDR));
                    if (pnmhdr.code == NM_CUSTOMDRAW)
                    {
                        var pnmlv = (NMCUSTOMDRAW)m.GetLParam(typeof(NMCUSTOMDRAW));
                        switch (pnmlv.dwDrawStage)
                        {
                            case (int)CDDS.CDDS_PREPAINT:
                                {
                                    m.Result = new IntPtr((int)CDRF.CDRF_NOTIFYITEMDRAW);
                                    break;
                                }

                            case (int)CDDS.CDDS_ITEMPREPAINT:
                                {
                                    if (((int)pnmlv.dwItemSpec == TBCD_THUMB))
                                    {
                                        IntPtr hBrush = CreateSolidBrush(ColorTranslator.ToWin32(barColor));
                                        FillRect(pnmlv.hdc, ref pnmlv.rc, hBrush);
                                        DeleteObject(hBrush);

                                        m.Result = new IntPtr((int)CDRF.CDRF_SKIPDEFAULT);
                                    }
                                    else
                                        m.Result = new IntPtr((int)CDRF.CDRF_NOTIFYPOSTPAINT);
                                    break;
                                }

                            case (int)CDDS.CDDS_ITEMPOSTPAINT:
                                {
                                    m.Result = new IntPtr((int)CDRF.CDRF_DODEFAULT);
                                    break;
                                }
                        }
                    }
                    return;
                }
                else
                    base.WndProc(ref m);
            }

            private const int NM_FIRST = 0;
            private const int NM_CLICK = NM_FIRST - 2;
            private const int NM_CUSTOMDRAW = NM_FIRST - 12;
            private const int WM_REFLECT = 0x2000;
            private const int WM_NOFITY = 0x4E;
        }
    }
}
