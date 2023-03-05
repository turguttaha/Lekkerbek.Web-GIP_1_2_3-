using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lekkerbek.Web.Controllers
{
    public class DishController : Controller
    {
        private readonly LekkerbekContext _context;

        public DishController(LekkerbekContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(_context.Dishes);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                Dish dish = new Dish();
                dish.Name = collection["Name"];
                dish.Description = collection["Description"];
                dish.Price = int.Parse(collection["Price"]);
                dish.Discount = int.Parse(collection["Discount"]);
                _context.Dishes.Add(dish);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            Dish dish = _context.Dishes.Find(id);
            return View(dish);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                Dish dish = _context.Dishes.Find(id);
                dish.Name = collection["Name"];
                dish.Description = collection["Description"];
                dish.Price = int.Parse(collection["Price"]);
                dish.Discount = int.Parse(collection["Discount"]);
                _context.Dishes.Update(dish);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
