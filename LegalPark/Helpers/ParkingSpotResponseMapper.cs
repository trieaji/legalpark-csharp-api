using AutoMapper;
using LegalPark.Models.DTOs.Response.Merchant;
using LegalPark.Models.DTOs.Response.ParkingSpot;
using LegalPark.Models.Entities;

namespace LegalPark.Helpers
{
    public class ParkingSpotResponseMapper
    {
        private readonly IMapper _mapper;

        public ParkingSpotResponseMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public ParkingSpotResponse MapToParkingSpotResponse(ParkingSpot parkingSpot)
        {
            
            if (parkingSpot == null)
            {
                return null;
            }

            ParkingSpotResponse response = new ParkingSpotResponse();

            response.Id = parkingSpot.Id.ToString();
            response.SpotNumber = parkingSpot.SpotNumber;

            
            response.SpotType = parkingSpot.SpotType.ToString();
            response.Status = parkingSpot.Status.ToString();

            response.Floor = parkingSpot.Floor ?? 0; 

            
            if (parkingSpot.Merchant != null)
            {
                response.Merchant = _mapper.Map<MerchantResponse>(parkingSpot.Merchant);
            }

            return response;
        }
    }
}
