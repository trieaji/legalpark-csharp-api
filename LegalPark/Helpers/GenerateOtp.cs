using System;

namespace LegalPark.Helpers
{
    public class GenerateOtp
    {
        // Menggunakan instance Random statis untuk performa dan keamanan thread yang memadai.
        // readonly memastikan instance ini hanya dapat diinisialisasi sekali.
        private static readonly Random randomGenerator = new Random();

        /// <summary>
        /// Menghasilkan nomor acak 4 digit sebagai string.
        /// Angka yang dihasilkan berada dalam rentang [1000, 9999].
        /// </summary>
        /// <returns>String yang berisi nomor acak 4 digit.</returns>
        public static string GenerateRandomNumber()
        {
            // Menghasilkan angka acak antara 1000 (inklusif) dan 10000 (eksklusif)
            int randomNumber = randomGenerator.Next(1000, 10000);
            return randomNumber.ToString();
        }

        /// <summary>
        /// Menghasilkan tanggal dan waktu kedaluwarsa yang diatur satu hari dari sekarang.
        /// </summary>
        /// <returns>Objek DateTime yang mewakili tanggal dan waktu kedaluwarsa.</returns>
        public static DateTime GetExpiryDate()
        {
            // Menggunakan DateTime.Now untuk mendapatkan tanggal dan waktu saat ini.
            DateTime today = DateTime.Now;

            // Menambahkan satu hari ke tanggal saat ini
            DateTime expiryDate = today.AddDays(1);

            return expiryDate;
        }
    }
}
