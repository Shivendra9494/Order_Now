using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ClientAppOD.APIPost;
using ClientAppOD.CustomModels;
using ClientAppOD.Helper;
using ClientAppOD.OrderPages;
using ClientAppOD.PushNotification;
using ClientAppOD.Services;
using Newtonsoft.Json;
using Plugin.FacebookClient;

using Xamarin.AppleSignIn;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ClientAppOD.UserPages
{
    public partial class UserSignUpPage : ContentPage
    {
        bool Validated = true;
        bool _FromUserDetail = false;
        IFacebookClient _facebookService = CrossFacebookClient.Current;
        CustomerPostHelper customerPostHelper = new CustomerPostHelper();
        public UserSignUpPage(bool FromUserDetail = false)
        {
            InitializeComponent();
            this.BindingContext = new LoginViewModel(this);
            _FromUserDetail = FromUserDetail;
            
            lblCookies.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => OpenBrowser("https://www.orderdirectly.com/privacypolicy/"))
            });
            lblPrivacy.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => OpenBrowser("https://www.orderdirectly.com/privacypolicy/"))
            });
            lblTerms.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => OpenBrowser("https://www.orderdirectly.com/privacypolicy/"))
            });

        }
        protected void btnCancelClicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
        protected void backClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
        protected void btnLoginclicked(object sender, EventArgs e)
        {
            HandleLogin();

        }
        public async Task OpenBrowser(string uri)
        {
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        //private async void SignIn_Event(object sender, EventArgs e)
        //{
        //    //AppleAccount account = null;
        //    //bool Is13 = UIDevice.CurrentDevice.CheckSystemVersion(13, 0);
        //    //if (Is13)
        //    //{
        //    //    var appleSignIn = Xamarin.Forms.DependencyService.Get<IAppleSignInService>();

        //    //    account = await appleSignIn.SignInAsync();
        //    //    await HandleLoginFromApple(account);
        //    //}
        //    //else
        //    //{
        //    //    MessagingCenter.Subscribe<PaymentWebView, AppleAccount>(this, MessagingFields.LoginFromApple, HandleLoginFromApple);
        //    //    await Navigation.PushAsync(new PaymentWebView("https://appleid.apple.com/auth/authorize?client_id="+StaticFields.AppleLoginId+"&redirect_uri=https://orderdirectly.biz/spicy/index&response_type=code%20id_token&scope=email%20name&response_mode=form_post&state=bus_" + StaticFields.CurrentStoreInfo.ID));
        //    //}


        //}

        private async Task HandleLoginFromApple(AppleAccount account)
        {
            if (account != null)
            {
                if (account.Name == "Error")
                {
                    await DisplayAlert("Error", account.RefreshToken, "OK");
                }
                else
                {
                    var EmailPayload = account.IdToken.Payload.Any(x => x.Key == "email");
                    if (EmailPayload)
                    {
                        string email = account.IdToken.Payload.FirstOrDefault(x => x.Key == "email").Value.ToString();
                        var customer = await customerPostHelper.GetCustomer(email, account.UserId);
                        string name = account.Name;
                        if (string.IsNullOrEmpty(name))
                        {
                            name = email.Split('@')[0];
                        }
                        if (customer == null)
                        {
                            customer = await customerPostHelper.GetCustomer(name, email, account.UserId, StaticFields.CurrentStoreInfo.ID, StaticFields.CurrentPostCode, true);
                            if (customer == null)
                            {
                                lblError1.IsVisible = true;
                                lblError1.Text = "something went wrong, please try again";
                            }
                            else
                            {
                                StaticFields.CurrentCustomer = customer;
                                Preferences.Set(PreferenceFields.CustomerId, customer.ID);
                                Preferences.Set(PreferenceFields.CustomerEmail, customer.Email);
                                Preferences.Set(PreferenceFields.CustomerPassword, customer.Pswd);
                                NotificationService.CreateExternalUserId(customer.ID.ToString());
                                if (!string.IsNullOrEmpty(customer.PostalCode))
                                {
                                    Preferences.Set(PreferenceFields.CustomerPostCode, customer.PostalCode);
                                }
                                if (_FromUserDetail)
                                {
                                    await Navigation.PopModalAsync();
                                }
                                else
                                {
                                    await Navigation.PushAsync(new CheckOutPage());
                                }
                                MessagingCenter.Send(this, MessagingFields.UserLoggedInChanged, 1);
                                // Navigation.RemovePage(this);
                            }
                        }
                        else
                        {
                            StaticFields.CurrentCustomer = customer;
                            Preferences.Set(PreferenceFields.CustomerId, customer.ID);
                            Preferences.Set(PreferenceFields.CustomerEmail, customer.Email);
                            Preferences.Set(PreferenceFields.CustomerPassword, customer.Pswd);
                            NotificationService.CreateExternalUserId(customer.ID.ToString());
                            if (!string.IsNullOrEmpty(customer.PostalCode))
                            {
                                Preferences.Set(PreferenceFields.CustomerPostCode, customer.PostalCode);
                            }
                            if (_FromUserDetail)
                            {
                                await Navigation.PopModalAsync();
                            }
                            else
                            {
                                await Navigation.PushAsync(new CheckOutPage());
                            }
                            MessagingCenter.Send(this, MessagingFields.UserLoggedInChanged, 1);
                            // Navigation.RemovePage(this);

                        }
                    }
                }
            }
        }
        private void HandleLoginFromApple(PaymentWebView sender, AppleAccount account)
        {
            HandleLoginFromApple(account);
            MessagingCenter.Unsubscribe<PaymentWebView, int>(this, MessagingFields.LoginFromApple);
        }
        private async System.Threading.Tasks.Task HandleLogin()
        {
            if (string.IsNullOrEmpty(entryName.Text))
            {
                lblError0.IsVisible = true;
                lblError0.Text = "Name is empty";
                Validated = false;
            }
            else
            {
                lblError0.IsVisible = true;
                lblError0.Text = "";
            }
            if (string.IsNullOrEmpty(entryEmail.Text))
            {
                lblError1.IsVisible = true;

                lblError1.Text = "Email is empty";
                Validated = false;
            }
            else
            {
                lblError1.IsVisible = true;

                lblError1.Text = "";
            }

            if (string.IsNullOrEmpty(entryPassword.Text))
            {
                lblError2.IsVisible = true;
                lblError2.Text = "Password is empty";
                Validated = false;
            }
            else
            {
                lblError2.IsVisible = true;
                lblError2.Text = "";
            }
            if (!string.IsNullOrEmpty(entryEmail.Text) && !string.IsNullOrEmpty(entryPassword.Text))
            {
                Validated = true;
            }
            if (!Validated)
            {
                return;
            }
            if (!EmailHelper.IsValidEmail(entryEmail.Text))
            {
                lblError1.IsVisible = true;
                lblError1.Text = "Email is invalid";
                return;
            }
            else
            {
                Customer customerNew = new Customer()
                {
                    Email = entryEmail.Text,
                    FirstName = entryName.Text,
                    Pswd = entryPassword.Text,
                    BusinessDetailID = StaticFields.CurrentStoreInfo.ID,
                    TermsAcceptedIP = " ",
                    PostalCode = StaticFields.CurrentPostCode
                };
                var customer = await customerPostHelper.GetCustomer(customerNew.FirstName, customerNew.Email, customerNew.Pswd, customerNew.BusinessDetailID, customerNew.PostalCode);
                if (customer == null)
                {
                    lblError0.IsVisible = true;
                    lblError0.Text = "something went wrong, please try again";
                }
                else if(customer.FirstName== "Email already exist")
                {
                    lblError0.IsVisible = true;
                    lblError0.Text = "Email already exists";
                }
                else
                {
                    StaticFields.CurrentCustomer = customer;
                    Preferences.Set(PreferenceFields.CustomerId, customer.ID);
                    Preferences.Set(PreferenceFields.CustomerEmail, customer.Email);
                        Preferences.Set(PreferenceFields.CustomerPassword, entryPassword.Text);
                    NotificationService.CreateExternalUserId(customer.ID.ToString());
                    if (!string.IsNullOrEmpty(customer.PostalCode))
                    {
                        Preferences.Set(PreferenceFields.CustomerPostCode, customer.PostalCode);
                        
                    }
                    if (_FromUserDetail)
                    {
                        await Navigation.PopModalAsync();
                    }
                    else
                    {
                        await Navigation.PushAsync(new CheckOutPage());
                    }
                    MessagingCenter.Send(this, MessagingFields.UserLoggedInChanged, 1);
                    Navigation.RemovePage(this);

                }
            }
        }
        public async Task LoginFacebookAsync()
        {
            AuthNetwork authNetwork = new AuthNetwork()
            {
                Name = "Facebook",
                Icon = "ic_fb",
                Foreground = "#FFFFFF",
                Background = "#4768AD"
            };
            try
            {

                if (_facebookService.IsLoggedIn)
                {
                    _facebookService.Logout();
                }

                EventHandler<FBEventArgs<string>> userDataDelegate = null;

                userDataDelegate = async (object sender, FBEventArgs<string> e) =>
                {
                    switch (e.Status)
                    {
                        case FacebookActionStatus.Completed:
                            var facebookProfile = await Task.Run(() => JsonConvert.DeserializeObject<FacebookProfile>(e.Data));
                            var Name = $"{facebookProfile.FirstName} {facebookProfile.LastName}";
                            var customer = await customerPostHelper.GetSocialCustomer(facebookProfile.Id, facebookProfile.Email, Name, "", string.IsNullOrEmpty(StaticFields.CurrentPostCode) ? "" : StaticFields.CurrentPostCode, StaticFields.CurrentStoreInfo.ID, true);

                            StaticFields.CurrentCustomer = customer;
                            Preferences.Set(PreferenceFields.CustomerId, customer.ID);
                            NotificationService.CreateExternalUserId(customer.ID.ToString());

                            Preferences.Set(PreferenceFields.CustomerEmail, customer.Email);
                            Preferences.Set(PreferenceFields.CustomerPassword, customer.Pswd);
                            if (!string.IsNullOrEmpty(customer.PostalCode))
                            {
                                Preferences.Set(PreferenceFields.CustomerPostCode, customer.PostalCode);
                            }
                            if (_FromUserDetail)
                            {
                                await Navigation.PopModalAsync();
                            }
                            else
                            {
                                await Navigation.PushAsync(new CheckOutPage());
                            }
                            MessagingCenter.Send(this, MessagingFields.UserLoggedInChanged, 1);
                            //Navigation.RemovePage(this);

                            break;
                        case FacebookActionStatus.Canceled:
                            await DisplayAlert("Facebook Auth", "Canceled", "Ok");

                            break;
                        case FacebookActionStatus.Error:
                            await DisplayAlert("Facebook Auth", "Error", "Ok");
                            break;
                        case FacebookActionStatus.Unauthorized:
                            await DisplayAlert("Facebook Auth", "Unauthorized", "Ok");
                            break;
                    }

                    _facebookService.OnUserData -= userDataDelegate;
                };

                _facebookService.OnUserData += userDataDelegate;

                string[] fbRequestFields = { "email", "first_name", "picture", "gender", "last_name" };
                string[] fbPermisions = { "email" };
                var res = await _facebookService.RequestUserDataAsync(fbRequestFields, fbPermisions);


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
        private async Task PushAction(Page page)
        {
            await Navigation.PushAsync(page);
        }

        async void tool_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
