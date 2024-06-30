using Invoice.Models;
using Invoice.Services;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CsvHelperService _csvHelperService;
        private readonly GenUtility _GenUtility;
        public CategoryController(CsvHelperService csvHelperService, GenUtility  GenUtility)
        {
            _csvHelperService = csvHelperService;
            _GenUtility = GenUtility;
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
            try
            {
                if (ModelState.IsValid)
                {
                    _csvHelperService.AddCategory(category);
                    return RedirectToAction("Index");
                }
                return View(category);
            }
            catch (Exception ex)
            {
                _GenUtility.LogError(ex);
                return View(category);
            }

           
        }

        public IActionResult Edit(int id)
        {
            var category = _csvHelperService.GetCategoryById(id);
            try
            {
              
                if (category == null)
                {
                    return NotFound();
                }
                return View(category);
            }
            catch (Exception ex)
            {
                _GenUtility.LogError(ex);
                return View(category);
            }
            
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    _csvHelperService.UpdateCategory(category);
                    return RedirectToAction("Index");
                }
                return View(category);
            }
            catch (Exception ex)
            {
                _GenUtility.LogError(ex);
                return View(category);
            }
        }

        public IActionResult Delete(int id)
        {
            var category = _csvHelperService.GetCategoryById(id);
            try
            {
                if (category == null)
                {
                    return NotFound();
                }
                return View(category);
            }
            catch (Exception ex)
            {
                _GenUtility.LogError(ex);
                return View(category);
            }
           
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _csvHelperService.DeleteCategory(id);
               
            }
            catch (Exception ex)
            {
                _GenUtility.LogError(ex);
              
            }
            return RedirectToAction("Index");
        }
    }
}
