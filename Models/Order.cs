namespace TelescopeShop.Models
{
    public class Order
    {
        public int OrderId { get; set; } // Primary Key
        public string UserId { get; set; } // Foreign Key to User
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItem> OrderItems { get; set; } // Navigation Property
    }

}
