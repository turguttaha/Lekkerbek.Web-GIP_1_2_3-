using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lekkerbek.Web.Controllers
{
    public class RestaurantManagementController : Controller
    {
        private readonly RestaurantManagementService _restaurantManagementService;
        public RestaurantManagementController(RestaurantManagementService restaurantManagementService)
        {
            _restaurantManagementService = restaurantManagementService;
        }
        // GET: RestaurantManagment
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OpeningsHours()
        {
           var list=  _restaurantManagementService.GetAllOpeningsHours();
            return View(list);
        }


        // GET: RestaurantManagment/Create
        public ActionResult ReadOpeningsHours([DataSourceRequest] DataSourceRequest request)
        {
            var list = _restaurantManagementService.GetAllOpeningsHours();
            return Json(list.ToDataSourceResult(request));
        }

        // GET: RestaurantManagment/Details/5
        public ActionResult CreateOpeningsHour()
        {
            return View();
        }

        // POST: RestaurantManagment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOpeningsHour([Bind("DayOfWeek,StartTime,EndTime")] RestaurantOpeninghours restaurantOpeninghours)
        {
            try
            {
                _restaurantManagementService.CreateOpeningsHour(restaurantOpeninghours);
                return RedirectToAction(nameof(OpeningsHours));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult DeleteOpeningsHour([DataSourceRequest] DataSourceRequest request, RestaurantOpeninghours restaurantOpeninghours)

        {
            ModelState.Remove("StartTime"); ModelState.Remove("EndTime");
            if (restaurantOpeninghours != null && ModelState.IsValid)
            {
                _restaurantManagementService.DestroyOpeningsHour(restaurantOpeninghours);
            }
            return Json(new[] { restaurantOpeninghours }.ToDataSourceResult(request, ModelState));
        }

        // GET: RestaurantManagment/Edit/5
        public ActionResult EditOpeningsHour(int id)
        {
           var entity = _restaurantManagementService.GetSpecificOpeningsHour(id);
            return View(entity);
        }

        // POST: RestaurantManagment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOpeningsHour(int id, [Bind("DayOfWeek,StartTime,EndTime")] RestaurantOpeninghours restaurantOpeninghours)
        {
            try
            {
                _restaurantManagementService.UpdateOpeningsHour(restaurantOpeninghours);
                return RedirectToAction(nameof(OpeningsHours));
            }
            catch
            {
                var entity = _restaurantManagementService.GetSpecificOpeningsHour(id);
                return View(entity);
            }
        }












        //HOLIDAY///////////////////
        public ActionResult HolidayDays()
        {
            var list = _restaurantManagementService.GetAllHolidayDays();
            return View(list);
        }


        // GET: RestaurantManagment/Create
        public ActionResult ReadHolidayDays([DataSourceRequest] DataSourceRequest request)
        {
            var list = _restaurantManagementService.GetAllHolidayDays();
            return Json(list.ToDataSourceResult(request));
        }


        // GET: RestaurantManagment/Details/5
        public ActionResult CreateHolidayDay()
        {
            return View();
        }


        // POST: RestaurantManagment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateHolidayDay([Bind("Description,StartDate,EndDate")] RestaurantHoliday restaurantHoliday)
        {
            try
            {
                _restaurantManagementService.CreateHolidayDay(restaurantHoliday);
                return RedirectToAction(nameof(HolidayDays));
            }
            catch
            {
                return View();
            }
        }




        public ActionResult DeleteHolidayDay([DataSourceRequest] DataSourceRequest request, RestaurantHoliday restaurantHoliday)

        {
            ModelState.Remove("StartDate"); ModelState.Remove("EndDate");
            if (restaurantHoliday != null && ModelState.IsValid)
            {
                _restaurantManagementService.DestroyHolidayDay(restaurantHoliday);
            }
            return Json(new[] { restaurantHoliday }.ToDataSourceResult(request, ModelState));
        }



        // GET: RestaurantManagment/Edit/5
        public ActionResult EditHolidayDay(int id)
        {
            var entity = _restaurantManagementService.GetSpecificHolidayDay(id);
            return View(entity);
        }


        // POST: RestaurantManagment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditHolidayDay(int id, [Bind("Description,StartDate,EndDate")] RestaurantHoliday restaurantHoliday)
        {
            try
            {
                _restaurantManagementService.UpdateHolidayDay(restaurantHoliday);
                return RedirectToAction(nameof(HolidayDays));
            }
            catch
            {
                var entity = _restaurantManagementService.GetSpecificHolidayDay(id);
                return View(entity);
            }
        }




    }
}
