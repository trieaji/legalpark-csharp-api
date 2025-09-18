using AutoMapper;
using LegalPark.Models.DTOs.Response.ParkingSpot;
using LegalPark.Models.DTOs.Response.Report;
using LegalPark.Models.DTOs.Response.Vehicle;
using LegalPark.Models.Entities;

namespace LegalPark.Helpers
{
    public class ReportResponseMapper
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// Konstruktor untuk dependency injection IMapper.
        /// </summary>
        /// <param name="mapper">Instance dari AutoMapper.</param>
        public ReportResponseMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Memetakan entitas ParkingTransaction ke DTO UserParkingHistoryReportResponse.
        /// </summary>
        /// <param name="transaction">Entitas ParkingTransaction yang akan dipetakan.</param>
        /// <returns>Objek UserParkingHistoryReportResponse yang sudah dipetakan.</returns>
        public UserParkingHistoryReportResponse MapToUserParkingHistoryReportResponse(ParkingTransaction transaction)
        {
            if (transaction == null)
            {
                return null;
            }

            // Gunakan AutoMapper untuk pemetaan dasar
            var response = _mapper.Map<UserParkingHistoryReportResponse>(transaction);

            // Pemetaan properti yang membutuhkan konversi eksplisit
            response.TransactionId = transaction.Id.ToString();
            response.Status = transaction.Status.ToString();
            response.PaymentStatus = transaction.PaymentStatus.ToString();

            // Pemetaan objek bersarang (nested objects) secara manual jika diperlukan
            if (transaction.Vehicle != null)
            {
                response.Vehicle = _mapper.Map<VehicleResponse>(transaction.Vehicle);
            }

            if (transaction.ParkingSpot != null)
            {
                response.ParkingSpot = _mapper.Map<ParkingSpotResponse>(transaction.ParkingSpot);
            }

            return response;
        }
    }
}
