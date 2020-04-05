using System;
using System.Collections.Generic;
using ClientAppOD.Helper;
using OD.Data;
using Xamarin.Forms;

namespace ClientAppOD
{
    public partial class OptionListPage : ContentPage
    {
        
        public OptionListPage(MenuItemPropertyModel OptionProModel,MenuItemModel OptionModel, string MenuItemName)
        {
            InitializeComponent();
            if (OptionProModel != null)
            {
                if (OptionProModel.OptionItem != null)
                {
                    ListOption.ItemsSource = OptionProModel.OptionItem.MenuDishProperties;
                }
                lblMenuName.Text = MenuItemName;
                lblOptionName.Text = OptionProModel.DisplayName;
            }
            
            if (OptionModel != null)
            {
                if (OptionModel.OptionItem != null)
                {
                    ListOption.ItemsSource = OptionModel.OptionItem.MenuDishProperties;
                }
                lblMenuName.Text = MenuItemName;
                lblOptionName.Text = OptionModel.DisplayName;
            }
            
            
        }
        private void CancelClicked(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, MessagingFields.CancelClickedOnOption, e);
        }
        void OptionListItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
                var selectedOption = e.SelectedItem as MenuDishProperty;
                MessagingCenter.Send(this, MessagingFields.OptionSelected, selectedOption);
                Navigation.PopAsync();
             
        }

    }
}
