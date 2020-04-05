using System;
using System.Collections.Generic;
using System.Linq;
using ClientAppOD.APIPost;
using ClientAppOD.CustomModels;
using ClientAppOD.ErrorPages;
using ClientAppOD.Helper;
using ClientAppOD.MenuPages;
using ClientAppOD.OrderPages;
using ClientAppOD.UserPages;
using Com.OneSignal;
using Com.OneSignal.Abstractions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ClientAppOD
{
    public partial class App : Application
    {
        public EventHandler<string> AppIconChanged;
        public IFacebookLoginService FacebookLoginService { get; private set; }
        
        public App(IFacebookLoginService facebookLoginService=null)
        {
            InitializeComponent();
            try
            {
              //  OneSignal.Current.StartInit("27148fa9-b69e-4ab7-bffe-40be04921ddc").InFocusDisplaying(OSInFocusDisplayOption.Notification).HandleNotificationOpened(HandleNotificationReceived).EndInit();
            }
            catch
            {

            }
            if (facebookLoginService != null)
            {
                FacebookLoginService = facebookLoginService;
            }
            //to be removed
            Preferences.Set(PreferenceFields.SelectedStoreId, 599);
            var busId = Preferences.Get(PreferenceFields.SelectedStoreId, 0);

            var FromSearchPage = Preferences.ContainsKey(PreferenceFields.FromSearchPage);
            MainPage = new NavigationPage(new MenuCategoryPage());
            //if (busId != 0 && !FromSearchPage)
            //{
            //    if (Device.RuntimePlatform == Device.Android)
            //        MainPage = new NavigationPage(new MenuCategoryPage());
            //    else if (Device.RuntimePlatform == Device.iOS)
                   
            //    MainPage = new MainTabbedPage();
            //}
            //else
            //{
            //    var id = "599";// GetRestroDetails();
               
            //    if (!string.IsNullOrEmpty(id) && id != "0" && !FromSearchPage)
            //    {
            //        Preferences.Set(PreferenceFields.SelectedStoreId, id);
            //        //DependencyService.Get<IChangeIconService>().ChangeIcon("store" + id);
            //        MainPage = new MainTabbedPage();

            //    }
            //    else
            //    {
            //        MainPage = new NavigationPage(new SearchPage());
            //    }

            //}
           


        }
        
        private async static void HandleNotificationReceived(OSNotificationOpenedResult notification)
        {
            
            try
            {
                //EmailHelper.SendMail("sunil@orderdirectly.com","Step-1","OD Native App","Notification Clicked");
                OSNotificationPayload payload = notification.notification.payload;
                Dictionary<string, object> additionalData = payload.additionalData;

                if (additionalData != null)
                {
                    
                    //EmailHelper.SendMail("sunil@orderdirectly.com", "Step-2", "OD Native App", "Notification Clicked");
                    if (additionalData.ContainsKey("orderId"))
                    {
                        string oo = additionalData["orderId"].ToString();
                        var orderId  =Convert.ToInt32(oo);
                        if (orderId > 0)
                        {

                           // EmailHelper.SendMail("sunil@orderdirectly.com", "Step-4", "OD Native App", "Notification Clicked");
                            
                            OrderPostHelper orderPostHelper = new OrderPostHelper();
                            var _order = await orderPostHelper.GetOrder(orderId);
                            try
                            {
                                var tabbedPage = App.Current.MainPage as TabbedPage;
                                var page = tabbedPage.Children[0] as NavigationPage;
                                if (page.CurrentPage.GetType() == typeof(OrderDetailPage))
                                {
                                    page.Navigation.RemovePage(page.Navigation.NavigationStack.Last());
                                    await App.Current.MainPage.Navigation.PushModalAsync(new OrderDetailPage(_order, true));
                                }
                                else
                                {
                                    page = tabbedPage.Children[1] as NavigationPage;

                                    if (page.Navigation.ModalStack.Count > 0)
                                    {
                                        var ii = (page.Navigation.ModalStack[0] as NavigationPage);
                                        if (ii.CurrentPage.GetType() == typeof(OrderDetailPage))
                                        {
                                            //ii.Navigation.RemovePage(ii.Navigation.NavigationStack.Last());
                                        }
                                        else
                                        {
                                            await App.Current.MainPage.Navigation.PushModalAsync(new OrderDetailPage(_order, true));
                                        }
                                    }
                                    //page.Navigation.RemovePage(page.Navigation.NavigationStack.Last());
                                }
                            }
                           catch
                            {

                            }
                            
                        };
                    }
                }
            }
            catch(Exception ex)
            {
                EmailHelper.SendMail("sunil@orderdirectly.com", "Error", "OD Native App", ex.Message+ "--"+ ex.StackTrace);
            }
           

        }
        string GetRestroDetails()
        {
            try
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
                url = StaticFields.ServerURL + "/api/AppRedirectInfoes?IP=" + IP;
                req = System.Net.WebRequest.Create(url);
                resp = req.GetResponse();
                sr = new System.IO.StreamReader(resp.GetResponseStream());
                response = sr.ReadToEnd().Trim();
                return response;
            }
            catch
            {
                return "";
            }



        }
        public void ChangeIcon(string icon)
        {
            AppIconChanged?.Invoke(this, icon);
        }
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
