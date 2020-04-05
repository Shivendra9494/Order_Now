using System;
using Foundation;
using ClientAppOD.Helper;
using UIKit;
using Xamarin.Forms;
using ClientAppOD.iOS.Helper;
using Facebook.LoginKit;

[assembly: Dependency(typeof(ChangeIconService))]
namespace ClientAppOD.iOS.Helper
{
    public class ChangeIconService : IChangeIconService
    {
        NSError error = new NSError();

        public ChangeIconService()
        {
        }

        public void ChangeIcon(string iconName)
        {

            var name = UIApplication.SharedApplication.AlternateIconName;
            if(name != iconName || name==null)
            {
                if (iconName != null)
                    UIApplication.SharedApplication.SetAlternateIconName(iconName, (err) =>
                    {
                        HandleAction(err);
                    });

                else
                    UIApplication.SharedApplication.SetAlternateIconName(null, (err) => {
                        HandleAction(err);
                    });
                UIApplication.SharedApplication.ApplicationIconBadgeNumber = 1;
            }
            
        }


        void HandleAction(NSError obj)
        {
            //Console.Write(obj.ToString());
        }

    }
    public class FacebookLoginService : IFacebookLoginService
    {
        public string AccessToken => Facebook.CoreKit.AccessToken.CurrentAccessToken?.TokenString;

        public Action<string, string> AccessTokenChanged { get; set; }

        public FacebookLoginService()
        {
            // TODO: Remove observer
            NSNotificationCenter.DefaultCenter.AddObserver(
                new NSString(Facebook.CoreKit.AccessToken.DidChangeNotification),
                (n) =>
                {
                    AccessTokenChanged?.Invoke(
                        n.UserInfo[Facebook.CoreKit.AccessToken.OldTokenKey]?.ToString(),
                        n.UserInfo[Facebook.CoreKit.AccessToken.NewTokenKey]?.ToString());
                });
        }

        public void Logout()
        {
            using (var loginManager = new LoginManager())
            {
                loginManager.LogOut();
            }
        }
    }
}
