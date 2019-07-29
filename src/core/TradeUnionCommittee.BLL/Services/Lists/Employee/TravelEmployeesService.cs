﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeUnionCommittee.BLL.ActualResults;
using TradeUnionCommittee.BLL.Configurations;
using TradeUnionCommittee.BLL.DTO.Employee;
using TradeUnionCommittee.BLL.Enums;
using TradeUnionCommittee.BLL.Helpers;
using TradeUnionCommittee.BLL.Interfaces.Lists.Employee;
using TradeUnionCommittee.DAL.EF;
using TradeUnionCommittee.DAL.Entities;
using TradeUnionCommittee.DAL.Enums;

namespace TradeUnionCommittee.BLL.Services.Lists.Employee
{
    internal class TravelEmployeesService : ITravelEmployeesService
    {
        private readonly TradeUnionCommitteeContext _context;
        private readonly AutoMapperConfiguration _mapperService;
        private readonly HashIdConfiguration _hashIdUtilities;

        public TravelEmployeesService(TradeUnionCommitteeContext context, AutoMapperConfiguration mapperService, HashIdConfiguration hashIdUtilities)
        {
            _context = context;
            _mapperService = mapperService;
            _hashIdUtilities = hashIdUtilities;
        }

        public async Task<ActualResult<IEnumerable<TravelEmployeesDTO>>> GetAllAsync(string hashIdEmployee)
        {
            try
            {
                var id = _hashIdUtilities.DecryptLong(hashIdEmployee);
                var travel = await _context.EventEmployees
                    .Include(x => x.IdEventNavigation)
                    .Where(x => x.IdEmployee == id && x.IdEventNavigation.Type == TypeEvent.Travel)
                    .OrderByDescending(x => x.StartDate)
                    .ToListAsync();
                var result = _mapperService.Mapper.Map<IEnumerable<TravelEmployeesDTO>>(travel);
                return new ActualResult<IEnumerable<TravelEmployeesDTO>> { Result = result };
            }
            catch (Exception exception)
            {
                return new ActualResult<IEnumerable<TravelEmployeesDTO>>(DescriptionExceptionHelper.GetDescriptionError(exception));
            }
        }

        public async Task<ActualResult<TravelEmployeesDTO>> GetAsync(string hashId)
        {
            try
            {
                var id = _hashIdUtilities.DecryptLong(hashId);
                var travel = await _context.EventEmployees
                    .Include(x => x.IdEventNavigation)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (travel == null)
                {
                    return new ActualResult<TravelEmployeesDTO>(Errors.TupleDeleted);
                }
                var result = _mapperService.Mapper.Map<TravelEmployeesDTO>(travel);
                return new ActualResult<TravelEmployeesDTO> { Result = result };
            }
            catch (Exception exception)
            {
                return new ActualResult<TravelEmployeesDTO>(DescriptionExceptionHelper.GetDescriptionError(exception));
            }
        }

        public async Task<ActualResult<string>> CreateAsync(TravelEmployeesDTO item)
        {
            try
            {
                var travelEmployees = _mapperService.Mapper.Map<EventEmployees>(item);
                await _context.EventEmployees.AddAsync(travelEmployees);
                await _context.SaveChangesAsync();
                var hashId = _hashIdUtilities.EncryptLong(travelEmployees.Id);
                return new ActualResult<string> { Result = hashId };
            }
            catch (Exception exception)
            {
                return new ActualResult<string>(DescriptionExceptionHelper.GetDescriptionError(exception));
            }
        }

        public async Task<ActualResult> UpdateAsync(TravelEmployeesDTO item)
        {
            try
            {
                _context.Entry(_mapperService.Mapper.Map<EventEmployees>(item)).State = EntityState.Modified;
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
                var result = await _context.EventEmployees.FindAsync(id);
                if (result != null)
                {
                    _context.EventEmployees.Remove(result);
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