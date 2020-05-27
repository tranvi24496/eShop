using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.ViewModels.Catalog.Products
{
    public class ProductImageViewModel
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public string Caption { get; set; }
        public bool IsDefault { get; set; }
        public long FileSize { get; set; }
    }
}
