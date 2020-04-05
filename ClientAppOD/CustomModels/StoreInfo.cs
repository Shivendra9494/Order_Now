using System;
using System.Collections.Generic;

namespace ClientAppOD.CustomModels
{
    public class StoreInfo
    {
        public int ID { get; set; }
        public string BusinessName { get; set; }
        public DateTimeOffset CurrentServerTime { get; set; }
        public bool IsOpen { get; set; }
        public DateTimeOffset? NextCollectionTime { get; set; }
        public DateTimeOffset? NextDeliveryTime { get; set; }
        public string FoodType { get; set; }
        public string Phone { get; set; }
        public bool? LayaltyEnabled { get; set; }
        public List<Discount> Discounts { get; set; }
        public decimal? ServiceCharge { get; set; }
        public string ClientId { get; set; }
        public string Address { get; set; }
        public bool DeliveryAllowed { get; set; }
        public bool CollectionAllowed { get; set; }
        public DateTimeOffset? CollectionClosedTime { get; set; }
        public DateTimeOffset? DeliveryClosedTime { get; set; }
        public string AnnoucementAboveMenu { get; set; }
        public string AnnoucementBelowMenu { get; set; }
        public DateTimeOffset MenuUpdatedOn { get; set; }
        public decimal? LoyaltyMinimumAmount { get; set; }
        public string NativeAppImage { get; set; }
    }
}
