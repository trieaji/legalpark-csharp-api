using System;

namespace LegalPark.Helpers
{
    public class GenerateOtp
    {
        
        private static readonly Random randomGenerator = new Random();

        
        public static string GenerateRandomNumber()
        {
            
            int randomNumber = randomGenerator.Next(1000, 10000);
            return randomNumber.ToString();
        }

        
        public static DateTime GetExpiryDate()
        {
            
            DateTime today = DateTime.Now;

            
            DateTime expiryDate = today.AddDays(1);

            return expiryDate;
        }
    }
}
