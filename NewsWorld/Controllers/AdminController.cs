using Microsoft.AspNetCore.Mvc;
using NewsWorld.Data;
using X.PagedList;
using X.PagedList.Extensions;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> Login(string Username, string Password)
        {
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Username == Username && a.Password == Password);

            if (admin != null)
            {
                TempData["loginSuccess"] = "Login Successful";
                HttpContext.Session.SetString("AdminUser", admin.Username);
                return RedirectToAction("Dashboard", "Admin");
            }

            ViewBag.Error = "Invalid Username or Password";
            return View();
        }

        //If the account has been deleted, clear the session and redirect to login.
        public async Task<IActionResult> Dashboard()
        {
            var user = HttpContext.Session.GetString("AdminUser");

            if (string.IsNullOrEmpty(user))
            {
                return RedirectToAction("Login", "Admin");
            }

            // Check whether the admin still exists
            var adminExists = await _context.Admins.AnyAsync(a => a.Username == user);

            if (!adminExists)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Admin");
            }

            ViewBag.CategoryCount = await _context.Categories.CountAsync();
            ViewBag.CityCount = await _context.Cities.CountAsync();
            ViewBag.NewsCount = await _context.News.CountAsync();
            ViewBag.AdminCount = await _context.Admins.CountAsync();

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

        // Admin Users List
        public IActionResult Users(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var admins = _context.Admins
                                 .OrderBy(a => a.Id)
                                 .ToPagedList(pageNumber, pageSize);

            return View(admins);
        }

        // GET: AdminUser Add User
        public IActionResult AddUser()
        {
            return View();
        }

        // POST: AdminUser Add User
        [HttpPost]
        public async Task<IActionResult> AddUser(string Username, string Password)
        {
            var existingUser = await _context.Admins
                                 .FirstOrDefaultAsync(x => x.Username == Username);

            if (existingUser != null)
            {
                ViewBag.Error = "Username already exists.";
                return View();
            }

            var admin = new Admin
            {
                Username = Username,
                Password = Password 
            };

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            TempData["adminSuccess"] = "Admin added successfully.";

            return RedirectToAction("Users");
        }

        // Edit Admin User

        public IActionResult EditAdmin(int id)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.Id == id);

            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        [HttpPost]
        public async Task<IActionResult> EditAdmin(Admin model)
        {
            var admin = await _context.Admins
                          .FirstOrDefaultAsync(a => a.Id == model.Id);

            if (admin == null)
            {
                return NotFound();
            }

            // Check for duplicate username
            bool exists = await _context.Admins.AnyAsync(a =>
                            a.Username == model.Username &&
                            a.Id != model.Id);

            if (exists)
            {
                ViewBag.Error = "Username already exists.";
                return View(model);
            }

            string oldUsername = admin.Username;

            admin.Username = model.Username;

            await _context.SaveChangesAsync();

            var currentUser = HttpContext.Session.GetString("AdminUser");

            // If the logged-in admin changed their own username
            if (currentUser == oldUsername)
            {
                HttpContext.Session.SetString("AdminUser", model.Username);
            }

            TempData["Success"] = "Admin updated successfully.";

            return RedirectToAction("Users");
        }

        //Delete Admin User
        [HttpPost]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var admin = await _context.Admins
                          .FirstOrDefaultAsync(x => x.Id == id);

            if (admin == null)
                return NotFound();

            // Prevent deleting the last admin account
            if (await _context.Admins.CountAsync() == 1)
            {
                TempData["Error"] = "Cannot delete the last administrator account.";
                return RedirectToAction("Users");
            }   

            var currentUser = HttpContext.Session.GetString("AdminUser");

            bool deletingCurrentUser = admin.Username == currentUser;

            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();

            if (deletingCurrentUser)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            TempData["Success"] = "Admin deleted successfully.";
            return RedirectToAction("Users");
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