using LegalPark.Util;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Services.Payment
{
    public interface IPaymentService
    {
        Task<PaymentResult> ProcessParkingPayment(string userId, decimal amount, string parkingTransactionId, string verificationCode);
    }
}
