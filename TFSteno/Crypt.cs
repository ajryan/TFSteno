using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace TFSteno
{
    public static class Crypt
    {
        private static readonly byte[] _Salt;

        static Crypt()
        {
            var saltString = ConfigurationManager.AppSettings["cipherSalt"];
            _Salt = Encoding.UTF8.GetBytes(saltString);
        }

        public static string Encrypt(string plain)
        {
            if (plain == null) throw new ArgumentNullException("plain");

            var plainBytes = Encoding.UTF8.GetBytes(plain);
            var cipherBytes = ProtectedData.Protect(plainBytes, _Salt, DataProtectionScope.LocalMachine);
            return Convert.ToBase64String(cipherBytes);
        }

        public static string Decrypt(string cipher)
        {
            if (cipher == null) throw new ArgumentNullException("cipher");

            var cipherBytes = Convert.FromBase64String(cipher);
            var plainBytes = ProtectedData.Unprotect(cipherBytes, _Salt, DataProtectionScope.LocalMachine);
            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}