using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWorld.Data;
using NewsWorld.Models;
using System.Linq;
using X.PagedList;
using X.PagedList.Extensions;

namespace NewsWorld.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Admin/Login
        [HttpPost]
        public IActionResult Login(string Username, string Password)
        {
            var admin = _context.Admins
                .FirstOrDefault(a => a.Username == Username && a.Password == Password);

            if (admin != null)
            {
                TempData["loginSuccess"] = "Login Successful";
                HttpContext.Session.SetString("AdminUser", admin.Username);
                return RedirectToAction("Dashboard", "Admin");
            }

            ViewBag.Error = "Invalid Username or Password";
            return View();
        }
        public IActionResult Dashboard()
        {
            ViewBag.CategoryCount = _context.Categories.Count();

            ViewBag.CityCount = _context.Cities.Count();

            ViewBag.NewsCount = _context.News.Count();

            ViewBag.AdminCount = _context.Admins.Count();

            var user = HttpContext.Session.GetString("AdminUser");

            if (user == null)
            {
                // Not logged in → go to login
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }

        public IActionResult Index()
        {
            var adminUser = HttpContext.Session.GetString("AdminUser");

            if (adminUser != null)
            {
                return RedirectToAction("Dashboard");
            }

            return RedirectToAction("Login");
        }

        // GET: AdminUser
        public IActionResult AddUser()
        {
            return View();
        }

        // POST: AdminUser
        [HttpPost]
        public IActionResult AddUser(string Username, string Password)
        {
            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
            {
                var admin = new Admin
                {
                    Username = Username,
                    Password = Password
                };

                _context.Admins.Add(admin);
                _context.SaveChanges();

                TempData["adminSuccess"] = "Admin User Created Successfully";

                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "All fields are required";
            return View();
        }


        // Logout
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}