using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWorld.Data;
using NewsWorld.Models;
using X.PagedList;
using X.PagedList.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var news = await _context.News
                .Include(x => x.Category)
                .Include(x => x.City)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return View(news.ToPagedList(pageNumber, pageSize));
        }

        // =========================
        // ADD NEWS
        // =========================
        [HttpGet]
        public IActionResult AddNews()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.CityList = new SelectList(_context.Cities, "CityId", "CityName");
            return View();
        }

        // ADD NEWS (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNews(News news, IFormFile ImageFile)
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
                await _context.SaveChangesAsync();

                TempData["newsSuccess"] = "News Published Successfully";

                return RedirectToAction("Dashboard", "Admin");
            }

            ViewBag.Categories = _context.Categories.ToList();

            ViewBag.CityList = new SelectList(
                _context.Cities,
                "CityId",
                "CityName"
            );

            return View(news);
        }

        // NEWS DETAILS
        public async Task<IActionResult> Details(int id)
        {
            var news = await _context.News
            .Include(x => x.Category)
            .Include(x => x.City)
            .FirstOrDefaultAsync(x => x.Id == id);

            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // =========================
        // MANAGE NEWS
        // =========================
        public async Task<IActionResult> ManageNews(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var news = await _context.News
                .Include(x => x.Category)
                .Include(x => x.City)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return View(news.ToPagedList(pageNumber, pageSize));
        }

        // EDIT NEWS (GET)
        [HttpGet]
        public async Task<IActionResult> EditNews(int id)
        {
            var news = await _context.News.FindAsync(id);

            if (news == null)
            {
                return NotFound();
            }

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.CityList = new SelectList(_context.Cities, "CityId", "CityName", news.CityId);
            return View(news);
        }

        // EDIT NEWS (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNews(News news, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                var existingNews = await _context.News.FindAsync(news.Id);

                if (existingNews == null)
                {
                    return NotFound();
                }

                existingNews.Title = news.Title;
                existingNews.SortDescription = news.SortDescription;
                existingNews.FullDescription = news.FullDescription;
                existingNews.CategoryId = news.CategoryId;
                existingNews.CityId = news.CityId;

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

                await _context.SaveChangesAsync();

                TempData["newsSuccess"] = "News Updated Successfully";

                return RedirectToAction("ManageNews");
            }

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.CityList = new SelectList(_context.Cities, "CityId", "CityName", news.CityId);
            return View(news);
        }

        // =========================
        // DELETE NEWS
        // =========================
        public async Task<IActionResult> Delete(int id)
        {
            var news = await _context.News.FindAsync(id);

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
            await _context.SaveChangesAsync();

            TempData["newsSuccess"] = "News Deleted Successfully";

            return RedirectToAction("ManageNews");
        }

        // ACTIVE/INACTIVE NEWS STATUS TOGGLE
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var news = await _context.News.FindAsync(id);

            if (news != null)
            {
                news.IsActive = !news.IsActive;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ManageNews");
        }
    }
}