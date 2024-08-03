using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogging _logger;
        public VillaAPIController(ILogging logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet("GetAllVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            _logger.Log("Getting all villas", "");
            return Ok(_db.Villas.ToList());
        }

        [HttpGet("GetVillaById/{id:int}", Name ="GetVillaById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDto> GetVillaById(int id)
        {
            if(id == 0)
            {
                _logger.Log("Error finding villa with Id: " + id, "error");
                return BadRequest();
            }

            var villa = _db.Villas.FirstOrDefault(u => u.Id.Equals(id));

            if (villa == null)
            {
                return NotFound();
            }

            return Ok(villa);
        }

        [HttpPost("CreateVillas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<VillaDto>> CreateVillas(IEnumerable<VillaDto> villaDtos)
        {
            if (villaDtos == null)
            {
                return BadRequest(villaDtos);
            }

            var existingVillaNames = _db.Villas.Select(v => v.Name.ToLower()).ToList();

            var villasToCreate = villaDtos.Where(v => !existingVillaNames.Contains(v.Name.ToLower()));
            var existingVillas = villaDtos.Where(v => existingVillaNames.Contains(v.Name.ToLower()));

            foreach (var villaDto in villasToCreate)
            {
                if (villaDto.Id > 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }

            var models = villasToCreate.Select(villaDto => new Villa
            {
                Amenity = villaDto.Amenity,
                Details = villaDto.Details,
                ImageUrl = villaDto.ImageUrl,
                Name = villaDto.Name,
                Occupancy = villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft,
                CreatedDate = DateTime.Now,
                UpdateDate = DateTime.Now
            });

            _db.Villas.AddRange(models);
            _db.SaveChanges();

            var response = new
            {
                CreatedVillas = villasToCreate,
                ExistingVillas = existingVillas.Select(v => new { v.Name, Message = "Villa already exists" })
            };

            return Ok(response);
        }

        [HttpDelete("DeleteVilla/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }

            var villa = _db.Villas.FirstOrDefault(u => u.Id.Equals(id));
            if(villa == null)
            {
                return NotFound();
            }
            _db.Villas.Remove(villa);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdatedVilla([FromBody] VillaDto villaDto)
        {
            if (villaDto == null || villaDto.Id == 0)
            {
                return BadRequest();
            }

            var existingVilla = _db.Villas.AsNoTracking().FirstOrDefault(u => u.Id == villaDto.Id);
            if (existingVilla == null)
            {
                return NotFound();
            }

            Villa updatedModel = new()
            {
                Id = villaDto.Id,
                Name = villaDto.Name,
                Details = villaDto.Details,
                Amenity = villaDto.Amenity,
                ImageUrl = villaDto.ImageUrl,
                Occupancy = villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft,
                CreatedDate = existingVilla.CreatedDate, 
                UpdateDate = DateTime.Now 
            };

            _db.Villas.Update(updatedModel);
            _db.SaveChanges();

            return NoContent();
        }



        [HttpPatch("UpdatePartialVilla/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }

            var existingVilla = _db.Villas.AsNoTracking().FirstOrDefault(u => u.Id == id);
            if (existingVilla == null)
            {
                return NotFound();
            }

            VillaDto villaDto = new()
            {
                Id = existingVilla.Id,
                Name = existingVilla.Name,
                Details = existingVilla.Details,
                Amenity = existingVilla.Amenity,
                ImageUrl = existingVilla.ImageUrl,
                Occupancy = existingVilla.Occupancy,
                Rate = existingVilla.Rate,
                Sqft = existingVilla.Sqft,
                UpdateDate = DateTime.Now 
            };

            patchDto.ApplyTo(villaDto, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Villa updatedModel = new()
            {
                Id = villaDto.Id,
                Name = villaDto.Name,
                Details = villaDto.Details,
                Amenity = villaDto.Amenity,
                ImageUrl = villaDto.ImageUrl,
                Occupancy = villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft,
                CreatedDate = existingVilla.CreatedDate, 
                UpdateDate = DateTime.Now 
            };

            _db.Villas.Update(updatedModel);
            _db.SaveChanges();

            return NoContent();
        }

    }
}
