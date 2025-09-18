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
            // Periksa jika objek input null untuk menghindari NullReferenceException
            if (parkingSpot == null)
            {
                return null;
            }

            ParkingSpotResponse response = new ParkingSpotResponse();

            response.Id = parkingSpot.Id.ToString();
            response.SpotNumber = parkingSpot.SpotNumber;

            // Konversi enum ke string. Di C#, .ToString() setara dengan .name() di Java.
            response.SpotType = parkingSpot.SpotType.ToString();
            response.Status = parkingSpot.Status.ToString();

            response.Floor = parkingSpot.Floor ?? 0; // Menggunakan 0 jika floor null

            // Peta objek Merchant menggunakan AutoMapper
            if (parkingSpot.Merchant != null)
            {
                response.Merchant = _mapper.Map<MerchantResponse>(parkingSpot.Merchant);
            }

            return response;
        }
    }
}
