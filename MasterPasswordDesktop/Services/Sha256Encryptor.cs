using System;
using System.Security.Cryptography;
using System.Text;

namespace MasterPasswordDesktop.Infrastructure
{
    public class Sha256Encryptor : IEncryptProvider
    {
        public bool Check(string source, string key)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                string hash = GetHash(sha256Hash, source);

                StringComparer comparer = StringComparer.OrdinalIgnoreCase;

                return comparer.Compare(hash, key) == 0;
            }
        }

        private string GetHash(HashAlgorithm hashAlgorithm, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}
