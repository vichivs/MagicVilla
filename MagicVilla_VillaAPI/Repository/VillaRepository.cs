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

        public IEnumerable<Villa> GetAllVillas()
        {
            return _db.Villas.ToList();
        }

        public Villa GetVillaById(int id)
        {
            return _db.Villas.FirstOrDefault(u => u.Id == id);
        }

        public void CreateVillas(IEnumerable<VillaDto> villaDtos)
        {
            var existingVillaNames = _db.Villas.Select(v => v.Name.ToLower()).ToList();
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

            _db.Villas.AddRange(models);
        }

        public void DeleteVilla(int id)
        {
            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
            if (villa != null)
            {
                _db.Villas.Remove(villa);
            }
        }

        public void UpdateVilla(VillaDto villaDto)
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

        public void UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchDto)
        {
            var existingVilla = _db.Villas.AsNoTracking().FirstOrDefault(u => u.Id == id);
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

        public bool VillaExists(int id)
        {
            return _db.Villas.Any(e => e.Id == id);
        }

        public bool SaveChanges()
        {
            return _db.SaveChanges() >= 0;
        }

        public List<Villa> GetVillasByName(IEnumerable<string> villaNames)
        {
            var lowerNames = villaNames.Select(name => name.ToLower()).ToList();
            return _db.Villas.Where(v => lowerNames.Contains(v.Name.ToLower())).ToList();
        }
    }
}
