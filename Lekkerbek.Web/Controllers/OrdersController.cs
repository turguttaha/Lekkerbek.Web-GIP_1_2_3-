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
            ViewBag.listOfTheOrder  = FilterOrderLines(id);


            return View(order);
        }

        // !!!!Creating Order Starts here!!!!
        // GET: Orders/Create
        public IActionResult SelectCustomer()
        {
            
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerId", "Name");
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
            DateTime selectedDateTime = (DateTime)TempData["SelectedDateTime"];

            var usedTimeSlots = _context.TimeSlots.Where(t => t.StartTimeSlot == selectedDateTime).ToList();

            TempData["SelectedDateTime"] = selectedDateTime;
            var allChefId = _context.Chefs.ToList();
            List<int> ids = new List<int>();


            if (usedTimeSlots.Count() != 0)
            {
                foreach (var test in usedTimeSlots)
                {

                  ids.Add((int)test.ChefId);
         
                
                    
                }
            }
            if(usedTimeSlots.Count() == 2)
            {
                TempData["ChefError"] = "This time slot is full!";
                return RedirectToAction("SelectTimeSlot", "Orders");
            }

            if (ids.Count() < 2)
            {
                ViewData["ChefId"] = new SelectList(_context.Chefs.Where(r => ids.Contains(r.ChefId) == false), "ChefId", "ChefName");

                return View();
            }
            else 
            {
                TempData["errorChefs"] = "There are no chefs availible for this time slot!";
                return RedirectToAction("SelectTimeSlot", "Orders");
            }

            
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
            ViewData["MenuItemId"] = new SelectList(_context.MenuItems, "MenuItemId", "Name");
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
            orderLine.MenuItem = _context.MenuItems.Find(orderLine.MenuItemId);
            Order.TemproraryCart.Add(orderLine);
            
            ViewData["Message"] = "Your Dish is added";
            ViewBag.TemproraryCart = Order.TemproraryCart;
            ViewData["MenuItemId"] = new SelectList(_context.MenuItems, "MenuItemId", "Name");ModelState.Clear();
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
            _context.Add(timeSlot);
            await _context.SaveChangesAsync();

            //Order Object aanmaken

            var lastTimeSlot = _context.TimeSlots.OrderByDescending(t=>t.Id).FirstOrDefault();

            Order order = new Order();
            order.CustomerId = (int)TempData["SelectedCustomerId"];
            order.TimeSlotID = lastTimeSlot.Id;
            _context.Add(order);
            await _context.SaveChangesAsync();

            var lastOrder = _context.Orders.OrderByDescending(t => t.OrderID).FirstOrDefault();

            //Ordelines Object aanmaken/toevoegen aan order*
            foreach (OrderLine item in Order.TemproraryCart.ToList())
            {
                item.MenuItem = null;
                item.OrderID = lastOrder.OrderID;
                _context.Add(item);
                await _context.SaveChangesAsync();
                Order.TemproraryCart.Remove(item);
            }

            return RedirectToAction("index", "Orders");
        }

        // Completed order right here------------------------


        // GET: Orders/EditOrder/5
        public async Task<IActionResult> EditOrder(int? id)
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

            var timeSlotItem = await _context.TimeSlots.FindAsync(order.TimeSlotID);
            DateTime startTimeSlot = timeSlotItem.StartTimeSlot;
            DateTime endTimeSlot = startTimeSlot.AddMinutes(15);
            DateTime now = DateTime.Now;
            DateTime twoHoursAgo = endTimeSlot.AddMinutes(-60);

            if (twoHoursAgo > now)
            {
                ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerId", "Name", order.CustomerId);

                var timeSlot = await _context.TimeSlots.FindAsync(order.TimeSlotID);

                TempData["SelectDate"] = timeSlot.StartTimeSlot.ToString("yyyy-MM-dd");
                TempData["time"] = timeSlot.StartTimeSlot.ToString("H:mm");

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

                ViewBag.listOfTheOrder = FilterOrderLines(id);

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
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerId", "Name", order.CustomerId);
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
            ViewData["DishID"] = new SelectList(_context.MenuItems, "DishId", "Name", orderLine.MenuItemId);
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
            ViewData["DishID"] = new SelectList(_context.MenuItems, "DishId", "DishId", orderLine.MenuItemId);
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
            int x = (int)order.CustomerId;
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

        private List<OrderLine> FilterOrderLines(int? Orderid)
        {
            if (Orderid == null )
            {
                return new List<OrderLine>();
            }
            //filtering orderlines occording to orderId
            List<OrderLine> allOrderLines = _context.OrderLines.ToList();
            List<OrderLine> filteredOrderLines = new List<OrderLine>();
            foreach (var orderLine in allOrderLines.Where(c => c.OrderID == Orderid))
            {
                if (!filteredOrderLines.Contains(orderLine))
                {
                    orderLine.MenuItem = _context.MenuItems.Find(orderLine.MenuItemId);
                    filteredOrderLines.Add(orderLine);                  
                }
                    
            }
            return filteredOrderLines;
        }
    }
}
