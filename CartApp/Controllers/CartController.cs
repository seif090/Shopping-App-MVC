using CartApp.Helpers;
using CartApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace CartApp.Controllers
{
    public class CartController : Controller
    {
        private const string CartSessionKey = "CartItems";
        private static List<Product> _products = new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "Laptop",
                Description = "High-performance laptop",
                Price = (int)999.99M,
                ImageUrl = "/images/laptop.jpg",
            },
            new Product
            {
                Id = 2,
                Name = "Smartphone",
                Description = "Latest smartphone",
                Price = (int)699.99M,
                ImageUrl = "/images/phone.jpg",
            },
            new Product
            {
                Id = 3,
                Name = "Headphones",
                Description = "Wireless headphones",
                Price = (int)199.99M,
                ImageUrl = "/images/headphones.jpg",
            },
        };

        public IActionResult Index()
        {
            var cartItems =
                HttpContext.Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
            return View(cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            var cartItems =
                HttpContext.Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
            var product = _products.FirstOrDefault(p => p.Id == productId);

            if (product != null)
            {
                var cartItem = cartItems.FirstOrDefault(item => item.Product.Id == productId);
                if (cartItem != null)
                {
                    cartItem.Quantity++;
                }
                else
                {
                    cartItems.Add(new CartItem { Product = product, Quantity = 1 });
                }
                HttpContext.Session.Set(CartSessionKey, cartItems);
            }

            return Json(
                new { success = true, cartItemCount = cartItems.Sum(item => item.Quantity) }
            );
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var cartItems =
                HttpContext.Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
            var cartItem = cartItems.FirstOrDefault(item => item.Product.Id == productId);

            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                HttpContext.Session.Set(CartSessionKey, cartItems);
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult RemoveItem(int productId)
        {
            var cartItems =
                HttpContext.Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
            var cartItem = cartItems.FirstOrDefault(item => item.Product.Id == productId);

            if (cartItem != null)
            {
                cartItems.Remove(cartItem);
                HttpContext.Session.Set(CartSessionKey, cartItems);
            }

            return Json(new { success = true });
        }

        public IActionResult Checkout()
        {
            var cartItems =
                HttpContext.Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
            return View(cartItems);
        }

        [HttpPost]
        public IActionResult CompleteOrder()
        {
            // Clear the cart after successful order
            HttpContext.Session.Remove(CartSessionKey);
            return RedirectToAction("OrderConfirmation");
        }

        public IActionResult OrderConfirmation()
        {
            return View();
        }
    }
}
