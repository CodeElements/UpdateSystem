using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace CodeElements.UpdateSystem.Utilities
{
    internal class OperatingSystemProvider
    {
#if NETSTANDARD
        public static int GetOperatingSystemType()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return 32; //Windows (Other)

            return 106; //Linux (Other)
        }
#else
        public static int GetOperatingSystemType()
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
                    switch (fileVersion.ProductMajorPart)
                    {
                        case 6:
                            if (fileVersion.FileMinorPart == 0)
                                return isServer ? 20 : 1; //Windows Server 2008 | Windows Vista
                            else if (fileVersion.FileMinorPart == 1)
                                return isServer ? 20 : 2; //Windows Server 2008 R2 | Windows 7
                            else //greater than 1
                                return isServer ? 21 : 3; //Windows Server 2012 | Windows 8/8.1
                        case 10:
                            return isServer ? 22 : 4; //Windows Server 2016 | Windows 10
                        default:
                            return 32; //Windows (Other)
                    }
            }

            //https://stackoverflow.com/questions/5116977/how-to-check-the-os-version-at-runtime-e-g-windows-or-linux-without-using-a-con
            //int p = (int) Environment.OSVersion.Platform;
            //if (p == 4 || p == 6 || p == 128)
            return 106; //Linux (Other)
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