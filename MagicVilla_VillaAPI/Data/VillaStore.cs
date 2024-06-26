using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI.Data
{
    public static class VillaStore
    {
        public static List<VillaDTO> villaList =
        [
            new VillaDTO { Id = 1, Name = "Pool View", Occupancy = 4, Sqft = 400 },
            new VillaDTO { Id = 2, Name = "Beach View", Occupancy = 3, Sqft = 300 }
        ];
    }
}
