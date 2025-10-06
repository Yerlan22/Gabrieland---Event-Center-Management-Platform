using System.Text;
using System.Security.Cryptography;

namespace gabrieland.api.Utilities
{
    public class Encrytion 
    {
        public static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256 hash from string
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2")); // "x2" formats as hexadecimal
                }
                return builder.ToString();
            }
        }
    }
}
