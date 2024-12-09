using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TelescopeShop.Models;
using TelescopeShop.Data;

namespace TelescopeShop.Controllers
{
    public class BasketController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor to inject ApplicationDbContext
        public BasketController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddToBasket(int productId, int quantity)
        {
            // Get the user ID from the current user claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                // Handle unauthenticated users (e.g., redirect to login)
                return RedirectToAction("Login", "Account");
            }

            // Check if the user already has a basket
            var basket = await _context.Baskets.Include(b => b.BasketItems)
                                               .FirstOrDefaultAsync(b => b.UserId == userId);

            if (basket == null)
            {
                // Create a new basket for the user
                basket = new Basket { UserId = userId, BasketItems = new List<BasketItem>() };
                _context.Baskets.Add(basket);
            }

            // Check if the item is already in the basket
            var basketItem = basket.BasketItems.FirstOrDefault(bi => bi.ProductId == productId);
            if (basketItem != null)
            {
                // Update quantity if the item already exists in the basket
                basketItem.Quantity += 1;
            }
            else
            {
                // Add a new item to the basket
                var product = await _context.Product.FindAsync(productId);
                if (product == null)
                {
                    // Handle the case where the product doesn't exist
                    return NotFound();
                }

                basket.BasketItems.Add(new BasketItem
                {
                    ProductId = productId,
                    Quantity = 1,
                    Price = product.Price // Fetch product price from the database
                });
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect to the basket index page
            return RedirectToAction("Index", "Basket");
        }

        // Optional: Add an Index method to display the basket
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var basket = await _context.Baskets.Include(b => b.BasketItems)
                                               .ThenInclude(bi => bi.Product)
                                               .FirstOrDefaultAsync(b => b.UserId == userId);

            return View(basket);
        }

        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch the user's basket
            var basket = await _context.Baskets.Include(b => b.BasketItems)
                                               .ThenInclude(bi => bi.Product)
                                               .FirstOrDefaultAsync(b => b.UserId == userId);

            if (basket == null || !basket.BasketItems.Any())
            {
                // Redirect to basket if it's empty
                return RedirectToAction("Index", "Basket");
            }

            // Create a new order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = basket.BasketItems.Sum(bi => bi.Quantity * bi.Price),
                OrderItems = basket.BasketItems.Select(bi => new OrderItem
                {
                    ProductId = bi.ProductId,
                    Quantity = bi.Quantity,
                    Price = bi.Price
                }).ToList()
            };

            _context.Orders.Add(order);

            // Remove the user's basket
            _context.Baskets.Remove(basket);

            await _context.SaveChangesAsync();

            return RedirectToAction("OrderSuccess");
        }

        public IActionResult OrderSuccess()
        {
            return View();
        }


    }
}
