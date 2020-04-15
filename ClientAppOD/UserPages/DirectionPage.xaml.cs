using System;
using System.Collections.Generic;
using ClientAppOD.CustomModels;
using Xamarin.Forms;

namespace ClientAppOD.UserPages
{
    public partial class DirectionPage : ContentPage
    {
        public DirectionPage()
        {
            InitializeComponent();
            webView.Source = "https://orderdirectly.biz/direction.asp?id_c=" + StaticFields.CurrentStoreInfo.ClientId;
        }

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
