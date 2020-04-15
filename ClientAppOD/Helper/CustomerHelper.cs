using System;
using System.Windows.Input;
using ClientAppOD.UserPages;
using Xamarin.Forms;

namespace ClientAppOD
{
    public class CustomerHelper
    {
        public CustomerHelper()
        {
        }
    }
    public class CustomFrame:Frame
    {
        
    }
    public class FacebookLoginButton : View
    {
        public string[] Permissions
        {
            get { return (string[])GetValue(PermissionsProperty); }
            set { SetValue(PermissionsProperty, value); }
        }

        public static readonly BindableProperty PermissionsProperty =
            BindableProperty.Create(
                nameof(Permissions),
                typeof(string[]),
                typeof(FacebookLoginButton),
                // This permission is set by default, even if you don’t add it, but FB recommends to add it anyway
                defaultValue: new string[] { "public_profile", "email" });

        public Command<string> OnSuccess
        {
            get { return (Command<string>)GetValue(OnSuccessProperty); }
            set { SetValue(OnSuccessProperty, value); }
        }

        public static readonly BindableProperty OnSuccessProperty =
            BindableProperty.Create(nameof(OnSuccess), typeof(Command<string>), typeof(FacebookLoginButton));

        public Command<string> OnError
        {
            get { return (Command<string>)GetValue(OnErrorProperty); }
            set { SetValue(OnErrorProperty, value); }
        }

        public static readonly BindableProperty OnErrorProperty =
            BindableProperty.Create(nameof(OnError), typeof(Command<string>), typeof(FacebookLoginButton));

        public Command OnCancel
        {
            get { return (Command)GetValue(OnCancelProperty); }
            set { SetValue(OnCancelProperty, value); }
        }

        public static readonly BindableProperty OnCancelProperty =
            BindableProperty.Create(nameof(OnCancel), typeof(Command), typeof(FacebookLoginButton));

    }
    public interface IFacebookLoginService
    {
        string AccessToken { get; }
        Action<string, string> AccessTokenChanged { get; set; }
        void Logout();
    }
    public class LoginViewModel
    {
        readonly IFacebookLoginService facebookLoginService;
        //public ICommand OnFacebookLoginSuccessCmd { get; }
        //public ICommand OnFacebookLoginErrorCmd { get; }
        //public ICommand OnFacebookLoginCancelCmd { get; }
        //public Command FacebookLogoutCmd { get; }
        UserLoginPage _page;
        UserSignUpPage _pageSignup;
        int ii = 1;
        public LoginViewModel(UserLoginPage page)
        {
            _page = page;
            ii = 1;
          //  facebookLoginService = (Application.Current as App).FacebookLoginService;
         //   facebookLoginService.AccessTokenChanged = (string oldToken, string newToken) => FacebookLogoutCmd.ChangeCanExecute();
         //   FacebookLogoutCmd = new Command(() =>
         //       facebookLoginService.Logout(),
         //       () => !string.IsNullOrEmpty(facebookLoginService.AccessToken));
         //   OnFacebookLoginSuccessCmd = new Command<string>(
          //       (authToken) => DisplayAlert("Success", $"Authentication succeed: {authToken}"));

          //  OnFacebookLoginErrorCmd = new Command<string>(
          //      (err) => DisplayAlert("Error", $"Authentication failed: { err }"));

         //   OnFacebookLoginCancelCmd = new Command(
          //      () => DisplayAlert("Cancel", "Authentication cancelled by the user."));
        }
        public LoginViewModel(UserSignUpPage page)
        {
            _pageSignup = page;
            ii = 1;
            //facebookLoginService = (Application.Current as App).FacebookLoginService;
            //facebookLoginService.AccessTokenChanged = (string oldToken, string newToken) => FacebookLogoutCmd.ChangeCanExecute();
            //FacebookLogoutCmd = new Command(() =>
            //    facebookLoginService.Logout(),
            //    () => !string.IsNullOrEmpty(facebookLoginService.AccessToken));
            //OnFacebookLoginSuccessCmd = new Command<string>(
            //     (authToken) => DisplayAlertSignUp("Success", $"Authentication succeed: {authToken}"));

            //OnFacebookLoginErrorCmd = new Command<string>(
            //    (err) => DisplayAlertSignUp("Error", $"Authentication failed: { err }"));

            //OnFacebookLoginCancelCmd = new Command(
            //    () => DisplayAlertSignUp("Cancel", "Authentication cancelled by the user."));
        }
        //void DisplayAlert(string title, string msg)
        //{
        //    if(title=="Success" )
        //    {

        //        _page.LoginFacebookAsync();
               
        //    }
        //    else
        //    {
        //        (Application.Current as App).MainPage.DisplayAlert(title, msg, "OK");
        //    }
            
        //}
        //void DisplayAlertSignUp(string title, string msg)
        //{
        //    if (title == "Success")
        //    {

        //        _pageSignup.LoginFacebookAsync();

        //    }
        //    else
        //    {
        //        (Application.Current as App).MainPage.DisplayAlert(title, msg, "OK");
        //    }

        //}
    }
}
