using System;
using System.Collections.Generic;
using System.Linq;
using ClientAppOD.iOS.Helper;
using Com.OneSignal;
using Facebook.CoreKit;
using Foundation;
using UIKit;

namespace ClientAppOD.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            Rg.Plugins.Popup.Popup.Init();
            var altIconApp = new App(new FacebookLoginService());

            altIconApp.AppIconChanged += (sender, iconName) =>
            {
                // Null represents the default app icon
                // otherwise, it's the name defined in the info.plist
                var name = UIApplication.SharedApplication.AlternateIconName;
                if (name != iconName || name == null)
                {
                    if (iconName != null)
                        UIApplication.SharedApplication.SetAlternateIconName(iconName.ToLower(), (err) =>
                        {
                            HandleAction(err);
                        });

                    else
                        UIApplication.SharedApplication.SetAlternateIconName(null, (err) => {
                            HandleAction(err);
                        });
                }
                NSHttpCookieStorage CookieStorage = NSHttpCookieStorage.SharedStorage;
                foreach (var cookie in CookieStorage.Cookies)
                    CookieStorage.DeleteCookie(cookie);
            
            };
            LoadApplication(altIconApp);
            //app.StatusBarStyle = UIStatusBarStyle.LightContent;
            //OneSignal.Current.StartInit("27148fa9-b69e-4ab7-bffe-40be04921ddc").EndInit();
            return base.FinishedLaunching(app, options);
        }
        void HandleAction(NSError err)
        {
            Console.WriteLine("Set Alternate Icon: {0}", err);
        }
        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            // We need to handle URLs by passing them to their own OpenUrl in order to make the SSO authentication works.
            return ApplicationDelegate.SharedInstance.OpenUrl(application, url, sourceApplication, annotation);
        }
    }
}
