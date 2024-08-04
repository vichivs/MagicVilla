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
        public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillasAsync()
        {
            _logger.Log("Getting all villas", "");
            return Ok(await _villaRepository.GetAllVillasAsync());
        }

        [HttpGet("GetVillaById/{id:int}", Name = "GetVillaById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDto>> GetVillaByIdAsync(int id)
        {
            if (id == 0)
            {
                _logger.Log("Error finding villa with Id: " + id, "error");
                return BadRequest();
            }

            var villa = await _villaRepository.GetVillaByIdAsync(id);

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
        public async Task<ActionResult<IEnumerable<VillaDto>>> CreateVillasAsync(IEnumerable<VillaDto> villaDtos)
        {
            if (villaDtos == null)
            {
                return BadRequest(villaDtos);
            }

            var villaNames = villaDtos.Select(v => v.Name);
            var existingVillas = await _villaRepository.GetVillasByNameAsync(villaNames);

            var existingVillaNames = existingVillas.Select(v => v.Name.ToLower()).ToList();
            var villasToCreate = villaDtos.Where(v => !existingVillaNames.Contains(v.Name.ToLower()));

            await _villaRepository.CreateVillasAsync(villasToCreate);
            await _villaRepository.SaveChangesAsync();

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
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            if (!await _villaRepository.VillaExistsAsync(id))
            {
                return NotFound();
            }

            await _villaRepository.DeleteVillaAsync(id);
            await _villaRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatedVillaAsync([FromBody] VillaDto villaDto)
        {
            if (villaDto == null || villaDto.Id == 0)
            {
                return BadRequest();
            }

            if (!await _villaRepository.VillaExistsAsync(villaDto.Id))
            {
                return NotFound();
            }

            await _villaRepository.UpdateVillaAsync(villaDto);
            await _villaRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("UpdatePartialVilla/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatePartialVillaAsync(int id, JsonPatchDocument<VillaDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }

            if (!await _villaRepository.VillaExistsAsync(id))
            {
                return NotFound();
            }

            await _villaRepository.UpdatePartialVillaAsync(id, patchDto);
            await _villaRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
