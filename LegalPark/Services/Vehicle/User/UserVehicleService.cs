using LegalPark.Exception;
using LegalPark.Helpers;
using LegalPark.Models.DTOs.Request.Vehicle;
using LegalPark.Models.Entities;
using LegalPark.Repositories.User;
using LegalPark.Repositories.Vehicle;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LegalPark.Services.Vehicle.User
{
    public class UserVehicleService : IUserVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IUserRepository _userRepository;
        private readonly InfoAccount _infoAccount;
        private readonly VehicleResponseMapper _vehicleResponseMapper;

        
        public UserVehicleService(
            IVehicleRepository vehicleRepository,
            IUserRepository userRepository,
            InfoAccount infoAccount,
            VehicleResponseMapper vehicleResponseMapper)
        {
            _vehicleRepository = vehicleRepository;
            _userRepository = userRepository;
            _infoAccount = infoAccount;
            _vehicleResponseMapper = vehicleResponseMapper;
        }

        public async Task<IActionResult> UserRegisterVehicle(VehicleRequest request)
        {
            
            var currentUser = await _infoAccount.GetAsync();
            if (currentUser == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.Unauthorized, "FAILED", "User not authenticated. Please log in.");
            }

            
            if (currentUser.AccountStatus != AccountStatus.ACTIVE)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.Forbidden, "FAILED", "Account is not active. Please verify your email first.");
            }

            
            var existingVehicle = await _vehicleRepository.findByLicensePlate(request.LicensePlate);
            if (existingVehicle != null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.Conflict, "FAILED", "Vehicle with this license plate is already registered.");
            }

            
            if (!Enum.TryParse(request.Type, true, out VehicleType vehicleType))
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED",
                    $"Invalid vehicle type provided: {request.Type}. Allowed types: {string.Join(", ", Enum.GetNames(typeof(VehicleType)))}");
            }

            
            var newVehicle = new LegalPark.Models.Entities.Vehicle
            {
                LicensePlate = request.LicensePlate,
                Type = vehicleType,
                OwnerId = currentUser.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            
            await _vehicleRepository.AddAsync(newVehicle);
            await _vehicleRepository.SaveChangesAsync();

            
            var savedVehicle = await _vehicleRepository.GetByIdAsync(newVehicle.Id);

            
            var response = _vehicleResponseMapper.MapToVehicleResponse(savedVehicle);

            return ResponseHandler.GenerateResponseSuccess(response);
        }

        public async Task<IActionResult> UserGetAllVehicle()
        {
            
            var vehicles = await _vehicleRepository.GetAllWithDetailsAsync();

            
            var responses = vehicles.Select(v => _vehicleResponseMapper.MapToVehicleResponse(v)).ToList();

            return ResponseHandler.GenerateResponseSuccess(responses);
        }

        public async Task<IActionResult> UserGetVehicleById(string id)
        {
            if (!Guid.TryParse(id, out Guid vehicleId))
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid ID format.");
            }

            
            var vehicle = await _vehicleRepository.GetByIdWithDetailsAsync(vehicleId);

            
            if (vehicle == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Vehicle not found with ID: {id}");
            }

            
            return ResponseHandler.GenerateResponseSuccess(_vehicleResponseMapper.MapToVehicleResponse(vehicle));
        }

        public async Task<IActionResult> UserUpdateVehicle(string id, VehicleUpdateRequest request)
        {
            
            var currentUser = await _infoAccount.GetAsync();
            if (currentUser == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.Unauthorized, "FAILED", "User not authenticated. Please log in.");
            }

            
            if (currentUser.AccountStatus != AccountStatus.ACTIVE)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.Forbidden, "FAILED", "Account is not active. Please verify your email first.");
            }

            if (!Guid.TryParse(id, out Guid vehicleId))
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid ID format.");
            }

            
            var existingVehicle = await _vehicleRepository.GetByIdAsync(vehicleId);

            
            if (existingVehicle == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Vehicle with ID {id} not found.");
            }

            
            if (existingVehicle.OwnerId != currentUser.Id)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.Forbidden, "FAILED", "You are not authorized to update this vehicle.");
            }

            
            if (request.Type != null)
            {
                if (Enum.TryParse(request.Type, true, out VehicleType newType))
                {
                    existingVehicle.Type = newType;
                }
                else
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", $"Invalid vehicle type provided: {request.Type}.");
                }
            }

            
            if (request.LicensePlate != null)
            {
                
                var duplicateVehicle = await _vehicleRepository.findByLicensePlate(request.LicensePlate);
                if (duplicateVehicle != null && duplicateVehicle.Id != existingVehicle.Id)
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.Conflict, "FAILED", "Vehicle with this license plate is already registered.");
                }
                existingVehicle.LicensePlate = request.LicensePlate;
            }

            
            existingVehicle.UpdatedAt = DateTime.UtcNow;

            
            _vehicleRepository.Update(existingVehicle);
            await _vehicleRepository.SaveChangesAsync();

            
            var response = _vehicleResponseMapper.MapToVehicleResponse(existingVehicle);

            return ResponseHandler.GenerateResponseSuccess(response);
        }
    }
}
