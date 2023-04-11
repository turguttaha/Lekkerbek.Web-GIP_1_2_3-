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
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace Lekkerbek.Web.Controllers
{
    public class OrdersCashierController : Controller
    {
        private readonly IOrderCashierService _orderCashierService;

        public OrdersCashierController(IOrderCashierService orderCashierService)
        {
            
            _orderCashierService = orderCashierService;
        }

        // GET: Orders according to finished property
        public IActionResult Index()
        {
            //var orderCashier = _orderCashierService.Read();
            return View();
            //var lekkerBekContext = _orderCashierService.Read();
            //return View();
            //var lekkerbekContext = _context.Orders.Include(o => o.Customer).Include(o => o.TimeSlot).Where(c=>c.Finished==false);
            //return View(await lekkerbekContext.ToListAsync());
        }
        public ActionResult EditingPopup_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_orderCashierService.Read().ToDataSourceResult(request));
        }
        
        // Pay Off page Get: order to pay
        public async Task<IActionResult> Bill(int? id)
        {
            if (id == null || _orderCashierService.Read() == null)
            {
                return NotFound();
            }

            var order = _orderCashierService.GetSpecificOrder(id);
            TempData["OrderIdFromBill"] = id;
            int x = (int)id;
            TempData["OrderID"] = x;


            //filtering orderlines according to orderId
            //List<OrderLine> allOrderLines = _context.OrderLines.Include(c => c.Dish).ToList();
            List<OrderLine> allOrderLines = _orderCashierService.OrderLineRead(id);

            List <OrderLine> filteredOrderLines = new List<OrderLine>();

            foreach (var orderLine in allOrderLines.Where(c => c.OrderID == id))
            {
                if (!filteredOrderLines.Contains(orderLine))
                    filteredOrderLines.Add(orderLine);

            }
            //ViewBag.Dishes = _context.Dishes;


            ViewBag.listOfTheOrder = filteredOrderLines;


            if (order == null)
            {
                return NotFound();
            }
            double totalPrice = 0;
            foreach (var oorder in filteredOrderLines)
            {
                totalPrice += oorder.MenuItem.Price * oorder.DishAmount;
            }
            ViewBag.totalPrice = totalPrice;

            //var orderCount = _context.Orders.Where(c => c.CustomerID == order.CustomerID).ToList();
            var orderCount = _orderCashierService.GetOrders(order.CustomerId);
            
            if (orderCount.Count() >= 3)
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
            //var orderFinish = _context.Orders.Where(c => c.OrderID == id).FirstOrDefault();
            var orderFinish = _orderCashierService.GetSpecificOrder(id);
            //if (ModelState.IsValid)
            //{
            try
            {
                var discount = collection["Discount"];
                // Order.TemproraryCart.Add(orderLine);

                //List<OrderLine> allOrderLines2 = _context.OrderLines.Include(c => c.Dish).ToList();
                List<OrderLine> allOrderLines2 = _orderCashierService.OrderLineRead(id);
                List<OrderLine> filteredOrderLines2 = new List<OrderLine>();

                foreach (var orderLine in allOrderLines2.Where(c => c.OrderID == id))
                {
                    if (!filteredOrderLines2.Contains(orderLine))
                        filteredOrderLines2.Add(orderLine);

                }
                double totalPrice = 0;
                foreach (var oorder in filteredOrderLines2)
                {
                    totalPrice += oorder.MenuItem.Price * oorder.DishAmount;
                }


                //orderFinish.Finished = true;
                if ( orderFinish != null) 
                {
                    
                    orderFinish.Discount = int.Parse(collection["Discount"]);
                    ViewBag.totalPrice = totalPrice * (100 - orderFinish.Discount) / 100;
                    ViewBag.discount = discount;

                    //_context.Update(orderFinish);
                    //await _context.SaveChangesAsync();
                    _orderCashierService.Update(orderFinish);
                    
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
            if (id == null || _orderCashierService.Read() == null)
            {
                return NotFound();
            }

            var order = _orderCashierService.GetSpecificOrder(id);

            //filtering orderlines occording to orderId
            List<OrderLine> allOrderLines = _orderCashierService.OrderLineRead(id);
            List<OrderLine> filteredOrderLines = new List<OrderLine>();

            foreach (var orderLine in allOrderLines.Where(c => c.OrderID == id))
            {
                if (!filteredOrderLines.Contains(orderLine))
                    filteredOrderLines.Add(orderLine);

            }
            //ViewBag.Dishes = _context.Dishes;


            ViewBag.listOfTheOrder = filteredOrderLines;
            
            if (order == null)
            {
                return NotFound();
            }
            //var orderCount = _context.Orders.Where(c => c.CustomerID == order.CustomerID).ToList();
            var orderCount = _orderCashierService.GetOrders(order.CustomerId);

            if (orderCount.Count() >= 3)
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
            var orderFinish = _orderCashierService.GetSpecificOrder(id);

            //if (ModelState.IsValid)
            //{
            try
            {

                orderFinish.Finished = true;
                _orderCashierService.Update(orderFinish);
              
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
                List<OrderLine> allOrderLines = _orderCashierService.OrderLineRead(id);
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
                            " + item.MenuItem.Name + @"
                        </td>
                        <td>
                            " + item.MenuItem.Price + @"
                        </td>
                        <td>
                            " + item.DishAmount + @"
                        </td>
                        <td>
                            " + item.MenuItem.Price * item.DishAmount + @"
                        </td>
                        <td>
                            
                        </td>
                    </tr>";
                    totalPrice += item.MenuItem.Price * item.DishAmount;
                }
                
                bool discountBool = false;
                var orderFinishMail = _orderCashierService.GetSpecificOrder(id);


                if (orderFinishMail != null && orderFinishMail.Discount !=0 && orderFinishMail.Discount!=null) 
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
            if (id == null || _orderCashierService.GetAllCustomers == null)
            {
                return NotFound();
            }

            var customer = _orderCashierService.GetSpecificCustomer(id);
            if (customer == null)
            {
                return NotFound();
            }
            ViewData["PreferredDishId"] = new SelectList(_orderCashierService.GetAllPrefferedDishes(), "PreferredDishId", "Name", customer.PreferredDishId);
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
                _orderCashierService.UpdateCustomer(customer);
                //_context.Update(customer);
                //await _context.SaveChangesAsync();
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
            ViewData["PreferredDishId"] = new SelectList(_orderCashierService.GetAllPrefferedDishes(), "PreferredDishId", "PreferredDishId", customer.PreferredDishId);
            return View(customer);
        }
        private bool CustomerExists(int id)
        {
            bool exist = false;
            if (_orderCashierService.GetSpecificCustomer(id) != null) 
            {
                exist = true;
            }
            return exist;
            //return _context.Customers.Any(e => e.CustomerId == id);
        }
        private bool OrderExists(int id)
        {
            bool exist = false;
            if (_orderCashierService.GetSpecificOrder(id) != null)
            {
                exist = true;
            }
            return exist;
            //return _context.Orders.Any(e => e.OrderID == id);
        }
    }
}


