using CsvHelper;
using CsvHelper.Configuration;
using Invoice.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

public class CsvHelperService
{
    private readonly string _categoriesFilePath = "wwwroot/App_Data/Category.csv";
    private readonly string _productsFilePath = "wwwroot/App_Data/Products.csv";
    private readonly string _usersFilePath = "wwwroot/App_Data/Users.csv";
    private readonly string _discountsFilePath = "wwwroot/App_Data/Discounts.csv";
    private readonly string _taxesFilePath= "wwwroot/App_Data/Taxes.csv";
    public List<Category> GetAllCategories()
    {
        using (var reader = new StreamReader(_categoriesFilePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            return csv.GetRecords<Category>().ToList();
        }
    }

    public Category GetCategoryById(int id)
    {
        return GetAllCategories().FirstOrDefault(c => c.Id == id);
    }

    public void AddCategory(Category category)
    {
        var categories = GetAllCategories();
        category.Id = categories.Count > 0 ? categories.Max(c => c.Id) + 1 : 1;
        categories.Add(category);
        WriteCategoriesToCsv(categories);
    }

    public void UpdateCategory(Category category)
    {
        var categories = GetAllCategories();
        var existingCategory = categories.FirstOrDefault(c => c.Id == category.Id);
        if (existingCategory != null)
        {
            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            WriteCategoriesToCsv(categories);
        }
    }

    public void DeleteCategory(int id)
    {
        var categories = GetAllCategories();
        var categoryToRemove = categories.FirstOrDefault(c => c.Id == id);
        if (categoryToRemove != null)
        {
            categories.Remove(categoryToRemove);
            WriteCategoriesToCsv(categories);
        }
    }

    public void WriteCategoriesToCsv(List<Category> categories)
    {
        using (var writer = new StreamWriter(_categoriesFilePath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(categories);
        }
    }

    public List<Product> GetAllProducts()
    {
        using (var reader = new StreamReader(_productsFilePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            // Map records to Product objects, handling optional CategoryName
            csv.Context.RegisterClassMap<ProductMap>(); // Register the custom mapping

            return csv.GetRecords<Product>().ToList();
        }
    }

    public class ProductMap : ClassMap<Product>
    {
        public ProductMap()
        {
            Map(m => m.Id).Name("Id");
            Map(m => m.Name).Name("Name");
            Map(m => m.Description).Name("Description");
            Map(m => m.Price).Name("Price");
            Map(m => m.Quantity).Name("Quantity");
            Map(m => m.CategoryId).Name("CategoryId");
            Map(m => m.CategoryName).Name("CategoryName").Optional(); // Optional mapping for CategoryName
        }
    }

    public Product GetProductById(int id)
    {
        return GetAllProducts().FirstOrDefault(p => p.Id == id);
    }

    public void AddProduct(Product product)
    {
        var products = GetAllProducts();
        product.Id = products.Count > 0 ? products.Max(p => p.Id) + 1 : 1;
        products.Add(product);
        WriteProductsToCsv(products);
    }

    public void UpdateProduct(Product product)
    {
        var products = GetAllProducts();
        var existingProduct = products.FirstOrDefault(p => p.Id == product.Id);
        if (existingProduct != null)
        {
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Quantity = product.Quantity;
            existingProduct.CategoryId = product.CategoryId;
            WriteProductsToCsv(products);
        }
    }

    public void DeleteProduct(int id)
    {
        var products = GetAllProducts();
        var productToRemove = products.FirstOrDefault(p => p.Id == id);
        if (productToRemove != null)
        {
            products.Remove(productToRemove);
            WriteProductsToCsv(products);
        }
    }

    public void WriteProductsToCsv(List<Product> products)
    {
        using (var writer = new StreamWriter(_productsFilePath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(products);
        }
    }

    public List<User> GetAllUsers()
    {
        using (var reader = new StreamReader(_usersFilePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            return csv.GetRecords<User>().ToList();
        }
    }

    public void AddUser(User user)
    {
        var users = GetAllUsers();
        users.Add(user);
        WriteUsersToCsv(users);
    }

    public void WriteUsersToCsv(List<User> users)
    {
        using (var writer = new StreamWriter(_usersFilePath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(users);
        }
    }

    public User GetUserByUsername(string username)
    {
        return GetAllUsers().FirstOrDefault(u => u.Username == username);
    }


    public List<Discount> LoadDiscounts()
    {
        using (var reader = new StreamReader(_discountsFilePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var discounts = csv.GetRecords<Discount>().ToList();
            return discounts;
        }
    }
    public Product FindProduct(int productId)
    {
        using (var reader = new StreamReader(_productsFilePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var products = csv.GetRecords<Product>().ToList();
            var product = products.FirstOrDefault(p => p.Id == productId);
            return product;
        }
    }

    public List<Tax> LoadTaxes()
    {
        using (var reader = new StreamReader(_taxesFilePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var taxes = csv.GetRecords<Tax>().ToList();
            return taxes;
        }
    }

    public void UpdateUser(User updatedUser)
    {
        var users = GetAllUsers();
        var user = users.FirstOrDefault(u => u.Username == updatedUser.Username);

        if (user != null)
        {
            user.Email = updatedUser.Email;
            user.Address = updatedUser.Address;
            user.ContactNumber = updatedUser.ContactNumber;
        }

        using (var writer = new StreamWriter(_usersFilePath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(users);
        }
    }

    public string GetPaymentMethodName(int paymentMethodId)
    {
        // Implement logic to get payment method name based on paymentMethodId
        switch (paymentMethodId)
        {
            case 1:
                return "Credit Card";
            case 2:
                return "Debit Card";
            case 3:
                return "Paypal";
            case 4:
                return "Cash";
            default:
                return "Unknown";
        }
    }
}
