using System;
using System.Collections.Generic;
using ClientAppOD.MenuPages;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace ClientAppOD.ErrorPages
{
    public partial class LoginErrorPage : ContentPage
    {
        public LoginErrorPage()
        {
            InitializeComponent();
            BackgroundColor = Color.FromRgba(0, 0, 0, 0.5);
            
        }
        void TestClicked(object sender,EventArgs e)
        {
            //Navigation.PushPopupAsync(new LoginPopupPage());
        }
    }
    public class LoginPopupPage : PopupPage
    {
        public LoginPopupPage()
        {
            Button btnClose = new Button() { Text = "Close this" };
            btnClose.Clicked += BtnCloseOnClicked;
            IsAnimationEnabled = true;
            Content = new AskPostCode();
            // set the background to transparent color 
            // (actually darkened-transparency: notice the alpha value at the end)
            this.BackgroundColor = new Color(0, 0, 0, 0.4);
        }
        private void BtnCloseOnClicked(object sender, EventArgs eventArgs)
        {
            // Close the modal page
            PopupNavigation.PopAsync();
        }
        protected override bool OnBackgroundClicked()
        {
            return false;
        }
    }
}
