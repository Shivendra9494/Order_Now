using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClientAppOD.CustomModels;
using Newtonsoft.Json;

namespace ClientAppOD.Helper
{
    public class PostCodeHelper
    {
        public string GetPostCodeFromIP()
        {
            string url = "http://checkip.dyndns.org";
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            string response = sr.ReadToEnd().Trim();
            string[] a = response.Split(':');
            string a2 = a[1].Substring(1);
            string[] a3 = a2.Split('<');
            string IP = a3[0];
            return GetUserCountryByIp(IP);
        }
        public string GetUserCountryByIp(string ip)
        {
            IpInfo ipInfo = new IpInfo();
            try
            {
                string info = new WebClient().DownloadString("http://ipinfo.io/" + ip);
                ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
                RegionInfo myRI1 = new RegionInfo(ipInfo.Country);
                ipInfo.Country = myRI1.EnglishName;
            }
            catch (Exception)
            {
                ipInfo.Country = null;
            }

            return ipInfo.Postal;
        }
        public string GetPostCodeInfo(string postcode)
        {
            try
            {
                var postcodePattern = @"^(GIR 0AA)|[a-z-[qvx]](?:\d|\d{2}|[a-z-[qvx]]\d|[a-z-[qvx]]\d[a-z-[qvx]]|[a-z-[qvx]]\d{2})(?:\s?\d[a-z-[qvx]]{2})?$";

                var preg = new Regex(postcodePattern, RegexOptions.IgnoreCase);
                if(preg.IsMatch(postcode))
                {
                    return "200";
                }
                else
                {
                    return null;
                }
                //string url = "https://api.postcodes.io/postcodes/"+postcode.Replace(" ","");
                //System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                //using (System.Net.WebResponse resp = await Task.Run(async () => await req.GetResponseAsync()))
                //{
                    //System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                    //string response = sr.ReadToEnd().Trim();
                    
                    //return response;
                //}
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<List<DeliveryInfo>> GetDeliveryInfo(string postcode)
        {
            try
            {
                string url = StaticFields.ServerURL + "/api/ABusinessDetail?IsTest=true&postcode=" + postcode + "&busId=" + StaticFields.CurrentStoreInfo.ID;
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                using (System.Net.WebResponse resp = await Task.Run(async () => await req.GetResponseAsync()))
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                    string response = sr.ReadToEnd().Trim();
                    var deliveryInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DeliveryInfo>>(response);
                    return deliveryInfo;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
    public class IpInfo
    {

        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("loc")]
        public string Loc { get; set; }

        [JsonProperty("org")]
        public string Org { get; set; }

        [JsonProperty("postal")]
        public string Postal { get; set; }
    }
}
