using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientAppOD.APIPost;
using ClientAppOD.CustomModels;
using ClientAppOD.UserPages;
using Xamarin.Forms;

using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace ClientAppOD.OrderPages
{
    public partial class CheckOutPage : ContentPage
    {
        VoucherPostHelper voucherPostHelper = new VoucherPostHelper();
        public CheckOutPage()
        {
            InitializeComponent();
            CheckLogin();
            SetDeliveryTimeList();
            if (StaticFields.Deliverytype=="c")
            {
                lblDeliveryName.Text = "Collection Time";
            }
            stackVoucherClick.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => DownArraowClicked()
                )
            });
            stackDelivery.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => focus()
                )
            });
        }
        private async Task CheckLogin()
        {
            if (StaticFields.CurrentCustomer == null)
            {
                var page = new Xamarin.Forms.NavigationPage(new UserLoginPage(true))
                {
                    BarTextColor = Color.FromHex("236adb")
                };

                page.On<iOS>().SetModalPresentationStyle(Xamarin.Forms.PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.FormSheet);
                await Navigation.PushModalAsync(page, true);
            }
            
        }
        protected void btnCancelClicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
        private void focus(object sender,EventArgs e)
        {
            pickerDelivery.Focus();
        }
        private void focus()
        {
            pickerDelivery.Focus();
        }
        protected async void VoucherClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(entryVoucher.Text))
            {
                var voucher = await voucherPostHelper.GetVoucher(entryVoucher.Text);
                if(voucher!=null)
                {
                    lblError.TextColor = Color.DarkGreen;
                    StaticFields.CurrentOrder.DiscountID = voucher.ID;
                    
                    var discount = Math.Round((decimal)(StaticFields.CurrentOrder.OrderTotal * voucher.VoucherCodeDiscount) / 100, 2);
                    if(StaticFields.CurrentOrder.Discount!=null)
                    {
                        if (StaticFields.CurrentOrder.Discount > discount)
                        {
                            lblError.TextColor = Color.FromHex("EB6361");
                            lblError.Text = "Discount of £" + discount+" already applied";
                            return;
                        }
                        else
                        {
                            
                            StaticFields.CurrentOrder.SubTotal = Math.Round((decimal)(StaticFields.CurrentOrder.SubTotal + StaticFields.CurrentOrder.Discount), 2);
                            StaticFields.CurrentOrder.Discount = null;
                            StaticFields.CurrentOrder.DiscountID = null;
                            StaticFields.CurrentOrder.DiscountPercentage = "";
                        }
                    }
                    if (!string.IsNullOrEmpty(StaticFields.CurrentOrder.VoucherCode))
                    {
                        StaticFields.CurrentOrder.SubTotal = Math.Round((decimal)(StaticFields.CurrentOrder.SubTotal + StaticFields.CurrentOrder.VoucherCodeDiscount), 2);
                    }
                    StaticFields.CurrentOrder.VoucherCodeDiscount = discount;
                    StaticFields.CurrentOrder.VoucherCode = voucher.VoucherCodeText+"|"+voucher.VoucherCodeDiscount;
                    StaticFields.CurrentOrder.SubTotal = Math.Round((decimal)(StaticFields.CurrentOrder.SubTotal - discount), 2);
                    lblError.Text = "Voucher applied for £" + discount;
                }
                else
                {
                    lblError.TextColor = Color.FromHex("EB6361");
                    lblError.Text = "Couldn't find this voucher code";
                }
            }
            else
            {
                lblError.Text = "Voucher code can't be empty";
            }
        }
        protected void PayButtonClicked(object sender, EventArgs e)
        {
            StaticFields.CurrentOrder.Notes = editorNotes.Text;
            StaticFields.CurrentOrder.DeliveryTime = pickerDelivery.SelectedItem.ToString();
            Navigation.PushAsync(new ConfirmDetailsPage());
        }
        protected override void OnAppearing()
        {
            
            base.OnAppearing();
           


        }
        private void DownArraowClicked(object sender, EventArgs e)
        {
            DownArraowClicked();
        }
        private void DownArraowClicked()
        {
            var btn = stackVoucherClick.Children[1] as ImageButton;
            if (btn.Rotation == 270)
            {
                stackVoucher.IsVisible = true;
                btn.Rotation = 90;
            }
            else
            {
                stackVoucher.IsVisible = false;
                btn.Rotation = 270;
            }
        }
       
        private async Task SetDeliveryTimeList()
        {
            var CurrentTime = DateTimeOffset.UtcNow;
            DateTimeOffset FirstTime = CurrentTime.AddMinutes(15);
            if (FirstTime.Minute < 15 && FirstTime.Minute >= 0)
            {
                FirstTime= FirstTime.AddMinutes(15 - FirstTime.Minute);
            }
            else if (FirstTime.Minute < 30 && FirstTime.Minute >= 15)
            {
                FirstTime = FirstTime.AddMinutes(30 - FirstTime.Minute);
            }
            else if (FirstTime.Minute < 45 && FirstTime.Minute >= 30)
            {
                FirstTime = FirstTime.AddMinutes(45 - FirstTime.Minute);
            }
            else if (FirstTime.Minute <= 59 && FirstTime.Minute >= 45)
            {
                    FirstTime = FirstTime.AddMinutes(60 - FirstTime.Minute); 

            }
            //FirstTime = FirstTime.ToOffset(TimeSpan.FromHours(1));
            DateTimeOffset EndTime;
            if (StaticFields.CurrentStoreInfo.DeliveryClosedTime != null)
            {
                if (StaticFields.Deliverytype == "d")
                {
                    EndTime = (DateTimeOffset)StaticFields.CurrentStoreInfo.DeliveryClosedTime;

                    pickerDelivery.Items.Add("ASAP");
                }
                else
                {
                    EndTime = (DateTimeOffset)StaticFields.CurrentStoreInfo.CollectionClosedTime;
                }
            }
            else
            {
                EndTime = FirstTime.AddHours(2);
            }
            for (DateTimeOffset dt = FirstTime; dt <= EndTime; dt=dt.AddMinutes(15))
            {
                string item = "";
                if(dt.Date==DateTime.Now.Date)
                {
                    item = "Today at " + dt.ToString("hh:mm tt");
                }
                else
                {
                    item = "Tomorrow at " + dt.ToString("hh:mm tt");
                }
                pickerDelivery.Items.Add(item);
            }
            pickerDelivery.SelectedIndex = 0;
        }
    }
}
