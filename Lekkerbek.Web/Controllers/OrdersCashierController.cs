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
using System.IO;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Threading;
using Lekkerbek.Web.Services;

namespace Lekkerbek.Web.Controllers
{
    public class OrdersCashierController : Controller
    {
        private readonly LekkerbekContext _context;

        public OrdersCashierController(LekkerbekContext context)
        {
            _context = context;
        }

        // GET: Orders according to finished property
        public async Task<IActionResult> Index()
        {
            var lekkerbekContext = _context.Orders.Include(o => o.Customer).Include(o => o.TimeSlot).Where(c=>c.Finished==false);
            return View(await lekkerbekContext.ToListAsync());
        }

        // Pay Off page Get: order to pay
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

        // Pay off page discount func
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
                if ( orderFinish != null) 
                {
                    
                    orderFinish.Discount = int.Parse(collection["Discount"]);
                    ViewBag.totalPrice = totalPrice * (100 - orderFinish.Discount) / 100;
                    ViewBag.discount = discount;
                    
                    _context.Update(orderFinish);
                    await _context.SaveChangesAsync();
                }
                
                
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

        // Pay off page payment func sending mail
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

                _context.Update(orderFinish);
                await _context.SaveChangesAsync();
                String testMail = @"<table class=""table"">
                <thead>
                    <tr>
                        <th>
                            Dish Name
                        </th>
                        <th>
                            Dish Price
                        </th>
                        <th>
                            Dish Amount
                        </th>
                        <th>
                            Sub Total
                        </th>
            <th>
                
            </th>
                    </tr>
                </thead>
                <tbody>";
                

                //filtering orderlines occording to orderId
                List<OrderLine> allOrderLines = _context.OrderLines.Include(c => c.Dish).ToList();
                List<OrderLine> filteredOrderLines = new List<OrderLine>();

                foreach (var orderLine in allOrderLines.Where(c => c.OrderID == id))
                {
                    if (!filteredOrderLines.Contains(orderLine))
                        filteredOrderLines.Add(orderLine);

                }
                double totalPrice = 0;
                foreach (var item in filteredOrderLines)
                {
                            testMail += @" <tr>
                        <td>
                            " + item.Dish.Name + @"
                        </td>
                        <td>
                            " + item.Dish.Price + @"
                        </td>
                        <td>
                            " + item.DishAmount + @"
                        </td>
                        <td>
                            " + item.Dish.Price * item.DishAmount + @"
                        </td>
                        <td>
                            
                        </td>
                    </tr>";
                    totalPrice += item.Dish.Price * item.DishAmount;
                }
                
                bool discountBool = false;
                var orderFinishMail = _context.Orders.Where(c => c.OrderID == id).FirstOrDefault();
                
                
                if (orderFinishMail != null|| orderFinishMail.Discount !=0) 
                {
                    discountBool = true;
                }
                
                
                
                
                
                if (discountBool)
                {
                    testMail += @"
                    <tr>

                        <td>
                            Discount:
                        </td>
                        <td>
                        </td>
                        <td>
                        </td>
                    


                            <td>
                                "+ orderFinishMail.Discount+ @"
                            </td>
                            <td>
                            </td>
                    </tr>";
                    totalPrice = totalPrice * (double)(100 - orderFinish.Discount) / 100;
                }
               
                testMail += @"
                <tr>
                    <td>
                        Total Price:
                    </td>
                    <td>

                    </td>
                    <td>

                    </td>


                    <td>
                        " + totalPrice + @"
                    </td>
                    <td>
                    </td>
                </tr>
                </form></tbody></table>";


                ///send email
                ///
                EmailService emailService = new EmailService();
                emailService.SendMail("gipteam2.lekkerbek@gmail.com", "Your invoice of the Lekkerbek", testMail);
                
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

           // }

            return RedirectToAction(nameof(Index));
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


