﻿using TradeUnionCommittee.PDF.Service.Enums;

namespace TradeUnionCommittee.PDF.Service.Models
{
    public class ReportModel : DataModel
    {
        public TypeReport Type { get; set; }
        public string FullNameEmployee { get; set; }
    }
}
