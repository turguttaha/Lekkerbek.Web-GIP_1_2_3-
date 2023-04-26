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
using Microsoft.AspNetCore.Authorization;
using System.Net.Mime;
using Lekkerbek.Web.NewFolder;

namespace Lekkerbek.Web.Controllers
{
    [Authorize(Roles = "Cashier")]

    public class OrdersCashierController : Controller
    {
        private readonly IOrderCashierService _orderCashierService;
        private readonly ICustomerService _customerService;
        public OrdersCashierController(IOrderCashierService orderCashierService, ICustomerService customerService)
        {
            _customerService = customerService;
            _orderCashierService = orderCashierService;
        }

        // GET: Orders according to finished property
        public IActionResult Index()
        {
            return View();
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
            double totalPriceBTW = 0;
            foreach (var oorder in filteredOrderLines)
            {
                totalPrice += oorder.MenuItem.Price * oorder.DishAmount;
                totalPriceBTW += oorder.MenuItem.Price * oorder.DishAmount * (1 + (oorder.MenuItem.BtwNumber / 100));
            }
            ViewBag.totalPrice = totalPrice;
            ViewBag.totalPriceBTW = totalPriceBTW;

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
                    totalPrice += oorder.MenuItem.Price * oorder.DishAmount * (1 + (oorder.MenuItem.BtwNumber / 100));
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
        // GET: Customers/Create
        public IActionResult Create()
        {

            ViewData["PreferredDishId"] = _customerService.GetPreferredDishes();
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,FName,LName,Email,PhoneNumber,FirmName,ContactPerson,StreetName,City,PostalCode,Btw,BtwNumber,Birthday,PreferredDishId")] Customer customer)
        {
            //I put this in the comment. Because ModelState.IsValid is checking if all values are populated. But we do not fill the id value, it is added in dabase.
            //if (ModelState.IsValid)
            //{
            if (customer != null)
            {
                _customerService.Create(customer);
                return RedirectToAction(nameof(Index));
            }
            //}
            ViewData["PreferredDishId"] = _customerService.GetPreferredDishes(customer);
            return View(customer);
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
                
                String testMail = @"
<!DOCTYPE html>
<html>
    <head>
        <style>* {
    padding: 0;
    box-sizing: border-box;
}

#Container{
    width: 50em;
    margin: 0 auto;
    background-color: white;
}

h1{
    margin-left: 4px;
}

header{
    padding: 10px;
    position: relative;
}

main{
    border-style: solid;
    margin: 20px;
}

.Left{
    margin: 0;
    width: 250px;
    margin-left: 10px;
}

.Left-bottom{
    margin-top: 50px;
    margin-left: 10px;
    border-style: solid;
    width: 250px;
    padding-left: 10px;
}

.Grid{
    display: flex;
}

.Right{
    margin-right: 10px;
    margin-left: 150px;
}

img{
    width: 300px;
    height: 150px;
}

div{
    display: block;
}

body{
    background-color: gray;
}

.Above-footer{
    margin-top: 30px;
    margin-left: 20px;
}

footer{
    display: flex;
    align-items: center;
    border-top: 2px solid black;
    font-size: 12px;
    padding: 5px;
}

table{
    border-collapse: collapse;
}

th{
    background-color: lightgray;
    border-bottom: 1px solid black;
}

th, td{
    width:250px;
    text-align:center;
    padding:5px;
    border-right: 1px solid black;
}</style>
    </head>
    <body>
        <div id=""Container"">
            <header>
                <div class=""Left"">
                    <img src=""cid:Logo"" alt=""Logo"">
                    <H1>Bill</H1>
                </div>
                <div class=""Grid"">
                    <div class=""Left-bottom"">
                        <p>Number : 2023-01</p>
                        <p>Date : "+DateTime.Now.ToString()+ @"</p>
                    </div>
                    <div class=""Right"">
                        <h2>"+ orderFinish.Customer.FirmName+ @"</h2>
                        <p>"+ orderFinish.Customer.StreetName+ " "+ orderFinish.Customer.PostalCode + " " + orderFinish.Customer.City + @"</p>
                        <p>B.T.W.-nr. :" + orderFinish.Customer.Btw+orderFinish.Customer.BtwNumber +@"</p>
                    </div>
                </div>
            </header>
            <main>

<table>
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
                double totalPriceBTW = 0;
                foreach (var item in filteredOrderLines)
                {
                    /*
                            testMail += @" <tr>
                        <td>
                            " + item.MenuItem.Name + @"
                        </td>
                        <td>
                            " + item.MenuItem.Price + @"
                        </td>
                            " + item.MenuItem.BtwNumber + @"
                        <td>
                            " + item.DishAmount + @"
                        </td>
                        <td>
                            " + item.MenuItem.Price * item.DishAmount + @"
                        </td>
                        <td>
                            
                        </td>
                    </tr>";*/
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
                    </tr>";


                    totalPriceBTW += item.MenuItem.Price* item.DishAmount * (1 + (item.MenuItem.BtwNumber / 100));
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
                    </tr>";
                    totalPrice = totalPrice * (double)(100 - orderFinish.Discount) / 100;
                    totalPriceBTW = totalPriceBTW * (double)(100 - orderFinish.Discount) / 100;
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
                    
                </tr>
               </tbody></table></main>
                <div class=""Above-footer"">
                    <p>The invoice is payable within 30 days of the invoice date.</p>
                </div>
            <footer>
                <p>De Lekkerbek- Culinaire Kringstraat108/2- 3530 HOUTHALEN - TEL. : 0475/22.22.41</p>
                <p>B.T.W. : BE 04763.352.133        COMPANYNR : 0763.352.133</p>
                <p>PNB PARIBAS FORTIS : IBAN BE19 0013 5497 5612   -   BIC GEPA BE BB</p>
            </footer>
            
        </div>    
    </body>
</html>";
                if (orderFinish.Customer.FirmName != null && orderFinish.Customer.FirmName != "") 
                {
                    
                    AlternateView avHtml = AlternateView.CreateAlternateViewFromString
                        (testMail, null, MediaTypeNames.Text.Html);
                    // Create a LinkedResource object for each embedded image
                    LinkedResource pic1 = new LinkedResource(Directory.GetCurrentDirectory() + "/wwwroot/img/Logo Lekkerbek.jpeg", MediaTypeNames.Image.Jpeg);
                    pic1.ContentId = "Logo";
                    avHtml.LinkedResources.Add(pic1);
                    ///send email
                    ///
                    MailMessage m = new MailMessage();
                    m.AlternateViews.Add(avHtml);
                    EmailService emailService = new EmailService();
                    emailService.SendMail("frederik.vandekerkhove@gmail.com", "Your invoice of the Lekkerbek", testMail, m);
                }
                
                
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
           
            Thread.Sleep(5000);
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
        public async Task<IActionResult> EditCustomer(int id, [Bind("CustomerId,FName,LName,Email,PhoneNumber,FirmName,ContactPerson,StreetName,City,PostalCode,Btw,BtwNumber,Birthday,PreferredDishId")] Customer customer)
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


