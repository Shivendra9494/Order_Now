using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Text;
using ClientAppOD.CustomModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OD.Data;

namespace ClientAppOD.Helper
{
    public class PaymentHelper
    {
        string merchantKey = "";
        public string GetMerchantKey()
        {
            string header = StaticFields.header;

            WebResponse myWebResponse;
            Stream responseStream;

            var myUri = new Uri(StaticFields.SagepayUrl+"merchant-session-keys");
            var myWebRequest = WebRequest.Create(myUri);
            var myHttpWebRequest = (HttpWebRequest)myWebRequest;
            myHttpWebRequest.PreAuthenticate = true;
            var data = "{\"vendorName\": \"orderdirectly\" }";
            myHttpWebRequest.Method = "POST";
            //var data = Encoding.ASCII.GetBytes(postData);
            myHttpWebRequest.Headers.Add("Authorization", "Basic " + header);
            myHttpWebRequest.Accept = "application/json";
            myHttpWebRequest.ContentType = "application/json";
            myHttpWebRequest.ContentLength = data.Length;

            using (var streamWriter = new StreamWriter(myHttpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

            myWebResponse = myWebRequest.GetResponse();
            responseStream = myWebResponse.GetResponseStream();
            //if (responseStream == null) return null;

            var myStreamReader = new StreamReader(responseStream, Encoding.Default);
            var json = myStreamReader.ReadToEnd();
            var oMycustomclassname = Newtonsoft.Json.JsonConvert.DeserializeObject<MerchantKeyDetails>(json);
            responseStream.Close();
            myWebResponse.Close();
            return oMycustomclassname.merchantSessionKey;

        }
    
        public SagepayData GetSagePayData(Order order, Address sagePayBillingAddress,  Customer customer, string reusable = "", bool save = false, string cardIdentifier="",string transactionType="Payment")
        {
            SagepayData sagepayData = new SagepayData();


            var billingAddress = new BillingAddress()
            {
                address1 = sagePayBillingAddress.Address1,
                city = sagePayBillingAddress.City,

                country = "GB",
                postalCode = sagePayBillingAddress.PostCode
            };
            var shippingDetails = new ShippingDetails()
            {
                recipientFirstName = StaticFields.CurrentStoreInfo.ClientId,
                recipientLastName = "-",
                shippingAddress1 = sagePayBillingAddress.Address1,
                shippingCity = sagePayBillingAddress.City,
                shippingCountry = "GB",
                shippingPostalCode = sagePayBillingAddress.PostCode
            };


            PaymentMethod paymentMethod = new PaymentMethod()
            {
                card = new Card()
                {
                    cardIdentifier = cardIdentifier,
                    merchantSessionKey = merchantKey,
                    reusable=reusable
                }
            };
            if (reusable != null)
            {
                paymentMethod.card.reusable = reusable;
            }
            else
            {
                paymentMethod.card.save = save.ToString().ToLower();
            }
            //sagepayData.basket = basket;

            //sagepayData.deliveryAddress = sagePayDeliveryAddress;
            sagepayData.paymentMethod = paymentMethod;
            sagepayData.currency = "GBP";
            sagepayData.customerEmail = order.Email;
            sagepayData.transactionType = transactionType;
            sagepayData.vendorTxCode = order.ID.ToString();
            //sagepayData.vendorData = store.Name;
            sagepayData.description = StaticFields.CurrentStoreInfo.BusinessName;
            string FName = customer.FirstName.Length >= 20 ? customer.FirstName.Substring(0, 20) : customer.FirstName;

            sagepayData.customerFirstName = FName;
            sagepayData.customerLastName = string.IsNullOrEmpty(customer.LastName.Trim()) ? "-" : customer.LastName;
            sagePayBillingAddress.Surname = customer.LastName;
            sagePayBillingAddress.Firstnames = FName;
            sagePayBillingAddress.Phone = order.Phone;
            sagepayData.billingAddress = billingAddress;
            sagepayData.shippingDetails = shippingDetails;
            sagepayData.amount = Convert.ToInt32((order.SubTotal * 100));
            sagepayData.customerPhone = order.Phone;
            return sagepayData;
        }
        public string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public string GetCardIdentifier(string url, string data)
        {
            try
            {
                WebResponse myWebResponse;
                Stream responseStream;
                merchantKey = GetMerchantKey();
                var myWebRequest = WebRequest.Create(url);
                var myHttpWebRequest = (HttpWebRequest)myWebRequest;
                myHttpWebRequest.PreAuthenticate = true;
                myHttpWebRequest.Method = "POST";
                //var data = Encoding.ASCII.GetBytes(postData);
                myHttpWebRequest.Headers.Add("Authorization", "Bearer " + merchantKey);
                myHttpWebRequest.Accept = "application/json";
                myHttpWebRequest.ContentType = "application/json";
                myHttpWebRequest.ContentLength = data.Length;

                using (var streamWriter = new StreamWriter(myHttpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(data);
                }

                myWebResponse = myWebRequest.GetResponse();
                responseStream = myWebResponse.GetResponseStream();
                //if (responseStream == null) return null;

                var myStreamReader = new StreamReader(responseStream, Encoding.Default);
                var json = myStreamReader.ReadToEnd();
                var oMycustomclassname = Newtonsoft.Json.JsonConvert.DeserializeObject<Card>(json);
                responseStream.Close();
                myWebResponse.Close();
                return oMycustomclassname.cardIdentifier;
                
            }
            catch (WebException e)
            {
                try
                {
                    var reader = new StreamReader(e.Response.GetResponseStream());
                    var content1 = reader.ReadToEnd();
                    content1 = content1.Substring(content1.LastIndexOf("clientMessage\":") + 16);
                    content1 = content1.Substring(0, content1.IndexOf("\",\"code"));
                    return content1;
                }
                catch
                {
                    return "Error";
                }
                
            }
        }
        public string CheckCVV(string url, string data)
        {
            try
            {
                WebResponse myWebResponse;
                Stream responseStream;
                merchantKey = GetMerchantKey();
                var myWebRequest = WebRequest.Create(url);
                var myHttpWebRequest = (HttpWebRequest)myWebRequest;
                myHttpWebRequest.PreAuthenticate = true;
                myHttpWebRequest.Method = "POST";
                //var data = Encoding.ASCII.GetBytes(postData);
                myHttpWebRequest.Headers.Add("Authorization", "Bearer " + merchantKey);
                myHttpWebRequest.Accept = "application/json";
                myHttpWebRequest.ContentType = "application/json";
                myHttpWebRequest.ContentLength = data.Length;

                using (var streamWriter = new StreamWriter(myHttpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(data);
                }

                myWebResponse = myWebRequest.GetResponse();
                responseStream = myWebResponse.GetResponseStream();
                //if (responseStream == null) return null;

                var myStreamReader = new StreamReader(responseStream, Encoding.Default);
                var json = myStreamReader.ReadToEnd();
                var oMycustomclassname = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(json);
                responseStream.Close();
                myWebResponse.Close();
                return oMycustomclassname;

            }
            catch (WebException e)
            {
                try
                {
                    var reader = new StreamReader(e.Response.GetResponseStream());
                    var content1 = reader.ReadToEnd();
                    content1 = content1.Substring(content1.LastIndexOf("clientMessage\":") + 16);
                    content1 = content1.Substring(0, content1.IndexOf("\",\"code"));
                    return content1;
                }
                catch
                {
                    return "Error";
                }
            }
        }
    }
}
