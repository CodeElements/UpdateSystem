#if !NETSTANDARD
using System.Diagnostics;
using System.IO;

#else
using System.Linq;

#endif
using System;
using System.Runtime.InteropServices;
using CodeElements.UpdateSystem.Core.Internal;

namespace CodeElements.UpdateSystem.Utilities
{
    internal class OperatingSystemProvider
    {
#if NETSTANDARD
        public static OperatingSystemInfo GetInfo()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var description = RuntimeInformation.OSDescription;
                var versionPart = description.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).Last();

                return new OperatingSystemInfo(OperatingSystemType.Windows, Version.Parse(versionPart));
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return new OperatingSystemInfo(OperatingSystemType.OSX, null);
            
            return new OperatingSystemInfo(OperatingSystemType.Linux, null);
        }
#else
        public static OperatingSystemInfo GetInfo()
        {
            var osVersion = Environment.OSVersion;
            switch (osVersion.Platform)
            {
                case PlatformID.Win32NT:
                    //as suggested by Microsoft https://msdn.microsoft.com/en-us/library/windows/desktop/ms724429(v=vs.85).aspx
                    //https://stackoverflow.com/questions/25986331/how-to-determine-windows-version-in-future-proof-way
                    //version numbers: https://stackoverflow.com/questions/2819934/detect-windows-version-in-net

                    var versionEx = new OSVERSIONINFOEX
                    {
                        dwOSVersionInfoSize = (uint) Marshal.SizeOf(typeof(OSVERSIONINFOEX))
                    };
                    GetVersionEx(ref versionEx); //if that fails, we just have a workstation
                    var isServer = versionEx.wProductType == ProductType.VER_NT_SERVER;

                    var fileVersion =
                        FileVersionInfo.GetVersionInfo(
                            Path.Combine(Environment.SystemDirectory, "kernel32.dll"));

                    return new OperatingSystemInfo(
                        isServer ? OperatingSystemType.WindowsServer : OperatingSystemType.Windows,
                        new Version(fileVersion.ProductMajorPart, fileVersion.ProductMinorPart,
                            fileVersion.ProductBuildPart, 0));
            }

            //that should not happen as we are on .Net 4.6 that should not run on Linux (expect using Mono, but the .Net Standard version would
            //be better then)
            //https://stackoverflow.com/questions/5116977/how-to-check-the-os-version-at-runtime-e-g-windows-or-linux-without-using-a-con
            //int p = (int) Environment.OSVersion.Platform;
            //if (p == 4 || p == 6 || p == 128)
            return new OperatingSystemInfo(OperatingSystemType.Linux, osVersion.Version);
        }

        [DllImport("kernel32")]
        private static extern bool GetVersionEx(ref OSVERSIONINFOEX osvi);

        [StructLayout(LayoutKind.Sequential)]
        private struct OSVERSIONINFOEX
        {
            public uint dwOSVersionInfoSize;
            private readonly uint dwMajorVersion;
            private readonly uint dwMinorVersion;
            private readonly uint dwBuildNumber;
            private readonly uint dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] private readonly string szCSDVersion;
            private readonly ushort wServicePackMajor;
            private readonly ushort wServicePackMinor;
            private readonly ushort wSuiteMask;
            public readonly ProductType wProductType;
            private readonly byte wReserved;
        }

        private enum ProductType : byte
        {
            VER_NT_DOMAIN_CONTROLLER = 0x0000002,
            VER_NT_SERVER = 0x0000003,
            VER_NT_WORKSTATION = 0x0000001
        }
#endif
    }
}