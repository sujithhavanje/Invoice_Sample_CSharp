using Invoice.Models;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CsvHelperService _csvHelperService;

        public CategoryController(CsvHelperService csvHelperService)
        {
            _csvHelperService = csvHelperService;
        }

        public IActionResult Index()
        {
            var categories = _csvHelperService.GetAllCategories();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _csvHelperService.AddCategory(category);
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public IActionResult Edit(int id)
        {
            var category = _csvHelperService.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _csvHelperService.UpdateCategory(category);
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public IActionResult Delete(int id)
        {
            var category = _csvHelperService.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public IActionResult DeleteConfirmed(int id)
        {
            _csvHelperService.DeleteCategory(id);
            return RedirectToAction("Index");
        }
    }
}
