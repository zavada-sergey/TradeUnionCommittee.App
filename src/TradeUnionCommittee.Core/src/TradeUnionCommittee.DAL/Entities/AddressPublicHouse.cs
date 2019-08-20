﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TradeUnionCommittee.DAL.Enums;

namespace TradeUnionCommittee.DAL.Entities
{
    public class AddressPublicHouse
    {
        public AddressPublicHouse()
        {
            PublicHouseEmployees = new HashSet<PublicHouseEmployees>();
        }

        public long Id { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string NumberHouse { get; set; }
        public string NumberDormitory { get; set; }
        public TypeHouse Type { get; set; }
        [Timestamp]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column("xmin", TypeName = "xid")]
        public uint RowVersion { get; set; }

        public ICollection<PublicHouseEmployees> PublicHouseEmployees { get; set; }
    }
}
