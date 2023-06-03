using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lekkerbek.Web.Migrations;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Lekkerbek.Web.Controllers
{
    [Authorize(Roles= "Customer")]
    public class OrderModuleController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly OrderLineService _orderLineService;
        private readonly IMenuItemService _menuItemService;


        public OrderModuleController(ICustomerService customerService, IOrderService orderService, IMenuItemService menuItemService, UserManager<IdentityUser> userManager, OrderLineService orderLineService)
        {
            _customerService = customerService;
            _orderService = orderService;
            _menuItemService = menuItemService;
            _userManager = userManager;
            _orderLineService = orderLineService;
        }
        private async Task<Customer> GetCustomerAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = _customerService.Read().Where(c => c.IdentityUser == user).FirstOrDefault();
            return customer;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> DetailTemplate_HierarchyBinding_OrderAsync([DataSourceRequest] DataSourceRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = _customerService.Read().Where(c=>c.IdentityUser == user).FirstOrDefault();
            return Json(_orderService.FilterOrdersForCustomer(customer.CustomerId).ToDataSourceResult(request));
        }

        public ActionResult DetailTemplate_HierarchyBinding_Orderline(int orderID, [DataSourceRequest] DataSourceRequest request)
        {
            var a = _orderService.GetOrderLines();

            return Json(_orderService.GetOrderLines()
                .Where(orderline => orderline.OrderID == orderID)
                .ToDataSourceResult(request));
        }

        // GET: Orders/EditOrder/5
        public async Task<IActionResult> EditOrder(int? id)
        {

            if (id == null || _orderService.Read() == null)
            {
                return NotFound();
            }

            var order = _orderService.GetSpecificOrder(id);
            Customer customer = await GetCustomerAsync();

            if (order == null||customer.CustomerId!=order.CustomerId)
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
               
                TempData["SelectDate"] = timeSlotItem.StartTimeSlot.ToString("yyyy-MM-dd");
                TempData["time"] = timeSlotItem.StartTimeSlot.ToString("H:mm");

                ViewBag.TimeSlotsSelectList = _orderService.GetTimeDropDownList(timeSlotItem.StartTimeSlot);

                ViewBag.listOfTheOrder = _orderService.FilterOrderLines(id);

                return View(order);
            }
            TempData["TimesPast"] = "Order has less than 1 hours to prepare so no changes can be made to the order.";
            return RedirectToAction("Index");
        }

        // POST: Orders/EditOrder/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrder(int id, [Bind("OrderID,Finished,CustomerId")] Order order, IFormCollection collection)
        {
            if (id != order.OrderID)
            {
                return NotFound();
            }

            Customer customer = await GetCustomerAsync();
            if(customer.CustomerId !=order.CustomerId)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
            try
            {
                var orderForT = _orderService.GetSpecificOrder(id);

                var timeSlotItem = orderForT.TimeSlot;

                orderForT.Finished=order.Finished;
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


            return RedirectToAction("index");
            //}
            ViewData["CustomerID"] = _orderService.CustomerSelectList(order.CustomerId);
            // ViewData["TimeSlotID"] = new SelectList(_context.TimeSlots, "Id", "Id", order.TimeSlotID);
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
            Customer customer = await GetCustomerAsync();
            if (orderLine == null || orderLine.Order.CustomerId!=customer.CustomerId)
            {
                return NotFound();
            }
            ViewData["DishID"] = _orderService.MenuItemSelectList();
            //ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", orderLine.OrderID);
            return View(orderLine);
        }

        // POST: OrderLines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrderLine(int id, [Bind("OrderLineID,ExtraDetails,DishAmount,OrderID,MenuItemId,Order")] OrderLine orderLine)
        {
            if (id != orderLine.OrderLineID)
            {
                return NotFound();
            }
            Customer customer = await GetCustomerAsync();
            var customersOrders = _orderService.Read().Where(o => o.CustomerId == customer.CustomerId);
            bool orderlineCheck = true;
            foreach(Order order in customersOrders) 
            {           
                    if(order.OrderID!=orderLine.OrderID)
                    orderlineCheck = false;
            }
            if (!orderlineCheck) { return NotFound(); }
            // if (ModelState.IsValid)
            //{
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
            //}
            //ViewData["DishID"] = new SelectList(_context.MenuItems, "MenuItemId", "MenuItemId", orderLine.MenuItemId);
            //ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", orderLine.OrderID);
            return View(orderLine);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _orderService.Read() == null)
            {
                return NotFound();
            }

            var order = _orderService.GetSpecificOrder(id);
            Customer customer = await GetCustomerAsync();
            if (order == null||customer.CustomerId!=order.CustomerId)
            {
                return NotFound();
            }

            ViewBag.listOfTheOrder = _orderService.FilterOrderLines(id);


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
            Customer customer = await GetCustomerAsync();
            if(order.CustomerId!=customer.CustomerId)
            {
                return NotFound();
            }


            if (order != null)
            {

                if (_orderService.DeleteOrder(order))
                {
                    return RedirectToAction(nameof(Index));
                }

                TempData["TimesPast"] = "Order has less than 2 hours to prepare so it cannot be cancelled.";
                ViewBag.listOfTheOrder = _orderService.FilterOrderLines(id);

                return View(order);
            }
            return NotFound();
        }

        //// GET: Customers/Edit/5
        public async Task<IActionResult> EditCustomer()
        {
           
            if ( _customerService.Read() == null)
            {
                return NotFound();
            }

            Customer customer = await GetCustomerAsync();

            if (customer == null)
            {
                return NotFound();
            }
            ViewData["PreferredDishId"] = _customerService.GetPreferredDishes(customer);
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        //For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCustomerFinal([Bind("CustomerId,FName,LName,Email,PhoneNumber,FirmName,ContactPerson,StreetName,City,PostalCode,Btw,BtwNumber,Birthday,PreferredDishId")] Customer customer2)
        {
            Customer customer1 = await GetCustomerAsync();
            if (customer1.CustomerId != customer2.CustomerId)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{

                try
                {
                    _customerService.Update(customer2);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_customerService.CustomerExists(customer2.CustomerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));


           // }
            ViewData["PreferredDishId"] = _customerService.GetPreferredDishes(customer2);
            return RedirectToAction("EditCustomer",customer2);
        }

        public IActionResult MenuItemList()
        {

            return View();
        }
        public IActionResult ReadMenuItems([DataSourceRequest] DataSourceRequest request)
        {
            var menuItems = _menuItemService.Read();
            return Json(menuItems.ToDataSourceResult(request));
        }

        // POST: OrderLines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        public async Task<JsonResult> AddOrderLine(string menuItemId, string menuItemAmount, string extraDetails)
        {
            OrderLine orderLine = new OrderLine();
            orderLine.MenuItemId = int.Parse(menuItemId);
            orderLine.DishAmount = int.Parse(menuItemAmount);
            orderLine.ExtraDetails = extraDetails;
            
            orderLine.MenuItem = _orderService.GetSpecificMenuItem(orderLine.MenuItemId);
            Order.TemproraryCart.Add(orderLine);
            
            return Json(new { status = "Your Menu Item is Added!" });

        }

        public async Task<JsonResult> RemoveOrderLine(string id)
        {

          List<OrderLine> list =  Order.TemproraryCart;
            bool test = false;
            OrderLine newItem = null;
            foreach (var item in list) 
            {
                string itemId = string.Empty;
                itemId = item.MenuItemId.ToString()+item.DishAmount+item.ExtraDetails;
                if (itemId == id) 
                {
                    test = true;
                    newItem = item;
                }
                    

            }
            if (test) 
            { 
                Order.TemproraryCart.Remove(newItem);
            }
            return Json(new { status = "Het product is verwijderd" });
        }


        public async Task<JsonResult> TemporaryCart()
        {
            var list = Order.TemproraryCart;
            return Json(new { temporaryCart = list });
        }

        public async Task<IActionResult> CompleteOrder()
        {
            if(Order.TemproraryCart.Count == 0)
            {
                TempData["temporaryCartError"] = "You have to add at least 1 Menu Item before continue!";
                return RedirectToAction(nameof(MenuItemList));
            }
            DateTime now = DateTime.Now;
            ViewBag.TimeSlotsSelectList = _orderService.GetTimeDropDownList(now);
            Customer customer = await GetCustomerAsync();
            TempData["Street"] = customer.StreetName;
            TempData["City"] = customer.City;
            TempData["PostalCode"] = customer.PostalCode;

            return View();
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
        public async Task<JsonResult> UpdateAdress(string street, string city , string postalCode)
        {
            try
            {
                Customer customer = await GetCustomerAsync();
                customer.StreetName = street;
                customer.City = city;
                customer.PostalCode = postalCode;

                _customerService.Update(customer);
                return Json(new { status = "Your adress is succesfully updated" });
            }
            catch {
                return Json(new { status = "Error" });
            }

           
        }

        public async Task<IActionResult> CompleteOrderFinal(IFormCollection collection)
        {
            Customer customer = await GetCustomerAsync();
            if (customer.StreetName == null || customer.City == null || customer.PostalCode == null)
            {
                TempData["CreateError"] = "Please update your adress before complete the order ";
                return RedirectToAction(nameof(CompleteOrder));
            }
            //it might be IFormCollection???

            string x = collection["TimeSlotsSelectList"];
            String selectedDate = collection["StartTimeSlot"] + " " + x;
            DateTime timeSlotDateAndTime = Convert.ToDateTime(selectedDate);
            //TimeSlot Object aanmaken

            TimeSlot timeSlot = new TimeSlot();
            timeSlot.StartTimeSlot = timeSlotDateAndTime; 

            Order order = new Order();
            order.CustomerId = customer.CustomerId;

            _orderService.CreateOrder(timeSlot, order);
            return RedirectToAction(nameof(Index));
        }

    }
}
