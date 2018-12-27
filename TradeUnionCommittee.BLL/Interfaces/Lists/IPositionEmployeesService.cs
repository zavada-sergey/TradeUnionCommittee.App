﻿using System.Threading.Tasks;
using TradeUnionCommittee.BLL.DTO;
using TradeUnionCommittee.Common.ActualResults;

namespace TradeUnionCommittee.BLL.Interfaces.Lists
{
    public interface IPositionEmployeesService
    {
        Task<ActualResult<PositionEmployeesDTO>> GetAsync(string hashIdEmployee);
        Task<ActualResult> UpdateAsync(PositionEmployeesDTO dto);
    }
}