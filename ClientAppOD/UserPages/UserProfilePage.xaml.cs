using System;
using System.Collections.Generic;
using ClientAppOD.APIPost;
using ClientAppOD.CustomModels;
using Xamarin.Forms;

namespace ClientAppOD.UserPages
{
    public partial class UserProfilePage : ContentPage
    {
        CustomerPostHelper customerPostHelper = new CustomerPostHelper();
        public UserProfilePage()
        {
            InitializeComponent();
            entryName.Text = StaticFields.CurrentCustomer.FirstName;
            entryPhone.Text = StaticFields.CurrentCustomer.Phone;
            entryPostCode.Text = StaticFields.CurrentCustomer.PostalCode;
            entryEmail.Text = StaticFields.CurrentCustomer.Email;
            entryAddress1.Text = StaticFields.CurrentCustomer.Address1;
            entryAddress2.Text = StaticFields.CurrentCustomer.Address2;
            entryCity.Text = StaticFields.CurrentCustomer.City;
            
        }
        private async void btnUpdateClicked(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(entryName.Text))
            {
                lblError.Text = "Name can't be empty";
                return;
            }
            else if (string.IsNullOrEmpty(entryEmail.Text))
            {
                lblError.Text = "Email can't be empty";
                return;
            }
            else if (string.IsNullOrEmpty(entryPhone.Text))
            {
                lblError.Text = "Phone number can't be empty";
                return;
            }
            else if (string.IsNullOrEmpty(entryAddress1.Text))
            {
                lblError.Text = "Address can't be empty";
                return;
            }
            else if (string.IsNullOrEmpty(entryCity.Text))
            {
                lblError.Text = "City can't be empty";
                return;
            }
            else if (string.IsNullOrEmpty(entryPostCode.Text))
            {
                lblError.Text = "Postcode can't be empty";
                return;
            }
            else 
            {
                lblError.Text = "";
                
            }
            var customer = await customerPostHelper.UpdateCustomer(StaticFields.CurrentCustomer.ID, entryPhone.Text, entryAddress1.Text, entryAddress2.Text, entryCity.Text, entryPostCode.Text);
            StaticFields.CurrentCustomer = customer;
            await DisplayAlert("Update successful", "Your profile has beed updated", "Ok");
        }
    }
}
