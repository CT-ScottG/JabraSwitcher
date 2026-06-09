using System;
using System.Threading;
using System.Windows.Forms;
using CoreAudio;

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

                // Start minimized to the tray only when saved settings match the
                // devices currently available; otherwise show the form to configure.
                var settings = AppSettings.Load();
                bool startMinimized = settings != null && ConfiguredDevicesAvailable(settings);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormMain(settings, startMinimized));
            }
        }

        private static bool ConfiguredDevicesAvailable(AppSettings settings)
        {
            try
            {
                return AudioDevices.Exists(DataFlow.Render, settings.DefaultOutput)
                    && AudioDevices.Exists(DataFlow.Capture, settings.DefaultInput);
            }
            catch
            {
                // If we can't enumerate audio devices, fall back to showing the form.
                return false;
            }
        }
    }
}
