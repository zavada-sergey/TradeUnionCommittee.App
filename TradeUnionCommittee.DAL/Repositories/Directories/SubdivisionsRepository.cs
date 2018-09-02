﻿using System;
using TradeUnionCommittee.Common.ActualResults;
using TradeUnionCommittee.DAL.EF;
using TradeUnionCommittee.DAL.Entities;

namespace TradeUnionCommittee.DAL.Repositories.Directories
{
    public class SubdivisionsRepository : Repository<Subdivisions>
    {
        private readonly TradeUnionCommitteeEmployeesCoreContext _dbContext;

        public SubdivisionsRepository(TradeUnionCommitteeEmployeesCoreContext db) : base(db)
        {
            _dbContext = db;
        }

        public override ActualResult Create(Subdivisions item)
        {
            var result = new ActualResult();
            try
            {
                if (item.IdSubordinate == null || item.IdSubordinate == 0)
                {
                    _dbContext.Subdivisions.Add(new Subdivisions
                    {
                        Name = item.Name,
                        Abbreviation = item.Abbreviation
                    });
                }
                else
                {
                    _dbContext.Subdivisions.Add(new Subdivisions
                    {
                        Name = item.Name,
                        Abbreviation = item.Abbreviation,
                        IdSubordinate = item.IdSubordinate
                    });
                }

            }
            catch (Exception e)
            {
                result.IsValid = false;
                result.ErrorsList.Add(e.Message);
            }
            return result;
        }

        
    }
}