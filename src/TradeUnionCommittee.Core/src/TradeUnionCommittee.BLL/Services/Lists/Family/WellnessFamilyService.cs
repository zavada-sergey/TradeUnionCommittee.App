﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeUnionCommittee.BLL.ActualResults;
using TradeUnionCommittee.BLL.Configurations;
using TradeUnionCommittee.BLL.DTO.Family;
using TradeUnionCommittee.BLL.Enums;
using TradeUnionCommittee.BLL.Helpers;
using TradeUnionCommittee.BLL.Interfaces.Lists.Family;
using TradeUnionCommittee.DAL.EF;
using TradeUnionCommittee.DAL.Entities;
using TradeUnionCommittee.DAL.Enums;

namespace TradeUnionCommittee.BLL.Services.Lists.Family
{
    internal class WellnessFamilyService : IWellnessFamilyService
    {
        private readonly TradeUnionCommitteeContext _context;
        private readonly AutoMapperConfiguration _mapperService;
        private readonly HashIdConfiguration _hashIdUtilities;

        public WellnessFamilyService(TradeUnionCommitteeContext context, AutoMapperConfiguration mapperService, HashIdConfiguration hashIdUtilities)
        {
            _context = context;
            _mapperService = mapperService;
            _hashIdUtilities = hashIdUtilities;
        }

        public async Task<ActualResult<IEnumerable<WellnessFamilyDTO>>> GetAllAsync(string hashIdFamily)
        {
            try
            {
                var id = _hashIdUtilities.DecryptLong(hashIdFamily);
                var wellness = await _context.EventFamily
                    .Include(x => x.IdEventNavigation)
                    .Where(x => x.IdFamily == id && x.IdEventNavigation.Type == TypeEvent.Wellness)
                    .OrderByDescending(x => x.StartDate)
                    .ToListAsync();
                var result = _mapperService.Mapper.Map<IEnumerable<WellnessFamilyDTO>>(wellness);
                return new ActualResult<IEnumerable<WellnessFamilyDTO>> { Result = result };
            }
            catch (Exception exception)
            {
                return new ActualResult<IEnumerable<WellnessFamilyDTO>>(DescriptionExceptionHelper.GetDescriptionError(exception));
            }
        }

        public async Task<ActualResult<WellnessFamilyDTO>> GetAsync(string hashId)
        {
            try
            {
                var id = _hashIdUtilities.DecryptLong(hashId);
                var wellness = await _context.EventFamily
                    .Include(x => x.IdEventNavigation)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (wellness == null)
                {
                    return new ActualResult<WellnessFamilyDTO>(Errors.TupleDeleted);
                }
                var result = _mapperService.Mapper.Map<WellnessFamilyDTO>(wellness);
                return new ActualResult<WellnessFamilyDTO> { Result = result };
            }
            catch (Exception exception)
            {
                return new ActualResult<WellnessFamilyDTO>(DescriptionExceptionHelper.GetDescriptionError(exception));
            }
        }

        public async Task<ActualResult<string>> CreateAsync(WellnessFamilyDTO item)
        {
            try
            {
                var wellnessFamily = _mapperService.Mapper.Map<EventFamily>(item);
                await _context.EventFamily.AddAsync(wellnessFamily);
                await _context.SaveChangesAsync();
                var hashId = _hashIdUtilities.EncryptLong(wellnessFamily.Id);
                return new ActualResult<string> { Result = hashId };
            }
            catch (Exception exception)
            {
                return new ActualResult<string>(DescriptionExceptionHelper.GetDescriptionError(exception));
            }
        }

        public async Task<ActualResult> UpdateAsync(WellnessFamilyDTO item)
        {
            try
            {
                _context.Entry(_mapperService.Mapper.Map<EventFamily>(item)).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return new ActualResult();
            }
            catch (Exception exception)
            {
                return new ActualResult(DescriptionExceptionHelper.GetDescriptionError(exception));
            }
        }

        public async Task<ActualResult> DeleteAsync(string hashId)
        {
            try
            {
                var id = _hashIdUtilities.DecryptLong(hashId);
                var result = await _context.EventFamily.FindAsync(id);
                if (result != null)
                {
                    _context.EventFamily.Remove(result);
                    await _context.SaveChangesAsync();
                }
                return new ActualResult();
            }
            catch (Exception exception)
            {
                return new ActualResult(DescriptionExceptionHelper.GetDescriptionError(exception));
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        //--------------- Business Logic ---------------
    }
}