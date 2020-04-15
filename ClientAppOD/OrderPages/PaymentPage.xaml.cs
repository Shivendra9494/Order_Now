using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClientAppOD.APIPost;
using ClientAppOD.CustomModels;
using ClientAppOD.Helper;
using ClientAppOD.UserPages;
using Newtonsoft.Json;
using OD.Data;
using Plugin.InputKit.Shared.Controls;
using Xamarin.Forms;
namespace ClientAppOD.OrderPages
{
    public partial class PaymentPage : ContentPage
    {
        VoucherPostHelper voucherPostHelper = new VoucherPostHelper();
        int _tapCount = 0;
        PaymentHelper paymentHelper = new PaymentHelper();
        OrderPostHelper OrderPostHelper = new OrderPostHelper();
        public PaymentPage()
        {
            InitializeComponent();
            btnMonthSelector.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => btnMonthSelectorTapped()
                )
            });
            stackActionNewCard.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => PaymentClicked(stackActionNewCard)
                )
            });
            stackCash.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => PaymentClicked(stackCash)
                )
            });
            btnYearSelector.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => btnYearSelectorTapped()
                )
            });
            SaveCardCheck.IsChecked = true;
            foreach(var card in StaticFields.CurrentCustomer.CustomerCCs)
            {
                StackLayout stackLayout = new StackLayout()
                {
                    Padding = new Thickness(0),
                    Spacing = 0
                };
                StackLayout innerStackLayout = new StackLayout()
                {
                    Padding = new Thickness(0),
                    Orientation = StackOrientation.Horizontal
                };
                var exp = card.Expiry.Substring(0, 2) + "/" + card.Expiry.Substring(2, 2);
                var radioButton = new RadioButton()
                {
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    Color = Color.FromHex("236adb"),
                    Text = card.CardType + " ****" + card.LastDigits + " exp " + exp,
                    FontFamily= "Nunito-ExtraLight",
                    TextFontSize=14

                };
                radioButton.Clicked += RadioButton_Clicked;
                innerStackLayout.Children.Add(radioButton);
                var imageButton = new ImageButton()
                {
                    HorizontalOptions = LayoutOptions.End,
                    Source = ImageSource.FromFile("leftBlue.png"),
                    Rotation = 270,
                    BackgroundColor = Color.Transparent,
                    Padding = new Thickness(0),
                    WidthRequest = 20,
                    VerticalOptions = LayoutOptions.Center
                };
                imageButton.Clicked += ImageButton_Clicked;
                innerStackLayout.Children.Add(imageButton);
                innerStackLayout.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => PaymentClicked(innerStackLayout)
                )
                });
                stackLayout.Children.Add(innerStackLayout);
                StackLayout secondInnerStackLayout = new StackLayout()
                {
                    Padding = new Thickness(0),
                    IsVisible=false
                };
                secondInnerStackLayout.Children.Add(new Label()
                {
                    
                    Text = card.Identifier,
                    IsVisible=false
                });
                StackLayout second1stack = new StackLayout()
                {
                    Padding = new Thickness(0),
                    Orientation = StackOrientation.Horizontal,
                    Spacing=10
                };
                Image image = new Image()
                {
                    HorizontalOptions=LayoutOptions.Start
                };
                if(card.CardType.ToLower().Contains("visa"))
                {
                    image.Source = ImageSource.FromFile("visa.png");
                }
                else
                {
                    image.Source = ImageSource.FromFile("mastercard.png");
                }
                second1stack.Children.Add(image);
                second1stack.Children.Add(new Label()
                {
                    Text=card.CardType,
                    HorizontalOptions=LayoutOptions.StartAndExpand
                });
                secondInnerStackLayout.Children.Add(second1stack);
                secondInnerStackLayout.Children.Add(new Label()
                {
                    Text = "Security number",
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    TextColor = Color.Black
                });
                StackLayout second2stack = new StackLayout()
                {
                    Padding = new Thickness(0),
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 10
                };
                Frame frame = new Frame()
                {
                    Padding = new Thickness(0),
                    HasShadow = false,
                    HeightRequest = 30,
                    WidthRequest = 150,
                    BorderColor = Color.Black
                };
                frame.Content = new Entry()
                {
                    WidthRequest = 150,
                    Keyboard = Keyboard.Numeric,
                    MaxLength = 3
                };
                second2stack.Children.Add(frame);
                second2stack.Children.Add(new Image()
                {
                    Source =ImageSource.FromFile("cvv.png"),
                    HorizontalOptions = LayoutOptions.Start,
                    HeightRequest = 40
                });
                second2stack.Children.Add(new Label()
                {
                    Text = "The 3 digit number on the back of your card",
                    HorizontalOptions = LayoutOptions.StartAndExpand
                });
                secondInnerStackLayout.Children.Add(second2stack);
                secondInnerStackLayout.Children.Add(new BoxView()
                {
                    Margin = new Thickness(0, 10, 0, 10)
                });
                secondInnerStackLayout.Children.Add(new Label()
                {
                    Margin = new Thickness(0, 10, 0, 10),
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    FontSize = 14,
                    TextColor = Color.DarkRed,
                    Text = "I have a discount voucher"
                });
                secondInnerStackLayout.Children.Add(new BoxView()
                {
                    Margin = new Thickness(0)
                });
                var orderButton = new Button()
                {
                    CornerRadius = 1,
                    Text = "Place my order",
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    TextColor = Color.White,
                    FontSize = 18,
                    BackgroundColor = Color.FromHex("236adb")
                };
                orderButton.Clicked += btnExistingCardClicked;
                secondInnerStackLayout.Children.Add(orderButton);
                
                stackLayout.Children.Add(secondInnerStackLayout);
                stackLayout.Children.Add(new BoxView()
                {
                    Margin = new Thickness(-10, 10,-10,0)
                }); 
                PaymentGroupRadio.Children.Insert(0, stackLayout);

            }
            if (string.IsNullOrEmpty(StaticFields.CurrentCustomer.Address1) || string.IsNullOrEmpty(StaticFields.CurrentCustomer.City) || string.IsNullOrEmpty(StaticFields.CurrentCustomer.PostalCode))
            {
                stackBillingAddress.IsVisible = true;
            }
            else
            {
                checkBillingAddress.IsChecked = true;
                checkBillingAddress.Text = StaticFields.CurrentCustomer.Address1 + " " + (string.IsNullOrEmpty(StaticFields.CurrentCustomer.Address2) ? "" : StaticFields.CurrentCustomer.Address2) + " " + StaticFields.CurrentCustomer.City + " " + StaticFields.CurrentCustomer.PostalCode + "(untick this box to change your billing address)";
                entryAddress1.Text = StaticFields.CurrentCustomer.Address1;
                entryAddress2.Text = (string.IsNullOrEmpty(StaticFields.CurrentCustomer.Address2) ? "" : StaticFields.CurrentCustomer.Address2);
                entryCity.Text = StaticFields.CurrentCustomer.City;
                entryPostCode.Text = StaticFields.CurrentCustomer.PostalCode;
            }
            MessagingCenter.Subscribe<PaymentWebView, string>(this, MessagingFields.FailedPayment, UpdateError);
          
            if (StaticFields.CurrentOrder.ServiceCharge == null)
            {
                StaticFields.CurrentOrder.ServiceCharge = (decimal)StaticFields.CurrentStoreInfo.ServiceCharge;
                StaticFields.CurrentOrder.SubTotal += (decimal)StaticFields.CurrentStoreInfo.ServiceCharge;
            }
            UpdateBasket();
        }
        protected void backClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
        private void btnMonthSelectorTapped()
        {
            _tapCount += 1;
            if (_tapCount > 1)
            {
                _tapCount = 0;
                return;
            }
            entryMonth.Focus();
            _tapCount = 0;
        }
        private void btnYearSelectorTapped()
        {
            _tapCount += 1;
            if (_tapCount > 1)
            {
                _tapCount = 0;
                return;
            }
            entryYear.Focus();
            _tapCount = 0;
        }
        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            PaymentClicked((sender as ImageButton).Parent as StackLayout);
        }

        private void RadioButton_Clicked(object sender, EventArgs e)
        {
            PaymentClicked((sender as RadioButton).Parent as StackLayout);
        }

        void UpdateError(PaymentWebView sender, string text)
        {
            lblError.Text = "Error : " + text;
            scroll.ScrollToAsync(0, 0, false);
            lblError.IsVisible = true;
        }
        private void checkBillingAddressCheckChanged(object sender, EventArgs e)
        {
            if(checkBillingAddress.IsChecked)
            {
                stackBillingAddress.IsVisible = false;
            }
            else
            {
                stackBillingAddress.IsVisible = true;
            }
        }

        private void btnCODClicked(object sender,EventArgs e)
        {
            var order = StaticFields.CurrentOrder;
            AddDefaultFields(order, PaymentType.COD,OrderStatus.SentToPrinter);
            AddOrder(order);
        }

        private void btnPaymentClicked(object sender, EventArgs e)
        {
            var order = StaticFields.CurrentOrder;
            AddDefaultFields(order, PaymentType.Card, OrderStatus.Checkout);
            AddOrder(order);

        }
        private void btnExistingCardClicked(object sender, EventArgs e)
        {
            var order = StaticFields.CurrentOrder;
            AddDefaultFields(order, PaymentType.Card, OrderStatus.Checkout);
            string cvv = (((((sender as Button).Parent as StackLayout).Children[3] as StackLayout).Children[0] as Frame).Content as Entry).Text;
            AddOrder(order,(((sender as Button).Parent as StackLayout).Children[0] as Label).Text,cvv);

        }
        private async Task AddOrder(Order order, string cardIdentifier="", string cvv = "")
        {
            Loader.IsVisible = true;
            FramePayment.IsVisible = false;
            int Id = 0;
            string OrderId=  await OrderPostHelper.PostOrder(order);
            try
            {
                Id = Convert.ToInt32(OrderId);

            }
            catch(Exception ex)
            {
                Id = 0;
            }
            if(Id>0 && order.PaymentType==PaymentType.COD)
            {
                order.ID = Id;
                await Navigation.PushAsync(new OrderDetailPage(order,true));
                StaticFields.CurrentOrder = null;
                StaticFields.CurrentDiscount = null;
                foreach (var cat in MenuCatHelper.MenuCategories.Where(x => x.Name != "Want to repeat?" && x.SelectedQty > 0))
                {
                    foreach (var menuItem in cat.MenuItems.Where(x => x.OrderItems.Count() > 0))
                    {
                        menuItem.OrderItems = new ObservableCollection<OrderItem>();
                    }
                    cat.SelectedQty = 0;

                }
               
            }
            else if(Id > 0 && order.PaymentType == PaymentType.Card)
            {
                StaticFields.CurrentOrder.ID = Id;
                order.ID = Id;
                
                 if(string.IsNullOrEmpty(cardIdentifier))
                {
                    cardDetails details = new cardDetails()
                    {
                        cardholderName = entryFirstName.Text + " " + entryLastName.Text,
                        cardNumber = entryCardNumber.Text.Replace(" ", ""),
                        expiryDate = entryMonth.SelectedItem.ToString() + entryYear.SelectedItem.ToString(),
                        securityCode = entrySecurityCode.Text
                    };
                    var data = "{\"cardDetails\":{\"cardholderName\": \"" + details.cardholderName + "\",\"cardNumber\": \"" + details.cardNumber + "\",\"expiryDate\": \"" + details.expiryDate + "\",\"securityCode\": \"" + details.securityCode + "\"}}";

                    cardIdentifier = paymentHelper.GetCardIdentifier(StaticFields.SagepayUrl + "card-identifiers", data);
                    var status = PayCard(SaveCardCheck.IsChecked, order, entryAddress1.Text, entryAddress2.Text, entryCity.Text, entryPostCode.Text, cardIdentifier);

                }
                else
                {
                    var data = "{\"securityCode\": \"" + cvv + "\"}";

                    var cvvOK = paymentHelper.CheckCVV(StaticFields.SagepayUrl + "card-identifiers/"+cardIdentifier+"/security-code", data);
                    if(string.IsNullOrEmpty(cvvOK))
                    {
                        var status = PayCard(false, order, entryAddress1.Text, entryAddress2.Text, entryCity.Text, entryPostCode.Text, cardIdentifier, "Payment","true");
                    }
                    else
                    {
                        lblError.Text = "Error : " + cvvOK;
                        scroll.ScrollToAsync(0, 0, false);
                        lblError.IsVisible = true;
                    }

                }
            }
            
            FramePayment.IsVisible = true;
            Loader.IsVisible = false;
        }
        public async Task<string> PayCard(bool SaveCard, Order order, string address1, string address2, string city, string postcode, string cardIdentifier, string transactionType = "Payment",string reusable=null)
        {
            string header = StaticFields.header;
            var store = StaticFields.CurrentStoreInfo;
            SagepayData sagepayData = new SagepayData();
            try
            {
                if (string.IsNullOrEmpty(address1))
                {
                    address1 = "NA";
                }
                if (string.IsNullOrEmpty(address2))
                {
                    address2 = "NA";
                }
                if (string.IsNullOrEmpty(city))
                {
                    city = "NA";
                }
                if (string.IsNullOrEmpty(postcode))
                {
                    postcode = "NA";
                }


                var sagePayBillingAddress = new Address()
                {
                    Address1 = address1,
                    Address2 = address2,
                    City = city,
                    State = "-",
                    Country = "GB",
                    PostCode = postcode
                };
                CustomerHelper customerHelper = new CustomerHelper();
                Customer customer = StaticFields.CurrentCustomer;

                sagepayData = paymentHelper.GetSagePayData(StaticFields.CurrentOrder, sagePayBillingAddress, customer,reusable, SaveCard, cardIdentifier,transactionType);
                var data = JsonConvert.SerializeObject(sagepayData);
                WebResponse myWebResponse;
                Stream responseStream;

                var myUri = new Uri(StaticFields.SagepayUrl + "transactions");
                var myWebRequest = WebRequest.Create(myUri);
                var myHttpWebRequest = (HttpWebRequest)myWebRequest;
                myHttpWebRequest.PreAuthenticate = true;
                myHttpWebRequest.Method = "POST";
                //var data = Encoding.ASCII.GetBytes(postData);
                myHttpWebRequest.Headers.Add("Authorization", "Basic " + header);
                myHttpWebRequest.Accept = "application/json";
                myHttpWebRequest.ContentType = "application/json";

                using (var streamWriter = new StreamWriter(myHttpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(data);
                }

                myWebResponse = myWebRequest.GetResponse();
                responseStream = myWebResponse.GetResponseStream();
                //if (responseStream == null) return null;

                var myStreamReader = new StreamReader(responseStream, Encoding.Default);
                var json = myStreamReader.ReadToEnd();
                var oMycustomclassname = JsonConvert.DeserializeObject<SecureThreeD>(json);
                responseStream.Close();
                myWebResponse.Close();

                var status = oMycustomclassname.status;
                if (status == "Ok")
                {
                    var value = await OrderPostHelper.UpdatedOrder(order.ID, OrderStatus.SentToPrinter, oMycustomclassname.transactionId);
                    if(value>0)
                    {
                        StaticFields.CurrentOrder.PaymentType = PaymentType.Card;
                        await Navigation.PushAsync(new OrderDetailPage(StaticFields.CurrentOrder,true));
                        StaticFields.CurrentOrder = null;
                        StaticFields.CurrentDiscount = null;
                        foreach (var cat in MenuCatHelper.MenuCategories.Where(x => x.Name != "Want to repeat?" && x.SelectedQty > 0))
                        {
                            foreach (var menuItem in cat.MenuItems.Where(x => x.OrderItems.Count() > 0))
                            {
                                menuItem.OrderItems = new ObservableCollection<OrderItem>();
                            }
                            cat.SelectedQty = 0;

                        }
                    }
                }
                else if (status == "3DAuth")
                {

                    await Navigation.PushAsync(new PaymentWebView(StaticFields.ServerURL + "/AppPayment/Auth3DHandle?OrderId=" + StaticFields.CurrentOrder.ID + "&SaveCard=true&paReq=" + oMycustomclassname.paReq+ "&acsUrl="+oMycustomclassname.acsUrl+ "&transactionId="+oMycustomclassname.transactionId));

                }
                return status;
            }
            catch (WebException e)
            {
                
                var reader = new StreamReader(e.Response.GetResponseStream());
                var content1 = reader.ReadToEnd();
               
                return content1;
            }

        }

        private void NewCardTapped(object sender,EventArgs e)
        {
            _tapCount += 1;
            if (_tapCount > 1)
            {
                return;
            }
            radioCOD.IsChecked = false;
            stackCOD.IsVisible = false;
            btnArrowCOD.Rotation = 0;
            if (stackNewCard.IsVisible)
            {
                stackNewCard.IsVisible = false;
                btnArrowNewCard.Rotation = 0;
                RadioNewCard.IsChecked = false;
            }
            else
            {
                stackNewCard.IsVisible = true;
                btnArrowNewCard.Rotation = 180;
                RadioNewCard.IsChecked = true;
            }
            
            _tapCount = 0;
        }
        private void CODTapped(object sender, EventArgs e)
        {
            _tapCount += 1;
            if (_tapCount > 1)
            {
                return;
            }
            stackNewCard.IsVisible = false;
            btnArrowNewCard.Rotation = 0;
            RadioNewCard.IsChecked = false;
            if (stackCOD.IsVisible)
            {
                
                radioCOD.IsChecked = false;
                stackCOD.IsVisible = false;
                btnArrowCOD.Rotation = 0;
            }
            else
            {
                
                btnArrowCOD.Rotation = 180;
                radioCOD.IsChecked = true;
                stackCOD.IsVisible = true;
                
            }
            _tapCount = 0;
        }
        private static void AddDefaultFields(Order order, string PaymentType, string status)
        {
            order.CustomerID = StaticFields.CurrentCustomer.ID;
            order.BusinessDetailID = StaticFields.CurrentStoreInfo.ID;
            order.Email = StaticFields.CurrentCustomer.Email;
            order.FirstName = StaticFields.CurrentCustomer.FirstName;
            order.LastName = StaticFields.CurrentCustomer.LastName;
            order.SessionId = "";
            order.Acknowledged = false;
            order.Printed = false;
            order.DiscountID = 0;
            order.PaymentType = PaymentType;
            order.Phone = StaticFields.CurrentCustomer.Phone;
            order.PostalCode = StaticFields.CurrentPostCode;
            order.Status = status;
            order.ClientIP = "";
            order.ValidForLoyality = false;
            order.DeliveryType = StaticFields.CurrentOrder.DeliveryType;
            if (order.ServiceCharge == null)
            {
                order.ServiceCharge = (decimal)StaticFields.CurrentStoreInfo.ServiceCharge;
                order.SubTotal += (decimal)StaticFields.CurrentStoreInfo.ServiceCharge;
            }
        }

        private void PaymentClicked(StackLayout stack)
        {
            
           if((stack.Children[1] as ImageButton).Rotation == 270)
            {
                foreach (var child in PaymentGroupRadio.Children)
                {
                    if (child.GetType() == typeof(StackLayout))
                    {
                        ((child as StackLayout).Children[1] as StackLayout).IsVisible = false;
                        (((child as StackLayout).Children[0] as StackLayout).Children[0] as RadioButton).IsChecked = false;
                        (((child as StackLayout).Children[0] as StackLayout).Children[1] as ImageButton).Rotation = 270;
                    }
                }
                ((stack.Parent as StackLayout).Children[1] as StackLayout).IsVisible = true;
                (stack.Children[0] as RadioButton).IsChecked = true;
                (stack.Children[1] as ImageButton).Rotation = 90;
            }
           else
            {
                ((stack.Parent as StackLayout).Children[1] as StackLayout).IsVisible = false;
                (stack.Children[0] as RadioButton).IsChecked = false;
                (stack.Children[1] as ImageButton).Rotation = 270;
            }
            
        }
        private void UpdateBasket()
        {
            
                    stackOrderItem.Children.Clear();
            
                    foreach (var orderItem in StaticFields.CurrentOrder.OrderItems)
                    {
                       
                        StackLayout topStack = new StackLayout()
                        {
                            Padding = new Thickness(0)
                        };
                        StackLayout firstStack = new StackLayout()
                        {
                            Orientation = StackOrientation.Horizontal,
                            Padding = new Thickness(0, 10, 0, 0)
                        };
                        firstStack.Children.Add(new Label()
                        {
                            Text = orderItem.Qta + " x " + orderItem.Name,
                            FontAttributes = FontAttributes.Bold,
                            HorizontalOptions = LayoutOptions.StartAndExpand
                        });

                        firstStack.Children.Add(new Label()
                        {
                            Text = orderItem.Total.ToString(),
                            HorizontalOptions = LayoutOptions.EndAndExpand,
                            MinimumWidthRequest=100
                        });

                        topStack.Children.Add(firstStack);
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
                        topStack.Children.Add(secondStack);
                        
                        stackOrderItem.Children.Add(topStack);
                        
                    }
            if (StaticFields.CurrentOrder.Discount != null)
            {
                lblDiscountPercentage.Text = StaticFields.CurrentOrder.DiscountPercentage + "% discount";
                lblDiscount.Text = "-" + ((decimal)StaticFields.CurrentOrder.Discount).ToString("F2");
                stackDiscount.IsVisible = true;
            }
            else
            {
                stackDiscount.IsVisible = false;
            }
            if (StaticFields.CurrentOrder.VoucherCodeDiscount > 0)
            {
                lblVoucherCode.Text = "Discount voucher";
                lblVoucherAmount.Text = "-" + ((decimal)StaticFields.CurrentOrder.VoucherCodeDiscount).ToString("F2");
                stackVoucher.IsVisible = true;
            }
            else
            {
                stackVoucher.IsVisible = false;
            }
            if (StaticFields.Deliverytype == "d")
            {
                stackDeliveryFee.IsVisible = true;
                lblDeliveryFee.Text = ((decimal)StaticFields.CurrentOrder.ShippingFee).ToString("F2");
                lblDelivery.Text = "Delivery " + StaticFields.CurrentOrder.DeliveryTime;
                imgDelivery.Source = ImageSource.FromFile("delivery.png");
            }
            else
            {
                stackDeliveryFee.IsVisible = false;
                lblDelivery.Text = "Collection " + StaticFields.CurrentOrder.DeliveryTime;
                imgDelivery.Source = ImageSource.FromFile("BasketBlue.png");
            }
            if(StaticFields.CurrentOrder.ServiceCharge>0)
            {
                stackServiceCharge.IsVisible = true;
                lblServiceCharge.Text =  ((decimal)StaticFields.CurrentOrder.ServiceCharge).ToString("F2");
            }
            else
            {
                stackServiceCharge.IsVisible = false;
            }
            lblTotalPrice.Text = ((decimal)StaticFields.CurrentOrder.OrderTotal).ToString("F2");
            lblSubTotal.Text = "£" + ((decimal)StaticFields.CurrentOrder.SubTotal).ToString("F2");
            lblAddess.Text = StaticFields.CurrentStoreInfo.Address;
            

        }

        void VoucherButton_Clicked(System.Object sender, System.EventArgs e)
        {
            var stack = (sender as Button).Parent as StackLayout;
            var frame = stack.Children[1] as Frame;
            if(frame.IsVisible)
            {
                frame.IsVisible = false;
                (stack.Children[3] as Button).IsVisible = false;
            }
            else
            {
                frame.IsVisible = true;
                (stack.Children[3] as Button).IsVisible = true;
            }
        }

        async void VoucherApply_Clicked(System.Object sender, System.EventArgs e)
        {
            var stack = (sender as Button).Parent as StackLayout;
            var frame = stack.Children[1] as Frame;
            var errorLabel = stack.Children[2] as Label;
            var entryVoucher = frame.Content as Entry;
            if (!string.IsNullOrEmpty(entryVoucher.Text))
            {
                var voucher = await voucherPostHelper.GetVoucher(entryVoucher.Text);
                if (voucher != null)
                {
                    errorLabel.TextColor = Color.DarkGreen;
                    StaticFields.CurrentOrder.DiscountID = voucher.ID;

                    var discount = Math.Round((decimal)(StaticFields.CurrentOrder.OrderTotal * voucher.VoucherCodeDiscount) / 100, 2);
                    if (StaticFields.CurrentOrder.Discount != null)
                    {
                        if (StaticFields.CurrentOrder.Discount > discount)
                        {
                            errorLabel.TextColor = Color.FromHex("EB6361");
                            errorLabel.Text = "Discount of £" + discount + " already applied";
                            return;
                        }
                        else
                        {
                            StaticFields.CurrentOrder.SubTotal = Math.Round((decimal)(StaticFields.CurrentOrder.SubTotal + StaticFields.CurrentOrder.Discount), 2);
                            StaticFields.CurrentOrder.Discount = null;
                            StaticFields.CurrentOrder.DiscountID = null;
                            StaticFields.CurrentOrder.DiscountPercentage = "";
                        }
                    }
                    if (!string.IsNullOrEmpty(StaticFields.CurrentOrder.VoucherCode))
                    {
                        StaticFields.CurrentOrder.SubTotal = Math.Round((decimal)(StaticFields.CurrentOrder.SubTotal + StaticFields.CurrentOrder.VoucherCodeDiscount), 2);
                    }
                    StaticFields.CurrentOrder.VoucherCodeDiscount = discount;
                    StaticFields.CurrentOrder.VoucherCode = voucher.VoucherCodeText + "|" + voucher.VoucherCodeDiscount;
                    StaticFields.CurrentOrder.SubTotal = Math.Round((decimal)(StaticFields.CurrentOrder.SubTotal - discount), 2);
                    errorLabel.Text = "Voucher applied for £" + discount;
                    UpdateBasket();
                }
                else
                {
                    errorLabel.TextColor = Color.FromHex("EB6361");
                    errorLabel.Text = "Couldn't find this voucher code";
                }
            }
            else
            {
                errorLabel.Text = "Voucher code can't be empty";
            }
        }

        void ImageButton_Clicked_1(System.Object sender, System.EventArgs e)
        {
        }

    }
}
