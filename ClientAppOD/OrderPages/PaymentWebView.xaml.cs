using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ClientAppOD.CustomModels;
using ClientAppOD.Helper;
using ClientAppOD.UserPages;

using OD.Data;
using Xamarin.AppleSignIn;
using Xamarin.Forms;

namespace ClientAppOD.OrderPages
{
    public partial class PaymentWebView : ContentPage
    {
        public PaymentWebView(string URL)
        {
            InitializeComponent();
            
            webView.Source = URL;
            webView.Navigating += WebView_Navigating;
            webView.Navigated += WebView_Navigated;
            
            
        }

        private void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            Loader.IsVisible = false;
            webView.IsVisible = true;
        }

        private async void WebView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            Loader.IsVisible = true;
            webView.IsVisible = false;
            if (e.Url.ToLower().Contains("thankyou"))
            {
                var OrderId = Convert.ToInt32(e.Url.Substring(e.Url.LastIndexOf("/") + 1));
                StaticFields.CurrentOrder.ID = OrderId;
                StaticFields.CurrentOrder.PaymentType = PaymentType.Card;
                await Navigation.PushAsync(new OrderDetailPage(StaticFields.CurrentOrder,true));
                StaticFields.CurrentOrder = null;
                StaticFields.CurrentDiscount = null;
                foreach (var cat in MenuCatHelper.MenuCategories.Where(x => x.Name != "Want to repeat?" && x.SelectedQty > 0))
                {
                    foreach (var menuItem in cat.MenuItems.Where(x => x.OrderItems.Count() > 0))
                    {
                        menuItem.OrderItems = new ObservableCollection<OrderItem>();
                    }
                    cat.SelectedQty = 0;

                }
            }
            else if(e.Url.ToLower().Contains("failedpayment"))
            {
                var error = e.Url.Split('&')[1].Split('=')[1];
                MessagingCenter.Send(this, MessagingFields.FailedPayment, error);
                await Navigation.PopAsync();
            }
            else if (e.Url.ToLower().Contains("appleresponse"))
            {
                try
                {
                    var id_token1 = e.Url.Substring(e.Url.IndexOf("id_token=") + 9);
                    var id_token = id_token1.Split('&')[0];
                    AppleAccount appleAccount = new AppleAccount();
                   // appleAccount.IdToken = JwtToken.Decode(new NSString(id_token, NSStringEncoding.UTF8).ToString());
                    appleAccount.Name = "";
                    MessagingCenter.Send(this, MessagingFields.LoginFromApple, appleAccount);
                }
                finally
                {
                    await Navigation.PopAsync();
                }
            }

        }

       async void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
