using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientAppOD.APIPost;
using ClientAppOD.CustomModels;
using ClientAppOD.ErrorPages;
using ClientAppOD.Helper;
using ClientAppOD.MenuPages;
using ClientAppOD.OrderPages;
using ClientAppOD.SubPages;
using ClientAppOD.TrialPages;
using ClientAppOD.UserPages;
using OD.Data;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace ClientAppOD
{
    public partial class BasketPage : ContentPage
    {
        int _tapCount = 0;
        List<OrderViewItem> OrderItems = new List<OrderViewItem>();
        public BasketPage()
        {
            InitializeComponent();

            _tapCount = 0;
            alergyStack.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => AlergyTapped()
                )
            });
            stackGotoPayment.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => btnCheckOutclicked()
                )
            });
            btnDelivery.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => btnDeliveryClicked()
                )
            });
            btnCollection.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => btnCollectionclicked()
                )
            });
            StaticFields.CurrentOrder = new OD.Data.Order();
            StaticFields.CurrentOrder.DeliveryType = "d";
            UpdateBasket();
        }

        private void UpdateBasket()
        {
            if (!StaticFields.CurrentStoreInfo.DeliveryAllowed)
            {
                btnDelivery.IsVisible = false;
            }
            if (!StaticFields.CurrentStoreInfo.CollectionAllowed)
            {
                btnCollection.IsVisible = false;
            }
            var TotalQty = MenuCatHelper.MenuCategories.Sum(x => x.SelectedQty);
            if (TotalQty > 0)
            {
                stackMain.IsVisible = true;
                stackEmptyMenu.IsVisible = false;
            }
            else
            {
                stackMain.IsVisible = false;
                stackEmptyMenu.IsVisible = true;
            }

            decimal total = 0;
            decimal SubTotal = 0;
            //stackOrderItem.Children.Clear();
            OrderItems.Clear();
            listViewItems.ItemsSource = null;
            foreach (var cat in MenuCatHelper.MenuCategories.Where(x => x.Name != "Want to repeat?" && x.SelectedQty > 0))
            {
                foreach (var menuItem in cat.MenuItems.Where(x => x.OrderItems.Count() > 0))
                {
                    foreach (var orderItem in menuItem.OrderItems)
                    {
                        StaticFields.CurrentOrder.OrderItems.Add(orderItem);
                        total += (decimal)orderItem.Total;
                        SubTotal += (decimal)orderItem.Total;

                        OrderViewItem orderViewItem = new OrderViewItem();
                        orderViewItem.orderItem = orderItem;
                        orderViewItem.menuItem = menuItem;
                        orderViewItem.ItemName = orderItem.Name;
                        orderViewItem.Qta = orderItem.Qta.ToString();
                        orderViewItem.Total = orderItem.Total.ToString();
                        string text = "";
                        foreach (var model in orderItem.OrderModels.OrderByDescending(x => x.ItemType))
                        {

                            if (model.ItemType == "Option")
                            {
                                text += model.ItemName + Environment.NewLine;
                            }
                            else
                            {

                                foreach (var extramodel in model.OrderModelExtras)
                                {

                                    if (Convert.ToInt32(extramodel.Qty) > 1)
                                    {
                                        text += "  + " + extramodel.Qty + " x " + extramodel.ItemName + Environment.NewLine;
                                    }
                                    else
                                    {
                                        text += "  + " + extramodel.ItemName + Environment.NewLine;
                                    }
                                }

                            }

                        }
                        orderViewItem.OptionsText = text;

                        OrderItems.Add(orderViewItem);
                    }

                }

            }
            if (stackOrderItem.Children.Count > 0)
            {
                //stackOrderItem.Children.RemoveAt(stackOrderItem.Children.Count - 1);
            }
            else
            {
                frameCheckOut.IsVisible = false;
            }
            if (StaticFields.CurrentOrder.ServiceCharge != null)
            {
                StaticFields.CurrentOrder.ServiceCharge = null;
            }
            StaticFields.CurrentOrder.OrderTotal = Math.Round(total, 2);
            StaticFields.CurrentOrder.SubTotal = Math.Round(total, 2);

            if (StaticFields.Deliverytype == "d")
            {

                SetToDelivery();
            }
            else
            {
                SetToCollection();
            }
            updateDiscountLessText();
            listViewItems.ItemsSource = OrderItems;
        }


        private void UpdateTotalFromModelEditPage(OptionsModalEditPageView sender, int i)
        {
            UpdateBasket();
            MessagingCenter.Unsubscribe<OptionsModalEditPageView, int>(this, MessagingFields.EditOrderItem);
        }
        protected void PickerFocused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            var picker = sender as Xamarin.Forms.Picker;
            picker.Items.Clear();
            for (int i = 1; i < 100; i++)
            {
                picker.Items.Add(i.ToString());
            }

        }
        async void backClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            updateDiscountLessText();
        }

        private void updateDiscountLessText()
        {

            if (StaticFields.CurrentDiscount != null)
            {
                if (StaticFields.CurrentDiscount.MinimumAmount > StaticFields.CurrentOrder.OrderTotal)
                {
                    btnDiscountLess.Text = "Spend £" + (StaticFields.CurrentDiscount.MinimumAmount - StaticFields.CurrentOrder.OrderTotal) + " more to get " + StaticFields.CurrentDiscount.DiscountPercentage + "% discount";
                }

                else
                {
                    btnDiscountLess.Text = "";
                }
            }
            else if (StaticFields.CurrentDiscount == null && StaticFields.CurrentStoreInfo.LoyaltyMinimumAmount != null && StaticFields.CurrentCustomer != null && StaticFields.CurrentOrder.OrderTotal > 0)
            {
                if (StaticFields.CurrentStoreInfo.LoyaltyMinimumAmount > StaticFields.CurrentOrder.OrderTotal)
                {
                    btnDiscountLess.Text = "Spend £" + (StaticFields.CurrentStoreInfo.LoyaltyMinimumAmount - StaticFields.CurrentOrder.OrderTotal) + " more to earn a loyalty stamp";
                }
                else
                {
                    btnDiscountLess.Text = "";
                }

            }
            else
            {
                btnDiscountLess.Text = "";
            }
            if (StaticFields.CurrentOrder.OrderItems.Count == 0)
            {
                btnDiscountLess.Text = "";
                frameCheckOut.IsVisible = false;
            }
        }

        protected async void btnCheckOutclicked()
        {
            if (StaticFields.CurrentStoreInfo != null)
            {

                if (StaticFields.Deliverytype == "d" && StaticFields.CurrentStoreInfo.NextDeliveryTime == null)
                {
                    await DisplayAlert(StaticFields.CurrentStoreInfo.BusinessName, "Delivery not available now", "OKAY");
                    _tapCount = 0;
                    return;
                }
                else if (StaticFields.Deliverytype == "c" && StaticFields.CurrentStoreInfo.NextCollectionTime == null)
                {
                    await DisplayAlert(StaticFields.CurrentStoreInfo.BusinessName, "Collection not available now", "OKAY");
                    _tapCount = 0;
                    return;
                }
                var DelTime = "";
                var colTime = "";
                var CurrentTime = DateTimeOffset.UtcNow;
                if (StaticFields.CurrentStoreInfo.NextCollectionTime != null)
                {
                    if (StaticFields.CurrentStoreInfo.NextCollectionTime != null && ((DateTimeOffset)StaticFields.CurrentStoreInfo.NextCollectionTime).Date == StaticFields.CurrentStoreInfo.CurrentServerTime.Date)
                    {
                        colTime = "Collection start at " + ((DateTimeOffset)(StaticFields.CurrentStoreInfo.NextCollectionTime)).ToString("hh:mm tt");
                    }
                    else if (StaticFields.CurrentStoreInfo.NextCollectionTime != null)
                    {
                        colTime = "Collection start tommorrow at " + ((DateTimeOffset)(StaticFields.CurrentStoreInfo.NextCollectionTime)).ToString("hh:mm tt");
                    }
                }
                if (StaticFields.CurrentStoreInfo.NextDeliveryTime != null)
                {
                    if (StaticFields.CurrentStoreInfo.NextDeliveryTime != null && ((DateTimeOffset)StaticFields.CurrentStoreInfo.NextDeliveryTime).Date == StaticFields.CurrentStoreInfo.CurrentServerTime.Date)
                    {
                        DelTime = "Delivery start at " + ((DateTimeOffset)(StaticFields.CurrentStoreInfo.NextDeliveryTime)).ToString("hh:mm tt");
                    }
                    else if (StaticFields.CurrentStoreInfo.NextDeliveryTime != null)
                    {
                        DelTime = "Delivery start tommorrow at " + ((DateTimeOffset)(StaticFields.CurrentStoreInfo.NextDeliveryTime)).ToString("hh:mm tt");
                    }
                }
                if (StaticFields.CurrentStoreInfo.NextDeliveryTime != null)
                {
                    if (StaticFields.Deliverytype == "d" && !(StaticFields.CurrentStoreInfo.NextDeliveryTime <= CurrentTime && StaticFields.CurrentStoreInfo.DeliveryClosedTime >= CurrentTime))
                    {
                        await DisplayAlert(StaticFields.CurrentStoreInfo.BusinessName + Environment.NewLine + "is closed now", DelTime + Environment.NewLine + colTime, "OKAY");
                        _tapCount = 0;
                        return;
                    }
                }
                if (StaticFields.CurrentStoreInfo.NextCollectionTime != null)
                {

                    if (StaticFields.Deliverytype == "c" && !(StaticFields.CurrentStoreInfo.NextCollectionTime <= CurrentTime && StaticFields.CurrentStoreInfo.CollectionClosedTime >= CurrentTime))
                    {
                        await DisplayAlert(StaticFields.CurrentStoreInfo.BusinessName + Environment.NewLine + "is closed now", DelTime + Environment.NewLine + colTime, "OKAY");
                        _tapCount = 0;
                        return;
                    }
                }

                if (!StaticFields.CurrentStoreInfo.IsOpen)
                {
                    await DisplayAlert(StaticFields.CurrentStoreInfo.BusinessName, "closed now", "OKAY");
                    _tapCount = 0;
                    return;
                }


            }
            if (StaticFields.CurrentCustomer == null)
            {
                MessagingCenter.Subscribe<UserLoginPage, bool>(this, MessagingFields.UserLoggedInChangedBasket, UpdateAfterLogin);
                var page = new Xamarin.Forms.NavigationPage(new UserLoginPage(false))
                {
                    BarTextColor = Color.FromHex("236adb")
                };

                page.On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.FormSheet);
                await Navigation.PushModalAsync(page, true);
            }
            else
            {
                await Navigation.PushModalAsync(new Xamarin.Forms.NavigationPage(new CheckOutPage())
                {
                    BarTextColor = Color.FromHex("236adb")


                }, true);
            }
        }
        private async void UpdateAfterLogin(UserLoginPage sender, bool success)
        {
            if (StaticFields.CurrentCustomer != null)
            {
                StaticFields.CurrentOrder = new OD.Data.Order();
                StaticFields.CurrentOrder.DeliveryType = "d";
                UpdateBasket();
            }
        }
        protected void btnDeliveryClicked()
        {
            if (StaticFields.CurrentStoreInfo.DeliveryAllowed)
            {
                SetToDelivery();
            }

        }

        private async Task AlergyTapped()
        {
            _tapCount += 1;
            if (_tapCount > 1)
            {

                return;
            }
            await Navigation.PushPopupAsync(new TransparentModel(new AllergyWarning()));

            //await DisplayAlert("Do you have a food allergy?", "If you or someone you are ordering for has a food allergy or intolerance phone the restaurant on  " + StaticFields.CurrentStoreInfo.Phone, "Close");
            _tapCount = 0;
        }

        protected void btnCollectionclicked()
        {
            if (StaticFields.CurrentStoreInfo.DeliveryAllowed)
            {
                SetToCollection();
            }
        }

        private void SetToCollection()
        {
            UpdateDiscountInfo();
            stackDeliveryFee.IsVisible = false;
            lblTotalPrice.Text = "£" + ((decimal)StaticFields.CurrentOrder.OrderTotal).ToString("F2");
            lblSubTotal.Text = "£" + ((decimal)StaticFields.CurrentOrder.SubTotal).ToString("F2");
            btnCollection.BackgroundColor = Color.White;
            btnDelivery.Padding = new Thickness(0);
            btnCollection.Padding = new Thickness(30, 0);
            btnDelivery.BackgroundColor = Color.FromHex("EDEDED");
            StaticFields.CurrentOrder.DeliveryType = "c";
            StaticFields.Deliverytype = "c";
            stackGotoPayment.IsEnabled = true;
            stackGotoPayment.BackgroundColor = Color.FromHex("236abd");
            lbltitleMessage.Text = "You're all set";
            lbltitleMessage.TextColor = Color.Green;
            lblSubTotal.TextColor = Color.White;
            lblDeliveryMessage.TextColor = Color.White;
            StaticFields.CurrentOrder.ShippingFee = 0;
            stackPostcodeApplied.IsVisible = false;
        }
        private void SetToDelivery()
        {
            UpdateDiscountInfo();
            
            if (StaticFields.CurrentDeliveryInfo != null && StaticFields.CurrentDeliveryInfo.Message == "ok")
            {
                decimal DeliveryFee = 0;
                decimal? FreeDeliveryAmount = null;
                if (StaticFields.CurrentDeliveryInfoList != null)
                {
                    if (StaticFields.CurrentDeliveryInfoList.Count > 1)
                    {
                        var DeliveryInfo = StaticFields.CurrentDeliveryInfoList.Where(x => x.MinimumAmount <= StaticFields.CurrentOrder.OrderTotal);
                        DeliveryInfo infos = new DeliveryInfo();
                        if (DeliveryInfo.Count()>0)
                        {
                            infos = DeliveryInfo.OrderByDescending(x => x.MinimumAmount).First();
                        }
                        else
                        {
                            infos= StaticFields.CurrentDeliveryInfoList.OrderBy(x => x.MinimumAmount).First(); 
                        }
                        DeliveryFee =(Decimal)infos.DeliveryFee;
                        FreeDeliveryAmount = infos.FreeDeliveryAmount;
                    }
                }
                if (FreeDeliveryAmount == null
                        || FreeDeliveryAmount >= StaticFields.CurrentOrder.SubTotal)
                {

                    StaticFields.CurrentOrder.SubTotal = StaticFields.CurrentOrder.OrderTotal + Math.Round(DeliveryFee, 2);
                    StaticFields.CurrentOrder.ShippingFee = DeliveryFee;
                    stackDeliveryFee.IsVisible = true;
                    lblDeliveryFee.Text = "£" + (DeliveryFee).ToString("F2");
                }
                else
                {
                    StaticFields.CurrentOrder.ShippingFee = 0;
                    stackDeliveryFee.IsVisible = false;
                    lblDeliveryFee.Text = "";
                }

                lblTotalPrice.Text = "£" + ((decimal)StaticFields.CurrentOrder.OrderTotal).ToString("F2");
                lblSubTotal.Text = "£" + ((decimal)StaticFields.CurrentOrder.SubTotal).ToString("F2");
                stackPostcodeApplied.IsVisible = true;
                lblAppliedPostCode.Text = StaticFields.CurrentPostCode.ToUpper();
                btnDelivery.BackgroundColor = Color.White;
                btnCollection.Padding = new Thickness(0);
                btnDelivery.Padding = new Thickness(30, 0);
                btnCollection.BackgroundColor = Color.FromHex("EDEDED");
                StaticFields.CurrentOrder.DeliveryType = "d";
                StaticFields.Deliverytype = "d";
                if (StaticFields.CurrentDeliveryInfo.MinimumAmount > StaticFields.CurrentOrder.OrderTotal)
                {
                    stackGotoPayment.IsEnabled = false;
                    stackGotoPayment.BackgroundColor = Color.FromHex("f1edef");
                    lbltitleMessage.Text = "Delivery is £" + ((decimal)(StaticFields.CurrentDeliveryInfo.MinimumAmount - StaticFields.CurrentOrder.OrderTotal)).ToString("F2") + " away";
                    lbltitleMessage.TextColor = Color.FromHex("EB6361");
                    lblSubTotal.TextColor = Color.FromHex("2E3032");
                    lblDeliveryMessage.TextColor = Color.FromHex("2E3032");
                }
                else
                {
                    stackGotoPayment.IsEnabled = true;
                    stackGotoPayment.BackgroundColor = Color.FromHex("236abd");
                    lbltitleMessage.Text = "You're all set";
                    lbltitleMessage.TextColor = Color.Green;
                    lblSubTotal.TextColor = Color.White;
                    lblDeliveryMessage.TextColor = Color.White;
                }
                
            }
            else
            {
                MessagingCenter.Subscribe<AskPostCode, int>(this, MessagingFields.UpdateDeliverySwitch, SetDeliveryFromPostCodePage);
                Navigation.PushPopupAsync(new LoginPopupPage());
            }
        }
        private void SetDeliveryFromPostCodePage(AskPostCode sender, int i)
        {
            SetToDelivery();
            MessagingCenter.Unsubscribe<AskPostCode, int>(this, MessagingFields.UpdateDeliverySwitch);

        }
        private void UpdateDiscountInfo()
        {
            StaticFields.CurrentOrder.SubTotal = StaticFields.CurrentOrder.OrderTotal;
            if (StaticFields.CurrentCustomer != null)
            {
                var minAmt = StaticFields.CurrentStoreInfo.LoyaltyMinimumAmount;
                if (minAmt == null)
                {
                    minAmt = 0;
                }
                var ValidOrders = StaticFields.CurrentCustomer.Orders.Where(x => x.Cancelled != true && x.OrderTotal >= minAmt).Count();
                if (ValidOrders > 5)
                {
                    ValidOrders = ValidOrders % 6;
                }
                StaticFields.CurrentDiscount = StaticFields.CurrentStoreInfo.Discounts.FirstOrDefault(x => x.OrderCount == ValidOrders);
                var FirstDis = StaticFields.CurrentStoreInfo.Discounts.FirstOrDefault(x => x.OrderCount == 99);
                if (FirstDis != null)
                {

                    if (StaticFields.CurrentCustomer.Orders.Count == 0)
                    {

                        StaticFields.CurrentDiscount = FirstDis;
                    }
                }
            }
            if (StaticFields.CurrentDiscount != null)
            {
                if ((StaticFields.CurrentOrder.OrderTotal >= StaticFields.CurrentDiscount.MinimumAmount || StaticFields.CurrentDiscount.MinimumAmount == null || StaticFields.CurrentDiscount.MinimumAmount == 0) && (StaticFields.CurrentOrder.OrderTotal <= StaticFields.CurrentDiscount.MaximumAmount || StaticFields.CurrentDiscount.MaximumAmount == null || StaticFields.CurrentDiscount.MaximumAmount == 0))
                {
                    var discount = Math.Round((decimal)(StaticFields.CurrentOrder.SubTotal * StaticFields.CurrentDiscount.DiscountPercentage) / 100, 2);
                    StaticFields.CurrentOrder.SubTotal = Math.Round((decimal)(StaticFields.CurrentOrder.SubTotal - discount), 2);
                    StaticFields.CurrentOrder.Discount = discount;
                    StaticFields.CurrentOrder.DiscountID = StaticFields.CurrentDiscount.ID;
                    StaticFields.CurrentOrder.DiscountPercentage = StaticFields.CurrentDiscount.DiscountPercentage.ToString();
                    lblDiscountPercentage.Text = StaticFields.CurrentOrder.DiscountPercentage + "% discount";
                    lblDiscount.Text = "-£" + ((decimal)StaticFields.CurrentOrder.Discount).ToString("F2");
                    stackDiscount.IsVisible = true;
                }
                else
                {
                    stackDiscount.IsVisible = false;
                }
            }
            else
            {
                stackDiscount.IsVisible = false;
            }
        }
        private void BackToMenuClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            MessagingCenter.Subscribe<AskPostCode, int>(this, MessagingFields.UpdateDeliverySwitch, SetDeliveryFromPostCodePage);
            Navigation.PushPopupAsync(new LoginPopupPage());
        }

        void listOrderItems_ItemSelected(System.Object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var item = e.SelectedItem as OrderViewItem;
                listViewItems.SelectedItem = null;
                MessagingCenter.Subscribe<OptionsModalEditPageView, int>(this, MessagingFields.EditOrderItem, UpdateTotalFromModelEditPage);

                Navigation.PushPopupAsync(new TransparentModel(new OptionsModalEditPageView(item.orderItem, item.menuItem, item.menuItem.Name, item.menuItem.Description)), true);

            }
        }
        public void OnDelete(object sender, EventArgs e)
        {
            var _orderItem = (((Xamarin.Forms.MenuItem)sender).CommandParameter as OrderViewItem).orderItem;
            var _menuItem = (((Xamarin.Forms.MenuItem)sender).CommandParameter as OrderViewItem).menuItem;
            var cat = MenuCatHelper.MenuCategories.FirstOrDefault(x => x.ID == _menuItem.MenuCategoryID);
            if (cat != null)
            {
                cat.SelectedQty -= (int)_orderItem.Qta;
            }
            MenuCatHelper.MenuCategories.First(x => x.ID == _menuItem.MenuCategoryID).MenuItems.First(x => x.Id == _menuItem.Id).OrderItems.Remove(_orderItem);
            StaticFields.CurrentOrder.OrderItems.Remove(_orderItem);
            UpdateBasket();
        }
    }
    public class OrderViewItem
    {
        public string ItemName { get; set; }
        public string Total { get; set; }
        public string Qta { get; set; }
        public string OptionsText { get; set; }
        public OrderItem orderItem { get; set; }
        public OD.Data.MenuItem menuItem { get; set; }
    }

}
