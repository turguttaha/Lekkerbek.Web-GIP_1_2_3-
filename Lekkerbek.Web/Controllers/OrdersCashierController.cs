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

namespace Lekkerbek.Web.Controllers
{
    public class OrdersCashierController : Controller
    {
        private readonly LekkerbekContext _context;

        public OrdersCashierController(LekkerbekContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var lekkerbekContext = _context.Orders.Include(o => o.Customer).Include(o => o.TimeSlot).Where(c=>c.Finished==false);
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

            //filtering orderlines occording to orderId
            List<OrderLine> allOrderLines = _context.OrderLines.ToList();
            List<OrderLine> filteredOrderLines = new List<OrderLine>();

            foreach (var orderLine in allOrderLines.Where(c => c.OrderID == id))
            {
                if (!filteredOrderLines.Contains(orderLine))
                    filteredOrderLines.Add(orderLine);
            }


            ViewBag.listOfTheOrder = filteredOrderLines;


            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // !!!!Creating Order Starts here!!!!
        // GET: Orders/Create
        public IActionResult SelectCustomer()
        {

            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerId", "FName");
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
            String selectedDate = collection["StartTimeSlot"] + " " + x;
            DateTime timeSlotDateAndTime = Convert.ToDateTime(selectedDate);
            TempData["SelectedDateTime"] = timeSlotDateAndTime;

            return RedirectToAction("SelectChef", "Orders");
        }
        // GET: Orders/Edit/5
        public async Task<IActionResult> Bill(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.TimeSlot)
                .FirstOrDefaultAsync(m => m.OrderID == id);
            TempData["OrderIdFromBill"] = id;
            int x = (int)id;
            TempData["OrderID"] = x;


            //filtering orderlines occording to orderId
            List<OrderLine> allOrderLines = _context.OrderLines.Include(c => c.Dish).ToList();
            List<OrderLine> filteredOrderLines = new List<OrderLine>();

            foreach (var orderLine in allOrderLines.Where(c => c.OrderID == id))
            {
                if (!filteredOrderLines.Contains(orderLine))
                    filteredOrderLines.Add(orderLine);

            }
            ViewBag.Dishes = _context.Dishes;


            ViewBag.listOfTheOrder = filteredOrderLines;


            if (order == null)
            {
                return NotFound();
            }
            double totalPrice = 0;
            foreach (var oorder in filteredOrderLines)
            {
                totalPrice += oorder.Dish.Price * oorder.DishAmount;
            }
            ViewBag.totalPrice = totalPrice;

            var test = _context.Orders.Where(c => c.CustomerID == order.CustomerID).ToList();
            if (test.Count() >= 3)
            {
                ViewBag.Korting = true;
            }
            else
            {
                ViewBag.Korting = false;
            }



            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Bill(int id, [Bind("OrderID,OrderFinishedTime,Finished,CustomerID,TimeSlotID")] Order orderInfo, IFormCollection collection)
        {
            var orderFinish = _context.Orders.Where(c => c.OrderID == id).FirstOrDefault();

            //if (ModelState.IsValid)
            //{
            try
            {
                var discount = collection["Customer"];
                // Order.TemproraryCart.Add(orderLine);

                Console.WriteLine(discount.ToString());

                //orderFinish.Finished = true;
                ViewData["CustomerId"] = orderInfo.CustomerID;
                orderFinish.Discount = int.Parse(collection["Discount"]);
                ViewBag.totalPrice = double.Parse(collection["Customer"]) * (100 - orderFinish.Discount) / 100;
                ViewBag.discount = discount;
                //_context.Update(order);
                //await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(orderInfo.OrderID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.TimeSlot)
                .FirstOrDefaultAsync(m => m.OrderID == id);

            //filtering orderlines occording to orderId
            List<OrderLine> allOrderLines = _context.OrderLines.Include(c => c.Dish).ToList();
            List<OrderLine> filteredOrderLines = new List<OrderLine>();

            foreach (var orderLine in allOrderLines.Where(c => c.OrderID == id))
            {
                if (!filteredOrderLines.Contains(orderLine))
                    filteredOrderLines.Add(orderLine);

            }
            ViewBag.Dishes = _context.Dishes;


            ViewBag.listOfTheOrder = filteredOrderLines;


            if (order == null)
            {
                return NotFound();
            }


            var test = _context.Orders.Where(c => c.CustomerID == order.CustomerID).ToList();
            if (test.Count() >= 3)
            {
                ViewBag.Korting = true;
            }
            else
            {
                ViewBag.Korting = false;
            }
            return View();
            //}

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(int id, [Bind("OrderID,OrderFinishedTime,Finished,CustomerID,TimeSlotID")] Order order)
        {
            var orderFinish = _context.Orders.Where(c => c.OrderID == id).FirstOrDefault();

            //if (ModelState.IsValid)
            //{
            try
            {

                //orderFinish.Finished = true;
                Console.WriteLine("test");
                //_context.Update(order);
                //await _context.SaveChangesAsync();
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

            //}

            return RedirectToAction(nameof(Index));
        }
        public IActionResult SelectChef()
        {
            DateTime selectedDateTime = (DateTime)TempData["SelectedDateTime"];

            var usedTimeSlots = _context.TimeSlots.Where(t => t.StartTimeSlot == selectedDateTime).ToList();
            TempData["SelectedDateTime"] = selectedDateTime;
            var allChefId = _context.Chefs.ToList();
            List<int> ids = new List<int>();

            if (usedTimeSlots != null)
            {
                foreach (var test in usedTimeSlots)
                {
                    ids.Add((int)test.ChefId);
                }
            }


            ViewData["ChefId"] = new SelectList(_context.Chefs.Where(r => ids.Contains(r.ChefId) == false), "ChefId", "ChefName");

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
            ViewData["DishID"] = new SelectList(_context.Dishes, "DishId", "Name");
            ViewBag.TemproraryCart = Order.TemproraryCart;
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
            ViewData["Message"] = "Your Dish is added";
            ViewBag.TemproraryCart = Order.TemproraryCart;
            ViewData["DishID"] = new SelectList(_context.Dishes, "DishId", "Name");
            return View();

        }
       

        public async Task<IActionResult> CompleteOrder()
        {
            //TimeSlot Object aanmaken

            TimeSlot timeSlot = new TimeSlot();
            timeSlot.StartTimeSlot = (DateTime)TempData["SelectedDateTime"];
            timeSlot.ChefId = (int)TempData["SelectedChef"];
            _context.Add(timeSlot);
            await _context.SaveChangesAsync();

            //Order Object aanmaken

            var lastTimeSlot = _context.TimeSlots.OrderByDescending(t => t.Id).FirstOrDefault();

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
            //ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerId", "FName");
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerId", "FName", order.CustomerID);
            ViewData["TimeSlotID"] = new SelectList(_context.TimeSlots, "Id", "StartTimeSlot", order.TimeSlotID);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderID,Finished,CustomerID,TimeSlotID")] Order order)
        {
            if (id != order.OrderID)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
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
            //}
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
            //filtering orderlines occording to orderId
            List<OrderLine> allOrderLines = _context.OrderLines.ToList();
            List<OrderLine> filteredOrderLines = new List<OrderLine>();

            foreach (var orderLine in allOrderLines.Where(c => c.OrderID == id))
            {
                if (!filteredOrderLines.Contains(orderLine))
                    filteredOrderLines.Add(orderLine);
            }
            ViewBag.listOfTheOrder = filteredOrderLines;

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


            if (order != null)
            {
                var timeSlot = await _context.TimeSlots.FindAsync(order.TimeSlotID);
                DateTime startTimeSlot = timeSlot.StartTimeSlot;
                DateTime endTimeSlot = startTimeSlot.AddMinutes(15);
                DateTime now = DateTime.Now;
                DateTime twoHoursAgo = endTimeSlot.AddMinutes(-120);
                //filtering orderlines occording to orderId
                List<OrderLine> allOrderLines = _context.OrderLines.ToList();
                List<OrderLine> filteredOrderLines = new List<OrderLine>();

                foreach (var orderLine in allOrderLines.Where(c => c.OrderID == id))
                {
                    if (!filteredOrderLines.Contains(orderLine))
                        filteredOrderLines.Add(orderLine);
                }
                if (twoHoursAgo > now)
                {



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
        // GET: Customers/Edit/5
        public async Task<IActionResult> EditCustomer(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            ViewData["PreferredDishId"] = new SelectList(_context.PreferredDishes, "PreferredDishId", "Name", customer.PreferredDishId);
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCustomer(int id, [Bind("CustomerId,FName,LName,Email,PhoneNumber,Address,Birthday,PreferredDishId")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            // {
            //I put this in the comment. Because ModelState.IsValid is checking if all values are populated. But we do not fill the id value, it is added in dabase.

            try
            {
                _context.Update(customer);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(customer.CustomerId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Bill", new { id = (int)TempData["OrderIdFromBill"] });
            //}
            ViewData["PreferredDishId"] = new SelectList(_context.PreferredDishes, "PreferredDishId", "PreferredDishId", customer.PreferredDishId);
            return View(customer);
        }
        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }
    }
}


