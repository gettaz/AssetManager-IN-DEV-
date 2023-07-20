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
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.AssetName, opt => opt.MapFrom(src => src.AssetName))
                .ForMember(dest => dest.Ticker, opt => opt.MapFrom(src => src.Ticker))
                .ForMember(dest => dest.PriceBought, opt => opt.MapFrom(src => src.PriceBought))
                .ForMember(dest => dest.BrokerName, opt => opt.MapFrom(src => src.BrokerName))
                .ForMember(dest => dest.DateBought, opt => opt.MapFrom(src => src.DateBought))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));
            CreateMap<AssetDto, Asset>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.AssetCategories, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DateSold, opt => opt.Ignore());
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.AssetCategories, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }

}
