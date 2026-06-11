using CoreAudio;
using System;
using System.Linq;
using System.Windows.Forms;

namespace JabraSwitcher
{
    public partial class FormMain : Form
    {
        public string DefaultOutput { get; set; }
        public string DefaultInput { get; set; }
        private string _dongleName = "Not Found";
        private bool _headsetConnected;
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
            StartProvider(new JabraProvider());
        }

        private async void StartProvider(IHeadsetProvider provider)
        {
            provider.DongleConnected += OnDongleConnected;
            provider.DongleDisconnected += OnDongleDisconnected;
            provider.HeadsetConnected += OnHeadsetConnected;
            provider.HeadsetDisconnected += OnHeadsetDisconnected;
            try
            {
                await provider.StartAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start headset provider: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetDongleLabel(string text)
        {
            if (IsHandleCreated)
                labelDongleName.Invoke((MethodInvoker)(() => labelDongleName.Text = text));
            else
                labelDongleName.Text = text;
        }

        private void OnDongleConnected(object sender, DeviceEventArgs e)
        {
            _dongleName = e.DeviceName;
            SetDongleLabel(_dongleName);
            SwitchToSpeakers();
        }

        private void OnDongleDisconnected(object sender, DeviceEventArgs e)
        {
            _dongleName = "Not Found";
            _headsetConnected = false;
            SetDongleLabel(_dongleName);
            SwitchToSpeakers();
        }

        private void OnHeadsetConnected(object sender, DeviceEventArgs e)
        {
            _headsetConnected = true;
            SetDongleLabel($"{_dongleName} -> {e.DeviceName}");
            ShowToast("Headset Connected", $"{e.DeviceName} has been connected.");
            SwitchToHeadset();
        }

        private void OnHeadsetDisconnected(object sender, DeviceEventArgs e)
        {
            _headsetConnected = false;
            SetDongleLabel(_dongleName);
            ShowToast("Headset Disconnected", $"{e.DeviceName} has been disconnected.");
            SwitchToSpeakers();
        }

        private void SwitchToHeadset()
        {
            SwitchOutputDevice(_dongleName);
            SwitchInputDevice(_dongleName);
        }

        private void SwitchToSpeakers()
        {
            SwitchOutputDevice(DefaultOutput);
            SwitchInputDevice(DefaultInput);
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

        private void InitializeAudioDropDowns()
        {
            DefaultInput = AudioDevices.GetDefaultName(DataFlow.Capture);
            comboBoxInputs.Items.AddRange(AudioDevices.GetActiveNames(DataFlow.Capture).ToArray());
            if (DefaultInput != null)
                comboBoxInputs.SelectedItem = DefaultInput;
            else
                comboBoxInputs.Enabled = false;

            DefaultOutput = AudioDevices.GetDefaultName(DataFlow.Render);
            comboBoxOutputs.Items.AddRange(AudioDevices.GetActiveNames(DataFlow.Render).ToArray());
            if (DefaultOutput != null)
                comboBoxOutputs.SelectedItem = DefaultOutput;
            else
                comboBoxOutputs.Enabled = false;
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
            if (!_headsetConnected && DefaultInput != selectedInput)
            {
                DefaultInput = selectedInput;
                SwitchInputDevice(selectedInput);
            }
        }

        private void ComboBoxOutputs_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedOutput = comboBoxOutputs.SelectedItem.ToString();
            if (!_headsetConnected && DefaultOutput != selectedOutput)
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

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
            HideToTray();
        }

        #endregion
    }
}
