using AutoMapper;
using LegalPark.Models.DTOs.Response.Merchant;
using LegalPark.Models.DTOs.Response.ParkingSpot;
using LegalPark.Models.DTOs.Response.ParkingTransaction;
using LegalPark.Models.DTOs.Response.Vehicle;
using LegalPark.Models.Entities;

namespace LegalPark.Helpers
{
    public class ParkingTransactionResponseMapper
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// Konstruktor untuk inisialisasi ParkingTransactionResponseMapper dengan dependency injection.
        /// </summary>
        /// <param name="mapper">Instance dari AutoMapper.</param>
        public ParkingTransactionResponseMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Memetakan entitas ParkingTransaction ke DTO ParkingTransactionResponse.
        /// </summary>
        /// <param name="transaction">Entitas ParkingTransaction yang akan dipetakan.</param>
        /// <returns>Objek ParkingTransactionResponse yang sudah dipetakan.</returns>
        public ParkingTransactionResponse MapToParkingTransactionResponse(ParkingTransaction transaction)
        {
            if (transaction == null)
            {
                return null;
            }

            ParkingTransactionResponse response = new ParkingTransactionResponse();

            response.Id = transaction.Id.ToString();
            response.EntryTime = transaction.EntryTime;
            response.ExitTime = transaction.ExitTime;
            response.TotalCost = transaction.TotalCost ?? 0;
            response.Status = transaction.Status.ToString();
            response.PaymentStatus = transaction.PaymentStatus.ToString();
            response.CreatedAt = transaction.CreatedAt;
            response.UpdatedAt = transaction.UpdatedAt;

            // Memetakan Vehicle DTO menggunakan AutoMapper
            if (transaction.Vehicle != null)
            {
                response.Vehicle = _mapper.Map<VehicleResponse>(transaction.Vehicle);
            }

            // Memetakan ParkingSpot DTO secara manual
            if (transaction.ParkingSpot != null)
            {
                ParkingSpotResponse spotResponse = new ParkingSpotResponse();
                spotResponse.Id = transaction.ParkingSpot.Id.ToString();
                spotResponse.SpotNumber = transaction.ParkingSpot.SpotNumber;
                spotResponse.SpotType = transaction.ParkingSpot.SpotType.ToString();
                spotResponse.Status = transaction.ParkingSpot.Status.ToString();

                // Properti floor adalah int? di entitas, jadi perlu ditangani
                spotResponse.Floor = transaction.ParkingSpot.Floor ?? 0;

                // Memetakan Merchant DTO menggunakan AutoMapper
                if (transaction.ParkingSpot.Merchant != null)
                {
                    spotResponse.Merchant = _mapper.Map<MerchantResponse>(transaction.ParkingSpot.Merchant);
                }
                response.ParkingSpot = spotResponse;
            }

            return response;
        }
    }
}
