using System;
using System.Windows.Forms;
using System.Threading;

namespace JabraSwitcher
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var mutex = new Mutex(true, "JabraSwitcherMutex", out bool createdNew))
            {
                if (!createdNew)
                {
                    MessageBox.Show("Another instance is already running.", "Jabra Switcher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormMain());
            }
        }
    }
}
