using LegalPark.Models.Entities;
using LegalPark.Repositories.Merchant;
using System.Security.Cryptography;
using System.Text;

namespace LegalPark.Helpers
{
    public class CodeGeneratorUtil
    {
        private readonly IMerchantRepository _merchantRepository;

        // Karakter yang bisa digunakan untuk kode pendek
        private const string AlphanumericChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int ShortCodeLength = 8; // Panjang kode pendek

        // RandomNumberGenerator lebih aman dan lebih disukai daripada Random
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

        /// <summary>
        /// Constructor untuk dependency injection IMerchantRepository.
        /// </summary>
        /// <param name="merchantRepository">Instansiasi repository merchant.</param>
        public CodeGeneratorUtil(IMerchantRepository merchantRepository)
        {
            _merchantRepository = merchantRepository;
        }

        /// <summary>
        /// Menghasilkan kode alfanumerik pendek yang unik untuk merchant secara asinkron.
        /// Kode ini dijamin unik dengan memeriksa di database.
        /// </summary>
        /// <returns>Kode merchant pendek yang unik.</returns>
        public async Task<string> GenerateUniqueMerchantShortCodeAsync()
        {
            string generatedCode;
            bool isUnique = false;

            do
            {
                // Gunakan StringBuilder untuk performa yang lebih baik dalam loop
                StringBuilder shortCodeBuilder = new StringBuilder(ShortCodeLength);
                byte[] randomBytes = new byte[ShortCodeLength];
                Rng.GetBytes(randomBytes); // Isi array dengan byte acak

                for (int i = 0; i < ShortCodeLength; i++)
                {
                    // Ambil karakter dari string AlphanumericChars menggunakan byte acak
                    shortCodeBuilder.Append(AlphanumericChars[randomBytes[i] % AlphanumericChars.Length]);
                }
                generatedCode = shortCodeBuilder.ToString();

                // Periksa keunikan di database secara asinkron
                Merchant existingMerchant = await _merchantRepository.FindByMerchantCodeAsync(generatedCode);

                // Jika tidak ada merchant dengan kode ini, maka kode unik
                if (existingMerchant == null)
                {
                    isUnique = true;
                }
            } while (!isUnique);

            return generatedCode;
        }
    }
}
