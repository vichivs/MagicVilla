using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace MagicVilla_VillaAPI.Repositories
{
    public class VillaRepository : IVillaRepository
    {
        private readonly ApplicationDbContext _db;

        public VillaRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Villa>> GetAllVillasAsync()
        {
            return await _db.Villas.ToListAsync();
        }

        public async Task<Villa> GetVillaByIdAsync(int id)
        {
            return await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task CreateVillasAsync(IEnumerable<VillaDto> villaDtos)
        {
            var existingVillaNames = await _db.Villas.Select(v => v.Name.ToLower()).ToListAsync();
            var villasToCreate = villaDtos.Where(v => !existingVillaNames.Contains(v.Name.ToLower()));

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

            await _db.Villas.AddRangeAsync(models);
        }

        public async Task DeleteVillaAsync(int id)
        {
            var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
            if (villa != null)
            {
                _db.Villas.Remove(villa);
            }
        }

        public async Task UpdateVillaAsync(VillaDto villaDto)
        {
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
                CreatedDate = _db.Villas.AsNoTracking().FirstOrDefault(u => u.Id == villaDto.Id)?.CreatedDate ?? DateTime.Now,
                UpdateDate = DateTime.Now
            };

            _db.Villas.Update(updatedModel);
        }

        public async Task UpdatePartialVillaAsync(int id, JsonPatchDocument<VillaDto> patchDto)
        {
            var existingVilla = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if (existingVilla == null)
            {
                return;
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

            patchDto.ApplyTo(villaDto);

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
        }

        public async Task<bool> VillaExistsAsync(int id)
        {
            return await _db.Villas.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync() >= 0;
        }

        public async Task<List<Villa>> GetVillasByNameAsync(IEnumerable<string> villaNames)
        {
            var lowerNames = villaNames.Select(name => name.ToLower()).ToList();
            return await _db.Villas.Where(v => lowerNames.Contains(v.Name.ToLower())).ToListAsync();
        }

    }
}
