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
        public string DefaultSpeaker { get; set; }
        public string DefaultHeadset { get; set; } = "Not Found";
        public string HeadsetName { get; set; }

        public FormMain()
        {
            InitializeComponent();
            notifyIcon.MouseClick += notifyIcon_MouseClick;
            Resize += FormMain_Resize;
            FormClosing += FormMain_FormClosing;

            InitializeJabraSDK();
            InitializeSpeakerDropDown();
        }

        private async void InitializeJabraSDK()
        {
            var config = new Config(partnerKey: string.Empty);
            IManualApi jabraSdk = Init.InitManualSdk(config);

            jabraSdk.DeviceAdded.Subscribe(device =>
            {
                if (IsJabraLink(device.Name))
                {
                    DefaultHeadset = device.Name;
                    label3.Invoke((MethodInvoker)(() => label3.Text = DefaultHeadset));

                    // Don't let Link take over, until a headset is connected
                    SwitchAudioDevice(DefaultSpeaker);
                    return;
                }

                device.ConnectionList.Subscribe(connections =>
                {
                    if (connections.Count > 0)
                    {
                        // Headset connected
                        HeadsetName = connections[0].Device.Name;
                        label3.Invoke((MethodInvoker)(() => label3.Text = $"{DefaultHeadset} -> {HeadsetName}"));
                        ShowToast("Headset Connected", $"{HeadsetName} has been connected.");
                        SwitchAudioDevice(DefaultHeadset);
                    }
                    else
                    {
                        // Headset disconnected
                        label3.Invoke((MethodInvoker)(() => label3.Text = DefaultHeadset));
                        ShowToast("Headset Disconnected", $"{HeadsetName} has been disconnected.");
                        HeadsetName = string.Empty;
                        SwitchAudioDevice(DefaultSpeaker);
                    }
                });
            });

            jabraSdk.DeviceRemoved.Subscribe(device =>
            {
                if (IsJabraLink(device.Name))
                {
                    DefaultHeadset = "Not Found";
                    label3.Invoke((MethodInvoker)(() => label3.Text = DefaultHeadset));
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

        private void ShowToast(string title, string message)
        {
            notifyIcon.BalloonTipTitle = title;
            notifyIcon.BalloonTipText = message;
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.ShowBalloonTip(3000);
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
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

        private void InitializeSpeakerDropDown()
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator(Guid.Empty);
            DefaultSpeaker = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console).DeviceFriendlyName;
            foreach (var device in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                comboBox1.Items.Add(device.DeviceFriendlyName);
            }
            comboBox1.SelectedItem = DefaultSpeaker;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedSpeaker = comboBox1.SelectedItem.ToString();

            // Only switch if headset is not connected, and default device is different.
            if (string.IsNullOrEmpty(HeadsetName) && DefaultSpeaker != selectedSpeaker)
            {
                DefaultSpeaker = selectedSpeaker;
                SwitchAudioDevice(selectedSpeaker);
            }
        }

        private static void SwitchAudioDevice(string deviceName)
        {
            var enumerator = new MMDeviceEnumerator(Guid.Empty);
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            var targetDevice = devices.FirstOrDefault(d => d.DeviceFriendlyName.Contains(deviceName));
            if (targetDevice is null) { return; }

            enumerator.SetDefaultAudioEndpoint(targetDevice);
        }
    }
}
