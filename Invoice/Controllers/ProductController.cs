using Invoice.Models;
using Invoice.Services;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.Controllers
{
    public class ProductController : Controller
    {
        private readonly CsvHelperService _csvHelperService;
        private readonly GenUtility _GenUtility;
        public ProductController(CsvHelperService csvHelperService, GenUtility GenUtility)
        {
            _csvHelperService = csvHelperService;
            _GenUtility = GenUtility;
        }

        public IActionResult Index()
        {
            try
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

            catch (Exception ex)
            {
                _GenUtility.LogError(ex);
                return View(new Product());
            }

        }

        public IActionResult Create()
        {
            ViewBag.Categories = _csvHelperService.GetAllCategories();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _csvHelperService.AddProduct(product);
                    return RedirectToAction("Index");
                }
                ViewBag.Categories = _csvHelperService.GetAllCategories();
              
            }
            catch (Exception ex)
            {
                _GenUtility.LogError(ex);
            }
            return View(product);
        }

        public IActionResult Edit(int id)
        {
            var product = _csvHelperService.GetProductById(id);
            try
            {
                if (product == null)
                {
                    return NotFound();
                }
                ViewBag.Categories = _csvHelperService.GetAllCategories();
            }
            catch (Exception ex)
            {
                _GenUtility.LogError(ex);
            }


            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _csvHelperService.UpdateProduct(product);
                    return RedirectToAction("Index");
                }
                ViewBag.Categories = _csvHelperService.GetAllCategories();
            }
            catch (Exception ex)
            {
                _GenUtility.LogError(ex);
            }
            return View(product);
        }

        public IActionResult Delete(int id)
        {
            var product = _csvHelperService.GetProductById(id);
            try
            {
                if (product == null)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _GenUtility.LogError(ex);
            }
           
            return View(product);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _csvHelperService.DeleteProduct(id);
            }
            catch (Exception ex)
            {
                _GenUtility.LogError(ex);
            }
         
            return RedirectToAction("Index");
        }
    }
}
