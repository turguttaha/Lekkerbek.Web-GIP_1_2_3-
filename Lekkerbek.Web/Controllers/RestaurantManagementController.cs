using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Lekkerbek.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class RestaurantManagementController : Controller
    {
        private readonly IRestaurantManagementService _restaurantManagementService;
        public RestaurantManagementController(IRestaurantManagementService restaurantManagementService)
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
            if (ModelState.IsValid) { 
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
                            if ((openingHour.StartTime.TimeOfDay <= restaurantOpeninghours.EndTime.TimeOfDay && restaurantOpeninghours.EndTime.TimeOfDay < openingHour.EndTime.TimeOfDay) || (openingHour.StartTime.TimeOfDay < restaurantOpeninghours.StartTime.TimeOfDay && restaurantOpeninghours.StartTime.TimeOfDay <= openingHour.EndTime.TimeOfDay))
                            { 
                                conflict = true; 
                            }

                        }
                        if (openingHour.EndTime.TimeOfDay < restaurantOpeninghours.EndTime.TimeOfDay && restaurantOpeninghours.StartTime.TimeOfDay < openingHour.StartTime.TimeOfDay)
                        {
                            conflict = true;
                        }

                    }
                    if (conflict)
                    {
                        ModelState.AddModelError("Model", "Dit tijdslot is al bezet!");
                        ViewBag.oHError = "Dit tijdslot is al bezet!";
                        return View();
                    }
                    else
                        _restaurantManagementService.CreateOpeningsHour(restaurantOpeninghours);
                    return RedirectToAction(nameof(OpeningsHours));
                }
                else
                {
                    ModelState.AddModelError("Model", "Beginuur moet eerder zijn dan einduur");
                    ViewBag.oHError = "Beginuur moet eerder zijn dan einduur";
                    return View();
                }
              
            }
            catch
            {
                return View();
            }
            }
            return View(restaurantOpeninghours);
        }

        public ActionResult DeleteOpeningsHour([DataSourceRequest] DataSourceRequest request, RestaurantOpeninghours restaurantOpeninghours)
        {
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
            if (ModelState.IsValid) { 
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
                    ModelState.AddModelError("Model", "Er is een order op een op het moment dat u wilt wijzigen in de toekomst, de actie kan niet voltooid worden!");
                    ViewBag.oHError = "Er is een order op een op het moment dat u wilt wijzigen in de toekomst, de actie kan niet voltooid worden!";
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

                        if (openingHour.DayOfWeek == restaurantOpeninghours.DayOfWeek && (openingHour.StartTime.TimeOfDay < restaurantOpeninghours.EndTime.TimeOfDay && restaurantOpeninghours.EndTime.TimeOfDay < openingHour.EndTime.TimeOfDay) || (openingHour.DayOfWeek == restaurantOpeninghours.DayOfWeek && openingHour.StartTime.TimeOfDay < restaurantOpeninghours.StartTime.TimeOfDay && restaurantOpeninghours.StartTime.TimeOfDay < openingHour.EndTime.TimeOfDay))
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
                        ModelState.AddModelError("Model", "Dit tijdslot is al ingenomen!");
                        ViewBag.oHError = "Dit tijdslot is al ingenomen!";
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
                    ModelState.AddModelError("Model", "Startuur moet eerder zijn dan einduur");
                    ViewBag.oHError = "Startuur moet eerder zijn dan einduur";
                    return View();
                }

            }
            catch
            {
                var entity = _restaurantManagementService.GetSpecificOpeningsHour(id);
                return View(entity);
            }
            }
            return View(restaurantOpeninghours);
        }
        

        //////////////////////////////chef hollidays///////////////////////////////////////////
        public ActionResult ChefHoliday()
        {         
            return View();
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
            if (ModelState.IsValid) { 
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
                        ModelState.AddModelError("Model", "Er zijn niet genoeg chefs, deze chef mag geen vakantie nemen!");
                        ViewBag.hError = "Er zijn niet genoeg chefs, deze chef mag geen vakantie nemen!";
                        ViewData["ChefId"] = _restaurantManagementService.ChefsSelectList();
                        return View();
                    }

                    foreach (Order order in orderTimeSlots)
                    {

                        if (!orderTimeSlots.GroupBy(c => c.TimeSlot.StartTimeSlot).Where(x => x.Count() == allChefCount).IsNullOrEmpty())
                        {
                                var a = orderTimeSlots.GroupBy(c => c.TimeSlot.StartTimeSlot).Where(x => x.Count() == allChefCount);
                            ModelState.AddModelError("Model", "Deze chef moet een bestelling klaarmaken tijdens deze periode!");
                            ViewBag.hError = "Deze chef moet een bestelling klaarmaken tijdens deze periode!";
                            ViewData["ChefId"] = _restaurantManagementService.ChefsSelectList();
                            return View();
                        }
                    }

                }


                if (workerHoliday.StartDate.Date <= workerHoliday.EndDate.Date)
                {
                    foreach (var holiday in holidayList)
                    {
                        if ((workerHoliday.StartDate.Date < holiday.StartDate.Date && workerHoliday.EndDate.Date < holiday.StartDate.Date) || (workerHoliday.StartDate.Date > holiday.EndDate.Date && workerHoliday.EndDate.Date > holiday.EndDate.Date))
                        {

                        }
                        else
                        {
                            conflict = true;
                        }
                    }
                    if (conflict)
                    {
                        ModelState.AddModelError("Model", "De chef heeft al vakantie opgenomen tijdens deze periode, check dat er geen overlap is met bestaande vakantiedagen!");
                        ViewBag.hError = "De chef heeft al vakantie opgenomen tijdens deze periode, check dat er geen overlap is met bestaande vakantiedagen!";
                        ViewData["ChefId"] = _restaurantManagementService.ChefsSelectList();
                        return View();
                    }
                    else
                        _restaurantManagementService.CreateWorkerHoliday(workerHoliday);
                    return RedirectToAction(nameof(ChefHoliday));
                }
                else
                {
                    ModelState.AddModelError("Model", "Startuur moet eerder zijn dan einduur");
                    ViewBag.hError = "Startuur moet eerder zijn dan einduur";
                    ViewData["ChefId"] = _restaurantManagementService.ChefsSelectList();
                    return View();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return View();
            }

            }

            ViewData["ChefId"] = _restaurantManagementService.ChefsSelectList();
            return View(workerHoliday);

        }
        public ActionResult DeleteWorkerHoliday([DataSourceRequest] DataSourceRequest request, WorkerHoliday workerHoliday)
        {
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
            if (ModelState.IsValid) { 
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
                    int getChefsOnHolliday = allWorkersHolidayList.Where(c => c.StartDate <= workerHoliday.StartDate.Date && c.EndDate >= workerHoliday.StartDate.Date).Count();
                    //if == 1 => conflict bool
                    if (allChefCount - getChefsOnHolliday == 1)
                    {
                        ModelState.AddModelError("Model", "Er zijn niet genoeg chefs, deze chef mag geen vakantie nemen!");
                        ViewBag.hError = "Er zijn niet genoeg chefs, deze chef mag geen vakantie nemen!";
                        ViewData["ChefId"] = _restaurantManagementService.ChefsSelectList();
                        return View();
                    }

                    foreach (Order order in orderTimeSlots)
                    {

                        if (orderTimeSlots.GroupBy(c => c.TimeSlot.StartTimeSlot).Where(x => x.Count() == allChefCount) != null)
                        {
                            ModelState.AddModelError("Model", "Deze chef moet een bestelling klaarmaken tijdens deze periode!");
                            ViewBag.hError = "Deze chef moet een bestelling klaarmaken tijdens deze periode!";
                            ViewData["ChefId"] = _restaurantManagementService.ChefsSelectList();
                            return View();
                        }
                    }


                    
                }


                if (workerHoliday.StartDate <= workerHoliday.EndDate)
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
                        ModelState.AddModelError("Model", "De chef heeft al vakantie opgenomen tijdens deze periode, check dat er geen overlap is met bestaande vakantiedagen!");
                        ViewBag.hError = "De chef heeft al vakantie opgenomen tijdens deze periode, check dat er geen overlap is met bestaande vakantiedagen!";
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
                    ModelState.AddModelError("Model", "Startuur moet eerder zijn dan einduur");
                    ViewBag.hError = "Startuur moet eerder zijn dan einduur";
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

            return View(workerHoliday);
        }

        //HOLIDAY///////////////////
        public ActionResult HolidayDays()
        {
            return View();
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
            if (ModelState.IsValid) { 
            try
            {
                List<Order> orderTimeSlots = _restaurantManagementService.GetAllOrders(restaurantHoliday.StartDate, restaurantHoliday.EndDate);
                if(orderTimeSlots.Count > 0)
                {
                    ModelState.AddModelError("Model", "Er staan open bestellingen tijdens de periode die u wilt wijzigen!");
                        TempData["hError"] = "Er staan open bestellingen tijdens de periode die u wilt wijzigen!";
                    return View();
                }
                var holidayList = _restaurantManagementService.GetAllHolidayDays();
                bool conflict = false;
                if (restaurantHoliday.StartDate <= restaurantHoliday.EndDate)
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
                        ModelState.AddModelError("Model", "Dit tijdslot is al in gebruik!");
                        TempData["hError"] = "Dit tijdslot is al in gebruik!";
                        return View();
                    }
                    else
                        _restaurantManagementService.CreateHolidayDay(restaurantHoliday);
                    return RedirectToAction(nameof(HolidayDays));
                }
                else
                {
                    ModelState.AddModelError("Model", "Startuur moet eerder zijn dan einduur");
                        TempData["hError"] = "Startuur moet eerder zijn dan einduur";
                    return View();
                }
            }
            catch
            {
                return View();
            }

            }
            return View(restaurantHoliday);
        }

        public ActionResult DeleteHolidayDay([DataSourceRequest] DataSourceRequest request, RestaurantHoliday restaurantHoliday)
        {
            //ModelState.Remove("StartDate"); ModelState.Remove("EndDate");
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
            if(ModelState.IsValid) { 
            try
            {
                List<Order> orderTimeSlots = _restaurantManagementService.GetAllOrders(restaurantHoliday.StartDate, restaurantHoliday.EndDate);
                if (orderTimeSlots.Count > 0)
                {
                    ModelState.AddModelError("Model", "Er staan open bestellingen tijdens de periode die u wilt wijzigen!");
                    ViewBag.hError = "Er staan open bestellingen tijdens de periode die u wilt wijzigen!";
                    return View();
                }
                var holidayList = _restaurantManagementService.GetAllHolidayDays();
                RestaurantHoliday restaurantHoliday1 = holidayList.Find(h=>h.RestaurantHolidayId == id);
                holidayList.Remove(restaurantHoliday1);
                bool conflict = false;
                if (restaurantHoliday.StartDate <= restaurantHoliday.EndDate)
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
                        ModelState.AddModelError("Model", "Dit tijdslot is al in gebruik!");
                        ViewBag.hError = "Dit tijdslot is al in gebruik!";
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
                    ModelState.AddModelError("Model", "Startuur moet eerder zijn dan einduur");
                    ViewBag.hError = "Startuur moet eerder zijn dan einduur";
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
            return View(restaurantHoliday);
        }




    }
}
