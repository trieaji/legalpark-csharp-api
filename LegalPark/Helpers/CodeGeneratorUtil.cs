using LegalPark.Models.Entities;
using LegalPark.Repositories.Merchant;
using System.Security.Cryptography;
using System.Text;

namespace LegalPark.Helpers
{
    public class CodeGeneratorUtil
    {
        private readonly IMerchantRepository _merchantRepository;

        
        private const string AlphanumericChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int ShortCodeLength = 8; 

        
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

        
        public CodeGeneratorUtil(IMerchantRepository merchantRepository)
        {
            _merchantRepository = merchantRepository;
        }

        
        public async Task<string> GenerateUniqueMerchantShortCodeAsync()
        {
            string generatedCode;
            bool isUnique = false;

            do
            {
                
                StringBuilder shortCodeBuilder = new StringBuilder(ShortCodeLength);
                byte[] randomBytes = new byte[ShortCodeLength];
                Rng.GetBytes(randomBytes); 

                for (int i = 0; i < ShortCodeLength; i++)
                {
                    
                    shortCodeBuilder.Append(AlphanumericChars[randomBytes[i] % AlphanumericChars.Length]);
                }
                generatedCode = shortCodeBuilder.ToString();

                
                Merchant existingMerchant = await _merchantRepository.FindByMerchantCodeAsync(generatedCode);

                
                if (existingMerchant == null)
                {
                    isUnique = true;
                }
            } while (!isUnique);

            return generatedCode;
        }
    }
}
