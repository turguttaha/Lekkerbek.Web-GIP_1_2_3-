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

        //OpeningsHour///////////////////
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
                var holidayList = _restaurantManagementService.GetAllHolidayDays();
                bool conflict = false;
                if (restaurantHoliday.StartDate < restaurantHoliday.EndDate)
                {
                    foreach (var holiday in holidayList)
                    {

                        if ((restaurantHoliday.StartDate<holiday.StartDate&&restaurantHoliday.EndDate<holiday.StartDate)||(restaurantHoliday.StartDate>holiday.EndDate&&restaurantHoliday.EndDate>holiday.EndDate))
                        {

                        }
                       else
                        {
                            conflict = true;
                        }

                    }
                    if (conflict)
                    {
                        ModelState.AddModelError("Model", "This time slot is already taken!");
                        ViewBag.hError = "This time slot is already taken!";
                        return View();
                    }
                    else
                        _restaurantManagementService.CreateHolidayDay(restaurantHoliday);
                    return RedirectToAction(nameof(HolidayDays));
                }
                else
                {
                    ModelState.AddModelError("Model", "Start time should be earlier than endtime");
                    ViewBag.hError = "Start time should be earlier than endtime";
                    return View();
                }
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
        public ActionResult EditHolidayDay(int id, [Bind("RestaurantHolidayId,Description,StartDate,EndDate")] RestaurantHoliday restaurantHoliday)
        {
            try
            {
                var holidayList = _restaurantManagementService.GetAllHolidayDays();
                RestaurantHoliday restaurantHoliday1 = holidayList.Find(h=>h.RestaurantHolidayId == id);
                holidayList.Remove(restaurantHoliday1);
                bool conflict = false;
                if (restaurantHoliday.StartDate < restaurantHoliday.EndDate)
                {
                    foreach (var holiday in holidayList)
                    {

                        if ((restaurantHoliday.StartDate < holiday.StartDate && restaurantHoliday.EndDate < holiday.StartDate) || (restaurantHoliday.StartDate > holiday.EndDate && restaurantHoliday.EndDate > holiday.EndDate))
                        {

                        }
                        else
                        {
                            conflict = true;
                        }

                    }
                    if (conflict)
                    {
                        ModelState.AddModelError("Model", "This time slot is already taken!");
                        ViewBag.hError = "This time slot is already taken!";
                        return View();
                    }
                    else
                        restaurantHoliday1.StartDate = restaurantHoliday.StartDate;
                        restaurantHoliday1.EndDate = restaurantHoliday.EndDate;
                        restaurantHoliday1.Description = restaurantHoliday.Description;
                        _restaurantManagementService.UpdateHolidayDay(restaurantHoliday1);
                    return RedirectToAction(nameof(HolidayDays));
                }
                else
                {
                    ModelState.AddModelError("Model", "Start time should be earlier than endtime");
                    ViewBag.hError = "Start time should be earlier than endtime";
                    return View();
                }
                /////////////
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
