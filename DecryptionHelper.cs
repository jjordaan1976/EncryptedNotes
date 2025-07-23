using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class DecryptionHelper
{
    private const int KeyLength = 32;
    private const int PassLength = 16;

    public static string ReadAndDecryptFile(string filePath, string key, string password)
    {
        try
        {
            byte[] cipherBytes = File.ReadAllBytes(filePath);
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key.Length > KeyLength ? key.Substring(0, KeyLength) : key.PadRight(KeyLength));
            aes.IV = Encoding.UTF8.GetBytes(password.Length > PassLength ? password.Substring(0, PassLength) : password.PadRight(PassLength));

            var decryptor = aes.CreateDecryptor();
            byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }
        catch
        {
            return null; 
        }
    }
}
