using System.Security.Cryptography;

namespace LegalPark.Util
{
    public class JwtSecretKeyGenerator
    {
        public static void Main(string[] args)
        {
            // Untuk HS256, disarankan minimal 32 byte (256 bit)
            // Untuk HS512, disarankan minimal 64 byte (512 bit)
            int keyLengthBytes = 32; // Untuk HS256

            // Menggunakan RNGCryptoServiceProvider sebagai generator angka acak yang aman
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                byte[] keyBytes = new byte[keyLengthBytes];
                rngCsp.GetBytes(keyBytes);

                // Encode ke Base64 agar mudah disimpan dan dibaca
                string secretKey = Convert.ToBase64String(keyBytes);

                Console.WriteLine("Generated JWT Secret Key (Base64): " + secretKey);
                Console.WriteLine("Key Length (bytes): " + keyBytes.Length);
                Console.WriteLine("Key Length (bits): " + (keyBytes.Length * 8));
            }
        }
    }
}
