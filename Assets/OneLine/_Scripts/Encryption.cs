using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class Encryption
{
    // Changed key to a 32-byte (256-bit) key which is valid for AES
    private static readonly string key = "1lineSuperSecretKey!@#$%^&*()"; 

    public static string Encrypt(string plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            // Using SHA256 to ensure key is exactly 256 bits regardless of string length
            using (SHA256 sha256 = SHA256.Create())
            {
                var keyBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
                aesAlg.Key = keyBytes;
                aesAlg.GenerateIV();
                var iv = aesAlg.IV;

                using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, iv))
                using (var msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(iv, 0, iv.Length);
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
    }

    public static string Decrypt(string cipherText)
    {
        var fullCipher = Convert.FromBase64String(cipherText);
        using (Aes aesAlg = Aes.Create())
        {
            // Using SHA256 to ensure key is exactly 256 bits regardless of string length
            using (SHA256 sha256 = SHA256.Create())
            {
                var keyBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
                aesAlg.Key = keyBytes;
                var iv = new byte[16];
                Array.Copy(fullCipher, 0, iv, 0, iv.Length);
                aesAlg.IV = iv;
                using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                using (var msDecrypt = new MemoryStream(fullCipher, 16, fullCipher.Length - 16))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}
