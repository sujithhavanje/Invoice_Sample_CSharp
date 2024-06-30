using Microsoft.AspNetCore.Mvc.Rendering;

namespace Invoice.Models
{
    public class CartView
    {
        public List<Product> Products { get; set; }
        public List<CartItem> CartItems { get; set; }
        public IEnumerable<SelectListItem> Discounts { get; set; }
        public IEnumerable<SelectListItem> Taxes { get; set; }
        public IEnumerable<SelectListItem> PaymentMethods { get; set; }
    }
}
