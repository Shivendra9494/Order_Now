using System;
using System.Collections.Generic;
using System.Linq;
using ClientAppOD.APIPost;
using ClientAppOD.CustomModels;
using ClientAppOD.Helper;
using OD.Data;
using Xamarin.Forms;

namespace ClientAppOD.UserPages
{
    public partial class OrderDetailPage : ContentPage
    {
        private Order _order { get; set; }
        OrderPostHelper orderPostHelper = new OrderPostHelper();
        TimeHelper timeHelper = new TimeHelper();
        private bool _FromCurrentOrder=false;
        public OrderDetailPage(Order order,bool FromCurrentOrder=false)
        {
            InitializeComponent();
            _FromCurrentOrder = FromCurrentOrder;
            if (StaticFields.CurrentStoreInfo != null)
            {
                if (StaticFields.CurrentStoreInfo.NativeAppImage != null)
                {
                    TopImage.Source = new UriImageSource
                    {
                        Uri = new Uri(StaticFields.CurrentStoreInfo.NativeAppImage),
                        CachingEnabled = true,
                        CacheValidity = new TimeSpan(1, 0, 0, 0)
                    };
                }
            }
            _order = order;
            lblOrderNumber.Text = "Order No. " + order.ID;
            CheckOrderStatus(order);

            //lblDeliveryTime.Text=timeHelper.CalTime(_order.ResponceFromPrinter);
            //lblDeliveryType.Text = order.DeliveryType == "d" ? "Delivery Order" : "Collection Order";
            lblPayment.Text = order.PaymentType == PaymentType.Card ? "Paid Order" : "Cash Order";

            ImageStore.Source = ImageSource.FromFile("store" + order.BusinessDetailID + ".png");
            lblStoreName.Text = StaticFields.CurrentStoreInfo.BusinessName;
            lblStoreAddress.Text = StaticFields.CurrentStoreInfo.Address;
            if (order.DeliveryType == "d")
            {
                lblDeliveringTo.Text = "Delivering to :";
                lblDeliveryAddress.Text = order.Address;
            }
            else
            {
                lblDeliveringTo.Text = "Collection";
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

        private bool CheckOrderStatus(Order order)
        {
            
            if (order.Cancelled)
            {
                lblOrderDate.IsVisible = true;
                lblOrderDate.Text = "Cancelled" + Environment.NewLine + "as "+ _order.ResponceFromPrinter;
                lblOrderDate.TextColor = Color.FromHex("ff4f55");
                GridMain.IsVisible = false;
                TopImage.IsVisible = true;
                lblDeliveryType.IsVisible = false;
                lblSupportHandlingMessage.IsVisible = false;
                return false;
            }
            else if (_order.OrderDate != null)
            {
                
                var orderDate = Convert.ToDateTime(_order.OrderDate);
                if (_order.AcknowledgedDate != null)
                {
                    orderDate = Convert.ToDateTime(_order.AcknowledgedDate);
                }
                if (string.IsNullOrEmpty(_order.ResponceFromPrinter))
                {
                    lblOrderDate.IsVisible = false;
                    lblDeliveryType.Text = "Awaiting Confirmation";
                    GridMain.IsVisible = true;
                    TopImage.IsVisible = false;
                    btnRepeat.IsVisible = false;
                    if (OrderConfirmedCircle.BackgroundColor == Color.White)
                        OrderConfirmedCircle.BackgroundColor = Color.Transparent;
                    else
                        OrderConfirmedCircle.BackgroundColor = Color.White;
                    if (string.IsNullOrEmpty(_order.ResponceFromPrinter) && _order.SupportAcknowledgedAt != null)
                    {
                        lblSupportHandlingMessage.IsVisible = true;
                        
                    }
                    
                        return true;
                    
                    
                }
               
                else
                {
                    lblOrderDate.IsVisible = true;
                    lblSupportHandlingMessage.IsVisible = false;
                    var timeToDeliver = timeHelper.CalTime(_order.ResponceFromPrinter, orderDate);
                    if(DateTime.UtcNow> timeToDeliver.AddMinutes(15))
                    {
                        lblOrderDate.IsVisible = true;
                        lblOrderDate.Text = ((DateTime)order.OrderDate).ToString("dd MMMM yyyy");
                        lblDeliveryType.Text = order.DeliveryType == "d" ? "Delivered" : "Collected";
                        lblDeliveryType.TextColor = Color.LightGreen;
                        GridMain.IsVisible = false;
                        TopImage.IsVisible = true;
                        return false;
                    }
                    else
                    {
                        lblOrderDate.Text = order.DeliveryType == "d" ? "Arriving around" : "Ready to collect around";
                        OrderConfirmedCircle.WidthRequest = 24;
                        OrderConfirmedCircle.HeightRequest = 24;
                        OrderConfirmedCircle.CornerRadius = 12;
                        OrderConfirmedCircle.BackgroundColor = Color.White;
                        OrderConfirmCheckImage.IsVisible = true;
                        lblDeliveryType.Text = timeToDeliver.ToString("hh:mm tt") + " - " + timeToDeliver.AddMinutes(15).ToString("hh:mm tt");
                        OrderConfirmedThickLine.HeightRequest = 90;
                        GridMain.IsVisible = true;
                        TopImage.IsVisible = false;
                        btnRepeat.IsVisible = false;
                        return false;
                    }
                    
                }
               

            }
            else
            {
                return true;
            }
        }

        private void SetToCollection()
        {
            UpdateDiscountInfo();
            stackDeliveryFee.IsVisible = false;


        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                UpdateOrder();
                return CheckOrderStatus(_order);
            });
            Device.StartTimer(TimeSpan.FromMilliseconds(200), () =>
            {

                return CheckOrderStatus(_order);
            });
        }

