using LegalPark.Models.DTOs.Request.ParkingTransaction;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Services.ParkingTransaction.User
{
    public interface IUserParkingTransactionService
    {
        /// <summary>
        /// [PENGGUNA] Mencatat masuknya kendaraan ke slot parkir.
        /// </summary>
        /// <param name="request">Objek permintaan yang berisi detail masuk.</param>
        /// <returns>IActionResult yang mewakili status respons HTTP.</returns>
        Task<IActionResult> RecordParkingEntry(ParkingEntryRequest request);

        /// <summary>
        /// [PENGGUNA] Mencatat keluarnya kendaraan dari slot parkir dan memproses pembayaran.
        /// </summary>
        /// <param name="request">Objek permintaan yang berisi detail keluar.</param>
        /// <returns>IActionResult yang mewakili status respons HTTP.</returns>
        Task<IActionResult> RecordParkingExit(ParkingExitRequest request);

        /// <summary>
        /// [PENGGUNA] Mengambil transaksi parkir yang sedang aktif untuk plat nomor kendaraan pengguna.
        /// </summary>
        /// <param name="licensePlate">Plat nomor kendaraan.</param>
        /// <returns>IActionResult yang berisi detail transaksi aktif.</returns>
        Task<IActionResult> GetUserActiveParkingTransaction(string licensePlate);

        /// <summary>
        /// [PENGGUNA] Mengambil riwayat semua transaksi parkir yang terkait dengan plat nomor kendaraan pengguna.
        /// </summary>
        /// <param name="licensePlate">Plat nomor kendaraan.</param>
        /// <returns>IActionResult yang berisi riwayat transaksi.</returns>
        Task<IActionResult> GetUserParkingTransactionHistory(string licensePlate);

        /// <summary>
        /// [PENGGUNA] Mengambil detail transaksi parkir tertentu berdasarkan ID transaksi.
        /// </summary>
        /// <param name="transactionId">ID unik dari transaksi.</param>
        /// <param name="licensePlate">Plat nomor kendaraan.</param>
        /// <returns>IActionResult yang berisi detail transaksi.</returns>
        Task<IActionResult> GetUserParkingTransactionDetails(string transactionId, string licensePlate);
    }
}
