using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientAppOD.Helper;
using OD.Data;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ClientAppOD.CustomModels
{
    public class StaticFields
    {
        private static PaymentHelper paymentHelper = new PaymentHelper();
        public const string ServerURL = "https://orderdirectly.biz";
        public const string SagepayUrl = "https://pi-test.sagepay.com/api/v1/";
        public const string AppleLoginId = "com.orderitnow";
        public const string BGImageName = "bgfood1.jpg";
        public const string SmallImageName = "store3.png";
        public const int CurrentLocalVersion = 1;
        public static string CurrentPostCode { get; set; }

        public static Order CurrentOrder { get; set; }
        public static Discount CurrentDiscount { get; set; }
        public static Customer CurrentCustomer { get; set; }
        public static Store CurrentStore { get; set; }
        public static StoreInfo CurrentStoreInfo { get; set; }
        public static DeliveryInfo CurrentDeliveryInfo { get; set; }
        public static List<DeliveryInfo> CurrentDeliveryInfoList { get; set; }
        public static string Deliverytype { get; set; }
        public static string EmailFrom { get; set; }
        public static string PushMessageUserId { get; set; }
        public static ImageSource TopImage { get; set; }
        
        public static bool IconChangedFromSearchPage { get; set; }
        
        public static string header
        {
            get
            {
                //test
                return paymentHelper.Base64Encode("Tx8W2Fj52Z35WrgWsBQKd8JVz1PWKsRJYTkya4V6X6nllaobCj:u4JnJ4YVR08cbrjCv75BEtrNFQvM3RZWZYkPz6QE1QvYI0PglXCSlgO4qAu8UEosb");
               
            }
        }
        public static  bool ChangeToWebView
        {
            get
            {
                return GetGlobalSet();
            }
        }
        public static int CurrentServerVerion
        {
            get
            {
                return GetServerVersion();
            }
        }
        public static bool GetGlobalSet()
        {
            try
            {
                var Busid = Preferences.Get(PreferenceFields.SelectedStoreId, 0);
                string url = StaticFields.ServerURL + "/api/Globalset/get?BusId="+Busid+"&version="+StaticFields.CurrentLocalVersion;
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                using (System.Net.WebResponse resp = req.GetResponse())
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                    string response = sr.ReadToEnd().Trim();
                    response = response.Replace("k__BackingField", "").Replace("<", "").Replace(">", "");
                    var value = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(response);
                    return value;
                }
            }
            catch
            {
                return true;
            }
        }
        public static int GetServerVersion()
        {
            try
            {
                var Busid = Preferences.Get(PreferenceFields.SelectedStoreId, 0);
                string url = StaticFields.ServerURL + "/api/NativeAppVersion/get";
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                using (System.Net.WebResponse resp = req.GetResponse())
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                    string response = sr.ReadToEnd().Trim();
                    response = response.Replace("k__BackingField", "").Replace("<", "").Replace(">", "");
                    var value = Newtonsoft.Json.JsonConvert.DeserializeObject<int>(response);
                    return value;
                }
            }
            catch
            {
                return 1;
            }
        }
    }
}
