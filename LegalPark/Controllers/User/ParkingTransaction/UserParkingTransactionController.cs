using LegalPark.Models.DTOs.Request.ParkingTransaction;
using LegalPark.Services.ParkingTransaction.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Controllers.User.ParkingTransaction
{
    /// <summary>
    /// Controller untuk mengelola endpoint User Parking Transaction API.
    /// </summary>
    [ApiController]
    [Route("api/v1/user")]
    // Menggunakan atribut untuk Swagger/OpenAPI, jika perlu:
    // [Swashbuckle.AspNetCore.Annotations.SwaggerTag("User Parking Transaction API", Description = "Endpoint untuk Pengguna mengelola transaksi parkir mereka")]
    public class UserParkingTransactionController : ControllerBase
    {
        private readonly IUserParkingTransactionService _userParkingTransactionService;

        // Dependency Injection:
        // ASP.NET Core menggunakan constructor injection secara default
        public UserParkingTransactionController(IUserParkingTransactionService userParkingTransactionService)
        {
            _userParkingTransactionService = userParkingTransactionService;
        }

        /// <summary>
        /// Mencatat masuknya kendaraan ke slot parkir.
        /// </summary>
        /// <param name="request">Request body berisi data masuk parkir.</param>
        /// <returns>Respons HTTP.</returns>
        [Authorize(Roles = "User")]
        [HttpPost("parking-transactions/entry")]
        public async Task<IActionResult> RecordParkingEntry([FromBody] ParkingEntryRequest request)
        {
            // Validasi model otomatis dipicu oleh [ApiController]
            return await _userParkingTransactionService.RecordParkingEntry(request);
        }

        /// <summary>
        /// Mencatat keluarnya kendaraan dari slot parkir dan memproses pembayaran.
        /// </summary>
        /// <param name="request">Request body berisi data keluar parkir.</param>
        /// <returns>Respons HTTP.</returns>
        [Authorize(Roles = "User")]
        [HttpPost("parking-transactions/exit")]
        public async Task<IActionResult> RecordParkingExit([FromBody] ParkingExitRequest request)
        {
            // Validasi model otomatis dipicu oleh [ApiController]
            return await _userParkingTransactionService.RecordParkingExit(request);
        }

        /// <summary>
        /// Mengambil transaksi parkir aktif pengguna.
        /// </summary>
        /// <param name="licensePlate">Plat nomor kendaraan dari query string.</param>
        /// <returns>Respons HTTP.</returns>
        [Authorize(Roles = "User")]
        [HttpGet("parking-transactions/active")]
        public async Task<IActionResult> GetUserActiveParkingTransaction([FromQuery] string licensePlate)
        {
            // [FromQuery] digunakan untuk binding query parameter.
            return await _userParkingTransactionService.GetUserActiveParkingTransaction(licensePlate);
        }

        /// <summary>
        /// Mengambil riwayat transaksi parkir pengguna.
        /// </summary>
        /// <param name="licensePlate">Plat nomor kendaraan dari query string.</param>
        /// <returns>Respons HTTP.</returns>
        [Authorize(Roles = "User")]
        [HttpGet("parking-transactions/history")]
        public async Task<IActionResult> GetUserParkingTransactionHistory([FromQuery] string licensePlate)
        {
            // [FromQuery] digunakan untuk binding query parameter.
            return await _userParkingTransactionService.GetUserParkingTransactionHistory(licensePlate);
        }

        /// <summary>
        /// Mengambil detail transaksi parkir tertentu.
        /// </summary>
        /// <param name="transactionId">ID transaksi dari URL path.</param>
        /// <param name="licensePlate">Plat nomor kendaraan dari query string.</param>
        /// <returns>Respons HTTP.</returns>
        [Authorize(Roles = "User")]
        [HttpGet("parking-transactions/details/{transactionId}")]
        public async Task<IActionResult> GetUserParkingTransactionDetails(
            [FromRoute] string transactionId,
            [FromQuery] string licensePlate)
        {
            // [FromRoute] untuk path variable dan [FromQuery] untuk query string.
            return await _userParkingTransactionService.GetUserParkingTransactionDetails(transactionId, licensePlate);
        }
    }

}
