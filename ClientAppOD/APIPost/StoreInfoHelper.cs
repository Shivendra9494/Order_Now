using System;
using System.Threading.Tasks;
using ClientAppOD.CustomModels;
using Newtonsoft.Json;

namespace ClientAppOD.APIPost
{
    public class StoreInfoHelper
    {
        public async Task<StoreInfo> GetStoreInfo(int Id)
        {
            try
            {
                string url = StaticFields.ServerURL + "/api/ABusinessDetail/" + Id;
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                using (System.Net.WebResponse resp = await Task.Run(async () => await req.GetResponseAsync()))
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                    string response = sr.ReadToEnd().Trim();
                     
                    var microsoftDateFormatSettings = new JsonSerializerSettings
                    {
                        DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                        DateTimeZoneHandling = DateTimeZoneHandling.Local
                    };
                    var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<StoreInfo>(response,microsoftDateFormatSettings);
                    return obj;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}

