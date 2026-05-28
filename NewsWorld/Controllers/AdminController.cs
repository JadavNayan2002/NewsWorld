using Microsoft.AspNetCore.Mvc;
using NewsWorld.Data;
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

        //If the account has been deleted, clear the session and redirect to login.
        public IActionResult Dashboard()
        {
            var user = HttpContext.Session.GetString("AdminUser");

            if (string.IsNullOrEmpty(user))
            {
                return RedirectToAction("Login", "Admin");
            }

            // Check whether the admin still exists
            var adminExists = _context.Admins.Any(a => a.Username == user);

            if (!adminExists)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Admin");
            }

            ViewBag.CategoryCount = _context.Categories.Count();
            ViewBag.CityCount = _context.Cities.Count();
            ViewBag.NewsCount = _context.News.Count();
            ViewBag.AdminCount = _context.Admins.Count();

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

        // GET: AdminUser Add User
        public IActionResult AddUser()
        {
            return View();
        }

        // POST: AdminUser Add User
        [HttpPost]
        public IActionResult AddUser(string Username, string Password)
        {
            // Check if username already exists
            var existingUser = _context.Admins
                                       .FirstOrDefault(x => x.Username == Username);

            if (existingUser != null)
            {
                ViewBag.Error = "Username already exists.";
                return View();
            }

            var admin = new Admin
            {
                Username = Username,
                Password = Password // Hash password in production
            };

            _context.Admins.Add(admin);
            _context.SaveChanges();

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
        public IActionResult EditAdmin(Admin model)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.Id == model.Id);

            if (admin == null)
            {
                return NotFound();
            }

            // Check for duplicate username
            bool exists = _context.Admins.Any(a =>
                a.Username == model.Username &&
                a.Id != model.Id);

            if (exists)
            {
                ViewBag.Error = "Username already exists.";
                return View(model);
            }

            string oldUsername = admin.Username;

            admin.Username = model.Username;

            _context.SaveChanges();

            var currentUser = HttpContext.Session.GetString("AdminUser");

            // If the logged-in admin changed their own username
            if (currentUser == oldUsername)
            {
                HttpContext.Session.SetString("AdminUser", model.Username);
            }

            TempData["Success"] = "Admin updated successfully.";

            return RedirectToAction("Users");
        }


        // Logout
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // Admin Users List
        public IActionResult Users()
        {
            var admins = _context.Admins.ToList();
            return View(admins);
        }

        //Delete Admin User
        [HttpPost]
        public IActionResult DeleteAdmin(int id)
        {
            var admin = _context.Admins.FirstOrDefault(x => x.Id == id);

            if (admin == null)
                return NotFound();

            // Prevent deleting the last admin account
            if (_context.Admins.Count() == 1)
            {
                TempData["Error"] = "Cannot delete the last administrator account.";
                return RedirectToAction("Users");
            }   

            var currentUser = HttpContext.Session.GetString("AdminUser");

            bool deletingCurrentUser = admin.Username == currentUser;

            _context.Admins.Remove(admin);
            _context.SaveChanges();

            if (deletingCurrentUser)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            TempData["Success"] = "Admin deleted successfully.";
            return RedirectToAction("Users");
        }

    }
}