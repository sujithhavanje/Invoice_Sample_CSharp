using Invoice.Models;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.Controllers
{
    public class ProductController : Controller
    {
        private readonly CsvHelperService _csvHelperService;

        public ProductController(CsvHelperService csvHelperService)
        {
            _csvHelperService = csvHelperService;
        }

        public IActionResult Index()
        {
            var products = _csvHelperService.GetAllProducts();
            var categories = _csvHelperService.GetAllCategories();

            var productsWithCategoryNames = from product in products
                                            join category in categories
                                            on product.CategoryId equals category.Id
                                            select new Product
                                            {
                                                Id = product.Id,
                                                Name = product.Name,
                                                Description = product.Description,
                                                Price = product.Price,
                                                Quantity = product.Quantity,
                                                CategoryId = product.CategoryId,
                                                CategoryName = category.Name
                                            };

            return View(productsWithCategoryNames);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _csvHelperService.GetAllCategories();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _csvHelperService.AddProduct(product);
                return RedirectToAction("Index");
            }
            ViewBag.Categories = _csvHelperService.GetAllCategories();
            return View(product);
        }

        public IActionResult Edit(int id)
        {
            var product = _csvHelperService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.Categories = _csvHelperService.GetAllCategories();
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _csvHelperService.UpdateProduct(product);
                return RedirectToAction("Index");
            }
            ViewBag.Categories = _csvHelperService.GetAllCategories();
            return View(product);
        }

        public IActionResult Delete(int id)
        {
            var product = _csvHelperService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public IActionResult DeleteConfirmed(int id)
        {
            _csvHelperService.DeleteProduct(id);
            return RedirectToAction("Index");
        }
    }
}
