using CartApp.Helpers;
using CartApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace CartApp.Controllers
{
    public class WishListController : Controller
    {
        private const string WishListSessionKey = "WishListItems";
        private static List<Product> _products = new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "Laptop",
                Description = "High-performance laptop",
                Price = 999.99M,
                ImageUrl = "/images/laptop.jpg",
            },
            new Product
            {
                Id = 2,
                Name = "Smartphone",
                Description = "Latest smartphone",
                Price = 699.99M,
                ImageUrl = "/images/phone.jpg",
            },
            new Product
            {
                Id = 3,
                Name = "Headphones",
                Description = "Wireless headphones",
                Price = 199.99M,
                ImageUrl = "/images/headphones.jpg",
            },
        };

        public IActionResult Index()
        {
            var wishListItems =
                HttpContext.Session.Get<List<WishListItem>>(WishListSessionKey)
                ?? new List<WishListItem>();
            return View(wishListItems);
        }

        [HttpPost]
        public IActionResult AddToWishList(int productId)
        {
            var wishListItems =
                HttpContext.Session.Get<List<WishListItem>>(WishListSessionKey)
                ?? new List<WishListItem>();
            var product = _products.FirstOrDefault(p => p.Id == productId);

            if (product != null && !wishListItems.Any(w => w.Product.Id == productId))
            {
                wishListItems.Add(new WishListItem { Product = product, DateAdded = DateTime.Now });
                HttpContext.Session.Set(WishListSessionKey, wishListItems);
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult RemoveFromWishList(int productId)
        {
            var wishListItems =
                HttpContext.Session.Get<List<WishListItem>>(WishListSessionKey)
                ?? new List<WishListItem>();
            var item = wishListItems.FirstOrDefault(w => w.Product.Id == productId);

            if (item != null)
            {
                wishListItems.Remove(item);
                HttpContext.Session.Set(WishListSessionKey, wishListItems);
            }

            return Json(new { success = true });
        }
    }
}
