using LegalPark.Models.DTOs.Request.ParkingSpot;
using LegalPark.Services.ParkingSpot.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Controllers.Admin.ParkingSpot
{
    /// <summary>
    /// Controller untuk mengelola endpoint API yang terkait dengan slot parkir untuk admin.
    /// Mirip dengan @RestController dan @RequestMapping di Java Spring.
    /// </summary>
    [ApiController] // Menandai kelas sebagai API Controller
    [Route("api/v1/admin/parking-spots")] // Base path untuk semua endpoint di controller ini
    public class AdminParkingSpotController : ControllerBase
    {
        private readonly IAdminParkingSpotService _adminParkingSpotService;
        private readonly ILogger<AdminParkingSpotController> _logger;

        // Constructor untuk dependency injection
        public AdminParkingSpotController(IAdminParkingSpotService adminParkingSpotService, ILogger<AdminParkingSpotController> logger)
        {
            _adminParkingSpotService = adminParkingSpotService;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint untuk admin mendaftarkan slot parkir baru.
        /// Contoh: POST /api/v1/admin/parking-spots
        /// Mirip dengan @PostMapping di Java.
        /// </summary>
        /// <param name="request">Data permintaan untuk membuat slot parkir baru.</param>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateParkingSpot([FromBody] ParkingSpotRequest request)
        {
            _logger.LogInformation("Request to create a new parking spot.");
            return await _adminParkingSpotService.AdminCreateParkingSpot(request);
        }

        /// <summary>
        /// Endpoint untuk admin melihat semua slot parkir yang terdaftar.
        /// Contoh: GET /api/v1/admin/parking-spots
        /// Mirip dengan @GetMapping di Java.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllParkingSpots()
        {
            _logger.LogInformation("Request to get all parking spots.");
            return await _adminParkingSpotService.AdminGetAllParkingSpots();
        }

        /// <summary>
        /// Endpoint untuk admin melihat detail slot parkir berdasarkan ID.
        /// Contoh: GET /api/v1/admin/parking-spots/{id}
        /// Mirip dengan @GetMapping di Java.
        /// </summary>
        /// <param name="id">ID dari slot parkir.</param>
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetParkingSpotById([FromRoute] string id)
        {
            _logger.LogInformation("Request to get parking spot by ID: {Id}", id);
            return await _adminParkingSpotService.AdminGetParkingSpotById(id);
        }

        /// <summary>
        /// Endpoint untuk admin memperbarui data slot parkir.
        /// Contoh: PATCH /api/v1/admin/parking-spots/{id}
        /// Mirip dengan @PatchMapping di Java.
        /// </summary>
        /// <param name="id">ID dari slot parkir yang akan diperbarui.</param>
        /// <param name="request">Data permintaan untuk memperbarui slot parkir.</param>
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateParkingSpot([FromRoute] string id, [FromBody] ParkingSpotUpdateRequest request)
        {
            _logger.LogInformation("Request to update parking spot with ID: {Id}", id);
            return await _adminParkingSpotService.AdminUpdateParkingSpot(id, request);
        }

        /// <summary>
        /// Endpoint untuk admin menghapus slot parkir.
        /// Contoh: DELETE /api/v1/admin/parking-spots/{id}
        /// Mirip dengan @DeleteMapping di Java.
        /// </summary>
        /// <param name="id">ID dari slot parkir yang akan dihapus.</param>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParkingSpot([FromRoute] string id)
        {
            _logger.LogInformation("Request to delete parking spot with ID: {Id}", id);
            return await _adminParkingSpotService.AdminDeleteParkingSpot(id);
        }

        /// <summary>
        /// Endpoint untuk admin melihat semua slot parkir yang terkait dengan merchant tertentu.
        /// Contoh: GET /api/v1/admin/parking-spots/by-merchant/MERCH001
        /// Mirip dengan @GetMapping di Java.
        /// </summary>
        /// <param name="merchantIdentifier">ID dari merchant.</param>
        [Authorize(Roles = "Admin")]
        [HttpGet("by-merchant/{merchantIdentifier}")]
        public async Task<IActionResult> GetParkingSpotsByMerchant([FromRoute] string merchantIdentifier)
        {
            _logger.LogInformation("Request to get parking spots by merchant: {MerchantIdentifier}", merchantIdentifier);
            return await _adminParkingSpotService.AdminGetParkingSpotsByMerchant(merchantIdentifier);
        }
    }
}
