using LegalPark.Models.DTOs.Request.Balance;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Services.Balance
{
    public interface IBalanceService
    {
        Task<IActionResult> DeductBalance(DeductBalanceRequest request);
        Task<IActionResult> AddBalance(AddBalanceRequest request);
        Task<IActionResult> GetUserBalance(string userId);
    }
}
