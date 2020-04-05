using System;
using System.Threading.Tasks;
using ClientAppOD.CustomModels;

namespace ClientAppOD.APIPost
{
    public class TimeHelper
    {
        
        //public async static Task<DateTime> GetTime()
        //{
        //    //string url = StaticFields.ServerURL + "/api/AServerTime";
        //    //System.Net.WebRequest req = System.Net.WebRequest.Create(url);
        //    //using (System.Net.WebResponse resp = await Task.Run(async () => await req.GetResponseAsync()))
        //    //{
        //    //    System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
        //    //    string response = sr.ReadToEnd().Trim();
        //    //    var item = Newtonsoft.Json.JsonConvert.DeserializeObject<DateTime>(response);
        //    //    return item;
        //    //}
        //    return orderDate;
        //}
        public DateTime CalTime(string responceFromPrinter,DateTime orderDate)
        {
            DateTime ret =orderDate;
            try
            {
                int hour = 0;
                int min = 0;
                responceFromPrinter = responceFromPrinter.ToLower().Replace("_", "").Replace("hr", "h-h").Replace("mins", "m-m").Replace("minutes", "m-m");
                if (responceFromPrinter.Contains(":"))
                {
                    int hr = Convert.ToInt32(responceFromPrinter.Split(':')[0]);
                    int mn = Convert.ToInt32(responceFromPrinter.Split(':')[1]);
                    DateTime dt = new DateTime(orderDate.Year, orderDate.Month, orderDate.Day, hr, mn, 0);
                    if (dt.Hour <= 5)
                    {
                        dt = dt.AddDays(1);
                    }

                    ret = dt;
                }
                else if (responceFromPrinter.Contains("m-m") && responceFromPrinter.Contains("h-h"))
                {
                    min = Convert.ToInt32(responceFromPrinter.Replace("m-m", "M").Split('M')[0].Replace("h-h", "H").Split('H')[1]);
                    hour = Convert.ToInt32(responceFromPrinter.Replace("m-m", "M").Split('M')[0].Replace("h-h", "H").Split('H')[0]);
                    ret = orderDate.AddHours(hour).AddMinutes(min);
                }
                else if (responceFromPrinter.Contains("m-m"))
                {
                    min = Convert.ToInt32(responceFromPrinter.Replace("m-m", "M").Split('M')[0]);
                    ret = orderDate.AddMinutes(min);
                }
                else if (responceFromPrinter.Contains("h-h"))
                {

                    hour = Convert.ToInt32(responceFromPrinter.Replace("h-h", "H").Split('H')[0]);
                    ret = orderDate.AddHours(hour);
                }
            }
            catch
            {
                //ret = "";
            }
            return ret ;
        }
    }
}
