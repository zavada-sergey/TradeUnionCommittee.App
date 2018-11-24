﻿using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TradeUnionCommittee.Common.ActualResults;
using TradeUnionCommittee.DAL.EF;
using TradeUnionCommittee.DAL.Entities;
using TradeUnionCommittee.DAL.Enums;

namespace TradeUnionCommittee.DAL.Repositories.Directories
{
    public class SubdivisionsRepository : Repository<Subdivisions>
    {
        private readonly TradeUnionCommitteeEmployeesCoreContext _dbContext;

        public SubdivisionsRepository(TradeUnionCommitteeEmployeesCoreContext db) : base(db)
        {
            _dbContext = db;
        }

        public override async Task<ActualResult> Create(Subdivisions item)
        {
            var result = new ActualResult();
            try
            {
                if (item.IdSubordinate == null || item.IdSubordinate == 0)
                {
                    await _dbContext.Subdivisions.AddAsync(new Subdivisions
                    {
                        Name = item.Name,
                        Abbreviation = item.Abbreviation
                    });
                }
                else
                {
                    await _dbContext.Subdivisions.AddAsync(new Subdivisions
                    {
                        Name = item.Name,
                        Abbreviation = item.Abbreviation,
                        IdSubordinate = item.IdSubordinate
                    });
                }

            }
            catch (Exception e)
            {
               return new ActualResult(e.Message);
            }
            return result;
        }

        public override async Task<ActualResult> Update(Subdivisions item)
        {
            var result = new ActualResult();

            try
            {
                switch (item.SubdivisionUpdate)
                {
                    case SubdivisionUpdate.Name:
                        _dbContext.Entry(item).State = EntityState.Modified;
                        _dbContext.Entry(item).Property(x => x.IdSubordinate).IsModified = false;
                        _dbContext.Entry(item).Property(x => x.Abbreviation).IsModified = false;
                        break;
                    case SubdivisionUpdate.Abbreviation:
                        _dbContext.Entry(item).State = EntityState.Modified;
                        _dbContext.Entry(item).Property(x => x.IdSubordinate).IsModified = false;
                        _dbContext.Entry(item).Property(x => x.Name).IsModified = false;
                        break;
                    case SubdivisionUpdate.RestructuringUnits:
                        _dbContext.Entry(item).State = EntityState.Modified;
                        _dbContext.Entry(item).Property(x => x.Abbreviation).IsModified = false;
                        _dbContext.Entry(item).Property(x => x.Name).IsModified = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                return new ActualResult(e.Message);
            }
            return result;
        }
    }
}