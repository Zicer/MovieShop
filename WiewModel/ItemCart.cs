using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MovieShop.WiewModel
{
    public class ItemCart
    {
       //Movie
        
        public string Title { get; set; }
        public int ReleaseYear { get; set; }
        public decimal price { get; set; }
        public string NumCoppy { get; set; }
       
       
    }
}