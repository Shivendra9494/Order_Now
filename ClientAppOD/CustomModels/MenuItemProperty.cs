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
    
    public partial class MenuItemProperty
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MenuItemProperty()
        {
            this.MenuItemPropertyModels = new HashSet<MenuItemPropertyModel>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> IdMenuItem { get; set; }
        public string AllowToppings { get; set; }
        public string DishPropertiesGroupId { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
    
        public virtual MenuItem MenuItem { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MenuItemPropertyModel> MenuItemPropertyModels { get; set; }
    }
}
