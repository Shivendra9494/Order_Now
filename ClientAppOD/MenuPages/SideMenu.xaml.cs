using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace ClientAppOD.MenuPages
{
    public partial class SideMenu : ContentView
    {
        public ListView ListView;
        public SideMenu()
        {
            InitializeComponent();
            BindingContext = new SideMenuViewModel();
            ListView = MenuItemsListView;
        }

        class SideMenuViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MenuCategoryPagesMenuItem> MenuItems { get; set; }

            public SideMenuViewModel()
            {
                MenuItems = new ObservableCollection<MenuCategoryPagesMenuItem>(new[]
                {
                    new MenuCategoryPagesMenuItem { Id = 0, Title = "Find restaurants" },
                    new MenuCategoryPagesMenuItem { Id = 1, Title = "Your orders" },
                    new MenuCategoryPagesMenuItem { Id = 2, Title = "Help" },
                    new MenuCategoryPagesMenuItem { Id = 3, Title = "How are we doing?" },

                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
        async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            await PopupNavigation.PopAsync();
        }
    }

    
}




