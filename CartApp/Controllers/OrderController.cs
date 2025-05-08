using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CartApp.Models;
using CartApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;

[Authorize]
public class OrderController : Controller
{
        private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly PaymentService _paymentService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public OrderController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        PaymentService paymentService,
        IEmailService emailService,
        IConfiguration configuration
    )
    {
        _context = context;
        _userManager = userManager;
        _paymentService = paymentService;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<IActionResult> Checkout()
    {
        var userId = _userManager.GetUserId(User);
        var cartItems = await _context
            .CartItems.Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!cartItems.Any())
            return RedirectToAction("Index", "Cart");

        var totalAmount = cartItems.Sum(ci => ci.Product.Price * ci.Quantity);
        var paymentIntent = await _paymentService.CreatePaymentIntent(totalAmount);

        ViewBag.ClientSecret = paymentIntent.ClientSecret;
        ViewBag.StripePublicKey = _configuration["Stripe:PublicKey"];

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.Now,
            TotalAmount = totalAmount,
        };

        return View(order);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(Order order, string paymentIntentId)
    {
        if (ModelState.IsValid)
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = await _context
                .CartItems.Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            order.UserId = userId;
            order.OrderDate = DateTime.Now;
            order.TotalAmount = cartItems.Sum(ci => ci.Product.Price * ci.Quantity);
            order.Status = "Processing";

            foreach (var item in cartItems)
            {
                order.OrderItems.Add(
                    new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price,
                    }
                );

                // Update product stock
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity -= item.Quantity;
                }
            }

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            // Send order confirmation email
            await _emailService.SendEmailAsync(
                User.Identity.Name,
                "Order Confirmation",
                $"Thank you for your order! Your order number is {order.Id}."
            );

            return RedirectToAction("OrderConfirmation", new { id = order.Id });
        }

        return View("Checkout", order);
    }
        // ... existing controller code ...

    public async Task<IActionResult> OrderConfirmation(int id)
    {
        var userId = _userManager.GetUserId(User);
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

        if (order == null)
            return NotFound();

        return View(order);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
            return NotFound();

        order.Status = status;
        await _context.SaveChangesAsync();

        // Send status update email to customer
        var user = await _userManager.FindByIdAsync(order.UserId);
        if (user != null)
        {
            await _emailService.SendEmailAsync(
                user.Email,
                "Order Status Update",
                $"Your order #{order.Id} status has been updated to: {status}"
            );
        }

        return RedirectToAction(nameof(Details), new { id = order.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = _userManager.GetUserId(User);
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id && (o.UserId == userId || User.IsInRole("Admin")));

        if (order == null)
            return NotFound();

        return View(order);
    }
}

