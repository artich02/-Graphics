using System;
using System.Windows.Forms;

namespace Lab1
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form F = new Form1();
            if(DateTime.Now.Hour == 9)
                F.Text = "    '-[  UwU  ]-'";
            Application.Run(F);

        }
    }
}
