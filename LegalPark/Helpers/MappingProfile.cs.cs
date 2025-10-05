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
            
            CreateMap<Merchant, MerchantResponse>();
            CreateMap<Vehicle, VehicleResponse>();

            
            CreateMap<ParkingSpot, ParkingSpotResponse>();

            
            CreateMap<ParkingTransaction, UserParkingHistoryReportResponse>()
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus.ToString()))
                .ForMember(dest => dest.TotalCost, opt => opt.MapFrom(src => src.TotalCost ?? 0));

            
            CreateMap<Vehicle, VehicleResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

            
            CreateMap<User, UserBasicResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
        }
    }
}
