using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using OD.Data;

namespace ClientAppOD.CustomModels
{
    public class Customer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Customer()
        {
            this.CustomerCCs = new HashSet<CustomerCC>();
            this.Orders = new HashSet<Order>();
        }

        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Pswd { get; set; }
        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public int BusinessDetailID { get; set; }
        public string TermsAccepted { get; set; }
        public string TermsAcceptedAt { get; set; }
        public string TermsAcceptedIP { get; set; }
        public string resetcode { get; set; }
        public string GoogleId { get; set; }
        public string FacebookId { get; set; }
        public string Token { get; set; }

        public virtual Store BusinessDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CustomerCC> CustomerCCs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
    }
    public class CustomerCC
    {
        public int Id { get; set; }
        public string Identifier { get; set; }
        public string LastDigits { get; set; }
        public int CustomerID { get; set; }
        public string CardType { get; set; }
        public string Expiry { get; set; }

        public virtual Customer Customer { get; set; }
    }
    public class NetworkAuthData
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Logo { get; set; }

        public string Picture { get; set; }

        public string Background { get; set; }

        public string Foreground { get; set; }
        
    }
    public class Data
    {
        [JsonProperty("is_silhouette")]
        public bool IsSilhouette { get; set; }
        public int Height { get; set; }
        public string Url { get; set; }
        public int Width { get; set; }
    }

    public class Picture
    {
        public Data Data { get; set; }
    }

    public class FacebookProfile
    {
        public string Email { get; set; }
        public string Id { get; set; }
        public Picture Picture { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
    }
    public class AuthNetwork
    {
        public string Name { get; set; }

        public string Icon { get; set; }

        public string Background { get; set; }

        public string Foreground { get; set; }
    }
}
