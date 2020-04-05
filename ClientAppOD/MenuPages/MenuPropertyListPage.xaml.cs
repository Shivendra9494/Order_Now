using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ClientAppOD.CustomModels;
using ClientAppOD.Helper;
using ClientAppOD.PushNotification;
using ClientAppOD.SubPages;
using ClientAppOD.TrialPages;
using OD.Data;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using UIKit;
using Xamarin.Forms;

namespace ClientAppOD.MenuPages
{
    public partial class MenuPropertyListPage : ContentPage
    {
        private ObservableCollection<OD.Data.MenuItem> _menuItems = new ObservableCollection<OD.Data.MenuItem>();
        private OrderHelper orderHelper = new OrderHelper();
        int descMaxLines = 0;
        int _tapCount = 0;
        public MenuPropertyListPage(ObservableCollection<OD.Data.MenuItem> menuItems, string catName, string catDes)
        {
            InitializeComponent();
            double top = 0;
            //if (UIApplication.SharedApplication.KeyWindow != null)
            //{
            //    var sa = UIApplication.SharedApplication.KeyWindow.SafeAreaInsets;
            //    top = sa.Top;
            //}
            //this.Padding = new Thickness(0, top, 0, 0);
            frameCheckOut.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => basketClicked()
                )
            });
            stackDesc.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => DownArraowClicked()
                )
            });
            alergyStack.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => AlergyTapped()
                )
            });
            _menuItems = menuItems;
            lblTitle.Text = catName;
            UpdateMenuStack();
            catDes = "";
            if (menuItems.Count() > 3)
            {
                foreach (var menu in menuItems.Take(4))
                {
                    catDes += menu.Name + ", ";
                }
            }
            else
            {
                foreach (var menu in menuItems)
                {
                    catDes += menu.Name + ", ";
                }
            }
            catDes = catDes.Substring(0, catDes.Length - 2);
            MenuCatDesLabel.Text = catDes;
            descMaxLines = MenuCatDesLabel.MaxLines;
            MenuCatDesLabel.LineBreakMode = LineBreakMode.TailTruncation;
            if (string.IsNullOrEmpty(catDes))
            {
                MenuCatDesLabel.IsVisible = false;
                DescLine.IsVisible = false;
            }
            UpdateTotal();
            
        }

        private void UpdateMenuStack()
        {
            menuStack.Children.Clear();
            foreach (var item in _menuItems)
            {
                var stackTop = new StackLayout()
                {
                    Padding = new Thickness(0, 8),
                    Spacing = 2,
                    VerticalOptions = LayoutOptions.Center
                };
                
                var stack1 = new StackLayout()
                {
                    Padding = new Thickness(20, 0),
                    Spacing = 0,
                    Orientation = StackOrientation.Horizontal
                };
                var btn = new Button()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0, 10, 0),
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    BorderWidth = 0,
                    FontSize = 14,
                    BackgroundColor = Color.Green,
                    TextColor = Color.White,
                    WidthRequest = 24,
                    HeightRequest = 24,
                    CornerRadius = 12,
                    Text = item.OrderItems.Sum(x=>x.Qta).ToString()
                };
                if (item.MenuItemProperties.Count > 0)
                {
                    btn.IsVisible = false;
                }
                else if (item.MenuItemModels.Count > 0)
                {
                    btn.IsVisible = false;
                }
                else
                {
                    if (item.OrderItems.Count > 0)
                    {
                        btn.IsVisible = true;
                    }
                    else
                    {
                        btn.IsVisible = false;
                    }
                }
                stack1.Children.Add(btn);
                stack1.Children.Add(new Label()
                {
                    Text = item.Name,
                    TextColor = Color.FromHex("282728"),
                    FontSize = 15,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontFamily = "Nunito-Light",
                    HorizontalOptions = LayoutOptions.StartAndExpand
                });
                var priceLabel = new Label()
                {
                    WidthRequest = 80,
                    TextColor = Color.FromHex("236adb"),
                    FontFamily = "Nunito-ExtraLight",
                    HorizontalTextAlignment = TextAlignment.End,
                    HorizontalOptions = LayoutOptions.EndAndExpand,

                };
                priceLabel.Text = orderHelper.GetPriceForMenuItemPage(item.Id);
                stack1.Children.Add(priceLabel);
                stackTop.Children.Add(stack1);
                stackTop.Children.Add(new Label()
                {
                    Margin = new Thickness(20, 0),
                    TextColor = Color.FromHex("2E3032"),
                    FontFamily = "Nunito-ExtraLight",
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    MaxLines = 2,
                    LineBreakMode = LineBreakMode.TailTruncation,
                    HorizontalTextAlignment = TextAlignment.Start,
                    Text=item.Description
                });
                
                stackTop.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => ItemSelectedHandle(item)
                )
                });
                menuStack.Children.Add(stackTop);
                if (btn.IsVisible == false)
                {
                    foreach (var orderItem in item.OrderItems)
                    {
                        var grid = new Grid()
                        {
                            Padding = new Thickness(5, 15),
                            Margin = new Thickness(0),
                            BackgroundColor = Color.FromHex("#fcfbfa")
                        };
                        grid.ColumnDefinitions.Add(new ColumnDefinition()
                        {
                            Width = 50
                        });
                        grid.ColumnDefinitions.Add(new ColumnDefinition()
                        {
                            Width = GridLength.Star
                        });
                        grid.ColumnDefinitions.Add(new ColumnDefinition()
                        {
                            Width = 60
                        });
                        grid.Children.Add(new Button()
                        {
                            HorizontalOptions = LayoutOptions.Center,
                            Margin = new Thickness(0, 0, 0, 0),
                            VerticalOptions = LayoutOptions.StartAndExpand,
                            BorderWidth = 0,
                            FontSize = 14,
                            BackgroundColor = Color.Green,
                            TextColor = Color.White,
                            WidthRequest = 24,
                            HeightRequest = 24,
                            CornerRadius = 12,
                            Text = orderItem.Qta.ToString()

                        }, 0, 0);
                        grid.Children.Add(new Label()
                        {
                            HorizontalTextAlignment = TextAlignment.Start,
                            Text = orderItem.OrderText,
                            HorizontalOptions = LayoutOptions.StartAndExpand
                        }, 1, 0);
                        grid.Children.Add(new Label()
                        {
                            HorizontalTextAlignment = TextAlignment.Center,
                            HorizontalOptions = LayoutOptions.CenterAndExpand,
                            VerticalOptions = LayoutOptions.StartAndExpand,
                            VerticalTextAlignment=TextAlignment.Start,
                            TextColor = Color.FromHex("EB6361"),
                            WidthRequest=120,
                            Text= "£" + ((decimal)orderItem.Total).ToString("F2")
                        }, 2, 0);
                        StackLayout stackLayout = new StackLayout();
                        stackLayout.GestureRecognizers.Add(new TapGestureRecognizer
                        {
                            Command = new Command(() => listOrderItemSelected(orderItem)
                )
                        });
                        stackLayout.Children.Add(grid);
                        menuStack.Children.Add(stackLayout);
                    }
                }
                menuStack.Children.Add(new BoxView()
                {
                    Margin = new Thickness(0,10)
                });
            }
        }

        async void searchClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SearchMenuPage());
        }
        async void backClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
        async Task basketClicked()
        {
            _tapCount += 1;
            if (_tapCount > 1)
            {
                _tapCount = 0;
                return;
            }
            await Navigation.PushAsync(new BasketPage());
            _tapCount = 0;
        }
        protected override void OnAppearing()
        {
            UpdateMenuStack();

            UpdateTotal();
            //MessagingCenter.Subscribe<MenuItemsModel, OrderItem>(this, "ItemAdded", OptionSelected);
            base.OnAppearing();
        }

        private void UpdateTotal()
        {
            UpdateBottomBar();
            MessagingCenter.Send(this, MessagingFields.AddOrderItem, "aa");
        }
        private void UpdateTotalFromModelPage(OptionsModalPageView sender, int i)
        {
            try
            {
                UpdateMenuStack();
                UpdateBottomBar();
                MessagingCenter.Unsubscribe<OptionsModalPageView, int>(this, MessagingFields.AddOrderItem);
                MessagingCenter.Send(this, MessagingFields.AddOrderItem, "aa");
            }
            catch (Exception ex)
            {

            }
        }
        private void UpdateTotalFromModelEditPage(OptionsModalEditPageView sender, int i)
        {
            UpdateMenuStack();
            UpdateBottomBar();
            MessagingCenter.Unsubscribe<OptionsModalEditPageView, int>(this, MessagingFields.EditOrderItem);
            MessagingCenter.Send(this, MessagingFields.AddOrderItem, "aa");
        }
        protected async void ItemSelectedHandle(OD.Data.MenuItem item)
        {
            _tapCount += 1;
            if (_tapCount > 1)
            {
                _tapCount = 0;
                return;
            }
            
            if (item != null)
            {
                var menuItem = item as OD.Data.MenuItem;

                if (menuItem.OrderItems.Count > 0 && menuItem.MenuItemModels.Count == 0 && menuItem.MenuItemProperties.Count == 0)
                {
                    var orderItem = menuItem.OrderItems.First();
                    MessagingCenter.Subscribe<OptionsModalEditPageView, int>(this, MessagingFields.EditOrderItem, UpdateTotalFromModelEditPage);
                    await Navigation.PushPopupAsync(new TransparentModel(new OptionsModalEditPageView(orderItem, menuItem, menuItem.Name, menuItem.Description)));

                }
                else
                {
                    MessagingCenter.Subscribe<OptionsModalPageView, int>(this, MessagingFields.AddOrderItem, UpdateTotalFromModelPage);

                    await Navigation.PushPopupAsync(new TransparentModel(new OptionsModalPageView(menuItem, menuItem.Name, menuItem.Description)));
                    
                }

            }
            _tapCount = 0;
        }

        private void UpdateBottomBar()
        {
            decimal TotalPrice = 0;
            int TotalItem = 0;
            foreach (var AllCat in MenuCatHelper.MenuCategories.Where(x => x.Name != "Want to repeat?" && x.MenuItems.Where(y => y.OrderItems.Count > 0).Count() > 0))
            {
                foreach (var MenuItem in AllCat.MenuItems.Where(x => x.OrderItems.Count > 0))
                {
                    foreach (var OrderItem in MenuItem.OrderItems)
                    {
                        TotalPrice += (decimal)OrderItem.Total;
                        TotalItem += (int)OrderItem.Qta;
                    }
                }
            }
            if (TotalItem == 0)
            {
                frameCheckOut.IsVisible = false;
            }
            else
            {

                lblBasket.Text = "£" + TotalPrice.ToString("F2");
                btnOrderCount.Text = TotalItem.ToString();

                if (StaticFields.CurrentDeliveryInfo != null)
                {
                    if (StaticFields.Deliverytype == "d" && TotalPrice < StaticFields.CurrentDeliveryInfo.MinimumAmount)
                    {
                        lblDeliveryMessage.Text = "Delivery is £" + ((decimal)(StaticFields.CurrentDeliveryInfo.MinimumAmount - TotalPrice)).ToString("F2") + " away";
                        frameCheckOut.BackgroundColor = Color.FromHex("fcf1f0");
                        btnOrderCount.TextColor = Color.FromHex("fcf1f0");
                        btnOrderCount.BackgroundColor = Color.Black;
                        btnOrderCount.BorderColor = Color.FromHex("fcf1f0");
                        imageBasket.Source = ImageSource.FromFile("BasketBlack.png");
                        lblBasket.TextColor = Color.Black;
                        lblDeliveryMessage.TextColor = Color.Black;
                    }
                    else
                    {
                        lblDeliveryMessage.Text = "Ready to checkout";
                        frameCheckOut.BackgroundColor = Color.FromHex("236abd");
                        btnOrderCount.TextColor = Color.FromHex("236adb");
                        btnOrderCount.BackgroundColor = Color.White;
                        btnOrderCount.BorderColor = Color.FromHex("236abd");
                        imageBasket.Source = ImageSource.FromFile("BasketWhite.png");
                        lblBasket.TextColor = Color.White;
                        lblDeliveryMessage.TextColor = Color.White;
                    }
                }
                else
                {
                    lblDeliveryMessage.Text = "Ready to checkout";
                    frameCheckOut.BackgroundColor = Color.FromHex("236abd");
                    btnOrderCount.TextColor = Color.FromHex("236adb");
                    btnOrderCount.BackgroundColor = Color.White;
                    btnOrderCount.BorderColor = Color.FromHex("236abd");
                    imageBasket.Source = ImageSource.FromFile("BasketWhite.png");
                    lblBasket.TextColor = Color.White;
                    lblDeliveryMessage.TextColor = Color.White;
                }
                frameCheckOut.IsVisible = true;
            }
            updateDiscountLessText(TotalPrice);
        }
        private async void listOrderItemSelected(OrderItem orderItem)
        {
            _tapCount += 1;
            if (_tapCount > 1)
            {

                return;
            }
            if (orderItem != null)
            {
                
                var menuItem = _menuItems.FirstOrDefault(x => x.Id == orderItem.MenuItemId);
                MessagingCenter.Subscribe<OptionsModalEditPageView, int>(this, MessagingFields.EditOrderItem, UpdateTotalFromModelEditPage);
                await Navigation.PushPopupAsync(new TransparentModel(new OptionsModalEditPageView(orderItem, menuItem, menuItem.Name, menuItem.Description)));

            }
            _tapCount = 0;

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
        private void DownArraowClicked()
        {
            _tapCount += 1;
            if (_tapCount > 1)
            {
                _tapCount = 0;
                return;
            }
            if (btnDesc.Rotation == 0)
            {
                MenuCatDesLabel.LineBreakMode = LineBreakMode.WordWrap;
                if (descMaxLines < 0)
                {
                    descMaxLines = 3;
                }
                MenuCatDesLabel.MaxLines = descMaxLines;
                btnDesc.Rotation = 180;
            }
            else
            {
                MenuCatDesLabel.LineBreakMode = LineBreakMode.TailTruncation;
                MenuCatDesLabel.MaxLines = 1;
                btnDesc.Rotation = 0;
            }
            _tapCount = 0;
        }
        private void updateDiscountLessText(decimal TotalPrice)
        {

            if (StaticFields.CurrentDiscount != null && TotalPrice > 0)
            {
                if (StaticFields.CurrentDiscount.MinimumAmount > TotalPrice)
                {
                    frameDiscountLess.IsVisible = true;
                    lblDiscountLess.Text = "Spend £" + (StaticFields.CurrentDiscount.MinimumAmount - TotalPrice) + " more to get " + StaticFields.CurrentDiscount.DiscountPercentage + "% discount";
                }
                else
                {
                    lblDiscountLess.Text = "";
                    frameDiscountLess.IsVisible = false;
                }
            }
            else if (StaticFields.CurrentDiscount == null && StaticFields.CurrentStoreInfo != null && StaticFields.CurrentCustomer != null && TotalPrice > 0)
            {
                if (StaticFields.CurrentStoreInfo.LoyaltyMinimumAmount != null)
                {
                    if (StaticFields.CurrentStoreInfo.LoyaltyMinimumAmount > TotalPrice)
                    {
                        frameDiscountLess.IsVisible = true;
                        lblDiscountLess.Text = "Spend £" + (StaticFields.CurrentStoreInfo.LoyaltyMinimumAmount - TotalPrice) + " more to earn a loyalty stamp";
                    }
                    else
                    {
                        lblDiscountLess.Text = "";
                        frameDiscountLess.IsVisible = false;
                    }
                }
                else
                {
                    lblDiscountLess.Text = "";
                    frameDiscountLess.IsVisible = false;
                }
            }
            else
            {

                lblDiscountLess.Text = "";
                frameDiscountLess.IsVisible = false;
            }
        }
        private void DownArraowClicked(object sender, EventArgs e)
        {
            DownArraowClicked();
        }
    }
}
