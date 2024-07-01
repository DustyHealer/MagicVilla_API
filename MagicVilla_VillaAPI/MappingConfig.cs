using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            // If the name of the property is same, it will automatically map the properties
            CreateMap<Villa, VillaDTO>();
            CreateMap<VillaDTO, Villa>();

            // We dont need to write two lines like above, we can use reverse map to create reverse mapping
            CreateMap<Villa, VillaCreateDTO>().ReverseMap();
            CreateMap<Villa, VillaUpdateDTO>().ReverseMap();
        }
    }
}
