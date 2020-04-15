using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ClientAppOD.CustomModels;
using ClientAppOD.Helper;
using ClientAppOD.MenuPages;
using ClientAppOD.SubPages;
using ClientAppOD.TrialPages;
using OD.Data;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace ClientAppOD
{
    public partial class SearchMenuPage : ContentPage
    {
        private ObservableCollection<OD.Data.MenuItem> _menuItems = new ObservableCollection<OD.Data.MenuItem>();
        private OrderHelper OrderHelper = new OrderHelper();
        int descMaxLines = 0;
        int _tapCount = 0;
        string searchterm = "";
        public SearchMenuPage()
        {
            InitializeComponent();
            MasterDetailPage masterDetailRootPage = (MasterDetailPage)Xamarin.Forms.Application.Current.MainPage;
            foreach (var cat in MenuCatHelper.MenuCategories)
            {
                foreach(var menu in cat.MenuItems)
                {
                    _menuItems.Add(menu);
                }
            }
            frameCheckOut.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => basketClicked()
                 )
            });
            
            
            UpdateTotal();
           

        }
        protected override void OnAppearing()
        {
            if (!search.IsFocused)
            {
                search.Focus();
            }
            else
            {
                search.Unfocus();
            }
            base.OnAppearing(); 
        }
       
        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            search.Text = string.Empty;
        }
        
        private void SearchTextChanged(object sender, TextChangedEventArgs e)
        {
            if(e.NewTextValue.Length!=0)
            {
                    btnClear.IsVisible = true; 
               
                
            }
            else
            {
                btnClear.IsVisible = false;
            }
            if(e.NewTextValue.Length>2)
            {
                searchterm = e.NewTextValue.ToLower();
                var menu = _menuItems.Where(x => x.Name.ToLower().Contains(searchterm));
                if(menu.Count()==0)
                {
                    stackEmptyMenu.IsVisible = true;
                    ListViewMenuItem.IsVisible = false;
                }
                else
                {
                    stackEmptyMenu.IsVisible = false;
                    ListViewMenuItem.IsVisible = true;
                }
                this.BindingContext = menu;
            }
            else
            {
                this.BindingContext = null;
            }
        }

        void btnBackClicked(Object sender,EventArgs e)
        {
            Navigation.PopAsync();
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
       

        private void UpdateTotal()
        {
            UpdateBottomBar();
            MessagingCenter.Send(this, MessagingFields.AddOrderItem, "aa");
        }
        private void UpdateTotalFromModelPage(OptionsModalPageView sender, int i)
        {
            this.BindingContext = null;
            this.BindingContext = _menuItems.Where(x => x.Name.ToLower().Contains(searchterm));
            UpdateBottomBar();
            MessagingCenter.Unsubscribe<OptionsModalPageView, int>(this, MessagingFields.AddOrderItem);
            MessagingCenter.Send(this, MessagingFields.AddOrderItem, "aa");
            search.Unfocus();
        }
        //async void backClicked(object sender, EventArgs e)
        //{
        //    await Navigation.PopAsync();
        //}
        private void UpdateTotalFromModelEditPage(OptionsModalEditPageView sender, int i)
        {
            this.BindingContext = null;
            this.BindingContext = _menuItems.Where(x => x.Name.ToLower().Contains(searchterm));
            UpdateBottomBar();
            MessagingCenter.Unsubscribe<OptionsModalEditPageView, int>(this, MessagingFields.EditOrderItem);
            MessagingCenter.Send(this, MessagingFields.AddOrderItem, "aa");
            search.Unfocus();
        }
        async void ItemSelectedHandle(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            _tapCount += 1;
            if (_tapCount > 1)
            {
                _tapCount = 0;
                return;
            }
            if (e.Item == null) return;


            if (sender is ListView lv) lv.SelectedItem = null;
            if (e.Item != null)
            {
                MessagingCenter.Subscribe<OptionsModalPageView, int>(this, MessagingFields.AddOrderItem, UpdateTotalFromModelPage);
                var menuItem = e.Item as OD.Data.MenuItem;
                await Navigation.PushPopupAsync(new TransparentModel(new OptionsModalPageView(menuItem, menuItem.Name, menuItem.Description)));
                ListViewMenuItem.SelectedItem = null;

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

        }


        private async void listOrderItemSelected(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            if (e.Item == null) return;

            if (sender is ListView lv) lv.SelectedItem = null;
            if (e.Item != null)
            {
                var orderItem = e.Item as OrderItem;
                var menuItem = _menuItems.FirstOrDefault(x => x.Id == orderItem.MenuItemId);
                MessagingCenter.Subscribe<OptionsModalEditPageView, int>(this, MessagingFields.EditOrderItem, UpdateTotalFromModelEditPage);
                await Navigation.PushPopupAsync(new TransparentModel(new OptionsModalEditPageView(orderItem, menuItem, menuItem.Name, menuItem.Description)));

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
      
    }
}
