using System;
using System.Threading.Tasks;

namespace JabraSwitcher
{
    public class DeviceEventArgs : EventArgs
    {
        public string DeviceName { get; set; }
    }

    public interface IHeadsetProvider
    {
        event EventHandler<DeviceEventArgs> DongleConnected;
        event EventHandler<DeviceEventArgs> DongleDisconnected;
        event EventHandler<DeviceEventArgs> HeadsetConnected;
        event EventHandler<DeviceEventArgs> HeadsetDisconnected;
        Task StartAsync();
    }
}