        private async void UpdateOrder()
        {
            _order = await orderPostHelper.GetOrder(_order.ID);
            if (StaticFields.CurrentCustomer.Orders.Count > 0)
            {
                try
                {
                    StaticFields.CurrentCustomer.Orders.Remove(StaticFields.CurrentCustomer.Orders.First(o => o.ID == _order.ID));
                    StaticFields.CurrentCustomer.Orders.Add(_order);
                    StaticFields.CurrentCustomer.Orders.OrderByDescending(x => x.ID);
                }
                catch
                {

                }
            }
        }


        private async void Rbutton_Clicked(object sender, EventArgs e)
        {
            if (StaticFields.CurrentOrder == null)
            {
                StaticFields.CurrentOrder = new OD.Data.Order();

            }
            foreach (var item in _order.OrderItems)
            {
                var menuItemCat = MenuCatHelper.MenuCategories.FirstOrDefault(x => x.Name != "Want to repeat?" && x.MenuItems.Any(y => y.Id == item.MenuItemId));
                if (menuItemCat != null)
                {
                    var menuItem = menuItemCat.MenuItems.FirstOrDefault(x => x.Id == item.MenuItemId);
                    item.OrderText = menuItem.Name + " ";
                    if (item.MenuItemPropertyId > 0)
                    {
                        var SelectedProperty = menuItem.MenuItemProperties.FirstOrDefault(x => x.Id == item.MenuItemPropertyId);
                        if (SelectedProperty != null)
                        {
                            item.OrderText += " " + SelectedProperty.Name + ", ";

                        }
                    }
                    foreach (var model in item.OrderModels)
                    {
                        if (model.ItemType == "Option")
                        {
                            item.OrderText += model.ItemName + ", ";
                        }
                        else
                        {
                            foreach (var extra in model.OrderModelExtras)
                            {
                                if (Convert.ToInt32(extra.Qty) > 1)
                                {
                                    item.OrderText += extra.Qty + " x " + extra.ItemName + ", ";
                                }
                                else
                                {
                                    item.OrderText += extra.ItemName + ", ";
                                }
                            }
                        }
                    }
                    var length = item.OrderText.Length - 3;
                    item.OrderText = item.OrderText.Remove(length, 2);


                    menuItem.OrderItems.Add(item);
                    menuItemCat.SelectedQty += (int)item.Qta;
                }
            }
            MessagingCenter.Send(this, MessagingFields.AddOrderItem, "");
            MessagingCenter.Send(this, "SetActiveTab", 0);
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
                lblDiscount.Text = "- £" + ((decimal)_order.Discount).ToString("F2");
                stackDiscount.IsVisible = true;

            }
            else
            {
                stackDiscount.IsVisible = false;
            }
        }

        void scrolll_Scrolled(System.Object sender, Xamarin.Forms.ScrolledEventArgs e)
        {

            //stackMain.Margin = new Thickness(0, 150 - scrolll.ScrollY, 0, 0);
            //scrolll.Padding= new Thickness(0, 0, 0, 150 - scrolll.ScrollY);
            try
            {
                if (scrolll.ScrollY > -101 && scrolll.ScrollY < 0)
                    GridMain.Padding = new Thickness(10, -100 - scrolll.ScrollY, 10, 10);
            }
            catch
            {

            }



        }

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            if (_FromCurrentOrder)
            {
                MessagingCenter.Send(this, MessagingFields.OrderCompleted, _order.ID);
                Navigation.PopModalAsync();
            }
            else
            {
                Navigation.PopAsync();
            }
           
            
        }
    }
}
