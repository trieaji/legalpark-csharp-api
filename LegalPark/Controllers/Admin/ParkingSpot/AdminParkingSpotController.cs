using LegalPark.Models.DTOs.Request.ParkingSpot;
using LegalPark.Services.ParkingSpot.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Controllers.Admin.ParkingSpot
{
    
    [ApiController] 
    [Route("api/v1/admin/parking-spots")] 
    public class AdminParkingSpotController : ControllerBase
    {
        private readonly IAdminParkingSpotService _adminParkingSpotService;
        private readonly ILogger<AdminParkingSpotController> _logger;

        
        public AdminParkingSpotController(IAdminParkingSpotService adminParkingSpotService, ILogger<AdminParkingSpotController> logger)
        {
            _adminParkingSpotService = adminParkingSpotService;
            _logger = logger;
        }

        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateParkingSpot([FromBody] ParkingSpotRequest request)
        {
            _logger.LogInformation("Request to create a new parking spot.");
            return await _adminParkingSpotService.AdminCreateParkingSpot(request);
        }

        
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllParkingSpots()
        {
            _logger.LogInformation("Request to get all parking spots.");
            return await _adminParkingSpotService.AdminGetAllParkingSpots();
        }

        
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetParkingSpotById([FromRoute] string id)
        {
            _logger.LogInformation("Request to get parking spot by ID: {Id}", id);
            return await _adminParkingSpotService.AdminGetParkingSpotById(id);
        }

        
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateParkingSpot([FromRoute] string id, [FromBody] ParkingSpotUpdateRequest request)
        {
            _logger.LogInformation("Request to update parking spot with ID: {Id}", id);
            return await _adminParkingSpotService.AdminUpdateParkingSpot(id, request);
        }

        
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParkingSpot([FromRoute] string id)
        {
            _logger.LogInformation("Request to delete parking spot with ID: {Id}", id);
            return await _adminParkingSpotService.AdminDeleteParkingSpot(id);
        }

        
        [Authorize(Roles = "Admin")]
        [HttpGet("by-merchant/{merchantIdentifier}")]
        public async Task<IActionResult> GetParkingSpotsByMerchant([FromRoute] string merchantIdentifier)
        {
            _logger.LogInformation("Request to get parking spots by merchant: {MerchantIdentifier}", merchantIdentifier);
            return await _adminParkingSpotService.AdminGetParkingSpotsByMerchant(merchantIdentifier);
        }
    }
}
