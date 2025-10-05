using LegalPark.Exception;
using LegalPark.Helpers;
using LegalPark.Models.DTOs.Request.Vehicle;
using LegalPark.Models.Entities;
using LegalPark.Repositories.User;
using LegalPark.Repositories.Vehicle;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LegalPark.Services.Vehicle.Admin
{
    public class AdminVehicleService : IAdminVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IUserRepository _userRepository;
        private readonly VehicleResponseMapper _vehicleResponseMapper;

        
        public AdminVehicleService(
            IVehicleRepository vehicleRepository,
            IUserRepository userRepository,
            VehicleResponseMapper vehicleResponseMapper)
        {
            _vehicleRepository = vehicleRepository;
            _userRepository = userRepository;
            _vehicleResponseMapper = vehicleResponseMapper;
        }

        
        public async Task<IActionResult> AdminRegisterVehicle(VehicleRequest request)
        {
            
            if (string.IsNullOrEmpty(request.OwnerId))
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Owner ID is required for admin to create vehicle.");
            }

            
            if (!Guid.TryParse(request.OwnerId, out Guid ownerId))
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid Owner ID format.");
            }

            var owner = await _userRepository.GetByIdAsync(ownerId);
            if (owner == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", "Owner not found with ID: " + request.OwnerId);
            }

            
            var existingVehicle = await _vehicleRepository.findByLicensePlate(request.LicensePlate);
            if (existingVehicle != null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.Conflict, "FAILED", "Vehicle with this license plate is already registered.");
            }

            // Convert the VehicleType string to an enum VehicleType
            if (!Enum.TryParse(request.Type, true, out VehicleType vehicleType))
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED",
                    "Invalid vehicle type provided: " + request.Type + ". Allowed types: " +
                    string.Join(", ", Enum.GetNames(typeof(VehicleType))));
            }

            var vehicle = new LegalPark.Models.Entities.Vehicle
            {
                Id = Guid.NewGuid(), 
                LicensePlate = request.LicensePlate,
                Type = vehicleType,
                OwnerId = owner.Id, 
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _vehicleRepository.AddAsync(vehicle);
            await _vehicleRepository.SaveChangesAsync();

            // Reload the vehicle object with the navigation property (Owner) already filled in.
            // This is necessary because AddAsync does not automatically load the navigation property.
            vehicle.Owner = owner;

            var response = _vehicleResponseMapper.MapToVehicleResponse(vehicle);
            return ResponseHandler.GenerateResponseSuccess(response);
        }

       
        public async Task<IActionResult> AdminGetAllVehicles()
        {
            var vehicles = await _vehicleRepository.GetAllWithDetailsAsync();
            var responses = vehicles.Select(v => _vehicleResponseMapper.MapToVehicleResponse(v)).ToList();
            return ResponseHandler.GenerateResponseSuccess(responses);
        }

        
        public async Task<IActionResult> AdminGetVehicleById(string id)
        {
            if (!Guid.TryParse(id, out Guid vehicleId))
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid Vehicle ID format.");
            }

            var vehicle = await _vehicleRepository.GetByIdWithDetailsAsync(vehicleId);
            if (vehicle == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", "Vehicle not found with ID: " + id);
            }

            var response = _vehicleResponseMapper.MapToVehicleResponse(vehicle);
            return ResponseHandler.GenerateResponseSuccess(response);
        }

        
        public async Task<IActionResult> AdminGetVehiclesByUserId(string userId)
        {
            if (!Guid.TryParse(userId, out Guid ownerId))
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid User ID format.");
            }

            var user = await _userRepository.GetByIdAsync(ownerId);
            if (user == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", "User not found with ID: " + userId);
            }

            var vehicles = await _vehicleRepository.findByOwner(user);
            var responses = vehicles.Select(v => _vehicleResponseMapper.MapToVehicleResponse(v)).ToList();
            return ResponseHandler.GenerateResponseSuccess(responses);
        }

        
        public async Task<IActionResult> AdminDeleteVehicle(string id)
        {
            if (!Guid.TryParse(id, out Guid vehicleId))
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid Vehicle ID format.");
            }

            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
            if (vehicle == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", "Vehicle not found with ID: " + id);
            }

            _vehicleRepository.Delete(vehicle);
            await _vehicleRepository.SaveChangesAsync();

            return ResponseHandler.GenerateResponseSuccess("Vehicle with ID " + id + " has been deleted successfully.");
        }
    }
}
