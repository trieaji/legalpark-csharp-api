using LegalPark.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Services.ParkingTransaction.Admin
{
    public interface IAdminParkingTransactionService
    {
        Task<IActionResult> AdminGetAllParkingTransactions();
        Task<IActionResult> AdminGetParkingTransactionById(Guid transactionId);
        Task<IActionResult> AdminGetParkingTransactionsByVehicleId(Guid vehicleId);
        Task<IActionResult> AdminGetParkingTransactionsByParkingSpotId(Guid parkingSpotId);
        Task<IActionResult> AdminGetParkingTransactionsByMerchantId(Guid merchantId);
        Task<IActionResult> AdminGetParkingTransactionsByParkingStatus(ParkingStatus status);
        Task<IActionResult> AdminGetParkingTransactionsByPaymentStatus(PaymentStatus paymentStatus);
        Task<IActionResult> AdminUpdateParkingTransactionPaymentStatus(Guid transactionId, PaymentStatus newPaymentStatus);
        Task<IActionResult> AdminCancelParkingTransaction(Guid transactionId);
    }
}
