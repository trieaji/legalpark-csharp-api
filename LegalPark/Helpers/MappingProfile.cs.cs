using AutoMapper;
using LegalPark.Models.DTOs.Response.Merchant;
using LegalPark.Models.DTOs.Response.ParkingSpot;
using LegalPark.Models.DTOs.Response.Report;
using LegalPark.Models.DTOs.Response.User;
using LegalPark.Models.DTOs.Response.Vehicle;
using LegalPark.Models.Entities;

namespace LegalPark.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Peta entitas Merchant ke DTO MerchantResponse
            CreateMap<Merchant, MerchantResponse>();
            CreateMap<Vehicle, VehicleResponse>();

            // Pemetaan entitas ParkingSpot. 
            // Perhatikan bahwa properti "Floor" di DTO adalah int, 
            // sementara di entitas adalah int?. AutoMapper akan menangani ini.
            CreateMap<ParkingSpot, ParkingSpotResponse>();

            // Pemetaan entitas ParkingTransaction ke DTO UserParkingHistoryReportResponse.
            // AutoMapper secara otomatis akan mencoba memetakan properti 
            // dengan nama yang sama, seperti EntryTime, ExitTime, dll.
            CreateMap<ParkingTransaction, UserParkingHistoryReportResponse>()
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus.ToString()))
                .ForMember(dest => dest.TotalCost, opt => opt.MapFrom(src => src.TotalCost ?? 0));

            // Peta entitas Vehicle ke DTO VehicleResponse
            CreateMap<Vehicle, VehicleResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

            // Peta entitas User ke DTO UserBasicResponse
            CreateMap<User, UserBasicResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
        }
    }
}
