namespace TelescopeShop.Models
{
    public class Basket
    {
        public int BasketId { get; set; }
        public string UserId { get; set; }
        public List<BasketItem> BasketItems { get; set; }
    }

}
