using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ClientAppOD.CustomModels;
using ClientAppOD.Helper;
using ClientAppOD.MenuPages;
using OD.Data;
using Xamarin.Forms;

namespace ClientAppOD.OrderPages
{
    public partial class ThankyouPage : ContentPage
    {
        private Order _order { get; set; }
        public ThankyouPage(Order order)
        {
            InitializeComponent();
            _order = order;
            lblOrderNumber.Text = "Order No. " + order.ID;
            lblOrderDate.Text = ((DateTime)order.OrderDate).ToString("dd MMMM yyyy");

            lblPayment.Text = order.PaymentType == PaymentType.Card ? "Order Paid" : "Cash Order";
            if (order.DeliveryType == "d")
            {
                lblDeliveringTo.Text = "Delivery Order :";
                lblDeliveryAddress.Text = order.Address;
            }
            else
            {
                lblDeliveringTo.Text = "Collection Order";
            }
            if (!string.IsNullOrEmpty(order.Notes))
            {
                lblNotes.Text = order.Notes;
            }
            else
            {
                stackNotes.IsVisible = false;
            }
            decimal total = 0;
            decimal SubTotal = 0;
            foreach (var orderItem in _order.OrderItems)
            {

                total += (decimal)orderItem.Total;
                SubTotal += (decimal)orderItem.Total;
                stackOrderItem.Children.Add(new BoxView()
                {
                    Margin = new Thickness(0)
                });
                StackLayout firstStack = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(0, 10, 0, 0)
                };
                firstStack.Children.Add(new Label()
                {
                    Text = orderItem.Qta + " x " + orderItem.Name,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromHex("236adb"),
                    HorizontalOptions = LayoutOptions.StartAndExpand
                });

                firstStack.Children.Add(new Label()
                {
                    Text = "£" + orderItem.Total.ToString(),
                    HorizontalOptions = LayoutOptions.EndAndExpand,
                    MinimumWidthRequest = 100
                });

                stackOrderItem.Children.Add(firstStack);
                StackLayout secondStack = new StackLayout();
                foreach (var model in orderItem.OrderModels.OrderByDescending(x => x.ItemType))
                {
                    if (model.ItemType == "Option")
                    {
                        secondStack.Children.Add(new Label()
                        {
                            Text = model.ItemName,
                            HorizontalOptions = LayoutOptions.StartAndExpand
                        });
                    }
                    else
                    {
                        foreach (var extramodel in model.OrderModelExtras)
                        {
                            string text = "+ " + extramodel.ItemName;
                            if (Convert.ToInt32(extramodel.Qty) > 1)
                            {
                                text = "+ " + extramodel.Qty + " x " + extramodel.ItemName;
                            }
                            secondStack.Children.Add(new Label()
                            {
                                Text = text,
                                HorizontalOptions = LayoutOptions.StartAndExpand
                            });
                        }
                    }

                }
                stackOrderItem.Children.Add(secondStack);

            }
            lblTotalPrice.Text = "£" + ((decimal)_order.OrderTotal).ToString("F2");
            lblSubTotal.Text = "£" + ((decimal)_order.SubTotal).ToString("F2");
            if (StaticFields.Deliverytype == "d")
            {

                SetToDelivery();
            }
            else
            {
                SetToCollection();
            }
            if (_order.ServiceCharge != null)
            {
                stackServiceCharge.IsVisible = true;
                lblServiceCharge.Text = "£" + ((decimal)_order.ServiceCharge).ToString("F2");
            }
        }
        private async void btnHomeClicked(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, MessagingFields.OrderCompleted, _order.ID);
            await Navigation.PopModalAsync();

        }
        private void SetToCollection()
        {
            UpdateDiscountInfo();
            stackDeliveryFee.IsVisible = false;


        }
        private void SetToDelivery()
        {
            if (StaticFields.CurrentDeliveryInfo != null)
            {
                UpdateDiscountInfo();
                stackDeliveryFee.IsVisible = true;
                lblDeliveryFee.Text = "£" + ((decimal)StaticFields.CurrentDeliveryInfo.DeliveryFee).ToString("F2");


                lblTotalPrice.Text = "£" + ((decimal)_order.OrderTotal).ToString("F2");
                lblSubTotal.Text = "£" + ((decimal)_order.SubTotal).ToString("F2");



            }

        }

        private void UpdateDiscountInfo()
        {

            if (_order.Discount != null && _order.Discount > 0)
            {
                lblDiscountPercentage.Text = _order.DiscountPercentage + "% discount";
                lblDiscount.Text = "-" + ((decimal)_order.Discount).ToString("F2");
                stackDiscount.IsVisible = true;

            }
            else
            {
                stackDiscount.IsVisible = false;
            }
        }
    }
}
