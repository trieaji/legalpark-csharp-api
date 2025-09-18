using LegalPark.Exception;
using LegalPark.Helpers;
using LegalPark.Models.DTOs.Request.ParkingSpot;
using LegalPark.Models.Entities;
using LegalPark.Repositories.Merchant;
using LegalPark.Repositories.ParkingSpot;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LegalPark.Services.ParkingSpot.Admin
{
    public class AdminParkingSpotService : IAdminParkingSpotService
    {
        private readonly IParkingSpotRepository _parkingSpotRepository;
        private readonly IMerchantRepository _merchantRepository;
        private readonly ParkingSpotResponseMapper _parkingSpotResponseMapper;

        // Gunakan Constructor Injection untuk memasukkan dependencies.
        public AdminParkingSpotService(
            IParkingSpotRepository parkingSpotRepository,
            IMerchantRepository merchantRepository,
            ParkingSpotResponseMapper parkingSpotResponseMapper)
        {
            _parkingSpotRepository = parkingSpotRepository;
            _merchantRepository = merchantRepository;
            _parkingSpotResponseMapper = parkingSpotResponseMapper;
        }

        public async Task<IActionResult> AdminCreateParkingSpot(ParkingSpotRequest request)
        {
            // 1. Cari Merchant berdasarkan merchantCode
            var merchant = await _merchantRepository.FindByMerchantCodeAsync(request.MerchantCode);
            if (merchant == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Merchant not found with code: {request.MerchantCode}");
            }

            // 2. Cek apakah spotNumber sudah ada di merchant yang sama
            var existingSpot = await _parkingSpotRepository.findBySpotNumberAndMerchant(request.SpotNumber, merchant);
            if (existingSpot != null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.Conflict, "FAILED", $"Parking spot with number '{request.SpotNumber}' already exists for this merchant.");
            }

            // 3. Konversi DTO ke Entity
            var parkingSpot = new Models.Entities.ParkingSpot
            {
                SpotNumber = request.SpotNumber,
                Floor = request.Floor,
                MerchantId = merchant.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Konversi String ke Enum SpotType
            if (Enum.TryParse(request.SpotType, true, out SpotType spotType))
            {
                parkingSpot.SpotType = spotType;
            }
            else
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", $"Invalid spot type: {request.SpotType}");
            }

            // Set status awal sebagai AVAILABLE
            parkingSpot.Status = ParkingSpotStatus.AVAILABLE;

            // 4. Tambahkan ke database
            await _parkingSpotRepository.AddAsync(parkingSpot);

            //5. Simpan ke database
            await _parkingSpotRepository.SaveChangesAsync();

            // Setelah AddAsync, objek 'parkingSpot' sudah diperbarui dengan ID dari database.
            // Pastikan entitas memiliki properti navigasi `Merchant` dimuat.
            parkingSpot.Merchant = merchant;

            // 6. Konversi Entity yang disimpan ke DTO Response
            var response = _parkingSpotResponseMapper.MapToParkingSpotResponse(parkingSpot);
            return ResponseHandler.GenerateResponseSuccess(response);
        }

        public async Task<IActionResult> AdminGetAllParkingSpots()
        {
            var parkingSpots = await _parkingSpotRepository.GetAllAsync();
            var responses = parkingSpots.Select(ps => _parkingSpotResponseMapper.MapToParkingSpotResponse(ps)).ToList();
            return ResponseHandler.GenerateResponseSuccess(responses);
        }

        public async Task<IActionResult> AdminGetParkingSpotById(string id)
        {
            if (!Guid.TryParse(id, out Guid parkingSpotId))
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid ID format.");
            }

            var parkingSpot = await _parkingSpotRepository.GetByIdAsync(parkingSpotId);
            if (parkingSpot == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Parking spot not found with ID: {id}");
            }


            var response = _parkingSpotResponseMapper.MapToParkingSpotResponse(parkingSpot);
            return ResponseHandler.GenerateResponseSuccess(response);
        }

        public async Task<IActionResult> AdminUpdateParkingSpot(string id, ParkingSpotUpdateRequest request)
        {
            if (!Guid.TryParse(id, out Guid parkingSpotId))
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid ID format.");
            }

            var parkingSpot = await _parkingSpotRepository.GetByIdAsync(parkingSpotId);
            if (parkingSpot == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Parking spot not found with ID: {id}");
            }

            // Perbarui field yang tidak null di request
            if (request.SpotNumber != null)
            {
                // Jika spotNumber diubah, cek keunikan dengan merchant saat ini
                var existingSpot = await _parkingSpotRepository.findBySpotNumberAndMerchant(request.SpotNumber, parkingSpot.Merchant);
                if (existingSpot != null && existingSpot.Id != parkingSpotId)
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.Conflict, "FAILED", $"Parking spot with number '{request.SpotNumber}' already exists for this merchant.");
                }
                parkingSpot.SpotNumber = request.SpotNumber;
            }

            if (request.SpotType != null)
            {
                if (Enum.TryParse(request.SpotType, true, out SpotType spotType))
                {
                    parkingSpot.SpotType = spotType;
                }
                else
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", $"Invalid spot type: {request.SpotType}");
                }
            }

            if (request.Status != null)
            {
                if (Enum.TryParse(request.Status, true, out ParkingSpotStatus status))
                {
                    parkingSpot.Status = status;
                }
                else
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", $"Invalid status: {request.Status}");
                }
            }

            if (request.Floor.HasValue)
            {
                parkingSpot.Floor = request.Floor;
            }

            // Cek jika merchantCode diubah
            if (request.MerchantCode != null && !string.Equals(request.MerchantCode, parkingSpot.Merchant?.MerchantCode, StringComparison.OrdinalIgnoreCase))
            {
                var newMerchant = await _merchantRepository.FindByMerchantCodeAsync(request.MerchantCode);
                if (newMerchant == null)
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"New Merchant not found with code: {request.MerchantCode}");
                }

                string spotNumberToCheck = request.SpotNumber ?? parkingSpot.SpotNumber;
                var existingSpotInNewMerchant = await _parkingSpotRepository.findBySpotNumberAndMerchant(spotNumberToCheck, newMerchant);

                if (existingSpotInNewMerchant != null)
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.Conflict, "FAILED", $"Parking spot with number '{spotNumberToCheck}' already exists for the new merchant.");
                }

                parkingSpot.MerchantId = newMerchant.Id;
            }

            parkingSpot.UpdatedAt = DateTime.UtcNow;

            // Simpan perubahan
            _parkingSpotRepository.Update(parkingSpot);

            await _parkingSpotRepository.SaveChangesAsync();

            var response = _parkingSpotResponseMapper.MapToParkingSpotResponse(parkingSpot);
            return ResponseHandler.GenerateResponseSuccess(response);
        }

        public async Task<IActionResult> AdminDeleteParkingSpot(string id)
        {
            if (!Guid.TryParse(id, out Guid parkingSpotId))
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid ID format.");
            }

            var parkingSpot = await _parkingSpotRepository.GetByIdAsync(parkingSpotId);
            if (parkingSpot == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Parking spot not found with ID: {id}");
            }

            _parkingSpotRepository.Delete(parkingSpot);

            await _parkingSpotRepository.SaveChangesAsync();

            return ResponseHandler.GenerateResponseSuccess(null);
        }

        public async Task<IActionResult> AdminGetParkingSpotsByMerchant(string merchantIdentifier)
        {
            var merchant = await _merchantRepository.FindByMerchantCodeAsync(merchantIdentifier);
            if (merchant == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Merchant not found with identifier: {merchantIdentifier}");
            }

            var parkingSpots = await _parkingSpotRepository.findByMerchant(merchant);
            var responses = parkingSpots.Select(ps => _parkingSpotResponseMapper.MapToParkingSpotResponse(ps)).ToList();
            return ResponseHandler.GenerateResponseSuccess(responses);
        }
    }
}
