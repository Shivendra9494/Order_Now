using System;
namespace ClientAppOD.Helper
{
    public class AppFields
    {
        public const string RestroId = "RestroId";
        public const string IconChanged = "IconChanged";
    }
    public class MessagingFields
    {
        public const string AddOrderItem = "AddOrderItem";
        public const string EditOrderItem = "EditOrderItem";
        public const string OptionSelected = "OptionSelected";
        public const string ExtraSelected = "ExtraSelected";
        public const string ExtraRemoved = "ExtraRemoved";
        public const string CancelClickedOnExtra = "CancelClickedOnExtra";
        public const string CancelClickedOnOption = "CancelClickedOnOption";
        public const string OrderCompleted = "OrderCompleted";
        public const string UpdateDeliverySwitch = "UpdateDeliverySwitch";
        public const string UserLoggedInChanged = "UserLoggedInChanged";
        public const string UserLoggedInChangedBasket = "UserLoggedInChangedBasket";
        public const string UserLoggedInChangedOrders = "UserLoggedInChangedOrders";
        public const string UserLoggedOutChanged = "UserLoggedOutChanged";
        public const string FailedPayment = "FailedPayment";
        public const string LoginFromApple = "LoginFromApple";
    }
    public class PaymentType
    {
        public const string COD = "Cash On Delivery";
        public const string Card = "Card";
    }
    public class OrderStatus
    {
        public const string JustCreated = "Just Created";
        public const string UserAddingItems = "User Adding Items";
        public const string UserAddingDelivery = "User Adding Delivery";
        public const string AddingUserDetails = "Adding User Details";
        public const string Checkout = "User Checking Out";
        public const string Completed = "Completed";
        public const string SentToPrinter = "Sent To Printer";
    }
}
