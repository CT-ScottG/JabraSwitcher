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
        private readonly AppSettings _appSettings;
        private readonly bool _startMinimized;
        private bool _initialMinimizeHandled;

        public FormMain() : this(null, false) { }

        public FormMain(AppSettings settings, bool startMinimized)
        {
            InitializeComponent();

            _appSettings = settings ?? new AppSettings();
            _startMinimized = startMinimized;

            notifyIcon.MouseClick += NotifyIcon_MouseClick;
            Resize += FormMain_Resize;
            FormClosing += FormMain_FormClosing;

            InitializeAudioDropDowns();
            SelectSavedDevices();
            InitializeJabraSDK();
        }

        /// <summary>
        /// Selects the saved default devices in the combo boxes, when they are still present.
        /// </summary>
        private void SelectSavedDevices()
        {
            SelectMatchingItem(comboBoxOutputs, _appSettings.DefaultOutput);
            SelectMatchingItem(comboBoxInputs, _appSettings.DefaultInput);
        }

        private static void SelectMatchingItem(ComboBox comboBox, string deviceName)
        {
            if (string.IsNullOrEmpty(deviceName)) return;

            var match = comboBox.Items.Cast<object>()
                .FirstOrDefault(item => item.ToString().Contains(deviceName));
            if (match != null) comboBox.SelectedItem = match;
        }

        /// <summary>
        /// Keeps the window hidden on first show when configured to start minimized, so the
        /// app starts in the tray. Setting WindowState in the constructor instead fires the
        /// resize/hide path before the handle exists and crashes on ShowInTaskbar.
        /// </summary>
        protected override void SetVisibleCore(bool value)
        {
            if (_startMinimized && !_initialMinimizeHandled)
            {
                _initialMinimizeHandled = true;
                value = false;
            }

            base.SetVisibleCore(value);
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
                    labelJabraDongleName.Invoke((MethodInvoker)(() => labelJabraDongleName.Text = JabraDongle));

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
                        labelJabraDongleName.Invoke((MethodInvoker)(() => labelJabraDongleName.Text = $"{JabraDongle} -> {JabraHeadsetName}"));
                        ShowToast("Headset Connected", $"{JabraHeadsetName} has been connected.");
                        SwitchOutputDevice(JabraDongle);
                        SwitchInputDevice(JabraDongle);
                    }
                    else
                    {
                        // Headset disconnected
                        labelJabraDongleName.Invoke((MethodInvoker)(() => labelJabraDongleName.Text = JabraDongle));
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
                    JabraHeadsetName = string.Empty;
                    labelJabraDongleName.Invoke((MethodInvoker)(() => labelJabraDongleName.Text = JabraDongle));
                    SwitchOutputDevice(DefaultOutput);
                    SwitchInputDevice(DefaultInput);
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
            DefaultInput = AudioDevices.GetDefaultName(DataFlow.Capture);
            comboBoxInputs.Items.AddRange(AudioDevices.GetActiveNames(DataFlow.Capture).ToArray());
            comboBoxInputs.SelectedItem = DefaultInput;

            DefaultOutput = AudioDevices.GetDefaultName(DataFlow.Render);
            comboBoxOutputs.Items.AddRange(AudioDevices.GetActiveNames(DataFlow.Render).ToArray());
            comboBoxOutputs.SelectedItem = DefaultOutput;
        }

        private void SaveSettings()
        {
            _appSettings.DefaultInput = DefaultInput;
            _appSettings.DefaultOutput = DefaultOutput;
            try
            {
                _appSettings.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private static void SwitchOutputDevice(string deviceName) => AudioDevices.SetDefault(DataFlow.Render, deviceName);

        private static void SwitchInputDevice(string deviceName) => AudioDevices.SetDefault(DataFlow.Capture, deviceName);

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

        // Save button handler - add a Save button in designer and wire this up
        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
            HideToTray();
        }
        #endregion
    }
}
