using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ClientAppOD.CustomModels;
using ClientAppOD.Helper;
using OD.Data;
using Rg.Plugins.Popup.Services;
using UIKit;
using Xamarin.Forms;

namespace ClientAppOD.MenuPages
{
    public partial class OptionsModalEditPageView : ContentView
    {
        PriceConverter priceConverter = new PriceConverter();
        OD.Data.MenuItem _menuItem = new OD.Data.MenuItem();
        MenuItemProperty SelectedProperty = null;
        private OrderHelper orderHelper = new OrderHelper();
        private decimal Price = 0;
        private static ObservableCollection<OD.Data.MenuItemPropertyModel> ProModels;
        private static ObservableCollection<OD.Data.MenuItemModel> Models;
        private static bool IsAProperty;
        OrderItem _orderItem = new OrderItem();
        decimal? StartedPrice;
        private static decimal basePrice;
        public OptionsModalEditPageView(OrderItem orderItem,OD.Data.MenuItem menuItem, string Name, string Desc, string price = "")
        {
            InitializeComponent();
            Double top = 0;
            Double bottom = 0;
            if (UIApplication.SharedApplication.KeyWindow != null)
            {
                var sa = UIApplication.SharedApplication.KeyWindow.SafeAreaInsets;
                top = sa.Top;
                bottom = sa.Bottom;
            }
            if (bottom == 0)
            {
                bottom = 20;
            }
            frameTop.Padding = new Thickness(0, top, 0, 0);
            FrameAddToOrder.Padding = new Thickness(20, 5, 20, bottom);

            _menuItem = menuItem;
            _orderItem = orderItem;
            StartedPrice = orderItem.Total;
            scroll.Scrolled += Scroll_Scrolled;
            if (string.IsNullOrEmpty(price))
            {
                price = priceConverter.Convert(_menuItem.Id, null, null, null).ToString();
            }
            stackAddToOrder.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => AddToOrderClicked()
               )
            });
            Price = (decimal)_menuItem.Price;
            lblMenuName.Text = Name;
            lblTopMenuName.Text = Name;
            lblMenuPrice.Text = price;
            lblMenuDesc.Text = Desc;
            IsAProperty = false;
            SelectedProperty = null;
            double extraTotalPrice = 0;
            if (_menuItem.MenuItemProperties.Count() > 0)
            {
                IsAProperty = true;
                SelectedProperty = _menuItem.MenuItemProperties.FirstOrDefault(x => x.Id == orderItem.MenuItemPropertyId);
                StackLayout stackLayoutProUpper = AddTopStack();
                foreach (var menuItemPro in _menuItem.MenuItemProperties)
                {

                    var stackLayoutPro = new StackLayout()
                    {
                        Padding = new Thickness(20, 15),
                        Orientation = StackOrientation.Horizontal
                    };
                    stackLayoutPro.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(() => ProStackTapped(stackLayoutPro, menuItemPro.Id)
                        )
                    });
                    stackLayoutPro.Children.Add(new Label()
                    {
                        Text = menuItemPro.Id.ToString(),
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        HorizontalTextAlignment = TextAlignment.Start,
                        IsVisible = false
                    });
                    var dotImage = new Image()
                    {
                        Source = ImageSource.FromFile("GreenDot.png"),
                        HorizontalOptions = LayoutOptions.Start,
                        Margin = new Thickness(-5, 0, 5, 0),
                        IsVisible = false,
                        HeightRequest = 10
                    };
                    if (menuItemPro.Id == SelectedProperty.Id)
                    {
                        dotImage.IsVisible = true;
                        Price += (decimal)menuItemPro.Price;
                    }
                    stackLayoutPro.Children.Add(dotImage);
                    stackLayoutPro.Children.Add(new Label()
                    {
                        Text = menuItemPro.Name,
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        HorizontalTextAlignment = TextAlignment.Start
                    });
                    var PriceText = "";
                    if (price.Contains("from"))
                    {
                        basePrice = Convert.ToDecimal(price.Replace("from", "").Replace(" ", "").Replace("£", ""));
                        var menuPrice = (menuItemPro.Price != null) ? (decimal)menuItemPro.Price : 0;
                        if (menuPrice > basePrice)
                        {
                            PriceText = "+£" + (menuPrice - basePrice).ToString("F2");
                        }
                        else
                        {
                            PriceText = "";
                        }
                    }
                    else
                    {
                        basePrice = Convert.ToDecimal(price.Replace(" ", "").Replace("£", ""));
                    }
                    var text = PriceText;
                    if (menuItemPro.Price == 0)
                    {
                        text = "";
                    }
                    if (menuItemPro.Id == SelectedProperty.Id)
                    {
                        var priceLabel = (stackLayoutProUpper.Children[0] as StackLayout).Children[1] as Label;
                        if (SelectedProperty.Price > basePrice)
                        {
                            priceLabel.IsVisible = true;
                            text = "+£" + Convert.ToDecimal(SelectedProperty.Price - basePrice).ToString("F2");
                        }
                        else
                        {
                            priceLabel.IsVisible = true;
                            priceLabel.Text = " ";
                        }
                        priceLabel.Text = text;
                    }
                        stackLayoutPro.Children.Add(new Label()
                        {
                            Text = text,
                            HorizontalOptions = LayoutOptions.End,
                            HorizontalTextAlignment = TextAlignment.Start
                        });
                    
                        stackLayoutProUpper.Children.Add(stackLayoutPro);
                        stackLayoutProUpper.Children.Add(new BoxView());


                    
                }
                stackOptions.Children.Add(stackLayoutProUpper);

                foreach (var model in SelectedProperty.MenuItemPropertyModels.OrderBy(x => x.DisplayOrder))
                {
                    var stackLayoutUpper = new StackLayout()
                    {
                        Padding = new Thickness(0)
                    };
                    if (model.ItemType == "Option")
                    {

                        StackLayout stackLayout = new StackLayout()
                        {
                            BackgroundColor = Color.FromHex("f8f6f8"),
                            Padding = new Thickness(20, 35, 20, 5),
                            Orientation = StackOrientation.Horizontal
                        };
                        stackLayout.Children.Add(new Label()
                        {
                            Text = "Pick one",
                            HorizontalOptions = LayoutOptions.StartAndExpand,
                            HorizontalTextAlignment = TextAlignment.Start
                        });
                        stackLayout.Children.Add(new Image()
                        {
                            Source = ImageSource.FromFile("check.png"),
                            HeightRequest = 25,
                            IsVisible = false
                        });
                        stackLayout.Children.Add(new Image()
                        {
                            Source = ImageSource.FromFile("info.png"),
                            HeightRequest = 20,
                            IsVisible = false,

                        });
                        stackLayoutUpper.Children.Add(stackLayout);
                        stackLayoutUpper.Children.Add(new BoxView());
                        foreach (var item in model.OptionItem.MenuDishProperties)
                        {
                            item.selected = false;
                            var stackLayoutPro = new StackLayout()
                            {
                                Padding = new Thickness(20, 15),
                                Orientation = StackOrientation.Horizontal
                            };
                            stackLayoutPro.Children.Add(new Image()
                            {
                                Source = ImageSource.FromFile("GreenDot.png"),
                                HorizontalOptions = LayoutOptions.Start,
                                Margin = new Thickness(-5, 0, 5, 0),
                                IsVisible = false,
                                HeightRequest = 10
                            });
                            stackLayoutPro.Children.Add(new Label()
                            {
                                Text = item.DishProperty,
                                HorizontalOptions = LayoutOptions.StartAndExpand,
                                HorizontalTextAlignment = TextAlignment.Start
                            });
                            var text = (item.DishPropertyPrice != null) ? "+£" + ((decimal)item.DishPropertyPrice).ToString("F2") : "";
                            if (item.DishPropertyPrice == 0)
                            {
                                text = "";
                            }
                            stackLayoutPro.Children.Add(new Label()
                            {
                                Text = text,
                                HorizontalOptions = LayoutOptions.End,
                                HorizontalTextAlignment = TextAlignment.Start
                            });
                            stackLayoutPro.GestureRecognizers.Add(new TapGestureRecognizer
                            {
                                Command = new Command(() => OptionTapped(item, stackLayoutPro)
                            )
                            });
                            if (orderItem.OrderModels.Where(x => x.ItemType == "Option").FirstOrDefault(x => x.OptionID == item.ID) != null)
                            {
                                (stackLayoutPro.Children[0] as Image).IsVisible = true;
                                (stackLayoutPro.Children[1] as Label).TextColor = Color.FromHex("006824");
                                (stackLayoutPro.Children[1] as Label).FontFamily = "Nunito-ExtraLight";
                                (stackLayoutPro.Children[1] as Label).FontAttributes = FontAttributes.Bold;
                                item.selected = true;
                                Price += (decimal)item.DishPropertyPrice;
                            }
                            stackLayoutUpper.Children.Add(stackLayoutPro);
                            stackLayoutUpper.Children.Add(new BoxView());
                        }


                    }
                    else
                    {

                        StackLayout stackLayout = new StackLayout()
                        {
                            BackgroundColor = Color.FromHex("f8f6f8"),
                            Padding = new Thickness(20, 15),
                            Orientation = StackOrientation.Horizontal
                        };
                        stackLayout.Children.Add(new Label()
                        {
                            Text = "Choose extras",
                            HorizontalOptions = LayoutOptions.StartAndExpand,
                            HorizontalTextAlignment = TextAlignment.Start
                        });
                        stackLayout.Children.Add(new Label()
                        {
                            Text = "",
                            HorizontalOptions = LayoutOptions.End,
                            HorizontalTextAlignment = TextAlignment.End,
                            IsVisible = false,
                            TextColor = Color.FromHex("006824")
                        });

                        stackLayoutUpper.Children.Add(stackLayout);
                        stackLayoutUpper.Children.Add(new BoxView());
                        extraTotalPrice = 0;
                        foreach (var item in model.ExtraItem.MenuToppings)
                        {
                            item.Selected = 0;
                            var stackLayoutPro = new StackLayout()
                            {
                                Padding = new Thickness(20, 15),
                                Orientation = StackOrientation.Horizontal
                            };
                            
                            stackLayoutPro.Children.Add(new Button()
                            {
                                HorizontalOptions = LayoutOptions.Center,
                                VerticalOptions = LayoutOptions.Center,
                                BorderWidth = 0,
                                WidthRequest = 24,
                                HeightRequest = 24,
                                CornerRadius = 12,
                                TextColor = Color.White,
                                BackgroundColor = Color.FromHex("006824"),
                                FontSize = 14,
                                Margin = new Thickness(0, 0, 10, 0),
                                IsVisible = false
                            });
                            stackLayoutPro.Children.Add(new Label()
                            {
                                Text = item.Topping,
                                HorizontalOptions = LayoutOptions.StartAndExpand,
                                HorizontalTextAlignment = TextAlignment.Start
                            });
                            var text = (item.ToppingPrice != null) ? "+£" + ((decimal)item.ToppingPrice).ToString("F2") : "";
                            if (item.ToppingPrice == 0)
                            {
                                text = "";
                            }
                            stackLayoutPro.Children.Add(new Label()
                            {
                                Text = text,
                                HorizontalOptions = LayoutOptions.End,
                                HorizontalTextAlignment = TextAlignment.Start
                            });
                            var deleteBtn = new ImageButton()
                            {
                                Source = ImageSource.FromFile("delete.png"),
                                HorizontalOptions = LayoutOptions.End,
                                HeightRequest = 20,
                                WidthRequest = 40,
                                Padding = new Thickness(10, 0),
                                IsVisible = false

                            };
                            deleteBtn.Clicked += DeleteBtn_Clicked;
                            stackLayoutPro.Children.Add(deleteBtn);
                            stackLayoutPro.Children.Add(new Label()
                            {
                                Text = item.ID.ToString(),
                                HorizontalOptions = LayoutOptions.End,
                                HorizontalTextAlignment = TextAlignment.Start,
                                IsVisible = false
                            });
                            stackLayoutPro.Children.Add(new Label()
                            {
                                Text = model.ItemId.ToString(),
                                HorizontalOptions = LayoutOptions.End,
                                HorizontalTextAlignment = TextAlignment.Start,
                                IsVisible = false
                            });
                            stackLayoutPro.GestureRecognizers.Add(new TapGestureRecognizer
                            {
                                Command = new Command(() => ExtraTapped(item, stackLayoutPro)
                                    )
                            });
                            var selectedExtra = orderItem.OrderModels.Where(x => x.ItemType == "Extra").FirstOrDefault(x => x.OrderModelExtras.Where(y => y.ExtraId == item.ID).Count() > 0);
                            if (selectedExtra != null)
                            {
                                var selectedExtraModel = selectedExtra.OrderModelExtras.FirstOrDefault(x => x.ExtraId == item.ID);

                                item.Selected = Convert.ToInt32(selectedExtraModel.Qty);
                                var qtyButton = stackLayoutPro.Children[0] as Button;
                                qtyButton.Text = item.Selected.ToString();
                                qtyButton.IsVisible = true;
                                (stackLayoutPro.Children[3] as ImageButton).IsVisible = true;
                                extraTotalPrice += item.Selected * (double)item.ToppingPrice;
                            }
                            stackLayoutUpper.Children.Add(stackLayoutPro);
                            var priceLabel = ((stackLayoutPro.Parent as StackLayout).Children[0] as StackLayout).Children[1] as Label;
                            priceLabel.Text = "+£" + extraTotalPrice.ToString("F2");
                            priceLabel.IsVisible = true;
                            stackLayoutUpper.Children.Add(new BoxView());
                        }
                        Price += (decimal)extraTotalPrice;
                    }
                    stackOptions.Children.Add(stackLayoutUpper);
                }
                ProModels = new ObservableCollection<MenuItemPropertyModel>(SelectedProperty.MenuItemPropertyModels.OrderBy(x => x.DisplayOrder));

            }
            else
            {
                Models = new ObservableCollection<MenuItemModel>(_menuItem.MenuItemModels.OrderBy(x => x.DisplayOrder));
            }
            foreach (var model in _menuItem.MenuItemModels.OrderBy(x => x.DisplayOrder))
            {
                var stackLayoutUpper = new StackLayout()
                {
                    Padding = new Thickness(0)
                };
                if (model.ItemType == "Option")
                {

                    StackLayout stackLayout = new StackLayout()
                    {
                        BackgroundColor = Color.FromHex("f8f6f8"),
                        Padding = new Thickness(20, 35, 20, 5),
                        Orientation = StackOrientation.Horizontal
                    };
                    stackLayout.Children.Add(new Label()
                    {
                        Text = "Pick one",
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        HorizontalTextAlignment = TextAlignment.Start
                    });
                    stackLayout.Children.Add(new Image()
                    {
                        Source = ImageSource.FromFile("check.png"),
                        HeightRequest = 25
                    });
                    stackLayout.Children.Add(new Image()
                    {
                        Source = ImageSource.FromFile("info.png"),
                        HeightRequest = 20,
                        IsVisible = false,

                    });
                    stackLayoutUpper.Children.Add(stackLayout);
                    stackLayoutUpper.Children.Add(new BoxView());
                    foreach (var item in model.OptionItem.MenuDishProperties)
                    {
                        item.selected = false;
                        var stackLayoutPro = new StackLayout()
                        {
                            Padding = new Thickness(20, 15),
                            Orientation = StackOrientation.Horizontal
                        };
                        var dotImage = new Image()
                        {
                            Source = ImageSource.FromFile("GreenDot.png"),
                            HorizontalOptions = LayoutOptions.Start,
                            Margin = new Thickness(-5, 0, 5, 0),
                            IsVisible = false,
                            HeightRequest = 10
                        };
                        
                        stackLayoutPro.Children.Add(dotImage);
                        stackLayoutPro.Children.Add(new Label()
                        {
                            Text = item.DishProperty,
                            HorizontalOptions = LayoutOptions.StartAndExpand,
                            HorizontalTextAlignment = TextAlignment.Start
                        });
                        var text = (item.DishPropertyPrice != null) ? ((decimal)item.DishPropertyPrice).ToString("F2") : "";
                        if (item.DishPropertyPrice == 0)
                        {
                            text = "";
                        }
                        stackLayoutPro.Children.Add(new Label()
                        {
                            Text = text,
                            HorizontalOptions = LayoutOptions.End,
                            HorizontalTextAlignment = TextAlignment.Start
                        });
                        stackLayoutPro.GestureRecognizers.Add(new TapGestureRecognizer
                        {
                            Command = new Command(() => OptionTapped(item, stackLayoutPro)
                        )
                        });
                        if (orderItem.OrderModels.Where(x => x.ItemType == "Option").FirstOrDefault(x => x.OptionID == item.ID) != null)
                        {
                            (stackLayoutPro.Children[0] as Image).IsVisible = true;
                            (stackLayoutPro.Children[1] as Label).TextColor = Color.FromHex("006824");
                            (stackLayoutPro.Children[1] as Label).FontFamily = "Nunito-ExtraLight";
                            (stackLayoutPro.Children[1] as Label).FontAttributes = FontAttributes.Bold;
                            item.selected = true;
                            Price += (decimal)item.DishPropertyPrice;
                        }
                        stackLayoutUpper.Children.Add(stackLayoutPro);
                        stackLayoutUpper.Children.Add(new BoxView());
                    }

                }
                else
                {

                    StackLayout stackLayout = new StackLayout()
                    {
                        BackgroundColor = Color.FromHex("f8f6f8"),
                        Padding = new Thickness(20, 15),
                        Orientation = StackOrientation.Horizontal
                    };
                    stackLayout.Children.Add(new Label()
                    {
                        Text = "Choose extras",
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        HorizontalTextAlignment = TextAlignment.Start
                    });
                    stackLayout.Children.Add(new Label()
                    {
                        Text = "",
                        HorizontalOptions = LayoutOptions.End,
                        HorizontalTextAlignment = TextAlignment.End,
                        IsVisible = false,
                        TextColor = Color.FromHex("006824")
                    });
                    stackLayoutUpper.Children.Add(stackLayout);
                    stackLayoutUpper.Children.Add(new BoxView());
                    extraTotalPrice = 0;
                    foreach (var item in model.ExtraItem.MenuToppings)
                    {
                        item.Selected = 0;
                        var stackLayoutPro = new StackLayout()
                        {
                            Padding = new Thickness(20, 15),
                            Orientation = StackOrientation.Horizontal
                        };
                        stackLayoutPro.Children.Add(new Button()
                        {
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center,
                            BorderWidth = 0,
                            WidthRequest = 24,
                            HeightRequest = 24,
                            CornerRadius = 12,
                            TextColor = Color.White,
                            BackgroundColor = Color.FromHex("006824"),
                            FontSize = 14,
                            Margin = new Thickness(0, 0, 10, 0),
                            IsVisible = false
                        });
                        stackLayoutPro.Children.Add(new Label()
                        {
                            Text = item.Topping,
                            HorizontalOptions = LayoutOptions.StartAndExpand,
                            HorizontalTextAlignment = TextAlignment.Start
                        });
                        var text = (item.ToppingPrice != null) ? ("+£" + ((decimal)item.ToppingPrice).ToString("F2")) : "";
                        if (item.ToppingPrice == 0)
                        {
                            text = "";
                        }
                        stackLayoutPro.Children.Add(new Label()
                        {
                            Text = text,
                            HorizontalOptions = LayoutOptions.End,
                            HorizontalTextAlignment = TextAlignment.Start
                        });
                        var deleteBtn = new ImageButton()
                        {
                            Source = ImageSource.FromFile("delete.png"),
                            HorizontalOptions = LayoutOptions.End,
                            HeightRequest = 20,
                            WidthRequest = 40,
                            Padding = new Thickness(10, 0),
                            IsVisible = false

                        };
                        deleteBtn.Clicked += DeleteBtn_Clicked;
                        stackLayoutPro.Children.Add(deleteBtn);
                        stackLayoutPro.Children.Add(new Label()
                        {
                            Text = item.ID.ToString(),
                            HorizontalOptions = LayoutOptions.End,
                            HorizontalTextAlignment = TextAlignment.Start,
                            IsVisible = false
                        });
                        stackLayoutPro.Children.Add(new Label()
                        {
                            Text = model.ItemId.ToString(),
                            HorizontalOptions = LayoutOptions.End,
                            HorizontalTextAlignment = TextAlignment.Start,
                            IsVisible = false
                        });
                        stackLayoutPro.GestureRecognizers.Add(new TapGestureRecognizer
                        {
                            Command = new Command(() => ExtraTapped(item, stackLayoutPro)
                        )
                        });
                        var selectedExtra = orderItem.OrderModels.Where(x => x.ItemType == "Extra").FirstOrDefault(x => x.OrderModelExtras.Where(y => y.ExtraId == item.ID).Count() > 0);
                        if (selectedExtra != null)
                        {
                            var selectedExtraModel = selectedExtra.OrderModelExtras.FirstOrDefault(x => x.ExtraId == item.ID);
                            
                            item.Selected =Convert.ToInt32(selectedExtraModel.Qty);
                            var qtyButton = stackLayoutPro.Children[0] as Button;
                            qtyButton.Text = item.Selected.ToString();
                            qtyButton.IsVisible = true;
                            (stackLayoutPro.Children[3] as ImageButton).IsVisible = true;
                            extraTotalPrice += item.Selected * (double)item.ToppingPrice;
                        }
                        
                        stackLayoutUpper.Children.Add(stackLayoutPro);
                        var priceLabel = ((stackLayoutPro.Parent as StackLayout).Children[0] as StackLayout).Children[1] as Label;
                        priceLabel.Text = "+£" + extraTotalPrice.ToString("F2");
                        priceLabel.IsVisible = true;
                        stackLayoutUpper.Children.Add(new BoxView());
                    }
                    Price += (decimal)extraTotalPrice;
                }
                stackOptions.Children.Add(stackLayoutUpper);
            }
            (stackQuantity.Children[1] as Label).Text = orderItem.Qta.ToString();
            if(orderItem.Qta>1)
            {
                btnRemoveQuantity.IsEnabled = true;
            }
            UpdateBottomBar(false);
        }

        private static StackLayout AddTopStack()
        {
            StackLayout stackLayoutUpper = new StackLayout()
            {
                Padding = new Thickness(0)
            };
            StackLayout stackLayout = new StackLayout()
            {
                BackgroundColor = Color.FromHex("f8f6f8"),
                Padding = new Thickness(20, 35, 20, 5),
                Orientation = StackOrientation.Horizontal

            };

            stackLayout.Children.Add(new Label()
            {
                Text = "Choose one",
                HorizontalOptions = LayoutOptions.StartAndExpand,
                HorizontalTextAlignment = TextAlignment.Start
            });
            stackLayout.Children.Add(new Label()
            {
                Text = "",
                Margin = new Thickness(0, 0, 15, 0),
                HorizontalOptions = LayoutOptions.End,
                HorizontalTextAlignment = TextAlignment.End,
                IsVisible = false,
                TextColor = Color.FromHex("006824")
            });
            
            stackLayout.Children.Add(new Image()
            {
                Source = ImageSource.FromFile("check.png"),
                HeightRequest = 25
            });
            stackLayout.Children.Add(new Image()
            {
                Source = ImageSource.FromFile("info.png"),
                HeightRequest = 20,
                IsVisible = false,

            });
            stackLayoutUpper.Children.Add(stackLayout);
            stackLayoutUpper.Children.Add(new BoxView());
            return stackLayoutUpper;
        }

        private async void Scroll_Scrolled(object sender, ScrolledEventArgs e)
        {
            if (scroll.ScrollY > 190)
            {
                frameTop.IsVisible = true;
            }
            else
            {
                frameTop.IsVisible = false;
            }
            var height = stackQuantity.Height - 10;
            if (stackQuantity.IsVisible == false)
            {
                height = 100;
            }
            if ((scroll.Height + scroll.ScrollY) > (scroll.Content as StackLayout).Height + height)
            {
                await scroll.ScrollToAsync(0, (scroll.Content as StackLayout).Height + height - scroll.Height, false);
            }
        }

        async void btnCollectionClicked(object sender, EventArgs e)
        {
            await PopupNavigation.PopAsync();
        }
        async Task ProStackTapped(StackLayout stack, int Id)
        {
            for (int i = 1; 2 < stackOptions.Children.Count; i++)
            {
                stackOptions.Children.RemoveAt(2);
            }
            foreach (var model in _menuItem.MenuItemProperties.First(x => x.Id == Id).MenuItemPropertyModels.OrderBy(x => x.DisplayOrder))
            {
                var stackLayoutUpper = new StackLayout()
                {
                    Padding = new Thickness(0)
                };
                if (model.ItemType == "Option")
                {

                    StackLayout stackLayout = new StackLayout()
                    {
                        BackgroundColor = Color.FromHex("f8f6f8"),
                        Padding = new Thickness(20, 35, 20, 5),
                        Orientation = StackOrientation.Horizontal
                    };
                    stackLayout.Children.Add(new Label()
                    {
                        Text = "Pick one",
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        HorizontalTextAlignment = TextAlignment.Start
                    });
                    stackLayout.Children.Add(new Image()
                    {
                        Source = ImageSource.FromFile("check.png"),
                        HeightRequest = 25,
                        IsVisible = false
                    });
                    stackLayout.Children.Add(new Image()
                    {
                        Source = ImageSource.FromFile("info.png"),
                        HeightRequest = 20,
                        IsVisible = false,

                    });
                    stackLayoutUpper.Children.Add(stackLayout);
                    stackLayoutUpper.Children.Add(new BoxView());
                    foreach (var item in model.OptionItem.MenuDishProperties)
                    {
                        item.selected = false;
                        var stackLayoutPro = new StackLayout()
                        {
                            Padding = new Thickness(20, 15),
                            Orientation = StackOrientation.Horizontal
                        };
                        stackLayoutPro.Children.Add(new Image()
                        {
                            Source = ImageSource.FromFile("GreenDot.png"),
                            HorizontalOptions = LayoutOptions.Start,
                            Margin = new Thickness(-5, 0, 5, 0),
                            IsVisible = false,
                            HeightRequest = 10
                        });
                        stackLayoutPro.Children.Add(new Label()
                        {
                            Text = item.DishProperty,
                            HorizontalOptions = LayoutOptions.StartAndExpand,
                            HorizontalTextAlignment = TextAlignment.Start
                        });
                        var text = (item.DishPropertyPrice != null) ? "+£" + ((decimal)item.DishPropertyPrice).ToString("F2") : "";
                        if (item.DishPropertyPrice == 0)
                        {
                            text = "";
                        }
                        stackLayoutPro.Children.Add(new Label()
                        {
                            Text = text,
                            HorizontalOptions = LayoutOptions.End,
                            HorizontalTextAlignment = TextAlignment.Start
                        });
                        stackLayoutPro.GestureRecognizers.Add(new TapGestureRecognizer
                        {
                            Command = new Command(() => OptionTapped(item, stackLayoutPro)
                        )
                        });
                        stackLayoutUpper.Children.Add(stackLayoutPro);
                        stackLayoutUpper.Children.Add(new BoxView());
                    }


                }
                else
                {

                    StackLayout stackLayout = new StackLayout()
                    {
                        BackgroundColor = Color.FromHex("f8f6f8"),
                        Padding = new Thickness(20, 15),
                        Orientation = StackOrientation.Horizontal
                    };
                    stackLayout.Children.Add(new Label()
                    {
                        Text = "Choose extras",
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        HorizontalTextAlignment = TextAlignment.Start
                    });
                    stackLayout.Children.Add(new Label()
                    {
                        Text = "",
                        HorizontalOptions = LayoutOptions.End,
                        HorizontalTextAlignment = TextAlignment.End,
                        IsVisible = false,
                        TextColor = Color.FromHex("006824")
                    });
                    stackLayoutUpper.Children.Add(stackLayout);
                    stackLayoutUpper.Children.Add(new BoxView());
                    foreach (var item in model.ExtraItem.MenuToppings)
                    {
                        item.Selected = 0;
                        var stackLayoutPro = new StackLayout()
                        {
                            Padding = new Thickness(20, 15),
                            Orientation = StackOrientation.Horizontal
                        };
                        stackLayoutPro.Children.Add(new Button()
                        {
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center,
                            BorderWidth = 0,
                            WidthRequest = 24,
                            HeightRequest = 24,
                            CornerRadius = 12,
                            TextColor = Color.White,
                            BackgroundColor = Color.FromHex("006824"),
                            FontSize = 14,
                            Margin = new Thickness(0, 0, 10, 0),
                            IsVisible = false
                        });
                        stackLayoutPro.Children.Add(new Label()
                        {
                            Text = item.Topping,
                            HorizontalOptions = LayoutOptions.StartAndExpand,
                            HorizontalTextAlignment = TextAlignment.Start
                        });
                        var text = (item.ToppingPrice != null) ? "+£" + ((decimal)item.ToppingPrice).ToString("F2") : "";
                        if (item.ToppingPrice == 0)
                        {
                            text = "";
                        }
                        stackLayoutPro.Children.Add(new Label()
                        {
                            Text = text,
                            HorizontalOptions = LayoutOptions.End,
                            HorizontalTextAlignment = TextAlignment.Start
                        });
                        var deleteBtn = new ImageButton()
                        {
                            Source = ImageSource.FromFile("delete.png"),
                            HorizontalOptions = LayoutOptions.End,
                            HeightRequest = 20,
                            WidthRequest = 40,
                            Padding = new Thickness(10, 0),
                            IsVisible = false
                        };
                        deleteBtn.Clicked += DeleteBtn_Clicked;
                        stackLayoutPro.Children.Add(deleteBtn);
                        stackLayoutPro.Children.Add(new Label()
                        {
                            Text = item.ID.ToString(),
                            HorizontalOptions = LayoutOptions.End,
                            HorizontalTextAlignment = TextAlignment.Start,
                            IsVisible = false
                        });
                        stackLayoutPro.Children.Add(new Label()
                        {
                            Text = model.ItemId.ToString(),
                            HorizontalOptions = LayoutOptions.End,
                            HorizontalTextAlignment = TextAlignment.Start,
                            IsVisible = false
                        });
                        stackLayoutPro.GestureRecognizers.Add(new TapGestureRecognizer
                        {
                            Command = new Command(() => ExtraTapped(item, stackLayoutPro)
                                    )
                        });
                        stackLayoutUpper.Children.Add(stackLayoutPro);
                        stackLayoutUpper.Children.Add(new BoxView());
                    }
                }
                stackOptions.Children.Add(stackLayoutUpper);
            }
            var check = ((stack.Parent as StackLayout).Children[0] as StackLayout).Children[2] as Image;
            (((stack.Parent as StackLayout).Children[0] as StackLayout).Children[3] as Image).IsVisible = false;
            (((stack.Parent as StackLayout).Children[0] as StackLayout).Children[0] as Label).TextColor = Color.FromHex("2E3032");
            check.IsVisible = true;
            var priceLabel = ((stack.Parent as StackLayout).Children[0] as StackLayout).Children[1] as Label;

            SelectedProperty = _menuItem.MenuItemProperties.FirstOrDefault(x => x.Id == Id);
            if (SelectedProperty.Price > basePrice)
            {
                priceLabel.IsVisible = true;
                priceLabel.Text = "+£" + Convert.ToDecimal(SelectedProperty.Price - basePrice).ToString("F2");
            }
            else
            {
                priceLabel.IsVisible = true;
                priceLabel.Text = " ";
            }
            foreach (var innerStack in (stack.Parent as StackLayout).Children)
            {
                if (innerStack.GetType() == typeof(StackLayout))
                {
                    var childs = (innerStack as StackLayout).Children;
                    if (childs.Count > 3)
                    {
                        if (childs[3].GetType() == typeof(Label))
                        {
                            (childs[1] as Image).IsVisible = false;
                            (childs[2] as Label).TextColor = Color.FromHex("494749");
                            (childs[2] as Label).FontFamily = "Nunito-ExtraLight";
                            (childs[3] as Label).FontFamily = "Nunito-ExtraLight";
                        }
                    }
                }
            }
            (stack.Children[1] as Image).IsVisible = true;
            (stack.Children[2] as Label).TextColor = Color.FromHex("006824");
            (stack.Children[2] as Label).FontFamily = "Nunito-ExtraLight";
            (stack.Children[2] as Label).FontAttributes = FontAttributes.Bold;
            SelectedProperty = _menuItem.MenuItemProperties.FirstOrDefault(x => x.Id == Id);
            ProModels = new ObservableCollection<MenuItemPropertyModel>(SelectedProperty.MenuItemPropertyModels.OrderBy(x => x.DisplayOrder));
            Price = (decimal)_menuItem.Price + (decimal)SelectedProperty.Price;

            UpdateBottomBar();
            if (SelectedProperty == null)
            {
                await scroll.ScrollToAsync(0, stack.Y + (stack.Parent as StackLayout).Height, true);
            }
        }

        private void DeleteBtn_Clicked(object sender, EventArgs e)
        {
            var stack = (sender as ImageButton).Parent as StackLayout;
            var ItemId = Convert.ToInt32((stack.Children[4] as Label).Text);
            var ModelId = Convert.ToInt32((stack.Children[5] as Label).Text);
            double extraTotalPrice = 0;
            if (IsAProperty)
            {
                var item = ProModels.FirstOrDefault(x => x.ItemId == ModelId && x.ItemType == "Extra");
                var extra = item.ExtraItem.MenuToppings.FirstOrDefault(x => x.ID == ItemId);
                extra.Selected -= 1;
                var selectedExtra = item.ExtraItem.MenuToppings.Where(x => x.Selected > 0);
                if (selectedExtra.Count() > 0)
                {
                    string display = "Extra :";
                    foreach (var selected in selectedExtra)
                    {
                        display += selected.Topping + " x " + selected.Selected + ",";
                        extraTotalPrice += (double)selected.ToppingPrice * selected.Selected;
                    }
                    item.DisplayName = display.Substring(0, display.Length - 2);
                }
                else
                {
                    item.DisplayName = "Select Extras";
                }
                Price -= (decimal)extra.ToppingPrice;
                if (extra.Selected > 0)
                {
                    var qtyButton = stack.Children[0] as Button;
                    qtyButton.Text = extra.Selected.ToString();
                    qtyButton.IsVisible = true;
                    (stack.Children[3] as ImageButton).IsVisible = true;
                }
                else
                {
                    var qtyButton = stack.Children[0] as Button;
                    qtyButton.Text = "";
                    qtyButton.IsVisible = false;
                    (stack.Children[3] as ImageButton).IsVisible = false;
                }
            }
            else
            {
                var item = Models.FirstOrDefault(x => x.ItemId == ModelId && x.ItemType == "Extra");
                var extra = item.ExtraItem.MenuToppings.FirstOrDefault(x => x.ID == ItemId);
                extra.Selected -= 1;
                var selectedExtra = item.ExtraItem.MenuToppings.Where(x => x.Selected > 0);
                if (selectedExtra.Count() > 0)
                {
                    string display = "";
                    foreach (var selected in selectedExtra)
                    {
                        display += selected.Topping + " x " + selected.Selected + " ,";
                        extraTotalPrice += (double)selected.ToppingPrice * selected.Selected;
                    }
                    item.DisplayName = display.Substring(0, display.Length - 2);
                }
                else
                {
                    item.DisplayName = "Select Extras";
                }
                Price -= (decimal)extra.ToppingPrice;
                if (extra.Selected > 0)
                {
                    var qtyButton = stack.Children[0] as Button;
                    qtyButton.Text = extra.Selected.ToString();
                    qtyButton.IsVisible = true;
                    (stack.Children[3] as ImageButton).IsVisible = true;
                }
                else
                {
                    var qtyButton = stack.Children[0] as Button;
                    qtyButton.Text = "";
                    qtyButton.IsVisible = false;
                    (stack.Children[3] as ImageButton).IsVisible = false;
                }
            }
            var priceLabel = ((stack.Parent as StackLayout).Children[0] as StackLayout).Children[1] as Label;
            if (extraTotalPrice > 0)
            {
                priceLabel.Text = "+£" + extraTotalPrice.ToString("F2");
                priceLabel.IsVisible = true;
            }
            else
            {
                priceLabel.Text = "";
                priceLabel.IsVisible = false;
            }

            UpdateBottomBar();
        }
        async Task OptionTapped(MenuDishProperty e, StackLayout stack)
        {

            if (IsAProperty)
            {
                var item = ProModels.FirstOrDefault(x => x.ItemId == e.MenuDishPropertiesGroupID && x.ItemType == "Option");
                item.DisplayName = e.DishProperty;
                foreach (var EachOption in item.OptionItem.MenuDishProperties)
                {
                    EachOption.selected = false;
                }
                var option = item.OptionItem.MenuDishProperties.FirstOrDefault(x => x.ID == e.ID);
                option.selected = true;

            }
            else
            {
                var item = Models.FirstOrDefault(x => x.ItemId == e.MenuDishPropertiesGroupID && x.ItemType == "Option");
                foreach (var EachOption in item.OptionItem.MenuDishProperties)
                {
                    EachOption.selected = false;
                }
                var option = item.OptionItem.MenuDishProperties.FirstOrDefault(x => x.ID == e.ID);
                option.selected = true;
                item.DisplayName = e.DishProperty;
            }
            foreach (var innerStack in (stack.Parent as StackLayout).Children)
            {
                if (innerStack.GetType() == typeof(StackLayout))
                {
                    var childs = (innerStack as StackLayout).Children;
                    if (childs.Count() > 2)
                    {
                        if (childs[1].GetType() == typeof(Label))
                        {
                            (childs[0] as Image).IsVisible = false;
                            (childs[1] as Label).TextColor = Color.FromHex("494749");
                            (childs[1] as Label).FontFamily = "Nunito-ExtraLight";
                        }
                    }
                }
            }
            var check = ((stack.Parent as StackLayout).Children[0] as StackLayout).Children[1] as Image;
            (((stack.Parent as StackLayout).Children[0] as StackLayout).Children[2] as Image).IsVisible = false;
            (((stack.Parent as StackLayout).Children[0] as StackLayout).Children[0] as Label).TextColor = Color.FromHex("2E3032");
            check.IsVisible = true;
            (stack.Children[0] as Image).IsVisible = true;
            (stack.Children[1] as Label).TextColor = Color.FromHex("006824");
            (stack.Children[1] as Label).FontFamily = "Nunito-ExtraLight";
            (stack.Children[1] as Label).FontAttributes = FontAttributes.Bold;

            Price += (decimal)e.DishPropertyPrice;

            UpdateBottomBar();
            await scroll.ScrollToAsync(0, (stack.Parent as StackLayout).Y + (stack.Parent as StackLayout).Height, true);

        }

        void ExtraTapped(MenuTopping e, StackLayout stack)
        {
            double extraTotalPrice = 0;
            if (IsAProperty)
            {
                var item = ProModels.FirstOrDefault(x => x.ItemId == e.MenuToppingsGroupID && x.ItemType == "Extra");
                var extra = item.ExtraItem.MenuToppings.FirstOrDefault(x => x.ID == e.ID);
                extra.Selected += 1;
                var selectedExtra = item.ExtraItem.MenuToppings.Where(x => x.Selected > 0);

                string display = "";
                foreach (var selected in selectedExtra)
                {
                    display += selected.Topping + " x " + selected.Selected + ",";
                    extraTotalPrice += (double)selected.ToppingPrice * selected.Selected;
                }
                item.DisplayName = display.Substring(0, display.Length - 1);

                Price += (decimal)e.ToppingPrice;
                if (extra.Selected > 0)
                {
                    var qtyButton = stack.Children[0] as Button;
                    qtyButton.Text = extra.Selected.ToString();
                    qtyButton.IsVisible = true;
                    (stack.Children[3] as ImageButton).IsVisible = true;
                }
            }
            else
            {
                var item = Models.FirstOrDefault(x => x.ItemId == e.MenuToppingsGroupID && x.ItemType == "Extra");
                var extra = item.ExtraItem.MenuToppings.FirstOrDefault(x => x.ID == e.ID);
                extra.Selected += 1;
                var selectedExtra = item.ExtraItem.MenuToppings.Where(x => x.Selected > 0);

                if (selectedExtra.Count() > 0)
                {
                    string display = "Extras :";
                    foreach (var selected in selectedExtra)
                    {
                        extraTotalPrice += (double)selected.ToppingPrice * selected.Selected;
                        display += selected.Topping + " x " + selected.Selected + ",";
                    }
                    item.DisplayName = display.Substring(0, display.Length - 1);

                }
                else
                {
                    item.DisplayName = "Select Extras";
                }
                Price += (decimal)e.ToppingPrice;
                if (extra.Selected > 0)
                {
                    var qtyButton = stack.Children[0] as Button;
                    qtyButton.Text = extra.Selected.ToString();
                    qtyButton.IsVisible = true;
                    (stack.Children[3] as ImageButton).IsVisible = true;
                }
            }
            var priceLabel = ((stack.Parent as StackLayout).Children[0] as StackLayout).Children[1] as Label;
            priceLabel.Text = "+£" + extraTotalPrice.ToString("F2");
            priceLabel.IsVisible = true;



            UpdateBottomBar();
        }

        void UpdateBottomBar(bool missedVisible = true)
        {
            bool AllSelected = true;
            var TotalNumer = Convert.ToInt32((stackQuantity.Children[1] as Label).Text);
            lblPrice.Text = "£" + (Price * TotalNumer).ToString("F2");

            if (IsAProperty)
            {
                if (SelectedProperty != null)
                {
                    foreach (var item in SelectedProperty.MenuItemPropertyModels.Where(x => x.ItemType == "Option"))
                    {
                        if (item.OptionItem.MenuDishProperties.Where(x => x.selected == true).Count() == 0)
                        {
                            AllSelected = false;
                        }
                    }
                }
                else
                {
                    AllSelected = false;
                }

            }
            else
            {
                foreach (var item in _menuItem.MenuItemModels.Where(x => x.ItemType == "Option"))
                {
                    if (item.OptionItem.MenuDishProperties.Where(x => x.selected == true).Count() == 0)
                    {
                        AllSelected = false;
                    }
                }
            }

            if (AllSelected)
            {
                stackMissedItems.IsVisible = false;
                stackQuantity.IsVisible = true;
                stackAddToOrder.BackgroundColor = Color.FromHex("236abd");
                btnAddToOrder.TextColor = Color.White;
                lblPrice.TextColor = Color.White;
            }
            else
            {
                stackAddToOrder.BackgroundColor = Color.FromHex("f1edef");
                stackQuantity.IsVisible = false;
                btnAddToOrder.TextColor = Color.FromHex("B0B2B2");
                lblPrice.TextColor = Color.FromHex("B0B2B2");
                stackMissedItems.IsVisible = missedVisible;
            }
           if((Price * TotalNumer)!=StartedPrice)
            {
                btnAddToOrder.Text = "Update";
            }
           else
            {
                btnAddToOrder.Text = "Ok";
            }
        }

        private void OrderQtyRemoved(object sender, EventArgs e)
        {
            var label = ((sender as ImageButton).Parent as StackLayout).Children[1] as Label;
            var CurrentQty = Convert.ToInt32(label.Text);
            CurrentQty -= 1;
            label.Text = CurrentQty.ToString();
            if (CurrentQty == 1)
            {
                (sender as ImageButton).IsEnabled = false;
            }
            else
            {
                (sender as ImageButton).IsEnabled = true;
            }
            UpdateBottomBar();
        }
        private void OrderQtyAdded(object sender, EventArgs e)
        {
            var label = ((sender as ImageButton).Parent as StackLayout).Children[1] as Label;
            var CurrentQty = Convert.ToInt32(label.Text);
            CurrentQty += 1;
            label.Text = CurrentQty.ToString();
            if (CurrentQty > 1)
            {
                (((sender as ImageButton).Parent as StackLayout).Children[0] as ImageButton).IsEnabled = true;
            }
            else
            {
                (((sender as ImageButton).Parent as StackLayout).Children[0] as ImageButton).IsEnabled = false;
            }
            UpdateBottomBar();
        }
        private bool IfAllSelected()
        {
            bool AllSelected = true;
            if (IsAProperty)
            {
                if (SelectedProperty != null)
                {
                    foreach (var item in SelectedProperty.MenuItemPropertyModels.Where(x => x.ItemType == "Option"))
                    {
                        if (item.OptionItem.MenuDishProperties.Where(x => x.selected == true).Count() == 0)
                        {
                            AllSelected = false;
                        }
                    }
                }
                else
                {
                    AllSelected = false;
                }

            }
            else
            {
                foreach (var item in _menuItem.MenuItemModels.Where(x => x.ItemType == "Option"))
                {
                    if (item.OptionItem.MenuDishProperties.Where(x => x.selected == true).Count() == 0)
                    {
                        AllSelected = false;
                    }
                }
            }

            return AllSelected;
        }
        private async Task AddToOrderClicked()
        {
            if (!IfAllSelected())
            {
                stackMissedItems.IsVisible = true;
                await scroll.ScrollToAsync(stackMissedItems, ScrollToPosition.Start, true);
                return;
            }
            var orderItemId = orderHelper.GetNewOrderItemId(_menuItem);
            var label = stackQuantity.Children[1] as Label;
            var CurrentQty = Convert.ToInt32(label.Text);
            if (IsAProperty)
            {
                OrderItem orderItem = new OrderItem()
                {
                    ID = _orderItem.ID,
                    ItemType = "MenuItemProperty",
                    MenuItemId = _menuItem.Id,
                    MenuItemPropertyId = SelectedProperty.Id,
                    Name = _menuItem.Name + " " + SelectedProperty.Name,
                    OrderID = 0,
                    Price = _menuItem.Price + SelectedProperty.Price,
                    Total = (_menuItem.Price + SelectedProperty.Price)*CurrentQty,
                    Qta = CurrentQty,
                    OrderText = _menuItem.Name + " " + SelectedProperty.Name + ", "
                };
                if (SelectedProperty.MenuItemPropertyModels.Count() > 0)
                {
                    foreach (var tt in SelectedProperty.MenuItemPropertyModels)
                    {
                        if (tt.ItemType == "Option")
                        {
                            var selectedOption = tt.OptionItem.MenuDishProperties.FirstOrDefault(x => x.selected);
                            var OrderModel = new OrderModel()
                            {
                                Id = 0,
                                DisplayOrder = tt.DisplayOrder,
                                ItemName = tt.DisplayName,
                                ItemType = "Option",
                                OptionID = selectedOption.ID,
                                OrderItemID = 0,
                                Price = (decimal)selectedOption.DishPropertyPrice * orderItem.Qta,
                                Qty = "1"

                            };
                            orderItem.Total += OrderModel.Price;
                            orderItem.OrderText += OrderModel.ItemName + ", ";
                            orderItem.OrderModels.Add(OrderModel);
                        }
                        else
                        {
                            var OrderModel = new OrderModel()
                            {
                                Id = 0,
                                DisplayOrder = tt.DisplayOrder,
                                ItemName = tt.DisplayName,
                                ItemType = "Extra",
                                OptionID = null,
                                OrderItemID = 0,
                                Qty = "1",
                                Price = 0

                            };

                            var selectedExtras = tt.ExtraItem.MenuToppings.Where(x => x.Selected > 0);
                            foreach (var selecteExtra in selectedExtras)
                            {
                                var OrderModelExtra = new OrderModelExtra()
                                {
                                    DisplayOrder = 0,
                                    ExtraId = selecteExtra.ID,
                                    Id = 0,
                                    ItemName = selecteExtra.Topping,
                                    OrderModelId = 0,
                                    Price = (decimal)selecteExtra.ToppingPrice* selecteExtra.Selected*orderItem.Qta,
                                    Qty = selecteExtra.Selected.ToString()
                                };
                                if (selecteExtra.Selected > 1)
                                {
                                    orderItem.OrderText += OrderModelExtra.Qty + " x " + OrderModelExtra.ItemName + ", ";
                                }
                                else
                                {
                                    orderItem.OrderText += OrderModelExtra.ItemName + ", ";
                                }
                                OrderModel.OrderModelExtras.Add(OrderModelExtra);
                                OrderModel.Price += ((decimal)OrderModelExtra.Price);
                            }
                            orderItem.Total += OrderModel.Price;
                            orderItem.OrderModels.Add(OrderModel);
                        }

                    }
                }
                
                var length = orderItem.OrderText.Length - 3;
                orderItem.OrderText = orderItem.OrderText.Remove(length, 2);
                _menuItem.OrderItems.Remove(_orderItem);
                _menuItem.OrderItems.Add(orderItem);
            }
            else
            {
                OrderItem orderItem = new OrderItem()
                {
                    ID = _orderItem.ID,
                    ItemType = "MenuItem",
                    MenuItemId = _menuItem.Id,
                    MenuItemPropertyId = null,
                    Name = _menuItem.Name,
                    OrderID = 0,
                    Price = _menuItem.Price,
                    Qta = CurrentQty,
                    Total = _menuItem.Price*CurrentQty,
                    OrderText = _menuItem.Name + ", "
                };
                foreach (var tt in _menuItem.MenuItemModels)
                {
                    if (tt.ItemType == "Option")
                    {
                        var selectedOption = tt.OptionItem.MenuDishProperties.FirstOrDefault(x => x.selected);
                        var OrderModel = new OrderModel()
                        {
                            Id = 0,
                            DisplayOrder = tt.DisplayOrder,
                            ItemName = tt.DisplayName,
                            ItemType = "Option",
                            OptionID = selectedOption.ID,
                            OrderItemID = 0,
                            Price = (decimal)selectedOption.DishPropertyPrice*orderItem.Qta,
                            Qty = "1"

                        };
                        orderItem.OrderText += OrderModel.ItemName + ", ";
                        orderItem.Total += OrderModel.Price;
                        orderItem.OrderModels.Add(OrderModel);
                    }
                    else
                    {
                        var OrderModel = new OrderModel()
                        {
                            Id = 0,
                            DisplayOrder = tt.DisplayOrder,
                            ItemName = tt.DisplayName,
                            ItemType = "Extra",
                            OptionID = null,
                            OrderItemID = 0,
                            Qty = "1",
                            Price = 0

                        };
                        var selectedExtras = tt.ExtraItem.MenuToppings.Where(x => x.Selected > 0);
                        foreach (var selecteExtra in selectedExtras)
                        {
                            var OrderModelExtra = new OrderModelExtra()
                            {
                                DisplayOrder = 0,
                                ExtraId = selecteExtra.ID,
                                Id = 0,
                                ItemName = selecteExtra.Topping,
                                OrderModelId = 0,
                                Price = (decimal)selecteExtra.ToppingPrice * selecteExtra.Selected * orderItem.Qta,
                                Qty = selecteExtra.Selected.ToString()
                            };
                            if (selecteExtra.Selected > 1)
                            {
                                orderItem.OrderText += OrderModelExtra.Qty + " x " + OrderModelExtra.ItemName + ", ";
                            }
                            else
                            {
                                orderItem.OrderText += OrderModelExtra.ItemName + ", ";
                            }
                            OrderModel.OrderModelExtras.Add(OrderModelExtra);
                            OrderModel.Price += ((decimal)OrderModelExtra.Price);
                        }
                        
                        orderItem.Total += OrderModel.Price;
                        orderItem.OrderModels.Add(OrderModel);
                    }

                }
                var length = orderItem.OrderText.Length - 3;
                orderItem.OrderText = orderItem.OrderText.Remove(length, 2);
                _menuItem.OrderItems.Remove(_orderItem);
                _menuItem.OrderItems.Add(orderItem);
            }
            
            var cat = MenuCatHelper.MenuCategories.FirstOrDefault(x => x.ID == _menuItem.MenuCategoryID);
            if (cat != null)
            {
                cat.SelectedQty += CurrentQty;
                cat.SelectedQty -= (int)_orderItem.Qta;
            }
            _menuItem.OrderItems.Remove(_orderItem);
            try
            {
                StaticFields.CurrentOrder.OrderItems.Remove(_orderItem);
            }
            catch
            {

            }
            MessagingCenter.Send(this, MessagingFields.EditOrderItem, 1);
            await PopupNavigation.PopAsync();
        }
        
        private async void RemoveClicked(object sender, EventArgs e)
        {
            var cat = MenuCatHelper.MenuCategories.FirstOrDefault(x => x.ID == _menuItem.MenuCategoryID);
            if (cat != null)
            {
                cat.SelectedQty -= (int)_orderItem.Qta;
            }
            _menuItem.OrderItems.Remove(_orderItem);
            try
            {
                StaticFields.CurrentOrder.OrderItems.Remove(_orderItem);
            }
            catch
            {

            }
            
            MessagingCenter.Send(this, MessagingFields.EditOrderItem, 1);
            await PopupNavigation.PopAsync();
        }
        void MissedItem_Clicked(System.Object sender, System.EventArgs e)
        {
            bool scrolled = false;
            foreach (var stack in stackOptions.Children)
            {
                if (typeof(StackLayout) == stack.GetType())
                {
                    var topStack = (stack as StackLayout).Children[0];
                    if (typeof(StackLayout) == topStack.GetType())
                    {
                        var count = (topStack as StackLayout).Children.Count;
                        if (count >= 3)
                        {
                            var checkImage = (topStack as StackLayout).Children[count - 2];
                            if (typeof(Image) == checkImage.GetType())
                            {

                                if (checkImage.IsVisible != true)
                                {
                                    ((topStack as StackLayout).Children[count - 1] as Image).IsVisible = true;
                                    ((topStack as StackLayout).Children[0] as Label).TextColor = Color.FromHex("E65051");
                                    if (!scrolled)
                                    {
                                        scroll.ScrollToAsync(topStack, ScrollToPosition.Start, true);
                                        scrolled = true;
                                    }
                                }
                                else
                                {
                                    ((topStack as StackLayout).Children[count - 1] as Image).IsVisible = false;
                                }

                            }
                        }
                    }
                }
            }
        }
    }
}
