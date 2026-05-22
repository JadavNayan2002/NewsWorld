using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWorld.Data;
using NewsWorld.Models;
using X.PagedList.Extensions;

namespace NewsWorld.Controllers
{
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public NewsController(ApplicationDbContext context,
                              IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // NEWS LIST
        public IActionResult Index()
        {
            var newsList = _context.News
                                   .OrderByDescending(x => x.CreatedDate)
                                   .ToList();
            var news = _context.News
                   .Include(x => x.Category)
                   .ToList();

            return View(newsList);
        }

        // =========================
        // ADD NEWS
        // =========================
        // ADD NEWS (GET)
        [HttpGet]
        public IActionResult AddNews()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        // ADD NEWS (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddNews(News news, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Image Upload
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string folderPath = Path.Combine(_environment.WebRootPath, "newsimages");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string fileName = Guid.NewGuid().ToString()
                                      + Path.GetExtension(ImageFile.FileName);

                    string filePath = Path.Combine(folderPath, fileName);

                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageFile.CopyTo(stream);
                    }

                    news.ImagePath = "/newsimages/" + fileName;
                }

                _context.News.Add(news);
                _context.SaveChanges();

                TempData["newsSuccess"] = "News Published Successfully";

                return RedirectToAction("Dashboard", "Admin");
            }

            // IMPORTANT
            ViewBag.Categories = _context.Categories.ToList();

            return View(news);
        }

        // NEWS DETAILS
        public IActionResult Details(int id)
        {
            var news = _context.News.FirstOrDefault(x => x.Id == id);

            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // =========================
        // MANAGE NEWS
        // =========================
        public IActionResult ManageNews(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            // Fetch the news, JOIN the category data, and paginate it
            var newsList = _context.News
                                   .Include(n => n.Category) // <--- THIS LOADS THE CATEGORY NAMES
                                   .OrderByDescending(x => x.Id)
                                   .ToPagedList(pageNumber, pageSize);

            return View(newsList);
        }

        // EDIT NEWS (GET)
        [HttpGet]
        public IActionResult EditNews(int id)
        {
            var news = _context.News.Find(id);

            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // EDIT NEWS (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditNews(News news, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                var existingNews = _context.News.Find(news.Id);

                if (existingNews == null)
                {
                    return NotFound();
                }

                existingNews.Title = news.Title;
                existingNews.SortDescription = news.SortDescription;
                existingNews.FullDescription = news.FullDescription;
                existingNews.Category = news.Category;

                // Update Image
                if (ImageFile != null)
                {
                    string folderPath = Path.Combine(_environment.WebRootPath, "newsimages");

                    string fileName = Guid.NewGuid().ToString()
                                      + Path.GetExtension(ImageFile.FileName);

                    string filePath = Path.Combine(folderPath, fileName);

                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageFile.CopyTo(stream);
                    }

                    existingNews.ImagePath = "/newsimages/" + fileName;
                }

                _context.SaveChanges();

                TempData["newsSuccess"] = "News Updated Successfully";

                return RedirectToAction("ManageNews");
            }

            return View(news);
        }

        // =========================
        // DELETE NEWS
        // =========================
        public IActionResult Delete(int id)
        {
            var news = _context.News.Find(id);

            if (news == null)
            {
                return NotFound();
            }

            // Delete Image
            if (!string.IsNullOrEmpty(news.ImagePath))
            {
                string oldImagePath = Path.Combine(
                    _environment.WebRootPath,
                    news.ImagePath.TrimStart('/'));

                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            _context.News.Remove(news);
            _context.SaveChanges();

            TempData["newsSuccess"] = "News Deleted Successfully";

            return RedirectToAction("ManageNews");
        }

        // =========================
        // CATEGORY
        // =========================

        // Catergory List with Pagination
        public IActionResult Category(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            // 1. You fetched the data into a variable named 'news'
            var categories = _context.Categories
                             .OrderBy(x => x.Id)
                             .ToPagedList(pageNumber, pageSize);

            return View(categories);
        }

        [HttpGet]
        public IActionResult AddCategory()
        {
            return View();
        }

        // ADD CATEGORY
        [HttpPost]
        public IActionResult AddCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();

                TempData["Success"] = "Category Added Successfully";

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