using LegalPark.Models.DTOs.Request.Vehicle;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Services.Vehicle.User
{
    public interface IUserVehicleService
    {
        Task<IActionResult> UserRegisterVehicle(VehicleRequest request);
        Task<IActionResult> UserGetAllVehicle();
        Task<IActionResult> UserGetVehicleById(string id);
        Task<IActionResult> UserUpdateVehicle(string id, VehicleUpdateRequest request);
    }
}
