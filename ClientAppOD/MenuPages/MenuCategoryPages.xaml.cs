using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ClientAppOD.MenuPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuCategoryPages : MasterDetailPage
    {
        public readonly static BindableProperty WidthRatioProperty =
           BindableProperty.Create("WidthRatio",
           typeof(float),
           typeof(MenuCategoryPages),
           (float)0.68);

        //public float WidthRatio
        //{
        //    get { return (float)GetValue(WidthRatioProperty); }
        //    set { SetValue(WidthRatioProperty, value); }
        //}


        public float WidthRatio
        {
            get
            {
                return (float)GetValue(WidthRatioProperty);
            }
            set
            {
                SetValue(WidthRatioProperty, value);
            }
        }
        public MenuCategoryPages()
        {
            InitializeComponent();
             
          //  MasterPage.ListView.ItemSelected += ListView_ItemSelected;
            MasterPage.IconImageSource = "hamburger.png";
            WidthRatio = (float)0.68;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MenuCategoryPagesMenuItem;
            if (item == null)
                return;

            var page = (Page)Activator.CreateInstance(item.TargetType);
          //  page.Title = item.Title;

            Detail = new NavigationPage(page);
            IsPresented = false;

           // MasterPage.ListView.SelectedItem = null;
        }
    }
}
