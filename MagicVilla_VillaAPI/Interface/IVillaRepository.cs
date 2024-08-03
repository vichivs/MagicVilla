using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;

namespace MagicVilla_VillaAPI.Repositories
{
    public interface IVillaRepository
    {
        IEnumerable<Villa> GetAllVillas();
        Villa GetVillaById(int id);
        List<Villa> GetVillasByName(IEnumerable<string> villaNames);
        void CreateVillas(IEnumerable<VillaDto> villaDtos);
        void DeleteVilla(int id);
        void UpdateVilla(VillaDto villaDto);
        void UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchDto);
        bool VillaExists(int id);
        bool SaveChanges();
    }
}
