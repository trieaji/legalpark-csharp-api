using LegalPark.Models.Entities;
using LegalPark.Services.ParkingTransaction.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Controllers.Admin.ParkingTransaction
{
    /// <summary>
    /// Controller untuk mengelola endpoint API yang terkait dengan transaksi parkir untuk admin.
    /// Mirip dengan @RestController dan @RequestMapping di Java Spring.
    /// </summary>
    [ApiController] // Menandai kelas sebagai API Controller
    [Route("api/v1/admin/parking-transactions")] // Base path untuk semua endpoint di controller ini
    public class AdminParkingTransactionController : ControllerBase
    {
        private readonly IAdminParkingTransactionService _adminParkingTransactionService;
        private readonly ILogger<AdminParkingTransactionController> _logger;

        // Constructor untuk dependency injection
        public AdminParkingTransactionController(IAdminParkingTransactionService adminParkingTransactionService, ILogger<AdminParkingTransactionController> logger)
        {
            _adminParkingTransactionService = adminParkingTransactionService;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint untuk admin melihat semua transaksi parkir yang terdaftar.
        /// Contoh: GET /api/v1/admin/parking-transactions
        /// Mirip dengan @GetMapping di Java.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllParkingTransactions()
        {
            _logger.LogInformation("Request to get all parking transactions.");
            return await _adminParkingTransactionService.AdminGetAllParkingTransactions();
        }

        /// <summary>
        /// Endpoint untuk admin melihat detail transaksi parkir berdasarkan ID.
        /// Contoh: GET /api/v1/admin/parking-transactions/{transactionId}
        /// Mirip dengan @GetMapping di Java.
        /// </summary>
        /// <param name="transactionId">ID dari transaksi.</param>
        [Authorize(Roles = "Admin")]
        [HttpGet("{transactionId}")]
        public async Task<IActionResult> GetParkingTransactionById([FromRoute] Guid transactionId)
        {
            _logger.LogInformation("Request to get parking transaction by ID: {TransactionId}", transactionId);
            return await _adminParkingTransactionService.AdminGetParkingTransactionById(transactionId);
        }

        /// <summary>
        /// Endpoint untuk admin melihat semua transaksi parkir yang terkait dengan ID kendaraan tertentu.
        /// Contoh: GET /api/v1/admin/parking-transactions/by-vehicle/{vehicleId}
        /// Mirip dengan @GetMapping di Java.
        /// </summary>
        /// <param name="vehicleId">ID dari kendaraan.</param>
        [Authorize(Roles = "Admin")]
        [HttpGet("by-vehicle/{vehicleId}")]
        public async Task<IActionResult> GetParkingTransactionsByVehicleId([FromRoute] Guid vehicleId)
        {
            _logger.LogInformation("Request to get parking transactions by vehicle ID: {VehicleId}", vehicleId);
            return await _adminParkingTransactionService.AdminGetParkingTransactionsByVehicleId(vehicleId);
        }

        /// <summary>
        /// Endpoint untuk admin melihat semua transaksi parkir yang terkait dengan ID slot parkir tertentu.
        /// Contoh: GET /api/v1/admin/parking-transactions/by-spot/{parkingSpotId}
        /// Mirip dengan @GetMapping di Java.
        /// </summary>
        /// <param name="parkingSpotId">ID dari slot parkir.</param>
        [Authorize(Roles = "Admin")]
        [HttpGet("by-spot/{parkingSpotId}")]
        public async Task<IActionResult> GetParkingTransactionsByParkingSpotId([FromRoute] Guid parkingSpotId)
        {
            _logger.LogInformation("Request to get parking transactions by parking spot ID: {ParkingSpotId}", parkingSpotId);
            return await _adminParkingTransactionService.AdminGetParkingTransactionsByParkingSpotId(parkingSpotId);
        }

        /// <summary>
        /// Endpoint untuk admin melihat semua transaksi parkir yang terkait dengan ID merchant tertentu.
        /// Contoh: GET /api/v1/admin/parking-transactions/by-merchant/{merchantId}
        /// Mirip dengan @GetMapping di Java.
        /// </summary>
        /// <param name="merchantId">ID dari merchant.</param>
        [Authorize(Roles = "Admin")]
        [HttpGet("by-merchant/{merchantId}")]
        public async Task<IActionResult> GetParkingTransactionsByMerchantId([FromRoute] Guid merchantId)
        {
            _logger.LogInformation("Request to get parking transactions by merchant ID: {MerchantId}", merchantId);
            return await _adminParkingTransactionService.AdminGetParkingTransactionsByMerchantId(merchantId);
        }

        /// <summary>
        /// Endpoint untuk admin melihat semua transaksi parkir berdasarkan status parkir.
        /// Contoh: GET /api/v1/admin/parking-transactions/by-parking-status?status=ACTIVE
        /// Mirip dengan @GetMapping dan @RequestParam di Java.
        /// </summary>
        /// <param name="status">Status parkir yang akan difilter.</param>
        [Authorize(Roles = "Admin")]
        [HttpGet("by-parking-status")]
        public async Task<IActionResult> GetParkingTransactionsByParkingStatus([FromQuery] ParkingStatus status)
        {
            _logger.LogInformation("Request to get parking transactions by parking status: {Status}", status);
            return await _adminParkingTransactionService.AdminGetParkingTransactionsByParkingStatus(status);
        }

        /// <summary>
        /// Endpoint untuk admin melihat semua transaksi parkir berdasarkan status pembayaran.
        /// Contoh: GET /api/v1/admin/parking-transactions/by-payment-status?status=PENDING
        /// Mirip dengan @GetMapping dan @RequestParam di Java.
        /// </summary>
        /// <param name="status">Status pembayaran yang akan difilter.</param>
        [Authorize(Roles = "Admin")]
        [HttpGet("by-payment-status")]
        public async Task<IActionResult> GetParkingTransactionsByPaymentStatus([FromQuery] PaymentStatus status)
        {
            _logger.LogInformation("Request to get parking transactions by payment status: {Status}", status);
            return await _adminParkingTransactionService.AdminGetParkingTransactionsByPaymentStatus(status);
        }

        /// <summary>
        /// Endpoint untuk admin memperbarui status pembayaran transaksi secara manual.
        /// Contoh: PATCH /api/v1/admin/parking-transactions/{transactionId}/payment-status?newPaymentStatus=PAID
        /// Mirip dengan @PatchMapping dan @RequestParam di Java.
        /// </summary>
        /// <param name="transactionId">ID transaksi.</param>
        /// <param name="newPaymentStatus">Status pembayaran baru.</param>
        [Authorize(Roles = "Admin")]
        [HttpPatch("{transactionId}/payment-status")]
        public async Task<IActionResult> UpdateParkingTransactionPaymentStatus([FromRoute] Guid transactionId, [FromQuery] PaymentStatus newPaymentStatus)
        {
            _logger.LogInformation("Request to update payment status for transaction ID: {TransactionId} to new status: {NewPaymentStatus}", transactionId, newPaymentStatus);
            return await _adminParkingTransactionService.AdminUpdateParkingTransactionPaymentStatus(transactionId, newPaymentStatus);
        }

        /// <summary>
        /// Endpoint untuk admin membatalkan transaksi parkir.
        /// Contoh: PATCH /api/v1/admin/parking-transactions/{transactionId}/cancel
        /// Mirip dengan @PatchMapping di Java.
        /// </summary>
        /// <param name="transactionId">ID dari transaksi yang akan dibatalkan.</param>
        [Authorize(Roles = "Admin")]
        [HttpPatch("{transactionId}/cancel")]
        public async Task<IActionResult> CancelParkingTransaction([FromRoute] Guid transactionId)
        {
            _logger.LogInformation("Request to cancel parking transaction with ID: {TransactionId}", transactionId);
            return await _adminParkingTransactionService.AdminCancelParkingTransaction(transactionId);
        }
    }
}
