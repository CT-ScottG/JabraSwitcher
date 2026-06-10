using Jabra.NET.Sdk.Core;
using Jabra.NET.Sdk.Core.Types;
using System;
using System.Threading.Tasks;

namespace JabraSwitcher
{
    public class JabraProvider : IHeadsetProvider
    {
        public event EventHandler<DeviceEventArgs> DongleConnected;
        public event EventHandler<DeviceEventArgs> DongleDisconnected;
        public event EventHandler<DeviceEventArgs> HeadsetConnected;
        public event EventHandler<DeviceEventArgs> HeadsetDisconnected;

        private string _lastHeadsetName;

        public async Task StartAsync()
        {
            var config = new Config(partnerKey: string.Empty);
            IManualApi jabraSdk = Init.InitManualSdk(config);

            jabraSdk.DeviceAdded.Subscribe(device =>
            {
                if (IsJabraLink(device.Name))
                {
                    DongleConnected?.Invoke(this, new DeviceEventArgs { DeviceName = device.Name });
                    return;
                }

                device.ConnectionList.Subscribe(connections =>
                {
                    if (connections.Count > 0)
                    {
                        _lastHeadsetName = connections[0].Device.Name;
                        HeadsetConnected?.Invoke(this, new DeviceEventArgs { DeviceName = _lastHeadsetName });
                    }
                    else
                    {
                        HeadsetDisconnected?.Invoke(this, new DeviceEventArgs { DeviceName = _lastHeadsetName });
                        _lastHeadsetName = null;
                    }
                });
            });

            jabraSdk.DeviceRemoved.Subscribe(device =>
            {
                if (IsJabraLink(device.Name))
                    DongleDisconnected?.Invoke(this, new DeviceEventArgs { DeviceName = device.Name });
            });

            await jabraSdk.Start();
        }

        private static bool IsJabraLink(string deviceName) =>
            deviceName.Contains("Jabra Link 360")
            || deviceName.Contains("Jabra Link 370")
            || deviceName.Contains("Jabra Link 380")
            || deviceName.Contains("Jabra Link 390");
    }
}
