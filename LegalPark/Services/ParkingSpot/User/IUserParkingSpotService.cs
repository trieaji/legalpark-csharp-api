using LegalPark.Models.DTOs.Request.ParkingSpot;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Services.ParkingSpot.User
{
    public interface IUserParkingSpotService
    {
        Task<IActionResult> UserGetAvailableParkingSpotsAsync(AvailableSpotFilterRequest filter);
        Task<IActionResult> UserGetParkingSpotsByMerchantAsync(string merchantCode);
    }
}
