﻿using System;
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

namespace Lekkerbek.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: Orders
        public IActionResult Index()
        {
            //var lekkerbekContext = _context.Orders.Include(o => o.Customer).Include(o => o.TimeSlot);
            return View();
        }

        //Read func for Kendo

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _orderService.Read() == null)
            {
                return NotFound();
            }

            var order = _orderService.GetSpecificOrder(id);

            if (order == null)
            {
                return NotFound();
            }
            ViewBag.listOfTheOrder  = _orderService.FilterOrderLines(id);


            return View(order);
        }

        // !!!!Creating Order Starts here!!!!
        // GET: Orders/Create
        public IActionResult SelectCustomer()
        {
            
            ViewData["CustomerID"] = _orderService.CustomerSelectList();
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
          
            ViewBag.TimeSlotsSelectList = _orderService.GetTimeDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SelectTimeSlot(IFormCollection collection)
        {
            string x = collection["TimeSlotsSelectList"];
            String selectedDate = collection["StartTimeSlot"] + " "+ x;
            DateTime timeSlotDateAndTime = Convert.ToDateTime(selectedDate);
            TempData["SelectedDateTime"] = timeSlotDateAndTime;
            TempData["SelectedChef"] = int.Parse(collection["ChefId"]);
            
            return RedirectToAction("AddOrderLine", "Orders");
        }
        public async Task<JsonResult> LookUpChefs(string date, string time)
        {

            string x = time;
            String selectedDate = date;
            DateTime timeSlotDateAndTime = Convert.ToDateTime(selectedDate + " " + time);

            ViewData["ChefId"] = _orderService.CheckChefs(timeSlotDateAndTime);

            return Json(new { chefs = ViewData["ChefId"] });
        }

        // GET: OrderLines/Create
        public IActionResult AddOrderLine()
        {
            ViewData["DishID"] = _orderService.MenuItemSelectList();
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
            // go to database and get dish name via id
            orderLine.MenuItem = _orderService.GetSpecificMenuItem(orderLine.MenuItemId);
            Order.TemproraryCart.Add(orderLine);
            
            ViewData["Message"] = "Your Dish is added";

            ViewBag.TemproraryCart = Order.TemproraryCart;
            ViewData["DishID"] = _orderService.MenuItemSelectList();
            ModelState.Clear();
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteOrder()
        {
            //TimeSlot Object aanmaken

            TimeSlot timeSlot = new TimeSlot();
            timeSlot.StartTimeSlot = (DateTime)TempData["SelectedDateTime"];
            timeSlot.ChefId =(int)TempData["SelectedChef"];

            Order order = new Order();
            order.CustomerID = (int)TempData["SelectedCustomerId"];

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
                ViewData["CustomerID"] = _orderService.CustomerSelectList(order.CustomerID);

                TempData["SelectDate"] = timeSlotItem.StartTimeSlot.ToString("yyyy-MM-dd");
                TempData["time"] = timeSlotItem.StartTimeSlot.ToString("H:mm");

               
                ViewBag.TimeSlotsSelectList = _orderService.GetTimeDropDownList();

                ViewBag.listOfTheOrder = _orderService.FilterOrderLines(id);

                return View(order);
            }
            TempData["TimesPast"] = "Order has less than 1 hours to prepare so no changes can be made to the order.";
            return RedirectToAction("Index", "Orders");
        }

        // POST: Orders/EditOrder/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrder(int id, [Bind("OrderID,Finished,CustomerID")] Order order,IFormCollection collection)
        {
            if (id != order.OrderID)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
                try
                {

                TimeSlot timeSlot = new TimeSlot();               
                string x = collection["TimeSlotsSelectList"];
                String selectedDate = collection["TimeSlotID"] + " " + x;
                DateTime timeSlotDateAndTime = Convert.ToDateTime(selectedDate);
                timeSlot.StartTimeSlot = timeSlotDateAndTime;
                _context.Add(timeSlot);
                await _context.SaveChangesAsync();
                var lastTimeSlot = _context.TimeSlots.OrderByDescending(t => t.Id).FirstOrDefault();
                order.TimeSlotID = lastTimeSlot.Id;
                _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }


            return RedirectToAction("index", "Orders");
            //}
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerId", "Name", order.CustomerID);
            ViewData["TimeSlotID"] = new SelectList(_context.TimeSlots, "Id", "Id", order.TimeSlotID);
            return View(order);
        }

        // GET: OrderLines/Edit/5
        public async Task<IActionResult> EditOrderLine(int? id)
        {
            if (id == null || _context.OrderLines == null)
            {
                return NotFound();
            }

            var orderLine = await _context.OrderLines.FindAsync(id);
            if (orderLine == null)
            {
                return NotFound();
            }
            ViewData["DishID"] = new SelectList(_context.MenuItems, "MenuItemId", "Name", orderLine.MenuItemId);
            //ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", orderLine.OrderID);
            return View(orderLine);
        }

        // POST: OrderLines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrderLine(int id, [Bind("OrderLineID,ExtraDetails,DishAmount,OrderID,DishID")] OrderLine orderLine)
        {
            if (id != orderLine.OrderLineID)
            {
                return NotFound();
            }

            // if (ModelState.IsValid)
            //{
            try
                {
                
                    _context.Update(orderLine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                //if (!OrderLineExists(orderLine.OrderLineID))
                //{
                //    return NotFound();
                //}
                //else
                //{
                //    throw;
                //}
            }
            return RedirectToAction("EditOrder", new { id = orderLine.OrderID });
            //}
            ViewData["DishID"] = new SelectList(_context.MenuItems, "MenuItemId", "MenuItemId", orderLine.MenuItemId);
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", orderLine.OrderID);
            return View(orderLine);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.TimeSlot)
                .FirstOrDefaultAsync(m => m.OrderID == id);

            ViewBag.listOfTheOrder = FilterOrderLines(id);

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
            if (_context.Orders == null)
            {
                return Problem("Entity set 'LekkerbekContext.Orders'  is null.");
            }
            

            var order = await _context.Orders.FindAsync(id);
            int x = (int)order.CustomerID;
            order.Customer = _context.Customers.Find(x);
            
            if (order != null ) {
                var timeSlot = await _context.TimeSlots.FindAsync(order.TimeSlotID);
                DateTime startTimeSlot = timeSlot.StartTimeSlot;
                DateTime endTimeSlot = startTimeSlot.AddMinutes(15);
                DateTime now = DateTime.Now;
                DateTime twoHoursAgo = endTimeSlot.AddMinutes(-120);

                List<OrderLine> filteredOrderLines = FilterOrderLines(id);

                if (twoHoursAgo > now) {
                
                    //deleting order timeslot orderlines from database
                    if (timeSlot != null && filteredOrderLines != null)
                    {
                        _context.Orders.Remove(order);
                        _context.TimeSlots.Remove(timeSlot);
                        foreach (var orderLine in filteredOrderLines) { _context.OrderLines.Remove(orderLine); }
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                TempData["TimesPast"] = "Order has less than 2 hours to prepare so it cannot be cancelled.";
                ViewBag.listOfTheOrder = filteredOrderLines;

                return View(order);
            }
            return NotFound();


        }

        private bool OrderExists(int id)
        {
          return _context.Orders.Any(e => e.OrderID == id);
        }

       
    }
}
