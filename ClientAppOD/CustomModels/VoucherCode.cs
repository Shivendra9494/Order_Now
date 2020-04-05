using System;
namespace ClientAppOD.CustomModels
{
    public class VoucherCode
    {
            public int ID { get; set; }
            public string VoucherCodeText { get; set; }
            public Nullable<int> VoucherCodeDiscount { get; set; }
            public string VoucherType { get; set; }
            public string StartDate { get; set; }
            public string EndDate { get; set; }
            public int BusinessDetailID { get; set; }
            public string Status { get; set; }
            public int Total { get; set; }
            public int Redeemed { get; set; }
            public int Balance { get; set; }

            public virtual Store BusinessDetail { get; set; }
        
    }
}
