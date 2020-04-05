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
using UIKit;
using Xamarin.AppleSignIn;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ClientAppOD.UserPages
{
    public partial class UserLoginPage : ContentPage
    {
        bool Validated = true;
        bool _FromUserDetail = false;
        IFacebookClient _facebookService = CrossFacebookClient.Current;
        CustomerPostHelper customerPostHelper = new CustomerPostHelper();
        public UserLoginPage(bool FromUserDetail = false)
        {
            InitializeComponent();
            _FromUserDetail = FromUserDetail;
            this.BindingContext = new LoginViewModel(this);
            FrameFacebook.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => LoginFacebookAsync()
                )
            });
            lblCookies.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => PushAction(new PaymentWebView("https://www.orderdirectly.com/privacypolicy/")))
            });
            lblPrivacy.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => PushAction(new PaymentWebView("https://www.orderdirectly.com/privacypolicy/")))
            });
            lblTerms.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => PushAction(new PaymentWebView("https://www.orderdirectly.com/privacypolicy/")))
            });

        }
        protected void btnCancelClicked(object sender, EventArgs e)
        {
            //MessagingCenter.Unsubscribe<UserLoginPage, bool>(this, MessagingFields.UserLoggedInChangedBasket);
            Navigation.PopModalAsync();
        }
        protected void btnLoginclicked(object sender, EventArgs e)
        {
            HandleLogin();

        }
        private async void SignIn_Event(object sender, EventArgs e)
        {
            AppleAccount account = null;
            bool Is13 = UIDevice.CurrentDevice.CheckSystemVersion(13, 0);
            if (Is13)
            {
                var appleSignIn = Xamarin.Forms.DependencyService.Get<IAppleSignInService>();

                account = await appleSignIn.SignInAsync();
                await HandleLoginFromApple(account);
            }
            else
            {
                MessagingCenter.Subscribe<PaymentWebView, AppleAccount>(this, MessagingFields.LoginFromApple, HandleLoginFromApple);
                await Navigation.PushAsync(new PaymentWebView("https://appleid.apple.com/auth/authorize?client_id=" + StaticFields.AppleLoginId + "&redirect_uri=https://orderdirectly.biz/spicy/index&response_type=code%20id_token&scope=email%20name&response_mode=form_post&state=bus_" + StaticFields.CurrentStoreInfo.ID));
            }


        }

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
                                MessagingCenter.Send(this, MessagingFields.UserLoggedInChanged, 1);
                                if (_FromUserDetail)
                                {
                                    await Navigation.PopModalAsync();
                                    MessagingCenter.Send(this, MessagingFields.UserLoggedInChangedOrders, true);
                                }
                                else
                                {
                                    await Navigation.PopModalAsync();
                                    MessagingCenter.Send(this, MessagingFields.UserLoggedInChangedBasket, true);
                                    //MessagingCenter.Unsubscribe<UserLoginPage, bool>(this, MessagingFields.UserLoggedInChangedBasket);

                                }

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
                            MessagingCenter.Send(this, MessagingFields.UserLoggedInChanged, 1);
                            if (_FromUserDetail)
                            {
                                await Navigation.PopModalAsync();
                                MessagingCenter.Send(this, MessagingFields.UserLoggedInChangedOrders, true);
                            }
                            else
                            {
                                await Navigation.PopModalAsync();
                                MessagingCenter.Send(this, MessagingFields.UserLoggedInChangedBasket, true);
                                //MessagingCenter.Unsubscribe<UserLoginPage, bool>(this, MessagingFields.UserLoggedInChangedBasket);

                            }

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
        protected void btnForgotPasswordClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ForgetPasswordPage());
        }
        protected void btnSignUpClicked(object sender, EventArgs e)
        {
            MessagingCenter.Subscribe<UserSignUpPage, int>(this, MessagingFields.UserLoggedInChanged, UpdateStoreInfoFromSignUP);

            Navigation.PushAsync(new UserSignUpPage(_FromUserDetail), true);
        }
        private async System.Threading.Tasks.Task HandleLogin()
        {
            if (string.IsNullOrEmpty(entryEmail.Text))
            {
                lblError1.Text = "Email is empty";
                Validated = false;
                lblEmail.TextColor = Color.FromHex("EB6361");
            }
            else
            {
                lblError1.Text = "";
                lblEmail.TextColor = Color.FromHex("2E3032");
            }

            if (string.IsNullOrEmpty(entryPassword.Text))
            {
                lblError2.Text = "Password is empty";
                Validated = false;
                lblPassword.TextColor = Color.FromHex("EB6361");
            }
            else
            {
                lblError2.Text = "";
                lblPassword.TextColor = Color.FromHex("2E3032");
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
                lblError1.Text = "Email is invalid";
                return;
            }
            else
            {
                var customer = await customerPostHelper.GetCustomer(entryEmail.Text, entryPassword.Text);
                if (customer == null)
                {
                    await DisplayAlert("We couldn't log you in", "your email or password may be " +
                        "wrong or out of date. Please try again or reset your paasword", "Reset Password", "Ok");
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
                    MessagingCenter.Send(this, MessagingFields.UserLoggedInChanged, 1);
                    if (_FromUserDetail)
                    {

                        await Navigation.PopModalAsync();
                        MessagingCenter.Send(this, MessagingFields.UserLoggedInChangedOrders, true);
                    }
                    else
                    {
                        await Navigation.PopModalAsync();
                        MessagingCenter.Send(this, MessagingFields.UserLoggedInChangedBasket, true);
                        //MessagingCenter.Unsubscribe<UserLoginPage, bool>(this, MessagingFields.UserLoggedInChangedBasket);


                    }

                    Navigation.RemovePage(this);

                }
            }
        }
        private async void UpdateStoreInfoFromSignUP(UserSignUpPage sender, int i)
        {
            MessagingCenter.Send(this, MessagingFields.UserLoggedInChanged, 1);
            if (!_FromUserDetail)
            {
                MessagingCenter.Send(this, MessagingFields.UserLoggedInChangedBasket, true);
                //MessagingCenter.Unsubscribe<UserLoginPage, bool>(this, MessagingFields.UserLoggedInChangedBasket);
                await Navigation.PopModalAsync();
            }

            MessagingCenter.Unsubscribe<UserSignUpPage, int>(this, MessagingFields.UserLoggedInChanged);
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
                            MessagingCenter.Send(this, MessagingFields.UserLoggedInChanged, 1);
                            if (_FromUserDetail)
                            {
                                await Navigation.PopModalAsync();
                                MessagingCenter.Send(this, MessagingFields.UserLoggedInChangedOrders, true);
                            }
                            else
                            {
                                await Navigation.PopModalAsync();
                                MessagingCenter.Send(this, MessagingFields.UserLoggedInChangedBasket, true);
                                //MessagingCenter.Unsubscribe<UserLoginPage, bool>(this, MessagingFields.UserLoggedInChangedBasket);

                            }

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

        private async Task PushAction(Xamarin.Forms.Page page)
        {
            await Navigation.PushAsync(page, true);
        }
    }


}
