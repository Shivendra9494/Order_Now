using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClientAppOD.CustomModels;
using ClientAppOD.Helper;
using Newtonsoft.Json;
using OD.Data;

namespace ClientAppOD.APIPost
{
    public class OrderPostHelper
    {
        public async static Task<string> PostOrder(Order order)
        {
            if(string.IsNullOrEmpty(order.Notes))
            {
                order.Notes = "";
            }
            foreach(var item in order.OrderItems)
            {
                foreach(var model in item.OrderModels.Where(x=>x.ItemName==null))
                {
                    model.ItemName = " ";
                }
            }
            WebResponse myWebResponse;
            Stream responseStream;

            var myUri = new Uri(StaticFields.ServerURL + "/api/AOrder");
            var myWebRequest = WebRequest.Create(myUri);
            var myHttpWebRequest = (HttpWebRequest)myWebRequest;
            var data = JsonConvert.SerializeObject(order);
            
            myHttpWebRequest.Method = "POST";

            //var data = Encoding.ASCII.GetBytes(postData);
            myHttpWebRequest.Accept = "application/json";
            myHttpWebRequest.ContentType = "application/json";

            using (var streamWriter = new StreamWriter(await Task.Run(async () => await myHttpWebRequest.GetRequestStreamAsync())))
            {
                streamWriter.Write(data);
            }

            myWebResponse = await Task.Run(async () => await myWebRequest.GetResponseAsync());
            responseStream = myWebResponse.GetResponseStream();
            //if (responseStream == null) return null;

            var myStreamReader = new StreamReader(responseStream, Encoding.Default);
            var json = myStreamReader.ReadToEnd();
            var oMycustomclassname = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(json);
            responseStream.Close();
            return oMycustomclassname;
        }
       
        public async Task<Order> GetOrder(int OrderId)
        {
            try
            {
                string url = StaticFields.ServerURL + "/api/AOrder?OrderId=" + OrderId;
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                using (System.Net.WebResponse resp = await Task.Run(async () => await req.GetResponseAsync()))
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                    string response = sr.ReadToEnd().Trim();
                    response = response.Replace("k__BackingField", "").Replace("<", "").Replace(">", "");
                    var Customer = Newtonsoft.Json.JsonConvert.DeserializeObject<Order>(response);
                    return Customer;
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<int> UpdatedOrder(int OrderId,string status,string TransactionId)
        {
            try
            {
                string url = StaticFields.ServerURL + "/api/AOrder?orderId=" + OrderId+"&status="+status + "&TransactionId=" + TransactionId;
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                using (System.Net.WebResponse resp = await Task.Run(async () => await req.GetResponseAsync()))
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
                return 0;
            }
        }
    }
}
