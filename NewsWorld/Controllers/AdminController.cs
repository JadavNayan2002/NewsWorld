using Microsoft.AspNetCore.Mvc;
using NewsWorld.Data;       
using NewsWorld.Models;     
using System.Linq;

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

        // ✅ POST: Admin/Login
        [HttpPost]
        public IActionResult Login(string Username, string Password)
        {
            var admin = _context.Admins
                .FirstOrDefault(a => a.Username == Username && a.Password == Password);

            if (admin != null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid Username or Password";
            return View();
        }
    }
}