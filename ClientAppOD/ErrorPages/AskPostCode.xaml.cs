using System;
using System.Collections.Generic;
using ClientAppOD.APIPost;
using ClientAppOD.CustomModels;
using ClientAppOD.Helper;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using System.Linq;

namespace ClientAppOD.ErrorPages
{
    public partial class AskPostCode : ContentView
    {
        PostCodeHelper postCodeHelper = new PostCodeHelper();
        public AskPostCode()
        {
            InitializeComponent();
            entryPostcode.TextChanged += (sender, args) => ((Entry)sender).Text = ((Entry)sender).Text.ToUpper();
            frameTop.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => entryPostcode.Focus()
                )
            });
        }

        [Obsolete]
        async void btnContimueclicked(object sender,EventArgs e)
        {
            var btn = sender as Button;
            stack.IsEnabled = false;
            btn.Text = "Please wait..";
           
            if (string.IsNullOrEmpty(entryPostcode.Text))
            {
                lblError1.Text = "Postcode can't be empty";
                stack.IsEnabled = true;
                btn.Text = "Continue";
                return;
            }
            else
            {
                lblError1.Text = " ";
            }
            string postCode = entryPostcode.Text.Replace(" ", "").ToUpper();
            var info = postCodeHelper.GetPostCodeInfo(postCode);
            if(info==null)
            {
                lblError1.Text = "Please enter a valid postcode";
                stack.IsEnabled = true;
                btn.Text = "Continue";
                return;
            }
            else
            {
                lblError1.Text = " ";
            }
            if(info.Contains("200"))
            {
                var deliveryInfo = await postCodeHelper.GetDeliveryInfo(postCode);
                if (deliveryInfo != null)
                {
                    if (deliveryInfo.FirstOrDefault(x=>x.Message != "ok")!=null)
                    {
                        lblError1.Text = deliveryInfo.FirstOrDefault(x => x.Message != "ok").Message;
                        stack.IsEnabled = true;
                        btn.Text = "Continue";
                        return;
                    }
                    else
                    {
                        StaticFields.Deliverytype = "d";
                        StaticFields.CurrentDeliveryInfoList = deliveryInfo.OrderBy(x=>x.MinimumAmount).ToList();
                        StaticFields.CurrentDeliveryInfo = deliveryInfo.OrderBy(x=>x.MinimumAmount).First();
                        StaticFields.CurrentPostCode = postCode;
                        if(StaticFields.CurrentCustomer!=null)
                        {
                            if(!string.IsNullOrEmpty(StaticFields.CurrentCustomer.PostalCode))
                            {
                                if(StaticFields.CurrentCustomer.PostalCode.Replace(" ","").ToUpper()!= StaticFields.CurrentPostCode)
                                {
                                    CustomerPostHelper customerPostHelper = new CustomerPostHelper();
                                    await customerPostHelper.UpdateCustomer(StaticFields.CurrentCustomer.ID, "", "", "", "", StaticFields.CurrentPostCode);
                                    StaticFields.CurrentCustomer.PostalCode = StaticFields.CurrentPostCode;
                                }
                            }
                        }
                    }
                }
                else
                {
                    lblError1.Text = "Please try again";
                    stack.IsEnabled = true;
                    btn.Text = "Continue";
                    return;
                }
            }
            stack.IsEnabled = true;
            btn.Text = "Continue";
            try
            {
                MessagingCenter.Send(this, MessagingFields.UpdateDeliverySwitch, 1);
            }
            catch
            {

            }
            await PopupNavigation.PopAsync();
        }

        [Obsolete]
        async void btnCollectionClicked(object sender, EventArgs e)
        {
            StaticFields.Deliverytype = "c";
            await PopupNavigation.PopAsync();
        }
    }
}
