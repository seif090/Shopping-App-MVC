using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CartApp.Models;
using CartApp.Services;

namespace CartApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public ProductController(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _context.Products
                    .OrderByDescending(p => p.Id)
                    .ToListAsync();
                return View(products);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading products";
                return View(new List<Product>());
            }
        }

        public IActionResult Create()
        {
            return View(new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,StockQuantity")] Product product, IFormFile? imageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (imageFile != null)
                    {
                        product.ImageUrl = await _fileService.UploadImageAsync(imageFile);
                    }

                    product.CreatedAt = DateTime.UtcNow;
                    product.UpdatedAt = DateTime.UtcNow;

                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Product created successfully";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating product");
            }

            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                TempData["Error"] = "Product not found";
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,StockQuantity,ImageUrl")] Product product, IFormFile? imageFile)
        {
            if (id != product.Id)
                return NotFound();

            try
            {
                if (ModelState.IsValid)
                {
                    var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (existingProduct == null)
                    {
                        TempData["Error"] = "Product not found";
                        return RedirectToAction(nameof(Index));
                    }

                    if (imageFile != null)
                    {
                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                        {
                            await _fileService.DeleteImageAsync(existingProduct.ImageUrl);
                        }
                        product.ImageUrl = await _fileService.UploadImageAsync(imageFile);
                    }
                    else
                    {
                        product.ImageUrl = existingProduct.ImageUrl;
                    }

                    product.UpdatedAt = DateTime.UtcNow;
                    _context.Update(product);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Product updated successfully";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ProductExists(id))
                {
                    TempData["Error"] = "Product not found";
                    return RedirectToAction(nameof(Index));
                }
                throw;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating product");
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    TempData["Error"] = "Product not found";
                    return RedirectToAction(nameof(Index));
                }

                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    await _fileService.DeleteImageAsync(product.ImageUrl);
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Product deleted successfully";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting product";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ProductExists(int id)
        {
            return await _context.Products.AnyAsync(p => p.Id == id);
        }
    }
}