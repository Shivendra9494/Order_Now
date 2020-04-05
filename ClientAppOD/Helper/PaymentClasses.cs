using System;
using ClientAppOD.CustomModels;
using System.Web;
using OD.Data;

namespace ClientAppOD.Helper
{
    public class SagepayData
    {
        public string transactionType { get; set; }
        //public ShoppingBasket basket{ get; set;}
        public BillingAddress billingAddress { get; set; }
        public ShippingDetails shippingDetails { get; set; }
        public PaymentMethod paymentMethod { get; set; }
        public string customerEmail { get; set; }
        public string vendorTxCode { get; set; }

        public string currency { get; set; }
        public string customerFirstName { get; set; }
        public string customerLastName { get; set; }
        public string description { get; set; }
        public string customerPhone { get; set; }
        public int amount { get; set; }
    }
    public class RefundData
    {
        public string transactionType { get; set; }
        public string vendorTxCode { get; set; }
        public string referenceTransactionId { get; set; }
        public int amount { get; set; }
        public string currency { get; set; }
        public string description { get; set; }

    }
    public class errors
    {
        public string description { get; set; }
        public string property { get; set; }
        public string clientMessage { get; set; }
        public int code { get; set; }

    }
    public class SecureThreeD
    {
        public string statusCode { get; set; }
        public string statusDetail { get; set; }
        public string transactionId { get; set; }
        public string status { get; set; }

        public string acsUrl { get; set; }
        public string paReq { get; set; }

    }
    public class PaymentMethod
    {
        public Card card { get; set; }
    }
    public class Card
    {
        public string merchantSessionKey { get; set; }
        public string cardIdentifier { get; set; }
        public string reusable { get; set; }
        public string save { get; set; }
    }
    public class cardDetails
    {
        public string cardholderName { get; set; }
        public string cardNumber { get; set; }
        public string expiryDate { get; set; }
        public string securityCode { get; set; }
    }
    public class BillingAddress
    {
        public string address1 { get; set; }
        public string city { get; set; }
        public string postalCode { get; set; }
        public string country { get; set; }
    }
    public class ShippingDetails
    {
        public string recipientFirstName { get; set; }
        public string recipientLastName { get; set; }
        public string shippingAddress1 { get; set; }
        public string shippingCity { get; set; }
        public string shippingPostalCode { get; set; }
        public string shippingCountry { get; set; }
    }
    public class SageOrder
    {
        public string VendorTxCode { get; set; }
        public string SecurityKey { get; set; }

        public string VpsTxId { get; set; }

        public string RedirectUrl { get; set; }

        public DateTime DateInitialised { get; set; }
    }
    public class MerchantKeyDetails
    {
        public string merchantSessionKey { get; set; }

        public string expiry { get; set; }
    }
    public class SagePayResponse
    {
        public ResponseType Status { get; set; }
        public string VendorTxCode { get; set; }
        public string VPSTxId { get; set; }
        public string VPSSignature { get; set; }
        public string StatusDetail { get; set; }
        public string TxAuthNo { get; set; }
        public string AVSCV2 { get; set; }
        public string AddressResult { get; set; }
        public string PostCodeResult { get; set; }
        public string CV2Result { get; set; }
        public string GiftAid { get; set; }
        public string ThreeDSecureStatus { get; set; }
        public string CAVV { get; set; }
        public string AddressStatus { get; set; }
        public string PayerStatus { get; set; }
        public string CardType { get; set; }
        public string Last4Digits { get; set; }
        public string DeclineCode { get; set; }
        public string ExpiryDate { get; set; }
        public string FraudResponse { get; set; }
        public string BankAuthCode { get; set; }
        public string Token { get; set; }


        /// <summary>
        /// Was the transaction successful?
        /// </summary>
        public virtual bool WasTransactionSuccessful
        {
            get
            {
                return (Status == ResponseType.Ok ||
                        Status == ResponseType.Authenticated ||
                        Status == ResponseType.Registered);
            }
        }

       
    }
    public enum ResponseType
    {
        Unknown,
        Ok,
        NotAuthed,
        Abort,
        Rejected,
        Authenticated,
        Registered,
        Malformed,
        Error,
        Invalid,
    }
    public class Address
    {
        public string Surname { get; set; }
        public string Firstnames { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Phone { get; set; }

    }
    }
