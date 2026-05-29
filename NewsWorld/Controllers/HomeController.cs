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
        public async Task<IActionResult> Index(int? categoryId, int? cityId, string? search)
        {
            ViewBag.Categories = await _context.Categories
                                               .OrderBy(x => x.CategoryName)
                                               .ToListAsync();

            ViewBag.Cities = await _context.Cities
                                           .Where(x => x.IsActive)
                                           .OrderBy(x => x.CityName)
                                           .ToListAsync();

            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SelectedCityId = cityId;
            ViewBag.SearchQuery = search;

            var query = _context.News
                                .Include(x => x.Category)
                                .Include(x => x.City)
                                .Where(x => x.IsActive);

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
                                         x.FullDescription!.Contains(search)||
                                         x.City!.CityName.Contains(search) ||
                                         x.Category!.CategoryName.Contains(search));
            }

            var newsList = await query
                                    .OrderByDescending(x => x.CreatedDate)
                                    .ToListAsync();

            return View(newsList);
        }

        // =========================
        // NEWS DETAILS
        // =========================
        public async Task<IActionResult> NewsDetails(int id)
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
        // CATEGORY PAGE
        // =========================
        public async Task<IActionResult> Category(string id)
        {
            var categoryNews = await _context.News
                                             .Include(x => x.Category)
                                             .Include(x => x.City)
                                             .Where(x => x.Category!.CategoryName == id)
                                             .OrderByDescending(x => x.CreatedDate)
                                             .ToListAsync();

            ViewBag.Category = id;

            return View(categoryNews);
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