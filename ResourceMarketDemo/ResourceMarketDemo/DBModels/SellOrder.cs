//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ResourceMarketDemo.DBModels
{
    using System;
    using System.Collections.Generic;
    
    public partial class SellOrder
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ResourceTypeId { get; set; }
        public int ResourceSellAmount { get; set; }
        public int ResourceFilledAmount { get; set; }
        public byte CurrencyTypeId { get; set; }
        public decimal CurrencyPerResource { get; set; }
    
        public virtual CurrencyType CurrencyType { get; set; }
        public virtual ResourceType ResourceType { get; set; }
        public virtual User User { get; set; }
    }
}