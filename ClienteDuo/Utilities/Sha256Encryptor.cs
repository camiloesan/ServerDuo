using System;
using System.Security.Cryptography;
using System.Text;

namespace ClienteDuo.Utilities
{
    internal class Sha256Encryptor
    {
        protected Sha256Encryptor()
        {
        }

        public static string SHA256_hash(string value)
        {
            StringBuilder stringBuilder = new StringBuilder();

            using (SHA256 hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    stringBuilder.Append(b.ToString("x2"));
            }

            return stringBuilder.ToString();
        }
    }
}
