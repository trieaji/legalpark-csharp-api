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

        // Gunakan Constructor Injection untuk memasukkan dependencies.
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
            // Cek pengguna saat ini
            var currentUser = await _infoAccount.GetAsync();
            if (currentUser == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.Unauthorized, "FAILED", "User not authenticated. Please log in.");
            }

            // Cek status akun pengguna
            if (currentUser.AccountStatus != AccountStatus.ACTIVE)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.Forbidden, "FAILED", "Account is not active. Please verify your email first.");
            }

            // Periksa apakah plat nomor sudah ada
            var existingVehicle = await _vehicleRepository.findByLicensePlate(request.LicensePlate);
            if (existingVehicle != null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.Conflict, "FAILED", "Vehicle with this license plate is already registered.");
            }

            // Konversi String dari request menjadi VehicleType enum
            if (!Enum.TryParse(request.Type, true, out VehicleType vehicleType))
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED",
                    $"Invalid vehicle type provided: {request.Type}. Allowed types: {string.Join(", ", Enum.GetNames(typeof(VehicleType)))}");
            }

            // Buat entitas Vehicle baru
            var newVehicle = new LegalPark.Models.Entities.Vehicle
            {
                LicensePlate = request.LicensePlate,
                Type = vehicleType,
                OwnerId = currentUser.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Simpan ke database
            await _vehicleRepository.AddAsync(newVehicle);
            await _vehicleRepository.SaveChangesAsync();

            // Memuat ulang kendaraan dengan properti navigasi Owner
            // Asumsi VehicleRepository.GetByIdAsync memuat properti navigasi.
            var savedVehicle = await _vehicleRepository.GetByIdAsync(newVehicle.Id);

            // Konversi entitas yang disimpan ke DTO Response
            var response = _vehicleResponseMapper.MapToVehicleResponse(savedVehicle);

            return ResponseHandler.GenerateResponseSuccess(response);
        }

        public async Task<IActionResult> UserGetAllVehicle()
        {
            // Ambil semua kendaraan
            var vehicles = await _vehicleRepository.GetAllWithDetailsAsync();

            // Gunakan LINQ untuk memetakan ke VehicleResponse DTO
            var responses = vehicles.Select(v => _vehicleResponseMapper.MapToVehicleResponse(v)).ToList();

            return ResponseHandler.GenerateResponseSuccess(responses);
        }

        public async Task<IActionResult> UserGetVehicleById(string id)
        {
            if (!Guid.TryParse(id, out Guid vehicleId))
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid ID format.");
            }

            // Cari kendaraan berdasarkan ID
            var vehicle = await _vehicleRepository.GetByIdWithDetailsAsync(vehicleId);

            // Jika kendaraan tidak ditemukan
            if (vehicle == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Vehicle not found with ID: {id}");
            }

            // Konversi ke VehicleResponse dan kembalikan
            return ResponseHandler.GenerateResponseSuccess(_vehicleResponseMapper.MapToVehicleResponse(vehicle));
        }

        public async Task<IActionResult> UserUpdateVehicle(string id, VehicleUpdateRequest request)
        {
            // Cek pengguna saat ini
            var currentUser = await _infoAccount.GetAsync();
            if (currentUser == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.Unauthorized, "FAILED", "User not authenticated. Please log in.");
            }

            // Cek status akun pengguna
            if (currentUser.AccountStatus != AccountStatus.ACTIVE)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.Forbidden, "FAILED", "Account is not active. Please verify your email first.");
            }

            if (!Guid.TryParse(id, out Guid vehicleId))
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid ID format.");
            }

            // Ambil entitas Vehicle yang sudah ada dari repository
            var existingVehicle = await _vehicleRepository.GetByIdAsync(vehicleId);

            // Jika vehicle tidak ditemukan, kembalikan error NOT_FOUND
            if (existingVehicle == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Vehicle with ID {id} not found.");
            }

            // Cek otorisasi: pastikan pengguna saat ini adalah pemilik kendaraan
            if (existingVehicle.OwnerId != currentUser.Id)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.Forbidden, "FAILED", "You are not authorized to update this vehicle.");
            }

            // Perbarui properti 'Type' jika ada dalam request
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

            // Perbarui properti 'LicensePlate' jika ada dalam request
            if (request.LicensePlate != null)
            {
                // Cek apakah plat nomor sudah ada untuk pengguna lain
                var duplicateVehicle = await _vehicleRepository.findByLicensePlate(request.LicensePlate);
                if (duplicateVehicle != null && duplicateVehicle.Id != existingVehicle.Id)
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.Conflict, "FAILED", "Vehicle with this license plate is already registered.");
                }
                existingVehicle.LicensePlate = request.LicensePlate;
            }

            // Perbarui properti UpdatedAt
            existingVehicle.UpdatedAt = DateTime.UtcNow;

            // Simpan perubahan
            _vehicleRepository.Update(existingVehicle);
            await _vehicleRepository.SaveChangesAsync();

            // Konversi ke DTO Response dan kembalikan
            var response = _vehicleResponseMapper.MapToVehicleResponse(existingVehicle);

            return ResponseHandler.GenerateResponseSuccess(response);
        }
    }
}
