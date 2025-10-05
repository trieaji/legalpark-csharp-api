using LegalPark.Models.DTOs.Request.ParkingSpot;
using LegalPark.Services.ParkingSpot.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Controllers.User.ParkingSpot
{
    
    [ApiController]
    [Route("api/v1/user")]
    
    public class UserParkingSpotController : ControllerBase
    {
        private readonly IUserParkingSpotService _userParkingSpotService;

        
        public UserParkingSpotController(IUserParkingSpotService userParkingSpotService)
        {
            _userParkingSpotService = userParkingSpotService;
        }

        
        [Authorize(Roles = "User")]
        [HttpGet("parking-spots/available")]
        public async Task<IActionResult> GetAvailableParkingSpots([FromQuery] AvailableSpotFilterRequest filter)
        {
            
            return await _userParkingSpotService.UserGetAvailableParkingSpotsAsync(filter);
        }

        
        [Authorize(Roles = "User")]
        [HttpGet("parking-spots/by-merchant/{merchantCode}")]
        public async Task<IActionResult> GetParkingSpotsByMerchant([FromRoute] string merchantCode)
        {
            
            return await _userParkingSpotService.UserGetParkingSpotsByMerchantAsync(merchantCode);
        }
    }

}
