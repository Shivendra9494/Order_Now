using System;
namespace ClientAppOD.CustomModels
{
    public class Store
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public double? distance { get; set; }
        public string Image { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public string FoodType { get; set; }
        public string MinimumOrder { get; set; }
        public string DeliveryFee { get; set; }
        public string DeliveryTime { get; set; }

    }
    public class SelectedStore
    {
        public string ID { get; set; }
        public string Menu { get; set; }
    }
    public class OptionVisibility
    {
        public int Index { get; set; }
        public bool IsVisible { get; set; }
    }
}
