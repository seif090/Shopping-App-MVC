using System.Security.Claims;
using CartApp.Models;
using CartApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(
            ApplicationDbContext context,
            IEmailService emailService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _emailService = emailService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                TempData["Error"] = "Order not found";
                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    TempData["Error"] = "Order not found";
                    return RedirectToAction(nameof(Index));
                }

                if (!IsValidStatus(status))
                {
                    TempData["Error"] = "Invalid status";
                    return RedirectToAction(nameof(Details), new { id });
                }

                order.Status = status;
                order.LastUpdated = DateTime.UtcNow;
                order.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);

                await _context.SaveChangesAsync();

                if (order.User?.Email != null)
                {
                    await _emailService.SendEmailAsync(
                        order.User.Email,
                        "Order Status Updated",
                        GenerateEmailBody(order, status)
                    );
                }

                TempData["Success"] = "Order status updated successfully";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while updating the order status";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        private bool IsValidStatus(string status)
        {
            var validStatuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
            return validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
        }

        private string GenerateEmailBody(Order order, string status)
        {
            return $@"Dear {order.User?.UserName},

Your order #{order.Id} status has been updated to: {status}

Order Details:
- Order Date: {order.OrderDate:MM/dd/yyyy HH:mm}
- Total Amount: {order.TotalAmount:C}

If you have any questions, please contact our support team.

Thank you for shopping with us!";
        }
    }
}