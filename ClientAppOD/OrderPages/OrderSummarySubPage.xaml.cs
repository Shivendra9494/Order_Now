using System;
using System.Collections.Generic;
using System.Linq;
using OD.Data;
using Xamarin.Forms;

namespace ClientAppOD.OrderPages
{
    public partial class OrderSummarySubPage : ContentView
    {
        public OrderSummarySubPage(Order order)
        {
            InitializeComponent();
            foreach (var orderItem in order.OrderItems)
            {
                
                stackOrderItem.Children.Add(new BoxView());
                StackLayout firstStack = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(0, 10, 0, 0)
                };
                firstStack.Children.Add(new Label()
                {
                    Text = orderItem.Name,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromHex("404040"),
                    HorizontalOptions = LayoutOptions.StartAndExpand
                });
                if (orderItem.ItemType == "MenuItem" && orderItem.OrderModels.Count() == 0)
                {
                    Picker picker = new Picker()
                    {
                        HorizontalOptions = LayoutOptions.EndAndExpand
                    };
                    for (int i = 0; i < 100; i++)
                    {
                        picker.Items.Add(i.ToString());
                    }
                    picker.SelectedItem = orderItem.Qta.ToString();
                    firstStack.Children.Add(picker);
                }
                else
                {
                    firstStack.Children.Add(new Label()
                    {
                        Text = orderItem.Qta.ToString(),
                        HorizontalOptions = LayoutOptions.EndAndExpand
                    });
                }
                firstStack.Children.Add(new Label()
                {
                    Text = "£ " + orderItem.Total.ToString(),
                    HorizontalOptions = LayoutOptions.EndAndExpand
                });
                stackOrderItem.Children.Add(firstStack);
                StackLayout secondStack = new StackLayout();
                foreach (var model in orderItem.OrderModels)
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
                            secondStack.Children.Add(new Label()
                            {
                                Text = "+" + extramodel.ItemName,
                                HorizontalOptions = LayoutOptions.StartAndExpand
                            });
                        }
                    }

                }
                stackOrderItem.Children.Add(secondStack);

            }
        }
    }
}
