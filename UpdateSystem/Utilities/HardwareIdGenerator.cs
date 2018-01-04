using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace CodeElements.UpdateSystem.Utilities
{
    internal static class HardwareIdGenerator
    {
#if NETSTANDARD
        public static byte[] GenerateHardwareId()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return GetWindowsHardwareId();

            throw new NotImplementedException();
        }
#else
        public static byte[] GenerateHardwareId()
        {
            return GetWindowsHardwareId();
        }
#endif

        private static byte[] GetWindowsHardwareId()
        {
            var numberData = new byte[4];

#if NETSTANDARD
            var sb = new StringBuilder(260);
            var len = GetSystemDirectory(sb, 260);
            var systemDirectory = sb.ToString(0, len);
#else
            var systemDirectory = Environment.SystemDirectory;
#endif

            if (GetVolumeInformation(Path.GetPathRoot(systemDirectory), null, 255, out var serialNumber,
                out var _, out var _, null, 255))
                Buffer.BlockCopy(BitConverter.GetBytes(serialNumber), 0, numberData, 0, 4);
            else
                throw new InvalidOperationException(
                    $"The volume information could not be retrieved. Error: {Marshal.GetLastWin32Error()}");

            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(numberData);
            }
        }

#if NETSTANDARD
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = false)]
        private static extern int GetSystemDirectory([Out] StringBuilder sb, int length);
#endif

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetVolumeInformation(string rootPathName, StringBuilder volumeNameBuffer,
            int volumeNameSize, out uint volumeSerialNumber, out uint maximumComponentLength, out uint fileSystemFlags,
            StringBuilder fileSystemNameBuffer, int nFileSystemNameSize);
    }
}