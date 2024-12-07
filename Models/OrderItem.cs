namespace TelescopeShop.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; } // Primary Key
        public int OrderId { get; set; } // Foreign Key to Order
        public int ProductId { get; set; } // Foreign Key to Product
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public Order Order { get; set; } // Navigation Property
        public Product Product { get; set; } // Navigation Property
    }

}
