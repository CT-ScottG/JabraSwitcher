using CoreAudio;
using Jabra.NET.Sdk.Core;
using Jabra.NET.Sdk.Core.Types;
using System;
using System.Linq;
using System.Windows.Forms;

namespace JabraSwitcher
{
    public partial class FormMain : Form
    {
        public string DefaultOutput { get; set; }
        public string DefaultInput { get; set; }
        public string JabraDongle { get; set; } = "Not Found";
        public string JabraHeadsetName { get; set; }

        public FormMain()
        {
            InitializeComponent();
            notifyIcon.MouseClick += NotifyIcon_MouseClick;
            Resize += FormMain_Resize;
            FormClosing += FormMain_FormClosing;

            InitializeAudioDropDowns();
            InitializeJabraSDK();
        }

        private async void InitializeJabraSDK()
        {
            var config = new Config(partnerKey: string.Empty);
            IManualApi jabraSdk = Init.InitManualSdk(config);

            jabraSdk.DeviceAdded.Subscribe(device =>
            {
                if (IsJabraLink(device.Name))
                {
                    JabraDongle = device.Name;
                    label3.Invoke((MethodInvoker)(() => label3.Text = JabraDongle));

                    // Don't let Link take over, until a headset is connected
                    SwitchOutputDevice(DefaultOutput);
                    SwitchInputDevice(DefaultInput);
                    return;
                }

                device.ConnectionList.Subscribe(connections =>
                {
                    if (connections.Count > 0)
                    {
                        // Headset connected
                        JabraHeadsetName = connections[0].Device.Name;
                        label3.Invoke((MethodInvoker)(() => label3.Text = $"{JabraDongle} -> {JabraHeadsetName}"));
                        ShowToast("Headset Connected", $"{JabraHeadsetName} has been connected.");
                        SwitchOutputDevice(JabraDongle);
                        SwitchInputDevice(JabraDongle);
                    }
                    else
                    {
                        // Headset disconnected
                        label3.Invoke((MethodInvoker)(() => label3.Text = JabraDongle));
                        ShowToast("Headset Disconnected", $"{JabraHeadsetName} has been disconnected.");
                        JabraHeadsetName = string.Empty;
                        SwitchOutputDevice(DefaultOutput);
                        SwitchInputDevice(DefaultInput);
                    }
                });
            });

            jabraSdk.DeviceRemoved.Subscribe(device =>
            {
                if (IsJabraLink(device.Name))
                {
                    JabraDongle = "Not Found";
                    label3.Invoke((MethodInvoker)(() => label3.Text = JabraDongle));
                }
            });

            await jabraSdk.Start();
        }

        private static bool IsJabraLink(string deviceName)
        {
            return deviceName.Contains("Jabra Link 360")
                || deviceName.Contains("Jabra Link 370")
                || deviceName.Contains("Jabra Link 380")
                || deviceName.Contains("Jabra Link 390");
        }

        private void InitializeAudioDropDowns()
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator(Guid.Empty);

            DefaultInput = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console).DeviceFriendlyName;
            foreach (var device in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
            {
                comboBoxInputs.Items.Add(device.DeviceFriendlyName);
            }
            comboBoxInputs.SelectedItem = DefaultInput;

            DefaultOutput = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console).DeviceFriendlyName;
            foreach (var device in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                comboBoxOutputs.Items.Add(device.DeviceFriendlyName);
            }
            comboBoxOutputs.SelectedItem = DefaultOutput;
        }

        private void ComboBoxInputs_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedInput = comboBoxInputs.SelectedItem.ToString();

            // Only switch if headset is not connected, and default device is different.
            if (string.IsNullOrEmpty(JabraHeadsetName) && DefaultInput != selectedInput)
            {
                DefaultInput = selectedInput;
                SwitchInputDevice(selectedInput);
            }
        }

        private void ComboBoxOutputs_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedOutput = comboBoxOutputs.SelectedItem.ToString();

            // Only switch if headset is not connected, and default device is different.
            if (string.IsNullOrEmpty(JabraHeadsetName) && DefaultOutput != selectedOutput)
            {
                DefaultOutput = selectedOutput;
                SwitchOutputDevice(selectedOutput);
            }
        }

        private static void SwitchOutputDevice(string deviceName)
        {
            var enumerator = new MMDeviceEnumerator(Guid.Empty);
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            var targetDevice = devices.FirstOrDefault(d => d.DeviceFriendlyName.Contains(deviceName));
            if (targetDevice is null) { return; }

            enumerator.SetDefaultAudioEndpoint(targetDevice);
        }

        private static void SwitchInputDevice(string deviceName)
        {
            var enumerator = new MMDeviceEnumerator(Guid.Empty);
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
            var targetDevice = devices.FirstOrDefault(d => d.DeviceFriendlyName.Contains(deviceName));
            if (targetDevice is null) { return; }

            enumerator.SetDefaultAudioEndpoint(targetDevice);
        }

        #region NotifyIcon

        private void ShowToast(string title, string message)
        {
            notifyIcon.BalloonTipTitle = title;
            notifyIcon.BalloonTipText = message;
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.ShowBalloonTip(3000);
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ToggleWindowVisibility();
            }
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                HideToTray();
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            notifyIcon.Visible = false;
        }

        private void HideToTray()
        {
            Hide();
            ShowInTaskbar = false;
        }

        private void ToggleWindowVisibility()
        {
            if (Visible)
            {
                HideToTray();
                return;
            }

            ShowInTaskbar = true;
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
        }
        #endregion
    }
}
