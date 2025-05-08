using CartApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CartApp.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private const string CartSessionKey = "CartItems";

        public IActionResult Checkout()
        {
            var cartItems =
                HttpContext.Session.GetString(CartSessionKey) != null
                    ? System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(
                        HttpContext.Session.GetString(CartSessionKey))
                    : new List<CartItem>();
            if (!cartItems.Any())
                return RedirectToAction("Index", "Cart");

            var checkoutViewModel = new CheckoutViewModel
            {
                CartItems = cartItems,
                TotalAmount = cartItems.Sum(item => item.Product.Price * item.Quantity),
            };

            return View(checkoutViewModel);
        }

        [HttpPost]
        public IActionResult PlaceOrder(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Checkout", model);

            var cartItems =
                HttpContext.Session.GetString(CartSessionKey) != null
                    ? System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(
                        HttpContext.Session.GetString(CartSessionKey)
                    )
                    : new List<CartItem>();

            var order = new Order
            {
                UserId = User.Identity.Name,
                OrderDate = DateTime.Now,
                TotalAmount = cartItems.Sum(item => item.Product.Price * item.Quantity),
                OrderStatus = "Pending",
                OrderItems = cartItems
                    .Select(item => new OrderItem
                    {
                        ProductId = item.Product.Id,
                        Quantity = item.Quantity,
                        Price = item.Product.Price,
                    })
                    .ToList(),
            };

            // Save order to database (to be implemented)

            // Clear cart
            HttpContext.Session.Remove(CartSessionKey);

            return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
        }

        public IActionResult OrderConfirmation(int orderId)
        {
            // Get order details from database (to be implemented)
            return View();
        }

        public IActionResult OrderHistory()
        {
            // Get user's order history from database (to be implemented)
            return View();
        }
    }
}
