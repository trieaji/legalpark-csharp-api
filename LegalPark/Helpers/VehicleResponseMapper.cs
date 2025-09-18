using AutoMapper;
using LegalPark.Models.DTOs.Response.User;
using LegalPark.Models.DTOs.Response.Vehicle;
using LegalPark.Models.Entities;

namespace LegalPark.Helpers
{
    public class VehicleResponseMapper
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// Konstruktor untuk inisialisasi VehicleResponseMapper dengan dependency injection.
        /// </summary>
        /// <param name="mapper">Instance dari AutoMapper.</param>
        public VehicleResponseMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Memetakan entitas Vehicle ke DTO VehicleResponse.
        /// </summary>
        /// <param name="vehicle">Entitas Vehicle yang akan dipetakan.</param>
        /// <returns>Objek VehicleResponse yang sudah dipetakan.</returns>
        public VehicleResponse MapToVehicleResponse(Vehicle vehicle)
        {
            if (vehicle == null)
            {
                return null;
            }

            VehicleResponse response = new VehicleResponse();

            response.Id = vehicle.Id.ToString();
            response.LicensePlate = vehicle.LicensePlate;

            // Konversi enum VehicleType ke string
            response.Type = vehicle.Type.ToString();

            // Peta objek Owner DTO menggunakan AutoMapper
            if (vehicle.Owner != null)
            {
                response.Owner = _mapper.Map<UserBasicResponse>(vehicle.Owner);
            }

            return response;
        }
    }
}
