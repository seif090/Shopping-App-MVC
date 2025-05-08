using CartApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace CartApp.Controllers
{
    public class ProductController : Controller
    {
        private static List<Product> _products = new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "Product 1",
                Description = "Description 1",
                Price = (int)19.99M,
                ImageUrl = "/images/product1.jpg",
            },
            new Product
            {
                Id = 2,
                Name = "Product 2",
                Description = "Description 2",
                Price = (int)29.99M,
                ImageUrl = "/images/product2.jpg",
            },
            new Product
            {
                Id = 3,
                Name = "Product 3",
                Description = "Description 3",
                Price = (int)39.99M,
                ImageUrl = "/images/product3.jpg",
            },
        };

        public IActionResult Index()
        {
            return View(_products);
        }

        public IActionResult Details(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            return View(product);
        }
    }
}
