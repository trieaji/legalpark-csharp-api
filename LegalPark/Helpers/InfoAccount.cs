using LegalPark.Models.Entities;
using LegalPark.Repositories.User;
using System.Security.Claims;

namespace LegalPark.Helpers
{
    public class InfoAccount
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        
        public InfoAccount(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        
        public async Task<User?> GetAsync()
        {
            
            var userClaimsPrincipal = _httpContextAccessor.HttpContext?.User;
            if (userClaimsPrincipal == null || !userClaimsPrincipal.Identity.IsAuthenticated)
            {
                return null;
            }

            
            var userEmail = userClaimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;

            
            if (string.IsNullOrEmpty(userEmail))
            {
                return null;
            }

            
            var user = await _userRepository.findByEmail(userEmail);

            
            return user;
        }
    }

}
