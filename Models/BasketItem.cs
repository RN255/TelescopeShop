﻿namespace TelescopeShop.Models
{
    public class BasketItem
    {
        public int BasketItemId { get; set; }
        public int BasketId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public Basket Basket { get; set; }
        public Product Product { get; set; }
    }

}
