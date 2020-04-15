using System;
using System.Collections.Generic;
using System.Linq;
using ClientAppOD.APIPost;
using ClientAppOD.CustomModels;
using Xamarin.Forms;

namespace ClientAppOD.UserPages
{
    public partial class SavedCardsPage : ContentPage
    {
        CustomerPostHelper customerPostHelper = new CustomerPostHelper();
        public SavedCardsPage()
        {
            InitializeComponent();
            UpdateStack();
        }

        private void UpdateStack()
        {
            stackMain.Children.Clear();
            if (StaticFields.CurrentCustomer != null)
            {
                foreach (var card in StaticFields.CurrentCustomer.CustomerCCs)
                {
                    StackLayout stackLayout = new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        Padding = new Thickness(0, 20)
                    };
                    stackLayout.Children.Add(new Label()
                    {
                        Text = card.Id.ToString(),
                        IsVisible = false
                    });
                    StackLayout stackInner = new StackLayout()
                    {
                        Padding = new Thickness(0),
                        HorizontalOptions = LayoutOptions.StartAndExpand
                    };
                    stackInner.Children.Add(new Label()
                    {
                        Text = card.CardType + " ending in ****" + card.LastDigits,
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        HorizontalTextAlignment = TextAlignment.Start
                    });
                    stackInner.Children.Add(new Label()
                    {
                        Text = "Expiry: " + card.Expiry,
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        HorizontalTextAlignment = TextAlignment.Start
                    });
                    stackLayout.Children.Add(stackInner);
                    Button btn = new Button()
                    {
                        Text = "Remove",
                        Padding = new Thickness(20, 10),
                        BackgroundColor = Color.FromHex("236adb"),
                        TextColor = Color.White
                    };
                    btn.Clicked += Btn_Clicked;
                    stackLayout.Children.Add(btn);
                    stackMain.Children.Add(stackLayout);
                    stackMain.Children.Add(new BoxView());
                }
            }
        }

        private async void Btn_Clicked(object sender, EventArgs e)
        {
            int CardId = Convert.ToInt32((((sender as Button).Parent as StackLayout).Children[0] as Label).Text);
            var card = StaticFields.CurrentCustomer.CustomerCCs.FirstOrDefault(x => x.Id == CardId);
            var res = await DisplayAlert("Delete " + card.CardType + " ending ****" + card.LastDigits, "Are you sure to remove this card", "Yes", "No");
            if (res)
            {
                if (card != null)
                {
                    StaticFields.CurrentCustomer.CustomerCCs.Remove(card);
                    UpdateStack();
                    await customerPostHelper.DeleteCustomerCard(CardId);
                }
            }

        }

        async void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
