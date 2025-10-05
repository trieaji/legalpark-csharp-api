using LegalPark.Models.DTOs.Request.ParkingTransaction;
using LegalPark.Services.ParkingTransaction.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Controllers.User.ParkingTransaction
{
    
    [ApiController]
    [Route("api/v1/user")]
    
    public class UserParkingTransactionController : ControllerBase
    {
        private readonly IUserParkingTransactionService _userParkingTransactionService;

        
        public UserParkingTransactionController(IUserParkingTransactionService userParkingTransactionService)
        {
            _userParkingTransactionService = userParkingTransactionService;
        }

        
        [Authorize(Roles = "User")]
        [HttpPost("parking-transactions/entry")]
        public async Task<IActionResult> RecordParkingEntry([FromBody] ParkingEntryRequest request)
        {
            
            return await _userParkingTransactionService.RecordParkingEntry(request);
        }

        
        [Authorize(Roles = "User")]
        [HttpPost("parking-transactions/exit")]
        public async Task<IActionResult> RecordParkingExit([FromBody] ParkingExitRequest request)
        {
            
            return await _userParkingTransactionService.RecordParkingExit(request);
        }

        
        [Authorize(Roles = "User")]
        [HttpGet("parking-transactions/active")]
        public async Task<IActionResult> GetUserActiveParkingTransaction([FromQuery] string licensePlate)
        {
            
            return await _userParkingTransactionService.GetUserActiveParkingTransaction(licensePlate);
        }

        
        [Authorize(Roles = "User")]
        [HttpGet("parking-transactions/history")]
        public async Task<IActionResult> GetUserParkingTransactionHistory([FromQuery] string licensePlate)
        {
            
            return await _userParkingTransactionService.GetUserParkingTransactionHistory(licensePlate);
        }

        
        [Authorize(Roles = "User")]
        [HttpGet("parking-transactions/details/{transactionId}")]
        public async Task<IActionResult> GetUserParkingTransactionDetails(
            [FromRoute] string transactionId,
            [FromQuery] string licensePlate)
        {
            
            return await _userParkingTransactionService.GetUserParkingTransactionDetails(transactionId, licensePlate);
        }
    }

}
