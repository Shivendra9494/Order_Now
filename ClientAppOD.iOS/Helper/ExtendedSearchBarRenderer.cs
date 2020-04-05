using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using UIKit;
using System.Diagnostics;
using CoreGraphics;
using ClientAppOD.Helper;
using ClientAppOD.iOS.Renderers;
using System.Drawing;
using ClientAppOD;
using Facebook.LoginKit;
using ClientAppOD.SubPages;
using AuthenticationServices;

[assembly: ExportRenderer(typeof(SearchBar), typeof(ClientAppOD.iOS.Renderers.ExtendedSearchBarRenderer))]
[assembly: ExportRenderer(typeof(Entry), typeof(ClientAppOD.iOS.Renderers.CustomEntryRenderer))]
[assembly: ExportRenderer(typeof(Picker), typeof(ClientAppOD.iOS.Renderers.CustomPickerRenderer))]
[assembly: ExportRenderer(typeof(CustomPicker), typeof(CustomPickerRendererArrow))]
[assembly: ExportRenderer(typeof(Frame), typeof(CustomFrameRenderer))]

[assembly: ExportRenderer(typeof(CustomFrame), typeof(CustomFrameRenderer1))]
[assembly: ExportRenderer(typeof(UINavigationBar), typeof(CustomNavigationBarRenderer))]
[assembly: ExportRenderer(typeof(TabbedPage), typeof(TabbedPageRenderer))]
[assembly: ExportRenderer(typeof(ViewCell), typeof(CustomViewRenderer))]
[assembly: ExportRenderer(typeof(WebView), typeof(Xamarin.Forms.Platform.iOS.WkWebViewRenderer))]
[assembly: ExportRenderer(typeof(FacebookLoginButton), typeof(FacebookLoginButtonRenderer))]
[assembly: ExportRenderer(typeof(AppleSignInButton), typeof(AppleSignInButtonRenderer))]
namespace ClientAppOD.iOS.Renderers
{
    public class FacebookLoginButtonRenderer : ViewRenderer
    {
        bool disposed;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                var fbLoginBtnView = e.NewElement as FacebookLoginButton;
                var fbLoginbBtnCtrl = new LoginButton()
                {
                    
                };
                fbLoginbBtnCtrl.Completed += AuthCompleted;
                SetNativeControl(fbLoginbBtnCtrl);
            }
        }

        void AuthCompleted(object sender, LoginButtonCompletedEventArgs args)
        {
            var view = (this.Element as FacebookLoginButton);
            if (args.Error != null)
            {
                // Handle if there was an error
                view.OnError?.Execute(args.Error.ToString());

            }
            else if (args.Result.IsCancelled)
            {
                // Handle if the user cancelled the login request
                view.OnCancel?.Execute(null);
            }
            else
            {
                // Handle your successful login
                view.OnSuccess?.Execute(args.Result.Token.TokenString);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                if (this.Control != null)
                {
                    (this.Control as LoginButton).Completed -= AuthCompleted;
                    this.Control.Dispose();
                }
                this.disposed = true;
            }
            base.Dispose(disposing);
        }
    }
    public class ExtendedSearchBarRenderer : SearchBarRenderer
    {
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "Text")
            {
                Control.ShowsCancelButton = false;
            }
        }
        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);
            var searchbar = (UISearchBar)Control;
            if (e.NewElement != null)
            {
                Foundation.NSString _searchField = new Foundation.NSString("searchField");
                var textFieldInsideSearchBar = (UITextField)searchbar.ValueForKey(_searchField);
                textFieldInsideSearchBar.BackgroundColor = UIColor.FromRGB(247, 242, 243);
                searchbar.ShowsCancelButton = false;
            }
        }
    }
    public class NavigationPageRenderer : NavigationRenderer
    {
        public override void WillMoveToParentViewController(UIViewController parent)
        {
            try
            {
                if (parent != null)
                {
                    if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
                    {
                        // Obviously in a real application this would be more sophisticated or concrete here, but for
                        // the purposes of this demo we're just doing a quick n dirty check to illustrate things
                        parent.ModalPresentationStyle =  UIModalPresentationStyle.FormSheet;
                    }
                }

                base.WillMoveToParentViewController(parent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
    public class CustomEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            try
            {
                this.Control.LeftView = new UIView(new CGRect(0, 0, 8, this.Control.Frame.Height));
                this.Control.RightView = new UIView(new CGRect(0, 0, 8, this.Control.Frame.Height));
                this.Control.LeftViewMode = UITextFieldViewMode.Always;
                this.Control.RightViewMode = UITextFieldViewMode.Always;
                this.Control.BorderStyle = UITextBorderStyle.None;
            }
            catch
            {

            }
           
        }
    }
    public class CustomPickerRenderer : PickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);
            try
            {
                this.Control.LeftView = new UIView(new CGRect(0, 0, 8, this.Control.Frame.Height));
                this.Control.RightView = new UIView(new CGRect(0, 0, 8, this.Control.Frame.Height));
                this.Control.LeftViewMode = UITextFieldViewMode.Always;
                this.Control.RightViewMode = UITextFieldViewMode.Always;
                this.Control.BorderStyle = UITextBorderStyle.None;
            }
            catch
            {

            }

        }
    }
    public class CustomPickerRendererArrow : PickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            var element = (CustomPicker)this.Element;

            if (this.Control != null && this.Element != null && !string.IsNullOrEmpty(element.Image))
            {
                var downarrow = UIImage.FromBundle(element.Image);
                Control.RightViewMode = UITextFieldViewMode.Always;
                Control.RightView = new UIImageView(downarrow);
            }
        }
    }
    public class CustomFrameRenderer : FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Frame> e)
        {
            base.OnElementChanged(e);
            var elem = (Frame)this.Element;
            if (elem != null)
            {
                if (elem.HasShadow)
                {
                    // Shadow
                    this.Layer.ShadowColor = UIColor.Black.CGColor;
                    this.Layer.ShadowOpacity = 0.5f;
                    this.Layer.ShadowRadius = 0.5f;
                    this.Layer.ShadowOffset = new SizeF(0, 0);
                    //this.Layer.MasksToBounds = true;
                }

            }
        }
    }
    public class CustomFrameRenderer1 : FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Frame> e)
        {
            base.OnElementChanged(e);
            var elem = (Frame)this.Element;
            if (elem != null)
            {
                if (elem.HasShadow)
                {
                    // Shadow
                    this.Layer.ShadowColor = UIColor.Black.CGColor;
                    this.Layer.ShadowRadius = 3f;
                    this.Layer.ShadowOpacity = 0.9f;
                    //this.Layer.MasksToBounds = true;
                }

            }
        }
    }
    public class CustomViewRenderer : ViewCellRenderer
    {
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);
            cell.SelectedBackgroundView = new UIView
            {
                BackgroundColor = UIColor.White,
            };
            cell.SeparatorInset = UIEdgeInsets.Zero;
            return cell;
        }
        
    }

    public class CustomNavigationBarRenderer : NavigationRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                var att = new UITextAttributes();
                att.Font = UIFont.FromName("Nunito-Light", 14);
                UINavigationBar.Appearance.SetTitleTextAttributes(att);
            }
        }
        
    }
    public class TabbedPageRenderer : TabbedRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            UINavigationBar.Appearance.Translucent = false;
        }
    }
    public class AppleSignInButtonRenderer : ViewRenderer<AppleSignInButton, UIView>
    {
        public static ASAuthorizationAppleIdButtonType ButtonType { get; set; } = ASAuthorizationAppleIdButtonType.Default;

        bool Is13 => UIDevice.CurrentDevice.CheckSystemVersion(13, 0);

        ASAuthorizationAppleIdButton button;
        UIButton oldButton;

        protected override void OnElementChanged(ElementChangedEventArgs<AppleSignInButton> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Cleanup
                if (Is13)
                {
                    if (button != null)
                        button.TouchUpInside -= Button_TouchUpInside;
                }
                else
                {
                    if (oldButton != null)
                        oldButton.TouchUpInside -= Button_TouchUpInside;
                }
            }

            if (e.NewElement != null)
            {
                // Create
                if (Is13)
                {
                    if (button == null)
                    {
                        button = (ASAuthorizationAppleIdButton)CreateNativeControl();
                        button.TouchUpInside += Button_TouchUpInside;
                        SetNativeControl(button);
                    }
                }
                else
                {
                    if (oldButton == null)
                    {
                        oldButton = (UIButton)CreateNativeControl();
                        oldButton.TouchUpInside += Button_TouchUpInside;
                        oldButton.Layer.CornerRadius = 14;
                        oldButton.Layer.BorderWidth = 1;
                        oldButton.ClipsToBounds = true;
                        oldButton.SetTitle(" " + Element.Text, UIControlState.Normal);

                        switch (Element.ButtonStyle)
                        {
                            case AppleSignInButtonStyle.Black:
                                oldButton.BackgroundColor = UIColor.Black;
                                oldButton.SetTitleColor(UIColor.White, UIControlState.Normal);
                                oldButton.Layer.BorderColor = UIColor.Black.CGColor;
                                break;
                            case AppleSignInButtonStyle.White:
                                oldButton.BackgroundColor = UIColor.White;
                                oldButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
                                oldButton.Layer.BorderColor = UIColor.White.CGColor;
                                break;
                            case AppleSignInButtonStyle.WhiteOutline:
                                oldButton.BackgroundColor = UIColor.White;
                                oldButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
                                oldButton.Layer.BorderColor = UIColor.Black.CGColor;
                                break;
                        }

                        SetNativeControl(oldButton);
                    }
                }
            }
        }

        protected override UIView CreateNativeControl()
        {
            if (!Is13)
                return new UIButton(UIButtonType.Plain);
            else
                return new ASAuthorizationAppleIdButton(ButtonType, GetButtonStyle());
        }

        ASAuthorizationAppleIdButtonStyle GetButtonStyle()
        {
            switch (Element.ButtonStyle)
            {
                case AppleSignInButtonStyle.Black:
                    return ASAuthorizationAppleIdButtonStyle.Black;
                case AppleSignInButtonStyle.White:
                    return ASAuthorizationAppleIdButtonStyle.White;
                case AppleSignInButtonStyle.WhiteOutline:
                    return ASAuthorizationAppleIdButtonStyle.WhiteOutline;
            }

            return ASAuthorizationAppleIdButtonStyle.Black;
        }

        void Button_TouchUpInside(object sender, EventArgs e)
            => Element.InvokeSignInEvent(sender, e);
    }
}
