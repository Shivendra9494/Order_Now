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
    
    public partial class MenuItemPropertyModel
    {
        public int Id { get; set; }
        public int DisplayOrder { get; set; }
        public string ItemType { get; set; }
        public int ItemId { get; set; }
        public int MenuItemPropertyId { get; set; }
        public string DisplayName { get; set; }
        public virtual MenuItemProperty MenuItemProperty { get; set; }
        public virtual MenuDishPropertiesGroup OptionItem { get; set; }
        public virtual MenuToppingsGroup ExtraItem { get; set; }
    }
}
