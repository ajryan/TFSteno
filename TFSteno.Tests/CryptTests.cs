using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TFSteno.Tests
{
    [TestClass]
    public class CryptTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Encrypt_Null()
        {
            Crypt.Encrypt(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Dencrypt_Null()
        {
            Crypt.Decrypt(null);
        }

        [TestMethod]
        public void Roundtrip()
        {
            const string test = "Four score and seven years ago our forefathers";
            string encrypted = Crypt.Encrypt(test);
            string decrypted = Crypt.Decrypt(encrypted);
            Assert.AreEqual(test, decrypted);
        }
    }
}
