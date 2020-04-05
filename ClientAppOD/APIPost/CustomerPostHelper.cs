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
using Xamarin.Essentials;

namespace ClientAppOD.APIPost
{
    public class CustomerPostHelper
    {
        public async Task<Customer> GetCustomer(string email,string password)
        {
            try
            {
                string url = StaticFields.ServerURL + "/api/AUser?turnd=" + email + "&mrund=" + password + "&busId=" + StaticFields.CurrentStoreInfo.ID;

                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                AddAuthorizationHeaderOnLogin(email, password, req);
                using (System.Net.WebResponse resp = await Task.Run(async () => await req.GetResponseAsync()))
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                    string response = sr.ReadToEnd().Trim();
                    response = response.Replace("k__BackingField", "").Replace("<", "").Replace(">", "");
                    var Customer = Newtonsoft.Json.JsonConvert.DeserializeObject<Customer>(response);

                    return Customer;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static void AddAuthorizationHeader(WebRequest req)
        {
            if(Preferences.ContainsKey(PreferenceFields.CustomerEmail) && Preferences.ContainsKey(PreferenceFields.CustomerPassword))
            {
                string credidentials =Preferences.Get(PreferenceFields.CustomerEmail,"") + ":" + Preferences.Get(PreferenceFields.CustomerPassword, "");
                var authorization = Convert.ToBase64String(Encoding.Default.GetBytes(credidentials));
                req.Headers["Authorization"] = "Basic " + authorization;
            }
            
        }
        private static void AddAuthorizationHeaderOnLogin(string email, string password, WebRequest req)
        {
            string credidentials = email + ":" + password;
            var authorization = Convert.ToBase64String(Encoding.Default.GetBytes(credidentials));
            req.Headers["Authorization"] = "Basic " + authorization;
        }
        public async Task<int> GetCustomerExist(string email)
        {
            try
            {
                string url = StaticFields.ServerURL + "/api/AUser?email=" + email;
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                using (System.Net.WebResponse resp = await Task.Run(async () => await req.GetResponseAsync()))
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                    string response = sr.ReadToEnd().Trim();
                    response = response.Replace("k__BackingField", "").Replace("<", "").Replace(">", "");
                    var Customer = Newtonsoft.Json.JsonConvert.DeserializeObject<int>(response);
                    return Customer;
                }
            }
            catch
            {
                return 0;
            }
        }
        public async Task<int> UpdateCustomerResetCode(int Id,string resetCode)
        {
            try
            {
                string url = StaticFields.ServerURL + "/api/AUser?Id=" + Id+ "&resetCode="+resetCode+ "&IsReset=true";
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                using (System.Net.WebResponse resp = await Task.Run(async () => await req.GetResponseAsync()))
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                    string response = sr.ReadToEnd().Trim();
                    response = response.Replace("k__BackingField", "").Replace("<", "").Replace(">", "");
                    var Customer = Newtonsoft.Json.JsonConvert.DeserializeObject<int>(response);
                    return Customer;
                }
            }
            catch
            {
                return 0;
            }
        }
        public async Task<Customer> PostCustomer(Customer customer)
        {
            try
            {
                WebResponse myWebResponse;
                Stream responseStream;

                var myUri = new Uri(StaticFields.ServerURL + "/api/AUser");
                var myWebRequest = WebRequest.Create(myUri);
                var myHttpWebRequest = (HttpWebRequest)myWebRequest;
                var data = JsonConvert.SerializeObject(customer);
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
                var myStreamReader = new StreamReader(responseStream, Encoding.Default);
                var json = myStreamReader.ReadToEnd();
                var oMycustomclassname = Newtonsoft.Json.JsonConvert.DeserializeObject<Customer>(json);
                responseStream.Close();
                return oMycustomclassname;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public async Task<Customer> GetCustomer(int Id)
        {
            try
            {
                string url = StaticFields.ServerURL + "/api/AUser?id="+Id+"&busId="+StaticFields.CurrentStoreInfo.ID;
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                AddAuthorizationHeader(req);
                using (System.Net.WebResponse resp = await Task.Run(async () => await req.GetResponseAsync()))
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                    string response = sr.ReadToEnd().Trim();
                    response = response.Replace("k__BackingField", "").Replace("<", "").Replace(">", "");
                    var Customer = Newtonsoft.Json.JsonConvert.DeserializeObject<Customer>(response);
                    return Customer;
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public async Task<Customer> UpdateCustomer(int Id, string Phone, string Address1, string Address2, string City, string postcode)
        {
            try
            {
                string url = StaticFields.ServerURL + "/api/AUser?id=" + Id + "&Phone=" + Phone + "&Address1=" + Address1 + "&Address2=" + Address2 + "&City=" + City + "&postcode=" + postcode;
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                AddAuthorizationHeader(req);
                using (System.Net.WebResponse resp = await Task.Run(async () => await req.GetResponseAsync()))
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                    string response = sr.ReadToEnd().Trim();
                    response = response.Replace("k__BackingField", "").Replace("<", "").Replace(">", "");
                    var Customer = Newtonsoft.Json.JsonConvert.DeserializeObject<Customer>(response);
                    return Customer;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<Customer> GetCustomer(string name, string email, string pswd, int busId, string postcode,bool FromApple=false)
        {
            try
            {
                string url = StaticFields.ServerURL + "/api/AUser?name=" + name + "&email=" + email + "&pswd=" + pswd + "&busId=" + busId + "&postcode=" + postcode+"&FromApple=" + FromApple;
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                AddAuthorizationHeaderOnLogin(email, pswd, req);
                using (System.Net.WebResponse resp = await Task.Run(async () => await req.GetResponseAsync()))
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                    string response = sr.ReadToEnd().Trim();
                    response = response.Replace("k__BackingField", "").Replace("<", "").Replace(">", "");
                    var Customer = Newtonsoft.Json.JsonConvert.DeserializeObject<Customer>(response);
                    return Customer;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<int> DeleteCustomerCard(int CardId)
        {
            try
            {
                string url = StaticFields.ServerURL + "/api/AUserCard?CardId=" + CardId;
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                AddAuthorizationHeader(req);
                using (System.Net.WebResponse resp = await Task.Run(async () => await req.GetResponseAsync()))
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                    string response = sr.ReadToEnd().Trim();
                    var ret = Newtonsoft.Json.JsonConvert.DeserializeObject<int>(response);
                    return ret;
                }
            }
            catch
            {
                return 0;
            }
        }

        public async Task<Customer> GetSocialCustomer(string FacebookId, string Email, string Name, string Phone, string postcode, int busId, bool IsSocial)
        {
            try
            {
                string url = StaticFields.ServerURL + "/api/AUser?FacebookId=" + FacebookId + "&Email=" + Email + "&Name=" + Name + "&Phone=" + Phone  + "&postcode=" + postcode + "&busId=" + busId + "&IsSocial=" + IsSocial;
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                AddAuthorizationHeaderOnLogin(Email, FacebookId, req);
                using (System.Net.WebResponse resp = await Task.Run(async () => await req.GetResponseAsync()))
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                    string response = sr.ReadToEnd().Trim();
                    response = response.Replace("k__BackingField", "").Replace("<", "").Replace(">", "");
                    var Customer = Newtonsoft.Json.JsonConvert.DeserializeObject<Customer>(response);
                    return Customer;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
