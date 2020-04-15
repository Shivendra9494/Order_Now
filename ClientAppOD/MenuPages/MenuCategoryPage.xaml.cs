using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ClientAppOD.APIPost;
using ClientAppOD.CustomModels;
using ClientAppOD.ErrorPages;
using ClientAppOD.Helper;
using ClientAppOD.OrderPages;
using ClientAppOD.PushNotification;
using ClientAppOD.SubPages;
using ClientAppOD.TrialPages;
using ClientAppOD.UserPages;
using OD.Data;
using Rg.Plugins.Popup.Extensions;
using SQLite;
using Xamarin.Essentials;
using Xamarin.Forms;

using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace ClientAppOD.MenuPages
{
    public partial class MenuCategoryPage : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        string menu = "";
        int _tapCount = 0;
        PostCodeHelper postCodeHelper = new PostCodeHelper();
        StoreInfoHelper storeInfoHelper = new StoreInfoHelper();
        MenuCatHelper menuCatHelper = new MenuCatHelper();

        public MenuCategoryPage()
        {
            InitializeComponent();
            try
            {

                MessagingCenter.Subscribe<MenuPropertyListPage, string>(this, MessagingFields.AddOrderItem, UpdateTotalText);
                MessagingCenter.Subscribe<OrderDetailPage, int>(this, MessagingFields.OrderCompleted, OrderCompleted);
                MessagingCenter.Subscribe<SearchMenuPage, string>(this, MessagingFields.AddOrderItem, UpdateTotalText);
                MessagingCenter.Subscribe<OrderHistoryPage, string>(this, MessagingFields.AddOrderItem, UpdateTotalText);
                MessagingCenter.Subscribe<OrderDetailPage, string>(this, MessagingFields.AddOrderItem, UpdateTotalText);
                MessagingCenter.Subscribe<UserLoginPage, int>(this, MessagingFields.UserLoggedInChanged, UpdateStoreInfoFromLoginPage);
                MessagingCenter.Subscribe<UserMainMenuPage, int>(this, MessagingFields.UserLoggedOutChanged, UpdateStoreInfoFromLogOutPage);
                lblLoginToView.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => loginClicked()
                )
                });
                lblLoyaltyView.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => loginClicked()
                    )
                });
                StaticFields.Deliverytype = "d";
                CheckInternet();
                //if(StaticFields.OrderOpenedNotificationClicked == true)
                //{
                //    if(StaticFields.CurrentCustomer==null)
                //    {
                //        loginClicked();
                //        HandleNotificationClick();
                //    }
                //}

            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.Message + " | " + ex.StackTrace, "OK");
            }
        }



        private async void loginClicked()
        {
            _tapCount += 1;
            if (_tapCount > 1)
            {
                _tapCount = 0;
                return;
            }
            if (StaticFields.CurrentCustomer == null)
            {
                var page = new Xamarin.Forms.NavigationPage(new UserLoginPage(true))
                {
                    BarTextColor = Color.FromHex("236adb")
                };
                page.On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.FormSheet);
                await Navigation.PushModalAsync(page, true);
            }
            _tapCount = 0;
        }
        protected override void OnAppearing()
        {
            UpdateMenuStack();
            UpdateBottomBar();

            base.OnAppearing();
            //var FromSearchPage = Preferences.ContainsKey(PreferenceFields.FromSearchPage);
            //if (!FromSearchPage)
            //{
            //    Device.StartTimer(TimeSpan.FromSeconds(4), () =>
            //    {
            //        DependencyService.Get<IChangeIconService>().ChangeIcon("store" + Preferences.Get(PreferenceFields.SelectedStoreId, "0"));

            //        return false;
            //    });
            //}

        }
        private void UpdateAllMenu()
        {

            menuCatHelper = new MenuCatHelper();
            var Busid = Preferences.Get(PreferenceFields.SelectedStoreId, 0);
            if (Busid == 0)
            {
                Navigation.PopAsync();
            }
            else
            {
                StaticFields.CurrentStore = new Store()
                {
                    ID = Busid
                };
                SmallImage.Source = ImageSource.FromFile("store" + Busid + ".png");
            }
            if (StaticFields.ChangeToWebView)
            {
                Navigation.PushAsync(new PaymentWebView(StaticFields.ServerURL + "/menu/" + Busid));
            }

            if (MenuCatHelper.MenuCategories == null)
            {

                GetMenuCat();
            }
            RunTimer(Busid);
            _tapCount = 0;
            frameCheckOut.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => basketClicked()
                )
            });
            alergyStack.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => AlergyTapped()
                )
            });
            searchStack.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => searchClicked()
                )
            });
            checkVersion();
        }
        private async Task checkVersion()
        {
            if (StaticFields.CurrentLocalVersion != StaticFields.CurrentServerVerion)
            {
                var res = await DisplayAlert("Update Vaialable", "New update available on app store", "Update now", "Later");
                if (res)
                {
                    Device.OpenUri(new Uri("https://apps.apple.com/us/app/order-now-food-delivery/id1500430187?ls=1"));
                }
            }
        }
        private async Task RunTimer(int restId)
        {
            Device.StartTimer(TimeSpan.FromMinutes(2), () =>
            {

                Task.Factory.StartNew(async () =>
                {
                    StaticFields.CurrentStoreInfo = await storeInfoHelper.GetStoreInfo(restId);


                });
                if (StaticFields.CurrentStoreInfo != null)
                {
                    if (!StaticFields.CurrentStoreInfo.DeliveryAllowed)
                    {

                        StaticFields.Deliverytype = "c";
                    }
                    if (!StaticFields.CurrentStoreInfo.CollectionAllowed)
                    {

                        StaticFields.Deliverytype = "d";
                    }
                }
                // Don't repeat the timer (we will start a new timer when the request is finished)
                return true;
            });

        }
        private void ToolFirstClicked(object sender, EventArgs e)
        {
            OpenUserMenu();
        }
        private async Task OpenUserMenu()
        {
            MessagingCenter.Subscribe<UserLoginPage, int>(this, MessagingFields.UserLoggedInChanged, UpdateStoreInfoFromLoginPage);
            MessagingCenter.Subscribe<UserMainMenuPage, int>(this, MessagingFields.UserLoggedOutChanged, UpdateStoreInfoFromLogOutPage);
            await Navigation.PushAsync(new UserMainMenuPage());
        }

        private void UpdateStoreInfoFromLoginPage(UserLoginPage sender, int i)
        {
            if (StaticFields.CurrentStore != null)
            {
                UpdateStoreInfo(StaticFields.CurrentStore.ID);
            }
            //MessagingCenter.Unsubscribe<UserLoginPage, int>(this, MessagingFields.UserLoggedInChanged);

        }
        private void UpdateStoreInfoFromLogOutPage(UserMainMenuPage sender, int i)
        {
            if (StaticFields.CurrentStore != null)
            {
                UpdateStoreInfo(StaticFields.CurrentStore.ID);
            }

            //MessagingCenter.Unsubscribe<UserMainMenuPage, int>(this, MessagingFields.UserLoggedOutChanged);
        }
        private async Task<string> GetMenuCat()
        {
            //_connection = DependencyService.Get<ISQLiteLdb>().GetConnection();
            //await _connection.CreateTableAsync<SelectedStore>();
            //await _connection.DeleteAllAsync<SelectedStore>();
            //var store = await _connection.Table<SelectedStore>().FirstOrDefaultAsync();
            //if (store != null)
            //{
            //menu = store.Menu;
            //}
            // else
            //{

            //}
            if (StaticFields.CurrentStoreInfo == null)
            {
                await UpdateStoreInfo(StaticFields.CurrentStore.ID);
            }
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
            if (string.IsNullOrEmpty(menu))
            {
                menu = Preferences.Get(PreferenceFields.MainMenu, "");
                if (string.IsNullOrEmpty(menu))
                {
                    menu = await menuCatHelper.GetMenu(StaticFields.CurrentStore.ID);
                    Preferences.Set(PreferenceFields.MainMenu, menu);
                }
                //await _connection.InsertAsync(store);

            }
            if (string.IsNullOrEmpty(menu))
            {

                MenuCatHelper.MenuCategories = Newtonsoft.Json.JsonConvert.DeserializeObject<ObservableCollection<MenuCategory>>(MenuCatHelper.MenuString);
            }
            else
            {
                MenuCatHelper.MenuCategories = Newtonsoft.Json.JsonConvert.DeserializeObject<ObservableCollection<MenuCategory>>(menu);
            }

            UpdateMenuStack();




            stackMenuCat.IsVisible = true;
            stackHeader.IsVisible = true;
            relativeLayout.IsVisible = true;
            Loader.IsVisible = false;
            if ((string.IsNullOrEmpty(StaticFields.CurrentPostCode) || StaticFields.CurrentDeliveryInfo == null || StaticFields.CurrentDeliveryInfo.Message != "ok") && StaticFields.Deliverytype == "d")
            {

                await Navigation.PushPopupAsync(new LoginPopupPage());
                _tapCount = 0;



            }

            return menu;
        }

        private async Task CheckInternet()
        {
        mainStart:
            var current = Connectivity.NetworkAccess;

            if (current != NetworkAccess.Internet)
            {
                var res = await DisplayAlert("Connection Error", "No internet connection", "Retry", "Exit");
                if (res)
                {
                    goto mainStart;
                }
                else
                {
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }


            }
            else
            {
                UpdateAllMenu();
            }
        }
        private async Task UpdateMenuString()
        {
            var MenuUpdatedOn = Preferences.Get(PreferenceFields.MenuUpdateDate, "");
            if (!string.IsNullOrEmpty(MenuUpdatedOn))
            {
                if (MenuUpdatedOn != StaticFields.CurrentStoreInfo.MenuUpdatedOn.ToString())
                {
                    if (menuCatHelper == null)
                    {
                        menuCatHelper = new MenuCatHelper();
                    }


                    var menu = await menuCatHelper.GetMenu(StaticFields.CurrentStore.ID);
                    Preferences.Set(PreferenceFields.MainMenu, menu);
                    Preferences.Set(PreferenceFields.MenuUpdateDate, StaticFields.CurrentStoreInfo.MenuUpdatedOn.ToString());
                    if (string.IsNullOrEmpty(menu))
                    {

                        MenuCatHelper.MenuCategories = Newtonsoft.Json.JsonConvert.DeserializeObject<ObservableCollection<MenuCategory>>(MenuCatHelper.MenuString);
                    }
                    else
                    {
                        MenuCatHelper.MenuCategories = Newtonsoft.Json.JsonConvert.DeserializeObject<ObservableCollection<MenuCategory>>(menu);
                    }


                }

            }
        }
        private void UpdateMenuStack()
        {
            UpdateMenuString();
            if (MenuCatHelper.MenuCategories != null)
            {
                stackMenuCat.Children.Clear();
                if (StaticFields.CurrentCustomer != null)
                {
                    ObservableCollection<OD.Data.MenuItem> _menuItemsOrdered = new ObservableCollection<OD.Data.MenuItem>();
                    ObservableCollection<OD.Data.MenuItem> _menuItems = new ObservableCollection<OD.Data.MenuItem>();
                    foreach (var cat in MenuCatHelper.MenuCategories)
                    {

                        foreach (var menu in cat.MenuItems)
                        {
                            _menuItems.Add(menu);
                        }
                    }
                    foreach (var order in StaticFields.CurrentCustomer.Orders)
                    {
                        foreach (var item in order.OrderItems)
                        {
                            if (item.MenuItemId != 0)
                            {
                                if (_menuItemsOrdered.FirstOrDefault(x => x.Id == item.MenuItemId) == null)
                                {
                                    _menuItemsOrdered.Add(_menuItems.FirstOrDefault(x => x.Id == item.MenuItemId));
                                }
                            }
                            else if (item.MenuItemPropertyId != 0)
                            {
                                var menuItem = _menuItems.FirstOrDefault(x => x.MenuItemProperties.FirstOrDefault(y => y.Id == item.MenuItemPropertyId) != null);
                                if (_menuItemsOrdered.FirstOrDefault(x => x.Id == menuItem.Id) == null)
                                {
                                    _menuItemsOrdered.Add(menuItem);
                                }
                            }
                        }


                    }
                    if (StaticFields.CurrentCustomer.Orders.Count > 0)
                    {
                        if (MenuCatHelper.MenuCategories.First().Name != "Want to repeat?")
                        {
                            MenuCatHelper.MenuCategories.Insert(0, new MenuCategory()
                            {
                                Name = "Want to repeat?",
                                MenuItems = _menuItemsOrdered,
                                SelectedQty = 0,
                                StoreID = StaticFields.CurrentStoreInfo.ID,
                                ID = 0
                            });
                        }
                        else
                        {
                            MenuCatHelper.MenuCategories.First().MenuItems = _menuItemsOrdered;
                        }
                    }
                    else
                    {
                        if (MenuCatHelper.MenuCategories.First().Name == "Want to repeat?")
                        {
                            MenuCatHelper.MenuCategories.RemoveAt(0);
                        }
                    }

                }
                else
                {
                    if (MenuCatHelper.MenuCategories.First().Name == "Want to repeat?")
                    {
                        MenuCatHelper.MenuCategories.RemoveAt(0);
                    }
                }
                foreach (var cat in MenuCatHelper.MenuCategories)
                {
                    if (cat.MenuItems.Count > 0)
                    {

                        var stackHorizon = new StackLayout()
                        {
                            Padding = new Thickness(0, 8),
                            Spacing = 0,
                            Orientation = StackOrientation.Horizontal

                        };
                        var stack = new StackLayout()
                        {
                            Padding = new Thickness(0),
                            Spacing = 2,
                            HorizontalOptions = LayoutOptions.StartAndExpand

                        };

                        stack.Children.Add(new Label()
                        {
                            Text = cat.Name,
                            TextColor = Color.FromHex("282728"),
                            FontSize = 15,
                            HorizontalOptions = LayoutOptions.StartAndExpand,
                            FontFamily = "Nunito-Light",
                            FontAttributes = FontAttributes.Bold
                        });

                        string text = "";
                        foreach (var menuName in cat.MenuItems)
                        {
                            text += menuName.Name + ", ";
                        }
                        if (!string.IsNullOrEmpty(text))
                        {
                            text = text.Substring(0, text.Length - 2);
                        }


                        stack.Children.Add(new Label()
                        {
                            Text = text,
                            HorizontalOptions = LayoutOptions.StartAndExpand,
                            LineBreakMode = LineBreakMode.TailTruncation,
                            FontFamily = "Nunito-ExtraLight",
                            FontSize = 14,
                            TextColor = Color.FromHex("2E3032")

                        });
                        stackHorizon.GestureRecognizers.Add(new TapGestureRecognizer
                        {
                            Command = new Command(() => GoToMenuItemPage(cat.ID)
                        )
                        });
                        stackHorizon.Children.Add(stack);
                        if (cat.SelectedQty > 0)
                        {
                            stackHorizon.Children.Add(new Button()
                            {
                                Text = cat.SelectedQty.ToString(),
                                TextColor = Color.White,
                                HorizontalOptions = LayoutOptions.Center,
                                VerticalOptions = LayoutOptions.Center,
                                BorderWidth = 0,
                                FontSize = 14,
                                BackgroundColor = Color.Green,
                                WidthRequest = 24,
                                HeightRequest = 24,
                                CornerRadius = 12
                            });
                        }
                        stackMenuCat.Children.Add(stackHorizon);
                        stackMenuCat.Children.Add(new BoxView()
                        {
                            Margin = new Thickness(-20, 0)
                        });
                    }
                }
            }
        }

        private async Task UpdateStoreInfo(int restId)
        {

            StaticFields.CurrentStoreInfo = await storeInfoHelper.GetStoreInfo(restId);
            if (!StaticFields.CurrentStoreInfo.DeliveryAllowed)
            {
                StaticFields.Deliverytype = "c";
            }
            lblStoreName.Text = StaticFields.CurrentStoreInfo.BusinessName;
            lblTitle.Text = StaticFields.CurrentStoreInfo.BusinessName;
            lblFoodType.Text = StaticFields.CurrentStoreInfo.FoodType;
            var CustId = Preferences.Get(PreferenceFields.CustomerId, 0);
            if (CustId != 0)
            {
                CustomerPostHelper customerPostHelper = new CustomerPostHelper();
                StaticFields.CurrentCustomer = await customerPostHelper.GetCustomer(CustId);
            }
            stackLoyalityCards1.Children.Clear();
            stackLoyalityCards2.Children.Clear();
            if (StaticFields.CurrentStoreInfo != null)
            {

                if (StaticFields.CurrentStoreInfo.LayaltyEnabled != false)
                {
                    stackLoyalty.IsVisible = true;
                    if (StaticFields.CurrentCustomer == null)
                    {
                        lblLoginToView.Text = "LOGIN TO VIEW YOUR";
                        for (int i = 0; i < 3; i++)
                        {

                            var dis = StaticFields.CurrentStoreInfo.Discounts.FirstOrDefault(x => x.OrderCount == i);
                            if (i == 0)
                            {
                                var FirstDis = StaticFields.CurrentStoreInfo.Discounts.FirstOrDefault(x => x.OrderCount == 99);
                                if (FirstDis != null)
                                {
                                    dis = FirstDis;
                                }
                            }
                            var textDiscount = " ";
                            if (dis != null)
                            {
                                textDiscount = dis.DiscountPercentage.ToString() + "%";
                            }
                            StackLayout stack1 = addStamp(textDiscount);
                            stackLoyalityCards1.Children.Add(stack1);
                        }
                        for (int i = 3; i < 6; i++)
                        {

                            var dis = StaticFields.CurrentStoreInfo.Discounts.FirstOrDefault(x => x.OrderCount == i);
                            if (i == 0)
                            {
                                var FirstDis = StaticFields.CurrentStoreInfo.Discounts.FirstOrDefault(x => x.OrderCount == 99);
                                if (FirstDis != null)
                                {
                                    dis = FirstDis;
                                }
                            }
                            var textDiscount = " ";
                            if (dis != null)
                            {
                                textDiscount = dis.DiscountPercentage.ToString() + "%";
                            }
                            StackLayout stack1 = addStamp(textDiscount);
                            stackLoyalityCards2.Children.Add(stack1);
                        }
                    }
                    else
                    {
                        lblLoginToView.Text = StaticFields.CurrentCustomer.FirstName.ToUpper() + "'s";
                        StaticFields.CurrentPostCode = StaticFields.CurrentCustomer.PostalCode;
                        StaticFields.CurrentDeliveryInfoList = await postCodeHelper.GetDeliveryInfo(StaticFields.CurrentCustomer.PostalCode);
                        if (StaticFields.CurrentDeliveryInfoList != null)
                        {
                            if (StaticFields.CurrentDeliveryInfoList.Count > 1)
                            {
                                StaticFields.CurrentDeliveryInfo = StaticFields.CurrentDeliveryInfoList.FirstOrDefault(x => x.MinimumAmount > StaticFields.CurrentOrder.OrderTotal);
                                if (StaticFields.CurrentDeliveryInfo == null)
                                {
                                    StaticFields.CurrentDeliveryInfo = StaticFields.CurrentDeliveryInfoList.OrderBy(x => x.MinimumAmount).First();
                                }
                            }
                        }
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

                        for (int i = 0; i < 3; i++)
                        {
                            var dis = StaticFields.CurrentStoreInfo.Discounts.FirstOrDefault(x => x.OrderCount == i);
                            if (i == 0)
                            {
                                var FirstDis = StaticFields.CurrentStoreInfo.Discounts.FirstOrDefault(x => x.OrderCount == 99);
                                if (FirstDis != null)
                                {

                                    if (StaticFields.CurrentCustomer.Orders.Count == 0)
                                    {
                                        dis = FirstDis;
                                        StaticFields.CurrentDiscount = dis;
                                    }
                                }
                            }
                            var textDiscount = " ";
                            if (dis != null)
                            {
                                textDiscount = dis.DiscountPercentage.ToString() + "%";
                            }
                            if (ValidOrders < i)
                            {
                                try
                                {
                                    StackLayout stack1 = addStamp(textDiscount);
                                    stackLoyalityCards1.Children.Add(stack1);
                                }
                                catch (Exception ex)
                                {
                                    //
                                }
                            }
                            else
                            {
                                try
                                {
                                    if (i == ValidOrders)
                                    {
                                        StackLayout stack1 = addStamp(textDiscount, true, true);
                                        stackLoyalityCards1.Children.Add(stack1);
                                    }
                                    else
                                    {
                                        StackLayout stack1 = addStamp(textDiscount, true);
                                        stackLoyalityCards1.Children.Add(stack1);
                                    }


                                }
                                catch (Exception ex)
                                {
                                    //
                                }
                            }
                        }
                        for (int i = 3; i < 6; i++)
                        {

                            var dis = StaticFields.CurrentStoreInfo.Discounts.FirstOrDefault(x => x.OrderCount == i);

                            var textDiscount = " ";
                            if (dis != null)
                            {
                                textDiscount = dis.DiscountPercentage.ToString() + "%";
                            }
                            if (ValidOrders >= i)
                            {
                                try
                                {
                                    if (i == ValidOrders)
                                    {
                                        StackLayout stack1 = addStamp(textDiscount, true, true);
                                        stackLoyalityCards2.Children.Add(stack1);
                                    }
                                    else
                                    {
                                        StackLayout stack1 = addStamp(textDiscount, true);
                                        stackLoyalityCards2.Children.Add(stack1);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    //var uu = ex.Message;
                                }
                            }
                            else
                            {
                                try
                                {
                                    StackLayout stack1 = addStamp(textDiscount);
                                    stackLoyalityCards2.Children.Add(stack1);
                                }
                                catch (Exception ex)
                                {
                                    //var uu = ex.Message;
                                }
                            }
                        }
                    }
                }
                else
                {
                    stackLoyalty.IsVisible = false;
                }
                lblAnnoucementAboveMenu.Text = StaticFields.CurrentStoreInfo.AnnoucementAboveMenu;
                if (!string.IsNullOrEmpty(StaticFields.CurrentStoreInfo.AnnoucementAboveMenu))
                {
                    boxviewAnnoucement.IsVisible = true;
                }
                if (!string.IsNullOrEmpty(StaticFields.CurrentStoreInfo.AnnoucementBelowMenu))
                {
                    boxviewAnnoucementBelow.IsVisible = true;
                }
                lblAnnoucementBelowMenu.Text = StaticFields.CurrentStoreInfo.AnnoucementBelowMenu;
                if (Preferences.ContainsKey(PreferenceFields.MenuUpdateDate))
                {
                    var menuUpdatedOn = Preferences.Get(PreferenceFields.MenuUpdateDate, StaticFields.CurrentStoreInfo.CurrentServerTime.AddDays(3).ToString());

                }
                else
                {
                    Preferences.Set(PreferenceFields.MenuUpdateDate, StaticFields.CurrentStoreInfo.MenuUpdatedOn.ToString());
                }
            }


        }

        private static StackLayout addStamp(string textDiscount, bool IsSelected = false, bool onlive = false)
        {
            var stack1 = new StackLayout()
            {
                Padding = new Thickness(0),
                Margin = new Thickness(0),
                WidthRequest = 90,
                Spacing = 0,
                VerticalOptions = LayoutOptions.Center
            };
            var btn = new Button()
            {
                ImageSource = ImageSource.FromFile("starGray.png"),
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 74,
                Padding = new Thickness(10, 5, 10, 10),
                HeightRequest = 74,
                CornerRadius = 37
            };
            var lbl1 = new Label()
            {
                Text = textDiscount,
                Margin = new Thickness(0, -49, 0, 0),
                FontFamily = "Nunito-ExtraBold",
                FontSize = 12,
                WidthRequest = 50,
                TextColor = Color.FromHex("7a7b7c"),
                HorizontalTextAlignment = TextAlignment.Center
            };
            var lbl2 = new Label()
            {
                Text = textDiscount != " " ? "off" : " ",
                Margin = new Thickness(0, -5, 0, 0),
                FontFamily = "Nunito-ExtraBold",
                FontSize = 12,
                TextColor = Color.FromHex("7a7b7c"),
                WidthRequest = 50,
                HorizontalTextAlignment = TextAlignment.Center
            };
            if (IsSelected)
            {
                btn.ImageSource = ImageSource.FromFile("star.png");
                btn.BackgroundColor = Color.FromHex("1099d1");
                lbl1.TextColor = Color.FromHex("ef5850");
                lbl2.TextColor = Color.FromHex("ef5850");
            }
            if (onlive)
            {
                btn.HeightRequest = 86;
                btn.WidthRequest = 86;
                btn.CornerRadius = 43;
                CustomFrame frame = new CustomFrame()
                {
                    HeightRequest = 86,
                    WidthRequest = 86,
                    CornerRadius = 43,
                    Content = btn,
                    Padding = new Thickness(0),
                    Margin = new Thickness(0),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
                stack1.Children.Add(frame);
                lbl1.Margin = new Thickness(0, -53, 0, 0);
                lbl2.Margin = new Thickness(0, -5, 0, 0);

            }
            else
            {

                stack1.Children.Add(btn);
            }

            stack1.Children.Add(lbl1);
            stack1.Children.Add(lbl2);
            return stack1;
        }

        private async Task GoToMenuItemPage(int CatId)
        {
            _tapCount += 1;
            if (_tapCount > 1)
            {
                _tapCount = 0;
                return;
            }
            if (StaticFields.CurrentStoreInfo != null)
            {

                var menuCategory = MenuCatHelper.MenuCategories.FirstOrDefault(x => x.ID == CatId);

                await Navigation.PushAsync(new MenuPropertyListPage(new System.Collections.ObjectModel.ObservableCollection<OD.Data.MenuItem>(menuCategory.MenuItems), menuCategory.Name, menuCategory.Description), true);
                _tapCount = 0;

            }
            else
            {
                var Busid = Preferences.Get(PreferenceFields.SelectedStoreId, 0);
                StaticFields.CurrentStoreInfo = await storeInfoHelper.GetStoreInfo(Busid);
            }
        }
        private async Task AlergyTapped()
        {
            _tapCount += 1;
            if (_tapCount > 1)
            {

                return;
            }
            await Navigation.PushPopupAsync(new TransparentModel(new SideMenu()));

            //await DisplayAlert("Do you have a food allergy?", "If you or someone you are ordering for has a food allergy or intolerance phone the restaurant on "+ StaticFields.CurrentStoreInfo.Phone, "Close");
            _tapCount = 0;
        }
        async Task searchClicked()
        {
            _tapCount += 1;
            if (_tapCount > 1)
            {
                _tapCount = 0;
                return;
            }
            await Navigation.PushAsync(new SearchMenuPage());
            _tapCount = 0;
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
        void UpdateTotalText(MenuPropertyListPage sender, string text)
        {
            UpdateMenuStack();
            UpdateBottomBar();
        }
        void UpdateTotalText(SearchMenuPage sender, string text)
        {
            UpdateMenuStack();
            UpdateBottomBar();
        }

        void UpdateTotalText(OrderHistoryPage sender, string text)
        {
            UpdateMenuStack();
            UpdateBottomBar();
        }
        void UpdateTotalText(OrderDetailPage sender, string text)
        {
            UpdateMenuStack();
            UpdateBottomBar();
        }
        private void OrderCompleted(OrderDetailPage sender, int OrderId)
        {
            Navigation.PopToRootAsync();
            frameCheckOut.IsVisible = false;
            stackMenuCat.IsVisible = false;
            stackHeader.IsVisible = false;
            Loader.IsVisible = true;
            relativeLayout.IsVisible = false;
            UpdateStoreInfo(StaticFields.CurrentStoreInfo.ID);
            UpdateMenuStack();
            UpdateBottomBar();

            stackMenuCat.IsVisible = true;
            stackHeader.IsVisible = true;
            relativeLayout.IsVisible = true;
            Loader.IsVisible = false;




        }
        private void UpdateBottomBar()
        {
            decimal TotalPrice = 0;
            if (MenuCatHelper.MenuCategories != null)
            {

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
                    scrolll.Padding = new Thickness(0);
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
                            btnOrderCount.BorderColor = Color.Black;
                            imageBasket.Source = ImageSource.FromFile("BasketBlack.png");
                            lblBasket.TextColor = Color.Black;
                            lblDeliveryMessage.TextColor = Color.Black;
                        }
                        else
                        {
                            lblDeliveryMessage.Text = "Ready to checkout";
                            frameCheckOut.BackgroundColor = Color.FromHex("236abd");
                            btnOrderCount.TextColor = Color.FromHex("236abd");
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
                    scrolll.Padding = new Thickness(0, 0, 0, 60);
                }
            }
            updateDiscountLessText(TotalPrice);
            var belowannoucementHeight = 0;
            if (!string.IsNullOrEmpty(lblAnnoucementBelowMenu.Text))
            {
                belowannoucementHeight = 20;
            }
            if (frameCheckOut.IsVisible)
            {

                scrolll.Padding = new Thickness(0, 0, 0, -100 + belowannoucementHeight);
            }
            else
            {
                scrolll.Padding = new Thickness(0, 0, 0, -160 + belowannoucementHeight);
            }

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

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {




        }



    }
}
