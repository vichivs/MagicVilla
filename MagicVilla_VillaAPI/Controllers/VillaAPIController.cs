using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly IVillaRepository _villaRepository;
        private readonly ILogging _logger;

        public VillaAPIController(ILogging logger, IVillaRepository villaRepository)
        {
            _logger = logger;
            _villaRepository = villaRepository;
        }

        [HttpGet("GetAllVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            _logger.Log("Getting all villas", "");
            return Ok(_villaRepository.GetAllVillas());
        }

        [HttpGet("GetVillaById/{id:int}", Name = "GetVillaById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDto> GetVillaById(int id)
        {
            if (id == 0)
            {
                _logger.Log("Error finding villa with Id: " + id, "error");
                return BadRequest();
            }

            var villa = _villaRepository.GetVillaById(id);

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

            var villaNames = villaDtos.Select(v => v.Name);
            var existingVillas = _villaRepository.GetVillasByName(villaNames);

            var existingVillaNames = existingVillas.Select(v => v.Name.ToLower()).ToList();
            var villasToCreate = villaDtos.Where(v => !existingVillaNames.Contains(v.Name.ToLower()));

            _villaRepository.CreateVillas(villasToCreate);
            _villaRepository.SaveChanges();

            var response = new
            {
                CreatedVillas = villasToCreate.Select(v => new { v.Name, Message = "Villa successfully created" }),
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
            if (id == 0)
            {
                return BadRequest();
            }

            if (!_villaRepository.VillaExists(id))
            {
                return NotFound();
            }

            _villaRepository.DeleteVilla(id);
            _villaRepository.SaveChanges();

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

            if (!_villaRepository.VillaExists(villaDto.Id))
            {
                return NotFound();
            }

            _villaRepository.UpdateVilla(villaDto);
            _villaRepository.SaveChanges();

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

            if (!_villaRepository.VillaExists(id))
            {
                return NotFound();
            }

            _villaRepository.UpdatePartialVilla(id, patchDto);
            _villaRepository.SaveChanges();

            return NoContent();
        }
    }
}
