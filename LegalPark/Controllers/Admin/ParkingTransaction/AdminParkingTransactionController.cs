using LegalPark.Models.Entities;
using LegalPark.Services.ParkingTransaction.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Controllers.Admin.ParkingTransaction
{
    
    [ApiController] 
    [Route("api/v1/admin/parking-transactions")] 
    public class AdminParkingTransactionController : ControllerBase
    {
        private readonly IAdminParkingTransactionService _adminParkingTransactionService;
        private readonly ILogger<AdminParkingTransactionController> _logger;

        
        public AdminParkingTransactionController(IAdminParkingTransactionService adminParkingTransactionService, ILogger<AdminParkingTransactionController> logger)
        {
            _adminParkingTransactionService = adminParkingTransactionService;
            _logger = logger;
        }

        
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllParkingTransactions()
        {
            _logger.LogInformation("Request to get all parking transactions.");
            return await _adminParkingTransactionService.AdminGetAllParkingTransactions();
        }

        
        [Authorize(Roles = "Admin")]
        [HttpGet("{transactionId}")]
        public async Task<IActionResult> GetParkingTransactionById([FromRoute] Guid transactionId)
        {
            _logger.LogInformation("Request to get parking transaction by ID: {TransactionId}", transactionId);
            return await _adminParkingTransactionService.AdminGetParkingTransactionById(transactionId);
        }

        
        [Authorize(Roles = "Admin")]
        [HttpGet("by-vehicle/{vehicleId}")]
        public async Task<IActionResult> GetParkingTransactionsByVehicleId([FromRoute] Guid vehicleId)
        {
            _logger.LogInformation("Request to get parking transactions by vehicle ID: {VehicleId}", vehicleId);
            return await _adminParkingTransactionService.AdminGetParkingTransactionsByVehicleId(vehicleId);
        }

        
        [Authorize(Roles = "Admin")]
        [HttpGet("by-spot/{parkingSpotId}")]
        public async Task<IActionResult> GetParkingTransactionsByParkingSpotId([FromRoute] Guid parkingSpotId)
        {
            _logger.LogInformation("Request to get parking transactions by parking spot ID: {ParkingSpotId}", parkingSpotId);
            return await _adminParkingTransactionService.AdminGetParkingTransactionsByParkingSpotId(parkingSpotId);
        }

        
        [Authorize(Roles = "Admin")]
        [HttpGet("by-merchant/{merchantId}")]
        public async Task<IActionResult> GetParkingTransactionsByMerchantId([FromRoute] Guid merchantId)
        {
            _logger.LogInformation("Request to get parking transactions by merchant ID: {MerchantId}", merchantId);
            return await _adminParkingTransactionService.AdminGetParkingTransactionsByMerchantId(merchantId);
        }

        
        [Authorize(Roles = "Admin")]
        [HttpGet("by-parking-status")]
        public async Task<IActionResult> GetParkingTransactionsByParkingStatus([FromQuery] ParkingStatus status)
        {
            _logger.LogInformation("Request to get parking transactions by parking status: {Status}", status);
            return await _adminParkingTransactionService.AdminGetParkingTransactionsByParkingStatus(status);
        }

        
        [Authorize(Roles = "Admin")]
        [HttpGet("by-payment-status")]
        public async Task<IActionResult> GetParkingTransactionsByPaymentStatus([FromQuery] PaymentStatus status)
        {
            _logger.LogInformation("Request to get parking transactions by payment status: {Status}", status);
            return await _adminParkingTransactionService.AdminGetParkingTransactionsByPaymentStatus(status);
        }

        
        [Authorize(Roles = "Admin")]
        [HttpPatch("{transactionId}/payment-status")]
        public async Task<IActionResult> UpdateParkingTransactionPaymentStatus([FromRoute] Guid transactionId, [FromQuery] PaymentStatus newPaymentStatus)
        {
            _logger.LogInformation("Request to update payment status for transaction ID: {TransactionId} to new status: {NewPaymentStatus}", transactionId, newPaymentStatus);
            return await _adminParkingTransactionService.AdminUpdateParkingTransactionPaymentStatus(transactionId, newPaymentStatus);
        }

        
        [Authorize(Roles = "Admin")]
        [HttpPatch("{transactionId}/cancel")]
        public async Task<IActionResult> CancelParkingTransaction([FromRoute] Guid transactionId)
        {
            _logger.LogInformation("Request to cancel parking transaction with ID: {TransactionId}", transactionId);
            return await _adminParkingTransactionService.AdminCancelParkingTransaction(transactionId);
        }
    }
}
