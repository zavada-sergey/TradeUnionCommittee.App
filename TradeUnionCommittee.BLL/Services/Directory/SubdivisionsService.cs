﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeUnionCommittee.BLL.DTO;
using TradeUnionCommittee.BLL.Interfaces.Directory;
using TradeUnionCommittee.BLL.Utilities;
using TradeUnionCommittee.Common.ActualResults;
using TradeUnionCommittee.Common.Enums;
using TradeUnionCommittee.DAL.Entities;
using TradeUnionCommittee.DAL.Enums;
using TradeUnionCommittee.DAL.Interfaces;

namespace TradeUnionCommittee.BLL.Services.Directory
{
    public class SubdivisionsService : ISubdivisionsService
    {
        private readonly IUnitOfWork _database;
        private readonly IAutoMapperUtilities _mapperService;
        private readonly IHashIdUtilities _hashIdUtilities;

        public SubdivisionsService(IUnitOfWork database, IAutoMapperUtilities mapperService, IHashIdUtilities hashIdUtilities)
        {
            _database = database;
            _mapperService = mapperService;
            _hashIdUtilities = hashIdUtilities;
        }

        public async Task<ActualResult<IEnumerable<SubdivisionDTO>>> GetAllAsync() => 
            _mapperService.Mapper.Map<ActualResult<IEnumerable<SubdivisionDTO>>>(await _database.SubdivisionsRepository.Find(x => x.IdSubordinate == null));

        public async Task<ActualResult<SubdivisionDTO>> GetAsync(string hashId)
        {
            var id = _hashIdUtilities.DecryptLong(hashId, Enums.Services.Subdivision);
            return _mapperService.Mapper.Map<ActualResult<SubdivisionDTO>>(await _database.SubdivisionsRepository.Get(id));
        }

        public async Task<ActualResult<IEnumerable<SubdivisionDTO>>> GetSubordinateSubdivisions(string hashId)
        {
            var id = _hashIdUtilities.DecryptLong(hashId, Enums.Services.Subdivision);
            return _mapperService.Mapper.Map<ActualResult<IEnumerable<SubdivisionDTO>>>(await _database.SubdivisionsRepository.Find(x => x.IdSubordinate == id));
        }

        public async Task<Dictionary<string,string>> GetSubordinateSubdivisionsForMvc(string hashId)
        {
            var subordinateSubdivisions = await GetSubordinateSubdivisions(hashId);
            return subordinateSubdivisions.Result.ToDictionary(subdivision => $"{subdivision.HashIdMain},{subdivision.RowVersion}", subdivision => subdivision.Name);
        }

        //-------------------------------------------------------------------------------------------------------------------

        public async Task<ActualResult> CreateMainSubdivisionAsync(SubdivisionDTO dto)
        {
            if (!await CheckNameAsync(dto.Name) && !await CheckAbbreviationAsync(dto.Abbreviation))
            {
                await _database.SubdivisionsRepository.Create(_mapperService.Mapper.Map<Subdivisions>(dto));
                return _mapperService.Mapper.Map<ActualResult>(await _database.SaveAsync());
            }
            return new ActualResult(Errors.DuplicateData);
        }

        public async Task<ActualResult> CreateSubordinateSubdivisionAsync(SubdivisionDTO dto)
        {
            if (!await CheckNameAsync(dto.Name) && !await CheckAbbreviationAsync(dto.Abbreviation))
            {
                await _database.SubdivisionsRepository.Create(_mapperService.Mapper.Map<Subdivisions>(dto));
                return _mapperService.Mapper.Map<ActualResult>(await _database.SaveAsync());
            }
            return new ActualResult(Errors.DuplicateData);
        }

        //-------------------------------------------------------------------------------------------------------------------

        public async Task<ActualResult> UpdateNameSubdivisionAsync(SubdivisionDTO dto)
        {
            if (!await CheckNameAsync(dto.Name))
            {
                var model  = _mapperService.Mapper.Map<Subdivisions>(dto);
                model.SubdivisionUpdate = SubdivisionUpdate.Name;
                await _database.SubdivisionsRepository.Update(model);
                return _mapperService.Mapper.Map<ActualResult>(await _database.SaveAsync());
            }
            return new ActualResult(Errors.DuplicateData);
        }

        public async Task<ActualResult> UpdateAbbreviationSubdivisionAsync(SubdivisionDTO dto)
        {
            if (!await CheckNameAsync(dto.Abbreviation))
            {
                var model = _mapperService.Mapper.Map<Subdivisions>(dto);
                model.SubdivisionUpdate = SubdivisionUpdate.Abbreviation;
                await _database.SubdivisionsRepository.Update(model);
                return _mapperService.Mapper.Map<ActualResult>(await _database.SaveAsync());
            }
            return new ActualResult(Errors.DuplicateData);
        }

        public async Task<ActualResult> RestructuringUnits(RestructuringSubdivisionDTO dto)
        {
            await _database.SubdivisionsRepository.Update(_mapperService.Mapper.Map<Subdivisions>(dto));
            return _mapperService.Mapper.Map<ActualResult>(await _database.SaveAsync());
        }

        //-------------------------------------------------------------------------------------------------------------------

        public async Task<ActualResult> DeleteAsync(string hashId)
        {
            await _database.SubdivisionsRepository.Delete(_hashIdUtilities.DecryptLong(hashId, Enums.Services.Subdivision));
            return _mapperService.Mapper.Map<ActualResult>(await _database.SaveAsync());
        }

        //-------------------------------------------------------------------------------------------------------------------

        public async Task<bool> CheckNameAsync(string name)
        {
            var result = await _database.SubdivisionsRepository.Find(p => p.Name == name);
            return result.Result.Any();
        }

        public async Task<bool> CheckAbbreviationAsync(string name)
        {
            var result = await _database.SubdivisionsRepository.Find(p => p.Abbreviation == name);
            return result.Result.Any();
        }

        //-------------------------------------------------------------------------------------------------------------------

        public void Dispose()
        {
            _database.Dispose();
        }
    }
}