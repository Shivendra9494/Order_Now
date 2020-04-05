using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientAppOD.CustomModels;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ClientAppOD.SubPages
{
    public partial class AllergyWarning : ContentView
    {
        int _tapCount = 0;
        public AllergyWarning()
        {
            InitializeComponent();
            btnCall.Text = StaticFields.CurrentStoreInfo.Phone;
            btnCall.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => callStore()
                )
            });
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            await PopupNavigation.PopAsync();
        }
        private void callStore()
        {
            Device.OpenUri(new Uri("tel:" + StaticFields.CurrentStoreInfo.Phone));
        }
    }
}
