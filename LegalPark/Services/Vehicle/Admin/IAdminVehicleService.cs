using LegalPark.Models.DTOs.Request.Vehicle;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Services.Vehicle.Admin
{
    public interface IAdminVehicleService
    {
        Task<IActionResult> AdminRegisterVehicle(VehicleRequest request);
        Task<IActionResult> AdminGetAllVehicles();
        Task<IActionResult> AdminGetVehicleById(string id);
        Task<IActionResult> AdminGetVehiclesByUserId(string userId);
        Task<IActionResult> AdminDeleteVehicle(string id);
    }
}
