using LegalPark.Models.DTOs.Request.ParkingSpot;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Services.ParkingSpot.Admin
{
    public interface IAdminParkingSpotService
    {
        Task<IActionResult> AdminCreateParkingSpot(ParkingSpotRequest request);
        Task<IActionResult> AdminGetAllParkingSpots();
        Task<IActionResult> AdminGetParkingSpotById(string id);
        Task<IActionResult> AdminUpdateParkingSpot(string id, ParkingSpotUpdateRequest request);
        Task<IActionResult> AdminDeleteParkingSpot(string id);
        Task<IActionResult> AdminGetParkingSpotsByMerchant(string merchantIdentifier);
    }
}
