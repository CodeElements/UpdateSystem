using System;

namespace CodeElements.UpdateSystem.Core.Internal
{
    internal class OperatingSystemInfo
    {
        public OperatingSystemInfo(OperatingSystemType operatingSystem, Version version)
        {
            OperatingSystem = operatingSystem;
            Version = version;
        }

        public OperatingSystemType OperatingSystem { get; }
        public Version Version { get; }
    }
}