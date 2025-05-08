using CartApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace CartApp.Controllers
{
    public class HomeController : Controller
    {
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
            return View(_products);
        }
        public IActionResult Search(string query, int? categoryId)
{
    var products = _products.AsQueryable();

    if (!string.IsNullOrEmpty(query))
    {
        products = products.Where(p => 
            p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) || 
            p.Description.Contains(query, StringComparison.OrdinalIgnoreCase));
    }

    if (categoryId.HasValue)
    {
        products = products.Where(p => p.CategoryId == categoryId);
    }

    return View("Index", products.ToList());
}
    }
}
