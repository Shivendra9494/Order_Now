using System;
using System.Collections.Generic;
using System.Linq;
using ClientAppOD.CustomModels;
using ClientAppOD.Helper;
using ClientAppOD.MenuPages;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace ClientAppOD.UserPages
{
    public partial class OrderHistoryPage : ContentPage
    {
        public OrderHistoryPage()
        {
            InitializeComponent();
            MessagingCenter.Subscribe<UserLoginPage, bool>(this, MessagingFields.UserLoggedInChangedOrders, UpdateAfterLogin);
           
            RefreshPage();
        }

        private void RefreshPage()
        {
            if (StaticFields.CurrentCustomer == null)
            {
                scroll.IsVisible = false;
                stackLogin.IsVisible = true;
                stackEmptyMenu.IsVisible = false;
            }
            else
            {
                Updatestack();
                stackLogin.IsVisible = false;
                scroll.IsVisible = true;

            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            RefreshPage();
        }
        private void Updatestack()
        {
            stackMain.Children.Clear();
            if (StaticFields.CurrentCustomer.Orders.Count > 0)
            {
                stackEmptyMenu.IsVisible = false;
                foreach (var order in StaticFields.CurrentCustomer.Orders.OrderByDescending(x => x.ID))
                {
                    Frame frame = new Frame();
                    StackLayout stackLayout = new StackLayout()
                    {
                        Spacing = 10,
                        Padding = new Thickness(5)
                    }; stackLayout.Children.Add(new Label()
                    {
                        Text = order.ID.ToString(),
                        IsVisible = false

                    });
                    stackLayout.Children.Add(new Label()
                    {
                        Text = "Order No. " + order.ID

                    });
                    string OrderTypeText = "";
                    if (order.DeliveryType == "d" && order.OrderDate != null)
                    {
                        OrderTypeText = "Delivery - " + ((DateTime)order.OrderDate).ToString("dd/MM/yyyy");
                    }
                    else if (order.DeliveryType == "c" && order.OrderDate != null)
                    {
                        OrderTypeText = "Collection - " + ((DateTime)order.OrderDate).ToString("dd/MM/yyyy");
                    }
                    stackLayout.Children.Add(new Label()
                    {
                        Text = OrderTypeText

                    });
                    Button button = new Button()
                    {
                        Text = "View order",
                        TextColor = Color.FromHex("236adb")
                    };
                    button.Clicked += Button_Clicked;
                    stackLayout.Children.Add(button);
                    var Rbutton = new Button()
                    {
                        Text = "Repeat this order",
                        TextColor = Color.White,
                        BackgroundColor = Color.FromHex("236adb")
                    };
                    Rbutton.Clicked += Rbutton_Clicked;
                    stackLayout.Children.Add(Rbutton);
                    stackLayout.Children.Add(new BoxView());
                    StackLayout innerStack = new StackLayout()
                    {
                        Padding = new Thickness(0),
                        Orientation = StackOrientation.Horizontal
                    };
                    innerStack.Children.Add(new Label()
                    {
                        Text = order.OrderItems.Sum(x => x.Qta) + " items",
                        HorizontalOptions = LayoutOptions.StartAndExpand
                    });
                    innerStack.Children.Add(new Label()
                    {
                        Text = "£" + ((decimal)order.SubTotal).ToString("F2"),
                        HorizontalOptions = LayoutOptions.EndAndExpand
                    });
                    stackLayout.Children.Add(innerStack);
                    frame.Content = stackLayout;
                    stackMain.Children.Add(frame);
                }
            }
            else
            {
                stackEmptyMenu.IsVisible = true;
            }

        }

        private async void Rbutton_Clicked(object sender, EventArgs e)
        {
            var id = Convert.ToInt32((((sender as Button).Parent as StackLayout).Children[0] as Label).Text);
            var order = StaticFields.CurrentCustomer.Orders.FirstOrDefault(x => x.ID == id);
            if(StaticFields.CurrentOrder==null)
            {
                StaticFields.CurrentOrder = new OD.Data.Order();

            }
            foreach (var item in order.OrderItems)
            {
                var menuItemCat = MenuCatHelper.MenuCategories.FirstOrDefault(x => x.Name != "Want to repeat?" && x.MenuItems.Any(y => y.Id == item.MenuItemId));
               if(menuItemCat !=null)
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

        private void Button_Clicked(object sender, EventArgs e)
        {
            var id = Convert.ToInt32((((sender as Button).Parent as StackLayout).Children[0] as Label).Text);
            var order = StaticFields.CurrentCustomer.Orders.FirstOrDefault(x => x.ID == id);
            Navigation.PushAsync(new OrderDetailPage(order));
        }

        async void LoginButtonclicked(System.Object sender, System.EventArgs e)
        {
            stackLogin.IsVisible = false;
            
            var page = new Xamarin.Forms.NavigationPage(new UserLoginPage(true))
            {
                BarTextColor = Color.FromHex("236adb")
            };
            page.On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.FormSheet);
            await Navigation.PushModalAsync(page, true);
        }
        private async void UpdateAfterLogin(UserLoginPage sender, bool success)
        {
            RefreshPage();
            MessagingCenter.Unsubscribe<UserLoginPage, bool>(this, MessagingFields.UserLoggedInChangedOrders);

        }
    }
}
