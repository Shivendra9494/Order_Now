//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OD.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrderModelExtra
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public string Qty { get; set; }
        public int DisplayOrder { get; set; }
        public Nullable<decimal> Price { get; set; }
        public int OrderModelId { get; set; }
        public Nullable<int> ExtraId { get; set; }
    
        public virtual OrderModel OrderModel { get; set; }
    }
}
