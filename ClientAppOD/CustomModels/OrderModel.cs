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
    
    public partial class OrderModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrderModel()
        {
            this.OrderModelExtras = new HashSet<OrderModelExtra>();
        }
    
        public int Id { get; set; }
        public int DisplayOrder { get; set; }
        public string ItemType { get; set; }
        public string ItemName { get; set; }
        public string Qty { get; set; }
        public Nullable<decimal> Price { get; set; }
        public int OrderItemID { get; set; }
        public Nullable<int> OptionID { get; set; }
    
        public virtual OrderItem OrderItem { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderModelExtra> OrderModelExtras { get; set; }
    }
}
