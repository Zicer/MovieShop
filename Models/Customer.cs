using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MovieShop.Models
{
    public class Customer
    {
        public int Id { get; set; }

        //============================================================================
        [Required,StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        //============================================================================
        [Required, StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        //============================================================================
        [Required, StringLength(150)]
        [Display(Name = "Billing Adress")]
        public string BillingAdress { get; set; }

        //============================================================================
        [Required, StringLength(10),DataType(DataType.PostalCode)]
        [Display(Name = "Billing Zip")]
        public string BillingZip { get; set; }

        //============================================================================
        [Required, StringLength(165)]
        [Display(Name = "Billing City")]
        public string BillingCity { get; set; }

        //============================================================================
        [Required, StringLength(150)]
        [Display(Name = "Delivery Adress")]
        public string DeliveryAdress { get; set; }

        //============================================================================
        [Required, StringLength(165)]
        [Display(Name = "Delivery Zip")]
        public string DeliveryZip { get; set; }

        //============================================================================
        [Required, StringLength(150)]
        [Display(Name = "Delivery City")]
        public string DeliveryCity { get; set; }

        //============================================================================
        [Required, StringLength(256),DataType(DataType.EmailAddress)]
        //Need to add unique constrraint
        [Display(Name = "Email Address")]
        
        public string EmailAddress { get; set; }

        //============================================================================
        [Required, StringLength(20), DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string PhoneNo { get; set; }


    }
}