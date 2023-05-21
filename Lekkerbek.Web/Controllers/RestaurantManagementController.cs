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




        // GET: HolidayManagement/Details/5
        public ActionResult CreateRestaurantHolidayDay()
        {
            return View();
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
                var openingsHourList =  _restaurantManagementService.GetAllOpeningsHours();
                bool conflict = false;
                if (restaurantOpeninghours.StartTime < restaurantOpeninghours.EndTime)
                {
                    foreach (var openingHour in openingsHourList)
                    {

                        if (openingHour.DayOfWeek == restaurantOpeninghours.DayOfWeek && (openingHour.StartTime.TimeOfDay < restaurantOpeninghours.EndTime.TimeOfDay && restaurantOpeninghours.EndTime.TimeOfDay < openingHour.EndTime.TimeOfDay) || (openingHour.StartTime.TimeOfDay < restaurantOpeninghours.StartTime.TimeOfDay && restaurantOpeninghours.StartTime.TimeOfDay < openingHour.EndTime.TimeOfDay))
                        {

                            conflict = true;

                        }
                        if (openingHour.EndTime.TimeOfDay < restaurantOpeninghours.EndTime.TimeOfDay && restaurantOpeninghours.StartTime.TimeOfDay < openingHour.StartTime.TimeOfDay)
                        {
                            conflict = true;
                        }

                    }
                    if (conflict)
                    {
                        ModelState.AddModelError("Model", "This time slot is already taken!");
                        ViewBag.oHError = "This time slot is already taken!";
                        return View();
                    }
                    else
                        _restaurantManagementService.CreateOpeningsHour(restaurantOpeninghours);
                    return RedirectToAction(nameof(OpeningsHours));
                }
                else
                {
                    ModelState.AddModelError("Model", "Start time should be earlier than endtime");
                    ViewBag.oHError = "Start time should be earlier than endtime";
                    return View();
                }
              
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
                var openingsHourList = _restaurantManagementService.GetAllOpeningsHours();
                bool conflict = false;
                RestaurantOpeninghours rOH = openingsHourList.Find(o=>o.RestaurantOpeninghoursId==id);
                    openingsHourList.Remove(rOH);
                if (restaurantOpeninghours.StartTime < restaurantOpeninghours.EndTime)
                {
                    foreach (var openingHour in openingsHourList)
                    {

                        if (openingHour.DayOfWeek == restaurantOpeninghours.DayOfWeek && (openingHour.StartTime.TimeOfDay < restaurantOpeninghours.EndTime.TimeOfDay && restaurantOpeninghours.EndTime.TimeOfDay < openingHour.EndTime.TimeOfDay) || (openingHour.StartTime.TimeOfDay < restaurantOpeninghours.StartTime.TimeOfDay && restaurantOpeninghours.StartTime.TimeOfDay < openingHour.EndTime.TimeOfDay))
                        {

                            conflict = true;

                        }
                        if (openingHour.EndTime.TimeOfDay < restaurantOpeninghours.EndTime.TimeOfDay && restaurantOpeninghours.StartTime.TimeOfDay < openingHour.StartTime.TimeOfDay)
                        {
                            conflict = true;
                        }
                    }
                    if (conflict)
                    {
                        ModelState.AddModelError("Model", "This time slot is already taken!");
                        ViewBag.oHError = "This time slot is already taken!";
                        return View();
                    }
                    else
                    {
                        rOH.StartTime = restaurantOpeninghours.StartTime;
                        rOH.EndTime = restaurantOpeninghours.EndTime;
                        rOH.DayOfWeek = restaurantOpeninghours.DayOfWeek;
                        _restaurantManagementService.UpdateOpeningsHour(rOH);
                        return RedirectToAction(nameof(OpeningsHours));
                    }

                }
                else
                {
                    ModelState.AddModelError("Model", "Start time should be earlier than endtime");
                    ViewBag.oHError = "Start time should be earlier than endtime";
                    return View();
                }

            }
            catch
            {
                var entity = _restaurantManagementService.GetSpecificOpeningsHour(id);
                return View(entity);
            }
        }

        
    }
}
