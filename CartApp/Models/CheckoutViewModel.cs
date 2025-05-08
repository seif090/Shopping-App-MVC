using System.ComponentModel.DataAnnotations;

namespace CartApp.Models
{
    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public decimal TotalAmount { get; set; }

        [Required]
        public string ShippingAddress { get; set; }

        [Required]
        [CreditCard]
        public string CardNumber { get; set; }

        [Required]
        [StringLength(4, MinimumLength = 3)]
        public string CVV { get; set; }

        [Required]
        public string ExpiryDate { get; set; }
    }
}