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
    
    public partial class MenuDishProperty
    {
        public int ID { get; set; }
        public string DishProperty { get; set; }
        public Nullable<double> DishPropertyPrice { get; set; }
        public Nullable<int> DisplayoOder { get; set; }
        public int MenuDishPropertiesGroupID { get; set; }
    
        public virtual MenuDishPropertiesGroup MenuDishPropertiesGroup { get; set; }
        public bool selected { get; set; }
    }
}
