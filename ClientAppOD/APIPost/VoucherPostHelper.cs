using System;
using System.Threading.Tasks;
using ClientAppOD.CustomModels;

namespace ClientAppOD.APIPost
{
    public class VoucherPostHelper
    {
        public async Task<VoucherCode> GetVoucher(string code)
        {
            try
            {
                string url = StaticFields.ServerURL + "/api/avoucher?code=" + code + "&BusinessId=" + StaticFields.CurrentStoreInfo.ID;
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                using (System.Net.WebResponse resp = await Task.Run(async () => await req.GetResponseAsync()))
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                    string response = sr.ReadToEnd().Trim();
                    response = response.Replace("k__BackingField", "").Replace("<", "").Replace(">", "");
                    var item = Newtonsoft.Json.JsonConvert.DeserializeObject<VoucherCode>(response);
                    return item;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
