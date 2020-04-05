using System;
namespace ClientAppOD.CustomModels
{
    public class DeliveryInfo
    {
        public string CustomerPostCode { get; set; }
        public string BusinessPostCode { get; set; }
        public decimal? Distance { get; set; }
        public decimal? DeliveryFee { get; set; }
        public decimal? MinimumAmount { get; set; }
        public decimal? FreeDeliveryAmount { get; set; }
        public string Message { get; set; }
        public bool IsFree { get; set; }
    }
}
