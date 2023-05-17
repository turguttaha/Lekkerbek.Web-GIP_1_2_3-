using Lekkerbek.Web.Models;
using Lekkerbek.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lekkerbek.Web.Controllers
{
    public class HolidayManagementController : Controller
    {
        private readonly HolidayManagementService _holidayManagementService;
        public HolidayManagementController(HolidayManagementService holidayManagementService)
        {
            _holidayManagementService = holidayManagementService;
        }
        // GET: HolidayManagement
        public ActionResult Index()
        {
            return View();
        }

        // GET: HolidayManagement/Details/5
        public ActionResult CreateRestaurantHolidayDay()
        {
            return View();
        }

        // GET: HolidayManagment/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HolidayManagment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateHolidayDay([Bind("RestaurantHolidayId,StartDate,EndDate,Description")] RestaurantHolidayDays restaurantHolidayDays)
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


        // GET: HolidayManagment/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HolidayManagment/Edit/5
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

        // GET: HolidayManagment/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HolidayManagment/Delete/5
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
