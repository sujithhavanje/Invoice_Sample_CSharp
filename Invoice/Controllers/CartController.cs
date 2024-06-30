using Invoice.Models;
using Invoice.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

public class CartController : Controller
{
    private readonly CsvHelperService _csvHelperService;
    private readonly GenUtility _GenUtility;
    public CartController(CsvHelperService csvHelperService, GenUtility GenUtility)
    {
        _csvHelperService = csvHelperService;
        _GenUtility = GenUtility;
    }

    private List<CartItem> GetCartItems()
    {
        try
        {
            if (HttpContext.Session.TryGetValue("CartItems", out byte[] cartItemsData))
            {
                var cartItemsJson = Encoding.UTF8.GetString(cartItemsData);
                return JsonConvert.DeserializeObject<List<CartItem>>(cartItemsJson);
            }
        }
        catch (Exception ex)
        {
            _GenUtility.LogError(ex);
        }
       
        return new List<CartItem>();
    }

    private void SaveCartItems(List<CartItem> cartItems)
    {
        try
        {
            var cartItemsJson = JsonConvert.SerializeObject(cartItems);
            var cartItemsData = Encoding.UTF8.GetBytes(cartItemsJson);
            HttpContext.Session.Set("CartItems", cartItemsData);
        }
        catch (Exception ex)
        {
            _GenUtility.LogError(ex);
        }
    }

    public IActionResult Index()
    {
        var products = _csvHelperService.GetAllProducts();
        var cartItems = GetCartItems();

        var discounts = _csvHelperService.LoadDiscounts().Select(d => new SelectListItem
        {
            Value = d.Id.ToString(),
            Text = $"{d.Name}"
        }).ToList();

        var taxes = _csvHelperService.LoadTaxes().Select(t => new SelectListItem
        {
            Value = t.Id.ToString(),
            Text = $"{t.Name}"
        }).ToList();


        try
        {
            var cartView = new CartView
            {
                Products = products,
                CartItems = cartItems,
                Discounts = discounts,
                Taxes = taxes,
                PaymentMethods = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Credit Card" },
                new SelectListItem { Value = "2", Text = "Debit Card" },
                new SelectListItem { Value = "3", Text = "Paypal" },
                 new SelectListItem { Value = "3", Text = "Cash" },
                     new SelectListItem { Value = "3", Text = "Unknown" }
            }
            };
            return View(cartView);
        }
        catch (Exception ex)
        {
            _GenUtility.LogError(ex);
            return View(new CartView());
        }
        
       
    }

    [HttpPost]
    public IActionResult AddToCart(int productId, int quantity)
    {
        var cartItems = GetCartItems();
        var product = _csvHelperService.GetProductById(productId);

        var existingItem = cartItems.Find(item => item.ProductId == productId);

        try
        {

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cartItems.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = product.Name,
                    Quantity = quantity,
                    UnitPrice = product.Price
                });
            }

            SaveCartItems(cartItems);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _GenUtility.LogError(ex);
            return View(new CartItem());
        }

    }

    [HttpPost]
    public IActionResult RemoveFromCart(int productId)
    {
        var cartItems = GetCartItems();

        try
        {
            var itemToRemove = cartItems.FirstOrDefault(item => item.ProductId == productId);

            if (itemToRemove != null)
            {
                cartItems.Remove(itemToRemove);
                SaveCartItems(cartItems); // Save updated cart items
            }
        }
        catch (Exception ex)
        {
            _GenUtility.LogError(ex);
         
        }
        return RedirectToAction("Index");
    }


    [HttpPost]
    public IActionResult GenerateInvoice(int discountId, int taxId, int paymentMethodId)
    {
        try
        {
            var cartItems = GetCartItems();
            var discounts = _csvHelperService.LoadDiscounts();
            var taxes = _csvHelperService.LoadTaxes();

            // Calculate subtotal
            decimal subtotal = cartItems.Sum(item => item.Quantity * item.UnitPrice);

            // Apply discount
            var discount = discounts.FirstOrDefault(d => d.Id == discountId);
            decimal discountAmount = (discount != null) ? subtotal * (decimal)discount.Percentage / 100 : 0;

            // Apply tax
            var tax = taxes.FirstOrDefault(t => t.Id == taxId);
            decimal taxAmount = (tax != null) ? (subtotal - discountAmount) * (decimal)tax.Percentage / 100 : 0;

            // Calculate total amount
            decimal totalAmount = subtotal - discountAmount + taxAmount;

            var invoice = new Invoices
            {
                CartItems = cartItems,
                Subtotal = subtotal,
                DiscountAmount = discountAmount,
                TaxAmount = taxAmount,
                TotalAmount = totalAmount,
                PaymentMethod = _csvHelperService.GetPaymentMethodName(paymentMethodId) // Implement a method to get payment method name
            };
            return View("Invoice", invoice);
        }
        catch (Exception ex)
        {
            _GenUtility.LogError(ex);
            return View(new Invoices());
        }

    }
}
