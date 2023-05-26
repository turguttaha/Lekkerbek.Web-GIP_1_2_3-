using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lekkerbek.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
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

                        if (openingHour.DayOfWeek == restaurantOpeninghours.DayOfWeek )
                        {
                            if ((openingHour.StartTime.TimeOfDay < restaurantOpeninghours.EndTime.TimeOfDay && restaurantOpeninghours.EndTime.TimeOfDay < openingHour.EndTime.TimeOfDay) || (openingHour.StartTime.TimeOfDay < restaurantOpeninghours.StartTime.TimeOfDay && restaurantOpeninghours.StartTime.TimeOfDay < openingHour.EndTime.TimeOfDay))
                            { conflict = true; }

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
            {  //add here controle
                List<Order> orderListAffterNow = _restaurantManagementService.GetAllOrders(DateTime.Now);
                List<Order> conflictedOrders = new List<Order>();
                foreach (Order order in orderListAffterNow)
                {
                    
                    if(order.TimeSlot.StartTimeSlot.DayOfWeek.ToString() == restaurantOpeninghours.DayOfWeek.ToString())
                    {
                        if (order.TimeSlot.StartTimeSlot.TimeOfDay < restaurantOpeninghours.StartTime.TimeOfDay || order.TimeSlot.StartTimeSlot.TimeOfDay > restaurantOpeninghours.EndTime.TimeOfDay)
                        {
                            conflictedOrders.Add(order);
                        }
                    }
                }
                if(conflictedOrders.Count > 0) 
                {
                    ModelState.AddModelError("Model", "There is a pre-created order out of the selected time range, so you cannot change opening Hours!");
                    ViewBag.oHError = "There is a pre-created order out of the selected time range, so you cannot change opening Hours!";
                    return View();
                }
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
        //chef hollidays
        public ActionResult ChefHoliday()
        {
            var list = _restaurantManagementService.GetAllHolidayDays();
            return View(list);
        }
        public ActionResult ReadChefHoliday([DataSourceRequest] DataSourceRequest request)
        {
            var list = _restaurantManagementService.GetAllWorkerHolidays();
            return Json(list.ToDataSourceResult(request));
        }
        public ActionResult CreateChefHoliday()
        {
            ViewData["ChefId"] = _restaurantManagementService.ChefsSelectList();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateChefHoliday([Bind("Description,ChefId,StartDate,EndDate")] WorkerHoliday workerHoliday)
        {

            //extra check, amount of chefs that are on holliday compared to the total amount of chefs (if its the last chef, dont allow)
            //check how many orders there are, if the amount of orders on a given time range, is equal to the working chefs, then dont allow

            
            
            
            
            try
            {
                bool conflict = false;
                var holidayList = _restaurantManagementService.GetAllHolidaysFromAWorker(workerHoliday.ChefId);

                List<DateTime> tempWorkerHolliday = _restaurantManagementService.GetDateTimeRange(workerHoliday.StartDate, workerHoliday.EndDate);
                List<Order> orderTimeSlots = _restaurantManagementService.GetAllOrders(workerHoliday.StartDate, workerHoliday.EndDate);

                foreach (DateTime dateTime in tempWorkerHolliday)
                {
                    //check amount of chefs
                    int allChefCount = _restaurantManagementService.GetChefs().Count();
                    //check amount of Chefs on holliday
                    int getChefsOnHolliday = _restaurantManagementService.GetAllWokerHolidays(workerHoliday.ChefId).Where(c => (c.StartDate.Date <= workerHoliday.StartDate.Date && c.EndDate.Date >= workerHoliday.StartDate.Date)||(c.StartDate.Date <= workerHoliday.EndDate.Date && c.EndDate.Date >= workerHoliday.EndDate.Date)).Count();
                    //if == 1 => conflict bool
                    if (allChefCount - getChefsOnHolliday == 1)
                    {
                        ModelState.AddModelError("Model", "There are not enough chefs working in this period for this chef to take vacation!");
                        ViewBag.hError = "There are not enough chefs working in this period for this chef to take vacation!";
                        ViewData["ChefId"] = _restaurantManagementService.ChefsSelectList();
                        return View();
                    }

                    foreach (Order order in orderTimeSlots)
                    {

                        if (orderTimeSlots.GroupBy(c => c.TimeSlot.StartTimeSlot).Where(x => x.Count() == allChefCount) != null)
                        {
                            ModelState.AddModelError("Model", "This chef has an order that he needs to prepare during these dates!");
                            ViewBag.hError = "This chef has an order that he needs to prepare during these dates!";
                            ViewData["ChefId"] = _restaurantManagementService.ChefsSelectList();
                            return View();
                        }
                    }

                }


                if (workerHoliday.StartDate < workerHoliday.EndDate)
                {
                    foreach (var holiday in holidayList)
                    {
                        if ((workerHoliday.StartDate < holiday.StartDate && workerHoliday.EndDate < holiday.StartDate) || (workerHoliday.StartDate > holiday.EndDate && workerHoliday.EndDate > holiday.EndDate))
                        {

                        }
                        else
                        {
                            conflict = true;
                        }
                    }
                    if (conflict)
                    {
                        ModelState.AddModelError("Model", "This chef already is already on holliday during part of this time, check that there is no overlap between hollidays!");
                        ViewBag.hError = "This chef already is already on holliday during part of this time, check that there is no overlap between hollidays!";
                        ViewData["ChefId"] = _restaurantManagementService.ChefsSelectList();
                        return View();
                    }
                    else
                        _restaurantManagementService.CreateWorkerHoliday(workerHoliday);
                    return RedirectToAction(nameof(ChefHoliday));
                }
                else
                {
                    ModelState.AddModelError("Model", "Start time should be earlier than endtime");
                    ViewBag.hError = "Start time should be earlier than endtime";
                    ViewData["ChefId"] = _restaurantManagementService.ChefsSelectList();
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }
        public ActionResult DeleteWorkerHoliday([DataSourceRequest] DataSourceRequest request, WorkerHoliday workerHoliday)

        {
            ModelState.Remove("StartDate"); ModelState.Remove("EndDate");
            if (workerHoliday != null && ModelState.IsValid)
            {
                _restaurantManagementService.DestroyWorkerHoliday(workerHoliday);
            }
            return Json(new[] { workerHoliday }.ToDataSourceResult(request, ModelState));
        }



        // GET: RestaurantManagment/Edit/5
        public ActionResult EditWorkerHoliday(int id)
        {
            var entity = _restaurantManagementService.GetSpecificWorkerHoliday(id);
            return View(entity);
        }


        // POST: RestaurantManagment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditWorkerHoliday(int id, [Bind("WorkerHolidayId,ChefId,Description,StartDate,EndDate")] WorkerHoliday workerHoliday)
        {
            try
            {
                var allWorkersHolidayList = _restaurantManagementService.GetAllWorkerHolidays();
                WorkerHoliday workerHoliday1 = allWorkersHolidayList.Find(h => h.WorkerHolidayId == id);
                allWorkersHolidayList.Remove(workerHoliday1);
                bool conflict = false;
                var holidayList = _restaurantManagementService.GetAllHolidaysFromAWorker(workerHoliday.ChefId);

                List<DateTime> tempWorkerHolliday = _restaurantManagementService.GetDateTimeRange(workerHoliday.StartDate, workerHoliday.EndDate);
                List<Order> orderTimeSlots = _restaurantManagementService.GetAllOrders(workerHoliday.StartDate, workerHoliday.EndDate);
                
                foreach(DateTime dateTime in tempWorkerHolliday)
                {
                    //check amount of chefs
                    int allChefCount = _restaurantManagementService.GetChefs().Count();
                    //check amount of Chefs on holliday
                    int getChefsOnHolliday = allWorkersHolidayList.Where(c => c.StartDate <= workerHoliday.StartDate && c.EndDate >= workerHoliday.StartDate).Count();
                    //if == 1 => conflict bool
                    if (allChefCount - getChefsOnHolliday == 0)
                    {
                        ModelState.AddModelError("Model", "There are not enough chefs working in this period for this chef to take vacation!");
                        ViewBag.hError = "There are not enough chefs working in this period for this chef to take vacation!";
                        ViewData["ChefId"] = _restaurantManagementService.ChefsSelectList();
                        return View();
                    }

                    foreach (Order order in orderTimeSlots)
                    {

                        if (orderTimeSlots.GroupBy(c => c.TimeSlot.StartTimeSlot).Where(x => x.Count() == allChefCount) != null)
                        {
                            ModelState.AddModelError("Model", "This chef has an order that he needs to prepare during these dates!");
                            ViewBag.hError = "This chef has an order that he needs to prepare during these dates!";
                            ViewData["ChefId"] = _restaurantManagementService.ChefsSelectList();
                            return View();
                        }
                    }


                    
                }


                if (workerHoliday.StartDate < workerHoliday.EndDate)
                {
                    allWorkersHolidayList = allWorkersHolidayList.Where(c => c.ChefId == workerHoliday.ChefId).ToList();
                    foreach (var holiday in allWorkersHolidayList)
                    {

                        if ((workerHoliday.StartDate < holiday.StartDate && workerHoliday.EndDate < holiday.StartDate) || (workerHoliday.StartDate > holiday.EndDate && workerHoliday.EndDate > holiday.EndDate))
                        {

                        }
                        else
                        {
                            conflict = true;
                        }

                    }
                    if (conflict)
                    {
                        ModelState.AddModelError("Model", "This chef already is already on holliday during part of this time, check that there is no overlap between hollidays!");
                        ViewBag.hError = "This chef already is already on holliday during part of this time, check that there is no overlap between hollidays!";
                        return View();
                    }
                    else
                        workerHoliday1.StartDate = workerHoliday.StartDate;
                    workerHoliday1.EndDate = workerHoliday.EndDate;
                    workerHoliday1.Description = workerHoliday.Description;
                    _restaurantManagementService.UpdateWorkerHoliday(workerHoliday1);
                    return RedirectToAction(nameof(ChefHoliday));
                }
                else
                {
                    ModelState.AddModelError("Model", "Start time should be earlier than endtime");
                    ViewBag.hError = "Start time should be earlier than endtime";
                    return View();
                }
                /////////////
                //_restaurantManagementService.UpdateHolidayDay(workerHoliday);
                return RedirectToAction(nameof(ChefHoliday));
            }
            catch
            {
                
                var entity = _restaurantManagementService.GetSpecificHolidayDay(id);
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
                List<Order> orderTimeSlots = _restaurantManagementService.GetAllOrders(restaurantHoliday.StartDate, restaurantHoliday.EndDate);
                if(orderTimeSlots.Count > 0)
                {
                    ModelState.AddModelError("Model", "There is a pre-created order in the selected time range, so you cannot plan a holiday within this date range!");
                    ViewBag.hError = "There is a pre-created order in the selected time range, so you cannot plan a holiday within this date range!";
                    return View();
                }
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
                List<Order> orderTimeSlots = _restaurantManagementService.GetAllOrders(restaurantHoliday.StartDate, restaurantHoliday.EndDate);
                if (orderTimeSlots.Count > 0)
                {
                    ModelState.AddModelError("Model", "There is a pre-created order in the selected time range, so you cannot plan a holiday within this date range!");
                    ViewBag.hError = "There is a pre-created order in the selected time range, so you cannot plan a holiday within this date range!";
                    return View();
                }
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
