﻿using Invoice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

public class CartController : Controller
{
    private readonly CsvHelperService _csvHelperService;

    public CartController(CsvHelperService csvHelperService)
    {
        _csvHelperService = csvHelperService;
    }

    private List<CartItem> GetCartItems()
    {
        if (HttpContext.Session.TryGetValue("CartItems", out byte[] cartItemsData))
        {
            var cartItemsJson = Encoding.UTF8.GetString(cartItemsData);
            return JsonConvert.DeserializeObject<List<CartItem>>(cartItemsJson);
        }
        return new List<CartItem>();
    }

    private void SaveCartItems(List<CartItem> cartItems)
    {
        var cartItemsJson = JsonConvert.SerializeObject(cartItems);
        var cartItemsData = Encoding.UTF8.GetBytes(cartItemsJson);
        HttpContext.Session.Set("CartItems", cartItemsData);
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

    [HttpPost]
    public IActionResult AddToCart(int productId, int quantity)
    {
        var cartItems = GetCartItems();
        var product = _csvHelperService.GetProductById(productId);

        var existingItem = cartItems.Find(item => item.ProductId == productId);
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

    [HttpPost]
    public IActionResult RemoveFromCart(int productId)
    {
        var cartItems = GetCartItems();

        // Find the item to remove
        var itemToRemove = cartItems.FirstOrDefault(item => item.ProductId == productId);

        if (itemToRemove != null)
        {
            cartItems.Remove(itemToRemove);
            SaveCartItems(cartItems); // Save updated cart items
        }

        return RedirectToAction("Index");
    }


    [HttpPost]
    public IActionResult GenerateInvoice(int discountId, int taxId, int paymentMethodId)
    {
        // Implement your invoice generation logic here
        // Example: Fetch cart items and calculate totals
        var cartItems = GetCartItems(); // Replace with your cart retrieval method
        var discounts = _csvHelperService.LoadDiscounts(); // Replace with your discount service method
        var taxes = _csvHelperService.LoadTaxes(); // Replace with your tax service method

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
}