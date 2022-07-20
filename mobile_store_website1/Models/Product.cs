using System;
using System.Collections.Generic;

namespace mobile_store_website1.Models
{
    public partial class Product
    {
        public Product()
        {
            Orders = new HashSet<Order>();
        }

        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductColor { get; set; }
        public string? ProductYear { get; set; }
        public int? ProductPrice { get; set; }
        public int? ProductAvailability { get; set; }
        public byte[]? ProductImage { get; set; }
        public string? ProductImagePath { get; set; }
        public int? ModelId { get; set; }
        public byte[]? Image { get; set; }
       // public string productimages { get; set; }

        public virtual Model? Model { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
