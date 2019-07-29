﻿using System;

namespace TradeUnionCommittee.BLL.DTO.Children
{
    public class ActivityChildrenDTO
    {
        public string HashId { get; set; }
        public string HashIdChildren { get; set; }
        public string HashIdActivities { get; set; }
        public string NameActivities { get; set; }
        public DateTime DateEvent { get; set; }
        public uint RowVersion { get; set; }
    }
}