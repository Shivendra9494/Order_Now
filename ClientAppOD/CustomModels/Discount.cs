using System;
namespace ClientAppOD.CustomModels
{
    public class Discount
    {

        public int ID { get; set; }
        public Nullable<int> DiscountPercentage { get; set; }
        public Nullable<decimal> MinimumAmount { get; set; }
        public Nullable<decimal> MaximumAmount { get; set; }
        public string Available { get; set; }
        public Nullable<int> OrderCount { get; set; }
        public int BusinessDetailID { get; set; }

        public virtual Store BusinessDetail { get; set; }
    }
}
