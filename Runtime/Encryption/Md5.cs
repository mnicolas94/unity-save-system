using System.Security.Cryptography;
using System.Text;

namespace SaveSystem.Encryption
{
    public static class Md5
    {
        public static byte[] GenerateMd5(byte[] data)
        {
            var md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(data);
            return hash;
        }
        
        public static string GenerateMd5(string text)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(text);
            byte[] hash = GenerateMd5(inputBytes);
            return Md5HashToString(hash);
        }

        public static string Md5HashToString(byte[] hash)
        {
            var sb = new StringBuilder();
            foreach (var t in hash)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}