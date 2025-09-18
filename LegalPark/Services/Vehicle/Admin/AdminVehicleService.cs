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

        /// <summary>
        /// Konstruktor untuk dependency injection.
        /// Menginjeksi repository yang diperlukan dan mapper untuk DTO.
        /// </summary>
        /// <param name="vehicleRepository">Repository untuk entitas Vehicle.</param>
        /// <param name="userRepository">Repository untuk entitas User.</param>
        /// <param name="vehicleResponseMapper">Helper untuk memetakan Vehicle ke VehicleResponse.</param>
        public AdminVehicleService(
            IVehicleRepository vehicleRepository,
            IUserRepository userRepository,
            VehicleResponseMapper vehicleResponseMapper)
        {
            _vehicleRepository = vehicleRepository;
            _userRepository = userRepository;
            _vehicleResponseMapper = vehicleResponseMapper;
        }

        /// <summary>
        /// Mendaftarkan kendaraan baru atas nama pengguna tertentu.
        /// </summary>
        /// <param name="request">Objek VehicleRequest yang berisi data kendaraan.</param>
        /// <returns>Respons HTTP sukses dengan data kendaraan terdaftar atau respons kesalahan.</returns>
        public async Task<IActionResult> AdminRegisterVehicle(VehicleRequest request)
        {
            // Validasi ownerId
            if (string.IsNullOrEmpty(request.OwnerId))
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Owner ID is required for admin to create vehicle.");
            }

            // Mencari pemilik kendaraan berdasarkan ID
            if (!Guid.TryParse(request.OwnerId, out Guid ownerId))
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid Owner ID format.");
            }

            var owner = await _userRepository.GetByIdAsync(ownerId);
            if (owner == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", "Owner not found with ID: " + request.OwnerId);
            }

            // Cek duplikasi plat nomor
            var existingVehicle = await _vehicleRepository.findByLicensePlate(request.LicensePlate);
            if (existingVehicle != null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.Conflict, "FAILED", "Vehicle with this license plate is already registered.");
            }

            // Konversi string Tipe kendaraan menjadi enum VehicleType
            if (!Enum.TryParse(request.Type, true, out VehicleType vehicleType))
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED",
                    "Invalid vehicle type provided: " + request.Type + ". Allowed types: " +
                    string.Join(", ", Enum.GetNames(typeof(VehicleType))));
            }

            var vehicle = new LegalPark.Models.Entities.Vehicle
            {
                Id = Guid.NewGuid(), // Gunakan GUID baru
                LicensePlate = request.LicensePlate,
                Type = vehicleType,
                OwnerId = owner.Id, // Menggunakan ID pemilik yang ditemukan
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _vehicleRepository.AddAsync(vehicle);
            await _vehicleRepository.SaveChangesAsync();

            // Memuat ulang objek kendaraan dengan properti navigasi (Owner) yang sudah diisi
            // Ini diperlukan karena AddAsync tidak secara otomatis memuat properti navigasi.
            vehicle.Owner = owner;

            var response = _vehicleResponseMapper.MapToVehicleResponse(vehicle);
            return ResponseHandler.GenerateResponseSuccess(response);
        }

        /// <summary>
        /// Mengambil semua kendaraan yang terdaftar.
        /// </summary>
        /// <returns>Respons HTTP sukses dengan daftar semua kendaraan.</returns>
        public async Task<IActionResult> AdminGetAllVehicles()
        {
            var vehicles = await _vehicleRepository.GetAllWithDetailsAsync();
            var responses = vehicles.Select(v => _vehicleResponseMapper.MapToVehicleResponse(v)).ToList();
            return ResponseHandler.GenerateResponseSuccess(responses);
        }

        /// <summary>
        /// Mengambil data kendaraan berdasarkan ID.
        /// </summary>
        /// <param name="id">ID kendaraan.</param>
        /// <returns>Respons HTTP sukses dengan data kendaraan atau respons kesalahan jika tidak ditemukan.</returns>
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

        /// <summary>
        /// Mengambil daftar kendaraan milik pengguna tertentu.
        /// </summary>
        /// <param name="userId">ID pengguna.</param>
        /// <returns>Respons HTTP sukses dengan daftar kendaraan pengguna atau respons kesalahan.</returns>
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

        /// <summary>
        /// Menghapus kendaraan berdasarkan ID.
        /// </summary>
        /// <param name="id">ID kendaraan yang akan dihapus.</param>
        /// <returns>Respons HTTP sukses jika berhasil dihapus atau respons kesalahan jika tidak ditemukan.</returns>
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
