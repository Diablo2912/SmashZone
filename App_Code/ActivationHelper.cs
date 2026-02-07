using System.Security.Cryptography;
using System.Text;

namespace SmashZone.App_Code
{
    public static class ActivationHelper
    {
        // 32 bytes => 64 hex chars token
        public static string GenerateToken()
        {
            byte[] bytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            return ToHex(bytes);
        }

        public static string Sha256Hex(string input)
        {
            using (var sha = SHA256.Create())
            {
                byte[] data = sha.ComputeHash(Encoding.UTF8.GetBytes(input ?? ""));
                return ToHex(data);
            }
        }

        private static string ToHex(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
