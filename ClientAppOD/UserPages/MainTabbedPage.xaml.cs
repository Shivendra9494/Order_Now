using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientAppOD.Helper;
using ClientAppOD.MenuPages;
using ClientAppOD.OrderPages;
using ClientAppOD.UserPages;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ClientAppOD
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainTabbedPage : TabbedPage
    {
        public MainTabbedPage()
        {
            InitializeComponent();
            this.BarBackgroundColor = Color.FromHex("FFFFFF");
            SelectedTabColor = Color.FromHex("236adb");

            
        }

        protected override void OnAppearing()
        {
            MessagingCenter.Subscribe<OrderHistoryPage, int>(this, "SetActiveTab", (sender, index) => {
                CurrentPage = Children[index];
            });
            MessagingCenter.Subscribe<OrderDetailPage, int>(this, "SetActiveTab", (sender, index) => {
                CurrentPage = Children[index];
            });
            
        }
        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
            if(CurrentPage.GetType()==typeof(CheckOutPage))
            {
                this.HeightRequest = 0;
            }
        }
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<OrderHistoryPage>(this, "SetActiveTab");
            MessagingCenter.Unsubscribe<OrderDetailPage>(this, "SetActiveTab");
        }
    }
}
