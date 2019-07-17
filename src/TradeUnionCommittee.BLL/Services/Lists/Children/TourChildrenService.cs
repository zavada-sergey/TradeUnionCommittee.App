﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeUnionCommittee.BLL.ActualResults;
using TradeUnionCommittee.BLL.Configurations;
using TradeUnionCommittee.BLL.DTO.Children;
using TradeUnionCommittee.BLL.Enums;
using TradeUnionCommittee.BLL.Helpers;
using TradeUnionCommittee.BLL.Interfaces.Lists.Children;
using TradeUnionCommittee.DAL.EF;
using TradeUnionCommittee.DAL.Entities;
using TradeUnionCommittee.DAL.Enums;

namespace TradeUnionCommittee.BLL.Services.Lists.Children
{
    internal class TourChildrenService : ITourChildrenService
    {
        private readonly TradeUnionCommitteeContext _context;
        private readonly AutoMapperConfiguration _mapperService;
        private readonly HashIdConfiguration _hashIdUtilities;

        public TourChildrenService(TradeUnionCommitteeContext context, AutoMapperConfiguration mapperService, HashIdConfiguration hashIdUtilities)
        {
            _context = context;
            _mapperService = mapperService;
            _hashIdUtilities = hashIdUtilities;
        }

        public async Task<ActualResult<IEnumerable<TourChildrenDTO>>> GetAllAsync(string hashIdChildren)
        {
            try
            {
                var id = _hashIdUtilities.DecryptLong(hashIdChildren);
                var tour = await _context.EventChildrens
                    .Include(x => x.IdEventNavigation)
                    .Where(x => x.IdChildren == id && x.IdEventNavigation.Type == TypeEvent.Tour)
                    .OrderByDescending(x => x.StartDate)
                    .ToListAsync();
                var result = _mapperService.Mapper.Map<IEnumerable<TourChildrenDTO>>(tour);
                return new ActualResult<IEnumerable<TourChildrenDTO>> { Result = result };
            }
            catch (Exception exception)
            {
                return new ActualResult<IEnumerable<TourChildrenDTO>>(DescriptionExceptionHelper.GetDescriptionError(exception));
            }
        }

        public async Task<ActualResult<TourChildrenDTO>> GetAsync(string hashId)
        {
            try
            {
                var id = _hashIdUtilities.DecryptLong(hashId);
                var tour = await _context.EventChildrens
                    .Include(x => x.IdEventNavigation)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (tour == null)
                {
                    return new ActualResult<TourChildrenDTO>(Errors.TupleDeleted);
                }
                var result = _mapperService.Mapper.Map<TourChildrenDTO>(tour);
                return new ActualResult<TourChildrenDTO> { Result = result };
            }
            catch (Exception exception)
            {
                return new ActualResult<TourChildrenDTO>(DescriptionExceptionHelper.GetDescriptionError(exception));
            }
        }

        public async Task<ActualResult<string>> CreateAsync(TourChildrenDTO item)
        {
            try
            {
                var tourChildren = _mapperService.Mapper.Map<EventChildrens>(item);
                await _context.EventChildrens.AddAsync(tourChildren);
                await _context.SaveChangesAsync();
                var hashId = _hashIdUtilities.EncryptLong(tourChildren.Id);
                return new ActualResult<string> { Result = hashId };
            }
            catch (Exception exception)
            {
                return new ActualResult<string>(DescriptionExceptionHelper.GetDescriptionError(exception));
            }
        }

        public async Task<ActualResult> UpdateAsync(TourChildrenDTO item)
        {
            try
            {
                _context.Entry(_mapperService.Mapper.Map<EventChildrens>(item)).State = EntityState.Modified;
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
                var result = await _context.EventChildrens.FindAsync(id);
                if (result != null)
                {
                    _context.EventChildrens.Remove(result);
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
    }
}