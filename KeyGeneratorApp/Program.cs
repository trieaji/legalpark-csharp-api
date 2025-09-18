using System.Security.Cryptography;

public class Program
{
    /// <summary>
    /// Titik masuk utama untuk aplikasi konsol.
    /// Metode ini akan berjalan saat aplikasi dimulai.
    /// </summary>
    public static void Main(string[] args)
    {
        // Menentukan panjang kunci dalam byte.
        // Umumnya, HS256 membutuhkan minimal 32 byte (256 bit).
        // HS512 membutuhkan 64 byte (512 bit).
        const int keyLengthBytes = 32; // Pilih 32 byte untuk HS256

        // Menggunakan RNGCryptoServiceProvider untuk membuat angka acak yang aman secara kriptografis.
        // Ini adalah cara yang direkomendasikan di .NET untuk menghasilkan data sensitif.
        // `using` memastikan objek dibuang dengan benar setelah digunakan.
        using (var rngCsp = new RNGCryptoServiceProvider())
        {
            // Membuat array byte dengan ukuran yang ditentukan.
            byte[] keyBytes = new byte[keyLengthBytes];

            // Mengisi array byte dengan data acak yang kuat secara kriptografis.
            rngCsp.GetBytes(keyBytes);

            // Mengonversi array byte menjadi string Base64 yang aman untuk penyimpanan.
            string secretKey = Convert.ToBase64String(keyBytes);

            // Menampilkan kunci yang dihasilkan dan informasinya di konsol.
            Console.WriteLine("Generated JWT Secret Key (Base64):");
            Console.WriteLine(secretKey);
            Console.WriteLine(); // Baris kosong untuk keterbacaan
            Console.WriteLine("Key Length (bytes): " + keyBytes.Length);
            Console.WriteLine("Key Length (bits): " + (keyBytes.Length * 8));
        }
    }
}