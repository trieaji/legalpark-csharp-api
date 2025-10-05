using LegalPark.Models.DTOs.Request.Vehicle;
using LegalPark.Services.Vehicle.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Controllers.Admin.Vehicle
{
    
    [ApiController]
    [Route("api/v1/admin")]
    
    public class AdminVehicleController : ControllerBase
    {
        private readonly IAdminVehicleService _adminVehicleService;

        
        public AdminVehicleController(IAdminVehicleService adminVehicleService)
        {
            _adminVehicleService = adminVehicleService;
        }

        
        [Authorize(Roles = "Admin")]
        [HttpPost("vehicle/register")]
        public async Task<IActionResult> AdminRegisterVehicle([FromBody] VehicleRequest request) // [FromBody] untuk @RequestBody
        {
            return await _adminVehicleService.AdminRegisterVehicle(request);
        }

        
        [Authorize(Roles = "Admin")]
        [HttpGet("vehicles")]
        public async Task<IActionResult> AdminGetAllVehicles()
        {
            return await _adminVehicleService.AdminGetAllVehicles();
        }

        
        [Authorize(Roles = "Admin")]
        [HttpGet("vehicle/{id}")]
        public async Task<IActionResult> AdminGetVehicleById([FromRoute] string id) // [FromRoute] untuk @PathVariable
        {
            return await _adminVehicleService.AdminGetVehicleById(id);
        }

        
        [Authorize(Roles = "Admin")]
        [HttpGet("vehicle/by-user/{userId}")]
        public async Task<IActionResult> AdminGetVehiclesByUserId([FromRoute] string userId)
        {
            return await _adminVehicleService.AdminGetVehiclesByUserId(userId);
        }

        
        [Authorize(Roles = "Admin")]
        [HttpDelete("vehicle/{id}")]
        public async Task<IActionResult> AdminDeleteVehicle([FromRoute] string id)
        {
            return await _adminVehicleService.AdminDeleteVehicle(id);
        }
    }
}
