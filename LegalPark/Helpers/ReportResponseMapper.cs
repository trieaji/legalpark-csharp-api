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

        
        public ReportResponseMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        
        public UserParkingHistoryReportResponse MapToUserParkingHistoryReportResponse(ParkingTransaction transaction)
        {
            if (transaction == null)
            {
                return null;
            }

            
            var response = _mapper.Map<UserParkingHistoryReportResponse>(transaction);

            
            response.TransactionId = transaction.Id.ToString();
            response.Status = transaction.Status.ToString();
            response.PaymentStatus = transaction.PaymentStatus.ToString();

            
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
