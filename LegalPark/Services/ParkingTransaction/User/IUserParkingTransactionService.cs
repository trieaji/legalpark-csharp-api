using LegalPark.Models.DTOs.Request.ParkingTransaction;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Services.ParkingTransaction.User
{
    public interface IUserParkingTransactionService
    {

        /// [USER] Records the entry of vehicles into parking slots.
        Task<IActionResult> RecordParkingEntry(ParkingEntryRequest request);

        
        /// [USER] Records vehicles leaving parking slots and processes payments.
        Task<IActionResult> RecordParkingExit(ParkingExitRequest request);


        /// [USER] Retrieves active parking transactions for the user's vehicle license plate number.
        Task<IActionResult> GetUserActiveParkingTransaction(string licensePlate);


        /// [USER] Retrieves the history of all parking transactions associated with the user's vehicle license plate number.
        Task<IActionResult> GetUserParkingTransactionHistory(string licensePlate);


        /// [USER] Retrieves specific parking transaction details based on the transaction ID.
        Task<IActionResult> GetUserParkingTransactionDetails(string transactionId, string licensePlate);
    }
}
