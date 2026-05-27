using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWorld.Data;
using NewsWorld.Models;
using X.PagedList;
using X.PagedList.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NewsWorld.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Category(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var categories = _context.Categories
                             .OrderBy(x => x.Id)
                             .ToPagedList(pageNumber, pageSize);

            return View(categories);
        }

        // ADD CATEGORY
        [HttpGet]
        public IActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                var isDuplicate = _context.Categories.Any(x =>
                    x.CategoryName != null &&
                    category.CategoryName != null &&
                    x.CategoryName.ToLower() == category.CategoryName.ToLower());

                if (isDuplicate)
                {
                    ModelState.AddModelError("CategoryName", "This category already exists.");
                    return View(category);
                }

                _context.Categories.Add(category);
                _context.SaveChanges();

                TempData["Success"] = "Category Added Successfully";

                return RedirectToAction("Category");
            }

            return View(category);
        }

        // EDIT CATEGORY
        public IActionResult EditCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(x => x.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // EDIT POST CATEGORY
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                var isDuplicate = _context.Categories.Any(x =>
                    x.Id != category.Id &&
                    x.CategoryName != null &&
                    category.CategoryName != null &&
                    x.CategoryName.ToLower() == category.CategoryName.ToLower());

                if (isDuplicate)
                {
                    ModelState.AddModelError("CategoryName", "This category already exists.");
                    return View(category);
                }

                _context.Categories.Update(category);
                _context.SaveChanges();

                TempData["Success"] = "Category Updated Successfully";

                return RedirectToAction("Category");
            }

            return View(category);
        }

        //DELETE CATEGORY
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            // Check category used in news
            bool isUsed = _context.News.Any(x => x.CategoryId == id);

            if (isUsed)
            {
                TempData["Error"] = "Category is already used in News.";

                return RedirectToAction("Category");
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            TempData["Success"] = "Category Removed Successfully";

            return RedirectToAction("Category");
        }
    }
}