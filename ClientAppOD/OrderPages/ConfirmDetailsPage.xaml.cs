using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientAppOD.APIPost;
using ClientAppOD.CustomModels;
using ClientAppOD.ErrorPages;
using ClientAppOD.Helper;
using OD.Data;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace ClientAppOD.OrderPages
{
    public partial class ConfirmDetailsPage : ContentPage
    {
        PostCodeHelper postCodeHelper = new PostCodeHelper();
        CustomerPostHelper customerPostHelper = new CustomerPostHelper();
        public ConfirmDetailsPage()
        {
            InitializeComponent();
            if (StaticFields.CurrentCustomer != null)
            {
                entryName.Text = StaticFields.CurrentCustomer.FirstName + " " + StaticFields.CurrentCustomer.LastName;
                entryPhone.Text = StaticFields.CurrentCustomer.Phone;
                if (StaticFields.Deliverytype == "d")
                {
                    entryAddress1.Text = StaticFields.CurrentCustomer.Address1;
                    entryAddress2.Text = StaticFields.CurrentCustomer.Address2;
                    entryCity.Text = StaticFields.CurrentCustomer.City;
                    entryPostCode.Text = StaticFields.CurrentPostCode;
                }
                else
                {

                    stackDelivery.IsVisible = false;
                }
            }
            var order = StaticFields.CurrentOrder;
            if (order.ServiceCharge != null)
            {
                order.SubTotal -= (decimal)StaticFields.CurrentStoreInfo.ServiceCharge;
                order.ServiceCharge = null;

            }
            scroll.IsVisible = true;
            loader.IsVisible = false;
        }
        protected override void OnAppearing()
        {

            base.OnAppearing();
        }
        protected async void btnPaymentClicked(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(entryName.Text))
            {
                lblError1.Text = "Please enter your name";
                return;
            }
            else
            {
                lblError1.Text = " ";
            }
            if (string.IsNullOrWhiteSpace(entryPhone.Text))
            {
                lblError1.Text = "Please enter mobile number";
                return;
            }
            else
            {
                lblError1.Text = " ";
            }
            if (entryPhone.Text.Replace(" ", "").Replace("+", "").Length < 10 || entryPhone.Text.Replace(" ", "").Replace("+", "").Length > 11)
            {
                lblError1.Text = "Please enter correct mobile number";
                return;
            }
            else
            {
                lblError1.Text = " ";
            }
            StaticFields.CurrentOrder.Phone = entryPhone.Text.Replace(" ", "").Replace("+", "");
            if (StaticFields.Deliverytype == "d")
            {
                if (string.IsNullOrWhiteSpace(entryAddress1.Text))
                {
                    lblError1.Text = "Please enter your address";
                    return;
                }
                else
                {
                    lblError1.Text = " ";
                }
                if (string.IsNullOrWhiteSpace(entryCity.Text))
                {
                    lblError1.Text = "Please enter your city";
                    return;
                }
                else
                {
                    lblError1.Text = " ";
                }
                if (string.IsNullOrWhiteSpace(entryPostCode.Text))
                {
                    lblError1.Text = "Please enter your postcode";
                    return;
                }
                else
                {
                    lblError1.Text = " ";
                }
                if (entryPostCode.Text.Replace(" ", "").ToUpper() != StaticFields.CurrentPostCode.Replace(" ", "").ToUpper() && StaticFields.Deliverytype == "d")
                {
                    var info = postCodeHelper.GetPostCodeInfo(entryPostCode.Text);
                    if (info == null)
                    {
                        lblError1.Text = "Please enter a valid postcode";
                        return;
                    }
                    else
                    {
                        lblError1.Text = " ";
                    }
                    if (info.Contains("200"))
                    {
                        var deliveryInfo = await postCodeHelper.GetDeliveryInfo(entryPostCode.Text);
                        if (deliveryInfo != null)
                        {
                            if (deliveryInfo.FirstOrDefault(x => x.Message != "ok") != null)
                            {
                                lblError1.Text = deliveryInfo.FirstOrDefault(x => x.Message != "ok").Message;
                                return;
                            }
                            else
                            {
                                var response = await DisplayAlert("Postcode entered is not matching with your profile postcode", "Delivery charges may vary as per restaurant charges", "OK", "Cancel");
                                if (response)
                                {
                                    StaticFields.Deliverytype = "d";
                                    StaticFields.CurrentDeliveryInfoList = deliveryInfo.OrderBy(x => x.MinimumAmount).ToList();
                                    StaticFields.CurrentDeliveryInfo = deliveryInfo.OrderBy(x => x.MinimumAmount).First();
                                    StaticFields.CurrentPostCode = entryPostCode.Text.Replace(" ", "").ToUpper();
                                    StaticFields.CurrentOrder.SubTotal = StaticFields.CurrentOrder.SubTotal - Math.Round(((decimal)StaticFields.CurrentDeliveryInfo.DeliveryFee), 2);
                                    decimal DeliveryFee = 0;
                                    decimal? FreeDeliveryAmount = null;
                                    if (StaticFields.CurrentDeliveryInfoList != null)
                                    {
                                        if (StaticFields.CurrentDeliveryInfoList.Count > 1)
                                        {
                                            var DeliveryInfo = StaticFields.CurrentDeliveryInfoList.Where(x => x.MinimumAmount <= StaticFields.CurrentOrder.OrderTotal);
                                            DeliveryInfo infos = new DeliveryInfo();
                                            if (DeliveryInfo.Count() > 0)
                                            {
                                                infos = DeliveryInfo.OrderByDescending(x => x.MinimumAmount).First();
                                            }
                                            else
                                            {
                                                infos = StaticFields.CurrentDeliveryInfoList.OrderBy(x => x.MinimumAmount).First();
                                            }
                                            DeliveryFee = (Decimal)infos.DeliveryFee;
                                            FreeDeliveryAmount = infos.FreeDeliveryAmount;
                                        }
                                        if (FreeDeliveryAmount == null || FreeDeliveryAmount >= StaticFields.CurrentOrder.SubTotal)
                                        {

                                            StaticFields.CurrentOrder.SubTotal = StaticFields.CurrentOrder.OrderTotal + Math.Round(DeliveryFee, 2);
                                            StaticFields.CurrentOrder.ShippingFee = DeliveryFee;
                                        }
                                        else
                                        {
                                            StaticFields.CurrentOrder.ShippingFee = 0;
                                        }
                                    }


                                    StaticFields.CurrentPostCode = deliveryInfo.OrderBy(x => x.MinimumAmount).First().CustomerPostCode;
                                    if (string.IsNullOrEmpty(StaticFields.CurrentCustomer.Phone) ||
                                        string.IsNullOrEmpty(StaticFields.CurrentCustomer.Address1) ||
                                        string.IsNullOrEmpty(StaticFields.CurrentCustomer.City) ||
                                        string.IsNullOrEmpty(StaticFields.CurrentCustomer.PostalCode))
                                    {
                                        var customer = await customerPostHelper.UpdateCustomer(StaticFields.CurrentCustomer.ID, entryPhone.Text, entryAddress1.Text, entryAddress2.Text, entryCity.Text, entryPostCode.Text);
                                        StaticFields.CurrentCustomer = customer;

                                    }
                                    GoToPayment();
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                        else
                        {
                            lblError1.Text = "Please try again";
                            return;
                        }
                    }

                }
                else
                {
                    loader.IsVisible = true;
                    scroll.IsVisible = false;
                    if (string.IsNullOrEmpty(StaticFields.CurrentCustomer.Phone) ||
                        string.IsNullOrEmpty(StaticFields.CurrentCustomer.Address1) ||
                        string.IsNullOrEmpty(StaticFields.CurrentCustomer.City) ||
                        string.IsNullOrEmpty(StaticFields.CurrentCustomer.PostalCode))
                    {
                        var customer = await customerPostHelper.UpdateCustomer(StaticFields.CurrentCustomer.ID, entryPhone.Text, entryAddress1.Text, entryAddress2.Text, entryCity.Text, entryPostCode.Text);
                        StaticFields.CurrentCustomer = customer;

                    }
                    GoToPayment();
                    loader.IsVisible = false;
                    scroll.IsVisible = true;
                }
            }
            else
            {
                loader.IsVisible = true;
                scroll.IsVisible = false;
                if (string.IsNullOrEmpty(StaticFields.CurrentCustomer.Phone))
                {
                    var customer = await customerPostHelper.UpdateCustomer(StaticFields.CurrentCustomer.ID, entryPhone.Text, "", "", "", "");
                    StaticFields.CurrentCustomer = customer;

                }
                GoToPayment();
                loader.IsVisible = false;
                scroll.IsVisible = true;
            }
        }

        private async void GoToPayment()
        {
            if (StaticFields.Deliverytype == "d")
            {
                StaticFields.CurrentOrder.Address = entryAddress1.Text + " " + entryAddress2.Text + ", " + entryCity.Text;
                StaticFields.CurrentOrder.PostalCode = StaticFields.CurrentCustomer.PostalCode;
            }
            else
            {
                StaticFields.CurrentOrder.Address = " ";
                StaticFields.CurrentOrder.PostalCode = " ";
            }
            StaticFields.CurrentOrder.Phone = entryPhone.Text;
            StaticFields.CurrentOrder.FirstName = entryName.Text;
            StaticFields.CurrentOrder.DeliveryType = StaticFields.Deliverytype;
            StaticFields.CurrentOrder.OrderDate = DateTime.Now;
            loader.IsVisible = false;
            scroll.IsVisible = true;
            await Navigation.PushAsync(new PaymentPage());
            //var order = StaticFields.CurrentOrder;
            //AddOrder(order);

        }
        private async Task AddOrder(Order order)
        {
            order.CustomerID = StaticFields.CurrentCustomer.ID;
            order.BusinessDetailID = StaticFields.CurrentStoreInfo.ID;
            order.Email = StaticFields.CurrentCustomer.Email;
            order.FirstName = StaticFields.CurrentCustomer.FirstName;
            order.LastName = StaticFields.CurrentCustomer.LastName;
            order.SessionId = "";
            order.Acknowledged = false;
            order.Printed = false;
            order.DiscountID = 0;
            order.PaymentType = PaymentType.COD;
            order.Phone = StaticFields.CurrentCustomer.Phone;
            order.PostalCode = StaticFields.CurrentPostCode;
            order.Status = OrderStatus.Checkout;
            order.ClientIP = "";
            order.ValidForLoyality = false;
            order.DeliveryType = StaticFields.CurrentOrder.DeliveryType;
            if (order.ServiceCharge == null)
            {
                order.ServiceCharge = (decimal)StaticFields.CurrentStoreInfo.ServiceCharge;
                order.SubTotal += (decimal)StaticFields.CurrentStoreInfo.ServiceCharge;
            }
            int Id = 0;
            string OrderId = await OrderPostHelper.PostOrder(order);
            try
            {
                Id = Convert.ToInt32(OrderId);
            }
            catch (Exception ex)
            {
                Id = 0;
            }
            if (Id > 0)
            {
                StaticFields.CurrentOrder.ID = Id;
                //await Navigation.PushAsync(new PaymentWebView(StaticFields.ServerURL+"/AppPayment/index/"+StaticFields.CurrentOrder.ID));
                loader.IsVisible = false;
                scroll.IsVisible = true;
                await Navigation.PushAsync(new PaymentPage());
            }
        }
        protected void backClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        async void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
           await Navigation.PopModalAsync();
        }
    }
}
