using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SaveSystem.Encryption
{
    public static class DesEncryption
    {
        static readonly int Iterations = 1000;

        public static byte[] Encrypt(byte[] data, string password)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            // create instance of the DES crypto provider
            var des = new DESCryptoServiceProvider();

            // generate a random IV will be used a salt value for generating key
            des.GenerateIV();

            // use derive bytes to generate a key from the password and IV
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, des.IV, Iterations);

            // generate a key from the password provided
            byte[] key = rfc2898DeriveBytes.GetBytes(8);

            // encrypt the plainText
            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, des.CreateEncryptor(key, des.IV), CryptoStreamMode.Write))
            {
                // write the salt first not encrypted
                memoryStream.Write(des.IV, 0, des.IV.Length);

                // write the bytes into the crypto stream so that they are encrypted bytes
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();

                return memoryStream.ToArray();
            }
        }
        
        public static string Encrypt(string plainText, string password)
        {
            if (plainText == null)
            {
                throw new ArgumentNullException(nameof(plainText));
            }
            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            var encrypted = Encrypt(bytes, password);
            return Convert.ToBase64String(encrypted);
        }

        public static bool TryDecrypt(byte[] cipherBytes, string password, out byte[] decrypted)
        {
            if (cipherBytes == null || string.IsNullOrEmpty(password))
            {
                decrypted = new byte[]{};
                return false;
            }

            using (var memoryStream = new MemoryStream(cipherBytes))
            {
                // create instance of the DES crypto provider
                var des = new DESCryptoServiceProvider();

                // get the IV
                byte[] iv = new byte[8];
                memoryStream.Read(iv, 0, iv.Length);

                // use derive bytes to generate key from password and IV
                var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, iv, Iterations);

                byte[] key = rfc2898DeriveBytes.GetBytes(8);

                using var cryptoStream =
                    new CryptoStream(memoryStream, des.CreateDecryptor(key, iv), CryptoStreamMode.Read);
                int len = (int) (memoryStream.Length - memoryStream.Position);
                decrypted = new byte[len];
                cryptoStream.Read(decrypted, 0, len);
                return true;
            }
        }
        
        public static bool TryDecrypt(string cipherText, string password, out string plainText)
        {
            // its pointless trying to decrypt if the cipher text
            // or password has not been supplied
            if (string.IsNullOrEmpty(cipherText))
            {
                plainText = "";
                return false;
            }

            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            bool success = TryDecrypt(cipherBytes, password, out var decryptedBytes);
            plainText = success ? Encoding.UTF8.GetString(decryptedBytes) : "";

            return success;
        }
    }
}