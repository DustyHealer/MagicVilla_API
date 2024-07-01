using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{
    // [Route("api/[controller]")] can also be used but incase the controller filename changed later on, then it is a problem
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper _mapper;
        public VillaAPIController(IVillaRepository dbVilla, IMapper mapper)
        {
            _dbVilla = dbVilla;
            _mapper = mapper;
            this._response = new APIResponse();
        }

        // Http verbs helps swagger to create the proper documentation. If there is no httpverb swagger page will give an error
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<VillaDTO>>(villaList));
        }

        // Produces Response Type is used to document response types with the swagger
        [HttpGet("{id:int}", Name = "GetVilla")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(200, Type = typeof(VillaDTO))]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDTO>> GetVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villa = await _dbVilla.GetAsync(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<VillaDTO>(villa));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO villaCreateDTO)
        {
            //if (!ModelState.IsValid) // It will be used incase we are not using the APIController annotation
            //{
            //    return BadRequest(ModelState);
            //}
            if (await _dbVilla.GetAsync(u => u.Name.ToLower() == villaCreateDTO.Name.ToLower()) != null)
            {
                // Key should be unique
                ModelState.AddModelError("CustomError", "Villa Name already exists");
                return BadRequest(ModelState);
            }

            if (villaCreateDTO == null)
            {
                return BadRequest(villaCreateDTO);
            }
            
            // Since id is not present in VillaCreateDto, we dont need this
            //if (villaDTO.Id > 0)
            //{
            //    return StatusCode(StatusCodes.Status500InternalServerError);
            //}

            // Convert Dto to model, so that we can add to the table
            //Villa model = new()
            //{
            //    Amenity = villaCreateDTO.Amenity,
            //    Details = villaCreateDTO.Details,
            //    ImageUrl = villaCreateDTO.ImageUrl,
            //    Name = villaCreateDTO.Name,
            //    Occupancy = villaCreateDTO.Occupancy,
            //    Rate = villaCreateDTO.Rate,
            //    Sqft = villaCreateDTO.Sqft
            //};

            // Used automapper for the above mapping
            Villa model = _mapper.Map<Villa>(villaCreateDTO);

            await _dbVilla.CreateAsync(model);

            // When the resource is created, give the url where resource is created
            return CreatedAtRoute("GetVilla", new { id = model.Id }, model);
        }


        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = await _dbVilla.GetAsync(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            await _dbVilla.RemoveAsync(villa);
            
            // It means no content is returned in the response.
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody]VillaUpdateDTO villaUpdateDTO) 
        {
            if(villaUpdateDTO == null || id != villaUpdateDTO.Id)
            {
                return BadRequest();
            }

            // This code was required when we were using static data source
            //var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
            //if(villa == null)
            //{
            //    return NotFound();
            //}
            //villa.Name = villaDTO.Name;
            //villa.Sqft = villaDTO.Sqft;
            //villa.Occupancy = villaDTO.Occupancy;

            // Convert dto to model, so that we can update inside table
            //Villa model = new()
            //{
            //    Amenity = villaUpdateDTO.Amenity,
            //    Details = villaUpdateDTO.Details,
            //    Id = villaUpdateDTO.Id,
            //    ImageUrl = villaUpdateDTO.ImageUrl,
            //    Name = villaUpdateDTO.Name,
            //    Occupancy = villaUpdateDTO.Occupancy,
            //    Rate = villaUpdateDTO.Rate,
            //    Sqft = villaUpdateDTO.Sqft
            //};

            // Used automapper for the above mapping
            Villa model = _mapper.Map<Villa>(villaUpdateDTO);

            await _dbVilla.UpdateAsync(model);
            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO) 
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var villa = await _dbVilla.GetAsync(u => u.Id == id, false);
            if (villa == null)
            {
                return BadRequest();
            }

            // Convert model to dto, so that we can apply incoming dto to the new dto
            //VillaUpdateDTO villaUpdateDTO = new()
            //{
            //    Amenity = villa.Amenity,
            //    Details = villa.Details,
            //    Id = villa.Id,
            //    ImageUrl = villa.ImageUrl,
            //    Name = villa.Name,
            //    Occupancy = villa.Occupancy,
            //    Rate = villa.Rate,
            //    Sqft = villa.Sqft
            //};

            // Used automapper for the above mapping
            VillaUpdateDTO villaUpdateDTO = _mapper.Map<VillaUpdateDTO>(villa);
            patchDTO.ApplyTo(villaUpdateDTO, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Convert dto to model, so that we can update in the table
            //Villa model = new()
            //{
            //    Amenity = villaUpdateDTO.Amenity,
            //    Details = villaUpdateDTO.Details,
            //    Id = villaUpdateDTO.Id,
            //    ImageUrl = villaUpdateDTO.ImageUrl,
            //    Name = villaUpdateDTO.Name,
            //    Occupancy = villaUpdateDTO.Occupancy,
            //    Rate = villaUpdateDTO.Rate,
            //    Sqft = villaUpdateDTO.Sqft
            //};

            // Used automapper for the above mapping
            Villa model = _mapper.Map<Villa>(villaUpdateDTO);

            // Generally in patch we dont use update, we write a sp in sql to update given properties
            await _dbVilla.UpdateAsync(model);
            return NoContent();
        }
    }
}
