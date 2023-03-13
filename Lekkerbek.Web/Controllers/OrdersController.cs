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

namespace Lekkerbek.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly LekkerbekContext _context;

        public OrdersController(LekkerbekContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var lekkerbekContext = _context.Orders.Include(o => o.Customer).Include(o => o.TimeSlot);
            return View(await lekkerbekContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.TimeSlot)
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult SelectCustomer()
        {
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
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
            /*
            24
            which chef is free on *insert day*

            select chefID from TimeSlot where beginTimeSlot = 'datepickertime' and where (select )
            -> if count < 24 -> show in dropdown
             
             */


            List<SelectListItem> TimeSlotsSelectList = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "12:00", Value = "12:00"
                },
                new SelectListItem {
                    Text = "12:15", Value = "12:15"
                },
                new SelectListItem {
                    Text = "12:30", Value = "12:30"
                },
                new SelectListItem {
                    Text = "12:45", Value = "12:45"
                },
                new SelectListItem {
                    Text = "13:00", Value = "13:00"
                },
                new SelectListItem {
                    Text = "13:15", Value = "13:15"
                },
                new SelectListItem {
                    Text = "13:30", Value = "13:30"
                },
                new SelectListItem {
                    Text = "13:45", Value = "13:45"
                },
                new SelectListItem {
                    Text = "18:00", Value = "18:00"
                },
                new SelectListItem {
                    Text = "18:15", Value = "18:15"
                },
                new SelectListItem {
                    Text = "18:30", Value = "18:30"
                },
                new SelectListItem {
                    Text = "18:45", Value = "18:45"
                },
                new SelectListItem {
                    Text = "19:00", Value = "19:00"
                },
                new SelectListItem {
                    Text = "19:15", Value = "19:15"
                },
                new SelectListItem {
                    Text = "19:30", Value = "19:30"
                },
                new SelectListItem {
                    Text = "19:45", Value = "19:45"
                },
                new SelectListItem {
                    Text = "20:00", Value = "20:00"
                },
                new SelectListItem {
                    Text = "20:15", Value = "20:15"
                },
                new SelectListItem {
                    Text = "20:30", Value = "20:30"
                },
                new SelectListItem {
                    Text = "20:45", Value = "20:45"
                },
                new SelectListItem {
                    Text = "21:00", Value = "21:00"
                },
                new SelectListItem {
                    Text = "21:15", Value = "21:15"
                },
                new SelectListItem {
                    Text = "21:30", Value = "21:30"
                },
                new SelectListItem {
                    Text = "21:45", Value = "21:45"
                },
            };
            ViewBag.TimeSlotsSelectList = TimeSlotsSelectList;
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


            


            return RedirectToAction("SelectChef", "Orders");
        }
        public IActionResult SelectChef()
        {
            ViewData["ChefId"] = new SelectList(_context.Chefs, "ChefId", "ChefId");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SelectChef(IFormCollection collection)
        {
            TempData["SelectedChef"] = int.Parse(collection["ChefId"]);
            return RedirectToAction("AddOrderLine", "Orders");
        }
        // GET: OrderLines/Create
        public IActionResult AddOrderLine()
        {
            ViewData["DishID"] = new SelectList(_context.Dishes, "DishId", "DishId");
            return View();
        }

        // POST: OrderLines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrderLine([Bind("OrderLineID,ExtraDetails,DishAmount,OrderID,DishID")] OrderLine orderLine)
        {

            Order.TemproraryCart.Add(orderLine);
            ViewBag.TemproraryCart = Order.TemproraryCart;
            ViewData["Message"] = "Your Dish is added";
            ViewData["DishID"] = new SelectList(_context.Dishes, "DishId", "DishId");
            return View();

        }

        public async Task<IActionResult> CompleteOrder()
        {
            //TimeSlot Object aanmaken

            TimeSlot timeSlot = new TimeSlot();
            timeSlot.StartTimeSlot = (DateTime)TempData["SelectedDateTime"];
            timeSlot.ChefId =(int)TempData["SelectedChef"];
            _context.Add(timeSlot);
            await _context.SaveChangesAsync();

            //Order Object aanmaken

            var lastTimeSlot = _context.TimeSlots.OrderByDescending(t=>t.Id).FirstOrDefault();

            Order order = new Order();
            order.CustomerID = (int)TempData["SelectedCustomerId"];
            order.TimeSlotID = lastTimeSlot.Id;
            _context.Add(order);
            await _context.SaveChangesAsync();

            var lastOrder = _context.Orders.OrderByDescending(t => t.OrderID).FirstOrDefault();

            //Ordelines Object aanmaken/toevoegen aan order*
            foreach (OrderLine item in Order.TemproraryCart.ToList())
            {
                item.OrderID = lastOrder.OrderID;
                _context.Add(item);
                await _context.SaveChangesAsync();
                Order.TemproraryCart.Remove(item);
            }

            return RedirectToAction("index", "Orders");
        }
        public IActionResult Finnish(IFormCollection collection)
        {
            TempData["SelectedTime"] = collection["dates"];

            var customer = (_context.Customers.Where(c => c.Name == "Customer1")).FirstOrDefault<Customer>();
            //get the object data of the query you asked
            //in theory, ask date + time first; check if any of the 2 chefs are free; put the free chefs in a dropdown
            //thinking about ^this, would it be better to first choose day and time, look up in the database, and then show the free chefs to assign to the newly made timeslot
            return NotFound(ViewBag.SelectedTime + " " + TempData["SelectedDate"].ToString() + " " + customer.Name );


            return View();
        }
        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerId", "Name", order.CustomerID);
            ViewData["TimeSlotID"] = new SelectList(_context.TimeSlots, "Id", "Id", order.TimeSlotID);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderID,OrderFinishedTime,Finished,CustomerID,TimeSlotID")] Order order)
        {
            if (id != order.OrderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerId", "Name", order.CustomerID);
            ViewData["TimeSlotID"] = new SelectList(_context.TimeSlots, "Id", "Id", order.TimeSlotID);
            return View(order);
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
            if (order == null)
            {
                return NotFound();
            }

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
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
          return _context.Orders.Any(e => e.OrderID == id);
        }
    }
}