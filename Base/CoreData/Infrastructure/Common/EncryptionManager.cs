using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Serilog;

namespace CoreData.Infrastructure
{
    public static class EncryptionManager
    {
        public static string Encrypt(string text, string key = null, bool throwOnError = true)
        {
            try
            {
                key ??= ConfigurationManager.EncryptionSettings.SymmetricKey;

                var iv = new byte[16];

                using (var aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = iv;

                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    {
                        using (var ms = new MemoryStream())
                        {
                            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                            {
                                using (var sw = new StreamWriter(cs))
                                {
                                    sw.Write(text);
                                }
                            }

                            return Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Fatal(e, "EncryptionManager.Encrypt");

                if (throwOnError)
                    throw;

                return default;
            }
        }

        public static string Decrypt(string encrypted, string key = null, bool throwOnError = true)
        {
            try
            {
                key ??= ConfigurationManager.EncryptionSettings.SymmetricKey;

                var buffer = Convert.FromBase64String(encrypted);
                var iv = new byte[16];

                using (var aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = iv;

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    {
                        string result;
                        using (var ms = new MemoryStream(buffer))
                        {
                            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                            {
                                using (var sr = new StreamReader(cs))
                                {
                                    result = sr.ReadToEnd();
                                }
                            }
                        }

                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Fatal(e, "EncryptionManager.Decrypt");

                if (throwOnError)
                    throw;

                return default;
            }
        }
    }
}