using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;

namespace MagicVilla_VillaAPI.Repositories
{
    public interface IVillaRepository
    {
        Task<IEnumerable<Villa>> GetAllVillasAsync();
        Task<Villa> GetVillaByIdAsync(int id);
        Task<List<Villa>> GetVillasByNameAsync(IEnumerable<string> villaNames);
        Task CreateVillasAsync(IEnumerable<VillaDto> villaDtos);
        Task DeleteVillaAsync(int id);
        Task UpdateVillaAsync(VillaDto villaDto);
        Task UpdatePartialVillaAsync(int id, JsonPatchDocument<VillaDto> patchDto);
        Task<bool> VillaExistsAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}
