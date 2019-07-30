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

namespace TradeUnionCommittee.BLL.Services.Lists.Employee
{
    internal class MaterialAidEmployeesService : IMaterialAidEmployeesService
    {
        private readonly TradeUnionCommitteeContext _context;
        private readonly AutoMapperConfiguration _mapperService;
        private readonly HashIdConfiguration _hashIdUtilities;

        public MaterialAidEmployeesService(TradeUnionCommitteeContext context, AutoMapperConfiguration mapperService, HashIdConfiguration hashIdUtilities)
        {
            _context = context;
            _mapperService = mapperService;
            _hashIdUtilities = hashIdUtilities;
        }

        public async Task<ActualResult<IEnumerable<MaterialAidEmployeesDTO>>> GetAllAsync(string hashIdEmployee)
        {
            try
            {
                var id = _hashIdUtilities.DecryptLong(hashIdEmployee);
                var materialAid = await _context.MaterialAidEmployees
                    .Include(x => x.IdMaterialAidNavigation)
                    .Where(x => x.IdEmployee == id)
                    .OrderByDescending(x => x.DateIssue)
                    .ToListAsync();
                var result = _mapperService.Mapper.Map<IEnumerable<MaterialAidEmployeesDTO>>(materialAid);
                return new ActualResult<IEnumerable<MaterialAidEmployeesDTO>> { Result = result };
            }
            catch (Exception exception)
            {
                return new ActualResult<IEnumerable<MaterialAidEmployeesDTO>>(DescriptionExceptionHelper.GetDescriptionError(exception));
            }
        }

        public async Task<ActualResult<MaterialAidEmployeesDTO>> GetAsync(string hashId)
        {
            try
            {
                var id = _hashIdUtilities.DecryptLong(hashId);
                var materialAid = await _context.MaterialAidEmployees
                    .Include(x => x.IdMaterialAidNavigation)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (materialAid == null)
                {
                    return new ActualResult<MaterialAidEmployeesDTO>(Errors.TupleDeleted);
                }
                var result = _mapperService.Mapper.Map<MaterialAidEmployeesDTO>(materialAid);
                return new ActualResult<MaterialAidEmployeesDTO> { Result = result };
            }
            catch (Exception exception)
            {
                return new ActualResult<MaterialAidEmployeesDTO>(DescriptionExceptionHelper.GetDescriptionError(exception));
            }
        }

        public async Task<ActualResult<string>> CreateAsync(MaterialAidEmployeesDTO item)
        {
            try
            {
                var materialAidEmployees = _mapperService.Mapper.Map<MaterialAidEmployees>(item);
                await _context.MaterialAidEmployees.AddAsync(materialAidEmployees);
                await _context.SaveChangesAsync();
                var hashId = _hashIdUtilities.EncryptLong(materialAidEmployees.Id);
                return new ActualResult<string> { Result = hashId };
            }
            catch (Exception exception)
            {
                return new ActualResult<string>(DescriptionExceptionHelper.GetDescriptionError(exception));
            }
        }

        public async Task<ActualResult> UpdateAsync(MaterialAidEmployeesDTO item)
        {
            try
            {
                _context.Entry(_mapperService.Mapper.Map<MaterialAidEmployees>(item)).State = EntityState.Modified;
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
                var result = await _context.MaterialAidEmployees.FindAsync(id);
                if (result != null)
                {
                    _context.MaterialAidEmployees.Remove(result);
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