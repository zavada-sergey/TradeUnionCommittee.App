﻿using System;
using System.ComponentModel.DataAnnotations;

namespace TradeUnionCommittee.ViewModels.ViewModels.Children
{
    public class CreateGiftChildrenViewModel
    {
        [Required]
        public string HashIdChildren { get; set; }
        [Required(ErrorMessage = "Назва заходу не може бути порожньою!")]
        public string NameEvent { get; set; }
        [Required(ErrorMessage = "Назва подарунку не може бути порожньою!")]
        public string NameGift { get; set; }
        [Required(ErrorMessage = "Ціна не може бути порожньою!")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Знижка не може бути порожньою!")]
        public decimal Discount { get; set; }
        [Required(ErrorMessage = "Дата вручення не може бути порожньою!")]
        public DateTime DateGift { get; set; }
    }

    public class UpdateGiftChildrenViewModel : CreateGiftChildrenViewModel
    {
        [Required]
        public string HashId { get; set; }
        [Required]
        public uint RowVersion { get; set; }
    }
}