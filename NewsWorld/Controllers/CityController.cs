using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWorld.Data;
using NewsWorld.Models;
using X.PagedList.Extensions;

namespace NewsWorld.Controllers
{
    public class CityController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================== LIST =====================
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var cities = await _context.Cities
                .OrderByDescending(x => x.CityId)
                .ToListAsync();

            return View(cities.ToPagedList(pageNumber, pageSize));
        }

        // ===================== CREATE GET =====================
        [HttpGet]
        public IActionResult CreateCity()
        {
            return View();
        }

        // ===================== CREATE POST =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCity(City city)
        {
            if (ModelState.IsValid)
            {
                // Check duplicate city
                bool isExists = _context.Cities
                    .Any(x => x.CityName.ToLower() == city.CityName.ToLower());

                if (isExists)
                {
                    TempData["error"] = "City already exists!";
                    return View(city);
                }

                _context.Cities.Add(city);
                _context.SaveChanges();

                TempData["success"] = "City added successfully";

                return RedirectToAction("Index");
            }

            return View(city);
        }

        // ===================== EDIT GET =====================
        [HttpGet]
        public async Task<IActionResult> EditCity(int id)
        {
            var city = await _context.Cities.FindAsync(id);

            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        // ===================== EDIT POST =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCity(int id, City city)
        {
            if (id != city.CityId)
            {
                return NotFound();
            }

            bool isExists = await _context.Cities.AnyAsync(x =>
                x.CityName.Trim().ToLower() ==
                city.CityName.Trim().ToLower()
                && x.CityId != city.CityId);

            if (isExists)
            {
                ModelState.AddModelError("CityName",
                    "This City already exists.");
            }

            if (ModelState.IsValid)
            {
                _context.Cities.Update(city);

                await _context.SaveChangesAsync();

                TempData["success"] = "City Updated Successfully";

                return RedirectToAction(nameof(Index));
            }

            return View(city);
        }

        // ===================== DELETE =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var city = await _context.Cities.FindAsync(id);

            if (city != null)
            {
                _context.Cities.Remove(city);

                await _context.SaveChangesAsync();

                TempData["success"] = "City Deleted Successfully";
            }

            return RedirectToAction(nameof(Index));
        }

        // ===================== TOGGLE STATUS =====================
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var city = await _context.Cities.FindAsync(id);

            if (city == null)
            {
                return NotFound();
            }

            city.IsActive = !city.IsActive;

            await _context.SaveChangesAsync();

            //TempData["success"] = "City Status Updated";

            return RedirectToAction(nameof(Index));
        }
    }
}