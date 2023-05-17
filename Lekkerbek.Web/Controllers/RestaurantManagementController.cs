using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lekkerbek.Web.Controllers
{
    public class RestaurantManagementController : Controller
    {
        // GET: RestaurantManagment
        public ActionResult Index()
        {
            return View();
        }

        // GET: RestaurantManagment/Details/5
        public ActionResult CreateOpeningsHour()
        {
            return View();
        }

        // GET: RestaurantManagment/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RestaurantManagment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: RestaurantManagment/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RestaurantManagment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: RestaurantManagment/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RestaurantManagment/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
