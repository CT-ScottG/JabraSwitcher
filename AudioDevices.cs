using System;
using System.Collections.Generic;
using System.Linq;
using CoreAudio;

namespace JabraSwitcher
{
    /// <summary>
    /// Thin wrapper over CoreAudio's <see cref="MMDeviceEnumerator"/> for the handful
    /// of audio-endpoint operations this app needs. Device names are matched by
    /// substring against their friendly name.
    /// </summary>
    internal static class AudioDevices
    {
        /// <summary>Friendly name of the current default endpoint for the given flow, or null if none exists.</summary>
        public static string GetDefaultName(DataFlow flow)
        {
            try
            {
                return new MMDeviceEnumerator(Guid.Empty)
                    .GetDefaultAudioEndpoint(flow, Role.Console)
                    .DeviceFriendlyName;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>Friendly names of every active endpoint for the given flow.</summary>
        public static IEnumerable<string> GetActiveNames(DataFlow flow)
        {
            return new MMDeviceEnumerator(Guid.Empty)
                .EnumerateAudioEndPoints(flow, DeviceState.Active)
                .Select(d => d.DeviceFriendlyName);
        }

        /// <summary>True if any active endpoint's name contains <paramref name="deviceName"/>.</summary>
        public static bool Exists(DataFlow flow, string deviceName)
        {
            return GetActiveNames(flow).Any(name => name.Contains(deviceName ?? string.Empty));
        }

        /// <summary>
        /// Makes the first active endpoint whose name contains <paramref name="deviceName"/>
        /// the default device. No-op if no match is found.
        /// </summary>
        public static void SetDefault(DataFlow flow, string deviceName)
        {
            var enumerator = new MMDeviceEnumerator(Guid.Empty);
            var target = enumerator
                .EnumerateAudioEndPoints(flow, DeviceState.Active)
                .FirstOrDefault(d => d.DeviceFriendlyName.Contains(deviceName));
            if (target is null) return;

            enumerator.SetDefaultAudioEndpoint(target);
        }
    }
}
