using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientAppOD.CustomModels;
using ClientAppOD.Helper;
using ClientAppOD.MenuPages;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using SQLite;
using Xamarin.Essentials;
using Xamarin.Forms;
using PermissionStatus = Plugin.Permissions.Abstractions.PermissionStatus;

namespace ClientAppOD
{
    public partial class SearchPage : ContentPage
    {
        StoreListHelpers helper = new StoreListHelpers();
        List<Store> businessDetail = new List<Store>();
        private SQLiteAsyncConnection _connection;

        public SearchPage()
        {
            Preferences.Set(PreferenceFields.FromSearchPage, true);
            InitializeComponent();
            
            RefreshDataAsync();
            
           // _connection = DependencyService.Get<ISQLiteLdb>().GetConnection();
            MainList.ItemsSource = businessDetail;
            
        }
        protected async override void OnAppearing()
        {
            //await GetLocation();
            base.OnAppearing();
            MainList.IsVisible = true;
            loader.IsVisible = false;
            MenuCatHelper.MenuCategories = null;
            StaticFields.CurrentDeliveryInfo = null;
            StaticFields.CurrentDiscount = null;
            StaticFields.CurrentOrder = null;
            StaticFields.CurrentPostCode = null;
            StaticFields.CurrentStore = null;
            StaticFields.CurrentStoreInfo = null;

        }
        
        
        void HandleSearch(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                MainList.ItemsSource = businessDetail;
            }
            else
            {
                MainList.ItemsSource = businessDetail.Where(x => x.Name.ToLower().Contains(e.NewTextValue.ToLower()) || x.PostalCode.ToLower().Contains(e.NewTextValue.ToLower()));
            }
        }
        public string RefreshDataAsync(string searchText = null)
        {
            string url = StaticFields.ServerURL + "/api/business/get";
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            string response = sr.ReadToEnd().Trim();
            //content = content.Replace(">k__BackingField", "").Replace("<", "");
            businessDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Store>>(response);
            

            return "";
        }
        private async void LaunchTapped(object sender, ItemTappedEventArgs e)
        {
            
            var store = e.Item as Store;
            if(MainList.SelectedItem!=null)
            {
                MainList.SelectedItem = null;
            }
            //var rest = await _connection.Table<SelectedStore>().FirstOrDefaultAsync();
           
                    var rest = new SelectedStore()
                    {
                        ID = store.ID.ToString()
                    };
                    //var tt = await _connection.InsertAsync(rest);
                    //var list = _connection.QueryAsync<SelectedStore>("select * from SelectedStore");
                    //if (list.Result.Count > 0)
                    //{
                        //var ee = "";
                    //}
                   Preferences.Set(PreferenceFields.SelectedStoreId, rest.ID);

                Preferences.Set(PreferenceFields.SelectedStoreImage, store.MinimumOrder);
                
               // DependencyService.Get<IChangeIconService>().ChangeIcon("store"+rest.ID);
                await Navigation.PushAsync(new MainTabbedPage());
                //Navigation.RemovePage(this);
            
        }
        
        async Task<string> GetLocation()
        {
            double Lot;
            double Long;
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    Lot = location.Latitude;
                    Long = location.Longitude;

                    string url = "https://api.postcodes.io/postcodes?lon=" + Long + "&lat=" + Lot;
                    System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                    System.Net.WebResponse resp = await req.GetResponseAsync();
                    System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                    string response = sr.ReadToEnd().Trim();
                    var tt = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultLocation>(response);
                    var postcode = tt.result[0].postcode.Replace(" ", "").ToUpper();
                    StaticFields.CurrentPostCode = postcode;
                    SearchBar.Text = postcode;
                    if (businessDetail.Count() > 0)
                    {
                        foreach(var item in businessDetail)
                        {
                            item.distance = distance(item.Lat, item.Long, Lot, Long);
                        }
                        MainList.ItemsSource = businessDetail.Where(x=>x.distance<30).OrderBy(x=>x.distance);

                    }
                    
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                var result = await DisplayAlert("Location not enabled", "Enabled your location", "OK", "Cancel");
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                {
                    await DisplayAlert("Need location", "Gunna need that location", "OK");
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                var status = results[Permission.Location];
                if (status != PermissionStatus.Granted)
                {
                  await DisplayAlert("Location not granted", "Please allow Order Now to access your location", "OK");
                    
                }
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
            return "";
        }
        private double distance(double lat1, double lon1, double lat2, double lon2, char unit='M')
        {
            if ((lat1 == lat2) && (lon1 == lon2))
            {
                return 0;
            }
            else
            {
                double theta = lon1 - lon2;
                double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
                dist = Math.Acos(dist);
                dist = rad2deg(dist);
                dist = dist * 60 * 1.1515;
                if (unit == 'K')
                {
                    dist = dist * 1.609344;
                }
                else if (unit == 'N')
                {
                    dist = dist * 0.8684;
                }
                return (dist);
            }
        }
        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            var app = (App)Application.Current;
            var id = (((sender as Button).Parent as StackLayout).Children[3] as Label).Text;
            app.ChangeIcon("store" + id);
        }
    }
}
