using AutoMapper;
using Villa_API.Models;
using Villa_API.Models.Dto.Villa;
using Villa_API.Models.Dto.VillaNumber;
using Villa_API.Models.User;

namespace Villa_API.AutoMapper
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            // Villa config ...
            CreateMap<Villa, VillaDTO>().ReverseMap();
            CreateMap<Villa, VillaCreateDTO>().ReverseMap();
            CreateMap<Villa, VillaUpdateDTO>().ReverseMap();

            // VillaNumber config ...
            CreateMap<VillaNumber, VillaNumberDTO>().ReverseMap();
            CreateMap<VillaNumber, VillaNumberCreateDTO>().ReverseMap();
            CreateMap<VillaNumber, VillaNumberUpdateDTO>().ReverseMap();

            // Identity config ...
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();

        }
    }
}
