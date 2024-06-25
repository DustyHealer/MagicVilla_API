using MagicVilla_VillaAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        // Http verbs helps swagger to create the proper documentation. If there is no httpverb swagger page will give an error
        [HttpGet]
        public IEnumerable<Villa> GetVillas() 
        {
            return new List<Villa> {
                new Villa { Id = 1, Name = "Pool View" },
                new Villa { Id = 2, Name = "Beach View" }
            };
        }
    }
}
