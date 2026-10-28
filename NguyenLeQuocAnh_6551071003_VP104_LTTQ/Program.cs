using System;
using System.Windows.Forms;

namespace NguyenLeQuocAnh_6551071003_VP104_LTTQ
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}