using System;
using System.Collections.Generic;
using System.Linq;
using ClientAppOD.Helper;
using OD.Data;
using Xamarin.Forms;

namespace ClientAppOD.SubPages
{
    public partial class ExtraListPage : ContentPage
    {
        private MenuItemModel _ExtraModel = null;
        private MenuItemPropertyModel _ExtraProModel = null;
        private decimal _Price = 0;
        private bool IsAExtraProperty;
        public ExtraListPage(MenuItemModel ExtraModel, MenuItemPropertyModel ExtraProModel,string MenuItemName, decimal Price)
        {
            InitializeComponent();
            _ExtraModel = ExtraModel;
            btnAddToOrder.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => btnExtraClicked()
                )
            });
            imageBack.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => btnExtraClicked()
                )
            });
            if (ExtraProModel != null)
            {
                IsAExtraProperty = true;
                if (ExtraProModel.ExtraItem != null)
                {

                    _ExtraProModel = ExtraProModel;
                }
                lblMenuName.Text = MenuItemName;
                lblOptionName.Text = ExtraProModel.DisplayName;
                foreach (var item in _ExtraProModel.ExtraItem.MenuToppings)
                {
                    AddExtraStacks(item);

                }
            }
            if (ExtraModel != null)
            {
                IsAExtraProperty = false;
                if (ExtraModel.ExtraItem != null)
                {

                    _ExtraModel = ExtraModel;
                }
                lblMenuName.Text = MenuItemName;
                lblOptionName.Text = ExtraModel.DisplayName;
                foreach (var item in _ExtraModel.ExtraItem.MenuToppings)
                {
                    AddExtraStacks(item);

                }
            }
            _Price = Price;
            lblPrice.Text = "£" + Price.ToString("F2");
            
        }

        private void AddExtraStacks(MenuTopping item)
        {
            StackLayout stackLayoutUpper = new StackLayout()
            {
                Padding = new Thickness(0),
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.Fill,
                Spacing = 0
            };
            StackLayout stackLayout = new StackLayout()
            {
                Padding = new Thickness(20, 0),
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.Center
            };
            stackLayout.Children.Add(new Label()
            {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                Text = item.Topping
            });
            stackLayout.Children.Add(new Label()
            {
                HorizontalOptions = LayoutOptions.End,
                Margin=new Thickness(0,0,30,0),
                VerticalTextAlignment = TextAlignment.Center,
                Text = "+ £" + ((double)item.ToppingPrice).ToString("F2")
            });
            var but = new ImageButton()
            {
                HorizontalOptions = LayoutOptions.End,
                Source = "plus.png",
                BackgroundColor = Color.Transparent,
                VerticalOptions = LayoutOptions.Center
            };
            but.Clicked += But_Clicked;
            stackLayout.Children.Add(but);
            stackLayout.Children.Add(new Label()
            {
                IsVisible=false,
                Text = item.ID.ToString()
            });
            stackLayoutUpper.Children.Add(stackLayout);
            StackMain.Children.Add(stackLayoutUpper);
            StackMain.Children.Add(new BoxView()
            {
                HeightRequest = 1,
                HorizontalOptions = LayoutOptions.Fill,
                Color = Color.Gray
            });

            if(item.Selected>0)
            {
                var childStacks = stackLayoutUpper.Children;
                childStacks.Add(new BoxView()
                {
                    HeightRequest = 1,
                    HorizontalOptions = LayoutOptions.Fill,
                    Color = Color.Gray
                });

                var extraStack = new StackLayout()
                {
                    Padding = new Thickness(20, 0),
                    Orientation = StackOrientation.Horizontal,
                    VerticalOptions = LayoutOptions.Center,
                    BackgroundColor = Color.FromHex("fcfbfa"),
                    Spacing = 0
                };
                var RemoveExtra = new ImageButton()
                {
                    Source = "minus.png",
                    WidthRequest = 25,
                    Aspect = Aspect.AspectFill,
                    HorizontalOptions = LayoutOptions.Start

                };
                RemoveExtra.Clicked += RemoveExtra_Clicked;
                extraStack.Children.Add(RemoveExtra);
                extraStack.Children.Add(new Image()
                {
                    Source = "check.png",
                    WidthRequest = 30,
                    Margin = new Thickness(30, 0, 0, 0),
                    HorizontalOptions = LayoutOptions.Start
                });
                extraStack.Children.Add(new Label()
                {
                    Text = "£",
                    TextColor = Color.Green,
                    VerticalOptions = LayoutOptions.Center
                });
                var ExtraPrice = (decimal)item.ToppingPrice*item.Selected;
                _Price += ExtraPrice;
                extraStack.Children.Add(new Label()
                {
                    Text = ExtraPrice.ToString("F2"),
                    TextColor = Color.Green,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0, 0, 0)
                });
                extraStack.Children.Add(new Label()
                {
                    Text = item.Selected.ToString(),
                    TextColor = Color.Gray,
                    HorizontalOptions = LayoutOptions.EndAndExpand,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0, 10, 0)
                });
                extraStack.Children.Add(new Label()
                {
                    Text = ((decimal)item.ToppingPrice).ToString("F2"),
                    TextColor = Color.Green,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(-2, 0, 0, 0),
                    IsVisible = false
                });
                extraStack.Children.Add(new Label()
                {
                    Text = item.ID.ToString(),
                    TextColor = Color.Green,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(-2, 0, 0, 0),
                    IsVisible = false
                });
                childStacks.Add(extraStack);
            }
        }

        private void CancelClicked(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, MessagingFields.CancelClickedOnExtra, e);
        }
        private async void btnExtraClicked()
        {
            await Navigation.PopAsync();
        }

        private void But_Clicked(object sender, EventArgs e)
        {
            var stack = ((sender as ImageButton).Parent as StackLayout);
            var id = Convert.ToInt32((stack.Children[3] as Label).Text);
            var selectedExtra = new MenuTopping();
            if(IsAExtraProperty)
            {
                selectedExtra = _ExtraProModel.ExtraItem.MenuToppings.FirstOrDefault(x => x.ID == id);
            }
            else
            {
                selectedExtra = _ExtraModel.ExtraItem.MenuToppings.FirstOrDefault(x => x.ID == id);
            }
            MessagingCenter.Send(this, MessagingFields.ExtraSelected, selectedExtra);
            var stackUpper = stack.Parent as StackLayout;
            var childStacks = stackUpper.Children;
            if (childStacks.Count == 1)
            {
                childStacks.Add(new BoxView()
                {
                    HeightRequest = 1,
                    HorizontalOptions = LayoutOptions.Fill,
                    Color = Color.Gray
                });

                var extraStack = new StackLayout()
                {
                    Padding = new Thickness(20, 0),
                    Orientation = StackOrientation.Horizontal,
                    VerticalOptions = LayoutOptions.Center,
                    BackgroundColor = Color.FromHex("fcfbfa"),
                    Spacing = 0
                };
                var RemoveExtra = new ImageButton()
                {
                    Source = "minus.png",
                    WidthRequest = 25,
                    Aspect = Aspect.AspectFill,
                    HorizontalOptions = LayoutOptions.Start

                };
                RemoveExtra.Clicked += RemoveExtra_Clicked;
                extraStack.Children.Add(RemoveExtra);
                extraStack.Children.Add(new Image()
                {
                    Source = "check.png",
                    WidthRequest = 30,
                    Margin = new Thickness(30, 0, 0, 0),
                    HorizontalOptions = LayoutOptions.Start
                });
                extraStack.Children.Add(new Label()
                {
                    Text = "£",
                    TextColor = Color.Green,
                    VerticalOptions = LayoutOptions.Center
                });
                var ExtraPrice = Convert.ToDecimal((stack.Children[1] as Label).Text.Replace("+ £", ""));
                _Price += ExtraPrice;
                extraStack.Children.Add(new Label()
                {
                    Text = (stack.Children[1] as Label).Text.Replace("+ £", ""),
                    TextColor = Color.Green,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0, 0, 0)
                });
                extraStack.Children.Add(new Label()
                {
                    Text = "1",
                    TextColor = Color.Gray,
                    HorizontalOptions = LayoutOptions.EndAndExpand,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0, 10, 0)
                });
                extraStack.Children.Add(new Label()
                {
                    Text = (stack.Children[1] as Label).Text.Replace("+ £", ""),
                    TextColor = Color.Green,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(-2, 0, 0, 0),
                    IsVisible = false
                });
                extraStack.Children.Add(new Label()
                {
                    Text = selectedExtra.ID.ToString(),
                    TextColor = Color.Green,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(-2, 0, 0, 0),
                    IsVisible = false
                });
                childStacks.Add(extraStack);
            }
            else
            {
                var ExistingExtraStack = childStacks[2] as StackLayout;
                var NumberLable = ExistingExtraStack.Children[4] as Label;
                var NewNumber = Convert.ToInt32(NumberLable.Text) + 1;
                NumberLable.Text = NewNumber.ToString();
                var PriceLabel = ExistingExtraStack.Children[3] as Label;
                var ExtraPrice = Convert.ToDecimal((ExistingExtraStack.Children[5] as Label).Text);
                PriceLabel.Text = (ExtraPrice * NewNumber).ToString("F2");
                _Price += ExtraPrice;
            }
            lblPrice.Text = "£" + _Price.ToString("F2");
        }



        private void RemoveExtra_Clicked(object sender, EventArgs e)
        {
            try
            {
                var stack = (sender as ImageButton).Parent as StackLayout;
                var NumberLable = stack.Children[4] as Label;
                var IdLable = Convert.ToInt32((stack.Children[6] as Label).Text);
                MenuTopping menuTopping = new MenuTopping();
                if (IsAExtraProperty)
                {
                    menuTopping = _ExtraProModel.ExtraItem.MenuToppings.FirstOrDefault(x => x.ID == IdLable);
                }
                else
                {
                    menuTopping = _ExtraModel.ExtraItem.MenuToppings.FirstOrDefault(x => x.ID == IdLable);
                }
                MessagingCenter.Send(this, MessagingFields.ExtraRemoved, menuTopping);
                var ExtraPrice = Convert.ToDecimal((((stack.Parent as StackLayout).Children[0] as StackLayout).Children[1] as Label).Text.Replace("+ £", ""));
                _Price -= ExtraPrice;
                lblPrice.Text = "£" + _Price.ToString("F2");
                var Number = Convert.ToInt32(NumberLable.Text);
                if (Number == 1)
                {
                    var UpperStack = stack.Parent as StackLayout;
                    UpperStack.Children.Remove(UpperStack.Children[1]);
                    UpperStack.Children.Remove(UpperStack.Children[1]);
                }
                else
                {
                    var NewNumber = Number - 1;
                    NumberLable.Text = NewNumber.ToString();
                    var PriceLabel = stack.Children[3] as Label;
                    PriceLabel.Text = (Convert.ToDouble((stack.Children[5] as Label).Text) * NewNumber).ToString("F2");
                }
            }
            catch
            {

            }
        }
        
    }
}
