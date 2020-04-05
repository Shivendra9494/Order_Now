using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientAppOD.CustomModels;
using ClientAppOD.Helper;
using ClientAppOD.OrderPages;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace ClientAppOD.UserPages
{
    public partial class UserMainMenuPage : ContentPage
    {
        public UserMainMenuPage()
        {
            InitializeComponent();
              MessagingCenter.Subscribe<UserLoginPage, int>(this, MessagingFields.UserLoggedInChanged, Updatestack);
            stackLogin.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => HandleLogin())
            });
            stackUserOrderHistory.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => PushAction(new OrderHistoryPage()))
            });
            stackProfile.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => PushAction(new UserProfilePage()))
            });
            stackInfo.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => PushAction(new PaymentWebView("https://orderdirectly.biz/info.asp?id_c=" + StaticFields.CurrentStoreInfo.ClientId)))
            });
            stackDirection.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => PushAction(new DirectionPage()))
            });
            stackCard.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => PushAction(new SavedCardsPage()))
            });
            stackCall.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => callStore())
            });
            stackTerms.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => PushAction(new PaymentWebView("https://www.orderdirectly.com/terms/")))
            });
            stackPrivacy.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => PushAction(new PaymentWebView("https://www.orderdirectly.com/privacypolicy/")))
            });
            stackEULA.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => PushAction(new PaymentWebView("https://www.orderdirectly.com/privacypolicy/")))
            });
            stackCookies.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => PushAction(new PaymentWebView("https://www.orderdirectly.com/privacypolicy/")))
            });
            stackIconChange.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => ChangeIconToOriginal())
            });
            var name = UIApplication.SharedApplication.AlternateIconName;
            var FromSearchPage = Preferences.ContainsKey(PreferenceFields.FromSearchPage);
            if (name!=null && FromSearchPage)
            {
                stackIconChange.IsVisible = true;
            }
        }
        protected override void OnAppearing()
        {
            Loader.IsVisible = true;
            stackMain.IsVisible = false;
            Updatestack();
            
            stackMain.IsVisible = true;
            Loader.IsVisible = false;
            base.OnAppearing();
        }
        private void callStore()
        {
            Device.OpenUri(new Uri("tel:"+StaticFields.CurrentStoreInfo.Phone));
        }
        private void ChangeIconToOriginal()
        {
            var app = (App)Xamarin.Forms.Application.Current;
            app.ChangeIcon(null);
            stackIconChange.IsVisible = false;
        }
        private async Task HandleLogin()
        {
            if(StaticFields.CurrentCustomer==null)
            {
                var page = new Xamarin.Forms.NavigationPage(new UserLoginPage(true))
                {
                    BarTextColor = Color.FromHex("236adb")
                };
                page.On<iOS>().SetModalPresentationStyle(Xamarin.Forms.PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.FormSheet);
                await Navigation.PushModalAsync(page,true);
            }
            else
            {
                StaticFields.CurrentCustomer = null;
                StaticFields.CurrentDiscount = null;
                Preferences.Set(PreferenceFields.CustomerId, 0);
                Updatestack();
                MessagingCenter.Send(this, MessagingFields.UserLoggedOutChanged, 1);
                try
                {
                     var facebookLoginService = (Xamarin.Forms.Application.Current as App).FacebookLoginService;
                    facebookLoginService.Logout();
                }
                catch

                {

                }

            }
        }
        private void Updatestack()
        {
            if (StaticFields.CurrentCustomer == null)
            {
                lblLogin.Text = "Login";
                stackUserDetails.IsVisible = false;
                stackUserHistory.IsVisible = false;
            }
            else
            {
                lblLogin.Text = "Logout";
                lblUserName.Text = StaticFields.CurrentCustomer.FirstName;
                lblUserEmail.Text = StaticFields.CurrentCustomer.Email;
                stackUserDetails.IsVisible = true;
                stackUserHistory.IsVisible = true;
            }
        }
        private void Updatestack(UserLoginPage sender, int i)
        {
            if (StaticFields.CurrentCustomer == null)
            {
                lblLogin.Text = "Login";
                stackUserDetails.IsVisible = false;
                stackUserHistory.IsVisible = false;
            }
            else
            {
                lblLogin.Text = "Logout";
                lblUserName.Text = StaticFields.CurrentCustomer.FirstName;
                lblUserEmail.Text = StaticFields.CurrentCustomer.Email;
                stackUserDetails.IsVisible = true;
                stackUserHistory.IsVisible = true;
            }
        }
        private async Task PushAction(Xamarin.Forms.Page page)
        {
            await Navigation.PushAsync(page);
        }
   }
}
