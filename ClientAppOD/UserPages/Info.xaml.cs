using System;
using System.Collections.Generic;
using ClientAppOD.CustomModels;
using Xamarin.Forms;

namespace ClientAppOD.UserPages
{
    public partial class Info : ContentPage
    {
        public Info()
        {
            InitializeComponent();
            webView.Source = "https://orderdirectly.biz/info.asp?id_c="+StaticFields.CurrentStoreInfo.ClientId;
        }
    }
}
