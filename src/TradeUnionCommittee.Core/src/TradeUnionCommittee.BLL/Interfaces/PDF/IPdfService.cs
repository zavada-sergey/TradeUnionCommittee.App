﻿using System;
using System.Threading.Tasks;
using TradeUnionCommittee.BLL.ActualResults;
using TradeUnionCommittee.BLL.DTO;

namespace TradeUnionCommittee.BLL.Interfaces.PDF
{
    public interface IPdfService : IDisposable
    {
        Task<ActualResult<byte[]>> CreateReport(ReportPdfDTO dto);
    }
}