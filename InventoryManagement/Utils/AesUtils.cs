using System.Text;
using System.Security.Cryptography;

namespace InventoryManagement.Utils
{
    internal class AesUtils
    {

        private static Aes aes = Aes.Create();
        private static byte[] key = Encoding.UTF8.GetBytes("qwerasdfzxcvtyui");
        public static string Encrypt(string value)
        {
            byte[] iv = new byte[16];
            ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(value);
            MemoryStream msEncrypt = new MemoryStream();
            CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            csEncrypt.Write(plainTextBytes, 0, plainTextBytes.Length);
            csEncrypt.FlushFinalBlock();
            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        public static string Decrypt(string value)
        {
            byte[] iv = new byte[16];
            ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);
            byte[] bytes = Convert.FromBase64String(value);
            MemoryStream msDecrypt = new MemoryStream(bytes);
            CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            StreamReader srDecrypt = new StreamReader(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
    }
}
