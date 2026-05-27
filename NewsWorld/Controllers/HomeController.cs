using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWorld.Data;
using NewsWorld.Models;
using System.Diagnostics;

namespace NewsWorld.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // HOME PAGE
        // =========================
        public IActionResult Index(int? categoryId, int? cityId, string? search)
        {
            ViewBag.Categories = _context.Categories.OrderBy(x => x.CategoryName).ToList();
            ViewBag.Cities = _context.Cities.Where(x => x.IsActive).OrderBy(x => x.CityName).ToList();

            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SelectedCityId = cityId;
            ViewBag.SearchQuery = search;

            var query = _context.News
                               .Include(x => x.Category)
                               .Include(x => x.City)
                               .Where(x => x.IsActive == true);

            if (categoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == categoryId.Value);
            }

            if (cityId.HasValue)
            {
                query = query.Where(x => x.CityId == cityId.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Title!.Contains(search) ||
                                         x.SortDescription!.Contains(search) ||
                                         x.FullDescription!.Contains(search));
            }

            var newsList = query.OrderByDescending(x => x.CreatedDate).ToList();

            return View(newsList);
        }

        // =========================
        // NEWS DETAILS
        // =========================
        public IActionResult NewsDetails(int id)
        {
            var news = _context.News
                               .Include(x => x.Category)
                               .Include(x => x.City)
                               .FirstOrDefault(x => x.Id == id);

            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // =========================
        // CATEGORY PAGE
        // =========================


        public IActionResult Category(string id)
        {
            var categoryNews = _context.News
                                       .Include(x => x.Category)
                                       .Include(x => x.City)
                                       .Where(x => x.Category!.CategoryName == id)
                                       .OrderByDescending(x => x.CreatedDate)
                                       .ToList();

            ViewBag.Category = id;

            return View(categoryNews);
        }

        // =========================
        // PRIVACY
        // =========================
        public IActionResult Privacy()
        {
            return View();
        }

        // =========================
        // ERROR PAGE
        // =========================
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                            ?? HttpContext.TraceIdentifier
            });
        }
    }
}