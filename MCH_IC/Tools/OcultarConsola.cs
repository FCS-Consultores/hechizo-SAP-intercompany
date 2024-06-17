using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Intercompany.tools
{
    class OcultarConsola
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int HIDE = 0;
        const int SHOW = 5;

        public static void DisappearConsole()
        {
            ShowWindow(GetConsoleWindow(), HIDE);
        }
    }
}
