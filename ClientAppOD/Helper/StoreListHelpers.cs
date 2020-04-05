using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Xamarin.Essentials;

namespace ClientAppOD.Helper
{
    public class StoreListHelpers
    {
        public Coords? PostCodeToLongLat(string postcode)
        {
            postcode = postcode.Trim();
            // Download the XML response from Google  
            var client = new WebClient();
            var encodedPostCode = HttpUtility.UrlEncode(postcode);

            var url = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?address={0}&key=AIzaSyB6-dJF6vPzC-4gdVJINEJdYPbHSzFSXX4", encodedPostCode);
            var xml = client.DownloadString(url);
            var doc = new XmlDocument();
            doc.LoadXml(xml);



            double longitude;
            double lattitude;
            if (!Double.TryParse(doc.GetElementsByTagName("lng")[0].InnerText, out longitude)) return null;
            if (!Double.TryParse(doc.GetElementsByTagName("lat")[0].InnerText, out lattitude)) return null;

            return new Coords
            {
                Longitude = longitude,
                Latitude = lattitude
            };
        }
        
        async Task<string> GetLocation()
        {
            string t = "";
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    t = location.Latitude.ToString();
                    var yy = location.Longitude;
                    var uu = location.Altitude;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
            return "";
        }

        
    }
    public struct Coords
    {
        public double Longitude;
        public double Latitude;
    }
    public class LocationHelp
    {
        public string postcode { get; set; }
        public double distance { get; set; }
    }
    public class ResultLocation
    {
        public string status { get; set; }
        public List<LocationHelp> result { get; set; }
    }
}
