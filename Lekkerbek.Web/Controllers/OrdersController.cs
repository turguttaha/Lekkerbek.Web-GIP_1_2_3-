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
                var discount = collection["Discount"];
                // Order.TemproraryCart.Add(orderLine);

                List<OrderLine> allOrderLines2 = _context.OrderLines.Include(c => c.Dish).ToList();
                List<OrderLine> filteredOrderLines2 = new List<OrderLine>();

                foreach (var orderLine in allOrderLines2.Where(c => c.OrderID == id))
                {
                    if (!filteredOrderLines2.Contains(orderLine))
                        filteredOrderLines2.Add(orderLine);

                }
                double totalPrice = 0;
                foreach (var oorder in filteredOrderLines2)
                {
                    totalPrice += oorder.Dish.Price * oorder.DishAmount;
                }


                //orderFinish.Finished = true;
                if (orderFinish != null)
                {
                    orderFinish.Discount = int.Parse(collection["Discount"]);
                    ViewBag.totalPrice = totalPrice * (100 - orderFinish.Discount) / 100;
                    ViewBag.discount = discount;
                }

                //_context.Update(orderFinish);
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
            return View(order);
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

                orderFinish.Finished = true;
                Console.WriteLine("AAAAAAAAAAAAAA");
                _context.Update(orderFinish);
                await _context.SaveChangesAsync();

                ///send emsil
                /*
                string fromMail = "";
                string fromPassword = "";

                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromMail);
                message.Subject = "You better work or i will find your family";
                message.To.Add(new MailAddress(""));
                message.Body = "<html><body> Test Body </body></html>";
                message.IsBodyHtml = true;

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(fromMail, fromPassword),
                    EnableSsl = true,
                };

                smtpClient.Send(message);*/
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
            // go to database and get dish name via id
            orderLine.Dish = _context.Dishes.Find(orderLine.DishID);
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
                item.Dish = null;
                item.OrderID = lastOrder.OrderID;
                _context.Add(item);
                await _context.SaveChangesAsync();
                Order.TemproraryCart.Remove(item);
            }

            return RedirectToAction("index", "Orders");
        }

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
                ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerId", "FName", order.CustomerID);

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
            ViewData["DishID"] = new SelectList(_context.Dishes, "DishId", "Name", orderLine.DishID);
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
            ViewData["DishID"] = new SelectList(_context.Dishes, "DishId", "DishId", orderLine.DishID);
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
            order.Customer = _context.Customers.Find(order.CustomerID);
            
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
                    orderLine.Dish = _context.Dishes.Find(orderLine.DishID);
                    filteredOrderLines.Add(orderLine);                  
                }
                    
            }
            return filteredOrderLines;
        }
    }
}
