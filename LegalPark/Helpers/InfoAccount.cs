using LegalPark.Models.Entities;
using LegalPark.Repositories.User;
using System.Security.Claims;

namespace LegalPark.Helpers
{
    public class InfoAccount
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Konstruktor untuk inisialisasi InfoAccount dengan dependency injection.
        /// </summary>
        /// <param name="userRepository">Instance dari repository pengguna.</param>
        /// <param name="httpContextAccessor">Instance untuk mengakses HttpContext.</param>
        public InfoAccount(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            // Menyimpan instance repository dan http context accessor
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Mengambil data pengguna saat ini secara asinkron.
        /// </summary>
        /// <returns>Objek User, atau null jika pengguna tidak ditemukan atau tidak login.</returns>
        public async Task<User?> GetAsync()
        {
            // Memastikan HttpContext dan pengguna terautentikasi
            var userClaimsPrincipal = _httpContextAccessor.HttpContext?.User;
            if (userClaimsPrincipal == null || !userClaimsPrincipal.Identity.IsAuthenticated)
            {
                return null;
            }

            // Mendapatkan email pengguna dari klaim (claims)
            var userEmail = userClaimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;

            // Jika email tidak ditemukan, kembalikan null
            if (string.IsNullOrEmpty(userEmail))
            {
                return null;
            }

            // Mencari pengguna di database berdasarkan email menggunakan repository
            var user = await _userRepository.findByEmail(userEmail);

            // Mengembalikan objek User
            return user;
        }
    }

}
