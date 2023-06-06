using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Microsoft.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using Lekkerbek.Web.Services;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Lekkerbek.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: Orders

        //Read func for Kendo
        public IActionResult Index()
        {
            return View();
        }
        public ActionResult EditingPopup_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_orderService.GetOrderViewModels().ToDataSourceResult(request));
        }

        // GET: OrderLines
        public ActionResult DetailTemplate_HierarchyBinding_Orderline(int orderID, [DataSourceRequest] DataSourceRequest request)
        {
            return Json(_orderService.GetOrderLines()
                .Where(orderline => orderline.OrderID == orderID)
                .ToDataSourceResult(request));
        }

        // !!!!Creating Order Starts here!!!!
        // GET: Orders/Create
        public IActionResult SelectCustomer()
        {

            
            ViewBag.CustomerId = _orderService.CustomerSelectList();
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectCustomer(IFormCollection collection)
        {
            
            TempData["SelectedCustomerId"] = int.Parse(collection["CustomerID"]);
            
            return RedirectToAction("SelectTimeSlot", "Orders");

        }
        public IActionResult SelectTimeSlot()
        {
            //get datetime of today for first check instead of hardcoded value
            DateTime timeSlotDateAndTime = DateTime.Now;
            ViewBag.TimeSlotsSelectList = _orderService.GetTimeDropDownList(timeSlotDateAndTime);
            return View();
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SelectTimeSlot(IFormCollection collection)
        {
            string x = collection["TimeSlotsSelectList"];
            string selectedDate = collection["StartTimeSlot"] + " " + x;
            
            DateTime timeSlotDateAndTime = Convert.ToDateTime(selectedDate);
            TempData["SelectedDateTime"] = timeSlotDateAndTime;
            
            return RedirectToAction("AddOrderLine", "Orders");
        }
        public async Task<JsonResult> LookUpChefs(string date)
            {

            //get datetime of today for first check instead of hardcoded value
            DateTime timeSlotDateAndTime = Convert.ToDateTime(date + " 00:00");
            ViewBag.TimeSlotsSelectList = _orderService.GetTimeDropDownList(timeSlotDateAndTime);
            foreach (SelectListItem item in _orderService.GetTimeDropDownList(timeSlotDateAndTime)) 
            {
                Console.WriteLine(item.Value);
            }

            return Json(new { timeSlots = ViewBag.TimeSlotsSelectList });
        }

        // GET: OrderLines/Create
        public IActionResult AddOrderLine()
        {
            ViewData["MenuItemId"] = _orderService.MenuItemSelectList();
            ViewBag.TemproraryCart = Order.TemproraryCart;
            return View();
        }

        // POST: OrderLines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrderLine([Bind("OrderLineID,ExtraDetails,DishAmount,OrderID,MenuItemId")] OrderLine orderLine)
        {
            if (ModelState.IsValid) 
            { 
            // go to database and get dish name via id
            orderLine.MenuItem = _orderService.GetSpecificMenuItem(orderLine.MenuItemId);
            Order.TemproraryCart.Add(orderLine);
            
            ViewData["Message"] = "Het item werdt toegevoegd";

            ViewBag.TemproraryCart = Order.TemproraryCart;
            ViewData["MenuItemId"] = _orderService.MenuItemSelectList();

            }
            return View();


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteOrder()
        {
            //TimeSlot Object aanmaken

            TimeSlot timeSlot = new TimeSlot();
            timeSlot.StartTimeSlot = (DateTime)TempData["SelectedDateTime"];
            //timeSlot.ChefId =(int)TempData["SelectedChef"];

            Order order = new Order();
            order.CustomerId = (int)TempData["SelectedCustomerId"];

            _orderService.CreateOrder(timeSlot, order);

            return RedirectToAction("index", "Orders");
        }

        // Completed order right here------------------------


        // GET: Orders/EditOrder/5
        public async Task<IActionResult> EditOrder(int? id)
        {

            if (id == null || _orderService.Read() == null)
            {
                return NotFound();
            }

            var order =  _orderService.GetSpecificOrder(id);

            if (order == null)
            {
                return NotFound();
            }

            var timeSlotItem = _orderService.GetSpecificTimeSlot(order.TimeSlotID);
            DateTime startTimeSlot = timeSlotItem.StartTimeSlot;
            DateTime endTimeSlot = startTimeSlot.AddMinutes(15);
            DateTime now = DateTime.Now;
            DateTime twoHoursAgo = endTimeSlot.AddMinutes(-60);

            if (twoHoursAgo > now)
            {
                ViewData["CustomerID"] = _orderService.CustomerSelectList(order.CustomerId);

                TempData["SelectDate"] = timeSlotItem.StartTimeSlot.ToString("yyyy-MM-dd");
                TempData["time"] = timeSlotItem.StartTimeSlot.ToString("H:mm");

               
                ViewBag.TimeSlotsSelectList = _orderService.GetTimeDropDownList(startTimeSlot);

                ViewBag.listOfTheOrder = _orderService.FilterOrderLines(id);

                return View(order);
            }
            TempData["TimesPast"] = "U kan geen de bestelling niet wijzigen 1u voor deze wordt klaargemaakt.";
            return RedirectToAction("Index", "Orders");
        }

        // POST: Orders/EditOrder/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrder(int id, [Bind("OrderID,Finished,CustomerId")] Order order,IFormCollection collection)
        {
            if (id != order.OrderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                var orderForT = _orderService.GetSpecificOrder(id);

                var timeSlotItem = orderForT.TimeSlot;

                orderForT.Finished = order.Finished;
                orderForT.CustomerId = order.CustomerId;

                string x = collection["TimeSlotsSelectList"];
                String selectedDate = collection["TimeSlotID"] + " " + x;
                DateTime timeSlotDateAndTime = Convert.ToDateTime(selectedDate);
                timeSlotItem.StartTimeSlot = timeSlotDateAndTime;

                _orderService.UpdateOrder(timeSlotItem, orderForT);

            }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_orderService.OrderExists(order.OrderID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }


            return RedirectToAction("index", "Orders");
            }
            var timeSlotItem1 = _orderService.GetSpecificTimeSlot(order.TimeSlotID);

            ViewData["CustomerID"] = _orderService.CustomerSelectList(order.CustomerId);

            TempData["SelectDate"] = timeSlotItem1.StartTimeSlot.ToString("yyyy-MM-dd");
            TempData["time"] = timeSlotItem1.StartTimeSlot.ToString("H:mm");


            ViewBag.TimeSlotsSelectList = _orderService.GetTimeDropDownList(timeSlotItem1.StartTimeSlot);

            ViewBag.listOfTheOrder = _orderService.FilterOrderLines(id);
            return View(order);
        }

        // GET: OrderLines/Edit/5
        public async Task<IActionResult> EditOrderLine(int? id)
        {
            if (id == null || _orderService.GetOrderLines() == null)
            {
                return NotFound();
            }

            var orderLine = _orderService.GetSpecificOrderLine(id);
            if (orderLine == null)
            {
                return NotFound();
            }
            ViewData["DishID"] = _orderService.MenuItemSelectList();
            return View(orderLine);
        }

        // POST: OrderLines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrderLine(int id, [Bind("OrderLineID,ExtraDetails,DishAmount,OrderID,MenuItemId")] OrderLine orderLine)
        {
            if (id != orderLine.OrderLineID)
            {
                return NotFound();
            }

             if (ModelState.IsValid)
            {
            try
                {

                _orderService.UpdateOrderLine(orderLine);
                }
                catch (DbUpdateConcurrencyException)
                {
                if (!_orderService.OrderLineExists(orderLine.OrderLineID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("EditOrder", new { id = orderLine.OrderID });
            }
            ViewData["DishID"] = _orderService.MenuItemSelectList();
            return View(orderLine);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _orderService.Read() == null)
            {
                return NotFound();
            }

            var order =  _orderService.GetSpecificOrder(id);

            ViewBag.listOfTheOrder = _orderService.FilterOrderLines(id);

            if (order == null)
            {
                return NotFound();
            }
            TempData["TimesPast"] = "";
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_orderService.Read() == null)
            {
                return Problem("Entity set 'LekkerbekContext.Orders'  is null.");
            }   

            var order = _orderService.GetSpecificOrder(id);


            if (order != null)
            {

                if (_orderService.DeleteOrder(order))
                {
                    return RedirectToAction(nameof(Index));
                }
      
                TempData["TimesPast"] = "De bestelling kan niet geannuleerd worden 2u voor deze klaargemaakt wordt.";
                ViewBag.listOfTheOrder = _orderService.FilterOrderLines(id);

                return View(order);
            }
            return NotFound();
        }



       
    }
}
