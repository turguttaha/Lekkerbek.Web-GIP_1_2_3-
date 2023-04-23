﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lekkerbek.Web.Controllers
{
    [Authorize(Roles= "Customer")]
    public class OrderModuleController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly IMenuItemService _menuItemService;

        public OrderModuleController(ICustomerService customerService, IOrderService orderService, IMenuItemService menuItemService, UserManager<IdentityUser> userManager)
        {
            _customerService = customerService;
            _orderService = orderService;
            _menuItemService = menuItemService;
            _userManager = userManager;
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
               
                TempData["SelectDate"] = timeSlotItem.StartTimeSlot.ToString("yyyy-MM-dd");
                TempData["time"] = timeSlotItem.StartTimeSlot.ToString("H:mm");

                ViewBag.TimeSlotsSelectList = _orderService.GetTimeDropDownList();

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

            //if (ModelState.IsValid)
            //{
            try
            {

                TimeSlot timeSlot = new TimeSlot();
                string x = collection["TimeSlotsSelectList"];
                String selectedDate = collection["TimeSlotID"] + " " + x;
                DateTime timeSlotDateAndTime = Convert.ToDateTime(selectedDate);
                timeSlot.StartTimeSlot = timeSlotDateAndTime;

                _orderService.UpdateOrder(timeSlot, order);

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
            if (orderLine == null)
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

                TempData["TimesPast"] = "Order has less than 2 hours to prepare so it cannot be cancelled.";
                ViewBag.listOfTheOrder = _orderService.FilterOrderLines(id);

                return View(order);
            }
            return NotFound();
        }



        //// GET: Customers/Edit/5

        //public async Task<IActionResult> EditCustomer(int? id)
        //{
        //    if (id == null || _customerService.Read() == null)
        //    {
        //        return NotFound();
        //    }

        //    var customer = _customerService.GetSpecificCustomer(id);

        //    if (customer == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["PreferredDishId"] = _customerService.GetPreferredDishes(customer);
        //    return View(customer);
        //}

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> EditCustomer(int id, [Bind("CustomerId,FName,LName,Email,PhoneNumber,Address,Birthday,PreferredDishId")] Customer customer)
        //{
        //    if (id != customer.CustomerId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {

        //        try
        //        {
        //            _customerService.Update(customer);
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!_customerService.CustomerExists(customer.CustomerId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));


        //    }
        //    ViewData["PreferredDishId"] = _customerService.GetPreferredDishes(customer);
        //    return View(customer);
        //}

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


        public async Task<JsonResult> TemporaryCart()
        {
            var list = Order.TemproraryCart;
            return Json(new { temporaryCart = list });
        }

        public IActionResult CompleteOrder()
        {

            //here we can add temporary Cart list to send it to view

             ViewBag.TimeSlotsSelectList = _orderService.GetTimeDropDownList();
            //var customer = _customerService.GetSpecificCustomer(id);
            //TempData["SelectedCustomerId"] = customer.CustomerId;
            return View();
        }

        //public IActionResult CompleteOrder(int? id)
        //{
        //    // here we can add temporary Cart list to send it to view

        //    ViewBag.TimeSlotsSelectList = _orderService.GetTimeDropDownList();
        //    //var customer = _customerService.GetSpecificCustomer(id);
        //    //TempData["SelectedCustomerId"] = customer.CustomerId;
        //    return View();
        //}
        //public async Task<IActionResult> CompleteOrder(string date, string time)
        //{
        //    //it might be IFormCollection???
        //    string x = time;
        //    String selectedDate = date + " " + x;
        //    DateTime timeSlotDateAndTime = Convert.ToDateTime(selectedDate);
        //    //TimeSlot Object aanmaken

        //    TimeSlot timeSlot = new TimeSlot();
        //    timeSlot.StartTimeSlot = (DateTime)TempData["SelectedDateTime"]; // add from view via ajax
        //    //timeSlot.ChefId =(int)TempData["SelectedChef"];

        //    Order order = new Order();
        //    order.CustomerId = (int)TempData["SelectedCustomerId"];

        //    _orderService.CreateOrder(timeSlot, order);
        //    return RedirectToAction(nameof(Index));
        //}

    }
}
