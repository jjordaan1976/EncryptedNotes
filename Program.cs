using System;
using System.Windows.Forms;

namespace TabbedEditor
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new NotePad());
        }
    }
}
