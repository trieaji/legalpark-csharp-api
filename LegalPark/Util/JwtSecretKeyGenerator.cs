using System.Security.Cryptography;

namespace LegalPark.Util
{
    public class JwtSecretKeyGenerator
    {
        public static void Main(string[] args)
        {
            
            int keyLengthBytes = 32; 

            
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                byte[] keyBytes = new byte[keyLengthBytes];
                rngCsp.GetBytes(keyBytes);

                
                string secretKey = Convert.ToBase64String(keyBytes);

                Console.WriteLine("Generated JWT Secret Key (Base64): " + secretKey);
                Console.WriteLine("Key Length (bytes): " + keyBytes.Length);
                Console.WriteLine("Key Length (bits): " + (keyBytes.Length * 8));
            }
        }
    }
}
