using AssetManager.DTO;
using AssetManager.Models;
using AutoMapper;

namespace AssetManager.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Asset, AssetDto>();
            CreateMap<Category, CategoryDto>();
        }
    }
}
