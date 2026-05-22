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
        public IActionResult Index()
        {
            var newsList = _context.News
                                   .OrderByDescending(x => x.CreatedDate)
                                   .ToList();

            return View(newsList);
        }

        // =========================
        // NEWS DETAILS
        // =========================
        public IActionResult NewsDetails(int id)
        {
            var news = _context.News
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