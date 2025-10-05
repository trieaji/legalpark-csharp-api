using LegalPark.Models.DTOs.Request.Vehicle;
using LegalPark.Services.Vehicle.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Controllers.User.Vehicle
{
    
    [ApiController]
    [Route("api/v1/user")]
    
    public class UserVehicleController : ControllerBase
    {
        private readonly IUserVehicleService _userVehicleService;

        
        public UserVehicleController(IUserVehicleService userVehicleService)
        {
            _userVehicleService = userVehicleService;
        }

        
        [Authorize(Roles = "User")]
        [HttpPost("vehicle/register")]
        public async Task<IActionResult> UserRegisterVehicle([FromBody] VehicleRequest request)
        {
            return await _userVehicleService.UserRegisterVehicle(request);
        }

        
        [Authorize(Roles = "User")]
        [HttpGet("vehicles")]
        public async Task<IActionResult> UserGetAllVehicle()
        {
            return await _userVehicleService.UserGetAllVehicle();
        }

        
        [Authorize(Roles = "User")]
        [HttpGet("vehicle/{id}")]
        public async Task<IActionResult> UserGetVehicleById([FromRoute] string id)
        {
            return await _userVehicleService.UserGetVehicleById(id);
        }

        
        [Authorize(Roles = "User")]
        [HttpPatch("vehicle/{id}")]
        public async Task<IActionResult> UserUpdateVehicle([FromRoute] string id, [FromBody] VehicleUpdateRequest request)
        {
            return await _userVehicleService.UserUpdateVehicle(id, request);
        }
    }

}
