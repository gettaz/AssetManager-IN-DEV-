using AssetManager.DTO;
using AssetManager.Models;
using AutoMapper;

namespace AssetManager.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Asset, AssetDto>()
                .ForMember(dest => dest.AssetName, opt => opt.MapFrom(src => src.AssetName))
                .ForMember(dest => dest.Ticker, opt => opt.MapFrom(src => src.Ticker))
                .ForMember(dest => dest.PurchasePrice, opt => opt.MapFrom(src => src.PriceBought))
                .ForMember(dest => dest.BrokerName, opt => opt.MapFrom(src => src.Broker.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.DateBought, opt => opt.MapFrom(src => src.DateBought))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));

            CreateMap<AssetDto, Asset>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.PriceBought, opt => opt.MapFrom(src => src.PurchasePrice))
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
                .ForMember(dest => dest.BrokerId, opt => opt.Ignore())
                .ForMember(dest => dest.Broker, opt => opt.Ignore())
                .ForMember(dest => dest.DateSold, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore());

            CreateMap<Category, ClassificationDto>();
            CreateMap<Broker, ClassificationDto>();
            CreateMap<ClassificationDto, Category>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Assets, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }

}
