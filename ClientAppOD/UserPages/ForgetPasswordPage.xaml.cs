using System;
using System.Collections.Generic;
using ClientAppOD.APIPost;
using ClientAppOD.Helper;
using Xamarin.Forms;

namespace ClientAppOD.UserPages
{
    public partial class ForgetPasswordPage : ContentPage
    {
        EmailHelper emailHelper = new EmailHelper();
        CustomerPostHelper customerPostHelper = new CustomerPostHelper();
        public ForgetPasswordPage()
        {
            InitializeComponent();
        }
        private async void SendClicked(object sender,EventArgs e)
        {
            
            if(string.IsNullOrEmpty(entryEmail.Text))
            {
                lblError.Text = "Email is empty";
                return;
            }
            else
            {
                lblError.Text = "";
                
            }
            if(!EmailHelper.IsValidEmail(entryEmail.Text))
            {
                lblError.Text = "Email not valid";
                return;
            }
            else
            {
                lblError.Text = "";
                
            }
            btnSend.IsEnabled = false;
            var customer = await customerPostHelper.GetCustomerExist(entryEmail.Text);
            if (customer>0)
            {
                Guid g = Guid.NewGuid();
                string GuidString = Convert.ToBase64String(g.ToByteArray());
                GuidString = GuidString.Replace("=", "");
                GuidString = GuidString.Replace("+", "");
                int res = await customerPostHelper.UpdateCustomerResetCode(customer, GuidString);
                if (res > 0)
                {
                    EmailHelper.SendMailResetPassword(entryEmail.Text, "Recover your Password", "Order Directly", customer.ToString(), GuidString);
                    stackMain.IsVisible = false;
                    lblError.Text = "An email is on the way with a link for you to change your password";
                    lblError.TextColor = Color.Gray;
                }
                else
                {
                    lblError.Text = "There is some error while processing your request. Please try again";
                }
            }
            else
            {
                lblError.Text = "Could not found any user with email " + entryEmail.Text;
            }
            btnSend.IsEnabled = true;
        }
    }
}
