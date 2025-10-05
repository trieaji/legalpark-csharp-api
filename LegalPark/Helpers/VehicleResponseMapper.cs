using AutoMapper;
using LegalPark.Models.DTOs.Response.User;
using LegalPark.Models.DTOs.Response.Vehicle;
using LegalPark.Models.Entities;

namespace LegalPark.Helpers
{
    public class VehicleResponseMapper
    {
        private readonly IMapper _mapper;

        
        public VehicleResponseMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        
        public VehicleResponse MapToVehicleResponse(Vehicle vehicle)
        {
            if (vehicle == null)
            {
                return null;
            }

            VehicleResponse response = new VehicleResponse();

            response.Id = vehicle.Id.ToString();
            response.LicensePlate = vehicle.LicensePlate;

            
            response.Type = vehicle.Type.ToString();

            
            if (vehicle.Owner != null)
            {
                response.Owner = _mapper.Map<UserBasicResponse>(vehicle.Owner);
            }

            return response;
        }
    }
}
