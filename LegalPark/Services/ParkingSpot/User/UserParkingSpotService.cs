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

            
            if (!string.IsNullOrWhiteSpace(filter.MerchantCode))
            {
                
                var merchant = await _merchantRepository.FindByMerchantCodeAsync(filter.MerchantCode);
                if (merchant == null) 
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", "Merchant not found with code: " + filter.MerchantCode);
                }

                ParkingSpotStatus statusFilter = ParkingSpotStatus.AVAILABLE;
                SpotType? spotTypeFilter = null; 

                if (!string.IsNullOrWhiteSpace(filter.SpotType))
                {
                    try
                    {
                        
                        spotTypeFilter = (SpotType)Enum.Parse(typeof(SpotType), filter.SpotType.ToUpper());
                    }
                    catch (ArgumentException)
                    {
                        return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid spot type: " + filter.SpotType);
                    }
                }

                
                if (spotTypeFilter != null)
                {
                    // Search by merchant, AVAILABLE status, and spot type
                    availableSpots = await _parkingSpotRepository.findByMerchantAndStatusAndSpotType(merchant, statusFilter, spotTypeFilter.Value);
                }
                else
                {
                    // Search by merchant and AVAILABLE status
                    availableSpots = await _parkingSpotRepository.findByMerchantAndStatus(merchant, statusFilter);
                }

                
                if (filter.Floor.HasValue) 
                {
                    availableSpots = availableSpots.Where(spot => spot.Floor.HasValue && spot.Floor.Value == filter.Floor.Value).ToList();
                }
            }
            else
            {
                // If there is no merchantCode, search for everything that is AVAILABLE throughout the system
                availableSpots = await _parkingSpotRepository.findByStatus(ParkingSpotStatus.AVAILABLE);

                
                if (filter.Floor.HasValue)
                {
                    availableSpots = availableSpots.Where(spot => spot.Floor.HasValue && spot.Floor.Value == filter.Floor.Value).ToList();
                }

                
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
                    
                    availableSpots = availableSpots.Where(spot => spot.SpotType == spotTypeFilter).ToList();
                }
            }

            
            var responses = availableSpots.Select(spot => _parkingSpotResponseMapper.MapToParkingSpotResponse(spot)).ToList();

            
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
