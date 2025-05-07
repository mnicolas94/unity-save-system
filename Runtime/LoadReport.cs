using SaveSystem.Encryption;
using UnityEngine;

namespace SaveSystem
{
    public struct LoadReport
    {
        public bool Success;
        public string FailureReason;
        public string FilePath;
        public bool FileExisted;
        public string Version;
        public string DeviceId;
        public byte[] Checksum;
        public string ChecksumStr;
        public byte[] Data;
        public string DataStr;
        public bool DifferentVersion => Version != Application.version;
        public bool DifferentDevice => DeviceId != SystemInfo.deviceUniqueIdentifier;
        public bool DifferentChecksum => IsChecksumDifferent();

        private bool IsChecksumDifferent()
        {
            if (Data == null)
                return true;
            var newChecksum = Md5.GenerateMd5(Data);
            int len = newChecksum.Length;
            for (int i = 0; i < len; i++)
            {
                byte c = Checksum[i];
                byte nc = newChecksum[i];
                if (nc != c)
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            return $"LoadReport\n" +
                   $"\tSuccess: {Success.ToString()}\n" +
                   $"\tFailureReason: {FailureReason}\n" +
                   $"\tFilePath: {FilePath}\n" +
                   $"\tVersion: {Version}\n" +
                   $"\tDeviceId: {DeviceId}\n" +
                   $"\tChecksum: {Checksum}\n" +
                   $"\tJson: {Data}\n" +
                   $"\tFileExisted: {FileExisted.ToString()}\n" +
                   $"\tDifferentVersion: {DifferentVersion.ToString()}\n" +
                   $"\tDifferentDevice: {DifferentDevice.ToString()}\n" +
                   $"\tDifferentChecksum: {DifferentChecksum.ToString()}";
        }
    }
}