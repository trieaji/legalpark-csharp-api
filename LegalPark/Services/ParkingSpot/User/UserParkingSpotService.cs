using LegalPark.Exception;
using LegalPark.Helpers;
using LegalPark.Models.DTOs.Request.ParkingSpot;
using LegalPark.Models.Entities;
using LegalPark.Repositories.Merchant;
using LegalPark.Repositories.ParkingSpot;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LegalPark.Services.ParkingSpot.User
{
    public class UserParkingSpotService : IUserParkingSpotService
    {
        private readonly IParkingSpotRepository _parkingSpotRepository;
        private readonly IMerchantRepository _merchantRepository;
        private readonly ParkingSpotResponseMapper _parkingSpotResponseMapper;

        // Constructor Injection, setara dengan @Autowired di Java
        public UserParkingSpotService(
            IParkingSpotRepository parkingSpotRepository,
            IMerchantRepository merchantRepository,
            ParkingSpotResponseMapper parkingSpotResponseMapper)
        {
            _parkingSpotRepository = parkingSpotRepository;
            _merchantRepository = merchantRepository;
            _parkingSpotResponseMapper = parkingSpotResponseMapper;
        }

        public async Task<IActionResult> UserGetAvailableParkingSpotsAsync(AvailableSpotFilterRequest filter)
        {
            List<LegalPark.Models.Entities.ParkingSpot> availableSpots;

            // Logika filter:
            // C# menggunakan string.IsNullOrWhiteSpace() untuk memeriksa string null atau kosong
            if (!string.IsNullOrWhiteSpace(filter.MerchantCode))
            {
                // Menggunakan 'await' karena metode repository adalah asynchronous
                var merchant = await _merchantRepository.FindByMerchantCodeAsync(filter.MerchantCode);
                if (merchant == null) // Setara dengan .isEmpty() pada Optional di Java
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", "Merchant not found with code: " + filter.MerchantCode);
                }

                ParkingSpotStatus statusFilter = ParkingSpotStatus.AVAILABLE;
                SpotType? spotTypeFilter = null; // Menggunakan int? (nullable) setara dengan Optional<T>

                if (!string.IsNullOrWhiteSpace(filter.SpotType))
                {
                    try
                    {
                        // Konversi string ke enum, setara dengan Enum.valueOf() di Java
                        spotTypeFilter = (SpotType)Enum.Parse(typeof(SpotType), filter.SpotType.ToUpper());
                    }
                    catch (ArgumentException)
                    {
                        return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid spot type: " + filter.SpotType);
                    }
                }

                // Memanfaatkan metode repository yang spesifik
                if (spotTypeFilter != null)
                {
                    // Cari berdasarkan merchant, status AVAILABLE, dan spot type
                    availableSpots = await _parkingSpotRepository.findByMerchantAndStatusAndSpotType(merchant, statusFilter, spotTypeFilter.Value);
                }
                else
                {
                    // Cari berdasarkan merchant dan status AVAILABLE
                    availableSpots = await _parkingSpotRepository.findByMerchantAndStatus(merchant, statusFilter);
                }

                // Tambahkan filter floor jika ada.
                // Menggunakan LINQ .Where() untuk filter in-memory, setara dengan .stream().filter() di Java.
                if (filter.Floor.HasValue) // Cek apakah floor memiliki nilai, setara dengan filter.getFloor() != null di Java
                {
                    availableSpots = availableSpots.Where(spot => spot.Floor.HasValue && spot.Floor.Value == filter.Floor.Value).ToList();
                }
            }
            else
            {
                // Jika tidak ada merchantCode, cari semua yang AVAILABLE di seluruh sistem
                availableSpots = await _parkingSpotRepository.findByStatus(ParkingSpotStatus.AVAILABLE);

                // Tambahkan filter floor jika ada
                if (filter.Floor.HasValue)
                {
                    availableSpots = availableSpots.Where(spot => spot.Floor.HasValue && spot.Floor.Value == filter.Floor.Value).ToList();
                }

                // Tambahkan filter spotType jika ada
                if (!string.IsNullOrWhiteSpace(filter.SpotType))
                {
                    SpotType? spotTypeFilter = null;
                    try
                    {
                        spotTypeFilter = (SpotType)Enum.Parse(typeof(SpotType), filter.SpotType.ToUpper());
                    }
                    catch (ArgumentException)
                    {
                        return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid spot type: " + filter.SpotType);
                    }
                    // Menggunakan LINQ .Where()
                    availableSpots = availableSpots.Where(spot => spot.SpotType == spotTypeFilter).ToList();
                }
            }

            // Memetakan list entitas ke list DTO
            var responses = availableSpots.Select(spot => _parkingSpotResponseMapper.MapToParkingSpotResponse(spot)).ToList();

            // Mengembalikan respons sukses
            return ResponseHandler.GenerateResponseSuccess(responses);
        }

        public async Task<IActionResult> UserGetParkingSpotsByMerchantAsync(string merchantCode)
        {
            var merchant = await _merchantRepository.FindByMerchantCodeAsync(merchantCode);
            if (merchant == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", "Merchant not found with code: " + merchantCode);
            }

            var parkingSpots = await _parkingSpotRepository.findByMerchant(merchant);
            var responses = parkingSpots.Select(spot => _parkingSpotResponseMapper.MapToParkingSpotResponse(spot)).ToList();

            return ResponseHandler.GenerateResponseSuccess(responses);
        }
    }
}
