using LegalPark.Models.DTOs.Request.User;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Services.Auth
{
    
    public interface IAuthService
    {
        Task<IActionResult> Login(LoginRequest request);

        
        Task<IActionResult> Register(RegisterRequest request);

        Task<IActionResult> UpdateUserRole(UpdateRoleRequest request);


        Task<IActionResult> Logout(string email);


        
    }
}
